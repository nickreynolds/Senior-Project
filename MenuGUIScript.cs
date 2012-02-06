using UnityEngine;
using System.Collections;

public class MenuGUIScript : MonoBehaviour
{
    bool serverStarted = false;
    //MasterServer master;
    // Use this for initialization
    int timer = 0;
    void Start()
    {
        //master = new MasterServer();
        //MasterServer.ipAddress = "192.168.1.43";
        //MasterServer.port = 23466;
        //MasterServer.
        //MasterServer.updateRate = 100;
        //MasterServer.RequestHostList("regular");
    }

    void Awake()
    {
        //MasterServer.RequestHostList("regular");
    }



    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {

    }

    void OnGUI()
    {
        if (!serverStarted)
        {
            if (GUI.Button(new Rect(30, 30, 150, 50), "Start a Server"))
            {
                serverStarted = true;
                Network.InitializeServer(32, 25002, !Network.HavePublicAddress());
                //MasterServer.RegisterHost("regular", "nick", "test");
                Application.LoadLevel(1);
            }
        }
        if (GUI.Button(new Rect(30, 80, 150, 50), "Join Local Server"))
        {
            Network.Connect("127.0.0.1", 25002);
            Application.LoadLevel(1);
        }

            //HostData[] data = MasterServer.PollHostList();
            //// Go through all the hosts in the host list
            //foreach (HostData element in data)
            //{
            //    GUILayout.BeginHorizontal();
            //    var name = element.gameName + " " + element.connectedPlayers + " / " + element.playerLimit;
            //    GUILayout.Label(name);
            //    GUILayout.Space(5);
            //    string hostInfo;

            //    hostInfo = "[";
            //    foreach (string host in element.ip)
            //        hostInfo = hostInfo + host + ":" + element.port + " ";
            //    hostInfo = hostInfo + "]";
            //    GUILayout.Label(hostInfo);
            //    GUILayout.Space(5);
            //    GUILayout.Label(element.comment);
            //    GUILayout.Space(5);
            //    GUILayout.FlexibleSpace();
            //    if (GUILayout.Button("Connect"))
            //    {
            //        // Connect to HostData struct, internally the correct method is used (GUID when using NAT).
            //        Network.Connect(element);
            //    }
            //    GUILayout.EndHorizontal();
            //}
        
    }
}
