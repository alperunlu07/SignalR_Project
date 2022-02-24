using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePrefs : MonoSingleton<SavePrefs>
{
    public PlayerInfo playerInfo;
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        LoadPlayerPrefs();
    }
     
    void Update()
    {
        
    }

    public void LoadPlayerPrefs()
    {
        playerInfo = JsonModelHelper.StringToModel<PlayerInfo>(PlayerPrefs.GetString("PlayerInfo"));
    }
    public void SavePlayerPrefs()
    {
        string stringToSave = JsonModelHelper.ModelToString(playerInfo);
        PlayerPrefs.SetString("PlayerInfo", stringToSave);
        PlayerPrefs.Save();
    }

    [System.Serializable]
    public class PlayerInfo
    {
        public string userName; //{ get; set; } 
        public byte[] password;
        public string email;
        public string connectionID;
    }
}
