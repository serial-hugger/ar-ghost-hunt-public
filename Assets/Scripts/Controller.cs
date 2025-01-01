using System.Collections.Generic;
using UnityEngine;
using Niantic.Lightship.AR.Meshing;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem.XR;

public class Controller : MonoBehaviour
{
    public float globalDecayTime;

    public SaveManager saveScript;
    public Ads adScript;

    public bool testMode;

    public NotificationsOverlay popupScript;

    public MainUI mainUI;
    public EvidenceCapture captureScript;

    public bool toolDeskSpawned = false;
    public bool placingToolDesk = false;
    public float placingBlocked;
    public GameObject deskPrefab;
    public ToolDeskCursor toolDeskCursor;
    public ToolDesk toolDeskScript;
    public ARSession _session;
    public Camera activeCamera;

    public GameObject itemHolder;
    public Equipment heldEquipment;

    public Vector2 heldEquipmentOffset;

    public GameObject tripodPrefab;

    public ARSession sessionScript;
    public ARCameraManager sessionScriptCamera;

    public ARMeshManager meshManager;
    public LightshipMeshingExtension meshManagerExtension;

    public List<MeshFilter> ARMesh;

    public float pressHold = 0f;
    public Vector2 pressStart;

    public float xCeil;
    public float xFloor;
    public float yCeil;
    public float yFloor;
    public float zCeil;
    public float zFloor;

    public Text debugText;

    public GameObject interactionPrefab;

    public List<GhostInteraction> ghostAreas = new List<GhostInteraction>();

    public float timeTillAreaSpawnAttempt = 10f;

    public GameObject ghostPrefab;

    public float distanceBetweenWaypoints;
    public float distanceBetweenMasterWaypoints;

    public List<Waypoint> waypoints;
    public GameObject waypointPrefab;

    public Waypoint prevSpawnedWaypoint = null;

    public List<Waypoint> testPath = new List<Waypoint>();
    public Vector3 playerFloor;

    public List<Quaternion> prevCameraPositions = new List<Quaternion>(50);
    public Vector3 prevCameraRotation;
    public float timeTillCameraUpdate;

    public bool ghostVisionActive;

    public Light myLight;
    public GameObject arPrefab;
    public Material arMaterial;
    public Material arShadowMaterial;
    public List<Texture2D> photos;

    public Texture2D heldPhotoTex = null;
    public Texture2D prevHeldPhotoTex = null;

    public GameObject trashButton;
    public GameObject doneButton;

    public GameObject soundPage;
    public GameObject soundPageDone;
    public GameObject soundTrash;

    public Button camerasButton;

    public float timeTillWaypointGuess;
    public GameObject waypointGuesserPrefab;

    public GhostEntity ghostScript;

    public TMP_Dropdown dropdown;

    public GameObject debugMenu;

    public GhostInteraction closestGhostArea;
    public List<GhostInteraction> closeGhostAreas;

    public PhotoMediaPlayback photoPlaybackScript;
    public VideoMediaPlayback videoPlaybackScript;
    public SoundMediaPlayback soundPlaybackScript;
    public MainUI uiScript;

    public List<QuestItem> selectQuestItems = new List<QuestItem>();
    public bool deskSelectAllowed;
    public bool archiveSelectAllowed;

    public List<string> selectedEvidences = new List<string>();
    public int currentEvidenceAdd;

    public const TextureFormat photoFormat = TextureFormat.RGB565;
    public const TextureFormat videoFormat = TextureFormat.RGB565;

    public List<Equipment> placedCameras = new List<Equipment>();

    public bool canTripod;
    public string tripodFail;
    public float tripodDist;

    public bool revealingHandprint;

    public int totalLitCandles;

    public Location locationScript;

    public float analysisRewardMultiplier;

    public long timeDeskPlaced;

    public bool requestReview = false;

    public float adHeight;

    public GameObject topLeft;
    public GameObject topRight;
    public GameObject bottomLeft;
    public GameObject bottomRight;
    public GameObject center;

    public TrackedPoseDriver poseTrack;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        
        poseTrack.enabled = !Application.isEditor;
        
