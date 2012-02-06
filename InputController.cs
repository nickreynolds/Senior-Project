/*
 * Input Controller.cs - applies Kinect data to game
 *      
 * 
 * 
 *      Based on KinectModelController.cs - Moves every 'bone' given to match
 * 				the position of the corresponding bone given by
 * 				the kinect. Useful for viewing the point tracking
 * 				in 3D.
 * 
 * 		    Developed by Peter Kinney -- 6/30/2011
 * - Developed By Nick Reynolds
 */

using UnityEngine;
using System;
using System.Collections;

public class InputController : MonoBehaviour
{

    public KinectWrapper KinectBridge;

    public GameObject Hip_Center;
    public GameObject Spine;
    public GameObject Shoulder_Center;
    public GameObject Head;
    public GameObject Shoulder_Left;
    public GameObject Elbow_Left;
    public GameObject Wrist_Left;
    public GameObject Hand_Left;
    public GameObject Shoulder_Right;
    public GameObject Elbow_Right;
    public GameObject Wrist_Right;
    public GameObject Hand_Right;
    public GameObject Hip_Left;
    public GameObject Knee_Left;
    public GameObject Ankle_Left;
    public GameObject Foot_Left;
    public GameObject Hip_Right;
    public GameObject Knee_Right;
    public GameObject Ankle_Right;
    public GameObject Foot_Right;

    private GameObject[] _bones; //internal handle for the bones of the model
    //private Vector4[] _bonePos; //internal handle for the bone positions from the kinect

    public int player;
    public BoneMask Mask;

    public float MIN_TRANSLATION_RANGE = .08f;
    public float MAX_TRANSLATION_RANGE = .30f;
    public float MIN_AIM_RANGE = .075f;

    public GameObject Player;
    public PlayerScript script_Player;

    public int timer = 300;
    public float translation_distance = 0.0f;
    public float aiming_distance = 0.0f;

    public Vector3 aiming_nominalPointRightHandFromHips;
    public Vector3 aiming_nominalPointLeftHandFromHips;

    public Vector2 aim;
    public Vector2 translation;
    public Vector2 nominalMouseAim;
    public Vector2 mouseAim;

    public Boolean rightHanded = true;

    public int controllerType = 1;
    public Boolean isDebug = true;
    Vector3 normalMousePosition;
    Vector3 currentMousePosition;
    public int secondaryFireTimer = -1;
    public int primaryFireTimer = -1;
    public int raisePRISMSTimer = -1;

    Boolean pressedRight, pressedLeft, pressedUp, pressedDown, fireSecondary, firePrimary, pressed1, pressedJump;

    // Use this for initialization
    void Start()
    {
        //store bones in a list for easier access
        _bones = new GameObject[(int)BoneIndex.Num_Bones] {Hip_Center, Spine, Shoulder_Center, Head,
			Shoulder_Left, Elbow_Left, Wrist_Left, Hand_Left,
			Shoulder_Right, Elbow_Right, Wrist_Right, Hand_Right,
			Hip_Left, Knee_Left, Ankle_Left, Foot_Left,
			Hip_Right, Knee_Right, Ankle_Right, Foot_Right};
        //_bonePos = new Vector4[(int)BoneIndex.Num_Bones];
        Player = GameObject.FindGameObjectWithTag("Player");
        script_Player = Player.GetComponent(typeof(PlayerScript)) as PlayerScript;
        pressedRight = pressedLeft = pressedUp = pressedDown = false;
        fireSecondary = false;
        firePrimary = false;
        pressed1 = false;
        pressedJump = false;
    }

    // Update is called once per frame
    void Update()
    {
        //update all of the bones positions
        //KinectBridge.pollKinect();
        //for (int ii = 0; ii < (int)BoneIndex.Num_Bones; ii++)
        //{
        //    //_bonePos[ii] = KinectBridge.getBonePos(ii);
        //    if (((uint)Mask & (uint)(1 << ii)) > 0)
        //    {
        //        _bones[ii].transform.localPosition = KinectBridge.BonePos[player, ii];
        //    }
        //}

        if (Input.GetKey("a"))
        {
            pressedLeft = true;
        }
        if (Input.GetKey("d"))
        {
            pressedRight = true;
        }
        if (Input.GetKey("w"))
        {
            pressedUp = true;
        }
        if (Input.GetKey("s"))
        {
            pressedDown = true;
        }
        if (Input.GetMouseButton(1) && secondaryFireTimer < 0)
        {
            fireSecondary = true;
        }
        if (Input.GetMouseButton(0) && primaryFireTimer < 0)
        {
            firePrimary = true;
        }
        if (Input.GetKey("1") && raisePRISMSTimer < 0)
        {
            pressed1 = true;
        }

        bool grounded = false;
        if (Physics.Raycast(Player.transform.position, new Vector3(0.0f,-1.0f,0.0f), 2.0f))
        {
            grounded = true;
        }
        if (Input.GetKey("space"))
        {
            Debug.Log("pressed space bar");
            if (grounded)
            {
                Debug.Log("grounded");
                pressedJump = true;
            }
        }

    }

