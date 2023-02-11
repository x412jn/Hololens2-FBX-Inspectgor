using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using TriLibCore.Samples;
using System.ComponentModel;

namespace BCCH
{
    public class UiManager : MonoBehaviour
    {
        //Singleton
        public static UiManager instance;

        #region core attachment
        //[SerializeField]
        //HandColliderTrack handColliderTrack;


        public DirectionalIndicator directionalIndicator;

        [HideInInspector]
        public GameObject loadedObject;

        [HideInInspector]
        public GameObject loadedContainer;

        //public GameObject HOLDER_Prefab;
        //public Transform HOLDER_Anchor;
        [Space]
        [Header("DEBUG MESSAGE UI")]

        [SerializeField]
        GameObject menu_DebugMessage;

        [SerializeField]
        Text text_DebugMessage;

        #endregion



        #region Core Components
        [Space]
        [Header("MENUS")]
        [SerializeField]
        GameObject menu_sectioning;

        [SerializeField]
        GameObject menu_import;

        [SerializeField]
        GameObject menu_multiuser;

        [SerializeField]
        GameObject menu_toggle;

        [SerializeField]
        GameObject menu_modify;

        [SerializeField]
        GameObject menu_multiMedia;

        //[SerializeField]
        //GameObject menu_overlay;


        public void OnEnableMenu_Multiuser()
        {
            OnDisableMenu_All();
            menu_multiuser.SetActive(true);
        }

        public void OnEnableMenu_Import()
        {
            OnDisableMenu_All();
            menu_import.SetActive(true);
        }

        public void OnEnableMenu_Toggle()
        {
            OnDisableMenu_All();
            menu_toggle.SetActive(true);
        }

        public void OnEnableMenu_MultiMedia()
        {
            OnDisableMenu_All();
            menu_multiMedia.SetActive(true);
        }

        //public void OnEnableMenu_Overlay()
        //{
        //    OnDisableMenu_All();
        //    menu_overlay.SetActive(true);
        //}

        public void OnDisableMenu_All()
        {
            menu_import.SetActive(false);
            menu_multiuser.SetActive(false);
            menu_sectioning.SetActive(false);
            menu_DebugMessage.SetActive(false);
            menu_toggle.SetActive(false);
            menu_modify.SetActive(false);
            menu_multiMedia.SetActive(false);
            //menu_overlay.SetActive(false);
        }

        public void OnDisableMenu_All_WithOverlay()
        {
            SimulationManager.instance.Overlay_OnEnd();
            menu_import.SetActive(false);
            menu_multiuser.SetActive(false);
            menu_sectioning.SetActive(false);
            menu_DebugMessage.SetActive(false);
            menu_toggle.SetActive(false);
            menu_modify.SetActive(false);
            menu_multiMedia.SetActive(false);
            //menu_overlay.SetActive(false);
        }

        #endregion //core attachment


        #region universal methods

        private void Start()
        {
            //SINGLETON INIT
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }


            //ATTACHING OBJECTS
            //P.S. Im concerning about wether hand menu would not presence when hands are not detected,
            //Therefore Im using tag finding to binds the object
            //If needeed, you can try change the structure and make it serailize or public
            //rightHandMenu = GameObject.FindGameObjectWithTag("RightHandMenu").transform;
            //switchField_Toggle = rightHandMenu.transform.Find("RightMenuContent").Find("SwitchCollection");
            //sort_Toggle = switchField_Toggle.GetComponent<GridObjectCollection>();


            //INITIATING LISTS AND VARIABLES
            toggleSubChildren = new List<GameObject>();
            toggleSubChildrenName = new List<string>();
            REG_Switches_Toggle = new List<GameObject>();

            REG_Switches_Import = new List<GameObject>();
            REG_Switches_MultiMedia = new List<GameObject>();
        }

        public void OnCallMessage(string message)
        {
            //OnDisableMenu_All();
            menu_DebugMessage.SetActive(true);
            text_DebugMessage.text = message;
        }

        public void OnDeleteButton_Import(GameObject tempObject)
        {
            REG_Switches_Import.Remove(tempObject);
            Destroy(tempObject);
            sort_Import.UpdateCollection();
        }

        public void OnDeleteButton_Toggle(GameObject tempObject)
        {
            REG_Switches_Toggle.Remove(tempObject);
            Destroy(tempObject);
            sort_Toggle.UpdateCollection();
        }

