using UnityEngine;
using System.Collections;

public class ManipulatableScript : MonoBehaviour {

    public float desiredHeight;
    public float MIN_ALLOWED_HEIGHT = -25.0f;
    public float MAX_ALLOWED_HEIGHT = 25.0f;
    public int ownerID = -1;
    public int ID;
    public PlayerScript owner;
    NetworkView theView;

	// Use this for initialization
	void Start () 
    {
        desiredHeight = transform.position[1];
        theView = this.gameObject.GetComponent(typeof(NetworkView)) as NetworkView;
	}

    public void setID(int id)
    {
        ID = id;
    }

    [RPC]
    public void setManipulatableOwner(PlayerScript p)
    {
        if (p.getID() == ownerID)
        {
            return;
        }
        if(owner != null)
            owner.setManipulatableAsNotOwned(this);
        owner = p;
        ownerID = p.getID();
        owner.setManipulatableAsOwned(this);

        this.renderer.material.color = owner.getColor();
    }

    [RPC]
    public void setManipulatableOwnerID(int id)
    {
        Debug.Log("setting prism owner id: " + id);
        ownerID = id;
        GameObject[] taggedAsPlayers = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("number of players: " + taggedAsPlayers.Length);
        for (int i = 0; i < taggedAsPlayers.Length; i++)
        {
            PlayerScript ps = taggedAsPlayers[i].GetComponent(typeof(PlayerScript)) as PlayerScript;
            if (ps.getID() == ownerID)
                owner = ps;
        }
        owner.setManipulatableAsOwned(this);
        this.renderer.material.color = owner.getColor();
    }
    // Update is called once per frame
    void Update()
    {
	
	}

    void FixedUpdate()
    {
        float heightDifference = transform.position[1] - desiredHeight;
        if (Mathf.Abs(heightDifference) > .05f)
        {
            if (heightDifference > 0)
            {
                transform.position += new Vector3(0.0f, -0.05f, 0.0f);
            }
            else
            {
                transform.position += new Vector3(0.0f, 0.05f, 0.0f);
            }
        }
    }

    public void addDesiredHeight(float h)
    {
        desiredHeight += h;
        if (desiredHeight > MAX_ALLOWED_HEIGHT)
            desiredHeight = MAX_ALLOWED_HEIGHT;
        else if (desiredHeight < MIN_ALLOWED_HEIGHT)
            desiredHeight = MIN_ALLOWED_HEIGHT;
    }

    //public void networkRaiseHeight(float h)
    //{
    //    theView.RPC("raiseAPrism", RPCMode.All, 
    //}

    [RPC]
    public void raiseAPrism(float height)
    {
        addDesiredHeight(height);
    }
}
