using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Vuforia;
using System.Linq;

namespace BCCH
{
    public class OverlayManager : MonoBehaviour
    {
        DataSet dataSet;
        ObjectTracker objectTracker;

        List<TrackableBehaviour> tbList;

        // Use this for initialization
        void Start()
        {

            tbList = new List<TrackableBehaviour>();

            //Debug.LogError(Vuforia.VuforiaARController.Instance.WorldCenter.ToString());
        }



        // Load DataSet, make ModelTarget gameObject and activate it to target tracking
        void LoadDataSet()
        {
            objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        }

        //public void StartInitVuforia()
        //{
        //    if(VuforiaRuntime.Instance.InitializationState == VuforiaRuntime.InitState.INITIALIZING)
        //    {
        //        Log.Text(SimulationManager.instance.label, "On Pending Last Request");
        //        Debug.Log("On Pending Last Request");
        //    }
        //    else
        //    {
        //        Log.Text(SimulationManager.instance.label, "Start Calibration");
        //        StartCoroutine("InitVuforia");
        //    }
        //}

        //IEnumerator InitVuforia()
        //{
        //    VuforiaRuntime.Instance.Deinit();
        //    while(VuforiaRuntime.Instance.InitializationState != VuforiaRuntime.InitState.NOT_INITIALIZED)
        //    {
        //        yield return new WaitForSeconds(0.3f);
        //        if(VuforiaRuntime.Instance.InitializationState == VuforiaRuntime.InitState.NOT_INITIALIZED)
        //        {
        //            Log.Text(SimulationManager.instance.label, "deint complete");
        //            VuforiaRuntime.Instance.InitVuforia();
        //            while (VuforiaRuntime.Instance.InitializationState == VuforiaRuntime.InitState.INITIALIZING)
        //            {
        //                yield return new WaitForSeconds(0.3f);
        //                if (VuforiaRuntime.Instance.InitializationState == VuforiaRuntime.InitState.INITIALIZED)
        //                {
        //                    SimulationManager.instance.uiManager.OnCallMessage("Vuforia Calibrated");
        //                    goto Finish;
        //                }
        //            }
        //        }
        //    }


        //Finish:
        //    Debug.Log("Calibration Complete");

        //}

