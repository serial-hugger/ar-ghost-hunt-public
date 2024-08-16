using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.Lightship.AR.PersistentAnchors;
using Niantic.Lightship.Utilities;
using Niantic.Lightship.AR;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;

public class Equipment : MonoBehaviour
{
    public GameObject mySpawn;
    public bool pickedUp;

    public Vector3 startPos;
    public Vector3 startRot;

    public Vector3 placePos;

    public Vector3 relativeToDesk;
    public bool onDesk;

    public bool clipboard;
    public bool emf;
    public bool motion;
    public bool candle;
    public bool lighter;
    public bool compass;
    public bool dowsing;
    public bool bell;
    public bool uv;
    public bool liveCamera;

    public AudioSource myAudio;

    public GameObject dowsingRod1;
    public GameObject dowsingRod2;
    public float timeTillDowse;
    public int dowseState = 0;

    public Color emf1Color;
    public Color emf2Color;
    public Color emf3Color;
    public Color emf4Color;
    public Color emf5Color;

    public Renderer emf1Render;
    public Renderer emf2Render;
    public Renderer emf3Render;
    public Renderer emf4Render;
    public Renderer emf5Render;

    public GameObject uvBeam;

    public Transform compassNeedle;

    public GameObject lighterFlame;

    public Flame flame;
    public bool flameLit;

    public float emfLightsOn = 0f;

    public bool gravity;

    public Rigidbody rb;

    private ARSession _session;

    public Controller controlScript;

    public List<BoxCollider> colliders;

    Vector3 prevPos;
    Vector3 prevRot;

    public float timeTillSettle = 1f;
    public float yOffset = -999f;
    public Vector3 placedOriginPosition;
    public Vector3 placedOriginRotation;

    public float timeTillCheck = 0f;

    public float emfAmount;
    public float windAmount;
    public float motionAmount;
    public GhostInteraction lastGhostScript;

    public GameObject mySoundPrefab;

    public float cooldown;

    public GameObject toolThrown;
    public GameObject toolLit;

    public float timeToKeepLit;
    public float timeToKeepThrown;

    public GameObject myTripod;

    public float distToGround;

    public RawImage cameraImage;
    public RawImage renderImage;

    public Camera myCam;
    public Camera myDetectionCam;
    public RenderTexture renderTexture;
    public RenderTexture detectionTexture;
    public AspectRatioFitter imageFitter;
    public AspectRatioFitter renderFitter;

    public GameObject tutorialDigitalTarget;

    //Components for clipboard
    public GameObject inkBottle;
    public TextMeshPro inkWriting;
    public ParticleSystem inkSpill;
    public GameObject inkContents;
    public float inkTiltX = -90f;

    public GameObject finder;

    private Vector3 moveVelocity = Vector3.zero;
    private float xRotVelocity = 0f;
    private float yRotVelocity = 0f;
    private float zRotVelocity = 0f;

    public GameObject myAnchor;

