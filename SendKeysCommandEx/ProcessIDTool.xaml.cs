using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SendKeysCommandEx
{
    /// <summary>
    /// ProcessIDTool.xaml の相互作用ロジック
    /// </summary>
    public partial class ProcessIDTool : Window
    {
        const int GWL_STYLE = -16;
        const uint WS_MAXIMIZEBOX = 0x10000;
        const uint WS_MINIMIZEBOX = 0x20000;

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int index, int newStyle);

        public ProcessIDTool(MainWindow mainWindow)
        {
            InitializeComponent();

            // 必ず MainWindow を親とする
            this.Owner = mainWindow;
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            // MainWindow に閉じた事を通知する
            if (this.Owner != null)
                ((MainWindow)this.Owner).ProcessIDToolWindow = null;
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 最小化と最大化ボタンの無効化
            var helper = new System.Windows.Interop.WindowInteropHelper(this);
            long currentStyle = GetWindowLong(helper.Handle, GWL_STYLE);
            SetWindowLong(helper.Handle, GWL_STYLE, (int)(currentStyle & ~(WS_MINIMIZEBOX) & ~(WS_MAXIMIZEBOX)));
        }

        private void buttonSearchID_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBoxSearchID.Text))
            {
                MessageBox.Show(this, "プロセス名を指定して下さい。", "注意", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            string searchPattern = this.textBoxSearchID.Text;

            if (this.listBoxSearchID.Items.Count > 0) this.listBoxSearchID.Items.Clear();

            foreach (Process localProcess in Process.GetProcesses())
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(localProcess.ProcessName, searchPattern))
                    this.listBoxSearchID.Items.Add(
                        string.Format("名称:{0}\t ID:{1}", localProcess.ProcessName, localProcess.Id)
                    );
            }
        }

        private void buttonAddConvertID_Click(object sender, RoutedEventArgs e)
        {
            this.listBoxConvertID.Items.Add(
                string.Format("FROM:{0}\t→\tTO:{1}", this.textBoxConvertIDFrom.Text, this.textBoxConvertIDTo.Text)
            );
        }

        private void buttonDeleteConvertID_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBoxConvertID.SelectedIndex < 0) return;

            this.listBoxConvertID.Items.RemoveAt(this.listBoxConvertID.SelectedIndex);
        }

        private void buttonClearConvertID_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBoxConvertID.Items.Count < 1) return;

            this.listBoxConvertID.Items.Clear();
        }

        private void buttonConvertID_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBoxConvertID.Items.Count < 1) return;

            if (this.Owner == null) return;

            MainWindow main = (MainWindow)this.Owner;

            List<string> matchList = new List<string>();
            matchList.Add("Activate:");
            matchList.Add("プロセスID ");

            foreach (string convert in this.listBoxConvertID.Items)
            {
                string fromID = convert.Split('\t')[0].Split(':')[1];
                string toID = convert.Split('\t')[2].Split(':')[1];

                int currentSelected = main.listBoxSendKeys.SelectedIndex;

                for (int i = 0; i < main.listBoxSendKeys.Items.Count; i++)
                {
                    string command = (string)main.listBoxSendKeys.Items[i];

                    foreach (string matchWord in matchList)
                    {
                        int matchIndex = command.IndexOf(matchWord + fromID);

                        if (matchIndex >= 0)
                        {
                            main.listBoxSendKeys.Items.RemoveAt(i);
                            main.listBoxSendKeys.Items.Insert(i, command.Replace(matchWord + fromID, matchWord + toID));
                        }
                    }
                }

                if (currentSelected >= 0)
                    main.listBoxSendKeys.SelectedIndex = currentSelected;

                if (this.checkBoxConvertDefaultID.IsChecked.HasValue)
                    if (this.checkBoxConvertDefaultID.IsChecked.Value)
                        if (fromID.Equals(main.textBoxStartPID.Text))
                            main.textBoxStartPID.Text = toID;
            }
        }
    }
}
