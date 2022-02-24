using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    int sceneId;
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }
    public void LoadScene(int _sceneId = 0)
    {
        sceneId = _sceneId;
        SceneManager.LoadScene(sceneId);
    }
}
