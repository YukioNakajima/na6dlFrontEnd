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
			this.lbUrlList.Location = new System.Drawing.Point(0, 0);
			this.lbUrlList.Name = "lbUrlList";
			this.lbUrlList.ScrollAlwaysVisible = true;
			this.lbUrlList.Size = new System.Drawing.Size(400, 148);
			this.lbUrlList.TabIndex = 0;
			// 
			// btnAddList
			// 
			this.btnAddList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddList.Location = new System.Drawing.Point(406, 0);
			this.btnAddList.Name = "btnAddList";
			this.btnAddList.Size = new System.Drawing.Size(89, 23);
			this.btnAddList.TabIndex = 1;
			this.btnAddList.Text = "リストに追加...";
			this.btnAddList.UseVisualStyleBackColor = true;
			this.btnAddList.Click += new System.EventHandler(this.btnAddList_Click);
			// 
			// lblNovelTitle
			// 
			this.lblNovelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblNovelTitle.Location = new System.Drawing.Point(6, 17);
			this.lblNovelTitle.Name = "lblNovelTitle";
			this.lblNovelTitle.Size = new System.Drawing.Size(483, 17);
			this.lblNovelTitle.TabIndex = 0;
			this.lblNovelTitle.Text = "小説名";
			// 
			// lblTimeCount
			// 
			this.lblTimeCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lblTimeCount.Location = new System.Drawing.Point(222, 166);
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
			this.lblStatusNovel.Size = new System.Drawing.Size(297, 15);
			this.lblStatusNovel.TabIndex = 1;
			this.lblStatusNovel.Text = "Status";
			// 
			// lblProgress
			// 
			this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblProgress.Location = new System.Drawing.Point(311, 34);
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new System.Drawing.Size(100, 15);
			this.lblProgress.TabIndex = 2;
			this.lblProgress.Text = "  0% (   0 /    0)";
			// 
			// btnDelList
			// 
			this.btnDelList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDelList.Location = new System.Drawing.Point(406, 58);
			this.btnDelList.Name = "btnDelList";
			this.btnDelList.Size = new System.Drawing.Size(89, 23);
			this.btnDelList.TabIndex = 2;
			this.btnDelList.Text = "リストから削除";
			this.btnDelList.UseVisualStyleBackColor = true;
			this.btnDelList.Click += new System.EventHandler(this.btnDelList_Click);
			// 
			// btnDownload
			// 
			this.btnDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDownload.Location = new System.Drawing.Point(423, 113);
			this.btnDownload.Name = "btnDownload";
			this.btnDownload.Size = new System.Drawing.Size(72, 23);
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
			this.groupBox1.Location = new System.Drawing.Point(12, 190);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(495, 55);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "小説情報";
			// 
			// lblListProgress
			// 
			this.lblListProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lblListProgress.AutoSize = true;
			this.lblListProgress.Location = new System.Drawing.Point(358, 166);
			this.lblListProgress.Name = "lblListProgress";
			this.lblListProgress.Size = new System.Drawing.Size(23, 12);
			this.lblListProgress.TabIndex = 7;
			this.lblListProgress.Text = "0/0";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(275, 166);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(77, 12);
			this.label1.TabIndex = 6;
			this.label1.Text = "リスト進行状況";
			// 
			// lblStatusApp
			// 
			this.lblStatusApp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblStatusApp.Location = new System.Drawing.Point(18, 165);
			this.lblStatusApp.Name = "lblStatusApp";
			this.lblStatusApp.Size = new System.Drawing.Size(198, 14);
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
			this.pnlBtn.Controls.Add(this.btnAddList);
			this.pnlBtn.Controls.Add(this.btnDelList);
			this.pnlBtn.Controls.Add(this.btnDownload);
			this.pnlBtn.Controls.Add(this.lbUrlList);
			this.pnlBtn.Location = new System.Drawing.Point(3, 12);
			this.pnlBtn.Name = "pnlBtn";
			this.pnlBtn.Size = new System.Drawing.Size(504, 150);
			this.pnlBtn.TabIndex = 9;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(519, 254);
			this.Controls.Add(this.lblStatusApp);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblListProgress);
			this.Controls.Add(this.lblTimeCount);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.pnlBtn);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(535, 293);
			this.Name = "Form1";
			this.Text = "Form1";
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
	}
}

