using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public enum ReqTypes
{
    login,
    registry
}
public class HttpRequests: MonoSingleton<HttpRequests>
{
    string url = "https://localhost:5001/api/Users/ReqList";
    public Dictionary<ReqTypes, string> requestList = new Dictionary<ReqTypes, string>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        StartCoroutine(GetRequestList());


      
    }

    private void Update()
    {

    }

    IEnumerator GetRequestList()
    {
        yield return new WaitForEndOfFrame();
        while (requestList.Count < 1)
        {
            UnityWebRequest uwr = UnityWebRequest.Get(url);
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
            else
            { 
                requestList = JsonModelHelper.StringToModel<Dictionary<ReqTypes, string>>(uwr.downloadHandler.text);
                //for (int i = 0; i < requestList.Count; i++)
                //{
                //    Debug.Log(requestList[(ReqTypes)i]);
                //}                 
            }

            //yield return new WaitForSeconds(1f);
        }
        StartCoroutine(WaitForAutoLogin());
    }

    IEnumerator WaitForAutoLogin()
    {
        yield return new WaitForEndOfFrame();
        if (SavePrefs.Instance.playerInfo.userName != null)
        {
            Debug.Log(SavePrefs.Instance.playerInfo.userName);
            SendRequest(ReqTypes.login, JsonModelHelper.ModelToString(new User { userName = SavePrefs.Instance.playerInfo.userName, password = SavePrefs.Instance.playerInfo.password }));
        }
        
        yield return new WaitForSeconds(1f);
    }

    public void SendRequest(ReqTypes reqTypes, string req)
    { 
        if(requestList.Count > 0)
            StartCoroutine(postRequest(requestList[reqTypes], req));
        else
            InputFieldController.Instance.WarningMessage("connection error");
    }

    IEnumerator postRequest(string url, string json)
    {
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            string val = uwr.downloadHandler.text;
            
            
            long reqCode = uwr.responseCode;
            Debug.Log("code: " + uwr.responseCode.ToString());
            Debug.Log("Received: " + val);
            if (reqCode == 200) // success
            { 
                if(val.Length> 1)
                {
                   
                    var usrObj = JsonModelHelper.StringToModel<User>(val);
                    if(usrObj != null)
                    {
                        var aa = InputFieldController.Instance.GetUserLoginVals();
                        //if (aa.userName.Length > 1)
                        //{

                        //    SavePrefs.Instance.playerInfo.password = aa.password;
                        //    SavePrefs.Instance.SavePlayerPrefs();
                        //}
                        SavePrefs.Instance.playerInfo.userName = usrObj.userName;
                        if(aa.password != null)
                            SavePrefs.Instance.playerInfo.password = aa.password;
                        SavePrefs.Instance.playerInfo.email = usrObj.email;
                        SavePrefs.Instance.SavePlayerPrefs();
                    }
                    GameManager.Instance.LoadScene(1);
                } 
            }
            else // error
            { 
                if(Time.time > 2f)
                {
                    var msg = JsonModelHelper.StringToModel<HttpReq>(val);
                    Debug.Log("Received: " + msg.message);
                    InputFieldController.Instance.WarningMessage(msg.message);
                }

            } 
        }
    } 
}


public class HttpReq
{ 
    public int reqTypes { get; set; }
    public string message { get; set; }
}
