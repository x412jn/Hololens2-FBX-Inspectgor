using RESTClient;
using Azure.StorageServices;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BCCH_AzureLoad;

namespace BCCH
{
    public class SepAzureRequestTest : MonoBehaviour
    {
        [Header("SEP CORE")]
        [SerializeField]
        SepUiManager uiManager;

        [SerializeField]
        SimulationManager simulationManager;

        [SerializeField]
        Transform ANCHOR_prefab;
        

        [Space]
        [Header("Azure Storage Service")]
        [SerializeField]
        private string storageAccount;
        [SerializeField]
        private string accessKey;
        [SerializeField]
        private string container;

        private StorageServiceClient client;
        private BlobService blobService;

        [Header("Asset Bundle Demo")]
        public Text label;
        public string fileName = "cloud";
        private AssetBundle assetBundle;
        private GameObject loadedObject;

        // Start is called before the first frame update
        void Start()
        {
            if (string.IsNullOrEmpty(storageAccount) || string.IsNullOrEmpty(accessKey))
            {
                Log.Text(label, "Storage account and access key are required", "Enter storage account and access key in Unity Editor", Log.Level.Error);
                //uiManager.OnCallMessage("Storage account and access key are required");
            }

            client = StorageServiceClient.Create(storageAccount, accessKey);
            blobService = client.GetBlobService();
        }

        public void TappedLoadAssetBundle()
        {
            Debug.Log("Container:" + container+"\nFile name:"+fileName);
            string url = Path.Combine(client.PrimaryEndpoint() + container, fileName);
            Debug.Log("Load asset bundle: " + url);
            //UnloadAssetBundle();
            //string filename = assetBundleName + "-" + GetAssetBundlePlatformName() + ".unity3d";
            //string resourcePath = container + "/" + fileName;
            Log.Text(label, "Load asset bundle: " + url);
            //StartCoroutine(blobService.GetAssetBundle(GetAssetBundleComplete, resourcePath));

            //simulationManager.loadModelFromURL.url = url;
            //Debug.Log("Load asset bundle 2: " + simulationManager.loadModelFromURL.url);
            //Spawn object container
            simulationManager.REG_CurrentObject = Instantiate(simulationManager.HOLDER_Prefab, simulationManager.HOLDER_Anchor); 
            simulationManager.directionalIndicator.DirectionalTarget = simulationManager.REG_CurrentObject.transform;

            //assign anchor for object to download
            simulationManager.REG_ANCHOR_SpawnAnchor = simulationManager.REG_CurrentObject.transform.Find("heartPos");
            simulationManager.REG_ANCHOR_SpawnParent = simulationManager.REG_CurrentObject.transform;

            //simulationManager.loadModelFromURL.StartDownloading();
        }

    }
}

