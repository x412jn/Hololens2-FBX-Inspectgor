using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using BCCH;

public class SepTest : MonoBehaviour
{
    public Transform cam;
    
    private void Start()
    {
        Debug.LogError(cam.transform.TransformPoint(cam.transform.localPosition) + "\n" + cam.transform.localRotation + "\nCurrentFrame: " + Time.frameCount);
        //Debug.LogError(cam.localPosition.ToString() + "\nCurrentFrame: " + Time.frameCount);
        //Debug.LogError("World Center: \n" + VuforiaARController.Instance.WorldCenter.transform.position.ToString());
    }

}
