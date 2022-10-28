using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Map
{
    [System.Serializable]
    public class MapIcon
    {
        [System.Serializable]
        public enum IconStatus
        {
            Locked = -1,
            Available = 0,
            Complete1Star = 1,
            Complete2Star = 2,
            Complete3Star = 3
        }
        public RectTransform IconTransform {get; private set;}
        GameObject OnScene;
        GameObject AvailableParent, CompletedParent, LockedParent;
        
        private MapIcon(GameObject Sample, Transform Parent)
        {
            OnScene = GameObject.Instantiate(Sample, Parent);
            OnScene.SetActive(true);
            IconTransform = OnScene.GetComponent<RectTransform>();
            AvailableParent = IconTransform.GetChild(1).gameObject;
            CompletedParent = IconTransform.GetChild(2).gameObject;
            LockedParent = IconTransform.GetChild(3).gameObject;
        }
        
        public MapIcon(GameObject Sample, Transform Parent, IconStatus Status = IconStatus.Locked) : this(Sample, Parent)
        {
            UpdateStatus(Status);
        }
        
        public void UpdateStatus(IconStatus Status)
        {
            LockedParent.SetActive(     Status == IconStatus.Locked);
            AvailableParent.SetActive(  Status == IconStatus.Available);
            CompletedParent.SetActive(  Status == IconStatus.Complete1Star || 
                                        Status == IconStatus.Complete2Star ||
                                        Status == IconStatus.Complete3Star);
        }
        
        public void SetAnchoredX(float XAnchor)
        {
            IconTransform.anchorMin = new Vector2(XAnchor, 0);
            IconTransform.anchorMax = new Vector2(XAnchor, 1);
            IconTransform.offsetMin = Vector2.zero;
            IconTransform.offsetMax = Vector2.zero;
        }
        
        public Vector3 GetCenterPoint()
        {
            Vector3 Result = IconTransform.position;
            Result += IconTransform.TransformVector(Vector3.down * IconTransform.rect.height * 0.3f);
            Result -= Vector3.forward * Result.z;
            return Result;
        }
    }
}