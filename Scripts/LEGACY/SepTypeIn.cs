using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRKeyboard.Utils;
using UnityEngine.UI;

namespace BCCH
{
    public class SepTypeIn : MonoBehaviour
    {
        //private variables for prefabs
        KeyboardManager keyboardManager;
        //prefab
        public GameObject keyboard;
        public Transform keyboardAnchor;
        public Text targetText;

        private GameObject keyboardInstance;
        
        //private variables for keyboard
        private Text keyboardInput;

        //public variables for manager
        //to store objects that issued fom prefabs and read by another prefab
        public Text keyboardTarget;
        private SepTypeIn sepTypeInManager;

        private void Start()
        {
            sepTypeInManager = GameObject.FindWithTag("TypeInManager").GetComponent<SepTypeIn>();
        }

        public void OnGenerateKeyboard()
        {
            keyboardInstance = GameObject.FindWithTag("GazeKeyboard");
            
            if (keyboardInstance != null)
            {
                Destroy(keyboardInstance.gameObject);
            }
            keyboardInstance = Instantiate(keyboard, keyboardAnchor);
            keyboardManager = keyboardInstance.GetComponent<KeyboardManager>();
            sepTypeInManager.keyboardTarget = targetText;
            //keyboardInput = keyboardManager.inputText;
            //keyboardInput = keyboardInstance.transform.Find("Content").Find("input").GetComponent<Text>();
        }


        public void OnHitEnter()
        {
            keyboardManager = this.GetComponent<KeyboardManager>();
            keyboardInstance = this.gameObject;
            sepTypeInManager.keyboardTarget.text = keyboardManager.inputText.text;
            Destroy(keyboardInstance.gameObject);

        }
        
    }
}

