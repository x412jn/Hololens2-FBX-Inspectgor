#pragma warning disable 649
using TriLibCore;
using TriLibCore.General;
using TriLibCore.Mappers;
using RESTClient;
using Azure.StorageServices;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace BCCH
{
    public class SepTrilibByteLoader : MonoBehaviour
    {

        [Header("SEP CORE")]
        [SerializeField]
        SepUiManager uiManager;

        [SerializeField]
        SimulationManager simulationManager;

        [SerializeField]
        Transform ANCHOR_prefab;


        //[Space]
        //[Header("Azure Storage Service")]
        //[SerializeField]
        //private string storageAccount;
        //[SerializeField]
        //private string accessKey;
        //[SerializeField]
        [HideInInspector]
        public string container;


        //public Text label;
        [HideInInspector]
        public string fileName;

        private string fileName_Dat;

        private string fileName_Xml;

        private byte[] byteFile;
        private GameObject loadedObject;

        [Space]
        [Header("TriLib")]
        /// <summary>
        /// The last loaded GameObject.
        /// </summary>
        private GameObject _loadedGameObject;

        /// <summary>
        /// The progress indicator Text;
        /// </summary>
        private Text _progressText;

        private string filePath;

        
        public void PreDownloadVuforiaDataset(string downloadInfo_datName,string downloadInfo_xmlName)
        {
            if (!simulationManager.CHECK_startDownloading_dat && !simulationManager.CHECK_startDownloading_xml)
            {
                string resourcePath_Dat = container + "/" + downloadInfo_datName;
                Debug.Log("Load dat: " + resourcePath_Dat);

                string resourcePath_Xml = container + "/" + downloadInfo_xmlName;
                Debug.Log("Load xml: " + resourcePath_Xml);

                fileName_Dat = downloadInfo_datName;
                fileName_Xml = downloadInfo_xmlName;

                simulationManager.CHECK_startDownloading_dat = true;
                simulationManager.CHECK_startDownloading_xml = true;

                StartCoroutine(simulationManager.blobService.GetBlob(GetByte_Dat, resourcePath_Dat));
                StartCoroutine(simulationManager.blobService.GetBlob(GetByte_Xml, resourcePath_Xml));

            }
            else
            {
                uiManager.OnCallMessage("On Pending Last Request");
            }
        }


        public void TappedLoadByte()
        {
            if (!simulationManager.CHECK_startDownloading)
            {
                string resourcePath = container + "/" + fileName;
                Log.Text(simulationManager.label, "Loading file:" + fileName);
                simulationManager.CHECK_startDownloading = true;
                simulationManager.CHECK_downloadComplete = false;
                if (simulationManager.REG_CurrentObject != null)
                {
                    Destroy(simulationManager.REG_CurrentObject.gameObject);
                    simulationManager.REG_CurrentObject = null;
                }
                StartCoroutine(simulationManager.blobService.GetBlob(GetByteComplete, resourcePath));
            }
            else
            {
                uiManager.OnCallMessage("On Pending Last Request");
            }
        }

        private void GetByte_Dat(IRestResponse<byte[]> response)
        {
            if (response.IsError)
            {
                Log.Text(simulationManager.label, "Failed to load dat: " + response.StatusCode, response.ErrorMessage, Log.Level.Error);
                Debug.LogError("Failed to load dat: " + response.StatusCode);
                simulationManager.CHECK_startDownloading_dat = false;
            }
            else
            {
                Debug.Log("Loaded dat file:" + response.Url);


                //!!!INTERGRATE BYTE DIRECTORY RECORD HERE!!!
                saveData(response.Data, fileName_Dat, container);
                simulationManager.CHECK_startDownloading_dat = false;

                if (!simulationManager.CHECK_startDownloading_dat && !simulationManager.CHECK_startDownloading_xml)
                {
                    Log.Text(simulationManager.label, "Vuforia Dataset Downloaded");
                    if (!simulationManager.CHECK_startDownloading && simulationManager.CHECK_downloadComplete)
                    {
                        uiManager.OnCallMessage("Model loaded \nOverlay ready to use");
                    }
                }
            }
        }

        private void GetByte_Xml(IRestResponse<byte[]> response)
        {
            if (response.IsError)
            {
                Log.Text(simulationManager.label, "Failed to load xml: " + response.StatusCode, response.ErrorMessage, Log.Level.Error);
                simulationManager.CHECK_startDownloading_xml = false;
            }
            else
            {
                Debug.Log("Loaded xml file:" + response.Url);

                //!!!INTERGRATE BYTE DIRECTORY RECORD HERE!!!
                saveData(response.Data, fileName_Xml, container);
                simulationManager.CHECK_startDownloading_xml = false;

                if (!simulationManager.CHECK_startDownloading_dat && !simulationManager.CHECK_startDownloading_xml)
                {
                    Log.Text(simulationManager.label, "Vuforia Dataset Downloaded");
                    if (!simulationManager.CHECK_startDownloading && simulationManager.CHECK_downloadComplete)
                    {
                        uiManager.OnCallMessage("Model loaded \nOverlay ready to use");
                    }
                }
            }
        }


        private void GetByteComplete(IRestResponse<byte[]> response)
        {
            if (response.IsError)
            {
                Log.Text(simulationManager.label, "Failed to load file: " + response.StatusCode, response.ErrorMessage, Log.Level.Error);
                Debug.LogError("Failed to load file: " + response.StatusCode);
                simulationManager.CHECK_startDownloading = false;
            }
            else
            {
                Log.Text(simulationManager.label, "Fbx File Downloaded \nStart Loading to scene");
                Debug.Log("Loaded Fbx File:" + response.Url);
                byteFile = response.Data;

                //!!!INTERGRATE BYTE DIRECTORY RECORD HERE!!!
                saveData(byteFile, fileName, container);


                //!!INTERGRATE TRILIB CODE HERE!!!

                LoadModel();
            }
        }

        #region BYTE DATA SAVING

        public static void saveData(byte[] modelData, string dataFileName, string container)
        {
            string tempPath = Path.Combine(Application.persistentDataPath, container);
            tempPath = Path.Combine(tempPath, dataFileName);
            //Create Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
            }
            //Debug.Log(path);
            try
            {
                File.WriteAllBytes(tempPath, modelData);
                Debug.Log("Saved Data to: " +tempPath.Replace("/", "\\"));
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed To save Data to: " +tempPath.Replace("/", "\\"));
                Debug.LogWarning("Error: " +e.Message);
            }
        }

        #endregion


        #region TRILIB

        public void LoadModel()
        {
            //Spawn object container
            simulationManager.REG_CurrentObject = Instantiate(simulationManager.HOLDER_Prefab, simulationManager.HOLDER_Anchor);
            simulationManager.REG_CurrentSectioning_Plane = simulationManager.REG_CurrentObject.transform.Find("ClippingPlane").gameObject;
            simulationManager.REG_CurrentSectioning_Plane.SetActive(false);
            //simulationManager.REG_ANCHOR_MultiMediaAnchor = simulationManager.REG_CurrentObject.transform.Find("MultiMediaAnchor");

            //assign anchor for object to download
            simulationManager.REG_ANCHOR_SpawnAnchor = simulationManager.REG_CurrentObject.transform.Find("SPAWNED_POS");
            simulationManager.REG_ANCHOR_SpawnParent = simulationManager.REG_CurrentObject.transform;

            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
            var temppath = Path.Combine(Application.persistentDataPath, container);
            filePath = Path.Combine(temppath, fileName);
            Debug.Log("Loading From: " + filePath);
            AssetLoader.LoadModelFromFile(filePath, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, assetLoaderOptions);
            //assetLoaderFilePicker.LoadModelFromFilePickerAsync("Select a Model file", OnLoad, OnMaterialsLoad, OnProgress, OnBeginLoad, OnError, null, assetLoaderOptions);
            _progressText = simulationManager.REG_CurrentObject.transform.Find("Canvas").Find("progress").GetComponent<Text>();
            _progressText.enabled = true;
        }


        /// <summary>
        /// Called when the the Model begins to load.
        /// </summary>
        /// <param name="filesSelected">Indicates if any file has been selected.</param>
        private void OnBeginLoad(bool filesSelected)
        {
            //_loadModelButton.interactable = !filesSelected;
            _progressText.enabled = filesSelected;
        }

        /// <summary>
        /// Called when any error occurs.
        /// </summary>
        /// <param name="obj">The contextualized error, containing the original exception and the context passed to the method where the error was thrown.</param>
        private void OnError(IContextualizedError obj)
        {
            Debug.LogError($"An error occurred while loading your Model: {obj.GetInnerException()}");
            simulationManager.CHECK_startDownloading = false;
            simulationManager.REMOVE_CurrentObject();
        }

        /// <summary>
        /// Called when the Model loading progress changes.
        /// </summary>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        /// <param name="progress">The loading progress.</param>
        private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
        {
            _progressText.text = $"Progress: {progress:P}";
        }

        /// <summary>
        /// Called when the Model (including Textures and Materials) has been fully loaded.
        /// </summary>
        /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
        {
            if (assetLoaderContext.RootGameObject != null)
            {
                Debug.Log("Model fully loaded.");
                //Replace Material and attach to clipping plane
                //ClippingPlane plane = _loadedGameObject.transform.Find("ClippingPlane").GetComponent<ClippingPlane>();
                ClippingPlane plane = simulationManager.REG_CurrentSectioning_Plane.GetComponent<ClippingPlane>();

                Transform[] parentTransform = _loadedGameObject.transform.GetComponentsInChildren<Transform>();
                Color tempColor;
                foreach (Transform child in parentTransform)
                {
                    if (child.gameObject != _loadedGameObject)
                    {
                        simulationManager.REG_List_CurrentObject_Children.Add(child.gameObject);
                        tempColor = child.GetComponent<Renderer>().material.color;
                        child.GetComponent<Renderer>().material = simulationManager.HOLDER_Material_Custom;
                        child.GetComponent<Renderer>().material.color = tempColor;
                        plane.AddRenderer(child.GetComponent<Renderer>());
                        Debug.Log("Loaded Child :" + child.name + "\nColor: " + child.GetComponent<Renderer>().material.color);
                    }
                }
                plane.gameObject.SetActive(false);
                if (simulationManager.REG_INFO_IsVuforiaExist && !simulationManager.CHECK_startDownloading_dat && !simulationManager.CHECK_startDownloading_xml)
                {
                    uiManager.OnCallMessage("Model loaded \nOverlay ready to use");
                }
                Log.Text(simulationManager.label, "The file has been loaded");
            }
            else
            {
                Debug.Log("Model could not be loaded.");
            }
            //_loadModelButton.interactable = true;
            _progressText.enabled = false;
        }

        /// <summary>
        /// Called when the Model Meshes and hierarchy are loaded.
        /// </summary>
        /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
        /// <param name="assetLoaderContext">The context used to load the Model.</param>
        private void OnLoad(AssetLoaderContext assetLoaderContext)
        {
            if (_loadedGameObject != null)
            {
                Destroy(_loadedGameObject);
            }
            _loadedGameObject = assetLoaderContext.RootGameObject;
            if (_loadedGameObject != null)
            {
                Debug.Log("File Loaded");
                simulationManager.REG_CurrentObject_Spawned = _loadedGameObject;
                simulationManager.directionalIndicator.DirectionalTarget = simulationManager.REG_CurrentObject_Spawned.transform;
                _loadedGameObject.transform.parent = simulationManager.REG_ANCHOR_SpawnParent;
                _loadedGameObject.transform.position = simulationManager.REG_ANCHOR_SpawnAnchor.position;
                _loadedGameObject.name = simulationManager.REG_NAME_SpawnedObject;

            }
            simulationManager.CHECK_startDownloading = false;
            simulationManager.CHECK_downloadComplete = true;
        }
    }

    #endregion

}

