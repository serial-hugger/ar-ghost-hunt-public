using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tripod : MonoBehaviour
{
    public Transform leg1;
    public Transform leg2;
    public Transform leg3;

    public Transform castPoint;

    public GameObject attached;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = attached.transform.position;
        int layer_mask = LayerMask.GetMask("ARMesh");
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, -Vector3.up), out hit, 2f, layer_mask))
        {
            Vector3 tempRot = leg1.localEulerAngles;
            tempRot.x = -30f + (hit.distance*10f);
            leg1.localEulerAngles = tempRot;
            Vector3 tempScale = leg1.localScale;
            tempScale.y = hit.distance * 5f;
            leg1.localScale = tempScale;

            tempRot = leg2.localEulerAngles;
            tempRot.x = -30f + (hit.distance * 10f);
            leg2.localEulerAngles = tempRot;
            tempScale = leg2.localScale;
            tempScale.y = hit.distance * 5f;
            leg2.localScale = tempScale;

            tempRot = leg3.localEulerAngles;
            tempRot.x = -30f + (hit.distance * 10f);
            leg3.localEulerAngles = tempRot;
            tempScale = leg3.localScale;
            tempScale.y = hit.distance*5f;
            leg3.localScale = tempScale;
        }
    }
}
