﻿using System;
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

			StringBuilder wk = new StringBuilder(256);
			GetPrivateProfileString("NextDownLoad", "毎日", "2000/01/01 00:00:00", wk, 256, iniPath);
			nextEveryDay = DateTime.Parse(wk.ToString());
			GetPrivateProfileString("NextDownLoad", "毎週", "2000/01/01 00:00:00", wk, 256, iniPath);
			nextEveryWeek = DateTime.Parse(wk.ToString());
			GetPrivateProfileString("NextDownLoad", "毎月", "2000/01/01 00:00:00", wk, 256, iniPath);
			nextEveryMon = DateTime.Parse(wk.ToString());

			hWnd = this.Handle;

			lblNovelTitle.Text =
			lblStatusApp.Text =
			lblStatusNovel.Text = "";
			lblProgress.Text = "0% (0/0)";

			if (nextEveryDay > DateTime.Now)
			{
				btnDownload.Enabled = false;
				MessageBox.Show(this, $"最後にダウンロードしてから１２時間経過していません\nあと[{(nextEveryDay - DateTime.Now):hh\\:mm\\:ss}]");
				lblStatusApp.Text = $"実行可能迄後[{(nextEveryDay - DateTime.Now):hh\\:mm\\:ss}]";
			}
			int num = (int)GetPrivateProfileInt("ListItems", "Count", -1, iniPath);
			for (int i = 0; i < num; i++)
			{
				GetPrivateProfileString("ListItems", $"Item{i+1}","", wk, 256, iniPath);
				string item = wk.ToString();
				if (item != "")	lbUrlList.Items.Add(item);
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
		/// リストに追加ボタン押下処理
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
			if (MessageBox.Show(this, $"{lbUrlList.Items[lbUrlList.SelectedIndex]} をリストから削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				lbUrlList.Items.RemoveAt(lbUrlList.SelectedIndex);
				writeIniListItem();
			}
		}

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
		/// ダウンロードボタン押下処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnDownload_Click(object sender, EventArgs e)
		{
			busy = true;
			btnDownload.Enabled = busy;
			DownloadAll();
			busy = false;
			btnDownload.Enabled = !busy;
		}

		/// <summary>
		/// 
		/// </summary>
		private void DownloadAll()
		{
			lblStatusApp.Text = "ダウンロード開始";
			lblStatusNovel.Text = "";
			sStatus = "";
			latestDLDateTime = DateTime.Now;

			string section = DateTimeCheck();

			if (section != "")
			{
				timer1.Enabled = true;
				WritePrivateProfileString("NextDownLoad", "実行日時", $"{latestDLDateTime:yyyy/MM/dd HH:mm:ss}", iniPath);
				for (int idx =0; idx < lbUrlList.Items.Count; idx++)
				{
					lbUrlList.SelectedIndex = idx;
					if (listNovelDL((string)lbUrlList.Items[idx], section) == false)
					{
						break;
					}
				}
				timer1.Enabled = false;
				lbUrlList.SelectedIndex = -1;
				lblStatusApp.Text = "ダウンロード終了";
			}
			else
			{
				MessageBox.Show(this, $"最後にダウンロードしてから１２時間経過していません\nあと[{nextEveryDay - DateTime.Now}]");
			}
		}

		/// <summary>
		/// リストの小説をダウンロードする
		/// </summary>
		/// <param name="listPath">リストファイルパス</param>
		/// <param name="section">読み込みセクション</param>
		/// <returns>成功時true</returns>
		private bool listNovelDL(string listPath, string section)
		{
			string[] linebuf = File.ReadAllLines(listPath);
			if (listNovelSub(linebuf, section, true))
			{
				novelTotal = novelCount;
				lblListProgress.Text = "(" + "   0" + " / " + novelTotal.ToString().PadLeft(4) + ")";
				return listNovelSub(linebuf, section);
			}
			return false;
		}

		/// <summary>
		/// リストの小説をダウンロードする、サブ関数
		/// </summary>
		/// <param name="linebuf">リストファイルの内容</param>
		/// <param name="section">読み込みセクション</param>
		/// <param name="countOnly">小説のカウントのみする時true</param>
		/// <returns>成功時true</returns>
		private bool listNovelSub(string[] linebuf, string section, bool countOnly = false)
		{
			bool result = false;

			string DlBaseDir = "";
			string novelBaseDir = "";
			int seqno = 0;
			bool abortFlag = false;
			string filepath = "";
			string infopath = "";
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
							//小説情報ファイルを読み込む
							infopath = $@"{novelDir}\{fname}Info.txt";
							filepath=  $@"{novelDir}\{fname}{fext}";
							int latestChap = 0;
							DateTime latestDate = DateTime.Parse("2000/1/1");
							if(File.Exists(infopath))
							{
								using (StreamReader sr = new StreamReader(File.Open(infopath, FileMode.Open), new UTF8Encoding()))
								{
									string[] infos = sr.ReadLine().Split(',');
									if (infos.Length >= 2)
									{
										latestDate = DateTime.Parse(infos[0]);
										latestChap = int.Parse(infos[1]);
									}
								}
							}
							//途中までダウンロードできていれば続きをダウンロードし、マージする
							lblStatusNovel.Text = "ダウンロード開始";
							int startPage = 0;
							string tmppath = $@"{exeDirName}\tmp.txt";
							Process proc = null;
							if (latestChap > 0)
							{
								startPage = latestChap + 1;
								if (File.Exists(tmppath)) File.Delete(tmppath);
								//小説を続きの章から最新章までダウンロード
								//proc = downloadOne(hWnd, ldata, tmppath, startPage);
								//proc.WaitForExit();
								downloadOneAsync2(hWnd, ldata, tmppath, startPage);

								//小説ファイルをマージする
								if (File.Exists(tmppath))
								{
									List<string> buff = File.ReadAllLines(tmppath).ToList<string>();
									using (FileStream fs = File.Open(filepath, FileMode.Open))
									using (StreamWriter sw = new StreamWriter(fs, new UTF8Encoding()))
									{
										fs.Seek(0, SeekOrigin.End);
										int idx = 0;
										for (; idx < buff.Count; idx++)
										{
											if (buff[idx].IndexOf("［＃中見出し］") >= 0) break;
										}
										for (; idx < buff.Count; idx++)
										{
											sw.WriteLine(buff[idx]);
										}
									}
								}
							}
							else
							{
								//小説を最初から最新章までダウンロード
								//proc = downloadOne(hWnd, ldata, filepath);
								//proc.WaitForExit();
								downloadOneAsync2(hWnd, ldata, filepath);

							}
							//proc.WaitForExit();
							//リスト進行状況を表示する
							lblListProgress.Text = "(" + novelCount.ToString().PadLeft(4) + " / " + novelTotal.ToString().PadLeft(4) + ")";
							//小説情報ファイルを書き込む
							using (StreamWriter sw = new StreamWriter(File.Create(infopath), new UTF8Encoding()))
							{
								sw.WriteLine($"{DateTime.Now}, {ChapCount + latestChap}");
							}
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
			}
			return result;
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
		private string DateTimeCheck()
		{
			String result = "";

			DateTime tmpdt;
			DateTime nowDateTime = DateTime.Now;

			if (nowDateTime >= nextEveryMon)
			{
				int monInterval = DateTime.DaysInMonth(nowDateTime.Year, nowDateTime.Month) - nowDateTime.Day + 1;
				nextEveryMon = DateTime.Parse(nowDateTime.AddDays(monInterval).ToString("yyyy/MM/dd"));
				WritePrivateProfileString("NextDownLoad", "毎月", nextEveryMon.ToString(), iniPath);
				WritePrivateProfileString("NextDownLoad", "毎週", getEveryWeekNext(nowDateTime), iniPath);
				nextEveryDay = nowDateTime.AddHours(12);
				WritePrivateProfileString("NextDownLoad", "毎日", $"{nextEveryDay:yyyy/MM/dd HH:mm:ss}", iniPath);
				result = "毎月";
			}
			else if (nowDateTime >= nextEveryWeek)
			{
				WritePrivateProfileString("NextDownLoad", "毎週", getEveryWeekNext(nowDateTime), iniPath);
				nextEveryDay = nowDateTime.AddHours(12);
				WritePrivateProfileString("NextDownLoad", "毎日", $"{nextEveryDay:yyyy/MM/dd HH:mm:ss}", iniPath);
				result = "毎週";
			}
			else if (nowDateTime >= nextEveryDay)
			{
				nextEveryDay = nowDateTime.AddHours(12);
				WritePrivateProfileString("NextDownLoad", "毎日", $"{nextEveryDay:yyyy/MM/dd HH:mm:ss}", iniPath);
				result = "毎日";
			}
			return result;
		}

		/// <summary>
		/// 毎週の次回取得可能な最近の日時を取得する
		/// </summary>
		/// <param name="nowDateTime"></param>
		/// <returns></returns>
		private string getEveryWeekNext(DateTime nowDateTime)
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
			nextEveryWeek = DateTime.Parse(nowDateTime.AddDays(weekInterval).ToString("yyyy/MM/dd"));
			return $"{nextEveryWeek:yyyy/MM/dd HH:mm:ss}";
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
					}
					break;
			}

		}

		/// <summary>
		/// 小説1つをダウンロードする関数の非同期スレッド２
		/// 引数、戻り値付き関数を非同期にするのがよくわからかったので
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="URL"></param>
		/// <param name="filePath"></param>
		/// <param name="startChap"></param>
		private async void downloadOneAsync2(IntPtr hWnd, string URL, string filePath = null, int startChap = 0)
		{
			Task<Process> dl = downloadOneAsync(hWnd, URL, filePath, startChap);
			Process proc = await dl;
			proc.WaitForExit();
		}

		/// <summary>
		/// 小説1つをダウンロードする関数の非同期スレッド
		/// 引数、戻り値付き関数を非同期にするのがよくわからかったので
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="URL"></param>
		/// <param name="filePath"></param>
		/// <param name="startChap"></param>
		/// <returns></returns>
		private async Task<Process> downloadOneAsync(IntPtr hWnd, string URL, string filePath = null, int startChap = 0)
		{
			return downloadOne(hWnd, URL, filePath, startChap);
		}

		/// <summary>
		/// na6dl.exeを使って小説一つをダウンロードする
		/// </summary>
		/// <param name="URL"></param>
		/// <param name="filePath"></param>
		private Process downloadOne(IntPtr hWnd, string URL, string filePath = null, int startChap = 0)
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
			//proc.StartInfo.CreateNoWindow = true; // コンソール・ウィンドウを開かない
			//proc.StartInfo.UseShellExecute = false; // シェル機能を使用しない
			proc.SynchronizingObject = this;
			proc.Exited += new EventHandler(proc_Exited);//終了イベントを登録
			proc.EnableRaisingEvents = true;
			//起動する
			proc.Start();
			return proc;
		}

		/// <summary>
		/// コマンドプロンプトの終了イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void proc_Exited(object sender, EventArgs e)
		{
			//プロセスが終了したときに実行される
			lblStatusNovel.Text = (sStatus == "") ? "ダウンロード終了" : sStatus;
			procComplete = true;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			lblTimeCount.Text = $"{DateTime.Now - latestDLDateTime:hh\\:mm\\:ss}";
		}
	}
}