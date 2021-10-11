using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using Vuforia;

namespace BCCH
{
    public class SepCameraPosIndicator : MonoBehaviour
    {
        public SimulationManager simulationManager;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine("CheckCalibration");
        }

        IEnumerator CheckCalibration()
        {
            //BUILD 22
            while (this.transform.position == Vector3.zero)
            {
                yield return new WaitForEndOfFrame();
                if (this.transform.position != Vector3.zero)
                {
                    //simulationManager.uiManager.OnCallMessage(transform.TransformPoint(this.transform.localPosition) + "\nCurrentFrame: " + Time.frameCount);
                    Debug.LogError(transform.TransformPoint(this.transform.localPosition) + "\n" + this.transform.localRotation + "\nCurrentFrame: " + Time.frameCount);
                    simulationManager.REG_CALIBRATION_CameraInitPos = transform.TransformPoint(this.transform.localPosition);
                    simulationManager.REG_CALIBRATION_CameraInitRot = this.transform.rotation.eulerAngles;
                    
                }
            }
            if (this.transform.position != Vector3.zero)
            {
                //simulationManager.uiManager.OnCallMessage(transform.TransformPoint(this.transform.localPosition) + "\nCurrentFrame: " + Time.frameCount);
                Debug.LogError(transform.TransformPoint(this.transform.localPosition) + "\n" + this.transform.localRotation + "\nCurrentFrame: " + Time.frameCount);
                simulationManager.REG_CALIBRATION_CameraInitPos = transform.TransformPoint(this.transform.localPosition);
                simulationManager.REG_CALIBRATION_CameraInitRot = this.transform.rotation.eulerAngles;
            }

            //BUILD 18
            //while (this.GetComponent<TrackedPoseDriver>() == null)
            //{
            //    yield return new WaitForEndOfFrame();
            //    if (this.GetComponent<TrackedPoseDriver>() != null)
            //    {
            //        simulationManager.uiManager.OnCallMessage("Enabled relative transfrom\n"+ this.transform.localPosition.ToString() + "\nCurrentFrame: " + Time.frameCount);
            //        Debug.LogError("Enabled relative transfrom\n" + this.transform.localPosition.ToString() + "\nCurrentFrame: " + Time.frameCount);
            //        this.GetComponent<TrackedPoseDriver>().UseRelativeTransform = true;
            //    }
            //}
            //if (this.GetComponent<TrackedPoseDriver>() != null)
            //{
            //    simulationManager.uiManager.OnCallMessage("Enabled relative transfrom\n" + this.transform.localPosition.ToString() + "\nCurrentFrame: " + Time.frameCount);
            //    Debug.LogError("Enabled relative transfrom\n" + this.transform.localPosition.ToString() + "\nCurrentFrame: " + Time.frameCount);
            //    this.GetComponent<TrackedPoseDriver>().UseRelativeTransform = true;
            //}

            //UNTESTED
            //while (!Vuforia.CameraDevice.Instance.IsActive())
            //{
            //    yield return new WaitForEndOfFrame();
            //    if (Vuforia.CameraDevice.Instance.IsActive())
            //    {
            //        Debug.LogError(this.transform.localPosition.ToString() + "\nCurrentFrame: " + Time.frameCount);
            //        //Debug.LogError("World Center: \n" + VuforiaARController.Instance.WorldCenter.transform.position.ToString());
            //        simulationManager.uiManager.OnCallMessage(this.transform.localPosition.ToString() + "\nCurrentFrame: " + Time.frameCount);
            //        simulationManager.REG_CALIBRATION_CameraInitPos = this.transform.localPosition;
            //    }
            //}
            //if (Vuforia.CameraDevice.Instance.IsActive())
            //{
            //    Debug.LogError(this.transform.localPosition.ToString() + "\nCurrentFrame: " + Time.frameCount);
            //    //Debug.LogError("World Center: \n" + VuforiaARController.Instance.WorldCenter.transform.position.ToString());
            //    simulationManager.uiManager.OnCallMessage(this.transform.localPosition.ToString() + "\nCurrentFrame: " + Time.frameCount);
            //    simulationManager.REG_CALIBRATION_CameraInitPos = this.transform.localPosition;
            //}

            //BUILD 17
            //while (!VuforiaRuntimeUtilities.IsVuforiaEnabled() || !VuforiaRuntimeUtilities.IsWebCamUsed())
            //{
            //    yield return new WaitForEndOfFrame();
            //    if (VuforiaRuntimeUtilities.IsVuforiaEnabled() && VuforiaRuntimeUtilities.IsWebCamUsed())
            //    {
            //        Debug.LogError(this.transform.localPosition.ToString() + "\nCurrentFrame: " + Time.frameCount);

            //        //Debug.LogError("World Center: \n" + VuforiaARController.Instance.WorldCenter.transform.position.ToString());
            //        simulationManager.uiManager.OnCallMessage(this.transform.localPosition.ToString() + "\nCurrentFrame: " + Time.frameCount);
            //        simulationManager.REG_CALIBRATION_CameraInitPos = this.transform.localPosition;
            //    }
            //}
            //if (VuforiaRuntimeUtilities.IsVuforiaEnabled() && VuforiaRuntimeUtilities.IsWebCamUsed())
            //{

            //    Debug.LogError(this.transform.localPosition.ToString() + "\nCurrentFrame: " + Time.frameCount);

            //    //Debug.LogError("World Center: \n" + VuforiaARController.Instance.WorldCenter.transform.position.ToString());
            //    simulationManager.uiManager.OnCallMessage(this.transform.localPosition.ToString() + "\nCurrentFrame: " + Time.frameCount);
            //    simulationManager.REG_CALIBRATION_CameraInitPos = this.transform.localPosition;
            //}
        }
    }
}

