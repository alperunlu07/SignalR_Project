using Microsoft.AspNetCore.SignalR.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;


public class ServerController : MonoSingleton<ServerController>
{
    private static HubConnection connection;
    private bool tryConnect = false;
    public List<User> users = new List<User>();
    public List<string> clients = new List<string>();

    

    private void Awake()
    {
        DontDestroyOnLoad(this);

    }
    void Start()
    {
        connection = new HubConnectionBuilder()
                    .WithUrl("https://localhost:5001/signalr") 
                    .Build();
        InitServerMethots();
        Connect();        
    }
    private void InitServerMethots()
    {
        connection.Closed += async (error) =>
        {
            Debug.Log("connection closed");
            await Task.Delay(Random.Range(0, 5) * 1000);
            await connection.StartAsync();
        };
        connection.On<string>("receiveMessage", (message) =>
        {
            Debug.Log($": {message}");
        });
        connection.On<string>("broadcastMessage", (message) =>
        {
            Debug.Log($": {message}");
        });
        connection.On<string>("UserJoined", (message) =>
        {
            //NewUser(message);
            Debug.Log($": {message}");
        });
        connection.On<string>("AllUserId", (message) =>
        {
            Debug.Log($": {message}");
            //AllUser(message);
        });
        connection.On<string>("UserLeaved", (message) =>
        {
            //RemoveUser(message);
            Debug.Log($": {message}");
        });
        connection.On<string>("Ping", (message) =>
        {
            //pingReq = message; 
            Ping.Instance.Tick(message);
        });
    }

    private void NewUserId(string obj)
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

    public async void Send(string msg)
    {
        if (connection.State == HubConnectionState.Connected)
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
            if (!tryConnect)
                Connect();
        }
    }
    public async void SendWithMethot(string methotName, string msg)
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


}
