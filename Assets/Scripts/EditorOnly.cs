using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorOnly : MonoBehaviour
{
    public bool enabledInEditor;

    // Start is called before the first frame update
    void Awake()
    {
        if (Application.isEditor) {
            if (!enabledInEditor)
            {
                gameObject.SetActive(false);
            }
        }
        else {
            if (enabledInEditor)
            {
                gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