        /// <summary>
        /// Call this method on activate overlay
        /// </summary>
        /// <param name="Name">Current dataset name</param>
        /// <param name="xmlName">current xml name for this dataset</param>
        public void OnOverlayEnable(string datasetXml)
        {

            VuforiaARController.Instance.RegisterVuforiaStartedCallback(LoadDataSet);
            dataSet = objectTracker.CreateDataSet();

            string tempPath = Path.Combine(Application.persistentDataPath, SimulationManager.instance.REG_INFO_CurrentContainer);
            if (dataSet.Load(Path.Combine(tempPath, datasetXml), VuforiaUnity.StorageType.STORAGE_ABSOLUTE))   // load dataset from external source
            {
                //Log.Text(SimulationManager.instance.label, "Loaded xml:" + datasetXml + "\nPath: " + Path.Combine(tempPath, datasetXml));
                Debug.Log("Loaded xml:" + datasetXml + "\nPath: " + Path.Combine(tempPath, datasetXml));
                objectTracker.Stop();  // stop tracker so that we can add new dataset

                if (!objectTracker.ActivateDataSet(dataSet))    // activate dataset
                {
                    Debug.LogError("<color=red>Failed to Activate DataSet: " + datasetXml + "</color>");
                    Log.Text(SimulationManager.instance.label, "<color=red>Failed to Activate DataSet: " + datasetXml + "</color>");
                }

                //SimulationManager.instance.Overlay_OnLostTracked();


                //if (tbList[0] != null)
                //{
                //    for(int i = 0; i < tbList.Count; i++)
                //    {
                //        Destroy(tbList[i].gameObject);
                //    }
                //    tbList.Clear();
                //}

                IEnumerable<TrackableBehaviour> tbs = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();
                Debug.Log("Ienumerable proceeded");
                foreach (TrackableBehaviour tb in tbs)
                {
                    if (tb is ModelTargetBehaviour)
                    {
                        tbList.Add(tb);

                        Debug.Log("tb is model target behavior and tb is active and enabled");
                        tb.name = "Generated-" + tb.TrackableName;    // rename GameObject
                        Debug.Log("<color=yellow> TrackableName: " + tb.TrackableName + "</color>");
                        (tb as ModelTargetBehaviour).GuideViewMode = ModelTargetBehaviour.GuideViewDisplayMode.GuideView2D; // display guide view

                        //VuforiaARController.Instance.SetWorldCenter(tb);
                        SimulationManager.instance.REG_VUFORIA_CurrentTrackable = (tb as ModelTargetBehaviour);

                        if (SimulationManager.instance.REG_CurrentObject_Spawned != null && SimulationManager.instance.CHECK_downloadComplete)
                        {
                            //ATTENTION: Vuforia will create its own coordination system base on device pose,
                            //however, it is not synchronized with unity's coordination system. And two
                            //coordination system will cause overlay offset issue.
                            //There are about 133~155 frames of gaps between initialization of Unity and
                            //Vuforia runtime on Hololens2, if user moves their head(change device's
                            //physical position and rotation), the second coordination that created by Vuforia
                            //will be offset from unity's (0,0,0) point.

                            //And when you trying to overlay your virtual object to physical object,
                            //Vuforia will calculate the relative position base on their own coordination system
                            //instead of unity's coordination system, which will lead to offset issue.

                            //Right now, on Vuforia 9.8.8, there are no feasible solutions to manually synchronize
                            //two coordination. Thus, my recommend solution is create a new calibration cooridnation
                            //upon Vuforia's by calculating the value of offset (including position and rotation) between 
                            //two systems, then base on that offset value, create a new gameObject with corrected rotation
                            //information under vuforia trackable behavior, spawn your virtual object as child of that gameObject
                            //then applies the corrected position to the virtual object.

                            //This bug would will only pops up on Hololens since PC is way faster than that.
                            //Therefore, to solve this issue, you have to deploy this app on hololens device
                            //and compare the result by your self. Yet, to deploy this app on hololens, each time it will takes
                            //about 30 mins in total. And since we are running out of time, we haven't got chance to examing
                            //our hypothesis. If you wants fix this issue, we hope this might give you some help.

                            //(btw, we are using a class called "CameraPosIndicator" to detect the offset value.
                            //it will automatically detect the first time when camera position is not Vector3.zero and
                            //record it's position and rotation information to Simulation Manager that allows us to utilize.)

                            //Good luck!

                            //Establish new calibrated coordination
                            GameObject calibrateObject = Instantiate(
                                SimulationManager.instance.HOLDER_OverlayCalibration,
                                SimulationManager.instance.REG_VUFORIA_CurrentTrackable.transform);
                            calibrateObject.transform.localPosition = Vector3.zero;
                            calibrateObject.transform.localRotation = Quaternion.identity;

                            //calibrateObject.transform.localRotation = Quaternion.Euler(
                            //    -SimulationManager.instance.REG_CALIBRATION_CameraInitRot.x,
                            //    -SimulationManager.instance.REG_CALIBRATION_CameraInitRot.y,
                            //    -SimulationManager.instance.REG_CALIBRATION_CameraInitRot.z);

                            //calibrateObject.transform.rotation.Set(0, 0, 0, 1);
                            //calibrateObject.transform.localRotation.Set(
                            //    0-SimulationManager.instance.REG_CALIBRATION_CameraInitRot.x,
                            //    0-SimulationManager.instance.REG_CALIBRATION_CameraInitRot.y,
                            //    0-SimulationManager.instance.REG_CALIBRATION_CameraInitRot.z,
                            //    )

                            //SimulationManager.instance.uiManager.OnCallMessage("world: \n" + calibrateObject.transform.rotation.ToString() + "\n" + calibrateObject.transform.localRotation.ToString());

                            //REG_CurrentObject_Spawned.transform.parent = REG_ANCHOR_VuforiaTrackable;
                            SimulationManager.instance.REG_CurrentObject_Spawned.transform.parent = calibrateObject.transform;


                            //25
                            //SimulationManager.instance.REG_CurrentObject_Spawned.transform.localPosition = Vector3.zero + SimulationManager.instance.REG_CALIBRATION_CameraInitPos;

                            //26
                            //SimulationManager.instance.REG_CurrentObject_Spawned.transform.localPosition = Vector3.zero - SimulationManager.instance.REG_CALIBRATION_CameraInitPos;

                            //29
                            //SimulationManager.instance.REG_CurrentObject_Spawned.transform.localPosition = Vector3.zero + transform.InverseTransformDirection(SimulationManager.instance.REG_CALIBRATION_CameraInitPos);


                            SimulationManager.instance.REG_CurrentObject_Spawned.transform.localPosition = Vector3.zero;
                            //REG_CurrentObject_Spawned.transform.rotation = REG_ANCHOR_VuforiaTrackable.rotation;
                            SimulationManager.instance.REG_CurrentObject_Spawned.transform.localRotation = Quaternion.identity;
                            SimulationManager.instance.REG_CurrentObject_Spawned.transform.localScale = Vector3.one;
                        }

                        // add additional script components for trackable
                        //tb.gameObject.AddComponent<DefaultTrackableEventHandler>();
                        tb.gameObject.AddComponent<SepOnListening>();
                        tb.GetComponent<SepOnListening>().overlayManager = this;
                        tb.GetComponent<SepOnListening>().SimulationManager.instance = SimulationManager.instance;
                        tb.gameObject.AddComponent<TurnOffBehaviour>();
                        // GameObject.Find("Tracking").GetComponent<Tracking>().targetObject = tb.gameObject;   // Tracking status visualization - my own Canvas panel script 
                    }
                    else
                    {
                        tb.gameObject.SetActive(false);
                        if (SimulationManager.instance.REG_CurrentObject_Spawned != null && SimulationManager.instance.CHECK_downloadComplete)
                        {
                            SimulationManager.instance.REG_CurrentObject_Spawned.transform.parent = null;

                        }
                    }

                }
                if (!objectTracker.Start()) // start tracking
                {
                    Debug.LogError("<color=red>Tracker Failed to Start.</color>");
                    Log.Text(SimulationManager.instance.label, "<color=red>Tracker Failed to Start.</color>");
                }

            }
            else
            {
                Log.Text(SimulationManager.instance.label, "<color=red>Failed to load dataset: '" + SimulationManager.instance.REG_INFO_VuforiaDataName_Xml + "'</color>");
                Debug.LogError("<color=red>Failed to load dataset: '" + SimulationManager.instance.REG_INFO_VuforiaDataName_Xml + "'</color>");
            }


        }

