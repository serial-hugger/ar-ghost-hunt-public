using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Purchasing;
using System.Threading.Tasks;

public class SaveManager : MonoBehaviour
{
    public int version;

    string saveFile;
    string previousSave1File;
    string previousSave2File;
    string previousSave3File;
    string settingFile;

    public GameData gameData = new GameData();
    public GameData defaultData = new GameData();
    public SettingData settingData = new SettingData();
    public Controller controlScript;
    public Ads adScript;
    public Notifications notificationScript;

    public AudioSource myAudio;

    public MoonPhase moonScript;

    public bool isOldSave = false;
    public bool isEncryptedSave = false;
    public bool isSaveInvalid = false;

    public bool fileWriting = false;
    Task fileTask = null;
    Task settingTask = null;

    int prevName = -1;
    int prevMessage = -1;
    int prevQuestItem = -1;
    List<string> possibleNames = new List<string>()
    {
        "Evelyn Blackwood",
        "Yaron Vale",
        "Iris Juniper",
        "Nolan Orion",
        "Jack Lunis",
        "Tessa Starling",
        "Bryan Thorne",
        "Diana Fielding",
        "Tara Evergreen",
        "Aaron Miles",
        "Greg Wilder",
        "Lindsey Dawn",
        "Molly Harper",
        "Dominic Reed",
        "Rebecca Frost",
        "Tina Ross",
        "Olivia Mason",
        "Quentin Barrett",
        "Penelope Vale",
        "Isaac Lane"
    };
    List<Message> possibleMessages = new List<Message>()
    {
        new Message("",new List<QuestItem>(){},/*MESSAGE PARTS*/new List<string>(){"Hey! Can I have a "," for my website?"},/*TIME UPLOAD START*/-1,/*TIME UPLOAD END*/-1,/*TIME TO EXPIRE*/TimeSpan.TicksPerDay,true,true,null,100,10,false),

        new Message("",new List<QuestItem>(){},/*MESSAGE PARTS*/new List<string>(){"Hello! I'm looking for ",". Can you help?"},/*TIME UPLOAD START*/-1,/*TIME UPLOAD END*/-1,/*TIME TO EXPIRE*/TimeSpan.TicksPerDay,true,true,null,100,10,false),

        new Message("",new List<QuestItem>(){},/*MESSAGE PARTS*/new List<string>(){"Hello! I'm looking for a "," and a ",". Do you happen to have these?"},/*TIME UPLOAD START*/-1,/*TIME UPLOAD END*/-1,/*TIME TO EXPIRE*/TimeSpan.TicksPerDay,true,true,null,100,10,false),

        new Message("",new List<QuestItem>(){},/*MESSAGE PARTS*/new List<string>(){"I am currently organizing a paranormal-themed event and am in need of a ","."},/*TIME UPLOAD START*/-1,/*TIME UPLOAD END*/-1,/*TIME TO EXPIRE*/TimeSpan.TicksPerDay,true,true,null,100,10,false),

        new Message("",new List<QuestItem>(){},/*MESSAGE PARTS*/new List<string>(){ "I am coordinating pre-production for a short film within the paranormal genre and am hoping to incorporate ", " and a ","."},/*TIME UPLOAD START*/-1,/*TIME UPLOAD END*/-1,/*TIME TO EXPIRE*/TimeSpan.TicksPerDay,true,true,null,100,10,false)
    };
    List<QuestItem> possibleQuestItems = new List<QuestItem>()
    {
        new QuestItem("sound",new List<string>(){"any"},0,1,"ghostly voice"),
        new QuestItem("photo",new List<string>(){"ghost"},0,1,"ghost apparition photo"),
        new QuestItem("photo",new List<string>(){"ghostorb"},0,1,"ghost orbs photo"),
        new QuestItem("photo",new List<string>(){"ghosttrail"},0,1,"ghost trails photo"),
        new QuestItem("photo",new List<string>(){"handprint"},0,1,"handprint photo"),
        new QuestItem("photo",new List<string>(){"ghost","ghostorb"},0,5,"ghost apparition and ghost orbs in the same photo"),
        new QuestItem("photo",new List<string>(){"ghost","ghosttrail"},0,5,"ghost apparition and ghost trails in the same photo"),
        new QuestItem("photo",new List<string>(){"ghostorb","ghosttrail"},0,3,"ghost orbs and ghost trails in the same photo"),
        new QuestItem("photo",new List<string>(){"ghost","handprint"},0,3,"ghost apparition and handprint in the same photo"),
        new QuestItem("photo",new List<string>(){"clipboard"},0,1,"clipboard photo"),
        new QuestItem("video",new List<string>(){"ghost"},0,1,"ghost apparition video"),
        new QuestItem("video",new List<string>(){"ghostorb"},0,1,"ghost orbs video"),
        new QuestItem("video",new List<string>(){"ghosttrail"},0,1,"ghost trails video"),
        new QuestItem("video",new List<string>(){"handprint"},0,1,"handprint video"),
        new QuestItem("video",new List<string>(){"ghost","ghostorb"},0,5,"ghost apparition and ghost orbs in the same video"),
        new QuestItem("video",new List<string>(){"ghost","ghosttrail"},0,5,"ghost apparition and ghost trails in the same video"),
        new QuestItem("video",new List<string>(){"ghostorb","ghosttrail"},0,5,"ghost orbs and ghost trails in the same video"),
        new QuestItem("video",new List<string>(){"ghost","handprint"},0,3,"ghost apparition and handprint in the same video"),
        new QuestItem("video",new List<string>(){"clipboard"},0,1,"clipboard video"),
        new QuestItem("video",new List<string>(){"thrown"},0,5,"video of a ghost throwing something"),
        new QuestItem("any",new List<string>(){"ghost"},0,1,"ghost apparition evidence"),
        new QuestItem("any",new List<string>(){"ghostorb"},0,1,"ghost orbs evidence"),
        new QuestItem("any",new List<string>(){"ghosttrail"},0,1,"ghost trails evidence"),
        new QuestItem("any",new List<string>(){"handprint"},0,1,"handprint evidence"),
        new QuestItem("any",new List<string>(){"ghost","ghostorb"},0,5,"ghost apparition and ghost orbs in the same evidence"),
        new QuestItem("any",new List<string>(){"ghost","ghosttrail"},0,5,"ghost apparition and ghost trails in the same evidence"),
        new QuestItem("any",new List<string>(){"ghostorb","ghosttrail"},0,3,"ghost orbs and ghost trails in the same evidence"),
        new QuestItem("any",new List<string>(){"ghost","handprint"},0,3,"ghost apparition and handprint in the same evidence"),
        new QuestItem("any",new List<string>(){"clipboard"},0,1,"clipboard evidence")
    };

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (GameObject.FindGameObjectsWithTag("GameManager").Length > 1)
        {
            GameObject.Destroy(gameObject);
        }
        saveFile = Application.persistentDataPath + "/save.json";
        previousSave1File = Application.persistentDataPath + "/Previous Saves/save1.json";
        previousSave2File = Application.persistentDataPath + "/Previous Saves/save2.json";
        previousSave3File = Application.persistentDataPath + "/Previous Saves/save3.json";
        settingFile = Application.persistentDataPath + "/settings.json";
    }
    private void Start()
    {
        int.TryParse(Application.version,out version);
        ReadFile();
    }

    // Update is called once per frame
    private void Update()
    {
        if (fileWriting)
        {
            if (fileTask==null)
            {
                // Write JSON to file.
                string jsonString = JsonUtility.ToJson(gameData);
                gameData.prevAlignmentScore = GetTokenValue(jsonString);
                jsonString = JsonUtility.ToJson(gameData);
                fileTask = WriteFileAsync(saveFile, jsonString);
            }
            if (settingTask==null)
            {
                string jsonString = JsonUtility.ToJson(settingData);
                settingTask = WriteFileAsync(settingFile, jsonString);
            }
            if (fileTask.IsCompleted && settingTask.IsCompleted)
            {
                fileWriting = false;
                fileTask = null;
                settingTask = null;
                ReadFile();
            }
        }
        if (settingData.orientation == 0)
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        if (settingData.orientation == 1)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
        if (Tutorial.inTutorial)
        {
            if (Tutorial.tutorialPhase < 3)
            {
                gameData.money = 0;
                gameData.premium = defaultData.premium;
            }
        }
        if (gameData.premium && !Tutorial.inTutorial)
        {
            gameData.deskSlots = 5;
        }
        else
        {
            gameData.deskSlots = 3;
        }
        if (settingData.music)
        {
            if (SceneManager.GetActiveScene().name != "Menu")
            {
                myAudio.volume = Mathf.MoveTowards(myAudio.volume, 0f, 1.5f * Time.deltaTime);
            }
            else
            {
                myAudio.volume = 1f;
                if (!myAudio.isPlaying)
                {
                    myAudio.Play();
                }
            }
        }
        else
        {
            myAudio.volume = 0f;
        }
        if (controlScript && gameData.bonusType == "")
        {
            gameData.bonusType = controlScript.GetRandomEvidenceType();
            WriteFile();
        }
    }
    public void ReadFile()
    {
        if (!Tutorial.inTutorial && !fileWriting) {
            // Does the file exist?
            if (File.Exists(saveFile))
            {
                string fileCheck = File.ReadAllText(saveFile);
                if (!fileCheck.Contains("prevAlignmentScore"))
                {
                    isEncryptedSave = true;
                }
                string fileContents;
                //check if valid
                long tokenValue = 0;
                if (isEncryptedSave)
                {
                    fileContents = Rot39(File.ReadAllText(saveFile));
                }
                else
                {
                    fileContents = File.ReadAllText(saveFile);
                    tokenValue = GetTokenValue(fileContents);
                }

                gameData = JsonUtility.FromJson<GameData>(fileContents);
                if (!isEncryptedSave && gameData.prevAlignmentScore != tokenValue)
                {
                    isSaveInvalid = true;
                }
                fileContents = File.ReadAllText(settingFile);
                settingData = JsonUtility.FromJson<SettingData>(fileContents);
                defaultData = gameData;
                if (gameData.prevPlayedBuild < version)
                {
                    VersionChores(version);
                }
            }
            else
            {
                WriteFile();
            }
            if (gameData.deviceId != SystemInfo.deviceUniqueIdentifier)
            {
                gameData.premium = false;
                gameData.deviceId = SystemInfo.deviceUniqueIdentifier;
            }
        }
    }
    public void WriteSetting()
    {
        string jsonString = JsonUtility.ToJson(settingData);
        settingTask = WriteFileAsync(settingFile, jsonString);
    }
    static async Task WriteFileAsync(string path, string content)
    {
        using (StreamWriter outputFile = new StreamWriter(path))
        {
            await outputFile.WriteAsync(content);
        }
    }
    public void WriteFile()
    {
        if (!Tutorial.inTutorial && !fileWriting)
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Previous Saves/");
            if (File.Exists(previousSave3File))
            {
                File.Delete(previousSave3File);
            }
            if (File.Exists(previousSave2File))
            {
                File.Move(previousSave2File, previousSave3File);
            }
            if (File.Exists(previousSave1File))
            {
                File.Move(previousSave1File, previousSave2File);
            }
            if (File.Exists(saveFile))
            {
                File.Move(saveFile, previousSave1File);
            }
            fileWriting = true;
        }
    }
    public void WriteFileHang()
    {
        if (!Tutorial.inTutorial && !fileWriting)
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Previous Saves/");
            if (File.Exists(previousSave3File))
            {
                File.Delete(previousSave3File);
            }
            if (File.Exists(previousSave2File))
            {
                File.Move(previousSave2File, previousSave3File);
            }
            if (File.Exists(previousSave1File))
            {
                File.Move(previousSave1File, previousSave2File);
            }
            if (File.Exists(saveFile))
            {
                File.Move(saveFile, previousSave1File);
            }
            // Write JSON to file.
            string jsonString = JsonUtility.ToJson(gameData);
            gameData.prevAlignmentScore = GetTokenValue(jsonString);
            jsonString = JsonUtility.ToJson(gameData);
            //fileTask = WriteFileAsync(saveFile, jsonString);
            var sr = new StreamWriter(saveFile);
            sr.Write(jsonString);
            sr.Close();
        }
    }
    public void CreateNotifications()
    {
        notificationScript.DeleteNotifications();
        notificationScript.GetAnalysisMessage();
        //if (analysisMessage != null && (analysisMessage.evidenceUploading == null || analysisMessage.evidenceUploading == ""))
        //{
        //    ScheduleCalendarNotification(System.DateTime.Now.AddSeconds(10), "analyze", "Nothing is being analyzed!", "Submit evidence for analysis to recieve rewards later.", null, "analysis");
        //}
        if (notificationScript.analysisMessage != null && notificationScript.analysisMessage.evidenceUploading != null && notificationScript.analysisMessage.evidenceUploading != "")
        {
            if (TimeManager.validTime && notificationScript.analysisMessage.timeUploadEnd > TimeManager.GetTime())
            {
                long ticksTotal = notificationScript.analysisMessage.timeUploadEnd - TimeManager.GetTime();
                notificationScript.ScheduleCalendarNotification(System.DateTime.Now.AddTicks(ticksTotal), "analyze", "Analysis complete!", "Come collect your reward.", null, "analysis");
            }
            else
            {
                Debug.Log("Fail 2");
            }
        }
        else
        {
            Debug.Log("Fail 1");
        }
    }
    public void NewSave()
    {
        File.Delete(saveFile);
        Application.Quit();
    }
    public void AddEvidenceToDesk(int frameWidth, int frameHeight, byte[] photo, byte[] video, float[] sound, string type, float score, List<EvidenceCapture.FocalEvidence> focalEvidence, List<EvidenceCapture.AudibleEvidence> audibleEvidence)
    {
        bool neededPhoto = false;
        if (Tutorial.tutorial[Tutorial.tutorialPhase].advanceAction.Contains("evidence"))
        {
            for (int i = 0; i < focalEvidence.Count; i++)
            {
                if (Tutorial.tutorial[Tutorial.tutorialPhase].advanceAction.Contains(focalEvidence[i].type))
                {
                    neededPhoto = true;
                }
            }
        }
        if (Tutorial.inTutorial && neededPhoto)
        {
            gameData.deskEvidence.Insert(0, new Evidence(frameWidth, frameHeight, photo, video, sound, type, focalEvidence, audibleEvidence));
            controlScript.toolDeskScript.UpdateEvidence();
            Tutorial.AdvanceTutorial();
        }
        if (!Tutorial.inTutorial)
        {
            gameData.deskEvidence.Insert(0, new Evidence(frameWidth, frameHeight, photo, video, sound, type, focalEvidence, audibleEvidence));
            controlScript.toolDeskScript.UpdateEvidence();
            controlScript.UndoAllParanormal(focalEvidence);
        }
    }
    public void RemoveEvidenceFromDesk(Evidence evidence)
    {
        for (int i = 0; i < gameData.deskEvidence.Count; i++)
        {
            if (gameData.deskEvidence[i] == evidence)
            {
                gameData.deskEvidence.RemoveAt(i);
                controlScript.toolDeskScript.UpdateEvidence();
            }
        }
    }
    public void AddEvidenceToArchive(Evidence evidence)
    {
        gameData.archiveEvidence.Insert(0, evidence);
        controlScript.toolDeskScript.UpdateEvidence();
    }
    public void RemoveEvidenceFromArchive(Evidence evidence)
    {
        for (int i = 0; i < gameData.archiveEvidence.Count; i++)
        {
            if (gameData.archiveEvidence[i] == evidence)
            {
                gameData.archiveEvidence.RemoveAt(i);
                controlScript.toolDeskScript.UpdateEvidence();
            }
        }
    }
    public Evidence GetEvidenceFromId(string id)
    {
        for (int i = 0; i < gameData.deskEvidence.Count; i++)
        {
            if (gameData.deskEvidence[i].id == id)
            {
                return gameData.deskEvidence[i];
            }
        }
        for (int i = 0; i < gameData.archiveEvidence.Count; i++)
        {
            if (gameData.archiveEvidence[i].id == id)
            {
                return gameData.archiveEvidence[i];
            }
        }
        return null;
    }
    public Message GetMessageFromId(string id)
    {
        for (int i = 0; i < gameData.messages.Count; i++)
        {
            if (gameData.messages[i].id == id)
            {
                return gameData.messages[i];
            }
        }
        return new Message("", null, null, 0, 0, 0, false, false, "", 0, 0, false);
    }
    public void SetupMessages()
    {
        if (gameData.timeNextMessageAt <= 1000)
        {
            gameData.timeNextMessageAt = (Mathf.FloorToInt((TimeManager.GetTime() - System.TimeSpan.TicksPerDay * 7) / (System.TimeSpan.TicksPerHour * 4))) * (System.TimeSpan.TicksPerHour * 4);
        }
        bool hasMain = false;
        for (int i = 0; i < gameData.messages.Count; i++)
        {
            if (gameData.messages[i].name == "Serial")
            {
                hasMain = true;
            }
        }
        if (!hasMain)
        {
            gameData.messages.Add(new Message("Serial", new List<QuestItem>(){
            //QUEST ITEMS
            new QuestItem("any",new List<string>(){"any"},0,1,"any evidence")
            },
            new List<string>(){
                //MESSAGE PARTS
                "Come to me with any piece of ",
                " you collect and I can buy them as much as you want."
            },
            //TIME UPLOAD START
            -1,
            //TIME UPLOAD END
            -1,
            //TIME TO EXPIRE
            -1, true, false, null, 100, 10, false));
        }
        if (!Tutorial.inTutorial)
        {
            UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
            while (gameData.timeNextMessageAt < TimeManager.GetTime())
            {
                gameData.messages.Add(GenerateRandomMessage(gameData.timeNextMessageAt));
                gameData.timeNextMessageAt += (System.TimeSpan.TicksPerHour * 4);
            }
        }
        else
        {
            bool created = false;
            for (int i = 0; i < gameData.messages.Count;i++)
            {
                if (gameData.messages[i].name == "Bullet")
                {
                    created = true;
                }
            }
            if (!created) {
                gameData.messages.Add(new Message("Bullet", new List<QuestItem>(){
                //QUEST ITEMS
                new QuestItem("photo",new List<string>(){"handprint"},0,1,"Ghost on a target"),
                new QuestItem("photo",new List<string>(){"motion"},0,1,"Digital target"),
                new QuestItem("photo",new List<string>(){"dowsing"},0,1,"Flying target"),
                new QuestItem("photo",new List<string>(){"compass"},0,1,"Standing target")
                },
                new List<string>(){
                            //MESSAGE PARTS
                            "Please give me photos of: ",
                            " , ",
                            " , ",
                            " and ",
                            " to use for target practice!"
                },
                //TIME UPLOAD START
                -1,
                //TIME UPLOAD END
                -1,
                //TIME TO EXPIRE
                -1, true, true, null, 0, 500, false));
            }
        }
    }
    public int GetPlayerLevel(int? customExperience = null)
    {
        int experience = 0;
        if (customExperience==null)
        {
            experience = gameData.experience;
        }
        else
        {
            experience = (int)customExperience;
        }
        int nextLevelAmt = 100;
        int currentLevel = 1;
        while (experience >= nextLevelAmt)
        {
            experience -= nextLevelAmt;
            nextLevelAmt += 50;
            currentLevel++;
        }
        return currentLevel;
    }
    public int GetPlayerNextLevel(int? customExperience = null)
    {
        int experience = 0;
        if (customExperience == null)
        {
            experience = gameData.experience;
        }
        else
        {
            experience = (int)customExperience;
        }
        int nextLevelAmt = 100;
        int currentLevel = 1;
        while (experience >= nextLevelAmt)
        {
            experience -= nextLevelAmt;
            nextLevelAmt += 50;
            currentLevel++;
        }
        return nextLevelAmt;
    }
    public int GetPlayerSubExperience(int? customExperience = null)
    {
        int experience = 0;
        if (customExperience == null)
        {
            experience = gameData.experience;
        }
        else
        {
            experience = (int)customExperience;
        }
        int nextLevelAmt = 100;
        int currentLevel = 1;
        while (experience >= nextLevelAmt)
        {
            experience -= nextLevelAmt;
            nextLevelAmt += 50;
            currentLevel++;
        }
        return experience;
    }
    public Message GenerateRandomMessage(long time)
    {
        int message = -1;
        while (message == -1 || message == prevMessage)
        {
            message = UnityEngine.Random.Range(0, 476325467) % possibleMessages.Count;
        }
        prevMessage = message;

        Message baseMessage = possibleMessages[message];
        Message newMessage = new Message(baseMessage.name, baseMessage.questItems, baseMessage.messageParts, baseMessage.timeUploadStart, baseMessage.timeUploadEnd, baseMessage.timeExpire, baseMessage.deskAllow, baseMessage.archiveAllow, baseMessage.evidenceUploading, baseMessage.rewardExperience, baseMessage.rewardMoney, false);
        newMessage.questItems.Clear();
        for (int i = 0; i < newMessage.messageParts.Count - 1; i++)
        {
            int questItem = -1;
            while (questItem == -1 || questItem == prevQuestItem)
            {
                questItem = UnityEngine.Random.Range(0, 6674756) % possibleQuestItems.Count;
            }
            prevQuestItem = questItem;
            newMessage.questItems.Add(possibleQuestItems[questItem]);
        }
        newMessage.timeExpire = time + TimeSpan.TicksPerDay;

        int name = -1;
        while (name == -1 || name == prevName)
        {
            name = UnityEngine.Random.Range(0, 5485785) % possibleNames.Count;
        }
        prevName = name;

        newMessage.name = possibleNames[name];
        int totalDifficulty = 0;
        for (int i = 0; i < newMessage.questItems.Count; i++)
        {
            totalDifficulty += newMessage.questItems[i].difficulty;
        }
        float moneyRand = UnityEngine.Random.Range(0.7f, 1.3f);
        float experienceRand = UnityEngine.Random.Range(0.7f, 1.3f);
        newMessage.rewardMoney = (int)((totalDifficulty * 20) * moneyRand);
        newMessage.rewardExperience = (int)((totalDifficulty * 20) * experienceRand);
        return newMessage;
    }
    public bool CheckIfBought(string id)
    {
        for (int i = 0; i < gameData.purchasedShopItems.Count; i++)
        {
            if (gameData.purchasedShopItems[i] == id)
            {
                return true;
            }
        }
        return false;
    }
    public string Rot39(string input)
    {
        // This string contains 78 different characters in random order.
        var mix = "em-1v;2l/!sJO}>3@^utFI<U$HGST*p:'7Pa)jVn&oQK\\[g80w4#L~DbB9x]`E.{f5rz+6W(hiyRXA";
        var result = (input ?? "").ToCharArray();
        for (int i = 0; i < result.Length; ++i)
        {
            int j = mix.IndexOf(result[i]);
            result[i] = (j < 0) ? result[i] : mix[(j + 39) % 78];
        }
        return new string(result);
    }
    public bool HasBonusType(Evidence checkEvidence)
    {
        bool bonus = checkEvidence.type == gameData.bonusType;
        if (checkEvidence.focalEvidences != null)
        {
            foreach (var t in checkEvidence.focalEvidences)
            {
                if (t.type == gameData.bonusType)
                {
                    bonus = true;
                }
            }
        }
        if (checkEvidence.audibleEvidences != null)
        {
            foreach (var t in checkEvidence.audibleEvidences)
            {
                if (t.word == gameData.bonusType)
                {
                    bonus = true;
                }
            }
        }
        return bonus;
    }
    public void PurchaseIAPItem(Product product)
    {
        ReadFile();
        if (product.definition.id == "premium")
        {
            gameData.premium = true;
        }
        if (product.definition.id == "tickets1")
        {
            gameData.skipTickets += 1;
        }
        if (product.definition.id == "tickets5")
        {
            gameData.skipTickets += 5;
        }
        WriteFile();
    }

    private long GetTokenValue(string save)
    {
        string tempSave = save;
        int index = tempSave.IndexOf("\"prevAlignmentScore\":");
        char[] charSave = tempSave.ToCharArray();
        int current = index;
        string toRemove = charSave[current].ToString();
        while (charSave[current]!=',')
        {
            current++;
            toRemove += charSave[current].ToString();
        }
        tempSave = tempSave.Replace(toRemove,null);
        tempSave = tempSave.Replace("[", null);
        tempSave = tempSave.Replace("]", null);
        tempSave = tempSave.Replace("{", null);
        tempSave = tempSave.Replace("}", null);
        tempSave = tempSave.Replace(":", null);
        tempSave = tempSave.Replace(",", null);
        tempSave = tempSave.Replace("\"", null);
        tempSave = tempSave.ToLowerInvariant();
        char[] newSave = tempSave.ToCharArray();
        long temptoken = 63;
        for (int i = 0;i<newSave.Length;i++)
        {
            temptoken += (int)Char.GetNumericValue(newSave[i])+(i%3471);
        }
        return temptoken;
    }

    private void VersionChores(int prevVer)
    {
        /*if (prevVer < 1012)
        {
            for (int i = 0; i < gameData.deskEvidence.Count;i++)
            {
                if (gameData.deskEvidence[i].type=="video")
                {
                    gameData.money += 50;
                    RemoveEvidenceFromDesk(gameData.deskEvidence[i]);
                }
            }
            for (int i = 0; i < gameData.archiveEvidence.Count; i++)
            {
                if (gameData.archiveEvidence[i].type == "video")
                {
                    gameData.money += 50;
                    RemoveEvidenceFromArchive(gameData.archiveEvidence[i]);
                }
            }
        }*/
        gameData.prevPlayedBuild = int.Parse(Application.version);
    }
}
[System.Serializable]
public class SettingData
{
    public bool sounds = true;
    public bool music = true;
    public bool shadows = false;
    public bool occlusion = false;
    public int orientation = 0;
    public bool highMeshing = true;
    public uint meshUpdatePerSecond = 15;
    public float waypointDistance = 1.5f;
    public float masterWaypointDistance = 10;
    public float waypointUpdateFrequency = 1.0f;
}
[System.Serializable]
public class GameData
{
    public List<Evidence> deskEvidence = new List<Evidence>();
    public List<Evidence> archiveEvidence = new List<Evidence>();

