using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEntity : MonoBehaviour
{

    public bool started;
    public float speed;
    public List<Waypoint> myWay;
    public List<Vector3> myPath;
    int currentPath = 0;
    Controller controlScript;

    public float minCheck;
    public float maxCheck;

    public double moonAmt = 0;

    public float changeProbability;
    public float moonChangeAdd;
    float changeCheckTime;

    public float appearProbability;
    public float moonAppearAdd;
    float appearCheckTime;

    public float smackProbability;
    public float moonSmackAdd;
    float smackCheckTime;

    public float printProbability;
    public float moonPrintAdd;
    float printCheckTime;

    public float effectProbability;
    public float moonEffectAdd;
    float effectCheckTime;

    public bool gone;

    public string doing;

    public List<Material> myMaterials = new List<Material>();

    public GameObject soundGhast;
    public GameObject effectPrefab;

    public List<GameObject> ghostPrefabs;

    public GhostEntityAnimator animatorScript;

    public bool staticPhotoOnly;

    public float crouchAmt;

    public int myGhostType;

    public int testingGhost;
    private Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        appearCheckTime = maxCheck;
        changeCheckTime = maxCheck;
        effectCheckTime = maxCheck;
        printCheckTime = maxCheck;
        smackCheckTime = maxCheck;

        controlScript = GameObject.Find("Controller").GetComponent<Controller>();
        moonAmt = controlScript.saveScript.moonScript.GetPhase(new System.DateTime(TimeManager.GetTime()));
        if (!staticPhotoOnly)
        {
            int seed = 0;
            System.DateTime lastTime = new System.DateTime(controlScript.saveScript.gameData.lastValidTime);
            seed += lastTime.Year;
            seed += lastTime.Month;
            string token = controlScript.locationScript.token;
            for (int i = 0; i < token.Length;i++)
            {
                seed += (int)token[i] % 32;
            }
            Random.InitState(seed);
            if (!Application.isEditor) {
                myGhostType = Random.Range(0, ghostPrefabs.Count);
            }
            else
            {
                myGhostType = testingGhost;
            }
        }
        else
        {
            myGhostType = controlScript.ghostScript.myGhostType;
        }
        if (!staticPhotoOnly) {
            UpdateAction();
            gone = true;
            if (controlScript.testMode)
            {
                gone = false;
                myPath.Clear();
                currentPath = 0;
            }
        }
        speed = 1f;
        animatorScript = Instantiate(ghostPrefabs[myGhostType], transform.position, transform.rotation).GetComponent<GhostEntityAnimator>();
        //animatorScript = Instantiate(ghostPrefabs[2], transform.position, transform.rotation).GetComponent<GhostEntityAnimator>();
        animatorScript.transform.parent = transform;
        animatorScript.ghostScript = this;
        gone = true;
        animatorScript.vanish = true;
    }

    // Update is called once per frame
    void Update()
    {
        GhostMechanics();
        int layerMask = LayerMask.GetMask("ARMesh");
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, Vector3.up), out hit, 4f, layerMask))
        {
            crouchAmt = Mathf.Lerp(crouchAmt,1f-(Mathf.Min(2f,Mathf.Max(0f,hit.distance-2f))/2f),0.3f*Time.deltaTime);
        }
        else
        {
            crouchAmt = Mathf.Lerp(crouchAmt,0f, 0.3f * Time.deltaTime);
        }
        //makes talls ghosts shrink a bit while crouching
        if (animatorScript.tall)
        {
            Vector3 tempScale = animatorScript.gameObject.transform.localScale;
            float crouchSize = 0.8f - (crouchAmt / 5.0f);
            tempScale = new Vector3(crouchSize, crouchSize, crouchSize);
            animatorScript.gameObject.transform.localScale = tempScale;
        }
        if (staticPhotoOnly)
        {
            if (Physics.Raycast(new Ray(transform.position+(Vector3.up*3f), -Vector3.up), out hit, 10f, layerMask))
            {
                transform.position =  hit.point;
            }
        }
        //Set animation variables
        if (animatorScript.myAnimator) {
            if (doing is "looking" or "standing")
            {
                animatorScript.myAnimator.SetFloat("Speed", speed);
                animatorScript.myAnimator.SetFloat("Crouch", crouchAmt);
                animatorScript.myAnimator.SetBool("Idle", true);
                animatorScript.myAnimator.SetBool("Walking", false);
            }
            if (doing is "homing" or "pursuing" or "wandering")
            {
                animatorScript.myAnimator.SetFloat("Speed", speed);
                animatorScript.myAnimator.SetFloat("Crouch", crouchAmt);
                animatorScript.myAnimator.SetBool("Idle", false);
                animatorScript.myAnimator.SetBool("Walking", true);
            }
        }

    }
    // ReSharper disable Unity.PerformanceAnalysis
    public void GhostMechanics()
    {
        appearCheckTime -= 1f * Time.deltaTime;
        changeCheckTime -= 1f * Time.deltaTime;
        printCheckTime -= 1f * Time.deltaTime;
        smackCheckTime -= 1f * Time.deltaTime;
        effectCheckTime -= 1f * Time.deltaTime;

        if (!staticPhotoOnly)
        {
            //attempt effect creation
            if (effectCheckTime <= 0f)
            {
                if (Random.Range(0, 1000) < effectProbability + (moonEffectAdd*moonAmt) && gone)
                {
                    Waypoint way = controlScript.GetRandomWaypointInRange(10f, transform.position, controlScript.waypoints);
                    GhostEffectArea area = Instantiate(effectPrefab, way.floor, transform.rotation).GetComponent<GhostEffectArea>();
                    area.lifespan = Random.Range(10f, 100f);
                    area.orbSpawnRate = Random.Range(1f, 5f);
                    area.orbSpawnDistance = Random.Range(0.025f, 0.1f);
                    area.trailSpawnRate = Random.Range(0.1f, 1f);
                    area.trailSpawnDistance = Random.Range(0.025f, 0.1f);
                }
                effectCheckTime = Random.Range(minCheck, maxCheck);
            }
            //attempt changing action
            if (changeCheckTime <= 0f)
            {
                if (Random.Range(0,1000)<changeProbability+ (moonChangeAdd * moonAmt)) {
                    UpdateAction();
                }
                changeCheckTime = Random.Range(minCheck, maxCheck);
            }
            //attempt handprint
            if (printCheckTime <= 0f)
            {
                if (Random.Range(0,1000)<printProbability+ (moonPrintAdd * moonAmt)) {
                    controlScript.GetClosestWaypoint(transform.position, controlScript.waypoints).placedHandprint = true;
                }
                printCheckTime = Random.Range(minCheck, maxCheck);
            }
            //attempt smack
            if (smackCheckTime <= 0f)
            {
                if (Random.Range(0, 1000) < smackProbability+ (moonSmackAdd * moonAmt))
                {
                    AttemptEquipmentSmack();
                }
                smackCheckTime = Random.Range(minCheck, maxCheck);
            }
            //attempt reappear
            if (appearCheckTime <= 0f)
            {
                if (Random.Range(0,1000)<appearProbability+ (moonAppearAdd * moonAmt))
                {
                    controlScript.AppearGhost();
                }
                appearCheckTime = Random.Range(minCheck, maxCheck);
            }


            speed = Mathf.Max(0.5f, Mathf.Min(2f, Vector3.Distance(transform.position, _camera.transform.position) / 2f));

            //decay ghost
            if (!gone && Vector3.Distance(transform.position, _camera.transform.position) <= 5f)
            {
                animatorScript.visibility -= (5f - Vector3.Distance(transform.position, _camera.transform.position)) * 0.2f * Time.deltaTime;
            }

            //make ghost dissapear if decay is too high
            if (animatorScript.visibility < 0f && !controlScript.testMode)
            {
                animatorScript.visibility = 0f;
                if (!gone)
                {
                    //Instantiate(soundGhast, transform.position, transform.rotation);
                    gone = true;
                }
            }

            //vanish the animator script when there is time to be gone
            if (gone)
            {
                animatorScript.visibility += 0.005f * Time.deltaTime;
                animatorScript.vanish = true;
                animatorScript.maxVisibility = 1f;
            }
            else
            {
                animatorScript.visibility += 0.002f * Time.deltaTime;
                animatorScript.vanish = false;
            }
            if (doing is "looking" or "homing")
            {
                Vector3 tempRot;
                Vector3 oldRot;
                Vector3 newRot;
                oldRot = transform.eulerAngles;
                transform.LookAt(_camera.transform.position);
                newRot = transform.eulerAngles;
                transform.eulerAngles = oldRot;
                tempRot = transform.eulerAngles;
                tempRot.x = 0f;
                tempRot.z = 0f;
                tempRot.y = newRot.y;
                transform.eulerAngles = tempRot;
            }
            if (doing == "homing")
            {
                Vector3 tempPos = transform.position;
                tempPos.y = Mathf.Lerp(tempPos.y, controlScript.playerFloor.y, speed * 2f * Time.fixedDeltaTime);
                tempPos += transform.forward * (speed * 2f * Time.fixedDeltaTime);
                transform.position = tempPos;
            }
            if (myPath.Count > 0)
            {
                Vector3 tempRot;
                Vector3 oldRot;
                Vector3 newRot;
                if (!started)
                {
                    oldRot = transform.eulerAngles;
                    transform.LookAt(myPath[currentPath]);
                    newRot = transform.eulerAngles;
                    transform.eulerAngles = oldRot;
                    tempRot = transform.eulerAngles;
                    tempRot.x = 0f;
                    tempRot.z = 0f;
                    tempRot.y = newRot.y;
                    transform.eulerAngles = tempRot;
                    started = true;
                }
                transform.position = Vector3.MoveTowards(transform.position, myPath[currentPath], speed * Time.fixedDeltaTime);
                if (transform.position == myPath[currentPath])
                {
                    currentPath += 1;
                    if (currentPath >= myPath.Count - 1)
                    {
                        if (controlScript.testMode)
                        {
                            //GameObject.Destroy(gameObject);
                        }
                        GeneratePath();
                    }
                }
                oldRot = transform.eulerAngles;
                if (myPath.Count > 0 && currentPath < myPath.Count)
                {
                    transform.LookAt(myPath[currentPath]);
                }
                newRot = transform.eulerAngles;
                transform.eulerAngles = oldRot;
                tempRot = transform.eulerAngles;
                tempRot.x = 0f;
                tempRot.z = 0f;
                tempRot.y = Mathf.LerpAngle(oldRot.y, newRot.y, speed * Time.fixedDeltaTime);
                transform.eulerAngles = tempRot;

            }
        }
    }
    public void GeneratePath()
    {
        currentPath = 0;
        myWay.Clear();
        myPath.Clear();
            Waypoint myArea = controlScript.GetRandomWaypoint();
            if (doing == "wandering")
            {
                if (!started) {
                    transform.position = myArea.transform.position;
                }
                Waypoint destination = controlScript.GetRandomWaypointInRange(20f, _camera.transform.position, controlScript.waypoints);
                if (destination == null)
                {
                    destination = controlScript.GetRandomWaypoint();
                }
                myWay = controlScript.GetPathOfWaypoints(transform.position, destination.floor, controlScript.waypoints);
                for (int w = 0; w < myWay.Count; w++)
                {
                    myPath.Add(myWay[w].floor);
                }
                myPath.Add(controlScript.playerFloor);
            }
            if (doing == "pursuing")
            {
                if (!started)
                {
                    transform.position = myArea.transform.position;
                }
                myWay = controlScript.GetPathOfWaypoints(transform.position, _camera.transform.position, controlScript.waypoints);
                if (myWay!=null) {
                for (int w = 0; w < myWay.Count; w++)
                {
                    myPath.Add(myWay[w].floor);
                }
                }
                myPath.Add(controlScript.playerFloor);
            }
            if (doing is "standing" or "looking")
            {
                if (!Application.isEditor) {
                    Waypoint destination = controlScript.GetRandomWaypointOutsideRange(10f, _camera.transform.position, controlScript.waypoints);
                    if (destination == null)
                    {
                        destination = controlScript.GetRandomWaypoint();
                    }
                    if (!started) {
                        Vector3 tempRot = transform.eulerAngles;
                        tempRot.y = Random.Range(0f, 360f);
                        transform.eulerAngles = tempRot;
                        transform.position = destination.floor;
                    }
                }
            }
        started = true;
    }
    void UpdateAction()
    {
        if (!controlScript.testMode) {
            int action = Random.Range(0, 100);
            if (action is >= 10 and < 30)
            {
                doing = "wandering";
            }
            if (action is >= 30 and < 35)
            {
                if (!gone) {
                    doing = "pursuing";
                }
                else
                {
                    doing = "wandering";
                }
            }
            if (action is >= 35 and < 70)
            {
                doing = "standing";
            }
            if (action >= 70)
            {
                doing = "looking";
            }
        }
        GeneratePath();
        int tempRnd = Random.Range(0, 100);
        if (tempRnd<=25)
        {
            controlScript.GetClosestWaypoint(transform.position,controlScript.waypoints).placedHandprint = true;
        }
        if (tempRnd>=75) {
            AttemptEquipmentSmack();
        }
    }
    void AttemptEquipmentSmack()
    {
        GameObject[] equipmentFind = GameObject.FindGameObjectsWithTag("Equipment");
        GameObject currentClosest = null;
        for (int i = 0;i<equipmentFind.Length;i++)
        {
            if (currentClosest==null|| Vector3.Distance(transform.position, equipmentFind[i].transform.position) < Vector3.Distance(transform.position,currentClosest.transform.position))
            {
                if (equipmentFind[i].GetComponent<Equipment>()!=controlScript.heldEquipment) {
                    currentClosest = equipmentFind[i];
                }
            }
        }
        if (currentClosest!=null&&Vector3.Distance(transform.position, currentClosest.transform.position)<3f) {
            Rigidbody rb = currentClosest.GetComponent<Rigidbody>();
            Equipment equipScript = currentClosest.GetComponent<Equipment>();
            equipScript.noisesMade = 1;
            equipScript.ResetVariables();
            equipScript.timeToKeepThrown = 100f;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1.5f), Random.Range(-1f, 1f)), ForceMode.Impulse);
            rb.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)), ForceMode.Impulse);
        }
    }
}
