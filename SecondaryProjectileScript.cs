using UnityEngine;
using System.Collections;

public class SecondaryProjectileScript : MonoBehaviour 
{

    public Vector3 deltaPos;
    public int ownerID = -1;
    public int ID = -1;
    public PlayerScript owner;
    public int lifetime = -1;
	// Use this for initialization
	void Start () 
    {
        //lifetime = 500;
        deltaPos = new Vector3(0.0f, 0.0f, 1.0f);
        deltaPos = transform.rotation*deltaPos;
        deltaPos = .3f * deltaPos;

        transform.position += 10 * deltaPos;
        //Debug.Log("SECONDARY CREATED @: " + transform.position[0] + ", " + transform.position[1] + ", " + transform.position[2]);
	}

    public void reinitialize()
    {
        deltaPos = new Vector3(0.0f, 0.0f, 1.0f);
        deltaPos = transform.rotation * deltaPos;
        deltaPos = .3f * deltaPos;

        transform.position += 10 * deltaPos;
        //Debug.Log("SECONDARY REINITIALIZED @: " + transform.position[0] + ", " + transform.position[1] + ", " + transform.position[2]);
    }

    public void setLifeTime(int life)
    {
        lifetime = life;
    }
    public void setDeltaPos(Vector3 dPos)
    {
        deltaPos = dPos;
    }

    public void setOwnerID(int id)
    {
        ownerID = id;
    }

    public void setID(int id)
    {
        ID = id;
    }

    public void setOwner(PlayerScript p)
    {
        owner = p;
    }
	
	// Update is called once per frame
	void Update () 
    {
        
	}

    void FixedUpdate()
    {
        transform.position += deltaPos;
        if (lifetime > 0)
            lifetime--;
        if (lifetime == 0)
            Destroy(this.gameObject);
    }

    //check if hit PRISM, if so, set prism owner ID and add prism to the player's list of prisms. May have to remove it from other player's prisms
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Manipulatable")
        {
            ManipulatableScript ms = other.gameObject.GetComponent(typeof(ManipulatableScript)) as ManipulatableScript;
            NetworkView prismView = ms.gameObject.GetComponent(typeof(NetworkView)) as NetworkView;
            prismView.RPC("setManipulatableOwnerID", RPCMode.AllBuffered, owner.getID());   
                //setManipulatableOwnerID(owner.getID());

            //do this in ManipulatableScript.setOwner(PlayerScript p)
            //owner.setManipulatableAsOwned(ms);
        }
        //Debug.Log("SECONDARY DESTROYED BY: " + other.tag);
        Destroy(this.gameObject);
    }
}