    //used to decrease noise with consecutive hits
    public int noisesMade = 1;
    private Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        controlScript = GameObject.Find("Controller").GetComponent<Controller>();
        _session = controlScript.sessionScript;
        startPos = transform.position;
        startRot = transform.eulerAngles;
        CreateAnchor();
        rb.centerOfMass = rb.centerOfMass-(Vector3.up*0.01f);
        if (liveCamera)
        {
            renderTexture = new RenderTexture(myCam.pixelWidth, myCam.pixelHeight,32);
            detectionTexture = new RenderTexture(myDetectionCam.pixelWidth, myDetectionCam.pixelHeight, 32);
            imageFitter.aspectRatio = (float)myCam.pixelWidth / (float)myCam.pixelHeight;
            renderFitter.aspectRatio = (float)myCam.pixelWidth / (float)myCam.pixelHeight;
        }
        relativeToDesk = transform.position - controlScript.toolDeskScript.transform.position;
        onDesk = true;
        timeTillSettle = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (controlScript.heldEquipment == this)
        {
            pickedUp = true;
            //reset noises made
            noisesMade = 1;
        }
        if (!pickedUp)
        {
            transform.position = mySpawn.transform.position;
            transform.eulerAngles = mySpawn.transform.eulerAngles;
        }
        if (lighter)
        {
            if (controlScript.heldEquipment==this)
            {
                lighterFlame.SetActive(true);
            }
            else
            {
                lighterFlame.SetActive(false);
            }
        }
        if (tutorialDigitalTarget)
        {
            if (Tutorial.inTutorial)
            {
                tutorialDigitalTarget.SetActive(true);
                Vector3 flyRot = tutorialDigitalTarget.transform.localEulerAngles;
                flyRot.y += 100f * Time.fixedDeltaTime;
                tutorialDigitalTarget.transform.localEulerAngles = flyRot;
            }
            else
            {
                tutorialDigitalTarget.SetActive(false);
            }
        }
        if (clipboard && !onDesk && Tutorial.inTutorial && controlScript.heldEquipment!=this)
        {
            inkWriting.text = "BOO";
        }
        if (onDesk&&pickedUp)
        {
            transform.position = controlScript.toolDeskScript.gameObject.transform.position + relativeToDesk;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            timeToKeepThrown = 0f;
        }
        if (clipboard)
        {
            if (inkWriting.text == "")
            {
                Vector3 tempScale = inkWriting.transform.parent.localScale;
                tempScale.z = Mathf.MoveTowards(tempScale.z,0f,10f*Time.deltaTime);
                inkWriting.transform.parent.localScale = tempScale;

                inkTiltX = Mathf.MoveTowards(inkTiltX, -90f, 600f * Time.deltaTime);
                inkBottle.transform.localEulerAngles = new Vector3(inkTiltX, 30f, 0f);
                inkSpill.Stop();
                inkContents.SetActive(true);
            }
            else
            {
                timeToKeepLit = 1f;
                inkTiltX = Mathf.MoveTowards(inkTiltX, -5f, 600f * Time.deltaTime);
                inkBottle.transform.localEulerAngles = new Vector3(inkTiltX,30f,0f);

                if (inkTiltX < -5f)
                {
                    inkContents.SetActive(true);
                    if (!inkSpill.isPlaying) {
                        inkSpill.Play();
                    }
                }
                else
                {
                    inkContents.SetActive(false);
                    inkSpill.Stop();
                    Vector3 tempScale = inkWriting.transform.parent.localScale;
                    tempScale.z = Mathf.MoveTowards(tempScale.z, 1f, 10f * Time.deltaTime);
                    inkWriting.transform.parent.localScale = tempScale;
                }
            }
        }
        if (liveCamera)
        {
            if (myTripod) {
                myCam.gameObject.SetActive(true);
                myDetectionCam.gameObject.SetActive(true);
                renderImage.gameObject.SetActive(true);
                cameraImage.gameObject.SetActive(true);
                myCam.targetTexture = renderTexture;
                myDetectionCam.targetTexture = detectionTexture;
                renderImage.texture = renderTexture;
            }
            else
            {
                myCam.gameObject.SetActive(false);
                myDetectionCam.gameObject.SetActive(false);
                renderImage.gameObject.SetActive(false);
                cameraImage.gameObject.SetActive(false);
            }
        }
        timeToKeepLit -= 1f * Time.deltaTime;
        timeToKeepThrown -= 1f * Time.deltaTime;
        if (toolLit)
        {
            if (timeToKeepLit > 0f)
            {
                toolLit.SetActive(true);
            }
            else
            {
                toolLit.SetActive(false);
            }
        }
        if (toolThrown) {
            if (timeToKeepThrown > 0f)
            {
                toolThrown.SetActive(true);
            }
            else
            {
                toolThrown.SetActive(false);
            }
        }

        cooldown -= 1f * Time.deltaTime;

