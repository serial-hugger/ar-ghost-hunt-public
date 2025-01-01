using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhotoMedia : MonoBehaviour
{
    public Renderer myRenderer;

    public Material photo;

    public Evidence myEvidence;

    public TextMeshPro paperText;

    public float score;
    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < myRenderer.materials.Length; i++)
        {
                Material newMat = new Material(myRenderer.materials[i]);
                myRenderer.materials[i] = newMat;
                if (myRenderer.materials[i].name.Contains("Photo"))
                {
                    photo = myRenderer.materials[i];
                }
        }
    }

    private void Start()
    {
        //TEXTURE SET
        Texture2D tex;
        try
        {
            tex = new Texture2D(myEvidence.frameWidth, myEvidence.frameHeight, Controller.photoFormat, false);
        }
        catch
        {
            tex = new Texture2D(myEvidence.frameWidth, myEvidence.frameHeight, TextureFormat.RGBA32, false);
        }
        tex.LoadRawTextureData(myEvidence.photo);
        tex.Apply();
        photo.mainTexture = tex;


        //display paper text
        score = myEvidence.GetScore();
        myEvidence.SortEvidences();
        string paperString = "";
        int ghostParts = 0;
        for (int i = 0; i < myEvidence.focalEvidences.Count;i++)
        {
            if (myEvidence.focalEvidences[i].type.Contains("ghost"))
            {
                ghostParts += 1;
            }
        }
        bool ghostLabeled = false;
        for (int i = 0; i < myEvidence.focalEvidences.Count; i++)
        {
            if (myEvidence.focalEvidences.Count >= i+1)
            {
                if (myEvidence.focalEvidences[i].type.Contains("ghost")) {
                    if (ghostParts == 1) {
                        paperString += myEvidence.focalEvidences[i].type + "\n";
                    }
                    if (!ghostLabeled) {
                        if (ghostParts == 2)
                        {
                            paperString += "partial ghost" + "\n";
                            ghostLabeled = true;
                        }
                        if (ghostParts == 3)
                        {
                            paperString += "majority ghost" + "\n";
                            ghostLabeled = true;
                        }
                        if (ghostParts == 4)
                        {
                            paperString += "full ghost" + "\n";
                            ghostLabeled = true;
                        }
                    }
                }
                else
                {
                    paperString += myEvidence.focalEvidences[i].type + "\n";
                }
            }
            else
            {
                if (i<=0) {
                    paperString += "Nothing";
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
