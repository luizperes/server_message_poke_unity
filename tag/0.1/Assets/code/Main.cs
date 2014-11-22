using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Main : MonoBehaviour 
{
    private string txtMessage = string.Empty;
    private bool sentName = false;
    private PlayerData me = new PlayerData();
    public List<PlayerData> lstPlayers = new List<PlayerData>();
    public StringBuilder txtArea = new StringBuilder();

    public enum MESSAGE_TYPE
    {
        MT_POKE = 0,
        MT_MESSAGE = 1
    };

	// Use this for initialization
	void Start () 
    {
        
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnGUI()
    {   
        if (Network.peerType == NetworkPeerType.Client)
        {
            if (!sentName)
            {
                me.name = string.IsNullOrEmpty(PlayerPrefs.GetString("userName")) ? "(no name)" : PlayerPrefs.GetString("userName");
                me.player = Network.player;
                networkView.RPC("setNewUser", RPCMode.Server, me.name, me.player);
                sentName = true;
            }

            this.handleClientUI();
        }
        else if (Network.peerType == NetworkPeerType.Server)
        {
            this.handleServerUI();
        }
    }

    private void handleServerUI()
    {
        GUI.Label(new Rect(0, 0, 50, 20), "Server");
        GUI.Label(new Rect(0, 30, 20, 20), "" + Network.connections.Length);
    }

    private void handleClientUI()
    {
        GUI.Box(new Rect(50, 10, 200, Screen.height - 100), "");
        GUI.Box(new Rect(300, 10, Screen.width - 310, Screen.height - 100), "");

        if (GUI.Button(new Rect(50, Screen.height - 70, 200, 30), "POKE!!!"))
            networkView.RPC("sendMessage", RPCMode.Server, (int)MESSAGE_TYPE.MT_POKE, string.Empty, me.player);

        txtMessage = GUI.TextField(new Rect(300, Screen.height - 70, Screen.width - 410, 30), txtMessage);

        if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 70, 90, 30), "SEND"))
            networkView.RPC("sendMessage", RPCMode.Server, (int)MESSAGE_TYPE.MT_MESSAGE, txtMessage, me.player);

        for (int i = 0; i < lstPlayers.Count; i++)
            GUI.Label(new Rect(60, 20 + (30 * i), 100, 20), lstPlayers[i].name);

        GUI.TextArea(new Rect(310, 20, Screen.width - 330, Screen.height - 120), txtArea.ToString());
    }

    // Server
    [RPC]
    void setNewUser(string n, NetworkPlayer p)
    {
        networkView.RPC("createUser", RPCMode.OthersBuffered, n, p);
    }

    [RPC]
    void sendMessage(int type, string msg, NetworkPlayer p)
    {
        networkView.RPC("broadcastMessage", RPCMode.Others, type, msg, p);
    }

    // Client
    [RPC]
    void createUser(string n, NetworkPlayer p)
    {
        lstPlayers.Add(new PlayerData { name = n, player = p });

        Debug.Log("create user");
        for (int i = 0; i < lstPlayers.Count; i++)
            Debug.Log(lstPlayers[i]);
    }

    [RPC]
    void broadcastMessage(int type, string msg, NetworkPlayer p)
    {
        PlayerData tempUser = lstPlayers.Find(x => x.player.ToString() == p.ToString());
        Debug.Log("broadcast name: " + tempUser.name +" player:" +tempUser.player + " msg:" + msg);

        if (type == (int)MESSAGE_TYPE.MT_MESSAGE)
        {
            txtArea.AppendFormat("{0}: {1}\n", tempUser.name, msg);
            txtMessage = string.Empty;
        }
        else if (type == (int)MESSAGE_TYPE.MT_POKE)
        {
            if (me.player != tempUser.player)
            {
                txtArea.AppendFormat("{0} poked you.\n", tempUser.name);
            }
        }
    }

    void OnPlayerConnected(NetworkPlayer player)
    {
        Debug.Log("Player " + me.name + ", " + me.player+ " connected from " + player.ipAddress + ":" + player.port);
    }

    void OnServerInitialized()
    {
        Debug.Log("Server initialized and ready");
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        Debug.Log("Could not connect to server: " + error);
    }
}
