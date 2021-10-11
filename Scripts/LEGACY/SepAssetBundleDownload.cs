using RESTClient;
using Azure.StorageServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BCCH_AzureLoad;

namespace BCCH
{
    public class SepAssetBundleDownload : MonoBehaviour
    {
        [Header("SEP CORE")]
        [SerializeField]
        SepUiManager uiManager;

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
        public string assetBundleName = "cloud";
        public string fileName = "h01.fbx";
        private AssetBundle assetBundle;
        private GameObject loadedObject;

        private string localPath;

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

        

        void OnDestroy()
        {
            UnloadAssetBundle();
        }

        private void UnloadAssetBundle()
        {
            if (assetBundle != null)
            {
                RemovePrefabs();
                loadedObject = null;
                assetBundle.Unload(true);
                Debug.Log("Unloaded Asset Bundle");
            }
        }


        public void TappedLoadAssetBundle()
        {
            UnloadAssetBundle();
            string filename = assetBundleName + "-" + GetAssetBundlePlatformName() + ".unity3d";
            string resourcePath = container + "/" + filename;
            Log.Text(label, "Load asset bundle: " + resourcePath);
            StartCoroutine(blobService.GetAssetBundle(GetAssetBundleComplete, resourcePath));
        }

        public void TappedLoadByte()
        {
            UnloadAssetBundle();
            string filename = fileName;
            string resourcePath = container + "/" + filename;
            Log.Text(label, "Load Asset Byte: " + resourcePath);

        }


        private void GetAssetBundleComplete(IRestResponse<AssetBundle> response)
        {
            if (response.IsError)
            {
                Log.Text(label, "Failed to load asset bunlde: " + response.StatusCode, response.ErrorMessage, Log.Level.Error);
            }
            else
            {
                Log.Text(label, "Loaded Asset Bundle:" + response.Url);
                assetBundle = response.Data;
                StartCoroutine(LoadAssets(assetBundle, "CloudCube")); ;
            }
        }

        private IEnumerator LoadAssets(AssetBundle bundle, string name)
        {
            // Load the object asynchronously
            AssetBundleRequest request = bundle.LoadAssetAsync(name, typeof(GameObject));

            // Wait for completion
            yield return request;

            // Get the reference to the loaded object
            loadedObject = request.asset as GameObject;
            loadedObject.tag = "Player";

            //AddPrefab(new Vector3(0, 4, 0));
            AddPrefab(ANCHOR_prefab.position);
            Log.Text(label, "+ Prefab" + loadedObject.name, "Added prefab name: " + loadedObject.name);
        }

        private void AddPrefab(Vector3 position = default(Vector3))
        {
            AddPrefab(position, Quaternion.identity, Vector3.one, Color.clear);
        }

        private void AddPrefab(Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            if (assetBundle == null || loadedObject == null)
            {
                Log.Text(label, "Load asset bundle first", "Error, Asset Bundle was null", Log.Level.Warning);
                return;
            }
            GameObject gameObject = Instantiate(loadedObject, position, rotation);
            uiManager.directionalIndicator.DirectionalTarget = gameObject.transform;
            gameObject.transform.localScale = scale;
            gameObject.transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
        }

        public void TappedRemovePrefabs()
        {
            if (assetBundle == null)
            {
                Log.Text(label, "Tap 'Load Asset Bundle' first", "No asset bundles loaded", Log.Level.Warning);
                return;
            }
            RemovePrefabs();
            Log.Text(label, "- Remove Prefabs", "Remove Prefabs");
        }


        private void RemovePrefabs()
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject obj in objs)
            {
                Destroy(obj);
            }
        }

        private string GetAssetBundlePlatformName()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return SystemInfo.operatingSystem.Contains("64 bit") ? "x64" : "x86";
                case RuntimePlatform.WSAPlayerX86:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.WSAPlayerARM:
                    return "WSA";
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                default:
                    throw new Exception("Platform not listed");
            }
        }
    }
}

