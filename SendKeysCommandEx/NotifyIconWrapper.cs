using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SendKeysCommandEx
{
    public partial class NotifyIconWrapper : Component
    {
        private MainWindow _mainWindow = null;

        public NotifyIconWrapper()
        {
            InitializeComponent();
        }

        public NotifyIconWrapper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void Initialize(MainWindow win)
        {
            _mainWindow = win;
            this.toolStripMenuItemMinimize.Click += toolStripMenuItemMinimize_Click;
            this.toolStripMenuItemSendKeys.Click += toolStripMenuItemSendKeys_Click;
            this.toolStripMenuItemQuit.Click += toolStripMenuItemQuit_Click;
        }

        private bool _BalloonTipVisible = true;
        public bool BalloonTipVisible
        {
            get { return _BalloonTipVisible; }
            set { _BalloonTipVisible = value; }
        }

        public void ShowInfoBalloonTip(int timeout, string message)
        {
            if (_BalloonTipVisible)
                this.notifyIcon.ShowBalloonTip(timeout, string.Empty, message, ToolTipIcon.Info);
        }

        private void notifyIcon_MouseDown(object sender, MouseEventArgs e)
        {
            if (_mainWindow != null)
                _mainWindow.SkipActivateApp = true;
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Left.Equals(e.Button))
                if (_mainWindow != null)
                    _mainWindow.Reshow();
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (_mainWindow == null) e.Cancel = true;
        }

        private void contextMenuStrip_Opened(object sender, EventArgs e)
        {
            if (_mainWindow.Visible)
            {
                this.toolStripMenuItemMinimize.Visible = true;
                this.toolStripMenuItemSendKeys.Visible = false;
            }
            else
            {
                this.toolStripMenuItemMinimize.Visible = false;
                this.toolStripMenuItemSendKeys.Visible = true;
            }
        }

        private void toolStripMenuItemMinimize_Click(object sender, EventArgs e)
        {
            _mainWindow.WindowState = System.Windows.WindowState.Minimized;
        }

        private void toolStripMenuItemSendKeys_Click(object sender, EventArgs e)
        {
            _mainWindow.DoSendKeys();
        }

        private void toolStripMenuItemQuit_Click(object sender, EventArgs e)
        {
            _mainWindow.Close();
        }
    }
}
