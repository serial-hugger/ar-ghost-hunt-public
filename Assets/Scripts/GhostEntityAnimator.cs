using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEntityAnimator : MonoBehaviour
{
    //skin
    public List<Renderer> skinRends;
    //detections
    public List<Renderer> headRends;
    public List<Renderer> bodyRends;
    public List<Renderer> handRends;
    public List<Renderer> footRends;

    public Material skinMat;
    public Material staticSkinMat;

    public Material headMat;
    public Material staticHeadMat;
    public Material bodyMat;
    public Material staticBodyMat;
    public Material handMat;
    public Material staticHandMat;
    public Material footMat;
    public Material staticFootMat;

    private float timeTillCutoffOffsetChange = 0.075f;
    public float currentCutoffOffset = 0.8f;
    private float timeTillScaleChange = 0.1f;
    public float currentScale = 2f;

    [Range(0.0f, 1.0f)]
    public float visibility;

    public float maxVisibility = 1.0f;

    public bool vanish;

    public Animator myAnimator;

    public GhostEntity ghostScript;

    public bool tall;

    // Start is called before the first frame update
    void Start()
    {
        SetMaterials();
        if (ghostScript.staticPhotoOnly)
        {
            vanish = false;
            visibility = Random.Range(0.5f,1.0f);
            currentScale = Random.Range(1.995f, 2.005f);
            currentCutoffOffset = Random.Range(-0.01f, 0.01f);
            //update skin materials
            for (int i = 0; i < skinRends.Count; i++)
            {
                skinRends[i].material.mainTextureOffset = new Vector2(Time.time * 0.05f, Time.time * 0.05f);
                skinRends[i].material.mainTextureScale = new Vector2(currentScale, currentScale);
                skinRends[i].material.SetFloat("_Cutoff",  (0.88f - (visibility * 0.04f)) + currentCutoffOffset);
                skinRends[i].gameObject.layer = LayerMask.NameToLayer("PhotoOnly");
            }
            //update skin materials
            for (int i = 0; i < headRends.Count; i++)
            {
                headRends[i].material.mainTextureOffset = new Vector2(Time.time * 0.05f, Time.time * 0.05f);
                headRends[i].material.mainTextureScale = new Vector2(currentScale, currentScale);
                headRends[i].material.SetFloat("_Cutoff", (0.88f - (visibility * 0.04f)) + currentCutoffOffset);
            }
            //update body materials
            for (int i = 0; i < bodyRends.Count; i++)
            {
                bodyRends[i].material.mainTextureOffset = new Vector2(Time.time * 0.05f, Time.time * 0.05f);
                bodyRends[i].material.mainTextureScale = new Vector2(currentScale, currentScale);
                bodyRends[i].material.SetFloat("_Cutoff", (0.88f - (visibility * 0.04f)) + currentCutoffOffset);
            }
            //update hand materials
            for (int i = 0; i < handRends.Count; i++)
            {
                handRends[i].material.mainTextureOffset = new Vector2(Time.time * 0.05f, Time.time * 0.05f);
                handRends[i].material.mainTextureScale = new Vector2(currentScale, currentScale);
                handRends[i].material.SetFloat("_Cutoff", (0.88f - (visibility * 0.04f)) + currentCutoffOffset);
            }
            //update foot materials
            for (int i = 0; i < footRends.Count; i++)
            {
                footRends[i].material.mainTextureOffset = new Vector2(Time.time * 0.05f, Time.time * 0.05f);
                footRends[i].material.mainTextureScale = new Vector2(currentScale, currentScale);
                footRends[i].material.SetFloat("_Cutoff", (0.88f - (visibility * 0.04f)) + currentCutoffOffset);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!ghostScript.staticPhotoOnly)
        {
            if (visibility > maxVisibility)
            {
                visibility = maxVisibility;
            }
            maxVisibility -= 0.001f * Time.deltaTime;
            timeTillCutoffOffsetChange -= 1f * Time.deltaTime;
            timeTillScaleChange -= 1f * Time.deltaTime;
            if (timeTillCutoffOffsetChange <= 0f)
            {
                timeTillCutoffOffsetChange = 0.075f;
                currentCutoffOffset = Random.Range(-0.01f, 0.01f);
            }
            if (timeTillScaleChange <= 0f)
            {
                timeTillScaleChange = 0.1f;
                currentScale = Random.Range(1.995f, 2.005f);
            }
            UpdateMaterials(false);
        }
    }
    public void UpdateMaterials(bool instant)
    {
        int add;
        if (instant)
        {
            add = 1;
        }
        else
        {
            add = 0;
        }
        //update skin materials
        for (int i = 0; i < skinRends.Count; i++)
        {
            skinRends[i].material.mainTextureOffset = new Vector2(Time.time * 0.05f, Time.time * 0.05f);
            skinRends[i].material.mainTextureScale = new Vector2(currentScale, currentScale);
            if (!vanish)
            {
                skinRends[i].material.SetFloat("_Cutoff", Mathf.MoveTowards(skinRends[i].material.GetFloat("_Cutoff"), (0.88f - (visibility * 0.04f)) + currentCutoffOffset, (0.2f * Time.deltaTime)+add));
            }
            else
            {
                skinRends[i].material.SetFloat("_Cutoff", Mathf.MoveTowards(skinRends[i].material.GetFloat("_Cutoff"), 1f, (0.2f * Time.deltaTime) + add));
            }
        }
        //update head materials
        for (int i = 0; i < headRends.Count; i++)
        {
            headRends[i].material.mainTextureOffset = new Vector2(Time.time * 0.05f, Time.time * 0.05f);
            headRends[i].material.mainTextureScale = new Vector2(currentScale, currentScale);
            if (!vanish)
            {
                headRends[i].material.SetFloat("_Cutoff", Mathf.MoveTowards(headRends[i].material.GetFloat("_Cutoff"), (0.5f - (visibility * 0.04f)) + currentCutoffOffset, (0.2f * Time.deltaTime) + add));
            }
            else
            {
                headRends[i].material.SetFloat("_Cutoff", Mathf.MoveTowards(headRends[i].material.GetFloat("_Cutoff"), 1f, (0.2f * Time.deltaTime) + add));
            }
        }
        //update body materials
        for (int i = 0; i < bodyRends.Count; i++)
        {
            bodyRends[i].material.mainTextureOffset = new Vector2(Time.time * 0.05f, Time.time * 0.05f);
            bodyRends[i].material.mainTextureScale = new Vector2(currentScale, currentScale);
            if (!vanish)
            {
                bodyRends[i].material.SetFloat("_Cutoff", Mathf.MoveTowards(bodyRends[i].material.GetFloat("_Cutoff"), (0.5f - (visibility * 0.04f)) + currentCutoffOffset, (0.2f * Time.deltaTime) + add));
            }
            else
            {
                bodyRends[i].material.SetFloat("_Cutoff", Mathf.MoveTowards(bodyRends[i].material.GetFloat("_Cutoff"), 1f, (0.2f * Time.deltaTime) + add));
            }
        }
        //update hand materials
        for (int i = 0; i < handRends.Count; i++)
        {
            handRends[i].material.mainTextureOffset = new Vector2(Time.time * 0.05f, Time.time * 0.05f);
            handRends[i].material.mainTextureScale = new Vector2(currentScale, currentScale);
            if (!vanish)
            {
                handRends[i].material.SetFloat("_Cutoff", Mathf.MoveTowards(handRends[i].material.GetFloat("_Cutoff"), (0.5f - (visibility * 0.04f)) + currentCutoffOffset, (0.2f * Time.deltaTime) + add));
            }
            else
            {
                handRends[i].material.SetFloat("_Cutoff", Mathf.MoveTowards(handRends[i].material.GetFloat("_Cutoff"), 1f, (0.2f * Time.deltaTime) + add));
            }
        }
        //update foot materials
        for (int i = 0; i < footRends.Count; i++)
        {
            footRends[i].material.mainTextureOffset = new Vector2(Time.time * 0.05f, Time.time * 0.05f);
            footRends[i].material.mainTextureScale = new Vector2(currentScale, currentScale);
            if (!vanish)
            {
                footRends[i].material.SetFloat("_Cutoff", Mathf.MoveTowards(footRends[i].material.GetFloat("_Cutoff"), (0.5f - (visibility * 0.04f)) + currentCutoffOffset, (0.2f * Time.deltaTime) + add));
            }
            else
            {
                footRends[i].material.SetFloat("_Cutoff", Mathf.MoveTowards(footRends[i].material.GetFloat("_Cutoff"), 1f, (0.2f * Time.deltaTime) + add));
            }
        }
    }
    public void SetMaterials()
    {
        //set skin materials
        for (int s = 0; s < skinRends.Count; s++)
        {
            if (!ghostScript.staticPhotoOnly)
            {
                skinRends[s].material = skinMat;
            }
            else
            {
                skinRends[s].material = staticSkinMat;
            }
        }
        //set head materials
        for (int s = 0; s < headRends.Count; s++)
        {
            if (!ghostScript.staticPhotoOnly)
            {
                headRends[s].material = headMat;
            }
            else
            {
                headRends[s].material = staticHeadMat;
            }
        }
        //set body materials
        for (int s = 0; s < bodyRends.Count; s++)
        {
            if (!ghostScript.staticPhotoOnly)
            {
                bodyRends[s].material = bodyMat;
            }
            else
            {
                bodyRends[s].material = staticBodyMat;
            }
        }
        //set hand materials
        for (int s = 0; s < handRends.Count; s++)
        {
            if (!ghostScript.staticPhotoOnly)
            {
                handRends[s].material = handMat;
            }
            else
            {
                handRends[s].material = staticHandMat;
            }
        }
        //set foot materials
        for (int s = 0; s < footRends.Count; s++)
        {
            if (!ghostScript.staticPhotoOnly)
            {
                footRends[s].material = footMat;
            }
            else
            {
                footRends[s].material = staticFootMat;
            }
        }
    }
}
