using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using RESTClient;
using Azure.StorageServices;


namespace BCCH
{
    public class MultiMediaImport : MonoBehaviour
    {
        #region CORE ATTACHMENTS

        public BlobService blobService;

        #endregion //CORE ATTACHMENTS

        #region CORE VARIABLES
        [HideInInspector]
        public int id;
        [HideInInspector]
        public string downloadInfo_fileName;
        [HideInInspector]
        public string downloadInfo_container;

        string pathOfThisFile;

        enum TYPE
        {
            image,
            video,
            nope
        }

        TYPE mediaType;

        Texture2D tex;
        TextMeshPro title;
        Image image;
        VideoPlayer videoPlayer;
        VideoClip videoClip;

        string localPath;
        string containerPath;

        GameObject videoPlane;
        GameObject imagePlane;

        bool startDownloading = false;
        bool downloadComplete = false;
        #endregion //CORE VARIABLES


        #region CORE METHODS

        private void Start()
        {
            mediaType = TYPE.nope;
        }

        public void OnTappedLoadMultiMedia()
        {
            switch (Check_FileType(downloadInfo_fileName))
            {
                case "jpg":
                    mediaType = TYPE.image;
                    break;

                case "mp4":
                    mediaType = TYPE.video;
                    break;

                case null:
                    Debug.LogError("Detected irrational media type");
                    mediaType = TYPE.nope;
                    break;

                default:
                    Debug.LogError("Detected irrational media type");
                    mediaType = TYPE.nope;
                    break;
            }
            switch (mediaType)
            {
                case TYPE.image:
                case TYPE.video:
                    GameObject tempObject = Instantiate(SimulationManager.instance.HOLDER_Prefab_MultiMediaPanel, SimulationManager.instance.REG_ANCHOR_MultiMediaAnchor);
                    SimulationManager.instance.REG_List_CurrentInstance_MultiMedia.Add(tempObject);
                    title = tempObject.transform.Find("TitleBar").Find("Title").GetComponent<TextMeshPro>();
                    imagePlane = tempObject.transform.Find("ImagePlane").gameObject;
                    videoPlane = tempObject.transform.Find("VideoPlane").gameObject;
                    videoPlayer = tempObject.GetComponent<VideoPlayer>();
                    image = tempObject.transform.
                        Find("ImagePlane").
                        Find("UGUIScrollViewContent").
                        Find("Scroll View").
                        Find("Viewport").
                        Find("Content").
                        Find("GridLayout1").
                        Find("Column1").
                        Find("Image").GetComponent<Image>();
                    title.text = downloadInfo_fileName;
                    if (mediaType == TYPE.image)
                    {
                        TappedLoadImageMedia();
                    }
                    if (mediaType == TYPE.video)
                    {
                        TappedLoadVideoMedia();
                    }

                    break;

                case TYPE.nope:
                    return;

                default:
                    return;
            }

        }

        public void TappedLoadVideoMedia()
        {
            if (GetFileExist() && downloadComplete)
            {
                videoPlane.SetActive(true);
                videoPlayer.url = pathOfThisFile;
                return;
            }
            if (startDownloading)
            {
                Log.Text(SimulationManager.instance.label, "On Pending Last Request");
                Debug.Log("On Pending Last Request");
                return;
            }
            else
            {
                startDownloading = true;
            }
            string resourcePath = downloadInfo_container + "/" + downloadInfo_fileName;
            Log.Text(SimulationManager.instance.label, "Load: " + resourcePath);
            Debug.Log("Load: " + resourcePath);
            StartCoroutine(blobService.GetBlob(GetByte_Video, resourcePath));
        }


