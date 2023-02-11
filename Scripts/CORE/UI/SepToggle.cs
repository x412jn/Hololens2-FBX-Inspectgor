using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BCCH
{
    public class SepToggle : MonoBehaviour
    {
        [HideInInspector]
        public GameObject toggleObject;

        [HideInInspector]
        public int id;

        public DelegateBody delegateBody = new DelegateBody();

        public delegate void OnToggleObject();
        public OnToggleObject onToggleObject;

        public void ToggleObject()
        {
            toggleObject.SetActive(!toggleObject.activeSelf);
            onToggleObject();
        }
    }
}

