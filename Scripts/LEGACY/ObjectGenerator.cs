using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BCCH
{
    public class ObjectGenerator : MonoBehaviour
    {
        //CORE COMPONENTS ASSIGN
        HandColliderTrack handColliderTrack;

        //Prefab assign
        GameObject sectioningBox;

        //Parent Transform Assign
        Transform spawningParent;

        //Object Registration
        GameObject sectioningBox_Current;

        private void Start()
        {
            handColliderTrack = GameObject.Find("HandColTrack").GetComponent<HandColliderTrack>();
            spawningParent = GameObject.Find("[OBJECTS]").transform;
        }

        //Spawn Object
        public void Spawn_SectioningBox()
        {
            if (sectioningBox_Current != null)
            {
                Destroy(sectioningBox_Current);
            }
            sectioningBox_Current = Instantiate(sectioningBox, handColliderTrack.rightMiddleTip.position, handColliderTrack.rightMiddleTip.rotation);
            sectioningBox_Current.transform.SetParent(spawningParent);
        }
    }
}

