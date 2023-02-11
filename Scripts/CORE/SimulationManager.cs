using System.Collections;
using System.Collections.Generic;
using RESTClient;
using Azure.StorageServices;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using TriLibCore.Samples;
using Vuforia;
using WorldSpaceTransitions;
using TMPro;

namespace BCCH
{
    public class SimulationManager : MonoBehaviour
    {
        //Singleton
        public static SimulationManager instance;

        #region CORE ATTACHMENT

        //REGISTER VARIABLES

        /// <summary>
        /// The holder object that spawned on scene
        /// </summary>
        [HideInInspector]
        public GameObject REG_CurrentObject;

        /// <summary>
        /// The exact object that spawned under the holder
        /// </summary>
        [HideInInspector]
        public GameObject REG_CurrentObject_Spawned;

        /// <summary>
        /// The chidren object under current exact object
        /// </summary>
        [HideInInspector]
        public List<GameObject> REG_List_CurrentObject_Children;


        [HideInInspector]
        public GameObject REG_CurrentSectioning_Plane;

        [HideInInspector]
        public Transform REG_ANCHOR_SpawnAnchor;
        [HideInInspector]
        public Transform REG_ANCHOR_SpawnParent;



        //public Transform REG_ANCHOR_VuforiaTrackable;

        [HideInInspector]
        public string REG_NAME_SpawnedObject = "SpawnedObject";

        [HideInInspector]
        public string REG_INFO_CurrentContainer;

        /// <summary>
        /// Current containers from azure
        /// </summary>
        [HideInInspector]
        public List<string> REG_List_CurrentContainerList;

        /// <summary>
        /// Current fbx file name(with ".fbx") under selected container
        /// </summary>
        [HideInInspector]
        public string REG_INFO_CurrentFileName_Fbx;

        /// <summary>
        /// current instance of multimedia holder that spawned on scene
        /// </summary>
        [HideInInspector]
        public List<GameObject> REG_List_CurrentInstance_MultiMedia;

        [HideInInspector]
        public List<string> REG_List_CurrentFileList_MultiMedia;



        //VUFORIA
        [HideInInspector]
        public ModelTargetBehaviour REG_VUFORIA_CurrentTrackable;

        [HideInInspector]
        public string REG_INFO_VuforiaDataName_Dat;

        [HideInInspector]
        public string REG_INFO_VuforiaDataName_Xml;

        [HideInInspector]
        public string REG_INFO_CurrentVuforiaDataset;

        [HideInInspector]
        public bool REG_INFO_IsVuforiaExist = false;

        [HideInInspector]
        public Vector3 REG_CALIBRATION_CameraInitPos = new Vector3();

        //[HideInInspector]
        //public Vector3 REG_CALIBRATION_CameraInitRot =new Vector3();

        [HideInInspector]
        public Vector3 REG_CALIBRATION_CameraInitRot = new Vector3();

        //PROCEEDURAL VARIABLES
        [HideInInspector]
        public bool CHECK_startDownloading = false;

        [HideInInspector]
        public bool CHECK_downloadComplete = false;


        [HideInInspector]
        public bool CHECK_startDownloading_dat = false;

        [HideInInspector]
        public bool CHECK_startDownloading_xml = false;


        [HideInInspector]
        public bool CHECK_startOverlay = false;


        //STATE
        [HideInInspector]
        public bool CHECK_STATE_Sectionning = false;


        [Space]
        [Header("CORE UTILITIES")]
        public DirectionalIndicator directionalIndicator;
        public SepTextProfilier textProfilier;
        public SepTrilibByteLoader byteLoader;
        public SepOverlayManager overlayManager;


        #endregion //CORE ATTACHMENT


        #region file import

        #region AZURE

        [Space]
        [Header("AZURE")]
        public string storageAccount;
        public string accessKey;

        //public string container_model;
        //public string container_vulforia;
        //public string container_side;

        [HideInInspector]
        public StorageServiceClient client;
        [HideInInspector]
        public BlobService blobService;

        private List<Blob> items_Blob;
        private List<Container> items_Container;
        public Text label;

        public Text label_Overlay;

