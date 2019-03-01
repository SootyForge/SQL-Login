using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoginUI_HardCode
{
    public class Login : MonoBehaviour
    {
        public bool createMenu, forgotMenu, playerAcc, characterCreate, inputCode, resetPassword;
        public string username = "username", password = "password", email = "email", createAccToolTip, generatedCode, resetCode;
        public float scrW, scrH;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

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

            Debug.Log(www.text);
        }
    } 
}