        bool CHECK_tutorial = false;
        public void OnHandMenuTutorial()
        {
            if (!CHECK_tutorial)
            {

                SimulationManager.instance.label_Overlay.text = " ";
                CHECK_tutorial = true;
            }
        }

        #endregion //universal methods

        #region IMPORT METHODS
        [Space]
        [Header("FILE IMPORT")]
        public Transform switchField_Import;

        public GridObjectCollection sort_Import;

        List<GameObject> REG_Switches_Import;


        bool importSpawned = false;

        public void SpawnContainerList()
        {
            SimulationManager.instance.ListContainers();
            GameObject tempObject;
            SepFileImport fileImport;
            TextMeshPro toggleName;


            //Solution: point towards container
            if (SimulationManager.instance.REG_List_CurrentContainerList.Count != 0)
            {

                //Initiate the switch
                if (!importSpawned)
                {
                    importSpawned = true;
                    //StartCoroutine(SimulationManager.instance.SpawnFileList_fbx());
                    for (int i = 0; i < SimulationManager.instance.REG_List_CurrentContainerList.Count; i++)
                    {
                        //Update current temp container
                        //SimulationManager.instance.TEMP_INFO_CurrentContainer = SimulationManager.instance.REG_List_CurrentContainerList[i];


                        tempObject = Instantiate(SimulationManager.instance.HOLDER_Prefab_ImportSwitchTemplate, switchField_Import);
                        REG_Switches_Import.Add(tempObject);
                        fileImport = tempObject.GetComponent<SepFileImport>();
                        fileImport.id = (REG_Switches_Import.Count - 1);
                        fileImport.blobService = SimulationManager.instance.client.GetBlobService();
                        fileImport.fileNameList = new List<string>();
                        fileImport.fileNameList_MultiMedia = new List<string>();
                        fileImport.uiManager = this;
                        fileImport.downloadInfo_Container = SimulationManager.instance.REG_List_CurrentContainerList[i];
                        fileImport.ListBlobs();
                        Debug.Log("Generate file button for: " + fileImport.downloadInfo_Container);

                        toggleName = tempObject.transform.Find("IconAndText").Find("TextMeshPro").GetComponent<TextMeshPro>();
                        toggleName.text = SimulationManager.instance.REG_List_CurrentContainerList[i];
                    }
                    sort_Import.UpdateCollection();


                }
                else
                {
                    Log.Text(SimulationManager.instance.label, "Container list spawned");
                }


            }
            else
            {
                Debug.Log("File List Not Found");
                OnCallMessage("NO FILE LIST FOUND");
            }
        }


        #endregion //IMPORT METHODS

        #region TOGGLING METHODS


        [Space]
        [Header("TOGGLING")]
        [SerializeField]
        Transform switchField_Toggle;
        [SerializeField]
        GridObjectCollection sort_Toggle;

        GameObject toggleTarget;

        //for binding actual game object of spawned object's children
        List<GameObject> toggleSubChildren;

        //for binding name of those children
        List<string> toggleSubChildrenName;

        /*
         * for binding switches that spawned,
         * therefore we can initiate those swithces if we want to load different models
         */

        List<GameObject> REG_Switches_Toggle;

        //for checking wether there are switches been spawned already or not
        bool toggleSpawned = false;

        //call this method when a new model been spawned to assign its children objects to this
        public void ToggleChildAssign()
        {
            //if (toggleSpawned)
            //{
            //    toggleSpawned = false;
            //    toggleSubChildren.Clear();
            //    toggleSubChildrenName.Clear();
            //    for(int i = 0; i < REG_Switches_Toggle.Count; i++)
            //    {
            //        Destroy(REG_Switches_Toggle[i].gameObject);
            //    }
            //    REG_Switches_Toggle.Clear();
            //}

            int n = 0;
            if (SimulationManager.instance.REG_CurrentObject != null)
            {
                toggleTarget = SimulationManager.instance.REG_CurrentObject.transform.Find(SimulationManager.instance.REG_NAME_SpawnedObject).gameObject;
                Transform[] parentTransform = toggleTarget.transform.GetComponentsInChildren<Transform>();
                foreach (Transform child in parentTransform)
                {
                    if (child.gameObject != toggleTarget)
                    {
                        toggleSubChildren.Add(child.gameObject);
                        toggleSubChildrenName.Add(child.name);
                        Debug.Log("Index:" + n + "|Name:" + toggleSubChildrenName[n]);
                        n++;
                    }
                }
            }
            else
            {
                Debug.Log("ERROR:NO REGISTRATED OBJECT");
                OnCallMessage("NO OBJECTS FOUND");
            }
        }

