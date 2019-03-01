using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace LoginUI_Canvas
{
    public class Login : MonoBehaviour
    {
        #region Variables
        [Header("Login Menu Panels")]
        public GameObject panel2NewAcc;
        public GameObject panel3ForgotPass;
        public bool showNewAcc, showLostPass;

        // p = panel | 1 = Login | 2 = Create | 3 = Lost Password
        [Header("Panel Input Fields")]
        public InputField p1fieldUsername;
        public InputField p1fieldPassword,
                          p2fieldUsername, p2fieldPassword, p2fieldConfirmPassword, p2fieldEmail,
                          p3fieldEmail;

        #endregion

        #region Functions 'n' Methods
        // Start is called just before any of the Update methods is called the first time
        public void Start()
        {
            p1fieldUsername = GameObject.Find("1 InputField (Username)").GetComponent<InputField>();
            p1fieldPassword = GameObject.Find("1 InputField (Password)").GetComponent<InputField>();
        }

        public void BTN_ToggleNewAcc()
        {
            NewAccToggle();
        }

        bool NewAccToggle()
        {
            // If the Create New Account panel is closed...
            if (!showNewAcc)
            {
                // ... show the new account panel!
                showNewAcc = true;
                panel2NewAcc.SetActive(true);

                // Conditional check to only grab components if we haven't already done so.
                // Note: This was still running when it only had the one '== null' at the end for some reason.
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
    }
}
