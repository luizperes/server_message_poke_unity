using UnityEngine;
using System.Collections;

public class Server
{
    public const string address = "127.0.0.1";
    public const int port = 25001;
    public const int maxConnections = 10;
}

public class PlayerData
{
    public string name;
    public NetworkPlayer player;
}