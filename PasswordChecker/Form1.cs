using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace PasswordChecker
{
    public partial class frmPasswordChecker : Form
    {
        public frmPasswordChecker()
        {
            InitializeComponent();
        }

        //The maximum number of attempts allowed
        private const int MAX_INCORRECT_PASSWORDS = 3;
        //The user password
        private const string USER_PASSWORD = "correcthorsebatterystaple";
        //The admin password
        private const string ADMIN_PASSWORD = "Tr0ub4dor&3";
        //The PasswordChar that masks the password
        private const char HIDDEN_PASSWORD_MASK = '*';
        //The PasswordChar that shows the password
        private const char VISIBLE_PASSWORD_MASK = '\0';
        private const string SHOW_PASSWORD_TEXT = "Show Password";
        private const string HIDE_PASSWORD_TEXT = "Hide Password";

        //The number of attempts that the user has done
        private int intAttemptsCount = 0;

        //The incorrect passwords that the user have tried
        private string[] strIncorrectPasswords = new string[MAX_INCORRECT_PASSWORDS];

        //Whether the password is hidden or not 
        private bool blnPasswordHidden = true;

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            //Check if the entered password is empty
            if (txtPassword.Text != null && txtPassword.Text != "")
            {
                //Check if the user still have attempts left
                if (intAttemptsCount < MAX_INCORRECT_PASSWORDS)
                {
                    //Increase the attempt count regardless of whether the password is correct or not.
                    intAttemptsCount++;
                    //Check if the password is correct
                    if (txtPassword.Text.Equals(USER_PASSWORD))
                    {
                        MessageBox.Show("Correct password. You may now exit");
                        //Show the exit menu item
                        exitToolStripMenuItem.Visible = true;
                        //Reset the previous attempts and the text box
                        intAttemptsCount = 0;
                        strIncorrectPasswords = new string[MAX_INCORRECT_PASSWORDS];
                        txtPassword.Text = "";
                    }
                    else
                    {
                        //Log the incorrect password
                        strIncorrectPasswords[intAttemptsCount - 1] = txtPassword.Text;
                        //Clear the text box
                        txtPassword.Text = "";
                        //Check if the user finished his attempts
                        if (intAttemptsCount >= MAX_INCORRECT_PASSWORDS)
                        {
                            MessageBox.Show("Too many incorrect passwords. You have been locked out. Contact an administrator to unlock you");
                            //Disabled the text box
                            txtPassword.Enabled = false;
                        }
                        else
                        {
                            //Show the user the attempts left
                            MessageBox.Show("Incorrect password. You have " + (MAX_INCORRECT_PASSWORDS - intAttemptsCount) + " attempts left.");
                            txtPassword.Focus();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter a non-empty password");
            }
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Ask for the admin password
            string strEnteredPassword = InputBoxUtils.InputBoxDK('*', "Please enter your Admin's password:");
            //Check if the entered password is correct
            if (strEnteredPassword.Equals(ADMIN_PASSWORD))
            {
                MessageBox.Show("Welcome, Admin!");
                //Show the unlock and report menu items
                unlockToolStripMenuItem.Visible = true;
                reportToolStripMenuItem.Visible = true;
            }
            else
            {
                MessageBox.Show("Incorrect password. Please try again.");
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Exit the app
            Application.Exit();
        }

        private void unlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Enable the text box
            txtPassword.Enabled = true;
            //Reset the previous attempts
            intAttemptsCount = 0;
            strIncorrectPasswords = new string[MAX_INCORRECT_PASSWORDS];
        }

        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string strReportMessage = "Here are the incorrect attempted passwords:";
            //Loop over all the previous incorrect passwords
            foreach (string strPassword in strIncorrectPasswords)
            {
                //If the password is not empty, add it to the report message.
                if (strPassword != null && strPassword != "")
                    strReportMessage += "\n" + strPassword;
            }
            //Show the report
            MessageBox.Show(strReportMessage);
        }

        private void btnShowHidePassword_Click(object sender, EventArgs e)
        {
            btnShowHidePassword.Text = blnPasswordHidden ? HIDE_PASSWORD_TEXT : SHOW_PASSWORD_TEXT;
            txtPassword.PasswordChar = blnPasswordHidden ? VISIBLE_PASSWORD_MASK : HIDDEN_PASSWORD_MASK;
            blnPasswordHidden = !blnPasswordHidden;
        }
    }
}