    public int deskSlots = 3;
    public int archiveSlots = 10;

    public int experience = 0;
    public int money = 1000;

    public long timeNextMessageAt = 0;

    public List<Message> messages = new List<Message>();

    public List<string> purchasedShopItems = new List<string>();

    public long lastValidTime = 0;

    public int skipTickets = 0;
    public long skipTime = 0;

    public int ticketsClaimedToday = 0;
    public long timeClaimedToday = 0;

    public int prewatchedAds = 0;

    public long prevDayPlayed;
    public long prevAlignmentScore = 0;

    public bool premium;
    public string deviceId;

    public string bonusType = "";

    public int nextDailyReward = 0;
    public long prevDailyRewardClaimedTime = 0;
    public int prevPlayedBuild = 0;
    public long prevReviewRequest = 0;
}
[System.Serializable]
public class Message
{
    public string id;
    public string name;
    public List<QuestItem> questItems;
    public long timeUploadStart;
    public long timeUploadEnd;
    public List<string> messageParts;
    public long timeExpire;
    public bool deskAllow;
    public bool archiveAllow;
    public string evidenceUploading = null;
    public int rewardExperience;
    public int rewardMoney;
    public bool completed;
    public Message(string nName, List<QuestItem> nQuestItems, List<string> nMessageParts, long nTimeUploadStart, long nTimeUploadEnd, long nTimeExpire, bool nDeskAllow, bool nArchiveAllow,string nEvidenceUploading, int nRewardExperience, int nRewardMoney,bool nCompleted)
    {
        id = System.Guid.NewGuid().ToString();
        name = nName;
        questItems = nQuestItems;
        timeUploadStart = nTimeUploadStart;
        timeUploadEnd = nTimeUploadEnd;
        messageParts = nMessageParts;
        timeExpire = nTimeExpire;
        deskAllow = nDeskAllow;
        archiveAllow = nArchiveAllow;
        rewardExperience = nRewardExperience;
        rewardMoney = nRewardMoney;
        completed = nCompleted;

        evidenceUploading = nEvidenceUploading;
    }
}
[System.Serializable]
public class QuestItem
{
    //type = photo,video, sound
    public string type;
    public List<string> subjects;
    public int minimumScore;
    public int difficulty;
    public string messageText;
    public QuestItem(string nType,List<string> nSubjects,int nMinimumScore,int nDifficulty, string nMessageText)
    {
        type = nType;
        subjects = nSubjects;
        minimumScore = nMinimumScore;
        difficulty = nDifficulty;
        messageText = nMessageText;
    }
}
[System.Serializable]
public class Evidence
{
    public string id;
    public int frameWidth;
    public int frameHeight;
    public byte[] photo;
    public byte[] video;
    public float[] sound;

