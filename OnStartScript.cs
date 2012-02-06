using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OnStartScript : MonoBehaviour 
{

    ManipulatableScript ms;
    public GameObject prism;
    public GameObject player;
    List<ManipulatableScript> prismScripts;

	// Use this for initialization
	void OnNetworkLoadedLevel () 
    {
        prismScripts = new List<ManipulatableScript>();
        int index = 0;
        //prism = GameObject.FindGameObjectWithTag("Manipulatable");
        ms = prism.GetComponent(typeof(ManipulatableScript)) as ManipulatableScript;
        //if (Network.isServer)
        //{
            //for (int i = 0; i < 10; i++)
            //{
            //    for (int j = 0; j < 10; j++)
            //    {
            //        ManipulatableScript prismScript;
            //        prismScript = Instantiate(ms, new Vector3(-45.0f + (10.0f * i), -25.0f, -45.0f + (10.0f * j)), prism.transform.rotation) as ManipulatableScript;
            //        prismScript.setID(index);
            //        prismScripts.Add(prismScript);
            //        index++;
            //    }
           // }
            //Destroy(prism);
        //}

        Network.Instantiate(player, new Vector3(0.0f, 40.0f, 0.0f), Quaternion.identity, 1);
        
        //player.setID(
	}

   

    // Update is called once per frame
    void Update()
    {
	
	}
}
