using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

#region For sending emails
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Linq; 
#endregion

namespace LoginUI_Canvas
{
    public class Login : MonoBehaviour
    {
        #region Variables
        [Header("Login Menu Panels")]
        public GameObject panel2NewAcc; // !!! MAKE A TOOLTIP PANEL FOR 'email already exists' !!!
        public GameObject panel3ForgotPass;
        public GameObject panel4RecoveryCode;
        public bool showNewAcc, showLostPass, showRecCode;

        // p = panel | 1 = Login | 2 = Create | 3 = Lost Password | 4 = Recovery Code
        [Header("Panel Input Fields")]
        public InputField p1fieldUsername;
        public InputField p1fieldPassword,
                          p2fieldUsername, p2fieldPassword, p2fieldConfirmPassword, p2fieldEmail,
                          p3fieldEmail,
                          p4recoveryCode;

        // For random recovery code.
        private static System.Random random = new System.Random();
        public string username;
        public string code;

        #endregion

        #region Functions 'n' Methods
        // Start is called just before any of the Update methods is called the first time
        public void Start()
        {
            p1fieldUsername = GameObject.Find("1 InputField (Username)").GetComponent<InputField>();
            p1fieldPassword = GameObject.Find("1 InputField (Password)").GetComponent<InputField>();
        }
        
        // Interactive elements on the Main Login Menu.
        #region Panel 1 (Main Login Menu)
        
        // Where we open and close the Create New Account menu.
        #region Toggle Create New Account (Panel 2)

        // Where we execute NewAccToggle().
        public void BTN_ToggleNewAcc()
        {
            NewAccToggle();
        }

        // Where we setup our button's function.
        bool NewAccToggle()
        {
            // If the Create New Account panel is closed...
            if (!showNewAcc)
            {
                // ... show the new account panel!
                showNewAcc = true;
                panel2NewAcc.SetActive(true);

                // Conditional check to only grab components if we haven't already done so.
                if (p2fieldUsername == null || p2fieldPassword == null || p2fieldConfirmPassword == null || p2fieldEmail == null)
                {
                    p2fieldUsername = GameObject.Find("2 InputField (Username)").GetComponent<InputField>();
                    p2fieldPassword = GameObject.Find("2 InputField (Password)").GetComponent<InputField>();
                    p2fieldConfirmPassword = GameObject.Find("2 InputField (ConfirmPassword)").GetComponent<InputField>();
                    p2fieldEmail = GameObject.Find("2 InputField (Email)").GetComponent<InputField>();
                    print("Panel 2 Components");
                }
                return true;
            }
            // Otherwise close the New Account panel.
            else
            {
                showNewAcc = false;
                panel2NewAcc.SetActive(false);
                return false;
            }
        }

        #endregion

        // Where we open and close the Forgot Password menu.
        #region Toggle Forgot Password (Panel 3)
        
        // See '#region Toggle Create New Account (Panel 2)' for logic, because this works identically.
        public void BTN_ToggleLostPass()
        {
            LostPassToggle();
        }

        bool LostPassToggle()
        {
            if (!showLostPass)
            {
                showLostPass = true;
                panel3ForgotPass.SetActive(true);
                if (p3fieldEmail == null)
                {
                    p3fieldEmail = GameObject.Find("3 InputField (Email)").GetComponent<InputField>();
                    print("Panel 3 Components");
                }
                return true;
            }
            else
            {
                showLostPass = false;
                panel3ForgotPass.SetActive(false);
                return false;
            }
        }

        #endregion
        
        #endregion
        
        // Interactive elements on the Create New Account Menu.
        #region Panel 2 (Create New Account Menu)
        public void BTN_CreateUser()
        {
            StartCoroutine(CreateUser(p2fieldUsername.text, p2fieldPassword.text, p2fieldEmail.text));
        }

        IEnumerator CreateUser(string _username, string _password, string _email)
        {
            //Link to PHP
            string createUserURL = "http://localhost/squealsystem/InsertUser.php";

            //Info to send to the POST variable in PHP
            WWWForm insertUserForm = new WWWForm();
            insertUserForm.AddField("usernamePost", _username);
            insertUserForm.AddField("passwordPost", _password);
            insertUserForm.AddField("emailPost", _email);

            WWW www = new WWW(createUserURL, insertUserForm);

            yield return www;
            //createAccToolTip = www.text;
            Debug.Log(www.text);
        }
        #endregion
        
        // Interactive Elements on the Forgot Password menu.
        #region Panel 3 (Forgot Password Menu)
        public void BTN_SendEmail()
        {
            RecoveryCodeToggle();
        }

        bool RecoveryCodeToggle()
        {
            // !!! MAKE IT DO THIS ONLY IF SUCCESSFUL !!!
            if (!showRecCode)
            {
                // Open the Recovery Code Menu (and hide the Forgot Password Menu).
                showRecCode = true;
                panel4RecoveryCode.SetActive(true);
                LostPassToggle();

                // Run 'SendEmail()'.
                SendEmail();
                if (p4recoveryCode == null)
                {
                    p4recoveryCode = GameObject.Find("4 InputField (Code)").GetComponent<InputField>();
                    print("Panel 4 Components");
                }
                return true;
            }
            else
            {
                showRecCode = false;
                panel4RecoveryCode.SetActive(false);
                return false;
            }
        }
        #endregion

        // Where we get an emailed recovery code.
        #region Panel 4 (Recovery Code Menu) / Email Management
        void SendEmail()
        {
            // Give us a random string as the code ( '(8)' is an 8 character code).
            code = RandomString(8);

            // Create a new email.
            MailMessage mail = new MailMessage();
            MailAddress ourEmail = new MailAddress("sqlunityclasssydney@gmail.com", "SQueaL 2019");
            // Sent To.
            mail.To.Add(p3fieldEmail.text);
            // Sent From.
            mail.From = ourEmail;
            // Topic.
            mail.Subject = "SQueaL System User Reset - Jett";
            // Message.
            mail.Body = "Hello," + username + "\nReset your Accout using this code: " + code;

            // Used to access and use Google to send emails.
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 25;

            smtpServer.Credentials = new System.Net.NetworkCredential("sqlunityclasssydney@gmail.com", "sqlpassword") as ICredentialsByHost;

            smtpServer.EnableSsl = true;

            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
            { return true; };
            smtpServer.Send(mail);
            Debug.Log("Success");
        }

        public static string RandomString(int length)
        {
            // Random characters that are valid.
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
            // Get a random character from chars and assemble it into an array of random characters.
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        IEnumerator GetUser(string _email)
        {
            //Link to PHP
            string createUserURL = "http://localhost/squealsystem/CheckUser.php";

            //Info to send to the POST variable in PHP
            WWWForm getUserForm = new WWWForm();
            getUserForm.AddField("emailPost", _email);

            WWW www = new WWW(createUserURL, getUserForm);

            yield return www;
            //createAccToolTip = www.text;
            Debug.Log(www.text);
            if (www.text != "No User")
            {
                username = www.text;
            }
        }
        #endregion

        #endregion
    }
}
