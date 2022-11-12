using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace AutoBuyer
{
    public partial class App : Form
    {
        globalKeyboardHook gkh = new globalKeyboardHook();

        public App()
        {
            InitializeComponent();
        }

        private void ClickMouse()
        {
            int c = 0;

            POINT p = new POINT();

            while (true)
            {
                GetCursorPos(ref p);
                ClientToScreen(Handle, ref p);
                DoMouseLeftClick(p.x, p.y);

                c++;

                if (c == 1)
                    break;
            }
        }

        [DllImport("user32.dll")]

        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

        [StructLayout(LayoutKind.Sequential)]

        public struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]

        public static extern void mouse_event(int dsFlags, int dx, int dy, int cButtons, int dsExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        private void DoMouseLeftClick(int x, int y)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
        }

        [DllImport("user32.dll")]

        public static extern bool GetCursorPos(ref POINT lpPoint);

        private void timer_Tick(object sender, EventArgs e)
        {
            Cursor.Position = new Point(1475, 950);
            ClickMouse();
            SendKeys.Send("9");
            SendKeys.Send("9");
            Cursor.Position = new Point(1475, 915);
            ClickMouse();
            ClickMouse();
        }

        private void App_Load(object sender, EventArgs e)
        {
            gkh.HookedKeys.Add(Keys.F10);
            gkh.HookedKeys.Add(Keys.F11);
            gkh.KeyUp += new KeyEventHandler(btn_start_KeyUp);
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void btn_start_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F10)
            {
                timer.Enabled = true;
            }
            if (e.KeyCode == Keys.F11)
            {
                timer.Enabled = false;
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_options_MouseEnter(object sender, EventArgs e)
        {
            btn_options.BackColor = Color.Brown;
        }

        private void btn_options_MouseLeave(object sender, EventArgs e)
        {
            btn_options.BackColor = Color.Brown;
        }

        private void btn_start_MouseLeave(object sender, EventArgs e)
        {
            btn_start.BackColor = Color.Transparent;
        }

        private void btn_start_MouseEnter(object sender, EventArgs e)
        {
            btn_start.BackColor = Color.White;
        }

        class globalKeyboardHook
        {
            #region Constant, Structure and Delegate Definitions

            public struct keyboardHookStruct
            {
                public int vkCode;
                public int scanCode;
                public int flags;
                public int time;
                public int dwExtraInfo;
            }

            const int WH_KEYBOARD_LL = 13;
            const int WM_KEYDOWN = 0x100;
            const int WM_KEYUP = 0x101;
            const int WM_SYSKEYDOWN = 0x104;
            const int WM_SYSKEYUP = 0x105;
            #endregion

            #region Instance Variables
            public List<Keys> HookedKeys = new List<Keys>();
            IntPtr hhook = IntPtr.Zero;
            #endregion

            #region Events

            public event KeyEventHandler KeyDown;

            public event KeyEventHandler KeyUp;
            #endregion

            #region Constructors and Destructors

            public globalKeyboardHook()
            {
                hook();
            }

            ~globalKeyboardHook()
            {
                unhook();
            }
            #endregion

            #region Public Methods
 
            public void hook()
            {
                IntPtr hInstance = LoadLibrary("User32");
                delegateHookProc = hookProc;
                hhook = SetWindowsHookEx(WH_KEYBOARD_LL, delegateHookProc, hInstance, 0);
            }

            public delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct lParam);
            keyboardHookProc delegateHookProc;

            public void unhook()
            {
                UnhookWindowsHookEx(hhook);
            }

            public int hookProc(int code, int wParam, ref keyboardHookStruct lParam)
            {
                if (code >= 0)
                {
                    Keys key = (Keys)lParam.vkCode;
                    if (HookedKeys.Contains(key))
                    {
                        KeyEventArgs kea = new KeyEventArgs(key);
                        if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (KeyDown != null))
                        {
                            KeyDown(this, kea);
                        }
                        else
                        if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && (KeyUp != null))
                        {
                            KeyUp(this, kea);
                        }
                        if (kea.Handled)
                            return 1;
                    }
                }
                return CallNextHookEx(hhook, code, wParam, ref lParam);
            }
            #endregion

            #region DLL imports

            [DllImport("user32.dll")]
            static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);
            [DllImport("user32.dll")]
            static extern bool UnhookWindowsHookEx(IntPtr hInstance);

            [DllImport("user32.dll")]
            static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref keyboardHookStruct lParam);

            [DllImport("kernel32.dll")]
            static extern IntPtr LoadLibrary(string lpFileName);
            #endregion
        }
    }
}