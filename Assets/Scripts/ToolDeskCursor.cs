using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolDeskCursor : MonoBehaviour
{
    public Color allowedColor;
    public Color disallowedColor;

    public Renderer myRender1;
    public Renderer myRender2;

    public Transform TL;
    public Transform TR;
    public Transform BL;
    public Transform BR;

    public bool valid;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ValidPlacement();
        if (valid)
        {
            myRender1.material.color = allowedColor;
            myRender2.material.color = allowedColor;
        }
        else
        {
            myRender1.material.color = disallowedColor;
            myRender2.material.color = disallowedColor;
        }
    }

    void ValidPlacement()
    {
        //valid = true;
        //return;
        float longestDist = -1f;
        float shortestDist = -1f;
        RaycastHit hit;
        //TL
        if (Physics.Raycast(new Ray(TL.position, -Vector3.up), out hit))
        {
            if (hit.distance< shortestDist || shortestDist == -1f)
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
        if (Physics.Raycast(new Ray(TR.position, -Vector3.up), out hit))
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
        if (Physics.Raycast(new Ray(BL.position, -Vector3.up), out hit))
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
        if (Physics.Raycast(new Ray(BR.position, -Vector3.up), out hit))
        {
            if (hit.distance < shortestDist || shortestDist == -1f)
            {
                shortestDist = hit.distance;
            }
            if (hit.distance > longestDist || longestDist==-1f)
            {
                longestDist = hit.distance;
            }
        }
        else
        {
            valid = false;
            return;
        }
        if (Mathf.Abs(longestDist-shortestDist)<0.1f)
        {
            valid = true;
        }
        else
        {
            valid = false;
        }
    }
}
