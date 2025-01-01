using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ToolDesk : MonoBehaviour
{

    public Controller controlScript;
    public bool spawnedEquipment;

    public GameObject clipboardPrefab;
    public Transform clipboard1Spawn;
    public Transform clipboard2Spawn;
    public bool clipboard1Spawned;
    public bool clipboard2Spawned;
    public GameObject motionPrefab;
    public Transform motion1Spawn;
    public Transform motion2Spawn;
    public Transform motion3Spawn;
    public Transform motion4Spawn;
    public bool motion1Spawned;
    public bool motion2Spawned;
    public bool motion3Spawned;
    public bool motion4Spawned;
    public GameObject EMFPrefab;
    public Transform EMFSpawn;
    public bool EMFSpawned;
    public GameObject candlePrefab;
    public Transform candle1Spawn;
    public Transform candle2Spawn;
    public Transform candle3Spawn;
    public GameObject lighterPrefab;
    public Transform lighterSpawn;
    public bool candlesSpawned;
    public GameObject compassPrefab;
    public Transform compassSpawn;
    public bool compassSpawned;
    public GameObject dowsingPrefab;
    public Transform dowsingSpawn;
    public bool dowsingSpawned;
    public GameObject bellPrefab;
    public Transform bellSpawn;
    public bool bellSpawned;
    public GameObject uvPrefab;
    public Transform uv1Spawn;
    public Transform uv2Spawn;
    public bool uv1Spawned;
    public bool uv2Spawned;
    public GameObject cameraPrefab;
    public Transform camera1Spawn;
    public Transform camera2Spawn;
    public Transform camera3Spawn;
    public Transform camera4Spawn;
    public bool camera1Spawned;
    public bool camera2Spawned;
    public bool camera3Spawned;
    public bool camera4Spawned;

    public List<BoxCollider> colliders;

    public GameObject evidence1Holder;
    public GameObject evidence2Holder;
    public GameObject evidence3Holder;
    public GameObject evidence4Holder;
    public GameObject evidence5Holder;

    public Texture2D prevPic1;
    public string prevLabel1;

    public GameObject photographPrefab;
    public GameObject dvdPrefab;
    public GameObject cassettePrefab;

    public List<GameObject> evidences;

    public int prevShopAmt = 0;

    public RawImage cameraImage;
    public RawImage renderImage;
    public RawImage detectionImage;

    public AspectRatioFitter cameraFitter;
    public AspectRatioFitter renderFitter;
    public AspectRatioFitter detectionFitter;

    public int currentCameraViewing;

    public Text cameraLabel;

    public Vector3 locationStartPosition;
    public float locationStartLat;
    public float locationStartLon;

    // Start is called before the first frame update
    void Start()
    {
        controlScript = GameObject.Find("Controller").GetComponent<Controller>();
        CreateAnchor();
        UpdateEvidence();

        locationStartPosition = Camera.main.transform.position;
        locationStartLat = Niantic.Lightship.AR.Input.location.lastData.latitude;
        locationStartLat = Niantic.Lightship.AR.Input.location.lastData.longitude;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCameraViewing > controlScript.placedCameras.Count-1)
        {
            currentCameraViewing = 0;
        }
        if (controlScript.placedCameras.Count > 0)
        {
            cameraImage.gameObject.SetActive(true);
            renderImage.gameObject.SetActive(true);
            detectionImage.gameObject.SetActive(true);
            cameraImage.texture = controlScript.placedCameras[currentCameraViewing].cameraImage.texture;
            renderImage.texture = controlScript.placedCameras[currentCameraViewing].renderImage.texture;
            detectionImage.texture = controlScript.placedCameras[currentCameraViewing].detectionTexture;
            cameraLabel.text = ">>> CAMERA "+(currentCameraViewing+1).ToString() + " <<<";
            cameraFitter.aspectRatio = (float)cameraImage.texture.width / (float)cameraImage.texture.height;
            renderFitter.aspectRatio = (float)cameraImage.texture.width / (float)cameraImage.texture.height;
            detectionFitter.aspectRatio = (float)cameraImage.texture.width / (float)cameraImage.texture.height;
        }
        else
        {
            cameraImage.gameObject.SetActive(false);
            renderImage.gameObject.SetActive(false);
            detectionImage.gameObject.SetActive(false);
            cameraLabel.text = "NO SIGNAL...";
        }
        SpawnEquipment();
        int layer_mask = LayerMask.GetMask("ARMesh");
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position + (Vector3.up * 2f), -Vector3.up), out hit, 4f, layer_mask))
        {
            transform.position = hit.point;
        }
    }
    public void CreateAnchor()
    {
        RemoveAnchor();
        // Add an ARAnchor component if it doesn't have one already.
        if (gameObject.GetComponent<ARAnchor>() == null)
        {
            gameObject.AddComponent<ARAnchor>();
        }
    }
    public void RemoveAnchor()
    {
        if (gameObject.GetComponent<ARAnchor>() != null)
        {
            Destroy(gameObject.GetComponent<ARAnchor>());
        }
    }
    public void ChangeCamera(int amt)
    {
        currentCameraViewing += amt;
        if (currentCameraViewing<0)
        {
            currentCameraViewing = controlScript.placedCameras.Count-1;
        }
        if (currentCameraViewing > controlScript.placedCameras.Count-1)
        {
            currentCameraViewing = 0;
        }
    }
    public void SpawnEquipment()
    {
        Equipment equipScript = null;
        if (prevShopAmt!=controlScript.saveScript.gameData.purchasedShopItems.Count) {
            if (!clipboard1Spawned && controlScript.saveScript.CheckIfBought("clipboard")) {
                equipScript = Instantiate(clipboardPrefab, clipboard1Spawn.position, clipboard1Spawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = clipboard1Spawn.gameObject;
                clipboard1Spawned = true;
            }
            if (!clipboard2Spawned && controlScript.saveScript.CheckIfBought("clipboardupgrade1"))
            {
                equipScript = Instantiate(clipboardPrefab, clipboard2Spawn.position, clipboard2Spawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = clipboard2Spawn.gameObject;
                clipboard2Spawned = true;
            }
            if (!motion1Spawned && controlScript.saveScript.CheckIfBought("motionsensor"))
            {
                equipScript = Instantiate(motionPrefab, motion1Spawn.position, motion1Spawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = motion1Spawn.gameObject;
                motion1Spawned = true;
            }
            if (!motion2Spawned && controlScript.saveScript.CheckIfBought("motionsensorupgrade1"))
            {
                equipScript = Instantiate(motionPrefab, motion2Spawn.position, motion2Spawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = motion2Spawn.gameObject;
                motion2Spawned = true;
            }
            if (!motion3Spawned && controlScript.saveScript.CheckIfBought("motionsensorupgrade2"))
            {
                equipScript = Instantiate(motionPrefab, motion3Spawn.position, motion3Spawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = motion3Spawn.gameObject;
                motion3Spawned = true;
            }
            if (!motion4Spawned && controlScript.saveScript.CheckIfBought("motionsensorupgrade3"))
            {
                equipScript = Instantiate(motionPrefab, motion4Spawn.position, motion4Spawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = motion4Spawn.gameObject;
                motion4Spawned = true;
            }
            if (!EMFSpawned && controlScript.saveScript.CheckIfBought("emf"))
            {
                equipScript = Instantiate(EMFPrefab, EMFSpawn.position, EMFSpawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = EMFSpawn.gameObject;
                EMFSpawned = true;
            }
            if (!candlesSpawned && controlScript.saveScript.CheckIfBought("candles"))
            {
                equipScript = Instantiate(candlePrefab, candle1Spawn.position, candle1Spawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = candle1Spawn.gameObject;
                equipScript = Instantiate(candlePrefab, candle2Spawn.position, candle2Spawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = candle2Spawn.gameObject;
                equipScript = Instantiate(candlePrefab, candle3Spawn.position, candle3Spawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = candle3Spawn.gameObject;
                equipScript = Instantiate(lighterPrefab, lighterSpawn.position, lighterSpawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = lighterSpawn.gameObject;
                candlesSpawned = true;
            }
            if (!compassSpawned && controlScript.saveScript.CheckIfBought("compass"))
            {
                equipScript = Instantiate(compassPrefab, compassSpawn.position, compassSpawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = compassSpawn.gameObject;
                compassSpawned = true;
            }
            if (!dowsingSpawned && controlScript.saveScript.CheckIfBought("dowsingrod"))
            {
                equipScript = Instantiate(dowsingPrefab, dowsingSpawn.position, dowsingSpawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = dowsingSpawn.gameObject;
                dowsingSpawned = true;
            }
            if (!bellSpawned && controlScript.saveScript.CheckIfBought("bell"))
            {
                equipScript = Instantiate(bellPrefab, bellSpawn.position, bellSpawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = bellSpawn.gameObject;
                bellSpawned = true;
            }
            if (!uv1Spawned && controlScript.saveScript.CheckIfBought("uv"))
            {
                equipScript = Instantiate(uvPrefab, uv1Spawn.position, uv1Spawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = uv1Spawn.gameObject;
                uv1Spawned = true;
            }
            if (!uv2Spawned && controlScript.saveScript.CheckIfBought("uvupgrade1"))
            {
                equipScript = Instantiate(uvPrefab, uv2Spawn.position, uv2Spawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = uv2Spawn.gameObject;
                uv2Spawned = true;
            }
            if (!camera1Spawned && controlScript.saveScript.CheckIfBought("livecamera"))
            {
                equipScript = Instantiate(cameraPrefab, camera1Spawn.position, camera1Spawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = camera1Spawn.gameObject;
                camera1Spawned = true;
            }
            if (!camera2Spawned && controlScript.saveScript.CheckIfBought("cameraupgrade1"))
            {
                equipScript = Instantiate(cameraPrefab, camera2Spawn.position, camera2Spawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = camera2Spawn.gameObject;
                camera2Spawned = true;
            }
            if (!camera3Spawned && controlScript.saveScript.CheckIfBought("cameraupgrade2"))
            {
                equipScript = Instantiate(cameraPrefab, camera3Spawn.position, camera3Spawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = camera3Spawn.gameObject;
                camera3Spawned = true;
            }
            if (!camera4Spawned && controlScript.saveScript.CheckIfBought("cameraupgrade3"))
            {
                equipScript = Instantiate(cameraPrefab, camera4Spawn.position, camera4Spawn.rotation).GetComponent<Equipment>();
                equipScript.mySpawn = camera4Spawn.gameObject;
                camera4Spawned = true;
            }
            prevShopAmt = controlScript.saveScript.gameData.purchasedShopItems.Count;
        }
    }
    public void UpdateEvidence(){
        for (int i = 0; i< evidences.Count;i++)
        {
            GameObject.Destroy(evidences[i]);
        }
        evidences.Clear();
        for (int i = 0; i < controlScript.saveScript.gameData.deskEvidence.Count;i++)
        {
            GameObject evidenceHolder = null;
            if (i==0)
            {
                evidenceHolder = evidence1Holder;
            }
            if (i == 1)
            {
                evidenceHolder = evidence2Holder;
            }
            if (i == 2)
            {
                evidenceHolder = evidence3Holder;
            }
            if (i == 3)
            {
                evidenceHolder = evidence4Holder;
            }
            if (i == 4)
            {
                evidenceHolder = evidence5Holder;
            }
            if (evidenceHolder!=null) {
                Evidence evidence = controlScript.saveScript.gameData.deskEvidence[i];
                if (evidence.type == "photo")
                {
                    GameObject newEvidence = Instantiate(photographPrefab, evidenceHolder.transform.position, evidenceHolder.transform.rotation);
                    newEvidence.transform.SetParent(this.transform);
                    newEvidence.GetComponentInChildren<PhotoMedia>().myEvidence = evidence;
                    evidences.Add(newEvidence);
                }
                if (evidence.type == "video")
                {
                    GameObject newEvidence = Instantiate(dvdPrefab, evidenceHolder.transform.position, evidenceHolder.transform.rotation);
                    newEvidence.transform.SetParent(this.transform);
                    newEvidence.GetComponentInChildren<VideoMedia>().myEvidence = evidence;
                    evidences.Add(newEvidence);
                }
                if (evidence.type == "sound")
                {
                    GameObject newEvidence = Instantiate(cassettePrefab, evidenceHolder.transform.position, evidenceHolder.transform.rotation);
                    newEvidence.transform.SetParent(this.transform);
                    newEvidence.GetComponentInChildren<SoundMedia>().myEvidence = evidence;
                    evidences.Add(newEvidence);
                }
            }
        }
    }
}
