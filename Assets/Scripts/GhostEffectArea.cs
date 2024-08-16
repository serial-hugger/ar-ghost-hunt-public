using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEffectArea : MonoBehaviour
{
    public GameObject myCamera;

    public float orbSpawnRate;
    public float orbSpawnDistance;
    public GameObject orbPrefab;

    public float trailSpawnRate;
    public float trailSpawnDistance;
    public GameObject trailPrefab;

    private float timeTillOrb;
    private float timeTillTrail;

    public float lifespan;




    // Start is called before the first frame update
    void Start()
    {
        myCamera = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        lifespan -= 1f * Time.deltaTime;
        if (lifespan <= 0f)
        {
            GameObject.Destroy(gameObject);
        }
        //Spawn orbs
        timeTillOrb -= orbSpawnRate * Time.deltaTime;
        if (timeTillOrb <= 0f)
        {
            float randDist = Random.Range(0f- orbSpawnDistance, orbSpawnDistance);
            Vector3 randDir = Random.rotation.eulerAngles;
            GhostOrb newOrb = Instantiate(orbPrefab, transform.position+(randDir*randDist), new Quaternion()).GetComponent<GhostOrb>();
            newOrb.transform.eulerAngles = randDir;
            newOrb.transform.position += (randDir.normalized*randDist);
            newOrb.myCamera = myCamera;
            newOrb.decaySpeed = Random.Range(0.01f,0.1f);
            newOrb.myAlpha = Random.Range(0.1f, 1f);
            timeTillOrb = 1f;
        }
        //Spawn trails
        timeTillTrail -= trailSpawnRate * Time.deltaTime;
        if (timeTillTrail <= 0f)
        {
            float randDist = Random.Range(0f - trailSpawnDistance, trailSpawnDistance);
            Vector3 randDir = Random.rotation.eulerAngles;
            GhostLightTrail newTrail = Instantiate(trailPrefab, transform.position + (randDir * randDist), new Quaternion()).GetComponent<GhostLightTrail>();
            newTrail.transform.eulerAngles = randDir;
            newTrail.transform.position += (randDir.normalized * randDist);
            newTrail.speed = 5f;
            newTrail.hyperness = 100f;
            newTrail.lifeTime = Random.Range(0.5f,2.5f);
            timeTillTrail = 1f;
        }
    }

}
