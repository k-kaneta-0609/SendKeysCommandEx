namespace SendKeysCommandEx
{
    partial class NotifyIconWrapper
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotifyIconWrapper));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemMinimize = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSendKeys = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "SendKeysCommand";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            this.notifyIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDown);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemMinimize,
            this.toolStripMenuItemSendKeys,
            this.toolStripMenuItemQuit});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(134, 70);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            this.contextMenuStrip.Opened += new System.EventHandler(this.contextMenuStrip_Opened);
            // 
            // toolStripMenuItemMinimize
            // 
            this.toolStripMenuItemMinimize.Name = "toolStripMenuItemMinimize";
            this.toolStripMenuItemMinimize.Size = new System.Drawing.Size(133, 22);
            this.toolStripMenuItemMinimize.Text = "非表示化";
            // 
            // toolStripMenuItemSendKeys
            // 
            this.toolStripMenuItemSendKeys.Name = "toolStripMenuItemSendKeys";
            this.toolStripMenuItemSendKeys.Size = new System.Drawing.Size(133, 22);
            this.toolStripMenuItemSendKeys.Text = "SendKeys";
            this.toolStripMenuItemSendKeys.Visible = false;
            // 
            // toolStripMenuItemQuit
            // 
            this.toolStripMenuItemQuit.Name = "toolStripMenuItemQuit";
            this.toolStripMenuItemQuit.Size = new System.Drawing.Size(133, 22);
            this.toolStripMenuItemQuit.Text = "終了";
            this.contextMenuStrip.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMinimize;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSendKeys;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemQuit;
    }
}
