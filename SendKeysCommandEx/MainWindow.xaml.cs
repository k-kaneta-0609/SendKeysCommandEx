using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SendKeysCommandEx
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string COMP_MARK_EQUALS = "＝";
        private const string COMP_MARK_NOTEQUALS = "≠";
        private const string COMP_MARK_UNDER = "＜";
        private const string COMP_MARK_OVER = "＞";

        private const string SLEEP_COMMAND = "Sleep:";
        private const string ACTIVATE_COMMAND = "Activate:";
        private const string WAITACTIVATE_COMMAND = "WaitActivate:";
        private const string CANCEL_COMMAND = "Cancel:";

        private const string LOOP_COMMAND_HEAD = "Loop";
        private const string LOOP_ONCE_ACTION = "回目";
        private const string LOOP_EVERY_ACTION = "回毎";
        private const string LOOP_ONCE_COMMAND = LOOP_COMMAND_HEAD + LOOP_ONCE_ACTION + ":";
        private const string LOOP_EVERY_COMMAND = LOOP_COMMAND_HEAD + LOOP_EVERY_ACTION +":";

        private const string HALF_SPACE_WORD = "{半角スペース}";
        private const string FULL_SPACE_WORD = "{全角スペース}";
        private const string RANDOM_CHAR = "{ランダム文字}";

        private const string FILE_DIALOG_FILTER = "SendKeysファイル(*.skc)|*.skc|すべてのファイル(*.*)|*.*";
        private static Encoding FILE_ENCODIND = Encoding.UTF8;

        private const int WM_ACTIVATEAPP = 0x001C;

        private NotifyIconWrapper _notifyIcon;

        private ProcessIDTool _ProcessIDToolWindow = null;
        public ProcessIDTool ProcessIDToolWindow
        {
            get { return _ProcessIDToolWindow; }
            set { _ProcessIDToolWindow = value; }
        }

        private bool _SkipActivateApp = false;
        public bool SkipActivateApp
        {
            get { return _SkipActivateApp; }
            set { _SkipActivateApp = value; }
        }

        public bool Visible
        {
            get
            {
                return this.ShowInTaskbar;
            }
            set
            {
                if (value)
                {
                    if (this.ShowInTaskbar == false)
                        this.ShowInTaskbar = true;
                    if (this.WindowState == WindowState.Minimized)
                        this.WindowState = WindowState.Normal;
                }
                else
                {
                    if (this.WindowState != WindowState.Minimized)
                        this.WindowState = WindowState.Minimized;
                    if (this.ShowInTaskbar)
                        this.ShowInTaskbar = false;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // SendKeysコンボボックスの選択肢を設定
            this.comboBoxAddSendKeys.Items.Add("{ESC}");
            this.comboBoxAddSendKeys.Items.Add("{BACKSPACE}");
            this.comboBoxAddSendKeys.Items.Add("{CAPSLOCK}");
            this.comboBoxAddSendKeys.Items.Add("{TAB}");
            this.comboBoxAddSendKeys.Items.Add("{ENTER}");
            this.comboBoxAddSendKeys.Items.Add("{PRTSC}");
            this.comboBoxAddSendKeys.Items.Add("{SCROLLLOCK}");
            this.comboBoxAddSendKeys.Items.Add("{BREAK}");
            this.comboBoxAddSendKeys.Items.Add("{INSERT}");
            this.comboBoxAddSendKeys.Items.Add("{DELETE}");
            this.comboBoxAddSendKeys.Items.Add("{HOME}");
            this.comboBoxAddSendKeys.Items.Add("{END}");
            this.comboBoxAddSendKeys.Items.Add("{PGUP}");
            this.comboBoxAddSendKeys.Items.Add("{PGDN}");
            this.comboBoxAddSendKeys.Items.Add("{NUMLOCK}");
            this.comboBoxAddSendKeys.Items.Add("{UP}");
            this.comboBoxAddSendKeys.Items.Add("{DOWN}");
            this.comboBoxAddSendKeys.Items.Add("{LEFT}");
            this.comboBoxAddSendKeys.Items.Add("{RIGHT}");
            this.comboBoxAddSendKeys.Items.Add("{HELP}");
            this.comboBoxAddSendKeys.Items.Add("{F1}");
            this.comboBoxAddSendKeys.Items.Add("{F2}");
            this.comboBoxAddSendKeys.Items.Add("{F3}");
            this.comboBoxAddSendKeys.Items.Add("{F4}");
            this.comboBoxAddSendKeys.Items.Add("{F5}");
            this.comboBoxAddSendKeys.Items.Add("{F6}");
            this.comboBoxAddSendKeys.Items.Add("{F7}");
            this.comboBoxAddSendKeys.Items.Add("{F8}");
            this.comboBoxAddSendKeys.Items.Add("{F9}");
            this.comboBoxAddSendKeys.Items.Add("{F10}");
            this.comboBoxAddSendKeys.Items.Add("{F11}");
            this.comboBoxAddSendKeys.Items.Add("{F12}");
            this.comboBoxAddSendKeys.Items.Add("{F13}");
            this.comboBoxAddSendKeys.Items.Add("{F14}");
            this.comboBoxAddSendKeys.Items.Add("{F15}");
            this.comboBoxAddSendKeys.Items.Add("{F16}");
            this.comboBoxAddSendKeys.Items.Add(HALF_SPACE_WORD);
            this.comboBoxAddSendKeys.Items.Add(FULL_SPACE_WORD);
            this.comboBoxAddSendKeys.Items.Add(RANDOM_CHAR);

            // Window数比較記号コンボボックスの選択肢を設定
            this.comboBoxCancelWindowCount.Items.Add(COMP_MARK_EQUALS);
            this.comboBoxCancelWindowCount.Items.Add(COMP_MARK_NOTEQUALS);
            this.comboBoxCancelWindowCount.Items.Add(COMP_MARK_UNDER);
            this.comboBoxCancelWindowCount.Items.Add(COMP_MARK_OVER);
            this.comboBoxCancelWindowCount.SelectedIndex = 0;

            // ループ条件アクションコンボボックスの選択肢を設定
            this.comboBoxLoopActionCount.Items.Add(LOOP_ONCE_ACTION);
            this.comboBoxLoopActionCount.Items.Add(LOOP_EVERY_ACTION);
            this.comboBoxLoopActionCount.SelectedIndex = 0;

            // NotifyIconの表示
            _notifyIcon = new NotifyIconWrapper();
            _notifyIcon.Initialize(this);
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (_notifyIcon != null) _notifyIcon.Dispose();
        }

        /// <summary>
        /// ウインドウメッセージを処理するメソッドを定義
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));
        }

        /// <summary>
        /// ウインドウメッセージを処理し
        /// アクティブメッセージを受け取った時
        /// Windowを表示させる
        /// </summary>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if ((msg == WM_ACTIVATEAPP) && ((int)wParam != 0))
            {
                if (this.SkipActivateApp)
                {
                    this.SkipActivateApp = false;
                }
                else
                {
                    handled = true;
                    Reshow();
                }
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// 画面サイズ変更時に表示/非表示を切り替える
        /// </summary>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Visible = false;
            }
            else
            {
                this.Visible = true;
            }
        }

        private void menuOpenFile_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Title = "SendKeysコマンドの読み込み";
                ofd.Filter = FILE_DIALOG_FILTER;
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;
                if (this.listBoxSendKeys.Items.Count > 0) this.listBoxSendKeys.Items.Clear();
                try
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(ofd.FileName, FILE_ENCODIND))
                    {
                        while (sr.Peek() > -1)
                        {
                            this.listBoxSendKeys.Items.Add(sr.ReadLine());
                        }
                    }
                }
                catch (Exception ex)
                {
                    string msg = "ファイルの読み込みに失敗しました。" + Environment.NewLine + Environment.NewLine + ex.Message;
                    MessageBox.Show(this, msg, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void menuSaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBoxSendKeys.Items.Count < 1)
            {
                MessageBox.Show(this, "SendKeysコマンドが登録されていません。", "注意", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog())
            {
                sfd.Title = "SendKeysコマンドの保存";
                sfd.Filter = FILE_DIALOG_FILTER;
                sfd.OverwritePrompt = true;
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;
                try
                {
                    using (System.IO.StreamWriter sr = new System.IO.StreamWriter(sfd.FileName, false, FILE_ENCODIND))
                    {
                        foreach (string command in this.listBoxSendKeys.Items)
                        {
                            try
                            {
                                sr.WriteLine(command); 
                            }
                            catch (System.IO.IOException ex)
                            {
                                string msg = "ファイルの書き込みに失敗しました。" + Environment.NewLine + Environment.NewLine + ex.Message;
                                MessageBox.Show(this, msg, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }
                    }
                }
                catch (System.IO.PathTooLongException)
                {
                    MessageBox.Show(this, "指定されたファイルパス、または、ファイル名は使用できません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException || ex is System.Security.SecurityException)
                {
                    string msg = "ファイルの保存に失敗しました。" + Environment.NewLine + Environment.NewLine + ex.Message;
                    MessageBox.Show(this, msg, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void menuProssIDTool_Click(object sender, RoutedEventArgs e)
        {
            if (this.ProcessIDToolWindow == null)
            {
                this.ProcessIDToolWindow = new ProcessIDTool(this);
                this.ProcessIDToolWindow.Top = this.Top + 50d;
                this.ProcessIDToolWindow.Left = this.Left + 20d;
                this.ProcessIDToolWindow.Show();
            }
            else
            {
                if (this.ProcessIDToolWindow.IsActive == false)
                    this.ProcessIDToolWindow.Activate();
            }
        }

        private void buttonAddSendKeys_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.comboBoxAddSendKeys.Text))
            {
                MessageBox.Show(this, "SendKeysコマンドを登録して下さい。", "注意", MessageBoxButton.OK, MessageBoxImage.Exclamation );
                return;
            }
            
            StringBuilder command = new StringBuilder();
            if (this.checkBoxAddSendKeysShift.IsChecked.HasValue)
                if (this.checkBoxAddSendKeysShift.IsChecked.Value )
                    command.Append("+");
            if (this.checkBoxAddSendKeysCtrl.IsChecked.HasValue)
                if (this.checkBoxAddSendKeysCtrl.IsChecked.Value)
                    command.Append("^");
            if (this.checkBoxAddSendKeysAlt.IsChecked.HasValue)
                if (this.checkBoxAddSendKeysAlt.IsChecked.Value)
                    command.Append("%");
            command.Append(this.comboBoxAddSendKeys.Text);

            string reservedWord = string.Empty;
            if (command.ToString().StartsWith(SLEEP_COMMAND)) reservedWord = SLEEP_COMMAND;
            if (command.ToString().StartsWith(ACTIVATE_COMMAND)) reservedWord = ACTIVATE_COMMAND;
            if (command.ToString().StartsWith(WAITACTIVATE_COMMAND)) reservedWord = WAITACTIVATE_COMMAND;
            if (command.ToString().StartsWith(CANCEL_COMMAND)) reservedWord = CANCEL_COMMAND;
            if (command.ToString().StartsWith(LOOP_ONCE_COMMAND)) reservedWord = LOOP_ONCE_COMMAND;
            if (command.ToString().StartsWith(LOOP_EVERY_COMMAND)) reservedWord = LOOP_EVERY_COMMAND;
            if (string.IsNullOrEmpty(reservedWord) == false)
            {
                MessageBox.Show(this, reservedWord + " は予約語なので登録できません。", "注意", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            AddSendKeys(command.ToString());
        }

        private void buttonAddSleep_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBoxAddSleep.Text) || Convert.ToInt32(this.textBoxAddSleep.Text) == 0)
            {
                MessageBox.Show(this, "Sleep値は 1 以上を設定して下さい。", "注意", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            string command = SLEEP_COMMAND + this.textBoxAddSleep.Text;

            AddSendKeys(command);
        }

        private void buttonAddSleepRandom_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBoxAddSleepRandomFrom.Text) || Convert.ToInt32(this.textBoxAddSleepRandomFrom.Text) == 0)
            {
                MessageBox.Show(this, "Sleep値は 1 以上を設定して下さい。", "注意", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (string.IsNullOrWhiteSpace(this.textBoxAddSleepRandomTo.Text) || Convert.ToInt32(this.textBoxAddSleepRandomTo.Text) == 0)
            {
                MessageBox.Show(this, "Sleep値は 1 以上を設定して下さい。", "注意", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (Convert.ToInt32(this.textBoxAddSleepRandomTo.Text) < Convert.ToInt32(this.textBoxAddSleepRandomFrom.Text))
            {
                MessageBox.Show(this, "Sleepランダム範囲は From < To となるように設定して下さい。", "注意", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            string command = SLEEP_COMMAND + this.textBoxAddSleepRandomFrom.Text + "～" + this.textBoxAddSleepRandomTo.Text + " の範囲でランダムタイム";

            AddSendKeys(command);
        }

        private void buttonAddActive_Click(object sender, RoutedEventArgs e)
        {
            string command = ACTIVATE_COMMAND + this.textBoxActivePID.Text;

            AddSendKeys(command);
        }

        private void buttonAddWaitActive_Click(object sender, RoutedEventArgs e)
        {
            string command = WAITACTIVATE_COMMAND + this.textBoxWaitActivePID.Text;

            AddSendKeys(command);
        }

        private void buttonCancelWindowCount_Click(object sender, RoutedEventArgs e)
        {
            string command =
                CANCEL_COMMAND +
                this.labelCancelWindowCountPID.Content + " " +
                this.textBoxCancelWindowCountPID.Text + " " +
                this.labelCancelWindowCount2.Content + " " +
                this.comboBoxCancelWindowCount.Text + " " +
                this.textBoxCancelWindowCount.Text + " " +
                this.labelCancelWindowCount3.Content + "、" +
                "処理を中止します。";

            AddSendKeys(command);
        }

        private void buttonCancelWindowActive_Click(object sender, RoutedEventArgs e)
        {
            string command =
                CANCEL_COMMAND +
                this.labelCancelWindowActivePID.Content + " " +
                this.textBoxCancelWindowActivePID.Text + " " +
                "が非アクティブになった時、" +
                "処理を中止します。";

            AddSendKeys(command);
        }

        private void AddSendKeys(string command)
        {
            if ((this.checkBoxLoopAction.IsChecked.HasValue) && (this.checkBoxLoopAction.IsChecked.Value))
            {
                string loopCommand = string.Empty;
                if (LOOP_ONCE_ACTION.Equals(this.comboBoxLoopActionCount.Text))
                    loopCommand = LOOP_ONCE_COMMAND;
                if (LOOP_EVERY_ACTION.Equals(this.comboBoxLoopActionCount.Text))
                    loopCommand = LOOP_EVERY_COMMAND;
                if (string.IsNullOrEmpty(loopCommand) == false)
                    command = loopCommand + this.textBoxLoopActionCount.Text + ' ' + command;
            }

            int insertIndex = this.listBoxSendKeys.Items.Count;
            if (this.listBoxSendKeys.SelectedIndex >= 0)
                insertIndex = this.listBoxSendKeys.SelectedIndex;

            this.listBoxSendKeys.Items.Insert(insertIndex, command);
        }

        private void buttonDeleteSendKeys_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBoxSendKeys.SelectedIndex < 0) return;

            this.listBoxSendKeys.Items.RemoveAt(this.listBoxSendKeys.SelectedIndex);
        }

        private void buttonReleaseSelected_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBoxSendKeys.SelectedIndex < 0) return;

            this.listBoxSendKeys.SelectedIndex = -1;
        }

        private void buttonClearSendKeys_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBoxSendKeys.Items.Count < 1) return;

            this.listBoxSendKeys.Items.Clear();
        }

        private void buttonRunSendKeys_Click(object sender, RoutedEventArgs e)
        {
            DoSendKeys();
        }

        public void Reshow()
        {
            if (this.Visible == false) this.Visible = true;
            this.Activate();
        }

        public void DoSendKeys()
        {
            if (this.listBoxSendKeys.Items.Count <= 0)
            {
                Reshow();
                MessageBox.Show(this, "SendKeysコマンドが設定されていません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int loopCount = Convert.ToInt32(this.textBoxLoopCount.Text);
            if (loopCount < 1)
            {
                Reshow();
                MessageBox.Show(this, "実行回数は 1 以上に設定して下さい。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (this.Visible) this.Visible = false;

            bool validShowInfoBalloonTip = true;
            if (this.checkShowBalloon.IsChecked) validShowInfoBalloonTip = false;
            _notifyIcon.BalloonTipVisible = validShowInfoBalloonTip;

            string startPID = this.textBoxStartPID.Text;
            if (string.IsNullOrEmpty(startPID) == false)
            {
                _notifyIcon.ShowInfoBalloonTip(5000, "プロセスID[" + startPID + "]をアクティブにします。");
                if (ActiveWindow(Convert.ToInt32(startPID), 2000, true) == false) return;
            }

            _notifyIcon.ShowInfoBalloonTip(10000, "SendKeysを開始します。");

            for (int i = 1; i <= loopCount; i++)
            {
                _notifyIcon.ShowInfoBalloonTip(30000, "ループ" + i.ToString() + "回目");

                foreach (string command in this.listBoxSendKeys.Items)
                {
                    System.Windows.Forms.Application.DoEvents();

                    if (this.Visible)
                    {
                        _notifyIcon.ShowInfoBalloonTip(30000, "SendKeysを中止しました。");
                        return;
                    }

                    string cmd = command;
                    if (command.StartsWith(LOOP_ONCE_COMMAND))
                    {
                        int count = Convert.ToInt32(command.Substring(LOOP_ONCE_COMMAND.Length).Split(' ')[0]);
                        if (count == i)
                        {
                            cmd = command.Substring(command.Split(' ')[0].Length + 1);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (command.StartsWith(LOOP_EVERY_COMMAND))
                    {
                        int count = Convert.ToInt32(command.Substring(LOOP_EVERY_COMMAND.Length).Split(' ')[0]);
                        if ((count > 0) && (i % count == 0))
                        {
                            cmd = command.Substring(command.Split(' ')[0].Length + 1);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (DoCommand(cmd) == false) return;
                }
            }

            _notifyIcon.ShowInfoBalloonTip(360000, "SendKeysが正常終了しました。");

            if (this.checkEndActivate.IsChecked) Reshow();
        }

        private bool ActiveWindow(int processId, int sleepTime, bool showMsgBox)
        {
            try
            {
                Microsoft.VisualBasic.Interaction.AppActivate(processId);
            }
            catch (ArgumentException)
            {
                if (showMsgBox)
                {
                    Reshow();
                    MessageBox.Show(this, "プロセスID [" + processId.ToString() + "] が見つかりません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);                
                }
                else
                {
                    _notifyIcon.ShowInfoBalloonTip(360000, "プロセスID [" + processId.ToString() + "] が存在しない為、SendKeysを終了しました。");
                }
                return false;
            }
            System.Windows.Forms.Application.DoEvents();
            if (sleepTime > 0) System.Threading.Thread.Sleep(sleepTime);
            return true;
        }

        private bool DoCommand(string command)
        {
            if (command.StartsWith(SLEEP_COMMAND))
            {
                if (command.Contains("～"))
                {
                    string span = command.Substring(SLEEP_COMMAND.Length).Split(' ')[0];
                    int sleepRandomFrom = Convert.ToInt32(span.Split('～')[0]);
                    int sleepRandomTo = Convert.ToInt32(span.Split('～')[1]);
                    System.Threading.Thread.Sleep(new Random().Next(sleepRandomFrom, sleepRandomTo));
                }
                else
                {
                    int sleepTime = Convert.ToInt32(command.Substring(SLEEP_COMMAND.Length));
                    System.Threading.Thread.Sleep(sleepTime);
                }
            }
            else if (command.StartsWith(ACTIVATE_COMMAND))
            {
                int processID = Convert.ToInt32(command.Substring(ACTIVATE_COMMAND.Length));
                if (ActiveWindow(processID, 0, false) == false) return false;
            }
            else if (command.StartsWith(WAITACTIVATE_COMMAND))
            {
                int processID = Convert.ToInt32(command.Substring(WAITACTIVATE_COMMAND.Length));
                _notifyIcon.ShowInfoBalloonTip(30000, "プロセスID [" + processID.ToString() + "] がアクティブになるまで待機中" );
                while (ProcessWindow.IsForegroundWindow(processID) == false)
                {
                    System.Windows.Forms.Application.DoEvents();
                    if (this.Visible)
                    {
                        _notifyIcon.ShowInfoBalloonTip(30000, "SendKeysを中止しました。");
                        return false;
                    }
                    System.Threading.Thread.Sleep(200);
                }
            }
            else if (command.StartsWith(CANCEL_COMMAND))
            {
                string[] cancelWords = command.Split(' ');
                bool cancel = false;
                if (cancelWords.Length == 7)
                {
                    int processID = Convert.ToInt32(cancelWords[1]);
                    string comparison = cancelWords[4];
                    int compareWindowCount = Convert.ToInt32(cancelWords[5]);
                    int currentWindowCount = ProcessWindow.EnumWindowHandles(processID).Count;
                    if (COMP_MARK_EQUALS.Equals(comparison))
                    {
                        if (currentWindowCount == compareWindowCount) cancel = true;
                    }
                    if (COMP_MARK_NOTEQUALS.Equals(comparison))
                    {
                        if (currentWindowCount != compareWindowCount) cancel = true;
                    }
                    if (COMP_MARK_UNDER.Equals(comparison))
                    {
                        if (currentWindowCount < compareWindowCount) cancel = true;
                    }
                    if (COMP_MARK_OVER.Equals(comparison))
                    {
                        if (currentWindowCount > compareWindowCount) cancel = true;
                    }
                }
                if (cancelWords.Length == 3)
                {
                    int processID = Convert.ToInt32(cancelWords[1]);
                    if (ProcessWindow.IsForegroundWindow(processID) == false) cancel = true;
                }
                if (cancel)
                {
                    _notifyIcon.ShowInfoBalloonTip(360000, "中止条件に該当した為、SendKeysを終了しました。");
                    if (this.checkCancelActivate.IsChecked) Reshow();
                    return false;
                }
            }
            else
            {
                if (this.Visible == false)
                {
                    try
                    {
                        string[] randomChars =
                            {
                                "0","1","2","3","4","5","6","7","8","9",
                                "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
                                "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
                                "!","\"","#","$","{%}","&","'","{(}","{)}","*","{+}",",","-",".","/",":",";","<","=",">","?","@","[","\\","]","{^}","_","`","{{}","|","{}}","{~}"
                            };
                        int i = new Random().Next(0, randomChars.Length - 1);
                        System.Windows.Forms.SendKeys.SendWait(command.Replace(HALF_SPACE_WORD, " ").Replace(FULL_SPACE_WORD, "　").Replace(RANDOM_CHAR, randomChars[i]));
                    }
                    catch (Exception)
                    {
                        Reshow();
                        MessageBox.Show(this, "SendKeysエラー", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