        #endregion //AZURE

        [Space]
        [Header("File Import")]
        public GameObject HOLDER_Prefab;
        public Transform HOLDER_Anchor;

        //Switch Template
        public GameObject HOLDER_Prefab_ImportSwitchTemplate;



        public void ListBlobs()
        {
            Log.Text(label, "Listing blobs");
            StartCoroutine(blobService.ListBlobs(ListBlobsCompleted, REG_INFO_CurrentContainer));
        }


        public void ListContainers()
        {
            Log.Text(label, "Listing containers");
            StartCoroutine(blobService.ListContainers(ListContainersCompleted));
        }

        private void ListContainersCompleted(IRestResponse<ContainerResults> response)
        {
            if (response.IsError)
            {
                Log.Text(label, "List container error: " + response.ErrorMessage, "List container error: " + response.ErrorMessage, Log.Level.Error);
                Debug.LogError("List container error: " + response.ErrorMessage);
                return;
            }
            Log.Text(label, "Loaded containers: " + response.Data.Containers.Length, "Loaded containers: " + response.Data.Containers.Length);
            ReloadTable(response.Data.Containers);
        }


        private void ListBlobsCompleted(IRestResponse<BlobResults> response)
        {
            if (response.IsError)
            {
                Log.Text(label, "List blob error: " + response.ErrorMessage, "List blob error: " + response.ErrorMessage, Log.Level.Error);
                Debug.LogError("List blob error: " + response.ErrorMessage);
                return;
            }

            //Log.Text(label, "Loaded blobs: " + response.Data.Blobs.Length, "Loaded blobs: " + response.Data.Blobs.Length);
            Debug.Log("Loaded blobs: " + response.Data.Blobs.Length);
            ReloadTable(response.Data.Blobs);
        }


        private void ReloadTable(Blob[] blobs)
        {
            items_Blob.Clear();
            REG_List_CurrentFileList_MultiMedia.Clear();
            items_Blob.AddRange(blobs);
            //tableView.ReloadData();

            for (int i = 0; i < items_Blob.Count; i++)
            {
                REG_List_CurrentFileList_MultiMedia.Add(items_Blob[i].Name);
                Debug.Log("On adding blob number: " + i + "\nname: " + REG_List_CurrentFileList_MultiMedia[i]);
            }

        }



        private void ReloadTable(Container[] containers)
        {
            items_Container.Clear();
            REG_List_CurrentContainerList.Clear();
            items_Container.AddRange(containers);

            for (int i = 0; i < items_Container.Count; i++)
            {
                REG_List_CurrentContainerList.Add(items_Container[i].Name);
                Debug.Log("On adding container number: " + i + "\nname: " + REG_List_CurrentContainerList[i]);
            }
        }

        #endregion //File Import

        #region Sectionning
        [Space]
        [Header("Sectionning")]
        public GameObject HOLDER_Prefab_SectionningPanel;

        public Material HOLDER_Material_Custom;

        public GameObject SectionningCapePanel;

        public GameObject SectionningSetup;

        public void Sectionning_OnProcess(bool check)
        {
            if (CHECK_downloadComplete)
            {
                if (REG_CurrentSectioning_Plane != null)
                {
                    if (check)
                    {
                        Sectionning_Enable();
                    }
                    else
                    {
                        Sectionning_Disable();
                    }
                }
                else
                {
                    Log.Text(label, "Sectionning Plane Not Found");
                    CHECK_STATE_Sectionning = false;
                }
            }
            else
            {
                Log.Text(label, "Regirstered Object Not Found");
                CHECK_STATE_Sectionning = false;
            }
        }

        public void Sectionning_Enable()
        {
            REG_CurrentSectioning_Plane.SetActive(true);
            if (REG_CurrentObject_Spawned != null)
            {
                SectionningCapePanel.GetComponent<SectionPlaneCapped>().toBeSectioned = REG_CurrentObject_Spawned;
                SectionningSetup.GetComponent<SectionSetup>().model = REG_CurrentObject_Spawned;
                SectionningSetup.SetActive(true);
                SectionningCapePanel.SetActive(true);
            }
            //Log.Text(label, "Sectionning Enabled");

        }