        emfAmount -= 1f * Time.deltaTime;
        windAmount -= 1f * Time.deltaTime;
        motionAmount -= 1f * Time.deltaTime;
        emfAmount = Mathf.Max(emfAmount,0f);
        windAmount = Mathf.Max(windAmount, 0f);
        motionAmount = Mathf.Max(motionAmount, 0f);
        //DO ACTIONS FROM GHOST INTERACTIONS
        timeTillCheck -= 1f * Time.deltaTime;
        if (timeTillCheck<=0f)
        {
            if (candle)
            {
                if (Random.Range(0, 30) < windAmount)
                {
                    if (flameLit)
                    {
                        //candle evidence
                        timeToKeepLit = 1f;
                        controlScript.totalLitCandles--;
                    }
                    flameLit = false;
                }    
            }
            if (bell)
            {
                if (Random.Range(0, 30) < motionAmount)
                {
                    //bell evidence
                    timeToKeepLit = 1f;
                    if (controlScript.saveScript.settingData.sounds) {
                        myAudio.Play();
                    }
                }
            }
            if (compass && lastGhostScript)
            {
                if (Random.Range(0, 30) < emfAmount)
                {
                    //compass evidence
                    timeToKeepLit = 1f;
                    Vector3 target = lastGhostScript.transform.position;
                    Vector3 needleDir = new Vector3(target.x, compassNeedle.position.y, target.z);
                    compassNeedle.LookAt(needleDir);
                    Vector3 tempRot = compassNeedle.localEulerAngles;
                    tempRot.x = 0f;
                    tempRot.z = 0f;
                    compassNeedle.localEulerAngles = tempRot;
                }
            }
            timeTillCheck = 10f;
        }
        if (myAudio)
        {
            int layer_mask = LayerMask.GetMask("ARMesh");
            int walls = Physics.RaycastAll(new Ray(_camera.transform.position, transform.position),1000f, layer_mask).Length;
            myAudio.volume = Mathf.Max(1.0f-(walls*0.3f),0f);
        }
        //if (Vector3.Distance(transform.position,placePos)>5f&&controlScript.heldEquipment!=this)
        //{
        //    RemoveAnchor();
        //    transform.position = startPos;
        //    transform.eulerAngles = startRot;
        //    CreateAnchor();
        //}
        if (uv)
        {
            if (controlScript.heldEquipment == this || myTripod!=null)
            {
                uvBeam.SetActive(true);
            }
            else
            {
                uvBeam.SetActive(false);
            }
        }
        if (motion && cooldown<=0f)
        {
            int layer_mask = LayerMask.GetMask("ARMesh","GhostEntity");
            RaycastHit[] hits = Physics.RaycastAll(new Ray(transform.position, transform.up), 20f, layer_mask);
            float closestWallDistance = 100f;
            for (int i = 0; i<hits.Length;i++)
            {
                if (hits[i].transform.tag=="ARMesh"&&hits[i].distance<closestWallDistance)
                {
                    closestWallDistance = hits[i].distance;
                }
            }
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.tag == "GhostEntity" && hits[i].distance <= closestWallDistance)
                {
                    // motion evidence
                    timeToKeepLit = 1f;
                    controlScript.popupScript.DisplayPopup(0, "Sensor has detected motion");
                    cooldown = 10f;
                }
            }
        }
        if (dowsing)
        {
            if (controlScript.heldEquipment!=this)
            {
                dowseState = 0;
                timeTillDowse = Random.Range(5f, 50f);
            }
            if (dowseState==0) {
                if (timeTillDowse > 0f)
                {
                    timeTillDowse -= 1f * Time.deltaTime;
                }
                else
                {
                    int layer_mask = LayerMask.GetMask("GhostInteraction");
                    if (Physics.Raycast(new Ray(_camera.transform.position, _camera.transform.forward),20f,layer_mask))
                    {
                        //dowsing evidence
                        timeToKeepLit = 10f;
                        dowseState = 1;
                    }
                    else
                    {
                        timeTillDowse += Random.Range(5f, 50f);
                    }
                }
            }
            if (dowseState==1)
            {
                Vector3 tempRot = dowsingRod1.transform.localEulerAngles;
                tempRot.y = Mathf.LerpAngle(tempRot.y,10f,1f*Time.deltaTime);
                dowsingRod1.transform.localEulerAngles = tempRot;
                tempRot = dowsingRod2.transform.localEulerAngles;
                tempRot.y = Mathf.LerpAngle(tempRot.y, -10f, 1f * Time.deltaTime);
                dowsingRod2.transform.localEulerAngles = tempRot;
            }
            if (dowseState == 0)
            {
                Vector3 tempRot = dowsingRod1.transform.localEulerAngles;
                tempRot.y = Mathf.LerpAngle(tempRot.y, 0f, 1f * Time.deltaTime);
                dowsingRod1.transform.localEulerAngles = tempRot;
                tempRot = dowsingRod2.transform.localEulerAngles;
                tempRot.y = Mathf.LerpAngle(tempRot.y, 0f, 1f * Time.deltaTime);
                dowsingRod2.transform.localEulerAngles = tempRot;
            }
        }
        if (compass)
        {
            Vector3 direction = (Quaternion.Euler(0f,(controlScript.locationScript.cameraDirection- controlScript.locationScript.trueNorthDirection), 0f) * Vector3.forward);
            Vector3 target = transform.position + direction  * 5f;
            Vector3 needleDir = new Vector3(target.x, compassNeedle.position.y, target.z);
            Vector3 prevRot = compassNeedle.localEulerAngles;
            compassNeedle.LookAt(needleDir);
            Vector3 gotoRot = compassNeedle.localEulerAngles;
            compassNeedle.localEulerAngles = prevRot;
            Vector3 tempRot = compassNeedle.localEulerAngles;
            tempRot.y = Mathf.LerpAngle(tempRot.y, gotoRot.y, 2f * Time.deltaTime);
            tempRot.x = 0f;
            tempRot.z = 0f;
            compassNeedle.localEulerAngles = tempRot;
        }
        if (candle)
        {
            flame.gameObject.SetActive(flameLit);
        }
        if (emf) {
            if (emfLightsOn>6f)
            {
                emfLightsOn = 6f;
            }
            SetEMFLights();
            Random.InitState((int)Time.time);
            float noise = Random.Range(-0.5f, 0.5f);
            emfLightsOn = (Mathf.PerlinNoise(transform.position.x / 5f, transform.position.z / 5f) * 4f) + noise+emfAmount;
            if (emfLightsOn >= 4)
            {
                myAudio.pitch = Mathf.Min(2f,1f + ((Mathf.RoundToInt(emfLightsOn) - 4f)));
                myAudio.volume = 0f + ((Mathf.RoundToInt(emfLightsOn) - 4f)*0.001f);
                if (!myAudio.isPlaying && controlScript.saveScript.settingData.sounds)
                {
                    myAudio.Play();
                }
            }
            else
            {
                myAudio.Stop();
            }
        }
        if (timeTillSettle > 0f)
        {
            yOffset = -999f;
            timeTillSettle = Mathf.Min(timeTillSettle+GetMovement(),1f);
            timeTillSettle -= 1f * Time.deltaTime;
            rb.constraints = RigidbodyConstraints.None;
        }
        if (myTripod != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            timeToKeepThrown = 0f;
        }
        if ((motion)&&!onDesk)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            timeToKeepThrown = 0f;
            int layer_mask = LayerMask.GetMask("ARMesh","ToolDesk");
            RaycastHit hit;
            if (yOffset == -999f)
            {
                if (Physics.Raycast(new Ray(transform.position + (Vector3.up * 2f), -Vector3.up), out hit, 4f, layer_mask))
                {
                    yOffset = transform.position.y - hit.point.y;
                }
            }
            if (motion && controlScript.heldEquipment!=this) {
                if (Physics.Raycast(new Ray(placedOriginPosition, placedOriginRotation), out hit, 100f, layer_mask))
                {
                    transform.position = hit.point;
                    transform.LookAt(transform.position + hit.normal);
                    transform.Rotate(new Vector3(90f, 0f, 0f));
                }
            }
            //else
            //{
            //    if (Physics.Raycast(new Ray(transform.position + (Vector3.up * 2f), -Vector3.up), out hit, 4f, layer_mask))
            //    {
            //        transform.position = new Vector3(hit.point.x, hit.point.y+yOffset, hit.point.z);
            //    }
            //}
        }
        if (controlScript.heldEquipment!=this && !motion && !onDesk)
        {
            if (myTripod==null) {
                int layer_mask = LayerMask.GetMask("ARMesh");
                RaycastHit hit;
                if (Physics.Raycast(new Ray(placedOriginPosition, (transform.position - placedOriginPosition)), out hit, Vector3.Distance(placedOriginPosition, transform.position) * 0.9f, layer_mask))
                {
                    transform.position = hit.point;
                    transform.LookAt(transform.position + hit.normal);
                    transform.Rotate(new Vector3(90f, 0f, 0f));
                }
            }
            else
            {
                int layer_mask = LayerMask.GetMask("ARMesh");
                RaycastHit hit;
                if (Physics.Raycast(new Ray(transform.position + (Vector3.up * 2f), -Vector3.up), out hit, 10f, layer_mask))
                {
                    Vector3 tempPos = transform.position;
                    tempPos.y = hit.point.y + distToGround;
                    transform.position = tempPos;
                }
                
            }
        }
    }
    public void LateUpdate()
    {
        if (controlScript.heldEquipment == this)
        {
            rb.velocity = new Vector3(0f, 0f, 0f);
            rb.angularVelocity = new Vector3(0f, 0f, 0f);
            SetCollision(false);
            RemoveAnchor();
            transform.position = Vector3.SmoothDamp(transform.position, controlScript.itemHolder.transform.position - (Vector3.up * 0.1f),ref moveVelocity,0.05f);
            transform.rotation = Quaternion.Euler(
                    new Vector3(
                        Mathf.SmoothDampAngle(transform.rotation.eulerAngles.x,controlScript.itemHolder.transform.rotation.eulerAngles.x,ref xRotVelocity, 0.05f),
                        Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, controlScript.itemHolder.transform.rotation.eulerAngles.y, ref yRotVelocity, 0.05f),
                        Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, controlScript.itemHolder.transform.rotation.eulerAngles.z, ref zRotVelocity, 0.05f)
                    )
                );
            //transform.SetParent(controlScript.itemHolder.transform);
            ResetVariables();
            onDesk = false;
        }
        else
        {
            SetCollision(true);
            if (gravity)
            {
                rb.velocity -= Vector3.up * 0.03f;
            }
        }
    }
    // ReSharper disable Unity.PerformanceAnalysis
    public void CreateAnchor()
    {
            RemoveAnchor();
            GameObject holder = new GameObject("Holder");
            holder.transform.position = transform.position;
            holder.transform.eulerAngles = transform.eulerAngles;
            holder.AddComponent<ARAnchor>();
            transform.parent = holder.transform;
        myAnchor = holder;
    }
    // ReSharper disable Unity.PerformanceAnalysis
    public void RemoveAnchor()
    {
        if (myAnchor != null)
        {
            transform.parent = null;
            Destroy(myAnchor.GetComponent<ARAnchor>());
            Destroy(myAnchor);
        }
    }
    public void SetCollision(bool active)
    {
        for (int i = 0; i<colliders.Count;i++)
        {
            colliders[i].enabled = active;
        }
    }
    public void ResetVariables()
    {
        noisesMade = 1;
        yOffset = -999f;
        if (!motion) {
            timeTillSettle = 1f;
        }
        else
        {
            timeTillSettle = 0f;
        }
        onDesk = false;
    }
    public float GetMovement()
    {
        float score = 0f;
        //score+= Vector3.Angle(prevRot, transform.localEulerAngles);
        score += Vector3.Distance(prevPos, transform.localPosition);
        prevRot = transform.localEulerAngles;
        prevPos = transform.localPosition;
        return (rb.velocity.sqrMagnitude+ rb.angularVelocity.sqrMagnitude);
    }
    public Color GetDarkerColor(Color oldColor,float amt)
    {
        Color newColor = new Color(oldColor.r - amt, oldColor.g - amt, oldColor.b - amt);
        return newColor;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.sqrMagnitude>0.01f && pickedUp) {
            GameObject sound = Instantiate(mySoundPrefab, transform.position, transform.rotation);
            noisesMade++;
            sound.GetComponent<SoundEffect>().mySource.volume = sound.GetComponent<SoundEffect>().mySource.volume/noisesMade;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (clipboard && inkWriting.text == "") {
            if (collision.transform.CompareTag("GhostInteraction"))
            {
                if (collision.transform.GetComponent<GhostInteraction>().motionAmount>5)
                {
                    if (Random.Range(0,100)>99)
                    {
                        inkWriting.text = "MURDER";
                    }
                }
            }
        }
        if (!onDesk &&collision.transform.CompareTag("ToolDesk")) {
            if (timeTillSettle <= 0f)
            {
                onDesk = true;
                relativeToDesk = transform.position - collision.transform.position;
            }
        }
        if (onDesk && collision.transform.CompareTag("ARMesh"))
        {
                onDesk = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (candle) {
            Equipment equipScript = other.GetComponentInParent<Equipment>();
            if (equipScript!=null && equipScript.lighter && equipScript == controlScript.heldEquipment)
            {
                if (!flameLit)
                {
                    controlScript.totalLitCandles++;
                }
                flameLit = true;
            }
            if (equipScript != null && equipScript.candle && equipScript.flameLit && equipScript == controlScript.heldEquipment)
            {
                if (!flameLit)
                {
                    controlScript.totalLitCandles++;
                }
                flameLit = true;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("GhostInteraction"))
        {
            GhostInteraction ghostScript = other.GetComponent<GhostInteraction>();
            lastGhostScript = ghostScript;
            emfAmount = ghostScript.emfAmount;
            motionAmount = ghostScript.motionAmount;
            windAmount = ghostScript.windAmount;
        }
    }

    private void SetEMFLights()
    {
        if (emfLightsOn > 0f)
        {
            emfLightsOn -= 1f * Time.deltaTime;
        }
        if (emfLightsOn >= 1)
        {
            emf1Render.material.color = emf1Color;
        }
        else
        {
            emf1Render.material.color = GetDarkerColor(emf1Color, 0.9f);
        }
        if (emfLightsOn >= 2)
        {
            emf2Render.material.color = emf2Color;
        }
        else
        {
            emf2Render.material.color = GetDarkerColor(emf2Color, 0.9f);
        }
        if (emfLightsOn >= 3)
        {
            emf3Render.material.color = emf3Color;
        }
        else
        {
            emf3Render.material.color = GetDarkerColor(emf3Color, 0.9f);
        }
        emf4Render.material.color = emfLightsOn >= 4 ? emf4Color : GetDarkerColor(emf4Color, 0.9f);
        if (emfLightsOn >= 5)
        {
            emf5Render.material.color = emf5Color;
            timeToKeepLit = 1f;
        }
        else
        {
            emf5Render.material.color = GetDarkerColor(emf5Color, 0.9f);
        }
    }
    public void ReturnToDesk()
    {
        RemoveAnchor();
        if (myTripod != null)
        {
            GameObject.Destroy(myTripod);
            myTripod = null;
        }
        if (liveCamera)
        {
            controlScript.placedCameras.Remove(this);
        }
        transform.position = startPos;
        transform.eulerAngles = startRot;
        relativeToDesk = transform.position - controlScript.toolDeskScript.transform.position;
        pickedUp = false;
        onDesk = true;
        timeTillSettle = 0f;
        CreateAnchor();
    }
}