        public void SpawnToggle()
        {
            GameObject tempObject;
            SepToggle toggle;
            TextMeshPro toggleName;
            ToggleChildAssign();

            if (SimulationManager.instance.CHECK_downloadComplete)
            {
                if (!toggleSpawned)
                {
                    toggleSpawned = true;
                    for (int i = 0; i < toggleSubChildren.Count; i++)
                    {
                        tempObject = Instantiate(SimulationManager.instance.HOLDER_Prefab_ToggleSwitchTemplate, switchField_Toggle);
                        REG_Switches_Toggle.Add(tempObject);
                        toggle = tempObject.GetComponent<SepToggle>();
                        toggle.id = (REG_Switches_Toggle.Count - 1);
                        toggle.toggleObject = toggleSubChildren[i];
                        toggleName = tempObject.transform.Find("IconAndText").Find("TextMeshPro").GetComponent<TextMeshPro>();
                        toggleName.text = toggleSubChildrenName[i];
                    }
                    sort_Toggle.UpdateCollection();
                }
            }
            else
            {
                Debug.Log("ERROR:NO REGISTRATED OBJECT");
                OnCallMessage("NO OBJECTS FOUND");
                menu_toggle.SetActive(false);
            }
        }

        public void OnInitiateButtons_Toggle()
        {
            Debug.Log("Start initiating toggles");
            if (toggleSubChildren.Count > 0)
            {
                toggleSubChildren.Clear();
            }
            if (toggleSubChildrenName.Count > 0)
            {
                toggleSubChildrenName.Clear();
            }
            if (REG_Switches_Toggle.Count > 0)
            {
                for (int i = 0; i < REG_Switches_Toggle.Count; i++)
                {
                    Destroy(REG_Switches_Toggle[i].gameObject);
                    Debug.Log("Destroyed Toggle");
                }
                REG_Switches_Toggle.Clear();
                sort_Toggle.UpdateCollection();
            }
            toggleSpawned = false;

        }

        #endregion //TOGGLING METHODS

        #region OVERLAY METHODS

        [Space]
        [Header("OVERLAY")]
        public TextMeshPro overlayStatusDisplay;

        public void OnOverlayProcess()
        {
            if (Vuforia.VuforiaRuntime.Instance.InitializationState != Vuforia.VuforiaRuntime.InitState.INITIALIZED)
            {
                Log.Text(SimulationManager.instance.label, "Vuforia is not calibrated");
                return;
            }
            if (SimulationManager.instance.REG_INFO_IsVuforiaExist && !SimulationManager.instance.CHECK_startDownloading_dat && !SimulationManager.instance.CHECK_startDownloading_xml)
            {
                if (SimulationManager.instance.REG_CurrentObject_Spawned != null && SimulationManager.instance.CHECK_downloadComplete)
                {
                    OnDisableMenu_All();
                    SimulationManager.instance.CHECK_startOverlay = !SimulationManager.instance.CHECK_startOverlay;
                    if (SimulationManager.instance.CHECK_startOverlay)
                    {
                        SimulationManager.instance.Sectionning_Disable();
                        //overlayStatusDisplay.text = "On";
                        Log.Text(SimulationManager.instance.label, "Please gaze at the physical model \nby placing it inside the frame");
                        SimulationManager.instance.overlayManager.OnOverlayEnable(SimulationManager.instance.REG_INFO_VuforiaDataName_Xml);
                        //SimulationManager.instance.overlayManager.OnOverlayEnable(SimulationManager.instance.REG_INFO_VuforiaDataName_Xml);
                    }
                    else
                    {
                        //Log.Text(SimulationManager.instance.label, "Stop overlay", "Stop overlay");
                        //SimulationManager.instance.overlayManager.OnEndVuforia();
                        //SimulationManager.instance.REG_VUFORIA_CurrentTrackable = null;
                        //SimulationManager.instance.REG_CurrentObject_Spawned.transform.parent = SimulationManager.instance.REG_ANCHOR_SpawnParent;
                        //SimulationManager.instance.REG_CurrentObject_Spawned.transform.transform.localScale = Vector3.one;
                        //SimulationManager.instance.REG_CurrentObject_Spawned.transform.localPosition = Vector3.zero;
                        //SimulationManager.instance.REG_CurrentObject_Spawned.transform.localRotation = Quaternion.identity;
                        //overlayStatusDisplay.text = "Off";
                        SimulationManager.instance.Overlay_OnEnd();
                    }
                    //SimulationManager.instance.Overlay_OnProcess();
                }
                else
                {
                    Debug.Log("No object found");
                    Log.Text(SimulationManager.instance.label, "No Object Found");
                }
            }
            else
            {
                Log.Text(SimulationManager.instance.label, "VUFORIA DATA NOT EXIST");
            }
        }