        public void Sectionning_Disable()
        {
            SectionningCapePanel.GetComponent<SectionPlaneCapped>().toBeSectioned = null;
            SectionningSetup.GetComponent<SectionSetup>().model = null;
            SectionningSetup.SetActive(false);
            SectionningCapePanel.SetActive(false);
            if (REG_CurrentSectioning_Plane != null)
            {
                REG_CurrentSectioning_Plane.SetActive(false);
                CHECK_STATE_Sectionning = false;
            }
            //Log.Text(label, "Sectionning Disabled");
        }

        #endregion

        #region Overlay
        [Space]
        [Header("Overlay")]
        public GameObject HOLDER_OverlayCalibration;

        //public void Overlay_OnProcess()
        //{
        //    if (CHECK_startOverlay)
        //    {
        //        Log.Text(label, "start overlay");
        //        overlayManager.OnOverlayEnable(REG_INFO_VuforiaDataName_Xml);
        //    }
        //    else
        //    {
        //        Log.Text(label, "Stop overlay");
        //        Overlay_OnLostTracked();
        //    }
        //}


        public void Overlay_OnTracked()
        {


            //if(REG_CurrentObject_Spawned != null && CHECK_downloadComplete)
            //{
            //    //REG_CurrentObject_Spawned.transform.parent = REG_ANCHOR_VuforiaTrackable;
            //    REG_CurrentObject_Spawned.transform.parent = REG_VUFORIA_CurrentTrackable.transform;
            //    REG_CurrentObject_Spawned.transform.localPosition = Vector3.zero;
            //    //REG_CurrentObject_Spawned.transform.rotation = REG_ANCHOR_VuforiaTrackable.rotation;
            //    REG_CurrentObject_Spawned.transform.localRotation = Quaternion.identity;
            //    REG_CurrentObject_Spawned.transform.localScale = Vector3.one;
            //}
            label_Overlay.text = "Tracked";
        }

        public void Overlay_OnLostTracked()
        {
            //if(REG_CurrentObject_Spawned != null && CHECK_downloadComplete)
            //{
            //    REG_CurrentObject_Spawned.transform.parent = REG_ANCHOR_SpawnParent;
            //    REG_CurrentObject_Spawned.transform.transform.localScale = Vector3.one;
            //    REG_CurrentObject_Spawned.transform.localPosition = Vector3.zero;
            //    REG_CurrentObject_Spawned.transform.localRotation = Quaternion.identity;
            //}
            label_Overlay.text = "Gaze at the physical model";
        }

        public void Overlay_OnEnd()
        {
            overlayManager.OnEndVuforia();
            REG_VUFORIA_CurrentTrackable = null;
            REG_CurrentObject_Spawned.transform.parent = REG_ANCHOR_SpawnParent;
            REG_CurrentObject_Spawned.transform.transform.localScale = Vector3.one;
            REG_CurrentObject_Spawned.transform.localPosition = Vector3.zero;
            REG_CurrentObject_Spawned.transform.localRotation = Quaternion.identity;
            //UiManager.instance.overlayStatusDisplay.text = "Off";
            label_Overlay.text = " ";
        }

        #endregion

        #region TOGGLING
        [Space]
        [Header("Toggling prefab")]
        public GameObject HOLDER_Prefab_ToggleSwitchTemplate;


        #endregion

        #region Multi Media
        //Switch Template

        [Space]
        [Header("Multi Media")]
        public Transform REG_ANCHOR_MultiMediaAnchor;

        public GameObject HOLDER_Prefab_MultiMediaSwitchTemplate;

        public GameObject HOLDER_Prefab_MultiMediaPanel;


        #endregion //Multi Media

        private void Awake()
        {
            //SINGLETON
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
            REG_CALIBRATION_CameraInitPos = Vector3.zero;
        }

