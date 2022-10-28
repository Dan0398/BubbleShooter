using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Map
{
    public class MapCreator : MonoBehaviour
    {
        const int IconsPerLine = 2;
        [SerializeField] int TestIcons;
        [SerializeField] GameObject LineSample, IconSample;
        [SerializeField] Transform LinesParent;
        [SerializeField] RectTransform ContentParent, BackgroundTransform;
        MapIcon[] CreatedIcons;
        GameObject[] CreatedLines;
        Vector3[] MapLinePoints;
        [SerializeField] LineRenderer MapLine;
        
        void Start()
        {
            CreateTestMap();
        }
        
        void CreateTestMap()
        {
            CreateLines();
            CreateMarks();
            SetupLine();
            SetupBackground();
        }
        
        void CreateLines()
        {
            CreatedLines = new GameObject[Mathf.CeilToInt(TestIcons/(float)IconsPerLine)];
            for (int i = 0; i < CreatedLines.Length; i ++)
            {
                CreatedLines[i] = GameObject.Instantiate(LineSample, LinesParent);
                CreatedLines[i].name = "Map Line #" + i.ToString();
                CreatedLines[i].SetActive(true);
            }
        }
        
        void CreateMarks()
        {
            int IconsOnLine = 0, LineId = 0;
            bool Reverse = false;
            CreatedIcons = new MapIcon[TestIcons];
            for (int i = 0; i < TestIcons; i++)
            {
                CreatedIcons[i] = new MapIcon(IconSample, CreatedLines[LineId].transform, (MapIcon.IconStatus)Random.Range(-1,4));
                if ((!Reverse && IconsOnLine == 0) || (Reverse && IconsOnLine == 1))
                {
                    CreatedIcons[i].SetAnchoredX(0.3f);
                }   
                else// if (IconsOnLine == 1)
                {
                    CreatedIcons[i].SetAnchoredX(0.8f);
                }
                IconsOnLine++;
                if (IconsOnLine >= IconsPerLine)
                {
                    IconsOnLine = 0;
                    LineId++;
                    Reverse = LineId % 2 == 1;
                }
            }
        }
        
        void SetupLine()
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(ContentParent);
            var LineTransform = MapLine.transform;
            MapLinePoints = new Vector3[TestIcons];
            for (int i = 0; i < TestIcons; i++)
            {
                MapLinePoints[i] = LineTransform.InverseTransformPoint(CreatedIcons[i].GetCenterPoint());
            }
            MapLine.positionCount = TestIcons;
            MapLine.SetPositions(MapLinePoints);
        }
        
        void SetupBackground()
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(ContentParent);
            BackgroundTransform.offsetMax = Vector2.up * Mathf.Max(Screen.height, ContentParent.rect.height);
        }
    }
}