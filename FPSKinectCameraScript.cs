using UnityEngine;
using System.Collections;

public class FPSKinectCameraScript : MonoBehaviour {

    public PlayerScript script_player;
    public GameObject player;
	// Use this for initialization

    public GameObject KinectAvatar;
    public InputController script_kinect;

	void OnNetworkLoadedLevel () {
        player                      = GameObject.FindGameObjectWithTag("Player");
        script_player               = player.GetComponent(typeof(PlayerScript)) as PlayerScript;

        KinectAvatar                = GameObject.FindGameObjectWithTag("KinectAvatar");
        script_kinect               = KinectAvatar.GetComponent(typeof(InputController)) as InputController;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (player != null && script_player != null)
        {
            this.transform.position = player.transform.position + player.transform.up*2.0f;
            this.transform.rotation = script_player.getCameraRotation();
        }
	}

    void OnGUI()
    {
        if (player != null && script_player != null)
        {
            if (script_kinect.timer < 100)
            {
                GUI.Label(new Rect(100, 100, 600, 600), "5");
            }
            else if (script_kinect.timer < 200)
            {
                GUI.Label(new Rect(100, 100, 600, 600), "4");
            }
            else if (script_kinect.timer < 300)
            {
                GUI.Label(new Rect(100, 100, 600, 600), "3");
            }
            else if (script_kinect.timer < 400)
            {
                GUI.Label(new Rect(100, 100, 600, 600), "2");
            }
            else if (script_kinect.timer < 500)
            {
                GUI.Label(new Rect(100, 100, 600, 600), "1");
            }
            else if (script_kinect.timer < 550)
            {
                GUI.Label(new Rect(100, 100, 600, 600), "GO");
            }
        }
    }
}
