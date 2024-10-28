using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace na6dlFrontEnd
{

	[StructLayout(LayoutKind.Explicit)]
	public struct COPYDATASTRUCT32
	{
		[FieldOffset(0)] public UInt32 dwData;
		[FieldOffset(4)] public UInt32 cbData;
		[FieldOffset(8)] public IntPtr lpData;
	}


	public partial class Form1 : Form
	{
		[DllImport("KERNEL32.DLL")]
		public static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);
		[DllImport("KERNEL32.DLL")]
		public static extern uint GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);
		[DllImport("kernel32.dll")]
		private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpstring, string lpFileName);


		// WM_COPYDATAのメッセージID
		private const int WM_COPYDATA = 0x004A;
		private const int WM_USER = 0x400;
		private const int WM_DLINFO = WM_USER + 30;

		private string exeDirName = "";
		private string iniPath = "";
		private UInt32 TotalChap = 0;
		private int ChapCount = 0;
		private DateTime latestDLDateTime;
		private DateTime nextEveryDay;
		private DateTime nextEveryWeek;
		private DateTime nextEveryMon;
		private string sStatus = "";
		private int novelTotal = 0;
		private int novelCount = 0;
		private bool procComplete = false;
		private bool busy = false;
		private IntPtr hWnd = IntPtr.Zero;
		private string dlAfterOpeNovel1st = "";
		private string dlAfterOpeNovel1Later = "";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Form1()
		{
			InitializeComponent();
		}

		/// <summary>
		/// フォームロード時処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_Load(object sender, EventArgs e)
		{
			Assembly myAssembly = Assembly.GetEntryAssembly();
			string path = myAssembly.Location;
			exeDirName = Path.GetDirectoryName(path);
			iniPath = exeDirName + @"\" + Path.GetFileNameWithoutExtension(path) + ".ini";

			StringBuilder wk = new StringBuilder(512);
			GetPrivateProfileString("NextDownLoad", "毎日", "2000/01/01 00:00:00", wk, 512, iniPath);
			nextEveryDay = DateTime.Parse(wk.ToString());
			GetPrivateProfileString("NextDownLoad", "毎週", "2000/01/01 00:00:00", wk, 512, iniPath);
			nextEveryWeek = DateTime.Parse(wk.ToString());
			GetPrivateProfileString("NextDownLoad", "毎月", "2000/01/01 00:00:00", wk, 512, iniPath);
			nextEveryMon = DateTime.Parse(wk.ToString());

			GetPrivateProfileString("DownloadAfterOperation", "Novel1st", "", wk, 512, iniPath);
			dlAfterOpeNovel1st = wk.ToString();
			GetPrivateProfileString("DownloadAfterOperation", "Novel1Later", "", wk, 512, iniPath);
			dlAfterOpeNovel1Later = wk.ToString();

			hWnd = this.Handle;

			lblNovelTitle.Text =
			lblStatusApp.Text =
			lblStatusNovel.Text = "";
			lblProgress.Text = "";

			int num = (int)GetPrivateProfileInt("ListItems", "Count", -1, iniPath);
			for (int i = 0; i < num; i++)
			{
				GetPrivateProfileString("ListItems", $"Item{i + 1}", "", wk, 256, iniPath);
				string item = wk.ToString();
				if (item != "") lbUrlList.Items.Add(item);
			}

			lbUrlList_SelectedIndexChanged(null, null);

			//テスト用
			//終了後外部保コマンド
			if (false)
			{
				if (dlAfterOpeNovel1st != "")
				{
					exeAfterOperation(dlAfterOpeNovel1st, @"F:\Books\小説家になろうtest\宇宙の果てで謎の種を拾いました\宇宙の果てで謎の種を拾いました.txt");
				}

			}
			//挿絵リンク抽出
			if (false)
			{
				string[] buff = new string[]
				{
					"［＃水平線］［＃ここから罫囲み］",
					"読んでくださりありがとうございます。",
					"ゴブリンの邪悪さのお口直しにどうぞ。",
					"",
					"",
					"［＃リンクの図（//41743.mitemin.net/userpageimage/viewimagebig/icode/i813181/）入る］",
					"",
					"［＃ここで罫囲み終わり］［＃水平線］",
					"［＃改ページ］",
					"［＃中見出し］１－３０　２日目の朝［＃中見出し終わり］",
					"",
					"［＃水平線］［＃ここから罫囲み］",
					"本日もよろしくお願いします。",
					"",
					"［＃ここで罫囲み終わり］［＃水平線］",
					"",
					"",
					"　常に総動員するのは不味いので、基本的に半分が召喚され、半分が外部にいるという体制を取ることになった。",
					"　召喚されていない人は、パソコンで情報を集めて助言をしたり、連絡係をしたりと活動する。もちろん休憩に入ってもいい。深夜になったら全員が寝るというのは絶対に避けたいので、休憩も仕事なのだ。",
					"",
					"　というわけで、理想の猫の国を作るためにニート共は人形に宿って異世界へ働きに出た。",
					"",
					"「にゃふぅ！」",
					"",
					"",
					"［＃リンクの図（//41743.mitemin.net/userpageimage/viewimagebig/icode/i817371/）入る］",
					"",
					"",
					"　お人形さんにうじゃうじゃと囲まれてふんすぅとするのはミニャ。",
					"　今日からミニャもお外で元気に活動だ。",

				};

				string[] strs = getFigLink(buff);
			}

			if (nextEveryDay > DateTime.Now)
			{
				btnDownload.Enabled = false;
				MessageBox.Show(this, $"最後にダウンロードしてから１２時間経過していません\nあと[{(nextEveryDay - DateTime.Now):hh\\:mm\\:ss}]");
				lblStatusApp.Text = $"実行可能迄後[{(nextEveryDay - DateTime.Now):hh\\:mm\\:ss}]";
			}
		}

		/// <summary>
		/// フォーム閉じる処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if ((e.CloseReason == CloseReason.UserClosing)
			&& (MessageBox.Show(this, "終了しますか", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes))
			{
				e.Cancel = true;
				return;
			}
			if (busy && (MessageBox.Show(this, "ダウンロード中ですが本当に終了しますか", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes))
			{
				e.Cancel = true;
				return;
			}
		}

		/// <summary>
		/// リストにリストファイル追加ボタン押下処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnAddList_Click(object sender, EventArgs e)
		{
			lblStatusApp.Text =
			lblStatusNovel.Text = "";
			sStatus = "";

			string path = "";
			OpenFileDialog ofd = new OpenFileDialog();
			if (lbUrlList.Items.Count > 0)
			{
				path = (string)lbUrlList.Items[lbUrlList.Items.Count-1];
				ofd.InitialDirectory = path.Substring(0, path.LastIndexOf('\\'));
			}
			ofd.FileName = "URL.txt";
			ofd.Filter = "テキストファイル(*.txt)|*.txt|すべてのファイル(*.*)|*.*";
			ofd.FilterIndex = 1;
			ofd.Title = "追加するファイルを選択してください";
			ofd.RestoreDirectory = false;
			ofd.CheckFileExists = true;
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				lbUrlList.Items.Add(ofd.FileName);
				writeIniListItem();
			}
			lbUrlList_SelectedIndexChanged(null, null);
		}

		/// <summary>
		/// URLをリストに追加ボタン押下処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnUrlAdd_Click(object sender, EventArgs e)
		{
			frmInputURL frm = new frmInputURL();
			frm.ShowDialog();
			string str = frm.URLAddress.Trim();
			lbUrlList.Items.Add(str);
			writeIniListItem();
			lbUrlList_SelectedIndexChanged(null, null);
		}

		private void btnItemUp_Click(object sender, EventArgs e)
		{
			int sidx = lbUrlList.SelectedIndex;
			string strsave = (string)lbUrlList.Items[sidx - 1];
			lbUrlList.Items.RemoveAt(sidx - 1);
			lbUrlList.Items.Insert(sidx, strsave);
			btnItemUp.Enabled = ((sidx - 1) > 0);
			btnItemDn.Enabled = true;
		}

		private void btnItemDn_Click(object sender, EventArgs e)
		{
			int sidx = lbUrlList.SelectedIndex;
			int maxidx = lbUrlList.Items.Count - 1;
			string strsave = (string)lbUrlList.Items[sidx + 1];
			lbUrlList.Items.RemoveAt(sidx + 1);
			lbUrlList.Items.Insert(sidx, strsave);
			btnItemUp.Enabled = true;
			btnItemDn.Enabled = ((sidx + 1) < maxidx);
		}

		private void lbUrlList_SelectedIndexChanged(object sender, EventArgs e)
		{
			int sidx = lbUrlList.SelectedIndex;
			int maxidx = lbUrlList.Items.Count - 1;
			btnItemUp.Enabled = (sidx > 0);
			btnItemDn.Enabled = (sidx < maxidx) && (sidx >= 0);
		}

		/// <summary>
		/// リストから削除ボタン押下処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnDelList_Click(object sender, EventArgs e)
		{
			lblStatusApp.Text =
			lblStatusNovel.Text = "";
			sStatus = "";
			if(lbUrlList.SelectedIndex >= lbUrlList.Items.Count)
			{
				MessageBox.Show(this, "選択されていません", "Warning");
				return;
			}
			if (MessageBox.Show(this, $"{lbUrlList.Items[lbUrlList.SelectedIndex]} をリストから削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				lbUrlList.Items.RemoveAt(lbUrlList.SelectedIndex);
				writeIniListItem();
			}
			lbUrlList_SelectedIndexChanged(null, null);
		}

		/// <summary>
		/// リストに表示している項目をINIファイルに書き込み
		/// </summary>
		private void writeIniListItem()
		{
			int num = lbUrlList.Items.Count;
			WritePrivateProfileString("ListItems", "Count", num.ToString(), iniPath);
			for (int i = 0; i < num; i++)
			{
				WritePrivateProfileString("ListItems", $"Item{i + 1}", (string)lbUrlList.Items[i], iniPath);
			}
		}

		/// <summary>
		/// C#による仮想WndProc()、WM_COPYDATAとWM_USER+30(WM_DLINFO)をハンドルする
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			switch (m.Msg)
			{
				case WM_COPYDATA:
					{
						COPYDATASTRUCT32 cds = (COPYDATASTRUCT32)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT32));
						TotalChap = cds.dwData;
						lblNovelTitle.Text = Marshal.PtrToStringAuto(cds.lpData);
					}
					break;
				case WM_DLINFO:
					{
						ChapCount = (int)m.WParam;
						lblProgress.Text = $"{(int)(ChapCount * 100 / TotalChap)}".PadLeft(3) + $@"% ({ChapCount}/{TotalChap})";
						lblProgress.BackColor = ((ChapCount & 1) == 0) ? SystemColors.Control : Color.AliceBlue;
					}
					break;
			}
		}

		/// <summary>
		/// ダウンロードボタン押下処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void  btnDownload_Click(object sender, EventArgs e)
		{
			pnlBtn.Enabled = busy;
			await DownloadAllAsync();
			pnlBtn.Enabled = !busy;
		}

		/// <summary>
		/// ダウンロードの非同期用
		/// </summary>
		/// <returns></returns>
		private async Task DownloadAllAsync()
		{
			timer1.Enabled = true;
			Task task = Task.Run(() => { DownloadAll(); });
			await task;
			timer1.Enabled = false;
		}

		private delegate void dglbUrlListSelectedIndex(int idx);
		/// <summary>
		/// lbUrlListのSelectedIndexを操作する
		/// 非同期スレッド／メインスレッド共用
		/// </summary>
		/// <param name="idx"></param>
		private void lbUrlListSelectedIndex(int idx)
		{
			if (lbUrlList.InvokeRequired)
			{
				this.Invoke(new dglbUrlListSelectedIndex(lbUrlListSelectedIndex), idx);
				return;
			}
			lbUrlList.SelectedIndex = idx;
		}

		/// <summary>
		/// LabelのTextに文字列をセットする
		/// 非同期スレッド／メインスレッド共用
		/// </summary>
		/// <param name="label">対象ラベル</param>
		/// <param name="str">表示文字列</param>
		private delegate void dglblText(Label label, string str);
		private void lblText(Label label, string str)
		{
			if (label.InvokeRequired)
			{
				this.Invoke(new dglblText(lblText),label, str);
				return;
			}
			label.Text = str;
		}

		private delegate void dglblBkCol(Label label, Color col);
		private void lblBkCol(Label label, Color col)
		{
			if (label.InvokeRequired)
			{
				this.Invoke(new dglblBkCol(lblBkCol), col);
				return;
			}
			label.BackColor = col;
		}

		/// <summary>
		/// 表内の全てのリストファイルを利用してダウンロード
		/// </summary>
		private void DownloadAll()
		{
			lblText(lblStatusApp, "ダウンロード中");
			lblText(lblStatusNovel,"");
			sStatus = "";
			lblText(lblNovelTitle, "");

			busy = true;
			string section = CheckDateTime();

			if (section != "")
			{
				for (int idx =0; idx < lbUrlList.Items.Count; idx++)
				{
					lbUrlListSelectedIndex(idx);

					string listPath = (string)lbUrlList.Items[idx];
					if(listPath.IndexOf(@"https://") == 0)
					{
						DownloadNovel(listPath);
						continue;
					}

					string[] linebuf = File.ReadAllLines(listPath);
					if (listNovelDL(linebuf, section, true) == false)
					{
						break;
					}
					novelTotal = novelCount;
					lblText(lblListProgress, "(" + "   0" + " / " + novelTotal.ToString().PadLeft(4) + ")");
					if (listNovelDL(linebuf, section) == false)
					{
						break;
					}
				}
				//lbUrlList.SelectedIndex = -1;
				lbUrlListSelectedIndex(-1);
				lblText(lblStatusApp, "ダウンロード終了");
				WriteNextDateTime(section);
			}
			else
			{
				MessageBox.Show(this, $"最後にダウンロードしてから１２時間経過していません\nあと[{nextEveryDay - DateTime.Now}]");
			}
			busy = false;
		}

		/// <summary>
		/// リストファイルの小説をダウンロードする
		/// </summary>
		/// <param name="linebuf">リストファイルの内容</param>
		/// <param name="section">読み込みセクション</param>
		/// <param name="countOnly">小説のカウントのみする時true</param>
		/// <returns>成功時true</returns>
		private bool listNovelDL(string[] linebuf, string section, bool countOnly = false)
		{
			bool result = false;

			string DlBaseDir = "";
			string novelBaseDir = "";
			int seqno = 0;
			bool abortFlag = false;
			string filepath = "";
			//string infopath = "";
			string novelDir = "";

			novelCount = 0;

			try
			{
				foreach (string linedata in linebuf)
				{
					string ldata = linedata.Trim();
					//文字が無いか、コメントなら読み飛ばし
					if ((ldata == "")
					|| (ldata[0] == '\''))
					{
						continue;
					}

					switch (seqno)
					{
						//小説を保存するベースフォルダと、ダウンロードした各章テキストを保存するベースフォルダを取得する
						case 0:
							if (DlBaseDir == "") DlBaseDir = getSetting(ldata, "DL Folder:");
							if (novelBaseDir == "") novelBaseDir = getSetting(ldata, "Novel Folder:");
							if ((DlBaseDir != "") && (novelBaseDir != ""))
							{
								seqno++;
							}
							continue;
						//小説読み込みの最後のセクションを探す
						case 1:
							if ((ldata[0] == '#')
							&& (ldata.IndexOf(section) == 1))
							{
								seqno++;
							}
							break;
						//最後のセクションの次のセクションを示す"#"を見つけたら終了
						case 2:
							abortFlag = (ldata[0] == '#');
							break;
						default:
							seqno = 0;
							break;
					}
					if (abortFlag) break;

					if ((ldata.Length > 8)
					&& (ldata.Substring(0, 8) == "https://")
					&& (string.IsNullOrEmpty(filepath) == false))
					{
						novelCount++;
						if (countOnly == false)
						{
							string fname = Path.GetFileNameWithoutExtension(filepath).TrimEnd(new char[] {' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'});
							string fext = $"{Path.GetExtension(filepath)}";
							//小説を格納するフォルダがなければ作成
							novelDir = novelBaseDir + @"\"+ Path.GetDirectoryName(filepath);
							if (Directory.Exists(novelDir) == false)
							{
								Directory.CreateDirectory(novelDir);
							}
							//小説一つをダウンロード
							DownloadNovel(ldata, novelDir, fname);
							lblText(lblListProgress, "(" + novelCount.ToString().PadLeft(4) + " / " + novelTotal.ToString().PadLeft(4) + ")");
						}
						filepath = "";
					}
					else
					{
						filepath = ldata;
					}
				}
				result = true;
			}
			catch (Exception ex)
			{
				sStatus = $"ダウンロードエラー：{ex.Message}";
				lblText(lblStatusApp, sStatus);
			}
			return result;
		}

		/// <summary>
		/// 小説をダウンロード
		/// </summary>
		/// <param name="UrlAdr">URL</param>
		/// <param name="DirName">格納するディレクトリ名</param>
		/// <param name="fname">ファイル名</param>
		/// <param name="fext">ファイル拡張子</param>
		private void DownloadNovel(string UrlAdr,string DirName="", string fname="", string fext = ".txt")
		{
			//小説情報ファイルを読み込む
			string dirname = ((string.IsNullOrEmpty(DirName) ? exeDirName : DirName));
			string infopath = $@"{dirname}\{fname}Info.txt";
			string filepath = (string.IsNullOrEmpty(fname) ? "" : $@"{dirname}\{fname}{fext}");
			int latestChap = 0;
			DateTime latestDate = DateTime.Parse("2000/1/1");
			List<string> infoLines = null;
			try
			{
				if (File.Exists(infopath))
				{
					infoLines = File.ReadAllLines(infopath).ToList<string>();
					if (infoLines.Count > 0)
					{
						foreach (string ldata in infoLines)
						{
							string[] infos = ldata.Split(',');
							if (infos.Length >= 2)
							{
								if ((DateTime.TryParse(infos[0], out latestDate))
								&& (int.TryParse(infos[1], out latestChap)))
								{
									infoLines.Remove(ldata);
									break;
								}
							}
						}
					}
				}
				lblText(lblStatusNovel, "ダウンロード中");
				lblText(lblNovelTitle, "");
				lblText(lblProgress, "");

				int startPage = 0;
				string tmppath = $@"{exeDirName}\tmp.txt";
				Process proc = null;
				//途中までダウンロードできていれば続きをダウンロードし、マージする
				if (latestChap > 0)
				{
					startPage = latestChap + 1;
					if (File.Exists(tmppath)) File.Delete(tmppath);
					//小説を続きの章から最新章までダウンロード
					proc = na6dlDownload(hWnd, UrlAdr, tmppath, startPage);
					proc.WaitForExit();

					//小説ファイルをマージする
					if (File.Exists(tmppath))
					{
						//リンクの図を検索してリンクのみの文字列配列を取得し、情報ファイルの内容に追加・重複削除する
						getFigLink(File.ReadAllLines(tmppath), ref infoLines);
						if (dlAfterOpeNovel1Later != "")
						{
							exeAfterOperation(dlAfterOpeNovel1Later, tmppath);
						}
						//List<string> buff = File.ReadAllLines(tmppath).ToList<string>();
						string[] buff = File.ReadAllLines(tmppath);
						using (FileStream fs = File.Open(filepath, FileMode.Open))
						using (StreamWriter sw = new StreamWriter(fs, new UTF8Encoding()))
						{
							//int len = buff.Count;
							int len = buff.Length;
							fs.Seek(0, SeekOrigin.End);
							int idx = 0;
							for (; idx < len; idx++)
							{
								if (buff[idx].IndexOf("［＃中見出し］") >= 0) break;
							}
							for (; idx < len; idx++)
							{
								sw.WriteLine(buff[idx]);
							}
						}
					}
				}
				else
				{
					//小説を最初から最新章までダウンロード
					proc = na6dlDownload(hWnd, UrlAdr, filepath);
					proc.WaitForExit();
					infoLines = new List<string>();
					//リンクの図を検索してリンクのみの文字列配列を取得し、情報ファイルの内容に追加・重複削除する
					getFigLink(File.ReadAllLines(filepath), ref infoLines);

					if(File.Exists(filepath))
					{
						if (dlAfterOpeNovel1st != "")
						{
							exeAfterOperation(dlAfterOpeNovel1st, filepath);
						}
					}
					else
					{
						MessageBox.Show(this,"小説がダウンロード出来ません","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
					}
				}
				//小説情報ファイルを書き込む
				using (StreamWriter sw = new StreamWriter(File.Create(infopath), new UTF8Encoding()))
				{
					sw.WriteLine($"{DateTime.Now}, {ChapCount + latestChap}");
					if (infoLines.Count > 0)
					{
						foreach (string str in infoLines)
						{
							sw.WriteLine(str);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show( ex.Message);
			}
		}

		/// <summary>
		/// 外部プログラムの実行
		/// </summary>
		/// <param name="cmdLine">コマンドライン、%Fが有ればファイル名に置き換え　例"xxx.exe" "-r" "%F"」</param>
		/// <param name="filepath"></param>
		private void exeAfterOperation(string cmdline, string filepath)
		{
			int pos = 0;
			string arg = "";
			string filename = "";
			cmdline = cmdline.Trim();
			if (cmdline[0] == '"')
			{
				pos = cmdline.IndexOf('\"', 1);
				filename = cmdline.Substring(1, pos-1);//.Trim(new char[] { '"', ' ' });
			}
			else
			{
				pos = cmdline.IndexOf(' ', 1);
				filename = cmdline.Substring(0, pos);//.Trim(new char[] { '"', ' ' });
			}
			//引数を確定し、特定文字を置き換える
			arg = cmdline.Substring(pos +1);//.Trim(new char[] { '"', ' ' });
			//%Fをダウンロードファイルパスへ置換
			arg = arg.Replace("%F", filepath).Replace("%f", filepath);
			//%AをEXEディレクトリパスへ置換
			arg = arg.Replace("%A", exeDirName).Replace("%a", exeDirName);
			//プロセスを作成し、実行する
			ProcessStartInfo pInfo = new ProcessStartInfo();
			pInfo.FileName = filename;
			pInfo.Arguments = arg;
			//意味がない
			//pInfo.CreateNoWindow = true; // コンソール・ウィンドウを開かない
			//pInfo.UseShellExecute = false; // シェル機能を使用しない
			Process p = Process.Start(pInfo);
			//終了待ち
			p.WaitForExit();
			//MessageBox.Show("終了しました");
		}

		/// <summary>
		/// リストファイル内の設定項目を取得
		/// </summary>
		/// <param name="lineData"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		private string getSetting(string lineData, string key)
		{
			int idx = lineData.IndexOf(key);
			if (idx >= 0)
			{
				string res = lineData.Substring(idx + key.Length);
				return res;
			}
			return "";
		}

		/// <summary>
		/// 前回ダウンロードからの時間でダウンロードするセクションを決定する
		/// </summary>
		/// <returns></returns>
		private string CheckDateTime()
		{
			String result = "";

			//DateTime tmpdt;
			latestDLDateTime = DateTime.Now;

			if (latestDLDateTime >= nextEveryMon)
			{
				int monInterval = DateTime.DaysInMonth(latestDLDateTime.Year, latestDLDateTime.Month) - latestDLDateTime.Day + 1;
				nextEveryMon = DateTime.Parse(latestDLDateTime.AddDays(monInterval).ToString("yyyy/MM/dd"));
				nextEveryWeek = getEveryWeekNext(latestDLDateTime);
				nextEveryDay = latestDLDateTime.AddHours(12);
				result = "毎月";
			}
			else if (latestDLDateTime >= nextEveryWeek)
			{
				nextEveryWeek = getEveryWeekNext(latestDLDateTime);
				nextEveryDay = latestDLDateTime.AddHours(12);
				result = "毎週";
			}
			else if (latestDLDateTime >= nextEveryDay)
			{
				nextEveryDay = latestDLDateTime.AddHours(12);
				result = "毎日";
			}
			return result;
		}

		/// <summary>
		/// 次回ダウンロード時間を書き込む
		/// </summary>
		/// <returns></returns>
		private void WriteNextDateTime(string section)
		{
			switch(section)
			{
				case "毎月":
					WritePrivateProfileString("NextDownLoad", "毎月", nextEveryMon.ToString(), iniPath);
					WritePrivateProfileString("NextDownLoad", "毎週", nextEveryWeek.ToString(), iniPath);
					WritePrivateProfileString("NextDownLoad", "毎日", nextEveryDay.ToString(), iniPath);
					break;
				case "毎週":
					WritePrivateProfileString("NextDownLoad", "毎週", nextEveryWeek.ToString(), iniPath);
					WritePrivateProfileString("NextDownLoad", "毎日", nextEveryDay.ToString(), iniPath);
					break;
				case "毎日":
					WritePrivateProfileString("NextDownLoad", "毎日", nextEveryDay.ToString(), iniPath);
					break;
			}
			WritePrivateProfileString("NextDownLoad", "実行日時", latestDLDateTime.ToString(), iniPath);
		}

		/// <summary>
		/// 毎週の次回取得可能な最近の日時を取得する
		/// </summary>
		/// <param name="nowDateTime"></param>
		/// <returns></returns>
		private DateTime getEveryWeekNext(DateTime nowDateTime)
		{
			int weekInterval;
			StringBuilder wk = new StringBuilder(256);

			//毎週のINIで指定された曜日にダウンロードする
			GetPrivateProfileString("Setting", "曜日", "月", wk, 256, iniPath);
			switch (wk.ToString())
			{
				case "日": weekInterval = 7; break;
				default:
				case "月": weekInterval = 8; break;
				case "火": weekInterval = 9; break;
				case "水": weekInterval = 10; break;
				case "木": weekInterval = 11; break;
				case "金": weekInterval = 12; break;
				case "土": weekInterval = 13; break;
			}
			weekInterval -= (int)nowDateTime.DayOfWeek;
			if (weekInterval > 7) weekInterval -= 7;
			return DateTime.Parse(nowDateTime.AddDays(weekInterval).ToString("yyyy/MM/dd"));
		}

		/// <summary>
		/// na6dl.exeを使って小説一つをダウンロードする
		/// </summary>
		/// <param name="URL"></param>
		/// <param name="filePath"></param>
		private Process na6dlDownload(IntPtr hWnd, string URL, string filePath = null, int startChap = 0)
		{
			if ((URL.Contains("https://ncode.syosetu.com/n") == false)
			&& (URL.Contains("https://novel18.syosetu.com/n") == false))
			{
				return null;
			}

			//IntPtr hWnd = this.Handle;

			Process proc = new Process();
			proc.StartInfo.FileName = @"na6dl.exe";
			if(string.IsNullOrEmpty(filePath))
			{
				proc.StartInfo.Arguments = $" \"-h {hWnd}\" {URL}";
			}
			else if(startChap <= 0 )
			{
				proc.StartInfo.Arguments = $" \"-h {hWnd}\" \"{filePath}\" {URL}";
			}
			else
			{
				proc.StartInfo.Arguments = $" \"-h {hWnd}\" \"-s {startChap}\" \"{filePath}\" {URL}";
			}
			proc.StartInfo.CreateNoWindow = true; // コンソール・ウィンドウを開かない
			proc.StartInfo.UseShellExecute = false; // シェル機能を使用しない
			proc.SynchronizingObject = this;
			proc.Exited += new EventHandler(proc_Exited);//終了イベントを登録
			proc.EnableRaisingEvents = true;
			//起動する
			proc.Start();
			return proc;
		}

		/// <summary>
		/// "［＃リンクの図（//41743.mitemin.net/userpageimage/viewimagebig/icode/i813181/）入る］"と同様の文字列を含む行を抽出、
		/// リンクのみにして指定のリストにマージ、
		/// 重複を削除する
		/// </summary>
		/// <param name="strSrray">抽出元の文字列配列￥</param>
		/// <param name="destlist">マージする文字列のリスト</param>
		private void getFigLink(string[] strSrray, ref List<string> destlist)
		{
			//［＃リンクの図（//41743.mitemin.net/userpageimage/viewimagebig/icode/i813181/）入る］
			string[] strs = getFigLink(strSrray);
			destlist.AddRange(strs);
			destlist.Distinct();
		}

		private string[] getFigLink(string[] strSrray)
		{
			return strSrray.Where(str => str.Contains("リンクの図")).Select(str => Regex.Replace(str, @"^.*リンクの図（", "https:")).Select(str => Regex.Replace(str, @"）入る］.*", "")).ToArray();
		}

		/// <summary>
		/// コマンドプロンプトの終了イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void proc_Exited(object sender, EventArgs e)
		{
			//プロセスが終了したときに実行される
			//lblStatusNovel.Text = (sStatus == "") ? "ダウンロード終了" : sStatus;
			if (sStatus == "")
			{
				lblText(lblStatusNovel, "ダウンロード終了");
				lblText(lblProgress, $"100% ({TotalChap}/{TotalChap})");
			}
			else
			{
				lblText(lblStatusNovel, sStatus);
			}
			lblBkCol(lblProgress, SystemColors.Control);
			procComplete = true;
		}

		private string latesttime = "";
		private void timer1_Tick(object sender, EventArgs e)
		{
			string strtime = $"{DateTime.Now - latestDLDateTime:hh\\:mm\\:ss}";
			if(latesttime != strtime)
			{
				//lblTimeCount.Text = strtime;
				lblText(lblTimeCount, strtime);
				latesttime = strtime;
			}
		}

	}
}
