using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeveloperOnly : MonoBehaviour
{
    public bool enabledInDeveloper;

    // Start is called before the first frame update
    void Awake()
    {
        if (Debug.isDebugBuild||Application.isEditor)
        {
            if (!enabledInDeveloper)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (enabledInDeveloper)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
