using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordChecker
{
    public static class InputBoxUtils
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int CallNextHookEx(int hHook,
        int ncode, int wParam, int lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetModuleHandleA(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowsHookExA(int idHook, NewProcDelegate lpfn, int hmod,
        int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int UnhookWindowsHookEx(int hHook);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetCurrentThreadId();

        //private const EM_SETPASSWORDCHAR = &HCC;
        private const int WH_CBT = 5;

        private const int HCBT_ACTIVATE = 5;
        private const int HC_ACTION = 0;

        //The PasswordChar that masks the password
        private const char HIDDEN_PASSWORD_MASK = '*';
        //The PasswordChar that shows the password
        private const char VISIBLE_PASSWORD_MASK = '\0';
        private const string SHOW_PASSWORD_TEXT = "Show Password";
        private const string HIDE_PASSWORD_TEXT = "Hide Password";

        private static int hHook;

        private static char chrPasswordChar;

        public static bool blnPasswordHidden = true;

        public delegate int NewProcDelegate(int lngCode, int wParam, int lParam);


        public static int NewProc(int lngCode, int wParam, int lParam)
        {

            string strClassName;

            if (lngCode < HC_ACTION)
                return CallNextHookEx(hHook, lngCode, wParam, lParam);

            strClassName = new String(' ', 256);

            if (lngCode == HCBT_ACTIVATE)
            {
                Control test = Control.FromHandle(new IntPtr(wParam));


                System.Reflection.FieldInfo textBoxField = test.GetType().GetField("TextBox", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);


                if (textBoxField != null)
                {
                    TextBox textBox = (TextBox)textBoxField.GetValue(test);
                    textBox.PasswordChar = chrPasswordChar;
                    Button showHidePasswordButton = new Button
                    {
                        Location = new System.Drawing.Point(253, 64),
                        Name = "showHidePasswordButton",
                        Size = new System.Drawing.Size(97, 23),
                        Text = SHOW_PASSWORD_TEXT,
                        UseVisualStyleBackColor = true
                    };
                    showHidePasswordButton.Click += new EventHandler((object sender, EventArgs e) =>
                    {
                        showHidePasswordButton.Text = blnPasswordHidden ? HIDE_PASSWORD_TEXT : SHOW_PASSWORD_TEXT;
                        textBox.PasswordChar = blnPasswordHidden ? VISIBLE_PASSWORD_MASK : HIDDEN_PASSWORD_MASK;
                        blnPasswordHidden = !blnPasswordHidden;
                    });
                    test.Controls.Add(showHidePasswordButton);
                    showHidePasswordButton.BringToFront();
                }
            }

            return CallNextHookEx(hHook, lngCode, wParam, lParam);


        }

        public static string InputBoxDK(char PasswordChar, string Prompt, string Title = "", string DefaultResponse = "", int XPos = -1, int YPos = -1)
        {
            int lngModHwnd = 0;

            int lngThreadID = 0;

            chrPasswordChar = PasswordChar;
            lngThreadID = GetCurrentThreadId();

            lngModHwnd = GetModuleHandleA(null);

            hHook = SetWindowsHookExA(WH_CBT, NewProc, lngModHwnd, lngThreadID);
            try
            {
                return Interaction.InputBox(Prompt, Title, DefaultResponse, XPos, YPos);
            }
            finally
            {
                UnhookWindowsHookEx(hHook);
            }
        }
    }
}
