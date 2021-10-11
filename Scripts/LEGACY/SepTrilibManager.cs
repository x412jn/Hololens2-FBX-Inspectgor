using UnityEngine.UI;
using UnityEngine;
using TriLibCore;
using TriLibCore.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Azure.StorageServices;
using RESTClient;

namespace BCCH
{
    public class SepTrilibManager : MonoBehaviour
    {
        #region TriLib Loader
        //Lets the user load a new model by clicking a GUI Button
        //private void OnGUI()
        //{
        //    //Display a button to begin the model loading process
        //    if (GUILayout.Button("Load Model from File"))
        //    {
        //        //creates an AssetLoaderOptions instance
        //        //AssetLoaderOption is a class used to configure many aspects of the loading process
        //        //We won't change the default settings this time, so we can use the instance as it is
        //        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();

        //        //Creates the AssetLoaderFilePicker instance
        //        //AssetLoaderFilePicker is a class that allows users to select models from the local file system
        //        var assetLoaderFilePicker = AssetLoaderFilePicker.Create();

        //        //Show the model selection file-picker
        //        assetLoaderFilePicker.LoadModelFromFilePickerAsync("Select a File", OnLoad, OnMaterialsLoad, OnProgress, OnBeginLoad, OnError, null, assetLoaderOptions);
        //    }
        //}

        ////This event is called when the model is about to be loaded.
        ////You can use this event to do some loading preparation, like showing a loading screen in platforms without threading support
        ////this event receives a boolean indicating if any file has been selected on the file-picker dialog.
        //private void OnBeginLoad(bool anyModelSelected)
        //{

        //}

        ////This event is called when the model loading progress changes
        ////You can use this event to update a loading progress-bar, for instance
        ////the "progress" value comes as a normalized float (goes from 0 to 1)
        ////Platforms like UWP and WebGL don't call this method at this moment, since they don't use threads
        //private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
        //{

        //}

        ////This event is called when there is any critical error loading your model
        ////you can use this to show a message to user
        //private void OnError(IContextualizedError contextualizedError)
        //{
        //    Debug.Log("ERROR: TriLib Loading Error");
        //}

        ////this event is called when all model GameObjects and Meshes have been loaded
        ////There may still Materials and textures processing at this stage
        //private void OnLoad(AssetLoaderContext assetLoaderContext)
        //{
        //    //The root loaded GameObject is assigned to the "assetLoaderCOntext.RootGameObject" filed.
        //    //If you want to make sure the GameObject will be visible only when all Materials and
        //    //textures have been loaded, you can disable it at this step
        //    var myLoadedGameObject = assetLoaderContext.RootGameObject;
        //    myLoadedGameObject.SetActive(false);
        //}

        ////This event is called after OnLoad when all Materials and Textures have been loaded
        ////this event is also called after a critical loading erroro, so you can clean up any resource you want to
        //private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
        //{
        //    //the root loaded GameObject is assigned to the "assetLoaderContext.RootGameObject" field.
        //    //You can make the GameObject visible again at this step if you prefer to
        //    var myLoadedGameObject = assetLoaderContext.RootGameObject;
        //    myLoadedGameObject.SetActive(true);
        //    Debug.Log("Load complete");
        //}

        #endregion

        #region get list
        [SerializeField]
        private SepTextProfilier textProfilier;

        [SerializeField]
        private Text text;

        [Space]
        [Header("FileList")]
        [SerializeField]
        string url_fileList;

        [SerializeField]
        string startingWord_FileList;
        [SerializeField]
        string endingWord_FileList;
        [SerializeField]
        char seperatingWord_FileList;

        [Space]
        [Header("Host")]

        [SerializeField]
        string url_host;

        [SerializeField]
        string startingWord_Host;
        [SerializeField]
        string endingWord_Host;

        public void Btn_GetFileList()
        {
            StartCoroutine(GetFileList(url_fileList));
        }

        public void Btn_GetHost()
        {
            StartCoroutine(GetHost(url_host));
        }

        private IEnumerator GetFileList(string url)
        {
            string outPutWord = null;
            string[] outPutWords;
            using(UnityWebRequest unityWebRequest = UnityWebRequest.Get(url))
            {
                yield return unityWebRequest.SendWebRequest();
                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError || unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log("Error: " + unityWebRequest.error);
                }
                else
                {
                    //Debug.Log("Received: " + unityWebRequest.downloadHandler.text);
                    //text.text = unityWebRequest.downloadHandler.text;
                    
                    outPutWords = textProfilier.TextProfilier(
                        unityWebRequest.downloadHandler.text, 
                        startingWord_FileList, 
                        endingWord_FileList, 
                        seperatingWord_FileList);
                    for(int i = 0; i < outPutWords.Length; i++)
                    {
                        outPutWord += (outPutWords[i] + "\n");
                    }
                    text.text = outPutWord;
                }
            }
        }

        private IEnumerator GetHost(string url)
        {
            using(UnityWebRequest unityWebRequest = UnityWebRequest.Get(url))
            {
                yield return unityWebRequest.SendWebRequest();
                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError || unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log("Error: " + unityWebRequest.error);
                }
                else
                {
                    text.text = textProfilier.TextProfilier(
                        unityWebRequest.downloadHandler.text, 
                        startingWord_Host, 
                        endingWord_Host);
                }
            }
        }

        #endregion

        #region UI
        /// <summary>
        /// The progress indicator Text;
        /// </summary>
        [SerializeField]
        private Text _progressText;


        #endregion


        #region Azure Loader

        [Header("Azure Storage Service")]
        [SerializeField]
        private string storageAccount;
        [SerializeField]
        private string accessKey;
        [SerializeField]
        private string container;

        private StorageServiceClient client;
        private BlobService blobService;


        private List<Blob> items;
        public Text label;

        public void ListBlobs()
        {
            Log.Text(label, "Listing blobs");
            StartCoroutine(blobService.ListBlobs(ListBlobsCompleted, container));
        }

        private void ListBlobsCompleted(IRestResponse<BlobResults> response)
        {
            if (response.IsError)
            {
                Log.Text(label, "Failed to get list of blobs", "List blob error: " + response.ErrorMessage, Log.Level.Error);
                return;
            }

            Log.Text(label, "Loaded blobs: " + response.Data.Blobs.Length, "Loaded blobs: " + response.Data.Blobs.Length);
            //ReloadTable(response.Data.Blobs);
        }

        #endregion

        private void Start()
        {
            if (string.IsNullOrEmpty(storageAccount) || string.IsNullOrEmpty(accessKey))
            {
                Log.Text(label, "Storage account and access key are required", "Enter storage account and access key in Unity Editor", Log.Level.Error);
            }

            client = StorageServiceClient.Create(storageAccount, accessKey);
            blobService = client.GetBlobService();

            items = new List<Blob>();
        }

        public void OnRefresh()
        {
            ListBlobs();
        }

    }
}

