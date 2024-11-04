namespace na6dlFrontEnd
{
	partial class Form1
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.lbUrlList = new System.Windows.Forms.ListBox();
			this.btnAddList = new System.Windows.Forms.Button();
			this.lblNovelTitle = new System.Windows.Forms.Label();
			this.lblTimeCount = new System.Windows.Forms.Label();
			this.lblStatusNovel = new System.Windows.Forms.Label();
			this.lblProgress = new System.Windows.Forms.Label();
			this.btnDelList = new System.Windows.Forms.Button();
			this.btnDownload = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lblListProgress = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.lblStatusApp = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.pnlBtn = new System.Windows.Forms.Panel();
			this.btnItemUp = new System.Windows.Forms.Button();
			this.btnItemDn = new System.Windows.Forms.Button();
			this.btnUrlAdd = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.pnlBtn.SuspendLayout();
			this.SuspendLayout();
			// 
			// lbUrlList
			// 
			this.lbUrlList.AllowDrop = true;
			this.lbUrlList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lbUrlList.FormattingEnabled = true;
			this.lbUrlList.ItemHeight = 12;
			this.lbUrlList.Location = new System.Drawing.Point(11, 10);
			this.lbUrlList.Name = "lbUrlList";
			this.lbUrlList.ScrollAlwaysVisible = true;
			this.lbUrlList.Size = new System.Drawing.Size(395, 172);
			this.lbUrlList.TabIndex = 0;
			this.lbUrlList.SelectedIndexChanged += new System.EventHandler(this.lbUrlList_SelectedIndexChanged);
			// 
			// btnAddList
			// 
			this.btnAddList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddList.Location = new System.Drawing.Point(0, 2);
			this.btnAddList.Name = "btnAddList";
			this.btnAddList.Size = new System.Drawing.Size(106, 23);
			this.btnAddList.TabIndex = 1;
			this.btnAddList.Text = "リストを追加...";
			this.btnAddList.UseVisualStyleBackColor = true;
			this.btnAddList.Click += new System.EventHandler(this.btnAddList_Click);
			// 
			// lblNovelTitle
			// 
			this.lblNovelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblNovelTitle.Location = new System.Drawing.Point(6, 17);
			this.lblNovelTitle.Name = "lblNovelTitle";
			this.lblNovelTitle.Size = new System.Drawing.Size(505, 17);
			this.lblNovelTitle.TabIndex = 0;
			this.lblNovelTitle.Text = "小説名";
			// 
			// lblTimeCount
			// 
			this.lblTimeCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lblTimeCount.Location = new System.Drawing.Point(227, 193);
			this.lblTimeCount.Name = "lblTimeCount";
			this.lblTimeCount.Size = new System.Drawing.Size(47, 15);
			this.lblTimeCount.TabIndex = 5;
			this.lblTimeCount.Text = "00:00:00";
			// 
			// lblStatusNovel
			// 
			this.lblStatusNovel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblStatusNovel.Location = new System.Drawing.Point(8, 34);
			this.lblStatusNovel.Name = "lblStatusNovel";
			this.lblStatusNovel.Size = new System.Drawing.Size(319, 15);
			this.lblStatusNovel.TabIndex = 1;
			this.lblStatusNovel.Text = "Status";
			// 
			// lblProgress
			// 
			this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblProgress.Location = new System.Drawing.Point(333, 34);
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new System.Drawing.Size(100, 15);
			this.lblProgress.TabIndex = 2;
			this.lblProgress.Text = "  0% (   0 /    0)";
			// 
			// btnDelList
			// 
			this.btnDelList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDelList.BackColor = System.Drawing.Color.Yellow;
			this.btnDelList.Location = new System.Drawing.Point(0, 107);
			this.btnDelList.Name = "btnDelList";
			this.btnDelList.Size = new System.Drawing.Size(106, 23);
			this.btnDelList.TabIndex = 2;
			this.btnDelList.Text = "リストから削除";
			this.btnDelList.UseVisualStyleBackColor = false;
			this.btnDelList.Click += new System.EventHandler(this.btnDelList_Click);
			// 
			// btnDownload
			// 
			this.btnDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDownload.Location = new System.Drawing.Point(412, 159);
			this.btnDownload.Name = "btnDownload";
			this.btnDownload.Size = new System.Drawing.Size(106, 23);
			this.btnDownload.TabIndex = 3;
			this.btnDownload.Text = "ダウンロード開始";
			this.btnDownload.UseVisualStyleBackColor = true;
			this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.lblNovelTitle);
			this.groupBox1.Controls.Add(this.lblProgress);
			this.groupBox1.Controls.Add(this.lblStatusNovel);
			this.groupBox1.Location = new System.Drawing.Point(3, 217);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(517, 55);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "小説情報";
			// 
			// lblListProgress
			// 
			this.lblListProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lblListProgress.AutoSize = true;
			this.lblListProgress.Location = new System.Drawing.Point(363, 193);
			this.lblListProgress.Name = "lblListProgress";
			this.lblListProgress.Size = new System.Drawing.Size(23, 12);
			this.lblListProgress.TabIndex = 7;
			this.lblListProgress.Text = "0/0";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(280, 193);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(77, 12);
			this.label1.TabIndex = 6;
			this.label1.Text = "リスト進行状況";
			// 
			// lblStatusApp
			// 
			this.lblStatusApp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblStatusApp.Location = new System.Drawing.Point(18, 192);
			this.lblStatusApp.Name = "lblStatusApp";
			this.lblStatusApp.Size = new System.Drawing.Size(203, 14);
			this.lblStatusApp.TabIndex = 4;
			this.lblStatusApp.Text = "AppStatus";
			this.lblStatusApp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// pnlBtn
			// 
			this.pnlBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pnlBtn.Controls.Add(this.btnItemUp);
			this.pnlBtn.Controls.Add(this.btnItemDn);
			this.pnlBtn.Controls.Add(this.btnUrlAdd);
			this.pnlBtn.Controls.Add(this.btnAddList);
			this.pnlBtn.Controls.Add(this.btnDelList);
			this.pnlBtn.Location = new System.Drawing.Point(412, 12);
			this.pnlBtn.Name = "pnlBtn";
			this.pnlBtn.Size = new System.Drawing.Size(108, 133);
			this.pnlBtn.TabIndex = 9;
			// 
			// btnItemUp
			// 
			this.btnItemUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnItemUp.Location = new System.Drawing.Point(0, 60);
			this.btnItemUp.Name = "btnItemUp";
			this.btnItemUp.Size = new System.Drawing.Size(52, 23);
			this.btnItemUp.TabIndex = 4;
			this.btnItemUp.Text = "▲";
			this.btnItemUp.UseVisualStyleBackColor = true;
			this.btnItemUp.Click += new System.EventHandler(this.btnItemUp_Click);
			// 
			// btnItemDn
			// 
			this.btnItemDn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnItemDn.Location = new System.Drawing.Point(54, 59);
			this.btnItemDn.Name = "btnItemDn";
			this.btnItemDn.Size = new System.Drawing.Size(52, 23);
			this.btnItemDn.TabIndex = 4;
			this.btnItemDn.Text = "▼";
			this.btnItemDn.UseVisualStyleBackColor = true;
			this.btnItemDn.Click += new System.EventHandler(this.btnItemDn_Click);
			// 
			// btnUrlAdd
			// 
			this.btnUrlAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnUrlAdd.Location = new System.Drawing.Point(0, 31);
			this.btnUrlAdd.Name = "btnUrlAdd";
			this.btnUrlAdd.Size = new System.Drawing.Size(106, 23);
			this.btnUrlAdd.TabIndex = 1;
			this.btnUrlAdd.Text = "URLを追加...";
			this.btnUrlAdd.UseVisualStyleBackColor = true;
			this.btnUrlAdd.Click += new System.EventHandler(this.btnUrlAdd_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(524, 281);
			this.Controls.Add(this.lblStatusApp);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblListProgress);
			this.Controls.Add(this.lblTimeCount);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnDownload);
			this.Controls.Add(this.pnlBtn);
			this.Controls.Add(this.lbUrlList);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(535, 320);
			this.Name = "Form1";
			this.Text = "なろう小説ダウンローダー";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.groupBox1.ResumeLayout(false);
			this.pnlBtn.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox lbUrlList;
		private System.Windows.Forms.Button btnAddList;
		private System.Windows.Forms.Label lblNovelTitle;
		private System.Windows.Forms.Label lblTimeCount;
		private System.Windows.Forms.Label lblStatusNovel;
		private System.Windows.Forms.Label lblProgress;
		private System.Windows.Forms.Button btnDelList;
		private System.Windows.Forms.Button btnDownload;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblListProgress;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblStatusApp;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Panel pnlBtn;
		private System.Windows.Forms.Button btnUrlAdd;
		private System.Windows.Forms.Button btnItemUp;
		private System.Windows.Forms.Button btnItemDn;
	}
}

