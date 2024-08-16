using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointGuesser : MonoBehaviour
{
    public Transform topCheck;
    public Transform bottomCheck;
    public Transform leftCheck;
    public Transform rightCheck;

    public Controller controlScript;

    public bool valid;

    // Start is called before the first frame update
    void Start()
    {
        if (controlScript.GetClosestWaypointInRange(controlScript.distanceBetweenWaypoints, transform.position, controlScript.waypoints) == null) {
            ValidPlacement();
            if (valid)
            {
                RaycastHit hit;
                Physics.Raycast(new Ray(topCheck.position, -Vector3.up), out hit);
                List<Waypoint> rangeWay = controlScript.GetWaypointsInRange(controlScript.distanceBetweenMasterWaypoints, Camera.main.transform.position, controlScript.waypoints);
                Waypoint closestMaster = null;
                for (int i = 0; i < rangeWay.Count; i++)
                {
                    if (rangeWay[i].isMaster)
                    {
                        closestMaster = rangeWay[i];
                    }
                }
                Waypoint newWay = Instantiate(controlScript.waypointPrefab, transform.position + Vector3.up, new Quaternion()).GetComponent<Waypoint>();
                if (closestMaster != null)
                {
                    newWay.transform.parent = closestMaster.transform;
                }
                else
                {
                    newWay.isMaster = true;
                }
                newWay.controlScript = controlScript;
                newWay.SetFloor();
                controlScript.waypoints.Add(newWay);
                controlScript.prevSpawnedWaypoint = newWay;
            }
        }
        GameObject.Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ValidPlacement()
    {
        //valid = true;
        //return;
        float longestDist = -1f;
        float shortestDist = -1f;
        RaycastHit hit;
        //TL
        if (Physics.Raycast(new Ray(topCheck.position, -Vector3.up), out hit))
        {
            if (hit.distance < shortestDist || shortestDist == -1f)
            {
                shortestDist = hit.distance;
            }
            if (hit.distance > longestDist || longestDist == -1f)
            {
                longestDist = hit.distance;
            }
        }
        else
        {
            valid = false;
            return;
        }
        //TR
        if (Physics.Raycast(new Ray(bottomCheck.position, -Vector3.up), out hit))
        {
            if (hit.distance < shortestDist || shortestDist == -1f)
            {
                shortestDist = hit.distance;
            }
            if (hit.distance > longestDist || longestDist == -1f)
            {
                longestDist = hit.distance;
            }
        }
        else
        {
            valid = false;
            return;
        }
        //BL
        if (Physics.Raycast(new Ray(leftCheck.position, -Vector3.up), out hit))
        {
            if (hit.distance < shortestDist || shortestDist == -1f)
            {
                shortestDist = hit.distance;
            }
            if (hit.distance > longestDist || longestDist == -1f)
            {
                longestDist = hit.distance;
            }
        }
        else
        {
            valid = false;
            return;
        }
        //BR
        if (Physics.Raycast(new Ray(rightCheck.position, -Vector3.up), out hit))
        {
            if (hit.distance < shortestDist || shortestDist == -1f)
            {
                shortestDist = hit.distance;
            }
            if (hit.distance > longestDist || longestDist == -1f)
            {
                longestDist = hit.distance;
            }
        }
        else
        {
            valid = false;
            return;
        }
        if (Mathf.Abs(longestDist - shortestDist) < 0.5f)
        {
            valid = true;
        }
        else
        {
            valid = false;
        }
    }
}