    public float score;

    public string type;
    public List<EvidenceCapture.FocalEvidence> focalEvidences;
    public List<EvidenceCapture.AudibleEvidence> audibleEvidences;

    public string messageUploadingTo = null;

    public Evidence(int nFrameWidth, int nFrameHeight, byte[] aPhoto, byte[] aVideo,float[] aSound, string aType, List<EvidenceCapture.FocalEvidence> aFocalEvidence, List<EvidenceCapture.AudibleEvidence> aAudibleEvidence)
    {
        id = System.Guid.NewGuid().ToString();
        frameWidth = nFrameWidth;
        frameHeight = nFrameHeight;
        photo = aPhoto;
        video = aVideo;
        sound = aSound;
        type = aType;
        focalEvidences = aFocalEvidence;
        audibleEvidences = aAudibleEvidence;
        messageUploadingTo = null;
        score = GetScore();
    }
    public byte[] GetVideoFrame(int frame)
    {
        int start = frame * (frameWidth * frameHeight *2);
        byte[] rowVector = new byte[(frameWidth * frameHeight*2)];

        for (var i = 0; i < (frameWidth * frameHeight*2); i++)
            rowVector[i] = video[start+i];

        return rowVector;
    }
    public float GetScore()
    {
        float totalScore = 0f;
        if (focalEvidences!=null)
        {
            foreach (var t in focalEvidences)
            {
                t.SetScore();

                float frameBalanceX = 0f;
                float frameBalanceY = 0f;

                Texture2D detection = new Texture2D(frameWidth, frameHeight, Controller.photoFormat, false);

                if (type == "video")
                {
                    detection.LoadRawTextureData(GetVideoFrame(0));
                }
                if (type == "photo")
                {
                    detection.LoadRawTextureData(photo);
                }

                foreach (var t1 in focalEvidences)
                {
                    frameBalanceX += ((float)t1.x / detection.width) - 0.5f;
                    frameBalanceY += ((float)t1.y / detection.height) - 0.5f;
                }
                float framingScore = (((float)focalEvidences.Count - (Mathf.Abs(frameBalanceX) + Mathf.Abs(frameBalanceY)))) / (float)focalEvidences.Count;

                totalScore += t.score * framingScore;
            }
        }
        if (audibleEvidences != null)
        {
            foreach (var t in audibleEvidences)
            {
                t.SetScore();

                totalScore += t.score;
            }
        }
        return totalScore;
    }
    public void SortEvidences()
    {
        if (focalEvidences != null)
        {
            for (int i = 0; i < focalEvidences.Count - 1; i++)
            {
                float e1 = focalEvidences[i].score;
                float e2 = focalEvidences[i + 1].score;
                if (e1 > e2)
                {
                    EvidenceCapture.FocalEvidence temp = focalEvidences[i];
                    focalEvidences[i] = focalEvidences[i + 1];
                    focalEvidences[i + 1] = temp;
                    i = -1;
                }
            }
        }
        if (audibleEvidences!=null) {
            for (int i = 0; i < audibleEvidences.Count - 1; i++)
            {
                float e1 = audibleEvidences[i].score;
                float e2 = audibleEvidences[i + 1].score;
                if (e1 > e2)
                {
                    EvidenceCapture.AudibleEvidence temp = audibleEvidences[i];
                    audibleEvidences[i] = audibleEvidences[i + 1];
                    audibleEvidences[i + 1] = temp;
                    i = -1;
                }
            }
        }
    }
    public bool SatisfiesQuest(QuestItem quest)
    {
        int satisfiedSubjects = 0;
        bool ghostCounted = false;
        if (quest.type == type || quest.type == "any") {
            for (int s = 0; s < quest.subjects.Count;s++)
            {
                ghostCounted = false;
                if (type == "photo" || type == "video")
                {
                    foreach (var t in focalEvidences)
                    {
                        bool scoreMinReached = true;//GetScore() >= quest.minimumScore;
                        bool ghostCapture = (t.type == "ghost" || t.type == "ghosthead" || t.type == "ghostbody" || t.type == "ghosthand" || t.type == "ghostfoot");
                        if (scoreMinReached && (t.type==quest.subjects[s]|| (quest.subjects[s]=="ghost" && ghostCapture && !ghostCounted) || quest.subjects[s]=="any"))
                        {
                            if (ghostCapture)
                            {
                                ghostCounted = true;
                            }
                            satisfiedSubjects ++;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("NOT CORRECT TYPE");
            return false;
        }
        if (satisfiedSubjects>=quest.subjects.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}
