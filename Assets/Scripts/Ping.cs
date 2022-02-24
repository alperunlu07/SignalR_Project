using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ping: MonoSingleton<Ping>
{
    private float currentTime;
    public float pingValue;
    public float pingValueMs;
    public float pingWait = 10f;



    int counter = 0;
    int reqCounter = 0;
    void Start()
    {
        StartCoroutine(PingLoop());
    }
    public void Tick(string message)
    {
        if(int.TryParse(message,out reqCounter))
        {
            
        }
    }
    public void Tack()
    {

    }

    IEnumerator PingLoop()
    {
        yield return new WaitForSeconds(2f);
        while (true)
        {

            ServerController.Instance.SendWithMethot("Ping",counter.ToString());
            float sendTime = Time.time;
            yield return new WaitUntil(() => counter == reqCounter || (Time.time - sendTime) > pingWait);
            pingValue = Time.time - sendTime;
            pingValueMs = pingValue * 1000f;
            yield return new WaitForSeconds(1f);
            counter++;
        }
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0,h -(h * 5 / 100), w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 4 / 100;
        style.normal.textColor = new Color(1.0f, 0.0f, 0.5f, 1.0f);
 
        string text = string.Format(" " + pingValueMs.ToString("0.0") + " ms");
        GUI.Label(rect, text, style);
    }


}
