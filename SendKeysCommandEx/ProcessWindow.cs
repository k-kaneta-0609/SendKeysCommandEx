using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SendKeysCommandEx
{
    class ProcessWindow
    {
        private static int _processID;
        private static List<string> _titleList = null;
        private static List<IntPtr> _hWndList = null;

        /// <summary>
        /// 指定したプロセスIDのWindowがアクティブか調べる
        /// </summary>
        public static bool IsForegroundWindow(int processID)
        {
            IntPtr hWnd = GetForegroundWindow();

            int foregroundProcessID;
            GetWindowThreadProcessId(hWnd, out foregroundProcessID);

            return (processID == foregroundProcessID);
        }

        /// <summary>
        /// 指定したプロセスIDのWindowのタイトルを列挙する
        /// </summary>
        public static List<string> EnumWindowTitles(int processID)
        {
            Initialize(processID);

            EnumWindows(EnumWindowCallBack, IntPtr.Zero);

            return _titleList; 
        }

        /// <summary>
        /// 指定したプロセスIDのWindowハンドルを列挙する
        /// </summary>
        public static List<IntPtr> EnumWindowHandles(int processID)
        {
            Initialize(processID);

            EnumWindows(EnumWindowCallBack, IntPtr.Zero);

            return _hWndList;
        }

        private static void Initialize(int processID)
        {
            _processID = processID;

            if (_titleList != null)
            {
                _titleList.Clear();
                _titleList = null;
            }
            _titleList = new List<string>();

            if (_hWndList != null)
            {
                _hWndList.Clear();
                _hWndList = null;
            }
            _hWndList = new List<IntPtr>();
        }

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lParam);

        private delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        private const string MESSAGEBOX_CLASS_NAME = "#32770";
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        private static bool EnumWindowCallBack(IntPtr hWnd, IntPtr lparam)
        {
            // ウィンドウのプロセスのIDを取得する
            int processId;
            GetWindowThreadProcessId(hWnd, out processId);

            if (_processID == processId)
            {
                // ウィンドウの表示状態を取得する
                bool visible = IsWindowVisible(hWnd);

                // ウィンドウのクラス名を取得する
                StringBuilder className = new StringBuilder(256);
                GetClassName(hWnd, className, className.Capacity);

                // ウィンドウの状態を判定(表示状態のWindow、または、メッセージボックス)
                if ((visible) || (MESSAGEBOX_CLASS_NAME.Equals(className.ToString())))
                {
                    // ウィンドウのタイトルの長さを取得する
                    StringBuilder title = null;
                    int titleLength = GetWindowTextLength(hWnd);
                    if (titleLength > 0)
                    {
                        // ウィンドウのタイトルを取得する
                        title = new StringBuilder(titleLength + 1);
                        GetWindowText(hWnd, title, title.Capacity);
                    }
                    else
                    {
                        title = new StringBuilder();
                    }

                    // ウィンドウタイトルリストの追加
                    _titleList.Add(title.ToString());

                    // ウィンドウハンドルリストの追加
                    _hWndList.Add(hWnd);  
                }
            }

            // 次のウィンドウへ
            return true;
        }
    }
}
