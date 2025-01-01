using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public int prevTutorialPhase;
    public static int tutorialPhase;
    public static bool inTutorial;
    public AudioSource tutorialSpeech;

    public Controller controlScript;

    public float timeTillNextChar;
    public static bool speedingText;
    public static int currentChar;

    public GameObject window;
    public Text windowText;
    public GameObject cursor;
    public Image cursorRender;

    public int widthPiece;
    public int heightPiece;

    public static float autoProgress = 0f;

    public bool cursorFollowingUI;

    public float walkDistanceTillDetect = 100f;
    public Vector3 prevCameraPos;

    public static List<TutorialPhase> tutorial = new List<TutorialPhase>(){};

    public GameObject myInteraction;

    public GameObject boardSignObject;
    public bool boardSignSpawned;

    public int fontSize = 0;

    public GameObject textSound;
    private Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        widthPiece = Screen.width / 16;
        heightPiece = Screen.height / 16;
        tutorial = new List<TutorialPhase>(){
            new TutorialPhase("Welcome! It looks like our remote session has begun.",0.1f,null,"auto",Vector3.zero,new Vector3(widthPiece*10,heightPiece*3),null,0),
            new TutorialPhase("Today I will be teaching you the basics of using MirrorOS.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("You should see my cursor and text window that I will use to guide you.",0.1f,null,"auto",Vector3.zero,new Vector3(widthPiece*6,heightPiece*13),null,0),
            new TutorialPhase("Let\'s focus on setting up now.",0.1f,null,"auto",Vector3.zero,new Vector3(widthPiece*10,heightPiece*10),null,0),
            new TutorialPhase("Survey your environment for a good spot. We need a flat area with sufficient space.",0.1f,null,"auto",new Vector3(widthPiece*6,heightPiece*13),new Vector3(widthPiece*12,heightPiece*8),null,0),
            new TutorialPhase("Once you have found a good spot, hit the button at the bottom of the screen.",0.1f,null,"desk",Vector3.zero,Vector3.zero,"desk",0),
            new TutorialPhase("As you look around, you\'ll see a silhouette. Red implies an invalid placement.",0.1f,null,"auto",Vector3.zero,Vector3.zero,"deskcursor",0),
            new TutorialPhase("If it\'s red, but the area is sizable and flat, try inspecting the area from various angles.",0.1f,null,"auto",Vector3.zero,Vector3.zero,"deskcursor",0),
            new TutorialPhase("Once the silhouette turns green, tap anywhere on the screen to place the desk.",0.1f,null,"deskplace",Vector3.zero,Vector3.zero,"deskcursor",0),
            new TutorialPhase("Excellent! This will be your equipment storage area, although it\'s currently empty.",0.1f,null,"auto",new Vector3(widthPiece*11,heightPiece*12),new Vector3(widthPiece*8,heightPiece*8),null,0),
            new TutorialPhase("For training I\'ve sent you some money - just press the button at the bottom to view your apps.",0.1f,null,"apps",Vector3.zero,Vector3.zero,"apps",200),
            new TutorialPhase("Tap the shopping bag icon to access the shop.",0.1f,null,"shop",Vector3.zero,Vector3.zero,"shop",0),
            new TutorialPhase("Select the \"Equipment\" category.",0.1f,null,"shopequipment",Vector3.zero,Vector3.zero,"shopequipment",0),
            new TutorialPhase("Tap the button labeled \"EMF\".",0.1f,null,"shopemf",Vector3.zero,Vector3.zero,"shopemf",0),
            new TutorialPhase("This window provides information about the selected item.",0.1f,null,"auto",new Vector3(widthPiece*11,heightPiece*13),new Vector3(widthPiece*5,heightPiece*7),null,0),
            new TutorialPhase("Select \"Purchase\" to buy the EMF.",0.1f,null,"buy",Vector3.zero,Vector3.zero,"buy",0),
            new TutorialPhase("Awesome! You\'ve got your first piece of ghost hunting gear.",0.1f,null,"auto",new Vector3(widthPiece*11,heightPiece*12),Vector3.zero,null,0),
            new TutorialPhase("Close the shop now.",0.1f,null,"close",Vector3.zero,Vector3.zero,"close",0),
            new TutorialPhase("Fetch your EMF from the desk by getting close and tapping on it.",0.1f,null,"emf",Vector3.zero,Vector3.zero,"emf",0),
            new TutorialPhase("Begin to move away from the desk until the EMF starts to beep.",0.1f,null,"emfdetection",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("The beeping sound identifies a potential reading, we should leave our EMF here for now.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("To throw an item, simply hold down on the screen and flick upwards.",0.1f,null,"throw",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Our tools are robust, don\'t worry about damaging them.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("You\'ll need a few more items, so I am sending some more money. Open the apps again.",0.1f,null,"apps",Vector3.zero,Vector3.zero,"apps",2250),
            new TutorialPhase("Go to \"Shop\".",0.1f,null,"shop",Vector3.zero,Vector3.zero,"shop",0),
            new TutorialPhase("Click on the \"Equipment\" category.",0.1f,null,"shopequipment",Vector3.zero,Vector3.zero,"shopequipment",0),
            new TutorialPhase("Select \"Candles\".",0.1f,null,"shopcandles",Vector3.zero,Vector3.zero,"shopcandles",0),
            new TutorialPhase("Buy the candles.",0.1f,null,"buy",Vector3.zero,Vector3.zero,"buy",0),
            new TutorialPhase("Now, select \"Clipboard\".",0.1f,null,"shopclipboard",Vector3.zero,Vector3.zero,"shopclipboard",0),
            new TutorialPhase("Buy the clipboard.",0.1f,null,"buy",Vector3.zero,Vector3.zero,"buy",0),
            new TutorialPhase("Next, select \"UV Flashlight\".",0.1f,null,"shopuv",Vector3.zero,Vector3.zero,"shopuv",0),
            new TutorialPhase("Purchase the UV flashlight.",0.1f,null,"buy",Vector3.zero,Vector3.zero,"buy",0),
            new TutorialPhase("Find the \"Motion Sensor\".",0.1f,null,"shopmotionsensor",Vector3.zero,Vector3.zero,"shopmotionsensor",0),
            new TutorialPhase("Buy it.",0.1f,null,"buy",Vector3.zero,Vector3.zero,"buy",0),
            new TutorialPhase("Finally, locate the \"Live Camera\".",0.1f,null,"shoplivecamera",Vector3.zero,Vector3.zero,"shoplivecamera",0),
            new TutorialPhase("Buy that as well.",0.1f,null,"buy",Vector3.zero,Vector3.zero,"buy",0),
            new TutorialPhase("You can leave the shop now.",0.1f,null,"close",Vector3.zero,Vector3.zero,"close",0),
            new TutorialPhase("Your desk should now be filled with useful equipment.",0.1f,null,"auto",new Vector3(widthPiece*11,heightPiece*13),Vector3.zero,null,0),
            new TutorialPhase("Let\'s start with something simple. I\'ll guide you on how to light candles.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Pick up the lighter from the top shelf of the desk.",0.1f,null,"lighter",Vector3.zero,Vector3.zero,"lighter",0),
            new TutorialPhase("Once the lighter is lit, use the flame to light all three candles on the desk.",0.1f,null,"lightcandles",Vector3.zero,Vector3.zero,"candles",0),
            new TutorialPhase("Well done! Candles can help identify haunted areas when blown out.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("You can throw the lighter down now.",0.1f,null,"throw",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Fetch the clipboard from the desk next.",0.1f,null,"clipboard",Vector3.zero,Vector3.zero,"clipboard",0),
            new TutorialPhase("Sometimes, you need to place equipment down gently.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Tap on a flat, close area to set the clipboard at that location.",0.1f,null,"place",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Seems like the ink was knocked over, spilled ink can form words.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Let\'s record this evidence. Navigate to your apps.",0.1f,null,"apps",Vector3.zero,Vector3.zero,"apps",0),
            new TutorialPhase("Open the camera by clicking the corresponding icon.",0.1f,null,"camera",Vector3.zero,Vector3.zero,"camera",0),
            new TutorialPhase("Take a photo of the writing using the circular button.",0.1f,null,"evidenceclipboard",Vector3.zero,Vector3.zero,"clipboard",0),
            new TutorialPhase("Great! Now, let\'s revisit the area where the EMF detected a reading.",0.1f,null,"auto",Vector3.zero,Vector3.zero,"boardsign",0),
            new TutorialPhase("You\'ll notice a target placed. Snap a photo of it.",0.1f,null,"evidencecompass",Vector3.zero,Vector3.zero,"boardsign",0),
            new TutorialPhase("There goes a flying target! Once your camera recharges, capture it too.",0.1f,null,"evidencedowsing",Vector3.zero,Vector3.zero,"boardfly",0),
            new TutorialPhase("We\'ve taken three photos now. Bear in mind, the desk can normally only hold three pieces of evidence at any given time.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Besides the desk\'s storage, you also have an archive to hold more bits of evidence. I\'ll explain its usage later.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("As our desk is full, we need to create some space. Go ahead and close the camera for now.",0.1f,null,"close",Vector3.zero,Vector3.zero,"close",0),
            new TutorialPhase("Begin by opening your apps.",0.1f,null,"apps",Vector3.zero,Vector3.zero,"apps",0),
            new TutorialPhase("Locate and tap the icon that resembles a box.",0.1f,null,"stash",Vector3.zero,Vector3.zero,"stash",0),
            new TutorialPhase("Select the first image showing the target standing on the ground.",0.1f,null,"stashdesk1",Vector3.zero,Vector3.zero,"stashdesk1",0),
            new TutorialPhase("Next, click on the orange button featuring an open box to archive this image.",0.1f,null,"archive",Vector3.zero,Vector3.zero,"archive",0),
            new TutorialPhase("We\'ll also be archiving the image of the airborne target. Please select it.",0.1f,null,"stashdesk0",Vector3.zero,Vector3.zero,"stashdesk0",0),
            new TutorialPhase("Now, proceed to archive it.",0.1f,null,"archive",Vector3.zero,Vector3.zero,"archive",0),
            new TutorialPhase("We\'re ready to capture more images. Exit this menu to proceed.",0.1f,null,"close",Vector3.zero,Vector3.zero,"close",0),
            new TutorialPhase("Retrieve the UV flashlight from the desk.",0.1f,null,"uv",Vector3.zero,Vector3.zero,"uv",0),
            new TutorialPhase("There\'s something hidden on the front of the target board, only visible under UV light.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Stand in front of the target and use the UV flashlight to reveal the hidden image on it.",0.1f,null,"handprintreveal",Vector3.zero,Vector3.zero,"boardsign",0),
            new TutorialPhase("You\'ll need to document this as well. First, you will need to position the flashlight on a tripod.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Find a spot spacious enough and ensure you\'re not angling the device too steeply, the tripod button on the bottom left will be gray if invalid.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("After finding a suitable location with the UV light revealing the hidden image, set the tripod.",0.1f,null,"tripodreveal",Vector3.zero,Vector3.zero,"tripod",0),
            new TutorialPhase("Now, revisit your apps.",0.1f,null,"apps",Vector3.zero,Vector3.zero,"apps",0),
            new TutorialPhase("Launch the camera.",0.1f,null,"camera",Vector3.zero,Vector3.zero,"camera",0),
            new TutorialPhase("Take a photo of the hidden image",0.1f,null,"evidencehandprint",Vector3.zero,Vector3.zero,"boardsign",0),
            new TutorialPhase("Close the camera.",0.1f,null,"close",Vector3.zero,Vector3.zero,"close",0),
            new TutorialPhase("Now, let\'s explore another piece of equipment. Retrieve the motion sensor.",0.1f,null,"motion",Vector3.zero,Vector3.zero,"motion",0),
            new TutorialPhase("The Motion Sensor can be placed on any surface.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Tap on a desired surface to position it.",0.1f,null,"place",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Excellent! It will notify you whenever motion is detected. Wait one moment as I manually trigger it . . .",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Just like that. Don\'t worry, it cannot detect you, so you won\'t trigger it.",0.1f,null,"auto",Vector3.zero,Vector3.zero,"MotionPopup",0),
            new TutorialPhase("Given its ability to detect activity, it pairs well with the next piece of equipment.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Please pick up the live camera.",0.1f,null,"livecamera",Vector3.zero,Vector3.zero,"livecamera",0),
            new TutorialPhase("Set it up on a tripod in a position offering a clear view.",0.1f,null,"tripod",Vector3.zero,Vector3.zero,"tripod",0),
            new TutorialPhase("Well done. There\'s one more concealed detail to uncover.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("There\'s a digital target visible only through the live camera.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("You can view it on the camera\'s display.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Navigate back to your apps.",0.1f,null,"apps",Vector3.zero,Vector3.zero,"apps",0),
            new TutorialPhase("Open the camera app.",0.1f,null,"camera",Vector3.zero,Vector3.zero,"camera",0),
            new TutorialPhase("While the target is displayed on the camera\'s screen, it also streams to the monitor on your desk.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Go to your desk and take a photo of the target displayed on the monitor.",0.1f,null,"evidencemotion",Vector3.zero,Vector3.zero,"MonitorCanvas",0),
            new TutorialPhase("Great, you can now close the camera app.",0.1f,null,"close",Vector3.zero,Vector3.zero,"close",0),

            new TutorialPhase("With our collected photos, we\'re ready to put them to use.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Open your applications again.",0.1f,null,"apps",Vector3.zero,Vector3.zero,"apps",0),
            new TutorialPhase("Launch the \"Analyze\" app, represented by a magnifying glass icon.",0.1f,null,"analyze",Vector3.zero,Vector3.zero,"analyze",0),
            new TutorialPhase("This app lets you submit photos for expert analysis.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Upon completion, you\'ll be rewarded based on the evidence\'s quality and subject.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Press the large green \'+\' button to submit a piece of evidence.",0.1f,null,"add0",Vector3.zero,Vector3.zero,"add0",0),
            new TutorialPhase("You\'ve now entered the evidence selection screen.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Note that only items from your desk are accepted for analysis, not the archive.",0.1f,null,"auto",Vector3.zero,new Vector3(widthPiece*11,heightPiece*4),null,0),
            new TutorialPhase("Select the previously captured clipboard spill.",0.1f,null,"stashdesk2",Vector3.zero,Vector3.zero,"stashdesk2",0),
            new TutorialPhase("Hit the \"select\" button.",0.1f,null,"select",Vector3.zero,new Vector3(widthPiece*11,heightPiece*13),"select",0),
            new TutorialPhase("Normally, this takes some time. However, I\'ll speed up the process by reviewing it myself. One moment . . .",0.1f,null,"analysiscomplete",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Review complete. Press the \"collect\" button to receive your reward.",0.1f,null,"collect",Vector3.zero,Vector3.zero,"collect",0),
            new TutorialPhase("Exit this menu.",0.1f,null,"close",Vector3.zero,Vector3.zero,"close",0),
            new TutorialPhase("Return to your apps.",0.1f,null,"apps",Vector3.zero,Vector3.zero,"apps",0),
            new TutorialPhase("Launch the \"Messages\" app, represented by a speech bubble icon.",0.1f,null,"messages",Vector3.zero,Vector3.zero,"messages",0),
            new TutorialPhase("This app displays public requests for specific evidence.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("Open a message - we only have one for now.",0.1f,null,"message",Vector3.zero,Vector3.zero,"message",0),
            new TutorialPhase("Here you can read the message and view the requests in detail. Completed requests are marked green, while pending ones are red.",0.1f,null,"auto",new Vector3(widthPiece*11,heightPiece*14),Vector3.zero,null,0),
            new TutorialPhase("The green buttons correspond to the requests. Tap the first one.",0.1f,null,"add0",Vector3.zero,Vector3.zero,"add0",0),
            new TutorialPhase("The required evidence is specified in green text at the top.",0.1f,null,"auto",new Vector3(widthPiece*11,heightPiece*8),Vector3.zero,null,0),
            new TutorialPhase("Both desk and archive items can fulfill message requests.",0.1f,null,"auto",new Vector3(widthPiece*11,heightPiece*13),Vector3.zero,null,0),
            new TutorialPhase("Select the photo of the UV ghost from your desk.",0.1f,null,"stashdesk1",Vector3.zero,Vector3.zero,"stashdesk1",0),
            new TutorialPhase("Confirm your selection.",0.1f,null,"select",Vector3.zero,Vector3.zero,"select",0),
            new TutorialPhase("Now tap the second request.",0.1f,null,"add1",Vector3.zero,Vector3.zero,"add1",0),
            new TutorialPhase("Choose the photo of the digital target from your desk.",0.1f,null,"stashdesk0",Vector3.zero,Vector3.zero,"stashdesk0",0),
            new TutorialPhase("Confirm your selection.",0.1f,null,"select",Vector3.zero,Vector3.zero,"select",0),
            new TutorialPhase("Proceed to the third request.",0.1f,null,"add2",Vector3.zero,Vector3.zero,"add2",0),
            new TutorialPhase("Remember that archived items can also fulfill requests. Select the photo of the flying target.",0.1f,null,"stasharchive0",Vector3.zero,Vector3.zero,"stasharchive0",0),
            new TutorialPhase("Confirm your selection.",0.1f,null,"select",Vector3.zero,Vector3.zero,"select",0),
            new TutorialPhase("Now, handle the final, fourth request.",0.1f,null,"add3",Vector3.zero,Vector3.zero,"add3",0),
            new TutorialPhase("Select the photo of the standing target.",0.1f,null,"stasharchive1",Vector3.zero,Vector3.zero,"stasharchive1",0),
            new TutorialPhase("Finally, confirm your selection.",0.1f,null,"select",Vector3.zero,Vector3.zero,"select",0),
            new TutorialPhase("To fulfill this message, press the green \"upload\" button on the right.",0.1f,null,"upload",Vector3.zero,Vector3.zero,"upload",0),
            new TutorialPhase("You can now exit this screen.",0.1f,null,"close",Vector3.zero,Vector3.zero,"close",0),
            new TutorialPhase("Excellent! You\'re now familiar with both avenues for progression and growth.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("There\'s much more to discover through experimentation, but we\'ll leave it here for now.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
            new TutorialPhase("As a token of appreciation, you will get $1000 to start off. Best of luck moving forward.",0.1f,null,"auto",Vector3.zero,Vector3.zero,null,0),
        };
        prevTutorialPhase = -1;
        tutorialPhase = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!inTutorial)
        {
            window.SetActive(false);
            cursor.SetActive(false);
        }
        else
        {
            window.SetActive(true);
            cursor.SetActive(true);
            if (SceneManager.GetActiveScene().name == "Game") {
                window.transform.localScale = Vector3.MoveTowards(window.transform.localScale, new Vector3(1f, 1f, 1f), 5f * Time.fixedDeltaTime);
            }
        }
        if (inTutorial) {
            if (controlScript.totalLitCandles>=3 && tutorial[tutorialPhase].advanceAction== "lightcandles")
            {
                AdvanceTutorial();
            }
            if (tutorial[tutorialPhase].advanceAction == "handprintreveal" && controlScript.revealingHandprint && Vector3.Distance(myInteraction.transform.position, controlScript.activeCamera.transform.position) < 4f)
            {
                AdvanceTutorial();
            }
            if (tutorial[tutorialPhase].advanceAction == "emfdetection")
            {
                walkDistanceTillDetect -= Vector3.Distance(new Vector3(prevCameraPos.x, prevCameraPos.y, 0f), new Vector3(_camera.transform.position.x, _camera.transform.position.y, 0f));
                prevCameraPos = _camera.transform.position;
                if (Vector3.Distance(_camera.transform.position, controlScript.toolDeskScript.transform.position) >= 5f)
                {
                    GhostInteraction interactionScript = Instantiate(controlScript.interactionPrefab, _camera.transform.position, new Quaternion()).GetComponent<GhostInteraction>();
                    controlScript.ghostAreas.Add(interactionScript);
                    myInteraction = interactionScript.gameObject;
                    AdvanceTutorial();
                }
            }
            if (!boardSignSpawned && tutorial[tutorialPhase].mouseFollow == "boardsign")
            {
                if (myInteraction) {
                    Instantiate(boardSignObject, myInteraction.transform.position, new Quaternion());
                    boardSignSpawned = true;
                }
            }

            //cursor.SetActive(Random.Range(0,100)>10);
            if (tutorial[tutorialPhase].moveWindowTo != Vector3.zero)
            {
                if (window.transform.position != tutorial[tutorialPhase].moveWindowTo)
                {
                    if (Vector3.Distance(cursor.transform.position, window.transform.position + (Vector3.up * (heightPiece * 2))) < 0.1f)
                    {
                        window.transform.position = Vector3.MoveTowards(window.transform.position, tutorial[tutorialPhase].moveWindowTo, 1000f * Time.fixedDeltaTime);
                    }
                    else
                    {
                        cursor.transform.position = Vector3.MoveTowards(cursor.transform.position, window.transform.position + (Vector3.up * (heightPiece * 2)), 1000f * Time.fixedDeltaTime);
                    }
                }
            }
            if (prevTutorialPhase != tutorialPhase)
            {
                tutorialSpeech.clip = tutorial[tutorialPhase].voice;
                prevTutorialPhase = tutorialPhase;
                timeTillNextChar = tutorial[tutorialPhase].textTime;
                currentChar = 0;
                windowText.resizeTextForBestFit = true;
                windowText.text = tutorial[tutorialPhase].captionText;
                fontSize = windowText.fontSize;
                //windowText.resizeTextForBestFit = false;
                windowText.text = "";
                windowText.text += tutorial[tutorialPhase].captionText[currentChar];
                //windowText.fontSize = 70 - (tutorial[tutorialPhase].captionText.Length / 3);
                controlScript.saveScript.gameData.money += tutorial[tutorialPhase].moneyChange;
                prevCameraPos = _camera.transform.position;
                if (Tutorial.tutorial[tutorialPhase].mouseFollow == "MotionPopup")
                {
                    controlScript.popupScript.DisplayPopup(0, "Sensor has detected motion");
                }
                if (Tutorial.tutorial[tutorialPhase].advanceAction == "motionsensor")
                {
                    GameObject.Destroy(myInteraction);
                }
                speedingText = false;
                autoProgress = 0f;
                if (tutorial[tutorialPhase].advanceAction == "auto" && tutorial[tutorialPhase].moveWindowTo==Vector3.zero && tutorial[tutorialPhase].moveMouseTo == Vector3.zero)
                {
                    //tutorialPhase++;
                }
            }
            if (window.transform.localScale.x == 1f) {
                timeTillNextChar -= 2f * Time.fixedDeltaTime;
                if (speedingText)
                {
                    timeTillNextChar -= 2f * Time.fixedDeltaTime;
                }
            }
            if (currentChar < tutorial[tutorialPhase].captionText.Length - 1) {
                if (timeTillNextChar <= 0f)
                {
                    timeTillNextChar = tutorial[tutorialPhase].textTime;
                    currentChar++;
                    windowText.text += tutorial[tutorialPhase].captionText[currentChar];
                    Instantiate(textSound, _camera.transform.position, new Quaternion());
                }
            }
            else
            {
                if (tutorial[tutorialPhase].advanceAction == "auto")
                {
                    if (autoProgress > 1f)
                    {
                        if (tutorialPhase >= tutorial.Count-1)
                        {
                            tutorialPhase = 0;
                            SceneManager.LoadScene("Menu");
                            return;
                        }
                        tutorialPhase++;
                        autoProgress = 0f;
                    }
                    else
                    {
                        autoProgress += 0.2f * Time.fixedDeltaTime;
                    }
                }
                else
                {
                    autoProgress = 0f;
                }

            }
            if (tutorial[tutorialPhase].moveWindowTo == Vector3.zero || Vector3.Distance(window.transform.position, tutorial[tutorialPhase].moveWindowTo) < 0.1f)
            {
                if (tutorial[tutorialPhase].moveMouseTo != Vector3.zero)
                {
                    cursor.transform.position = Vector3.MoveTowards(cursor.transform.position, tutorial[tutorialPhase].moveMouseTo, 1000f * Time.fixedDeltaTime);
                }
                if (tutorial[tutorialPhase].mouseFollow != null)
                {
                    GameObject follow = FindFollowObject(tutorial[tutorialPhase].mouseFollow);
                    if (follow) {
                        if (cursorFollowingUI) {
                            cursor.transform.position = Vector3.MoveTowards(cursor.transform.position, follow.transform.position, 1000f * Time.fixedDeltaTime);
                        }
                        else
                        {
                            Vector3 screenPos = _camera.WorldToScreenPoint(follow.transform.position);
                            if (screenPos.z <= -0.5f)
                            {
                                cursorRender.enabled = false;
                            }
                            else
                            {
                                cursorRender.enabled = true;
                            }
                            Vector3 uiPos = new Vector3(screenPos.x, screenPos.y, screenPos.z);
                            cursor.transform.position = Vector3.MoveTowards(cursor.transform.position, uiPos, 1000f * Time.fixedDeltaTime);
                        }
                    }
                }
            }
        }
    }

    private GameObject FindFollowObject(string find)
    {
        ButtonBlocker blockerScript = FindButtonByAction(find);
        if (blockerScript)
        {
            cursorFollowingUI = true;
            return blockerScript.gameObject;
        }

        Equipment equipmentScript = FindEquipmentByAction(find);
        if (equipmentScript)
        {
            cursorFollowingUI = false;
            return equipmentScript.gameObject;
        }

        GameObject foundObject = FindObjectByAction(find);
        if (foundObject)
        {
            cursorFollowingUI = false;
            return foundObject;
        }

        return null;

    }

    private ButtonBlocker FindButtonByAction(string action)
    {
        ButtonBlocker[] uiButtons = Object.FindObjectsOfType<ButtonBlocker>();
        foreach (var t in uiButtons)
        {
            if (t.myAction==action)
            {
                return t;
            }
        }
        return null;
    }

    private Equipment FindEquipmentByAction(string action)
    {
        Equipment[] equipment = Object.FindObjectsOfType<Equipment>();
        foreach (var t in equipment)
        {
            if ((t.GetComponent<Camera>() && action == "livecamera") || (t.emf && action == "emf")|| (t.motion && action == "motion") || (t.clipboard && action == "clipboard") || (t.uv && action == "uv") || (t.candle && action == "candles") || (t.lighter && action == "lighter"))
            {
                return t;
            }
        }
        return null;
    }

    private GameObject FindObjectByAction(string action)
    {
        return GameObject.Find(action);
    }
    public static void AdvanceTutorial()
    {
        if (inTutorial) {
            speedingText = false;
            autoProgress = 0f;
            tutorialPhase++;
        }
    }
    public class TutorialPhase {
        public string captionText;
        public float textTime;
        public AudioClip voice;
        public string advanceAction;
        public Vector3 moveWindowTo;
        public Vector3 moveMouseTo;
        public string mouseFollow;
        public int moneyChange;
        public TutorialPhase(string nCaptionText, float nTextTime, AudioClip nVoice, string nAdvanceAction, Vector3 nMoveWindowTo, Vector3 nMoveMouseTo, string nMouseFollow, int nMoneyChange){
            captionText = nCaptionText;
            textTime = nTextTime;
            voice = nVoice;
            advanceAction = nAdvanceAction;
            moveWindowTo = nMoveWindowTo;
            moveMouseTo = nMoveMouseTo;
            mouseFollow = nMouseFollow;
            moneyChange = nMoneyChange;
        }
    }
}
