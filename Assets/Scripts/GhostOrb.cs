using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostOrb : MonoBehaviour
{
    public GameObject myCamera;
    public Vector3 direction;
    public float speed;
    public float myAlpha;
    public float decaySpeed;
    public SpriteRenderer myRender;

    public Vector3 visDir;

    public GameObject detection;

    public bool staticPhotoOnly;

    // Start is called before the first frame update
    void Start()
    {
        myCamera = Camera.main.gameObject;
        visDir = Random.rotation.eulerAngles;
        direction = Random.rotation.eulerAngles;
        speed = Random.Range(0.001f,0.01f);
        float newScale = Random.Range(0.01f,0.08f);
        myRender.transform.localScale = new Vector3(newScale, newScale, newScale);
        if (staticPhotoOnly)
        {
            myAlpha = Random.Range(0.4f,0.9f);
            myRender.transform.localScale = new Vector3(newScale/5f, newScale / 5f, newScale / 5f);
            myRender.gameObject.layer = LayerMask.NameToLayer("PhotoOnly");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!staticPhotoOnly) {
            float alphaDecrease = Mathf.Abs((Mathf.DeltaAngle(myCamera.transform.rotation.eulerAngles.y, visDir.y) * 0.03f) + (Mathf.DeltaAngle(myCamera.transform.rotation.eulerAngles.x, visDir.x) * 0.03f));
            myAlpha -= decaySpeed * Time.deltaTime;
            transform.LookAt(myCamera.transform);
            Color tempColor = myRender.color;
            tempColor.a = Mathf.Max(0f, myAlpha - alphaDecrease);
            myRender.color = tempColor;
            if (myRender.color.a > 0.02f)
            {
                detection.SetActive(true);
            }
            else
            {
                detection.SetActive(false);
            }
            transform.position += (direction * speed) * 0.001f;

            if (myAlpha <= 0f)
            {
                GameObject.Destroy(gameObject);
            }
        }
        else
        {
            Color tempColor = myRender.color;
            tempColor.a = myAlpha;
            myRender.color = tempColor;

            transform.LookAt(myCamera.transform);
        }
        myRender.enabled = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        //GameObject.Destroy(gameObject);
    }
}
