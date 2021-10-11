using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BCCH
{
    public class ApplicationManager : MonoBehaviour
    {
        public SimulationManager simulationManager;
        public void Quit()
        {
#if UNITY_EDITOR
            
            UnityEditor.EditorApplication.isPlaying = false;
#else
            
            simulationManager.REMOVE_CurrentObject();
            Application.Quit();
#endif
        }
    }
}

