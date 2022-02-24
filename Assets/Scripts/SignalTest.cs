using Microsoft.AspNetCore.SignalR.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class SignalTest : MonoBehaviour
{
    private static HubConnection connection;
    private bool tryConnect = false;
    public List<User> users = new List<User>();

    public float pingVal = 0;
    private string pingReq;
    private float pingTime;
    void Start()
    {
        connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:5001/signalr") 
            
            //.WithAutomaticReconnect()
            //.ConfigureLogging(logging => logging.Services.Add(new Microsoft.Extensions.DependencyInjection.ServiceDescriptor()))
            .Build();
 
        connection.Closed += async (error) =>
        {
            Debug.Log("connection closed");
            await Task.Delay(Random.Range(0, 5) * 1000);
            await connection.StartAsync();
        };
        connection.On<string>("receiveMessage", ( message) =>
        {
            Debug.Log($"{name}: {message}");
        });
        connection.On<string>("broadcastMessage", ( message) =>
        {
            Debug.Log($"{name}: {message}");
        });
        connection.On<string>("UserJoined", ( message) =>
        {
            //NewUser(message);
            Debug.Log($"{name}: {message}");
        });
        connection.On<string>("AllUserId", ( message) =>
        {
            Debug.Log($"{name}: {message}");
            //AllUser(message);
        });
        connection.On<string>("UserLeaved", (message) =>
        {
            //RemoveUser(message);
            Debug.Log($"{name}: {message}");
        });
        connection.On<string>("Ping", (message) =>
        {
            //pingReq = message; 
            Ping.Instance.Tick(message);
        });
        Connect();

        StartCoroutine(PingCO());

        //Send(Random.Range(0, 5).ToString());
        //Send(Random.Range(0, 5).ToString());
        //Send(Random.Range(0, 5).ToString());
    }

    IEnumerator PingCO()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            float fTime = Time.time;
            SendWithMethot("Ping", fTime.ToString());
            yield return new WaitUntil(WaitForPing);
            pingVal = Mathf.Round((Time.time - fTime)*1000);
        }
    }
    private bool WaitForPing()
    {
        return false;
    }

    private async void Connect()
    {
        tryConnect = true;
       
        try
        {
            await connection.StartAsync();

            Debug.Log("Connection started");
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
        tryConnect = false;
         
    }

    private void NewUser(string obj)
    {
        var newUser = ModelSerializer.String2Model<User>(obj);
        users.Add(newUser);
    }
    private void AllUser(string obj)
    {
        //users = ModelSerializer.String2Model<List<User>>(obj);
    }
    private void RemoveUser(string obj)
    {
        int removeIndex = users.FindIndex(x => x.connectionID == obj);
        if (removeIndex > -1)
            users.RemoveAt(removeIndex);
    }
    private async void Send(string msg)
    {
        if(connection.State == HubConnectionState.Connected)
        {
            try
            {
                await connection.InvokeAsync("SendMessageAsync", msg);
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        else
        {

            Debug.Log("Connection Error");
            if(!tryConnect)
                Connect();
        }
    }
    private async void SendWithMethot(string methotName, string msg)
    {
        if (connection.State == HubConnectionState.Connected)
        {
            try
            {
                await connection.InvokeAsync(methotName, msg);
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        else
        {

            Debug.Log("Connection Error");
            if (!tryConnect)
                Connect();
        }
    }

    private void SetPosition(int x)
    {
        //this.x = x;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Send(Random.Range(0, 5000).ToString());
        }
    //transform.position = new Vector2(x, 0);
    }
}


public class ModelSerializer
{

    public static string Model2String<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
    public static T String2Model<T>(string obj)
    {
        return JsonConvert.DeserializeObject<T>(obj);
    }
}