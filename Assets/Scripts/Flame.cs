using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
    public SpriteRenderer flame1;
    public SpriteRenderer flame2;

    public float timeTillChange;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeTillChange -= 1f * Time.deltaTime;
        if (timeTillChange<=0f)
        {
            timeTillChange = 0.01f;
            if (Random.Range(0,100)>50)
            {
                flame1.flipX = true;
                flame2.flipX = true;
            }
            else
            {
                flame1.flipX = false;
                flame2.flipX = false;
            }
            Vector3 tempScale = transform.localScale;
            tempScale.y = Random.Range(0.9f, 1.1f);
            transform.localScale = tempScale;
        }
    }
}
