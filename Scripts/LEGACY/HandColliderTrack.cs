using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace BCCH
{
    public class HandColliderTrack : MonoBehaviour
    {
        //Finger detector
        //[SerializeField]
        //float touchTime = 3.0f;
        //private float touchTimeCurrent = 0f;

        [HideInInspector]
        public Transform rightMiddleTip;
        Transform rightRingTip;
        Transform rightPinkyTip;
        Transform rightThumbTip;
        Transform leftIndexTip;

        //[HideInInspector]
        //public bool onTouch_Middle = false;
        //[HideInInspector]
        //public bool onTouch_Ring;
        //[HideInInspector]
        //public bool onTouch_Pinky;
        //[HideInInspector]
        //public bool onTouch_Thumb;

        //bool onReachMaximumTime_Middle = false;
        //bool onCancel_Middle = true;

        //[SerializeField]
        //float distanceDetection = 0.02f;
        //float distanceCurrent = 0;

        //Dev UI

        //[SerializeField]
        //GameObject onTouchText_Display;
        //[SerializeField]
        //TextMeshProUGUI onTouchText;


        private void Update()
        {
            //This method is for updating finger joint colliders
            FingerColliderTrack();
        }

        //private void FixedUpdate()
        //{

        //    //Finger touch operation detection
        //    FingerTouchDetection();

        //    if (onTouch_Middle)
        //    {
        //        if (touchTimeCurrent < touchTime)
        //        {
        //            touchTimeCurrent += Time.deltaTime;
        //        }
        //        else
        //        {
        //            onReachMaximumTime_Middle = true;

        //        }
        //        onTouchText_Display.SetActive(true);
        //        onTouchText_Display.transform.position = rightMiddleTip.position;
        //        onTouchText.text = touchTimeCurrent.ToString();
        //    }
        //    else
        //    {
        //        if (touchTimeCurrent > 0)
        //        {
        //            touchTimeCurrent -= Time.deltaTime;
        //            onTouchText_Display.SetActive(true);
        //            onTouchText_Display.transform.position = rightMiddleTip.position;
        //            onTouchText.text = touchTimeCurrent.ToString();
        //        }
        //        else
        //        {
        //            onTouchText_Display.SetActive(false);
        //            onReachMaximumTime_Middle = false;
        //        }
        //    }

        //}

        //public void FingerTouchDetection()
        //{
        //    if (leftIndexTip != null && rightMiddleTip != null)
        //    {
        //        distanceCurrent = Vector3.Distance(leftIndexTip.position, rightMiddleTip.position);
        //        //Debug.Log(distanceCurrent);
        //    }
        //    else
        //    {
        //        distanceCurrent = 0;
        //    }

        //    if (distanceCurrent != 0f && distanceCurrent <= distanceDetection)
        //    {
        //        onTouch_Middle = true;
        //    }
        //    else
        //    {
        //        onTouch_Middle = false;
        //    }

        //}

        public void FingerColliderTrack()
        {
            var handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
            if (handJointService != null)
            {
                rightMiddleTip = handJointService.RequestJointTransform(TrackedHandJoint.MiddleTip, Handedness.Right);
                rightRingTip = handJointService.RequestJointTransform(TrackedHandJoint.RingTip, Handedness.Right);
                rightPinkyTip = handJointService.RequestJointTransform(TrackedHandJoint.PinkyTip, Handedness.Right);
                rightThumbTip = handJointService.RequestJointTransform(TrackedHandJoint.ThumbTip, Handedness.Right);
                leftIndexTip = handJointService.RequestJointTransform(TrackedHandJoint.IndexTip, Handedness.Left);
            }
        }


    }
}

