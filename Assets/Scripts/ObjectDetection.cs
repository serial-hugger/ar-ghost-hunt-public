using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Niantic.Lightship.AR.ObjectDetection;
using Niantic.Lightship.AR.Subsystems.ObjectDetection;
using Niantic.Lightship.AR.XRSubsystems;
using TMPro;
using UnityEngine;

public class ObjectDetectionOutputManager : MonoBehaviour
{
    [SerializeField]
    private ARObjectDetectionManager _objectDetectionManager;

    [SerializeField]
    public TMP_Text _objectsDetectedText;

    private void Start()
    {
        _objectDetectionManager.enabled = true;
        _objectDetectionManager.MetadataInitialized += OnMetadataInitialized;
    }

    private void OnMetadataInitialized(ARObjectDetectionModelEventArgs args)
    {
        _objectDetectionManager.ObjectDetectionsUpdated += ObjectDetectionsUpdated;
    }

    private void ObjectDetectionsUpdated(ARObjectDetectionsUpdatedEventArgs args)
    {
        //Initialize our output string
        string resultString = "";
        var result = args.Results;

        if (result == null)
        {
            Debug.Log("No results found.");
            return;
        }

        //Reset our results string
        resultString = "";

        //Iterate through our results
        for (int i = 0; i < result.Count; i++)
        {
            var detection = result[i];
            var categorizations = detection.GetConfidentCategorizations();
            if (categorizations.Count <= 0)
            {
                break;
            }

            //Sort our categorizations by highest confidence
            categorizations.Sort((a, b) => b.Confidence.CompareTo(a.Confidence));

            //Iterate through found categoires and form our string to output
            for (int j = 0; j < categorizations.Count; j++)
            {
                var categoryToDisplay = categorizations[j];

                resultString += "Detected " + $"{categoryToDisplay.CategoryName}: " + "with " + $"{categoryToDisplay.Confidence} Confidence \n";
            }
        }

        //Output our string
        _objectsDetectedText.text = resultString;
    }


    private void OnDestroy()
    {
        _objectDetectionManager.MetadataInitialized -= OnMetadataInitialized;
        _objectDetectionManager.ObjectDetectionsUpdated -= ObjectDetectionsUpdated;
    }
}
