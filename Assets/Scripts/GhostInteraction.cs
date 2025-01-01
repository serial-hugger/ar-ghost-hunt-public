using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostInteraction : MonoBehaviour
{
    public float emfAmount;
    public float windAmount;
    public float motionAmount;

    public Controller controlScript;
    int prevWaypointAmount = 0;

    public float lifetime;

    public GameObject ghostPrefab;
    public GameObject orbPrefab;
    public GameObject trailPrefab;

    public GameObject myGhost;
    public List<GameObject> myOrbs;
    public List<GameObject> myTrails;

    public GameObject myEvidences;

    // Start is called before the first frame update
    private void Start()
    {
        controlScript = GameObject.Find("Controller").GetComponent<Controller>();
        float size = Random.Range(3f,6f);
        if (!Tutorial.inTutorial) {
            transform.localScale = new Vector3(size, size, size);
            lifetime = Mathf.Min(100f, controlScript.waypoints.Count * 2f);
            myEvidences = new GameObject("Ghost Area Evidences");
            myEvidences.transform.position = transform.position;
            SpawnHiddenEvidence();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!Tutorial.inTutorial) {
            bool ghostVisible = false;
            bool effectVisible = controlScript.closeGhostAreas.Contains(this);
            if (controlScript.closestGhostArea == this && controlScript.ghostScript.gone)
            {
                ghostVisible = true;
            }
            SetEvidenceVisibility(ghostVisible, effectVisible);
            lifetime -= 1f * Time.deltaTime;
            if (lifetime < 0f)
            {
                controlScript.ghostAreas.Remove(this);
                DestroyHiddenEvidence();
                GameObject.Destroy(gameObject);
            }
            if (prevWaypointAmount < controlScript.waypoints.Count)
            {
                prevWaypointAmount = controlScript.waypoints.Count;
                Vector3 tempPos = transform.position;
                tempPos.y = controlScript.GetClosestWaypoint(transform.position, controlScript.waypoints).transform.position.y;
                transform.position = tempPos;
            }
            //set emf
            Random.InitState((int)(Time.time / 60f) + (int)transform.position.x + (int)transform.position.z + 1);
            var rnd = Mathf.Pow(Random.value, 2);
            var index = Mathf.FloorToInt(0 + rnd * (10 - 0 + 1));
            emfAmount = Mathf.Clamp(index, 0, 10);
            //set wind
            Random.InitState((int)(Time.time / 60f) + (int)transform.position.x + (int)transform.position.z + 2);
            rnd = Mathf.Pow(Random.value, 2);
            index = Mathf.FloorToInt(0 + rnd * (10 - 0 + 1));
            windAmount = Mathf.Clamp(index, 0, 10);
            //set Motion
            Random.InitState((int)(Time.time / 60f) + (int)transform.position.x + (int)transform.position.z + 3);
            rnd = Mathf.Pow(Random.value, 2);
            index = Mathf.FloorToInt(0 + rnd * (10 - 0 + 1));
            motionAmount = Mathf.Clamp(index, 0, 10);
        }
        else
        {
            emfAmount = 10;
            windAmount = 10;
            motionAmount = 10;
        }
    }

    private void SpawnHiddenEvidence()
    {
        Quaternion direction = Random.rotation;
        GhostEntity ghostScript = Instantiate(ghostPrefab, transform.localPosition + (direction.eulerAngles * Random.Range(0f, 0.01f)), new Quaternion()).GetComponent<GhostEntity>();
        myGhost = ghostScript.gameObject;
        myGhost.transform.SetParent(myEvidences.transform);
        ghostScript.staticPhotoOnly = true;
        int orbAmount = Random.Range(0, 5);
        for (int o = 0; o < orbAmount; o++)
        {
            direction = Random.rotation;
            GhostOrb orbScript = Instantiate(orbPrefab, transform.localPosition + (direction.eulerAngles * Random.Range(0f, 0.01f)), new Quaternion()).GetComponent<GhostOrb>();
            myOrbs.Add(orbScript.gameObject);
            orbScript.staticPhotoOnly = true;
            orbScript.transform.SetParent(myEvidences.transform);
        }
        int trailAmount = Random.Range(0, 5);
        for (int t = 0; t < trailAmount; t++)
        {
            direction = Random.rotation;
            GhostLightTrail trailScript = Instantiate(trailPrefab, transform.localPosition + (direction.eulerAngles * Random.Range(0f, 0.01f)), new Quaternion()).GetComponent<GhostLightTrail>();
            myTrails.Add(trailScript.gameObject);
            trailScript.staticPhotoOnly = true;
            trailScript.speed = 5f;
            trailScript.hyperness = 100f;
            trailScript.lifeTime = Random.Range(0.5f, 2.5f);
            trailScript.transform.SetParent(myEvidences.transform);
        }
    }

    private void DestroyHiddenEvidence()
    {
        GameObject.Destroy(myGhost);
        myGhost = null;
        for (int o = 0; o < myOrbs.Count; o++)
        {
            GameObject.Destroy(myOrbs[o]);
        }
        myOrbs.Clear();
        for (int t = 0; t < myTrails.Count; t++)
        {
            GameObject.Destroy(myTrails[t]);
        }
        myTrails.Clear();
    }

    private void SetEvidenceVisibility(bool ghost, bool effects)
    {
        if (myGhost) {
            myGhost.SetActive(ghost);
        }
        for (int o = 0; o<myOrbs.Count;o++)
        {
            if (myOrbs[o])
            {
                myOrbs[o].SetActive(effects);
            }
        }
        for (int t = 0; t < myTrails.Count; t++)
        {
            if (myTrails[t])
            {
                myTrails[t].SetActive(effects);
            }
        }
    }
}