        private void Start()
        {
            //initiate tutorial phrase
            label_Overlay.text = "Gaze at your left palm";

            //instantiate lists related to core functions
            REG_List_CurrentContainerList = new List<string>();
            REG_List_CurrentObject_Children = new List<GameObject>();

            REG_List_CurrentFileList_MultiMedia = new List<string>();
            REG_List_CurrentInstance_MultiMedia = new List<GameObject>();

            //detect azure information
            if (string.IsNullOrEmpty(storageAccount) || string.IsNullOrEmpty(accessKey))
            {
                Log.Text(label, "Storage account and access key are required", "Enter storage account and access key in Unity Editor", Log.Level.Error);
            }

            //create azure service
            client = StorageServiceClient.Create(storageAccount, accessKey);
            blobService = client.GetBlobService();

            //Initiate lists related to azure that allows codes to utilize
            items_Blob = new List<Blob>();
            items_Container = new List<Container>();




            //Detect container list and store it into the list
            ListContainers();

        }


        public void REMOVE_CurrentObject()
        {
            //Disable Sectionning
            Sectionning_Disable();


            //deregister current sectionning pannel
            REG_CurrentSectioning_Plane = null;


            //Disable overlay and remove current vuforia tracker
            if (REG_VUFORIA_CurrentTrackable != null)
            {
                Overlay_OnEnd();
            }


            //Instantiate Toggle Buttons
            UiManager.instance.OnInitiateButtons_Toggle();

            //Instantiate multimedia buttons
            UiManager.instance.OnInstantiateButtons_MultiMedia();

            //Instantiate Spawned Object Information
            Debug.Log("ON CLEARING LISTS");
            if (REG_List_CurrentObject_Children.Count > 0)
            {
                REG_List_CurrentObject_Children.Clear();
            }
            if (REG_CurrentObject_Spawned != null)
            {
                Destroy(REG_CurrentObject_Spawned);
                REG_CurrentObject_Spawned = null;
            }
            if (REG_CurrentObject != null)
            {
                Destroy(REG_CurrentObject);
                REG_CurrentObject = null;
            }

            //Instantiate Multimedia Instance
            if (REG_List_CurrentFileList_MultiMedia.Count > 0)
            {
                REG_List_CurrentFileList_MultiMedia.Clear();
            }

            if (REG_List_CurrentInstance_MultiMedia.Count > 0)
            {
                Debug.Log("ON DESTROY MULTI MEDIA INSTANCE");
                for (int i = 0; i < REG_List_CurrentInstance_MultiMedia.Count; i++)
                {
                    if (REG_List_CurrentInstance_MultiMedia[i] != null)
                    {
                        Destroy(REG_List_CurrentInstance_MultiMedia[i].gameObject);
                    }
                }
                REG_List_CurrentInstance_MultiMedia.Clear();
            }
        }



        #region LEGACY

        //FILE IMPORT

        //[HideInInspector]
        //public string REG_INFO_Host;

        //[HideInInspector]
        //public bool CHECK_HostDownloaded = false;

        //[HideInInspector]
        //public bool CHECK_ListDownloaded = false;


        //downloading information
        //string url_Host = "https://bcchdigitallabdev.blogspot.com/2021/06/host.html";
        //string url_Host = "http://bcch.atwebpages.com/";
        //string keyword_host_start = "[Dev Only Start]";
        //string keyword_host_end = "[Dev Only End]";
        //bool state_startDownloadHost = false;


        //string keyword_file_start = "[start]";
        //string keyword_file_end = "[end]";
        //static string keyword_file_seperate_string = "|";
        //char keyword_file_seperate = keyword_file_seperate_string.ToCharArray()[0];
        //bool state_startDownloadFileList = false;

        //public string GetFileIndexUrl()
        //{
        //    Debug.Log("DOWNLOAD FROM: " + REG_INFO_Host + "/download/models/list.txt");
        //    return REG_INFO_Host + "/download/models/list.html";

        //}

        //public IEnumerator GetFileList()
        //{
        //    state_startDownloadFileList = true;
        //    string[] outPutWords;
        //    using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(GetFileIndexUrl()))
        //    {

