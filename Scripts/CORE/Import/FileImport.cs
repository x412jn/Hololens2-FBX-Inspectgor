using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RESTClient;
using Azure.StorageServices;

namespace BCCH
{
    public class FileImport : MonoBehaviour
    {
        #region CORE ATTACHMENT


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



        public void FileCheck_FBX()
        {
            for (int i = 0; i < fileNameList.Count; i++)
            {
                if (SimulationManager.instance.textProfilier.TextProfilier(fileNameList[i], "fbx"))
                {
                    Debug.Log("File name check complete at: " + fileNameList[i]);
                    fbxExist = true;
                    downloadInfo_FileName = fileNameList[i];
                    i = fileNameList.Count;
                }
            }
            if (!fbxExist)
            {
                UiManager.instance.OnDeleteButton_Import(this.gameObject);
            }
        }

        public void FileCheck_Dat()
        {
            for (int i = 0; i < fileNameList.Count; i++)
            {
                if (SimulationManager.instance.textProfilier.TextProfilier(fileNameList[i], "dat"))
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
                if (SimulationManager.instance.textProfilier.TextProfilier(fileNameList[i], "xml"))
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
                switch (SimulationManager.instance.textProfilier.GetFormat(fileNameList[i]))
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
            //Log.Text(SimulationManager.instance.label, "Listing blobs");
            Debug.Log("Listing blobs");
            StartCoroutine(blobService.ListBlobs(ListBlobsCompleted, downloadInfo_Container));
        }

        private void ListBlobsCompleted(IRestResponse<BlobResults> response)
        {
            if (response.IsError)
            {
                Log.Text(SimulationManager.instance.label, "Failed to get list of blobs", "List blob error: " + response.ErrorMessage, Log.Level.Error);
                return;
            }


            Debug.Log("Loaded blobs: " + response.Data.Blobs.Length);
            ReloadTable(response.Data.Blobs);
        }

        private void ReloadTable(Blob[] blobs)
        {

            for (int i = 0; i < blobs.Length; i++)
            {
                FileCheck_FBX();
                FileCheck_Dat();
                FileCheck_Xml();
                FileCheck_MultiMedia();
            }
        }
        public void OnTappedLoadFile()
        {
            if (SimulationManager.instance.CHECK_startDownloading && !SimulationManager.instance.CHECK_downloadComplete)
            {
                Log.Text(SimulationManager.instance.label, "On Pending Last Request");
                Debug.Log("On Pending Last Request");
                return;
            }
            if (SimulationManager.instance.CHECK_startDownloading_dat || SimulationManager.instance.CHECK_startDownloading_xml)
            {
                Log.Text(SimulationManager.instance.label, "On Pending Last Request");
                Debug.Log("On Pending Last Request");
                return;
            }
            if (fbxExist)
            {
                //Initiate REG object
                SimulationManager.instance.REMOVE_CurrentObject();


                SimulationManager.instance.byteLoader.fileName = downloadInfo_FileName;
                SimulationManager.instance.byteLoader.container = downloadInfo_Container;
                SimulationManager.instance.REG_INFO_CurrentFileName_Fbx = downloadInfo_FileName;
                SimulationManager.instance.REG_INFO_CurrentContainer = downloadInfo_Container;
                SimulationManager.instance.REG_INFO_IsVuforiaExist = vuforiaDataseExist;
                SimulationManager.instance.REG_INFO_VuforiaDataName_Dat = downloadInfo_VuforiaName_Dat;
                SimulationManager.instance.REG_INFO_VuforiaDataName_Xml = downloadInfo_VuforiaName_Xml;
                SimulationManager.instance.REG_INFO_CurrentVuforiaDataset = SimulationManager.instance.textProfilier.GetNameWithoutFormat(downloadInfo_VuforiaName_Xml);



                for (int i = 0; i < fileNameList_MultiMedia.Count; i++)
                {
                    SimulationManager.instance.REG_List_CurrentFileList_MultiMedia.Add(fileNameList_MultiMedia[i]);
                }
                //SimulationManager.instance.REG_List_CurrentFileList_MultiMedia = fileNameList_MultiMedia;
                SimulationManager.instance.byteLoader.TappedLoadByte();
                if (vuforiaDataseExist)
                {
                    SimulationManager.instance.byteLoader.PreDownloadVuforiaDataset(downloadInfo_VuforiaName_Dat, downloadInfo_VuforiaName_Xml);
                }
                UiManager.instance.OnDisableMenu_All();
            }
            else
            {
                Log.Text(SimulationManager.instance.label, "NO FBX FOUND");
            }

        }

        #endregion
    }
}
