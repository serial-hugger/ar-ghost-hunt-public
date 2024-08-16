using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class Waypoint : MonoBehaviour
{

    public Waypoint myClosest;

    public Controller controlScript;
    //distance from starting node
    public float gCost = 0.0f;
    //distance from ending node
    public float hCost = 0.0f;
    //gcost + hcost
    public float fCost = 0.0f;
    //the node that got this node to be checked
    public Waypoint openerNode;

    public Vector3 floor;
    public GameObject floorSphere;

    public GameObject handprintPrefab;
    public Handprint myHandprint;
    public bool placedHandprint;
    public Vector3 placedDirection;

    public GameObject itemPrefab;
    public CursedItem myItem;
    public bool placedItem;

    public float timeTillUpdate;

    public bool isMaster;

    public float updateFrequency;

    // Start is called before the first frame update
    void Start()
    {
        placedDirection = Random.insideUnitCircle.normalized;
        if (isMaster) {
            CreateAnchor();
        }
        myClosest = controlScript.GetClosestWaypoint(transform.position,controlScript.waypoints);
        updateFrequency = controlScript.saveScript.settingData.waypointUpdateFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        if (controlScript.testMode)
        {
            floorSphere.SetActive(true);
        }
        else
        {
            floorSphere.SetActive(false);
        }
        timeTillUpdate -= 1f * Time.deltaTime;
        if (timeTillUpdate<=0f) {
            timeTillUpdate += updateFrequency;
            GameObject ghost = GameObject.FindGameObjectWithTag("GhostEntity");
            if (Vector3.Distance(transform.position, Camera.main.transform.position) <= 5f || (ghost&& Vector3.Distance(transform.position, ghost.transform.position)<=5f))
            {
                SetFloor();
                if (placedHandprint)
                {
                    int layerMask = LayerMask.GetMask("ARMesh");
                    RaycastHit hit;
                    if (Physics.Raycast(new Ray(transform.position, placedDirection), out hit, 100f, layerMask))
                    {
                        if (!myHandprint)
                        {
                            GameObject newHandprint = Instantiate(handprintPrefab, hit.point, new Quaternion());
                            myHandprint = newHandprint.GetComponent<Handprint>();
                            newHandprint.transform.LookAt(newHandprint.transform.position + hit.normal);
                            //transform.Rotate(new Vector3(90f, 0f, 0f));
                        }
                        else
                        {
                            myHandprint.transform.position = hit.point;
                            myHandprint.transform.LookAt(myHandprint.transform.position + hit.normal);
                        }
                    }
                }
                if (placedItem)
                {
                    if (!myItem)
                    {
                        GameObject newItem = Instantiate(itemPrefab, floor, new Quaternion());
                        myItem = newItem.GetComponent<CursedItem>();
                        //transform.Rotate(new Vector3(90f, 0f, 0f));
                    }
                    else
                    {
                        myItem.transform.position = floor;
                    }
                }
            }
        }
    }
    public void SetFCost(Waypoint comingFrom)
    {
        int layerMask = LayerMask.GetMask("ARMesh");
        RaycastHit hit;
        if (Physics.Linecast(comingFrom.transform.position,transform.position, out hit, layerMask))
        {
            fCost = gCost + hCost + 10f;
        }
        else
        {
            fCost = gCost + hCost;
        }
        
    }
    public void SetFloor()
    {
        int layerMask = LayerMask.GetMask("ARMesh");
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, -Vector3.up), out hit, 2f, layerMask))
        {
            floor = hit.point;
        }
        else
        {
            List<Waypoint> closeWays = controlScript.GetWaypointsInRange(controlScript.distanceBetweenWaypoints*3,transform.position,controlScript.waypoints);
            float totalFloor = 0f;
            for (int i = 0; i < closeWays.Count;i++)
            {
                totalFloor += closeWays[i].floor.y;
            }
            floor = new Vector3(transform.position.x, totalFloor/ closeWays.Count, transform.position.z);
        }

        floorSphere.transform.position = floor;
    }
    public void CreateAnchor()
    {
        RemoveAnchor();
        // Add an ARAnchor component if it doesn't have one already.
        if (gameObject.GetComponent<ARAnchor>() == null)
        {
            gameObject.AddComponent<ARAnchor>();
        }
    }
    public void RemoveAnchor()
    {
        if (gameObject.GetComponent<ARAnchor>() != null)
        {
            Destroy(gameObject.GetComponent<ARAnchor>());
        }
    }
}
