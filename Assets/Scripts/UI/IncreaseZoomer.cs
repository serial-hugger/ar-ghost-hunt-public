using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncreaseZoomer : MonoBehaviour
{
    public Sprite xpSprite;
    public Sprite moneySprite;
    public Image render;
    public Vector3 goPos;
    public float xSpeed;
    public float ySpeed;

    public float timeTillGo;

    // Start is called before the first frame update
    void Start()
    {
        xSpeed = Random.Range(-300f,300f);
        ySpeed = Random.Range(-300f, 300f);
        timeTillGo = Random.Range(0.1f,.75f);
        transform.localScale = new Vector3(0f,0f,0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale= Vector3.MoveTowards(transform.localScale,new Vector3(1f,1f,1f),10f*Time.deltaTime);
        xSpeed = Mathf.MoveTowards(xSpeed,0f,100f*Time.deltaTime);
        ySpeed = Mathf.MoveTowards(ySpeed, 0f, 100f * Time.deltaTime);
        timeTillGo -= 1f * Time.deltaTime;
        if (timeTillGo <= 0f) {
            transform.position = Vector2.MoveTowards(transform.position, goPos, 2000f * Time.deltaTime);
        }
        Vector2 tempPos = transform.position;
        tempPos.x += xSpeed * Time.deltaTime;
        tempPos.y += ySpeed * Time.deltaTime;
        transform.position = tempPos;
        if (Vector2.Distance(transform.position,goPos)<=10f)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
