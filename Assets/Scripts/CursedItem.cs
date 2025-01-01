using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedItem : MonoBehaviour
{
    public bool collected;

    public MeshRenderer myRenderer;
    // Start is called before the first frame update
    void Start()
    {
        myRenderer = GetComponentInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (collected) {
            myRenderer.enabled = false;
        }
    }
}
