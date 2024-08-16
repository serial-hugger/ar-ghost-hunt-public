using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBoardSign : MonoBehaviour
{
    Controller controlScript;
    public GameObject detection;
    public GameObject flyingDetection;
    public GameObject flyingBoard;
    bool started = false;
    Waypoint myWaypoint;
    public Transform aiming;
    public Transform propeller;

    public bool flyingTarget;
    private Camera _camera;

    // Start is called before the first frame update
    private void Start()
    {
        _camera = Camera.main;
        controlScript = GameObject.Find("Controller").GetComponent<Controller>();
        myWaypoint = controlScript.GetClosestWaypoint(transform.position,controlScript.waypoints);
    }

    // Update is called once per frame
    private void Update()
    {
        if (flyingTarget) {
            flyingBoard.SetActive(true);
            flyingBoard.transform.LookAt(_camera.transform.position);
            Vector3 flyRot = flyingBoard.transform.eulerAngles;
            flyRot.x = -180f;
            flyRot.z = 0f;
            flyingBoard.transform.eulerAngles = flyRot;

            int layerMask = LayerMask.GetMask("ARMesh");
            RaycastHit hit;
            if (Physics.Raycast(new Ray(_camera.transform.position, aiming.GetChild(0).forward), out hit, 10f, layerMask))
            {
                flyingBoard.transform.position = Vector3.Lerp(flyingBoard.transform.position, _camera.transform.position+aiming.GetChild(0).forward*(hit.distance*0.5f),2.5f*Time.deltaTime);
            }
            else
            {
                flyingBoard.transform.position = Vector3.Lerp(flyingBoard.transform.position, _camera.transform.position + aiming.GetChild(0).forward * (10f * 0.5f), 2.5f * Time.deltaTime);
            }
        }
        else
        {
            flyingBoard.transform.position = Vector3.Lerp(flyingBoard.transform.position,transform.position-(Vector3.up*2f),2.5f*Time.deltaTime);
            if (flyingBoard.transform.localPosition.y<=-1f) {
                flyingBoard.SetActive(false);
            }
        }

        if (Tutorial.tutorial[Tutorial.tutorialPhase].mouseFollow == "boardfly")
        {
            flyingTarget = true;
            detection.SetActive(false);
        }
        else
        {
            flyingTarget = false;
        }

        //SET ROTATION OF PROPELLER
        Vector3 tempRot = propeller.transform.localEulerAngles;
        tempRot.z += 1000f * Time.deltaTime;
        propeller.transform.localEulerAngles = tempRot ;
        //SET ROTATION OF THE AIMER
        if (flyingTarget) {
            tempRot = aiming.transform.eulerAngles;
            tempRot.y += 25f * Time.deltaTime;
            aiming.transform.eulerAngles = tempRot;
        }

        transform.LookAt(controlScript.toolDeskScript.transform.position);
        Vector3 newRot = transform.eulerAngles;
        newRot.x = 0f;
        newRot.z = 0f;
        transform.eulerAngles = newRot;
        transform.position = myWaypoint.floor;
        if (Tutorial.tutorial[Tutorial.tutorialPhase].advanceAction=="evidenceghostbody")
        {
            started = true;
        }
        if (started)
        {
            if (Tutorial.tutorial[Tutorial.tutorialPhase].advanceAction != "evidenceghostbody")
            {
                GameObject.Destroy(detection);
            }
        }
    }
}
