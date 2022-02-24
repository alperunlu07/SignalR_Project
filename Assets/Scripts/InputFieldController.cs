using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SavePrefs;

public class InputFieldController : MonoSingleton<InputFieldController>
{
    public GameObject loginScreen, registryScreen;
    public InputField loginUserName, loginPassword;
    public Button loginSignIn, loginSignUp;
    public InputField registryUserName, registryPassword, registryPassword2, registryEmail;
    public Button registrySignIn, registrySignUp;
    public GameObject[] warningTexts;
    public bool state = false; // false login, true registry
    void Start()
    {
        loginScreen.SetActive(true);
        registryScreen.SetActive(false);
    }
     
   
    public void BtnClick(int id)
    {
        if(id == 0)
        {
            if (!state)
            {
                if(loginUserName.text.Length > 1 && loginPassword.text.Length > 1)
                {
                    User user = new User { userName = loginUserName.text, password = Crypto.MD5Hash(loginPassword.text) };
                    HttpRequests.Instance.SendRequest(ReqTypes.login, JsonModelHelper.ModelToString<User>(user));
                }
                else
                {
                    //TODO: Internal ui warnings
                }
            }
            else
            {
                state = false;
                loginScreen.SetActive(true);
                registryScreen.SetActive(false);
            }
        }
        else
        {
            if (!state)
            {
                state = true;
                loginScreen.SetActive(false);
                registryScreen.SetActive(true);
            }
            else
            {

                if (registryEmail.text.Length > 1 && IsValidEmail(registryEmail.text) && registryUserName.text.Length > 1 && registryPassword.text.Length > 1 && registryPassword2.text == registryPassword.text && registryEmail.text.Length > 1)
                {
                    User user = new User { email = registryEmail.text, userName = registryUserName.text, password = Crypto.MD5Hash(registryPassword.text) };
                    HttpRequests.Instance.SendRequest(ReqTypes.registry, JsonModelHelper.ModelToString<User>(user));
                }
                else if(!IsValidEmail(registryEmail.text))
                {
                    WarningMessage("invalid email adress");
                    //TODO: Internal ui warnings
                }
                else
                {
                    //TODO: Internal ui warnings
                    WarningMessage("invalid input");
                }
            }
        }
    }
    public PlayerInfo GetUserLoginVals()
    {
        return new PlayerInfo { userName = loginUserName.text, password = Crypto.MD5Hash(loginPassword.text) };
    }

    bool IsValidEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith("."))
        {
            return false; // suggested by @TK-421
        }
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch
        {
            return false;
        }
    }
    public void WarningMessage(string message)
    { 
        int index = state ? 1 : 0;
        warningTexts[index].SetActive(true);
        warningTexts[index].GetComponentInChildren<Text>().text = message;
        StartCoroutine(closeWarningMessageCO(warningTexts[index]));
    }
    IEnumerator closeWarningMessageCO(GameObject go)
    {
        yield return new WaitForSeconds(5f);
        go.GetComponentInChildren<Text>().text = "";
        go.SetActive(false);

    }

}