        public void TappedLoadImageMedia()
        {
            ChangeImage(new Texture2D(1, 1));
            if (startDownloading && !downloadComplete)
            {
                Log.Text(SimulationManager.instance.label, "On Pending Last Request");
                Debug.Log("On Pending Last Request");
                return;
            }
            else if (!startDownloading && !downloadComplete)
            {
                startDownloading = true;
            }
            else if (!startDownloading && downloadComplete)
            {
                startDownloading = true;
                downloadComplete = false;
            }
            string resourcePath = downloadInfo_container + "/" + downloadInfo_fileName;
            Log.Text(SimulationManager.instance.label, "Load: " + resourcePath);
            Debug.Log("Load: " + resourcePath);
            StartCoroutine(blobService.GetImageBlob(GetImageBlobComplete, resourcePath));
        }

        private void GetByte_Video(IRestResponse<byte[]> response)
        {
            if (response.IsError)
            {
                Log.Text(SimulationManager.instance.label, "Failed to load dat: " + response.StatusCode, response.ErrorMessage, Log.Level.Error);
                startDownloading = false;
                downloadComplete = false;
            }
            else
            {
                Log.Text(SimulationManager.instance.label, "Loaded video file:" + downloadInfo_fileName);


                //!!!INTERGRATE BYTE DIRECTORY RECORD HERE!!!
                saveData(response.Data, downloadInfo_fileName, downloadInfo_container);
                videoPlane.SetActive(true);
                videoPlayer.url = pathOfThisFile;
                startDownloading = false;
                downloadComplete = true;
                //downloadComplete = true;
                //startDownloading = false;
            }
        }


        private void GetImageBlobComplete(IRestResponse<Texture> response)
        {
            if (response.IsError)
            {
                Log.Text(SimulationManager.instance.label, "Failed to load image: " + response.StatusCode, response.ErrorMessage, Log.Level.Error);
                Debug.LogError("Failed to load image: " + response.StatusCode);
                startDownloading = false;
                downloadComplete = false;
            }
            else
            {
                Log.Text(SimulationManager.instance.label, "Loaded image:" + downloadInfo_fileName);
                Debug.Log("Loaded image:" + response.Url);
                imagePlane.SetActive(true);
                //saveData(response.Data, downloadInfo_fileName, downloadInfo_container);
                Texture texture = response.Data;
                ChangeImage(texture);
                startDownloading = false;
                downloadComplete = true;

            }
        }

        #endregion //CORE METHODS

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
                Debug.Log("Saved Data to: " + tempPath.Replace("/", "\\"));
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed To save Data to: " + tempPath.Replace("/", "\\"));
                Debug.LogWarning("Error: " + e.Message);
            }
        }

        #endregion

        #region UTILITIES



        private bool GetFileExist()
        {
            return File.Exists(pathOfThisFile);
        }

        public void GetPath()
        {
            string tempPath = Path.Combine(Application.persistentDataPath, downloadInfo_container);
            tempPath = Path.Combine(tempPath, downloadInfo_fileName);
            pathOfThisFile = tempPath;
        }

        private string Check_FileType(string input)
        {
            return SimulationManager.instance.textProfilier.GetFormat(input);
        }


        //IMAGE
        private void DisplayImage()
        {
            byte[] imageBytes = File.ReadAllBytes(localPath);
            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            texture.LoadImage(imageBytes);
            ChangeImage(texture);
        }

        private void ChangeImage(Texture2D texture)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            image.GetComponent<Image>().sprite = sprite;
        }

        private void ChangeImage(Texture texture)
        {
            ChangeImage(texture as Texture2D);
        }

        private bool IsFileReady()
        {
            if (string.IsNullOrEmpty(localPath) && !File.Exists(localPath))
            {
                Log.Text(SimulationManager.instance.label, "Tap 'Capture screenshot' button", "Capture screenshot first", Log.Level.Warning);
                Debug.Log("Tap 'Capture screenshot' button");
                return false;
            }
            return true;
        }

        public void GetPath_Container()
        {
            containerPath = Path.Combine(Application.persistentDataPath, downloadInfo_container);

        }

        public void GetPath_File()
        {
            localPath = Path.Combine(containerPath, downloadInfo_fileName);
        }

        #endregion //UTILITIES
    }
}