        #endregion //OVERLAY METHODS

        #region MULTI MEDIA

        [Space]
        [Header("MULTI MEDIA")]
        public Transform switchField_MultiMedia;

        public GridObjectCollection sort_MultiMedia;

        List<GameObject> REG_Switches_MultiMedia;

        [HideInInspector]
        public bool REG_CHECK_multiMediaSpawned = false;

        public void SpawnMultiMediaList()
        {

            GameObject tempObject;
            SepMultiMediaImport multiMediaImport;
            TextMeshPro toggleName;


            //Solution: point towards container
            if (SimulationManager.instance.REG_List_CurrentFileList_MultiMedia.Count != 0)
            {

                //Initiate the switch
                if (!REG_CHECK_multiMediaSpawned)
                {
                    REG_CHECK_multiMediaSpawned = true;
                    //StartCoroutine(SimulationManager.instance.SpawnFileList_fbx());
                    for (int i = 0; i < SimulationManager.instance.REG_List_CurrentFileList_MultiMedia.Count; i++)
                    {
                        //Update current temp container
                        //SimulationManager.instance.TEMP_INFO_CurrentContainer = SimulationManager.instance.REG_List_CurrentContainerList[i];


                        tempObject = Instantiate(SimulationManager.instance.HOLDER_Prefab_MultiMediaSwitchTemplate, switchField_MultiMedia);
                        REG_Switches_MultiMedia.Add(tempObject);
                        multiMediaImport = tempObject.GetComponent<SepMultiMediaImport>();
                        multiMediaImport.id = (REG_Switches_MultiMedia.Count - 1);
                        multiMediaImport.blobService = SimulationManager.instance.client.GetBlobService();
                        multiMediaImport.downloadInfo_fileName = SimulationManager.instance.REG_List_CurrentFileList_MultiMedia[i];
                        //fileImport.fileNameList = new List<string>();
                        multiMediaImport.uiManager = this;
                        multiMediaImport.downloadInfo_container = SimulationManager.instance.REG_INFO_CurrentContainer;
                        multiMediaImport.GetPath();
                        Debug.Log("Generate file button for: " + multiMediaImport.downloadInfo_fileName);

                        toggleName = tempObject.transform.Find("IconAndText").Find("TextMeshPro").GetComponent<TextMeshPro>();
                        toggleName.text = SimulationManager.instance.REG_List_CurrentFileList_MultiMedia[i];
                    }
                    sort_MultiMedia.UpdateCollection();


                }
                else
                {
                    Log.Text(SimulationManager.instance.label, "Multi Media List Spawned");
                }


            }
            else
            {
                Debug.Log("Multi Media Not Found");
                OnCallMessage("NO MULTI MEDIA FOUND");
            }
        }

        /// <summary>
        /// delete all current multi media button instance
        /// </summary>
        public void OnInstantiateButtons_MultiMedia()
        {
            if (REG_Switches_MultiMedia.Count > 0)
            {
                for (int i = 0; i < REG_Switches_MultiMedia.Count; i++)
                {
                    if (REG_Switches_MultiMedia[i] != null)
                    {
                        Destroy(REG_Switches_MultiMedia[i].gameObject);
                    }
                }
                REG_Switches_MultiMedia.Clear();

            }
            REG_CHECK_multiMediaSpawned = false;
        }

        #endregion //MULTI MEDIA

        #region SECTIONNING METHODS

        public void OnTapSectionning()
        {
            OnDisableMenu_All_WithOverlay();
            
            SimulationManager.instance.CHECK_STATE_Sectionning = !SimulationManager.instance.CHECK_STATE_Sectionning;
            if (SimulationManager.instance.CHECK_STATE_Sectionning)
            {
                Log.Text(SimulationManager.instance.label, "Sectionning Enabled");
            }
            else
            {
                Log.Text(SimulationManager.instance.label, "Sectionning Disabled");
            }
            SimulationManager.instance.Sectionning_OnProcess(SimulationManager.instance.CHECK_STATE_Sectionning);
        }

        #endregion //SECTIONNING METHODS



    }

}