        //        yield return unityWebRequest.SendWebRequest();
        //        if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError || unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
        //        {
        //            Debug.Log("Error: " + unityWebRequest.error);
        //            UiManager.instance.OnCallMessage("List: " + unityWebRequest.error + "/n url: " + GetFileIndexUrl());
        //            state_startDownloadFileList = false;
        //        }
        //        else
        //        {
        //            REG_List_CurrentFileList_MultiMedia.Clear();
        //            //Debug.Log("Received: " + unityWebRequest.downloadHandler.text);
        //            //text.text = unityWebRequest.downloadHandler.text;

        //            outPutWords = textProfilier.TextProfilier(
        //                unityWebRequest.downloadHandler.text,
        //                keyword_file_start,
        //                keyword_file_end,
        //                keyword_file_seperate);
        //            for (int i = 0; i < outPutWords.Length; i++)
        //            {
        //                //outPutWord += (outPutWords[i] + "\n");
        //                REG_List_CurrentFileList_MultiMedia.Add(outPutWords[i]);
        //            }
        //            CHECK_ListDownloaded = true;
        //        }
        //    }
        //}


        //public IEnumerator GetHost()
        //{
        //    state_startDownloadHost = true;
        //    using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(url_Host))
        //    {

        //        yield return unityWebRequest.SendWebRequest();
        //        if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError || unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
        //        {
        //            Debug.Log("Error: " + unityWebRequest.error);
        //            UiManager.instance.OnCallMessage("Host: " + unityWebRequest.error);
        //            state_startDownloadHost = false;
        //        }
        //        else
        //        {
        //            REG_INFO_Host = unityWebRequest.downloadHandler.text;
        //            //REG_INFO_Host = textProfilier.TextProfilier(
        //            //    unityWebRequest.downloadHandler.text,
        //            //    keyword_host_start,
        //            //    keyword_host_end);
        //            CHECK_HostDownloaded = true;
        //            Debug.Log(REG_INFO_Host);
        //        }
        //    }
        //}

        //FILE IMPORT



        //Transform switchField_Toggle;
        //Transform rightHandMenu;
        //GridObjectCollection switchGridCollection;

        //void thisIsStart()
        //{
        //rightHandMenu = GameObject.FindGameObjectWithTag("RightHandMenu").transform;
        //switchField_Toggle = rightHandMenu.transform.Find("RightMenuContent").Find("SwitchCollection");
        //sort_Toggle = switchField_Toggle.GetComponent<GridObjectCollection>();


        //Assign FBX Child
        //fbxSubChildren = new List<GameObject>();
        //fbxSubChildrenName = new List<string>();

        //FbxChildAssign();
        //}


        //Model loader
        //[SerializeField]
        //GameObject fbxTarget;
        //List<GameObject> fbxSubChildren;
        //List<string> fbxSubChildrenName;



        //Model assigner
        //void FbxChildAssign()
        //{

        //    int n = 0;
        //    Transform[] parentTransform = fbxTarget.transform.GetComponentsInChildren<Transform>();
        //    foreach (Transform child in parentTransform)
        //    {
        //        if (child.gameObject != fbxTarget)
        //        {
        //            fbxSubChildren.Add(child.gameObject);
        //            fbxSubChildrenName.Add(child.name);
        //            Debug.Log("Index:" + n + "|Name:" + fbxSubChildrenName[n]);
        //            n++;
        //        }
        //    }
        //}

        //switch generator

        //bool toggleSpawned = false;

        //public void SpawnToggle()
        //{
        //    if (!toggleSpawned)
        //    {
        //        toggleSpawned = true;
        //        for (int i = 0; i < fbxSubChildren.Count; i++)
        //        {
        //            GameObject tempObject = Instantiate(HOLDER_Prefab_ToggleSwitchTemplate, switchField_Toggle);
        //            SepToggle toggle = tempObject.GetComponent<SepToggle>();
        //            toggle.toggleObject = fbxSubChildren[i];

        //            TextMeshPro toggleName = tempObject.transform.Find("IconAndText").Find("TextMeshPro").GetComponent<TextMeshPro>();
        //            toggleName.text = fbxSubChildrenName[i];
        //        }
        //        sort_Toggle.UpdateCollection();
        //    }

        //}
        #endregion


    }

}
