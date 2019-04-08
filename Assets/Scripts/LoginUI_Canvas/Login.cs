using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        public GameObject panel5NewPass;

        public bool showNewAcc, showLostPass, showRecCode, showNewPass;

        // p = panel | 1 = Login | 2 = Create | 3 = Lost Password | 4 = Recovery Code | 5 = New Password
        [Header("Panel Input Fields")]
        public InputField p1fieldUsername;
        public InputField p1fieldPassword,
                          p2fieldUsername, p2fieldPassword, p2fieldConfirmPassword, p2fieldEmail,
                          p3fieldEmail,
                          p4fieldRecoveryCode,
                          p5fieldNewPassword, p5fieldConfirmPassword;

        // For random recovery code.
        private static System.Random random = new System.Random();
        public string username;
        public string password, email, code;

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
            
            // Where we (try to) enter the game.
            #region Login
                
                // Where we switch to a new scene (if we can login).
                public void BTN_Login()
                {
                    username = p1fieldUsername.text;
                    password = p1fieldPassword.text;
                    StartCoroutine(CheckLogin(username, password));
                }
                
                // Crikey! It works! I'm tired.
                IEnumerator CheckLogin(string _username, string _password)
                {
                    //Link to PHP
                    string getUserURL = "http://localhost/squealsystem/LoginUser.php";
                    
                    //Info to send to the POST variable in PHP
                    WWWForm getUserForm = new WWWForm();
                    getUserForm.AddField("usernamePost", _username);
                    getUserForm.AddField("passwordPost", _password);
                    
                    WWW www = new WWW(getUserURL, getUserForm);
                    
                    yield return www;
                    
                    Debug.Log(www.text);
                    
                    if (www.text != "Invalid Login")
                    {
                        LoginUser();
                    }
                }
                
                void LoginUser()
                {
                    SceneManager.LoadScene(1);
                }
                
            #endregion
            
            // Where we open (and close) Panel 2.
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

                        // Only grab components if we haven't already done so (conditional check).
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
                    // Otherwise close the New Account panel and empty the text fields.
                    else
                    {
                        showNewAcc = false;
                        panel2NewAcc.SetActive(false);

                        p2fieldUsername.text = "";
                        p2fieldPassword.text = "";
                        p2fieldConfirmPassword.text = "";
                        p2fieldEmail.text = "";

                        return false;
                    }
                }

            #endregion

            // Where we open (and close) Panel 3.
            #region Toggle Forgot Password (Panel 3)
                
                // See '#region Toggle Create New Account (Panel 2)' for logic, because this works identically.

                // Where we execute LostPassToggle().
                public void BTN_ToggleLostPass()
                {
                    LostPassToggle();
                }

                // Where we setup our button's function.
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

                        p3fieldEmail.text = "";

                        return false;
                    }
                }

            #endregion

            // Where we quit the executable entirely.
            #region Exit Executable
                
                // Self-explanatory.
                public void BTN_ExitExecutable()
                {
                    #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                    #endif
                    Application.Quit();
                }

            #endregion

        #endregion

        // Interactive elements on the Create New Account Menu.
        #region Panel 2 (Create New Account Menu)
            
            // Where we execute CreateUser() with the new account information supplied, and close Panel 2.
            public void BTN_CreateUser()
            {
                // If any of these InputFields are NOT empty...
                if (p2fieldUsername.text != "" && p2fieldPassword.text != "" && p2fieldEmail.text != "")
                {
                    // ... and if the two entered passwords match...
                    if (p2fieldPassword.text == p2fieldConfirmPassword.text)
                    {
                        // Create a new user account using the information provided in the appropriate InputFields, then close Panel 2.
                        StartCoroutine(CreateUser(p2fieldUsername.text, p2fieldPassword.text, p2fieldEmail.text));
                        NewAccToggle();
                    }
                }
            }

            // Where we create a new user account for the database.
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

                Debug.Log(www.text);
            }

        #endregion
        
        // Interactive elements on the Forgot Password Menu.
        #region Panel 3 (Forgot Password Menu)
            
            // Where we execute GetUser() with the email supplied.
            public void BTN_SendEmail()
            {
                // Self-explanatory.
                email = p3fieldEmail.text;
                StartCoroutine(GetUser(email));
            }
            
            // Where we check the database for the provided email, and execute 'SendEmail()' (if successful).
            IEnumerator GetUser(string _email)
            {
                //Link to PHP
                string getUserURL = "http://localhost/squealsystem/CheckUser.php";

                //Info to send to the POST variable in PHP
                WWWForm getUserForm = new WWWForm();
                getUserForm.AddField("emailPost", _email);

                WWW www = new WWW(getUserURL, getUserForm);

                yield return www;

                Debug.Log(www.text);

                // If the email is valid (doesn't return 'No User')...
                if (www.text != "No User")
                {
                    // Return the user.
                    username = www.text;
                }

                // If we get a user...
                if (username != "")
                {
                    // Close Panel 3, and open Panel 4.
                    LostPassToggle();
                    RecCodeToggle();

                    // Run 'SendEmail()'.
                    SendEmail();
                }
            }
            
            // Stuff for emailing a recovery code.
            #region Email Management
                
                // Where we email a recovery code generated with 'RandomString()'.
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
                    mail.Body = "Hello, " + username + "\nReset your Accout using this code: " + code;

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

                // Where we generate a random string with a specified number of characters.
                public static string RandomString(int length)
                {
                    // Random characters that are valid.
                    const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
                    // Get a random character from chars and assemble it into an array of random characters.
                    return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
                }

            #endregion
            
            // Interactive elements on the Recovery Code Menu.
            #region Panel 4 (Recovery Code Menu)
                
                // Where we submit our recovery code.
                public void BTN_SubmitCode()
                {
                    // Switch to Panel 5 (if the recovery code submitted is correct).
                    if (p4fieldRecoveryCode.text == code)
                    {
                        NewPassToggle();
                        RecCodeToggle();
                    }
                }
                
                // Where we execute 'RecCodeToggle()'.
                public void BTN_ToggleRecCode()
                {
                    RecCodeToggle();
                }

                // Where we setup our button's function.
                bool RecCodeToggle()
                {
                    if (!showRecCode)
                    {
                        showRecCode = true;
                        panel4RecoveryCode.SetActive(true);
                        
                        if (p4fieldRecoveryCode == null)
                        {
                            p4fieldRecoveryCode = GameObject.Find("4 InputField (Code)").GetComponent<InputField>();
                            print("Panel 4 Components");
                        }
                        return true;
                    }
                    else
                    {
                        showRecCode = false;
                        panel4RecoveryCode.SetActive(false);
                        
                        p4fieldRecoveryCode.text = "";
                        
                        return false;
                    }
                }

                // Interactive elements on the Recovery Code Menu.
                #region Panel 5 (Reset Password)
                    
                    // Where we submit our new password with the new password supplied.
                    public void BTN_NewPassword()
                    {
                        StartCoroutine(ResetPassword(p5fieldNewPassword.text, email));
                    }
                    
                    // Where we... you know what? The name is pretty self-explanatory.
                    IEnumerator ResetPassword(string _password, string _email)
                    {
                        if (p5fieldNewPassword.text == p5fieldConfirmPassword.text)
                        {
                            password = p5fieldNewPassword.text;
                            
                            //Link to PHP
                            string newPasswordURL = "http://localhost/squealsystem/UpdatePassword.php";
                            
                            //Info to send to the POST variable in PHP
                            WWWForm getPasswordForm = new WWWForm();
                            getPasswordForm.AddField("passwordPost", _password);
                            getPasswordForm.AddField("emailPost", _email);
                            
                            WWW www = new WWW(newPasswordURL, getPasswordForm);
                            
                            yield return www;
                            
                            Debug.Log(www.text);
                            
                            if (www.text == "Password Changed")
                            {
                                password = www.text;
                                email = null;
                                NewPassToggle();
                            }
                        }
                    }
                    
                    // Where we execute 'NewPassToggle()'.
                    public void BTN_ToggleNewPass()
                    {
                        NewPassToggle();
                    }
                    
                    // Where we setup our button's function.
                    bool NewPassToggle()
                    {
                        if (!showNewPass)
                        {
                            showNewPass = true;
                            panel5NewPass.SetActive(true);
                            
                            if (p5fieldNewPassword == null && p5fieldConfirmPassword == null)
                            {
                                p5fieldNewPassword = GameObject.Find("5 InputField (New)").GetComponent<InputField>();
                                p5fieldConfirmPassword = GameObject.Find("5 InputField (Re-enter)").GetComponent<InputField>();
                                print("Panel 5 Components");
                            }
                            return true;
                        }
                        else
                        {
                            showNewPass = false;
                            panel5NewPass.SetActive(false);
                            
                            p5fieldNewPassword.text = "";
                            p5fieldConfirmPassword.text = "";
                            
                            return false;
                        }
                    }
                    
                #endregion
            
            #endregion

        #endregion
        
        #endregion
    }
}
// I think I need a hug...