        /// <summary>
        /// Call this method to deactivate overlay
        /// </summary>
        public void OnEndVuforia()
        {
            VuforiaARController.Instance.UnregisterVuforiaStartedCallback(LoadDataSet);

            DeactivateActiveDataSets(true);

            //Destroy(SimulationManager.instance.REG_VUFORIA_CurrentTrackable.gameObject);
        }


        void DeactivateActiveDataSets(bool destroyDataSets = false)
        {
            List<DataSet> activeDataSets = this.objectTracker.GetActiveDataSets().ToList();

            foreach (DataSet ds in activeDataSets)
            {
                // The VuforiaEmulator.xml dataset (used by GroundPlane) is managed by Vuforia.
                if (!ds.Path.Contains("VuforiaEmulator.xml"))
                {
                    Log.Text(SimulationManager.instance.label, "Overlay Deactivated");
                    Debug.Log("Deactivating: " + ds.Path);
                    this.objectTracker.DeactivateDataSet(ds);
                    if (destroyDataSets)
                    {
                        //Log.Text(SimulationManager.instance.label, "Overlay Deactivated");
                        //Log.Text(SimulationManager.instance.label, "Destroyed: " + ds.Path);
                        Debug.Log("Destroyed: " + ds.Path);
                        this.objectTracker.DestroyDataSet(ds, true);
                    }
                }
            }
        }
    }
}

