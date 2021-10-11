using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RESTClient;
using Azure.StorageServices;

namespace BCCH
{
    public class SepFileImport : MonoBehaviour
    {
        #region CORE ATTACHMENT

        [HideInInspector]
        public SimulationManager simulationManager;
        [HideInInspector]
        public SepUiManager uiManager;

        [HideInInspector]
        public List<string> fileNameList;

        

        public BlobService blobService;

        #endregion

        #region CORE VARIABLES

        //FROM SPAWN

        [HideInInspector]
        public bool fbxExist = false;

        [HideInInspector]
        public int id;

        [HideInInspector]
        public string downloadInfo_FileName;

        [HideInInspector]
        public string downloadInfo_Container;

        [HideInInspector]
        public bool vuforiaDataseExist = false;

        [HideInInspector]
        public string downloadInfo_VuforiaName_Dat;

        [HideInInspector]
        public string downloadInfo_VuforiaName_Xml;

        [HideInInspector]
        public List<string> fileNameList_MultiMedia;

        #endregion

        #region CORE METHODS

        private void Start()
        {
            simulationManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationManager>();
            //fileNameList = new List<string>();

            //fileNameList_MultiMedia = new List<string>();
            //blobService = simulationManager.client.GetBlobService();
        }


        public void FileCheck_FBX()
        {
            for (int i = 0; i < fileNameList.Count; i++)
            {
                if (simulationManager.textProfilier.TextProfilier(fileNameList[i], "fbx"))
                {
                    Debug.Log("File name check complete at: " + fileNameList[i]);
                    fbxExist = true;
                    downloadInfo_FileName = fileNameList[i];
                    i = fileNameList.Count;
                }
            }
            if (!fbxExist)
            {
                uiManager.OnDeleteButton_Import(this.gameObject);
            }
        }

        public void FileCheck_Dat()
        {
            for(int i = 0; i < fileNameList.Count; i++)
            {
                if (simulationManager.textProfilier.TextProfilier(fileNameList[i], "dat"))
                {
                    if (downloadInfo_VuforiaName_Xml != null)
                    {
                        vuforiaDataseExist = true;
                    }
                    downloadInfo_VuforiaName_Dat = fileNameList[i];
                    i = fileNameList.Count;
                }
            }
        }

        public void FileCheck_Xml()
        {
            for (int i = 0; i < fileNameList.Count; i++)
            {
                if (simulationManager.textProfilier.TextProfilier(fileNameList[i], "xml"))
                {
                    if (downloadInfo_VuforiaName_Dat != null)
                    {
                        vuforiaDataseExist = true;
                    }
                    downloadInfo_VuforiaName_Xml = fileNameList[i];
                    i = fileNameList.Count;
                }
            }
        }

        public void FileCheck_MultiMedia()
        {
            for (int i = 0; i < fileNameList.Count; i++)
            {
                bool check = false;
                switch (simulationManager.textProfilier.GetFormat(fileNameList[i]))
                {
                    case "jpg":
                        check = true;
                        break;

                    case "mp4":
                        check = true;
                        break;

                    case null:
                        check = false;
                        break;

                    default:
                        check = false;
                        break;
                }
                if (check)
                {
                    Debug.Log("Loaded multi media file: " + fileNameList[i]);
                    fileNameList_MultiMedia.Add(fileNameList[i]);
                }
            }
        }

        public void ListBlobs()
        {
            //Log.Text(simulationManager.label, "Listing blobs");
            Debug.Log("Listing blobs");
            StartCoroutine(blobService.ListBlobs(ListBlobsCompleted, downloadInfo_Container));
        }

        private void ListBlobsCompleted(IRestResponse<BlobResults> response)
        {
            if (response.IsError)
            {
                Log.Text(simulationManager.label, "Failed to get list of blobs", "List blob error: " + response.ErrorMessage, Log.Level.Error);
                return;
            }

            
            Debug.Log("Loaded blobs: " + response.Data.Blobs.Length);
            ReloadTable(response.Data.Blobs);
        }

        private void ReloadTable(Blob[] blobs)
        {

            for (int i = 0; i < blobs.Length; i++)
            {
                fileNameList.Add(blobs[i].Name);
                Debug.Log("On adding blob number: " + i + "\nname: " + fileNameList[i]);
            }
            FileCheck_FBX();
            FileCheck_Dat();
            FileCheck_Xml();
            FileCheck_MultiMedia();
        }

        public void OnTappedLoadFile()
        {
            if (simulationManager.CHECK_startDownloading && !simulationManager.CHECK_downloadComplete)
            {
                Log.Text(simulationManager.label, "On Pending Last Request");
                Debug.Log("On Pending Last Request");
                return;
            }
            if (simulationManager.CHECK_startDownloading_dat || simulationManager.CHECK_startDownloading_xml)
            {
                Log.Text(simulationManager.label, "On Pending Last Request");
                Debug.Log("On Pending Last Request");
                return;
            }
            if (fbxExist)
            {
                //Initiate REG object
                simulationManager.REMOVE_CurrentObject();
                

                simulationManager.byteLoader.fileName = downloadInfo_FileName;
                simulationManager.byteLoader.container = downloadInfo_Container;
                simulationManager.REG_INFO_CurrentFileName_Fbx = downloadInfo_FileName;
                simulationManager.REG_INFO_CurrentContainer = downloadInfo_Container;
                simulationManager.REG_INFO_IsVuforiaExist = vuforiaDataseExist;
                simulationManager.REG_INFO_VuforiaDataName_Dat = downloadInfo_VuforiaName_Dat;
                simulationManager.REG_INFO_VuforiaDataName_Xml = downloadInfo_VuforiaName_Xml;
                simulationManager.REG_INFO_CurrentVuforiaDataset = simulationManager.textProfilier.GetNameWithoutFormat(downloadInfo_VuforiaName_Xml);


                Debug.Log("on tapped file import, multi media count:" + fileNameList_MultiMedia.Count);
                for(int i = 0; i < fileNameList_MultiMedia.Count; i++)
                {
                    simulationManager.REG_List_CurrentFileList_MultiMedia.Add(fileNameList_MultiMedia[i]);
                }
                //simulationManager.REG_List_CurrentFileList_MultiMedia = fileNameList_MultiMedia;
                simulationManager.byteLoader.TappedLoadByte();
                if (vuforiaDataseExist)
                {
                    simulationManager.byteLoader.PreDownloadVuforiaDataset(downloadInfo_VuforiaName_Dat, downloadInfo_VuforiaName_Xml);
                }
                uiManager.OnDisableMenu_All();
            }
            else
            {
                Log.Text(simulationManager.label, "NO FBX FOUND");
            }
            
        }

        #endregion

    }
}

