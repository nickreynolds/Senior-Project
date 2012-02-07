using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour {

    Transform playerTransform, cameraTransform;
    Vector3 lookAT;
    float speed = 0.0f;
    float forceScalar = .50f;
    float aimScalar = 3.0f;

    float rotationY = 0.0f;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    public float sensitivityX = 1.0f;
    public float sensitivityY = 1.0f;

    public int ID = -1;
    public NetworkViewID netid;
    public string theID;

    public List<ManipulatableScript> ownedPrisms;
    public SecondaryProjectileScript secondaryTemplate;
    public NetworkView thisView;

    public Color thisPlayerColor;
    public List<Vector3> allColors;
    public FPSKinectCameraScript camera_script;

	// Use this for initialization
    void OnNetworkInstantiate(NetworkMessageInfo info) 
    {
        thisPlayerColor = Color.black;
        ownedPrisms = new List<ManipulatableScript>();
        lookAT = new Vector3(0.0f, 0.0f, 1.0f);
        playerTransform = cameraTransform = this.transform;

        //deal with this when adding multiple player support
        //thisView = this.gameObject.GetComponent(typeof(NetworkView)) as NetworkView;
        netid = thisView.viewID;
        theID = netid.ToString();
        char[] space = {' '};
        int tempIndex = theID.IndexOfAny(space);
        theID = theID.Substring(tempIndex + 1);
        int tempID = int.Parse(theID);
        setID(tempID);
        //GameObject secondary = GameObject.FindGameObjectWithTag("Secondary");
        //secondaryTemplate = secondary.GetComponent(typeof(SecondaryProjectileScript)) as SecondaryProjectileScript;
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        camera_script = camera.GetComponent(typeof(FPSKinectCameraScript)) as FPSKinectCameraScript;
       
	}

    public void setID(int id)
    {
        ID = id;
    }

    [RPC]
    public void setColor(Vector3 v)
    {
        Color c = new Color(v.x, v.y, v.z, 1);
        thisPlayerColor = c;
    }

    public Color getColor()
    {
        return thisPlayerColor;
    }
	// Update is called once per frame
	void Update () 
    {
        transform.position = playerTransform.position;
        transform.rotation = playerTransform.rotation;

        if (thisPlayerColor == Color.black)
        {
            allColors = new List<Vector3>();
            allColors.Add(new Vector3(Color.blue.r, Color.blue.g, Color.blue.b));
            allColors.Add(new Vector3(Color.green.r, Color.green.g, Color.green.b));
            allColors.Add(new Vector3(Color.magenta.r, Color.magenta.g, Color.magenta.b));
            allColors.Add(new Vector3(Color.red.r, Color.red.g, Color.red.b));
            allColors.Add(new Vector3(Color.blue.r, Color.blue.g, Color.blue.b));
            allColors.Add(new Vector3(Color.blue.r, Color.blue.g, Color.blue.b));

            GameObject[] taggedAsPlayers = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < taggedAsPlayers.Length; i++)
            {
                PlayerScript ps = taggedAsPlayers[i].GetComponent(typeof(PlayerScript)) as PlayerScript;
                if (ps.getID() != ID)
                {
                    Vector3 v = new Vector3(ps.getColor().r, ps.getColor().g, ps.getColor().b);
                    allColors.Remove(v);
                }
            }
            Vector3 c = allColors[0];
            thisView.RPC("setColor", RPCMode.AllBuffered, c);
        }
	}

    public void applyTranslation(Vector3 force)
    {
        //this.rigidbody.
        //this.rigidbody.AddRelativeForce(forceScalar*force);  
        //this.rigidbody.velocity = this.transform.rotation * force;
        //this.rigidbody.velocity.y = 0.0f;
        playerTransform.position += playerTransform.rotation * force * forceScalar;
        cameraTransform.position += playerTransform.rotation * force * forceScalar;
    }

    public void applyAim(Vector2 aim)
    {
        float rotationX = transform.localEulerAngles.y + aim[0] * aimScalar;
        rotationY += aim[1] * aimScalar;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
        cameraTransform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

        //Debug.Log("Aim: " + aim[0] + ", " + aim[1]);
        playerTransform.Rotate(0, aim[0] * aimScalar, 0);
    }

    public void applyAim()
    {
        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        cameraTransform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

        //Debug.Log("Aim: " + Input.GetAxis("Mouse X") + ", " + Input.GetAxis("Mouse Y"));
        
        playerTransform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
    }

    public Quaternion getCameraRotation()
    {
        return cameraTransform.rotation;
    }

    public int getID()
    {
        return ID;
    }

    [RPC]
    public void disownAllPrisms(int nothing)
    {
        foreach (ManipulatableScript ms in ownedPrisms)
        {
            ms.makeNotOwned();
        }
        ownedPrisms.Clear();
    }

    public void setManipulatableAsOwned(ManipulatableScript ms)
    {
        ownedPrisms.Add(ms);
    }

    public void setManipulatableAsNotOwned(ManipulatableScript ms)
    {
        ownedPrisms.Remove(ms);
    }

    public void raisePrisms(float height)
    {
        foreach(ManipulatableScript ms in ownedPrisms)
        {
            NetworkView prismView = ms.gameObject.GetComponent(typeof(NetworkView)) as NetworkView;
            prismView.RPC("raiseAPrism", RPCMode.AllBuffered, height);
            
        }
    }

    public void OnDisconnectedFromServer()
    {
        Network.Destroy(this.gameObject);
    }




    public void fireSecondary()
    {
        //Debug.Log("PLAYER FIRING SECONDARY");
        SecondaryProjectileScript sps;
        //sps = Instantiate(secondaryTemplate, this.transform.position, this.transform.rotation) as SecondaryProjectileScript;
        //secondaryTemplate
        sps = Instantiate(secondaryTemplate, camera_script.getTransform().position, camera_script.getTransform().rotation) as SecondaryProjectileScript;
        sps.reinitialize();
        sps.setLifeTime(500);
        sps.setOwner(this);
    }

    public void firePrimary()
    {
    }

    
}
