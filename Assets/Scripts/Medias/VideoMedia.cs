using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VideoMedia : MonoBehaviour
{
    public Renderer myRenderer;

    public Material shot1;
    public Material shot2;
    public Material shot3;
    public Material shot4;
    public Material shot5;
    public Material shot6;
    public Material shot7;
    public Material shot8;
    public Material shot9;

    public Evidence myEvidence;

    public List<EvidenceCapture.FocalEvidence> focalEvidences;

    public TextMeshPro paperText;

    public float score;

    private void Awake()
    {
        for (int i = 0; i < myRenderer.materials.Length;i++)
        {
            if (myRenderer.materials[i].name.Contains("Shot")) {
                Material newMat = new Material(myRenderer.materials[i]);
                myRenderer.materials[i] = newMat;
                if (myRenderer.materials[i].name.Contains("Shot1"))
                {
                    shot1 = myRenderer.materials[i];
                }
                if (myRenderer.materials[i].name.Contains("Shot2"))
                {
                    shot2 = myRenderer.materials[i];
                }
                if (myRenderer.materials[i].name.Contains("Shot3"))
                {
                    shot3 = myRenderer.materials[i];
                }
                if (myRenderer.materials[i].name.Contains("Shot4"))
                {
                    shot4 = myRenderer.materials[i];
                }
                if (myRenderer.materials[i].name.Contains("Shot5"))
                {
                    shot5 = myRenderer.materials[i];
                }
                if (myRenderer.materials[i].name.Contains("Shot6"))
                {
                    shot6 = myRenderer.materials[i];
                }
                if (myRenderer.materials[i].name.Contains("Shot7"))
                {
                    shot7 = myRenderer.materials[i];
                }
                if (myRenderer.materials[i].name.Contains("Shot8"))
                {
                    shot8 = myRenderer.materials[i];
                }
                if (myRenderer.materials[i].name.Contains("Shot9"))
                {
                    shot9 = myRenderer.materials[i];
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        int amount = (EvidenceCapture.videoFramesPerSecond * 15) / 8;
        //TEXTURE SET
        Texture2D tex = new Texture2D(myEvidence.frameWidth, myEvidence.frameHeight, Controller.videoFormat, false);
        tex.LoadRawTextureData(myEvidence.GetVideoFrame(amount * 0));
        tex.Apply();
        shot1.mainTexture = tex;
        tex.LoadRawTextureData(myEvidence.GetVideoFrame(amount * 1));
        tex.Apply();
        shot2.mainTexture = tex;
        tex.LoadRawTextureData(myEvidence.GetVideoFrame(amount * 2));
        tex.Apply();
        shot3.mainTexture = tex;
        tex.LoadRawTextureData(myEvidence.GetVideoFrame(amount * 3));
        tex.Apply();
        shot4.mainTexture = tex;
        tex.LoadRawTextureData(myEvidence.GetVideoFrame(amount * 4));
        tex.Apply();
        shot5.mainTexture = tex;
        tex.LoadRawTextureData(myEvidence.GetVideoFrame(amount * 5));
        tex.Apply();
        shot6.mainTexture = tex;
        tex.LoadRawTextureData(myEvidence.GetVideoFrame(amount * 6));
        tex.Apply();
        shot7.mainTexture = tex;
        tex.LoadRawTextureData(myEvidence.GetVideoFrame(amount * 7));
        tex.Apply();
        shot8.mainTexture = tex;
        tex.LoadRawTextureData(myEvidence.GetVideoFrame(amount * 8));
        tex.Apply();
        shot9.mainTexture = tex;

        //display paper text
        score = myEvidence.GetScore();
        myEvidence.SortEvidences();
        string paperString = "";
        int ghostParts = 0;
        for (int i = 0; i < myEvidence.focalEvidences.Count; i++)
        {
            if (myEvidence.focalEvidences[i].type == "ghosthead" || myEvidence.focalEvidences[i].type == "ghostbody" || myEvidence.focalEvidences[i].type == "ghosthand" || myEvidence.focalEvidences[i].type == "ghostfoot")
            {
                ghostParts += 1;
            }
        }
        bool ghostLabeled = false;
        int lines = 0;
        for (int i = 0; i < myEvidence.focalEvidences.Count; i++)
        {
            if (lines<5) {
                if (myEvidence.focalEvidences.Count >= i + 1)
                {
                    if (myEvidence.focalEvidences[i].type == "ghosthead" || myEvidence.focalEvidences[i].type == "ghostbody"|| myEvidence.focalEvidences[i].type == "ghosthand"|| myEvidence.focalEvidences[i].type == "ghostfoot")
                    {
                        if (ghostParts == 1)
                        {
                            paperString += myEvidence.focalEvidences[i].type + "\n";
                            lines++;
                        }
                        if (!ghostLabeled)
                        {
                            if (ghostParts == 2)
                            {
                                paperString += "partial ghost" + "\n";
                                ghostLabeled = true;
                                lines++;
                            }
                            if (ghostParts == 3)
                            {
                                paperString += "majority ghost" + "\n";
                                ghostLabeled = true;
                                lines++;
                            }
                            if (ghostParts == 4)
                            {
                                paperString += "full ghost" + "\n";
                                ghostLabeled = true;
                                lines++;
                            }
                        }
                    }
                    else
                    {
                        paperString += myEvidence.focalEvidences[i].type + "\n";
                        lines++;
                    }
                }
                else
                {
                    if (i <= 0)
                    {
                        paperString += "Nothing";
                    }
                }
            }
        }
        if (myEvidence.focalEvidences.Count == 0)
        {
            paperString = "Nothing";
        }
        paperText.text = paperString;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
