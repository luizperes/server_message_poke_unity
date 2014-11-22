using UnityEngine;
using System.Collections;

public class Connect : MonoBehaviour {

    const float sizeWBox = 200;
    const float sizeHBox = 20;
    const int nElementH = 2;

    public string userName = string.Empty;

    void Start()
    {
        var useNat = !Network.HavePublicAddress();
        // this "if" prevents Unity from creating a server other than the specified.
        if (Server.address == "127.0.0.1" || Server.address == Network.player.ipAddress)
        {
            Network.InitializeServer(Server.maxConnections, Server.port, useNat);
            if (Network.isServer)
                Application.LoadLevel("chatroom");
        }
    }

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width / 2  - 150, Screen.height / 2 - 100, 300, 150), "");

        GUILayout.BeginHorizontal();
        userName = GUI.TextField(new Rect(getLeftPos(), getTopPos(1), sizeWBox, sizeHBox), userName, 25);
        //Server.lastName = userName;
        PlayerPrefs.SetString("userName", userName);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUI.Button(new Rect(getLeftPos(), getTopPos(2), sizeWBox, sizeHBox), "Log in to server"))
        {
            Network.Connect(Server.address, Server.port);
            Application.LoadLevel("chatroom");
        }
            
        GUILayout.EndHorizontal();
    }

    float getLeftPos()
    {
        return (Screen.width / 2) - (sizeWBox / 2);
    }

    float getTopPos(int currentElementNumber)
    {
        var posIni = (Screen.height / 2) - ((sizeHBox * nElementH) + (10 * (nElementH - 1)));
        return posIni + 30 * (currentElementNumber - 1);
    }


}
