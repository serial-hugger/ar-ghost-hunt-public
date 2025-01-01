using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostLightTrail : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public float hyperness;
    private float timeTillChange = 1f;
    public float xRotChange;
    public float yRotChange;
    public float zRotChange;

    public bool staticPhotoOnly;

    public TrailRenderer myTrail;
    public TrailRenderer detectionTrail;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Random.rotation;
        xRotChange = Random.Range(-5f, 5f);
        yRotChange = Random.Range(-5f, 5f);
        zRotChange = Random.Range(-5f, 5f);
        myTrail.time = Random.Range(0.5f,1.5f);
        detectionTrail.time = Random.Range(0.5f, 1.5f);

        if (staticPhotoOnly)
        {
            gameObject.layer = LayerMask.NameToLayer("PhotoOnly");
            myTrail.time = float.PositiveInfinity;
            detectionTrail.time = float.PositiveInfinity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= 1f * Time.deltaTime;
        if (lifeTime>=0f) {
            timeTillChange -= hyperness * Time.deltaTime;
            if (timeTillChange <= 0f)
            {
                xRotChange = Random.Range(-500f, 500f);
                yRotChange = Random.Range(-500f, 500f);
                zRotChange = Random.Range(-500f, 500f);
                timeTillChange = 1f;
            }
            //Move forward
            transform.position += transform.forward * speed * Time.deltaTime;
            //Rotate direction
            Vector3 tempRot = transform.eulerAngles;
            tempRot.x += xRotChange * Time.deltaTime;
            tempRot.y += yRotChange * Time.deltaTime;
            tempRot.z += zRotChange * Time.deltaTime;
            transform.eulerAngles = tempRot;
        }
        if (!staticPhotoOnly)
        {
            if (lifeTime < 0f - myTrail.time)
            {
                GameObject.Destroy(gameObject);
            }
        }
        else
        {
            if (lifeTime < 0f)
            {
                myTrail.time = float.PositiveInfinity;
                detectionTrail.time = float.PositiveInfinity;
            }
        }
    }
}