    void FixedUpdate()
    {
        //keep track of when to do start-of-game-events
        timer++;

        if (controllerType == 1)
        {
            //make all positions local
            if (timer == 500)
            {
                //nominalMouseAim = new Vector2(Input.mousePosition[0], Input.mousePosition[1]);
                if (rightHanded)
                {
                    aiming_nominalPointRightHandFromHips = Hand_Right.transform.localPosition - Hip_Center.transform.localPosition;
                }
                else
                {
                    aiming_nominalPointLeftHandFromHips = Hip_Left.transform.localPosition - Hip_Center.transform.localPosition;
                }
            }
            else if (timer > 550)
            {
                //mouseAim = new Vector2(Input.mousePosition[0], Input.mousePosition[1]) - nominalMouseAim;
                //find translation
                Vector3 translationDifference = Shoulder_Center.transform.localPosition - Hip_Center.transform.localPosition;
                Vector3 translationEffectiveDifference = translationDifference;
                translationEffectiveDifference[1] = 0.0f;
                translation_distance = translationEffectiveDifference.magnitude;
                translation = new Vector2(translationEffectiveDifference[0], translationEffectiveDifference[2]);
                if (translation_distance < MIN_TRANSLATION_RANGE)
                {
                    //movement too small, apply no translation
                    Vector3 empty = new Vector3(0.0f, 0.0f, 0.0f);
                    script_Player.applyTranslation(empty);
                }
                else if (translation_distance < MAX_TRANSLATION_RANGE)
                {
                    //movement in range, apply scalar multiple of it in XZ plane
                    Vector3 translationPointOhEight = translationEffectiveDifference.normalized * .08f;
                    translationEffectiveDifference -= translationPointOhEight;
                    script_Player.applyTranslation(3.0f*translationEffectiveDifference);
                }
                else
                {
                    //movement out of range, apply max translation in XZ plane
                    translationEffectiveDifference.Normalize();
                    translationEffectiveDifference *= .30f;
                    Vector3 translationPointOhEight = translationEffectiveDifference.normalized * .08f;
                    translationEffectiveDifference -= translationPointOhEight;
                    script_Player.applyTranslation(3.0f*translationEffectiveDifference);
                }

                //aiming
                Vector3 deltaAim = new Vector3(0.0f, 0.0f, 0.0f);
                if (rightHanded)
                {
                    Vector3 relativeHandPosition = Hand_Right.transform.localPosition - Hip_Center.transform.localPosition;
                    deltaAim = relativeHandPosition - aiming_nominalPointRightHandFromHips;
                    aiming_distance = deltaAim.magnitude;
                }
                else
                {
                    Vector3 relativeHandPosition = Hand_Left.transform.localPosition - Hip_Center.transform.localPosition;
                    deltaAim = relativeHandPosition - aiming_nominalPointLeftHandFromHips;
                    aiming_distance = deltaAim.magnitude;
                }

                Vector2 dAim = new Vector3(deltaAim[0], deltaAim[1]);
                aim = dAim;
                if (aiming_distance > MIN_AIM_RANGE)
                {
                    script_Player.applyAim(dAim);
                }

            }
        }
        else if (controllerType == 2)
        {
            if (timer == 500)
            {
                //Screen.lockCursor = true;
                //normalMousePosition = Input.mousePosition;
            }
            else if (timer > 500)
            {
                //currentMousePosition = Input.mousePosition;
                //Vector3 deltaAim = currentMousePosition - normalMousePosition;
                //Vector2 dAim = new Vector2(deltaAim[0], deltaAim[1]);
                //script_Player.applyAim(.001f*dAim);
                //Input.mousePosition.
                Vector3 translate = new Vector3(0.0f, 0.0f, 0.0f);
                if (pressedRight)
                {
                    translate += new Vector3(0.30f, 0.0f, 0.0f);
                }
                if (pressedLeft)
                {
                    translate += new Vector3(-0.30f, 0.0f, 0.0f);
                } 
                if (pressedUp)
                {
                    translate += new Vector3(0.0f, 0.0f, 0.3f);
                }
                if (pressedDown)
                {
                    translate += new Vector3(0.0f, 0.0f, -0.3f);
                }
                secondaryFireTimer--;
                if (secondaryFireTimer < -1000)
                    secondaryFireTimer = -5;
                if (fireSecondary)
                {
                    secondaryFireTimer = 50;
                    Debug.Log("Firing secondary");
                    script_Player.fireSecondary();
                }

                primaryFireTimer--;
                if (primaryFireTimer < -1000)
                    primaryFireTimer = -5;
                if (firePrimary)
                {
                    primaryFireTimer = 50;
                    script_Player.firePrimary();
                }
                pressedRight = pressedLeft = pressedUp = pressedDown = false;
                firePrimary = fireSecondary = false;
                script_Player.applyTranslation(translate);
                script_Player.applyAim();

                raisePRISMSTimer--;
                if (raisePRISMSTimer < -1000)
                    raisePRISMSTimer = -5;
                if (pressed1)
                {
                    raisePRISMSTimer = 100;
                    script_Player.raisePrisms(5.0f);
                }
                pressed1 = false;

                if (pressedJump)
                {
                    Debug.Log("apply jump force");
                    Player.rigidbody.AddForce(new Vector3(0.0f, 100.0f, 0.0f));
                }
                pressedJump = false;
            }
        }
    }

    void OnGUI()
    {
        if (controllerType == 1 && isDebug)
        {
            GUI.Label(new Rect(400, 100, 300, 30), "KINECT AIMING - X: " + aim[0] + " - Y: " + aim[1]);
            GUI.Label(new Rect(400, 130, 300, 30), "KINECT TRANSL - Z: " + translation[0] + " - Z: " + translation[1]);
            GUI.Label(new Rect(400, 160, 300, 30), "KINECT TRANSL LENGTH: " + translation_distance);
        }
        else if (controllerType == 2 && isDebug)
        {
            GUI.Label(new Rect(400, 190, 300, 30), "MOUSE AIMING - X: " + mouseAim[0] + " - Y: " + mouseAim[1]);
        }
    }
}
