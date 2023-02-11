using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BCCH
{
    public class ApplicationManager : MonoBehaviour
    {
        public void Quit()
        {
#if UNITY
#if UNITY_EDITOR
            
            UnityEditor.EditorApplication.isPlaying = false;
#else
imulationManager.instance.REMOVE_CurrentObject();
            Application.Quit();
#endif
#endif
        }
    }
}