        deskSelectAllowed = true;
        archiveSelectAllowed = true;
        mainUI.currentMenu = "none";
        saveScript = GameObject.Find("GameManager").GetComponent<SaveManager>();
        adScript = GameObject.Find("GameManager").GetComponent<Ads>();
        locationScript = GameObject.Find("GameManager").GetComponent<Location>();
        locationScript.controlScript = this;
        saveScript.controlScript = this;
        TimeManager.SetTime();
        saveScript.SetupMessages();
        if (saveScript.settingData.highMeshing)
        {
            meshManagerExtension.MaximumIntegrationDistance = 5f;
            meshManagerExtension.VoxelSize = 0.025f;
        }
        else
        {
            meshManagerExtension.MaximumIntegrationDistance = 5f;
            meshManagerExtension.VoxelSize = 0.05f;
        }
        meshManagerExtension.TargetFrameRate = (int)saveScript.settingData.meshUpdatePerSecond;
        distanceBetweenWaypoints = saveScript.settingData.waypointDistance;
        distanceBetweenMasterWaypoints = saveScript.settingData.masterWaypointDistance;
    }
    private void Update()
    {
        //SPAWN GHOST EDITOR
        if (toolDeskSpawned && Application.isEditor) {
            if (!GameObject.FindGameObjectWithTag("GhostEntity"))
            {
                Vector3 spawnLocation = new Vector3(100,100,100);
                ghostScript = Instantiate(ghostPrefab, spawnLocation, new Quaternion()).GetComponent<GhostEntity>().GetComponent<GhostEntity>();
            }
        }
        if (placingToolDesk && !toolDeskSpawned)
        {
            int layerMask = LayerMask.GetMask("ARMesh");
            RaycastHit hit;
            if (Physics.Raycast(new Ray(_camera.transform.position, _camera.transform.forward), out hit, 2f, layerMask))
            {
                toolDeskCursor.gameObject.SetActive(true) ;
                toolDeskCursor.transform.position = hit.point;
                toolDeskCursor.transform.LookAt(new Vector3(_camera.transform.position.x, toolDeskCursor.transform.position.y, _camera.transform.position.z));
            }
            else
            {
                toolDeskCursor.gameObject.SetActive(false);
            }
        }
        else
        {
            toolDeskCursor.gameObject.SetActive(false);
        }
        if (TimeManager.GetTime() - saveScript.gameData.prevReviewRequest > System.TimeSpan.TicksPerDay * 100 && requestReview && mainUI.currentMenu == "default")
        {
            requestReview = false;
            saveScript.gameData.prevReviewRequest = TimeManager.GetTime();
            mainUI.currentMenu = "review";
        }
        if (TimeManager.validTime && (saveScript.gameData.prevDailyRewardClaimedTime + System.TimeSpan.TicksPerHour * 48) < TimeManager.GetTime())
        {
            saveScript.gameData.nextDailyReward = 0;
        }
        if (!Tutorial.inTutorial && (mainUI.currentMenu == "default"|| mainUI.currentMenu == "none") && !heldEquipment && TimeManager.validTime && saveScript.gameData.prevDailyRewardClaimedTime+System.TimeSpan.TicksPerHour*20 < TimeManager.GetTime())
        {
            mainUI.SetCurrentMenu("daily");
        }
        if (!saveScript.settingData.shadows)
        {
            myLight.shadows = LightShadows.None;
            //arPrefab.GetComponent<MeshRenderer>().sharedMaterials[0] = arMaterial;
            //arPrefab.GetComponent<MeshRenderer>().sharedMaterials[1] = null;
        }
        if (saveScript.settingData.shadows)
        {
            myLight.shadows = LightShadows.Soft;
            //arPrefab.GetComponent<MeshRenderer>().sharedMaterials[0] = arMaterial;
            //arPrefab.GetComponent<MeshRenderer>().sharedMaterials[1] = arShadowMaterial;
        }
        placingBlocked -= 1f * Time.deltaTime;
        if (heldEquipment)
        {
            if (heldEquipment.liveCamera) {
                int layerMask = LayerMask.GetMask("ARMesh");
                RaycastHit hit;
                canTripod = false;
                if (Physics.Raycast(new Ray(_camera.transform.position + (_camera.transform.forward * 0.5f), -Vector3.up),out hit, 2f, layerMask))
                {
                    if (hit.distance>0.3f&&!Physics.Raycast(new Ray(_camera.transform.position, -Vector3.forward), 1f, layerMask)&& !Physics.Raycast(new Ray(_camera.transform.position, Vector3.forward), 1f, layerMask)&& !Physics.Raycast(new Ray(_camera.transform.position, -Vector3.right), 1f, layerMask)&& !Physics.Raycast(new Ray(_camera.transform.position, Vector3.right), 1f, layerMask))
                    {
                        float zAmount = 10f;
                        float xAmount = 45f;
                        if ((Mathf.Abs(_camera.transform.localEulerAngles.z) > 0f && Mathf.Abs(_camera.transform.localEulerAngles.z)< zAmount) || (Mathf.Abs(_camera.transform.localEulerAngles.z) > 360f- zAmount && Mathf.Abs(_camera.transform.localEulerAngles.z) <360f)) {
                            if ((Mathf.Abs(_camera.transform.localEulerAngles.x) > 0f && Mathf.Abs(_camera.transform.localEulerAngles.x) < xAmount) || (Mathf.Abs(_camera.transform.localEulerAngles.x) > 360f - xAmount && Mathf.Abs(_camera.transform.localEulerAngles.x) < 360f)) {
                                canTripod = true;
                                tripodDist = hit.distance;
                            }
                            else
                            {
                                tripodFail = "PITCHED TO FAR";
                            }
                        }
                        else
                        {
                            tripodFail = "TILTED TO FAR";
                        }
                    }
                    else
                    {
                        tripodFail = "NOT ENOUGH SPACE";
                    }
                }
                else
                {
                    tripodFail = "TOO FAR FROM GROUND";
                }
            }
            if (heldEquipment.uv)
            {
                int layerMask = LayerMask.GetMask("ARMesh");
                RaycastHit hit;
                canTripod = false;
                if (Physics.Raycast(new Ray(heldEquipment.transform.position, -Vector3.up),out hit, 2f, layerMask))
                {
                    if (hit.distance > 0.1f && !Physics.Raycast(new Ray(heldEquipment.transform.position, -Vector3.forward), 1f, layerMask) && !Physics.Raycast(new Ray(heldEquipment.transform.position, Vector3.forward), 1f, layerMask) && !Physics.Raycast(new Ray(heldEquipment.transform.position, -Vector3.right), 1f, layerMask) && !Physics.Raycast(new Ray(heldEquipment.transform.position, Vector3.right), 1f, layerMask))
                    {
                        canTripod = true;
                        tripodDist = hit.distance;
                    }
                }
            }
        }
        if (ghostScript && testMode)
        {
            debugMenu.SetActive(true) ;
        }
        else
        {
            debugMenu.SetActive(false);
        }
        //guess waypoint
        if (toolDeskSpawned) {
            timeTillWaypointGuess -= 1f * Time.deltaTime;
        }
        if (timeTillWaypointGuess < 0f)
        {
            UpdateCloseAreas();
            timeTillWaypointGuess = 1f;
            Vector3 guessPosition = new Vector3(_camera.transform.position.x+Random.Range(distanceBetweenWaypoints * -5f, distanceBetweenWaypoints * 5f), _camera.transform.position.y, _camera.transform.position.z + Random.Range(distanceBetweenWaypoints*-5f, distanceBetweenWaypoints*5f));
            RaycastHit hit;
            if (Physics.Raycast(new Ray(guessPosition, -Vector3.up), out hit))
            {
                WaypointGuesser newGuess = Instantiate(waypointGuesserPrefab, hit.point, new Quaternion()).GetComponent<WaypointGuesser>();
                newGuess.controlScript = this;
            }
        }

        if (!Tutorial.inTutorial && !Application.isEditor) {
            //spawn haunted areas
            if (toolDeskSpawned) {
                timeTillAreaSpawnAttempt -= 1f * Time.deltaTime;
            }
            if (timeTillAreaSpawnAttempt <= 0f)
            {
                timeTillAreaSpawnAttempt = 10f;
                if ((Random.Range(0, 300) < Mathf.Min(75, waypoints.Count) || (TimeManager.GetTime()-timeDeskPlaced)>System.TimeSpan.TicksPerMinute*3) && waypoints.Count > 1 && ghostAreas.Count <= (waypoints.Count / 20))
                {
                    //SPAWN GHOST
                    if (!GameObject.FindGameObjectWithTag("GhostEntity"))
                    {
                        Vector3 spawnLocation = GetRandomWaypoint().floor;
                        ghostScript = Instantiate(ghostPrefab, spawnLocation, new Quaternion()).GetComponent<GhostEntity>().GetComponent<GhostEntity>();
                    }


                    //SPAWN GHOST AREA
                    GameObject newArea;
                    if (toolDeskSpawned)
                    {
                        int randomMesh = Random.Range(0, ARMesh.Count);
                        Vector3 rawPoint = GetRandomWaypoint().transform.position;
                        newArea = Instantiate(interactionPrefab, rawPoint, new Quaternion());
                    }
                    else
                    {
                        newArea = Instantiate(interactionPrefab, new Vector3(Random.Range(xFloor, xCeil), Random.Range(yFloor, yCeil), Random.Range(zFloor, zCeil)), new Quaternion());
                    }
                    ghostAreas.Add(newArea.GetComponent<GhostInteraction>());
                }
            }
        }
        //Saves camera position history
        timeTillCameraUpdate -= 1f * Time.deltaTime;
        if (timeTillCameraUpdate<=0f)
        {
            timeTillCameraUpdate = 0.1f;
        }
        if (prevCameraPositions.Count>=50) {
            prevCameraPositions.RemoveAt(0);
        }
        prevCameraPositions.Add(_camera.transform.rotation);
        if (prevCameraPositions.Count>=50) {
            if (heldEquipment) {
                if (heldEquipment.uvBeam) {
                    itemHolder.transform.rotation = Quaternion.Lerp(itemHolder.transform.rotation, prevCameraPositions[30] *= Quaternion.Euler(20f, 180f, 0), 1f * Time.deltaTime);
                }
                else
                {
                    itemHolder.transform.rotation = _camera.transform.rotation * Quaternion.Euler(20f, 180f, 0);
                }
            }
        }

        if (!GetClosestWaypointInRange(distanceBetweenWaypoints,_camera.transform.position,waypoints)&&toolDeskSpawned)
        {
            List<Waypoint> rangeWay = GetWaypointsInRange(distanceBetweenMasterWaypoints, _camera.transform.position, waypoints);
            Waypoint closestMaster = null;
            for (int i = 0; i< rangeWay.Count;i++)
            {
                if (rangeWay[i].isMaster)
                {
                    closestMaster = rangeWay[i];
                }
            }
            Waypoint newWay = Instantiate(waypointPrefab, _camera.transform.position, new Quaternion()).GetComponent<Waypoint>();
            if (closestMaster!=null)
            {
                newWay.transform.parent = closestMaster.transform;
            }
            else
            {
                newWay.isMaster = true;
            }
            newWay.controlScript = this;
            newWay.SetFloor();
            waypoints.Add(newWay);
            prevSpawnedWaypoint = newWay;
        }
        if (toolDeskSpawned)
        {
            xCeil = Mathf.Max(xCeil,activeCamera.transform.position.x);
            xFloor = Mathf.Min(xFloor, activeCamera.transform.position.x);
            yCeil = Mathf.Max(yCeil, activeCamera.transform.position.y);
            yFloor = Mathf.Min(yFloor, activeCamera.transform.position.y);
            zCeil = Mathf.Max(zCeil, activeCamera.transform.position.z);
            zFloor = Mathf.Min(zFloor, activeCamera.transform.position.z);
        }
        if (_session == null)
        {
            return;
        }
        /*if (_session.CurrentFrame.LightEstimate!=null)
        {

            if (Application.platform == RuntimePlatform.Android) {
                myLight.intensity = _session.CurrentFrame.LightEstimate.AmbientIntensity;
                myLight.color = new Color(_session.CurrentFrame.LightEstimate.ColorCorrection[1], _session.CurrentFrame.LightEstimate.ColorCorrection[0], _session.CurrentFrame.LightEstimate.ColorCorrection[2]);
            }
            else
            {
                myLight.intensity = _session.CurrentFrame.LightEstimate.AmbientIntensity/1000f;
                myLight.colorTemperature = _session.CurrentFrame.LightEstimate.AmbientColorTemperature;
            }
        }*/
        if (Application.isEditor)
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0))
            {
                Touch touch = new Touch();
                touch.position = UnityEngine.Input.mousePosition;
                TouchBegan(touch);
            }
            if (UnityEngine.Input.GetKeyUp(KeyCode.Mouse0))
            {
                Touch touch = new Touch();
                touch.position = UnityEngine.Input.mousePosition;
                TouchEnded(touch);
            }
        }
        if (Niantic.Lightship.AR.Input.touchCount <= 0 && UnityEngine.Input.touchCount<=0 && (!Application.isEditor|| UnityEngine.Input.GetKey(KeyCode.Mouse0)))
        {
            return;
        }
        pressHold += 1f * Time.deltaTime;
        if (UnityEngine.Input.touchCount >0) {
            var touch = UnityEngine.Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                TouchBegan(touch);
            }
            if (touch.phase == TouchPhase.Ended)
            {
                TouchEnded(touch);
            }
        }
        else
        {
            if (Niantic.Lightship.AR.Input.touchCount>0) {
                var touch = Niantic.Lightship.AR.Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    TouchBegan(touch);
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    TouchEnded(touch);
                }
            }
        }
    }
    public void UndoAllParanormal(List<EvidenceCapture.FocalEvidence> evidences)
    {
        if (evidences!=null) {
            bool clipboardCapture = false;
            bool ghostCapture = false;
            bool effectCapture = false;
            bool handprintCapture = false;
            for (int i = 0; i < evidences.Count; i++)
            {
                if (evidences[i].type == "clipboard")
                {
                    clipboardCapture = true;
                }
                if (evidences[i].type == "handprint")
                {
                    handprintCapture = true;
                }
                if (evidences[i].type == "ghost" || evidences[i].type == "ghosthead" || evidences[i].type == "ghostbody" || evidences[i].type == "ghostfoot" || evidences[i].type == "hand")
                {
                    ghostCapture = true;
                }
                if (evidences[i].type == "ghostorb" || evidences[i].type == "ghosttrail")
                {
                    effectCapture = true;
                }
            }
            if (clipboardCapture) {
                GameObject[] equipment = GameObject.FindGameObjectsWithTag("Equipment");
                for (int i = 0; i < equipment.Length; i++)
                {
                    Equipment equipScript = equipment[i].GetComponent<Equipment>();
                    if (equipScript != null && equipScript.clipboard)
                    {
                        equipScript.inkWriting.text = "";
                    }
                }
            }
            if (ghostCapture || effectCapture || handprintCapture) {
                GameObject[] interactions = GameObject.FindGameObjectsWithTag("GhostInteraction");
                for (int i = 0; i < interactions.Length; i++)
                {
                    if (Random.Range(0, 1000) > 100) {
                        GhostInteraction interactionScript = interactions[i].GetComponent<GhostInteraction>();
                        interactionScript.lifetime = -1f;
                    }
                }
            }
            if (effectCapture) {
                GameObject[] effects = GameObject.FindGameObjectsWithTag("GhostEffects");
                for (int i = 0; i < effects.Length; i++)
                {
                    if (Random.Range(0, 1000) > 100)
                    {
                        GameObject.Destroy(effects[i]);
                    }
                }
            }
            if (ghostCapture) {
                GameObject[] ghosts = GameObject.FindGameObjectsWithTag("GhostEntity");
                for (int i = 0; i < ghosts.Length; i++)
                {
                    if (Random.Range(0, 1000) > 100)
                    {
                        GhostEntity ghostScript = ghosts[i].GetComponent<GhostEntity>();
                        if (ghostScript.staticPhotoOnly)
                        {
                            GameObject.Destroy(ghosts[i]);
                        }
                        else
                        {
                            ghostScript.animatorScript.visibility = 0f;
                            if (!ghostScript.gone)
                            {
                                ghostScript.gone = true;
                            }
                        }
                    }
                }
            }
            if (handprintCapture)
            {
                for (int i = 0; i < waypoints.Count; i++)
                {
                    if (waypoints[i].placedHandprint && Random.Range(0, 1000) > 100)
                    {
                        waypoints[i].placedHandprint = false;
                        GameObject.Destroy(waypoints[i].myHandprint);
                    }
                }
            }
        }
    }
    public void UpdateMeshList()
    {
        ARMesh.Clear();
        GameObject[] tempObj = GameObject.FindGameObjectsWithTag("ARMesh");
        for (int i = 0; i < tempObj.Length;i++)
        {
            ARMesh.Add(tempObj[i].GetComponent<MeshFilter>());
        }
    }


    // Update is called once per frame
    private void TouchBegan(Touch touch)
    {
        pressStart = touch.position;
        pressHold = 0f;
    }
    private void TouchEnded(Touch touch)
    {
        //speed up tutorial
        if (Tutorial.inTutorial && pressHold<1f && Vector2.Distance(touch.position, pressStart) < (Screen.height / 20f))
        {
            if (Tutorial.currentChar < Tutorial.tutorial[Tutorial.tutorialPhase].captionText.Length - 1)
            {
                Tutorial.speedingText = true;
            }
            else
            {
                if (Tutorial.autoProgress < 1f && Tutorial.tutorial[Tutorial.tutorialPhase].advanceAction == "auto")
                {
                    Tutorial.autoProgress = 1f;
                }
            }
        }
        if (placingBlocked<=0f&&!EventSystem.current.IsPointerOverGameObject() && (mainUI.currentMenu == "default" || !toolDeskSpawned)) {
            if (placingToolDesk && !toolDeskSpawned)
            {
                if (!Tutorial.inTutorial || Tutorial.tutorial[Tutorial.tutorialPhase].advanceAction == "deskplace") {
                    if (true) {
                        if (toolDeskCursor.valid || Application.isEditor) {
                            toolDeskScript = Instantiate(deskPrefab, toolDeskCursor.transform.position, toolDeskCursor.transform.rotation).GetComponent<ToolDesk>();

                            toolDeskSpawned = true;
                            timeDeskPlaced = TimeManager.GetTime();
                            //ENABLE UI
                            mainUI.currentMenu = "default";
                            xCeil = activeCamera.transform.position.x;
                            xFloor = activeCamera.transform.position.x;
                            yCeil = activeCamera.transform.position.y;
                            yFloor = activeCamera.transform.position.y;
                            zCeil = activeCamera.transform.position.z;
                            zFloor = activeCamera.transform.position.z;

                            locationScript.SpawnMappingSystem();
                            locationScript.SetHeading();
                            locationScript.cameraDirection = _camera.transform.eulerAngles.y;

                            Tutorial.AdvanceTutorial();
                        }
                        else
                        {
                            popupScript.DisplayError("INVALID PLACEMENT");
                        }
                    }
                    else
                    {
                        if (UnityEngine.Input.location.status == LocationServiceStatus.Failed) {
                            popupScript.DisplayError("GPS FAILED");
                        }
                        if (UnityEngine.Input.location.status == LocationServiceStatus.Initializing)
                        {
                            popupScript.DisplayError("GPS INITIALIZING");
                        }
                        if (UnityEngine.Input.location.status == LocationServiceStatus.Stopped)
                        {
                            popupScript.DisplayError("GPS STOPPED");
                        }
                    }
                }
            }
            else
            {
                Ray ray = _camera.ScreenPointToRay(touch.position);
                RaycastHit hit;
                int layer_mask = LayerMask.GetMask("ARMesh", "ToolDesk", "Equipment", "CursedItem");
                if (Physics.Raycast(ray, out hit, layer_mask))
                {
                    if (!Tutorial.inTutorial) {
                        if (hit.transform.tag == "CursedItem")
                        {
                            hit.transform.GetComponent<CursedItem>().collected = true;
                            return;
                        }
                        if (hit.transform.tag == "Media")
                        {
                            mainUI.prevMenu = "default";
                            PhotoMedia photoScript = hit.transform.GetComponent<PhotoMedia>();
                            VideoMedia videoScript = hit.transform.GetComponent<VideoMedia>();
                            SoundMedia audioScript = hit.transform.GetComponent<SoundMedia>();
                            Instantiate(soundPage, _camera.transform.position, transform.rotation);
                            if (photoScript != null)
                            {
                                if (saveScript.gameData.deskEvidence.Contains(photoScript.myEvidence)) {
                                    uiScript.currentMenu = "playbackphoto";
                                    photoPlaybackScript.myEvidence = photoScript.myEvidence;
                                    photoPlaybackScript.prevEvidence = null;
                                }
                            }
                            if (videoScript != null)
                            {
                                if (saveScript.gameData.deskEvidence.Contains(videoScript.myEvidence))
                                {
                                    uiScript.currentMenu = "playbackvideo";
                                    videoPlaybackScript.myEvidence = videoScript.myEvidence;
                                    videoPlaybackScript.prevEvidence = null;
                                }
                            }
                            if (audioScript != null)
                            {
                                if (saveScript.gameData.deskEvidence.Contains(audioScript.myEvidence))
                                {
                                    uiScript.currentMenu = "playbacksound";
                                    soundPlaybackScript.myEvidence = audioScript.myEvidence;
                                    soundPlaybackScript.prevEvidence = null;
                                }
                            }
                            return;
                        }
                    }
                    if (heldEquipment == null)
                    {
                        if (hit.transform.tag == "Equipment")
                        {
                            if (!Tutorial.inTutorial || hit.transform.name.ToLower().Contains(Tutorial.tutorial[Tutorial.tutorialPhase].advanceAction)) {
                                if (hit.distance < 1.5f)
                                {
                                    Tutorial.AdvanceTutorial();
                                    heldEquipment = hit.transform.GetComponent<Equipment>();
                                    if (heldEquipment.myTripod != null)
                                    {
                                        GameObject.Destroy(heldEquipment.myTripod);
                                        heldEquipment.myTripod = null;
                                    }
                                    if (heldEquipment.liveCamera)
                                    {
                                        placedCameras.Remove(heldEquipment);
                                    }
                                    return;
                                }
                                else
                                {
                                    popupScript.DisplayError("TOO FAR AWAY");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (pressHold < 1f) {
                            if (Vector2.Distance(touch.position, pressStart) < (Screen.height / 20f)) {
                                if (!Tutorial.inTutorial||Tutorial.tutorial[Tutorial.tutorialPhase].advanceAction=="place") {
                                    if (hit.distance < 1.5f)
                                    {
                                        if (hit.transform.tag == "ToolDesk")
                                        {
                                            Tutorial.AdvanceTutorial();
                                            heldEquipment.transform.position = hit.point;
                                            heldEquipment.transform.eulerAngles = new Vector3(0f, heldEquipment.transform.eulerAngles.y, 0f);
                                            if (heldEquipment.motion)
                                            {
                                                heldEquipment.onDesk = true;
                                                heldEquipment.relativeToDesk = heldEquipment.transform.position - toolDeskScript.transform.position;
                                            }
                                            heldEquipment.CreateAnchor();
                                            heldEquipment = null;
                                            return;
                                        }
                                        if (hit.transform.CompareTag("ARMesh"))
                                        {
                                            Tutorial.AdvanceTutorial();
                                            if (!heldEquipment.motion)
                                            {
                                                heldEquipment.transform.position = hit.point + (Vector3.up * 0.2f);
                                                heldEquipment.transform.eulerAngles = new Vector3(0f, heldEquipment.transform.eulerAngles.y, 0f);
                                                heldEquipment.CreateAnchor();
                                                heldEquipment.placedOriginPosition = ray.origin;
                                                heldEquipment = null;
                                                return;
                                            }
                                            else
                                            {
                                                heldEquipment.transform.position = hit.point;
                                                heldEquipment.transform.LookAt(heldEquipment.transform.position + hit.normal);
                                                heldEquipment.transform.Rotate(new Vector3(90f, 0f, 0f));
                                                heldEquipment.CreateAnchor();
                                                heldEquipment.placedOriginPosition = ray.origin;
                                                heldEquipment.placedOriginRotation = ray.direction;
                                                heldEquipment = null;
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        popupScript.DisplayError("TOO FAR AWAY");
                                    }
                                }
                            }
                        }
                    }
                }
                if (pressHold < 1f && Vector2.Distance(touch.position, pressStart) >= (Screen.height / 20f))
                {
                    if (heldEquipment) {
                        if (!Tutorial.inTutorial || Tutorial.tutorial[Tutorial.tutorialPhase].advanceAction == "throw")
                        {
                            Tutorial.AdvanceTutorial();
                            if (!heldEquipment.motion)
                            {
                                heldEquipment.CreateAnchor();
                                var tempHeld = heldEquipment;
                                heldEquipment.placedOriginPosition = ray.origin;
                                heldEquipment = null;
                                tempHeld.rb.AddForce(-tempHeld.transform.forward * ((((touch.position.y - pressStart.y) / Screen.height) * 100f) * (1f - pressHold)));
                                tempHeld.rb.AddForce(-tempHeld.transform.right * ((((touch.position.x - pressStart.x) / Screen.width) * 100f) * (1f - pressHold)));
                                return;
                            }
                            else
                            {
                                popupScript.DisplayError("NOT THROWABLE");
                            }
                        }
                    }
                    else
                    {
                        //testMode = !testMode;
                    }
                }
            }
        }
    }
    public void TripodHeldEquipment()
    {
        if (!Tutorial.inTutorial||(Tutorial.tutorial[Tutorial.tutorialPhase].advanceAction=="tripod")||(Tutorial.tutorial[Tutorial.tutorialPhase].advanceAction == "tripodreveal"&&revealingHandprint)) {
            if (heldEquipment != null && (heldEquipment.liveCamera || heldEquipment.uv))
            {
                if (canTripod) {
                    Tutorial.AdvanceTutorial();
                    heldEquipment.distToGround = tripodDist;
                    if (heldEquipment.liveCamera)
                    {
                        placedCameras.Add(heldEquipment);
                        heldEquipment.transform.position = _camera.transform.position + (_camera.transform.forward * 0.5f);
                        heldEquipment.transform.eulerAngles = new Vector3(-_camera.transform.eulerAngles.x, _camera.transform.eulerAngles.y + 180f, -_camera.transform.eulerAngles.z);
                        heldEquipment.CreateAnchor();
                        Tripod tripodScript = Instantiate(tripodPrefab, heldEquipment.transform.position, new Quaternion()).GetComponent<Tripod>();
                        heldEquipment.myTripod = tripodScript.gameObject;
                        tripodScript.attached = heldEquipment.gameObject;

                        float scaleAmt = 960f / _camera.pixelHeight;

                        _camera.cullingMask = captureScript.liveCameraSettings;

                        _camera.targetTexture = new RenderTexture(Mathf.FloorToInt(_camera.pixelWidth * scaleAmt), Mathf.FloorToInt(_camera.pixelHeight * scaleAmt), 24);

                        RenderTexture activeRenderTexture = RenderTexture.active;
                        RenderTexture.active = _camera.targetTexture;

                        _camera.Render();

                        Texture2D image = new Texture2D(_camera.targetTexture.width, _camera.targetTexture.height, photoFormat, false);
                        image.ReadPixels(new Rect(0, 0, _camera.targetTexture.width, _camera.targetTexture.height), 0, 0);
                        image.Apply();

                        heldEquipment.cameraImage.texture = image;

                        _camera.targetTexture = null;
                        _camera.cullingMask = captureScript.mainSettings;

                        heldEquipment.CreateAnchor();

                        heldEquipment = null;
                        return;
                    }
                    if (heldEquipment.uv)
                    {
                        heldEquipment.CreateAnchor();
                        Tripod tripodScript = Instantiate(tripodPrefab, heldEquipment.transform.position, new Quaternion()).GetComponent<Tripod>();
                        heldEquipment.myTripod = tripodScript.gameObject;
                        tripodScript.attached = heldEquipment.gameObject;
                        heldEquipment.CreateAnchor();
                        heldEquipment = null;
                        return;
                    }
                }
                else
                {
                    popupScript.DisplayError(tripodFail);
                }
            }
        }
    }
    public Waypoint GetRandomWaypoint()
    {
        return waypoints[Random.Range(0,waypoints.Count)];
    }
    public List<Waypoint> GetPathOfWaypoints(Vector3 startingPos, Vector3 endingPos, List<Waypoint> listWaypoints)
    {
        bool pathFound = false;
        List<Waypoint> neitherNodes = new List<Waypoint>(listWaypoints);
        List<Waypoint> openNodes = new List<Waypoint>();
        List<Waypoint> closedNodes = new List<Waypoint>();
        Waypoint startingWay = GetClosestWaypoint(startingPos, neitherNodes);
        Waypoint endingWay = GetClosestWaypoint(endingPos, neitherNodes);
        neitherNodes.Remove(startingWay);
        openNodes.Add(startingWay);
        startingWay.gCost = 0f;
        startingWay.hCost = Vector3.Distance(startingWay.transform.position, endingWay.transform.position);
        startingWay.SetFCost(startingWay);
        int layerMask = LayerMask.GetMask("ARMesh");
        while (!pathFound)
        {
            Waypoint current = openNodes[0];
            for (int i = 0;i<openNodes.Count;i++)
            {
                if (openNodes[i].fCost < current.fCost)
                {
                    current = openNodes[i];
                }
            }
            openNodes.Remove(current);
            closedNodes.Add(current);
            if (current == endingWay)
            {
                pathFound = true;
                List<Waypoint> path = new List<Waypoint>();
                Waypoint currentRetrace = current;
                while (currentRetrace!=startingWay)
                {
                    path.Add(currentRetrace);
                    currentRetrace = currentRetrace.openerNode;
                }
                path.Add(startingWay);
                path.Reverse();
                return path;
            }
            List<Waypoint> neighbors = new List<Waypoint>();
            neighbors.AddRange(GetWaypointsInRange(10f,current.transform.position,neitherNodes));
            neighbors.AddRange(GetWaypointsInRange(10f, current.transform.position, openNodes));
            for (int n = 0; n< neighbors.Count;n++)
            {
                if (!(current.myClosest == neighbors[n] || neighbors[n].myClosest == current)) {
                    if (Physics.Linecast(current.transform.position, neighbors[n].transform.position, layerMask))
                    {
                        continue;
                    }
                }
                if (neighbors[n].fCost< current.fCost || !openNodes.Contains(neighbors[n]))
                {
                    neighbors[n].gCost = Vector3.Distance(startingWay.floor, neighbors[n].floor);
                    neighbors[n].hCost = Vector3.Distance(endingWay.floor, neighbors[n].floor);
                    neighbors[n].SetFCost(current);
                    neighbors[n].openerNode = current;
                    if (!openNodes.Contains(neighbors[n]))
                    {
                        openNodes.Add(neighbors[n]);
                        neitherNodes.Remove(neighbors[n]);
                    }
                }
            }
        }
        return null;
    }
    public List<Waypoint> GetWaypointsInRange(float range, Vector3 position,List<Waypoint> listWaypoints)
    {
        List<Waypoint> closeWaypoints = new List<Waypoint>();
        foreach (var t in listWaypoints)
        {
            Vector3 newWayPos = t.transform.position;
            Vector3 cameraPos = position;
            if (Vector3.Distance(newWayPos, cameraPos) <= range)
            {
                closeWaypoints.Add(t);
            }
        }
        return closeWaypoints;
    }
    public Waypoint GetRandomWaypointInRange(float range, Vector3 position, List<Waypoint> listWaypoints)
    {
        List<Waypoint> tempWaypoints = GetWaypointsInRange(range,position,listWaypoints);
        if (tempWaypoints.Count > 0)
        {
            return tempWaypoints[Random.Range(0, tempWaypoints.Count - 1)];
        }
        else
        {
            return null;
        }
    }
    public List<Waypoint> GetWaypointsOutsideRange(float range, Vector3 position, List<Waypoint> listWaypoints)
    {
        List<Waypoint> closeWaypoints = new List<Waypoint>();
        foreach (var t in listWaypoints)
        {
            Vector3 newWayPos = t.transform.position;
            Vector3 cameraPos = position;
            if (Vector3.Distance(newWayPos, cameraPos) > range)
            {
                closeWaypoints.Add(t);
            }
        }
        return closeWaypoints;
    }
    public Waypoint GetRandomWaypointOutsideRange(float range, Vector3 position, List<Waypoint> listWaypoints)
    {
        List<Waypoint> tempWaypoints = GetWaypointsOutsideRange(range, position, listWaypoints);
        if (tempWaypoints.Count>0) {
            return tempWaypoints[Random.Range(0, tempWaypoints.Count - 1)];
        }
        else
        {
            return GetRandomWaypoint();
        }
    }
    public Waypoint GetClosestWaypointInRange(float range,Vector3 position, List<Waypoint> listWaypoints)
    {
        Waypoint currentClosest = null;
        foreach (var t in listWaypoints)
        {
            Vector3 newWayPos = t.transform.position;
            Vector3 oldWayPos = new Vector3(0f,0f,0f);
            if (currentClosest) {
                oldWayPos = currentClosest.transform.position;
            }
            Vector3 cameraPos = position;
            if (!currentClosest || Vector3.Distance(newWayPos,cameraPos)< Vector3.Distance(oldWayPos, cameraPos))
            {
                if (Vector3.Distance(newWayPos, cameraPos)<range)
                {
                    currentClosest = t;
                }
            }
        }
        return currentClosest;
    }
    public Waypoint GetClosestWaypoint(Vector3 position, List<Waypoint> listWaypoints)
    {
        Waypoint currentClosest = null;
        for (int i = 0; i < listWaypoints.Count; i++)
        {
            Vector3 newWayPos = listWaypoints[i].transform.position;
            Vector3 oldWayPos = new Vector3(0f, 0f, 0f);
            if (currentClosest)
            {
                oldWayPos = currentClosest.transform.position;
            }
            Vector3 cameraPos = position;
            if (!currentClosest || Vector3.Distance(newWayPos, cameraPos) < Vector3.Distance(oldWayPos, cameraPos))
            {
                if (true)
                {
                    currentClosest = listWaypoints[i];
                }
            }
        }
        return currentClosest;
    }
    public void SetPlayerFloor()
    {
        int layerMask = LayerMask.GetMask("ARMesh");
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, -Vector3.up), out hit, 2f, layerMask))
        {
            playerFloor = hit.point;
        }
    }
    public Color GetLightColor()
    {
        float rChannel = 0.5f;//_session.CurrentFrame.LightEstimate.ColorCorrection[0];
        float gChannel = 0.5f;//_session.CurrentFrame.LightEstimate.ColorCorrection[1];
        float bChannel = 0.5f;//_session.CurrentFrame.LightEstimate.ColorCorrection[2];
        if (rChannel > 1f) {
            gChannel -= rChannel - 1f;
            bChannel -= rChannel - 1f;
            rChannel -= 1f;
        }
        if (bChannel > 1f)
        {
            gChannel -= bChannel - 1f;
            rChannel -= bChannel - 1f;
            bChannel -= 1f;
        }
        return new Color(Mathf.Max(rChannel,0f), Mathf.Max(gChannel, 0f), Mathf.Max(bChannel, 0f));
    }
    public void AppearGhost()
    {
        ghostScript.animatorScript.maxVisibility = 1f;
        ghostScript.animatorScript.visibility = 1f;
        ghostScript.gone = false;
    }
    public void HereGhost()
    {
        ghostScript.transform.position = GetClosestWaypoint(_camera.transform.position,waypoints).floor;
    }
    public void ChangeGhost()
    {
        ghostScript.doing = dropdown.options[dropdown.value].text;
        ghostScript.GeneratePath();
    }

    private void UpdateCloseAreas()
    {
        closeGhostAreas.Clear();
        closestGhostArea = null;
        float closestDistance = 10000f;
        for (int i = 0; i<ghostAreas.Count;i++)
        {
            float dist = Vector3.Distance(_camera.transform.position, ghostAreas[i].transform.position);
            if (dist<=20f)
            {
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestGhostArea = ghostAreas[i];
                }
                closeGhostAreas.Add(ghostAreas[i]);
            }
        }

    }
    public void StartPlacingDesk()
    {
        placingToolDesk = true;
        placingBlocked = 1f;
        Instantiate(uiScript.openSound, _camera.transform.position, transform.rotation);
    }
    public string GetRandomEvidenceType()
    {
        List<string> evidences = new List<string> { "clipboard", "handprint", "ghost", "ghostorb", "thrown", "photo", "video", "sound" };
        return evidences[UnityEngine.Random.Range(0, evidences.Count)];
    }
    public void UnplaceDesk()
    {
        GameObject[] equipments = GameObject.FindGameObjectsWithTag("Equipment");
        foreach (var t in equipments)
        {
            GameObject.Destroy(t);
        }
        GameObject.Destroy(toolDeskScript.gameObject);
        toolDeskScript = null;
        placingToolDesk = false;
        toolDeskSpawned = false;
        uiScript.currentMenu = "none";
        StartPlacingDesk();
    }
    public void ReplaceItems()
    {
        GameObject[] equipments = GameObject.FindGameObjectsWithTag("Equipment");
        foreach (var t in equipments)
        {
            Equipment equipScript = t.GetComponent<Equipment>();
            if (equipScript!=null) {
                equipScript.ReturnToDesk();
            }
        }
    }
}
