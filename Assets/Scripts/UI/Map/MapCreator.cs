using System.Threading.Tasks;
using UnityEngine;
using IconStatus = Data.UserData.LevelStatus;

namespace UI.Map
{
    public class MapCreator : MonoBehaviour
    {
        const int IconsPerLine = 2;
        [SerializeField] int IconsShown;
        [SerializeField] GameObject LineSample, IconSample;
        [SerializeField] Transform LinesParent;
        [SerializeField] RectTransform ContentParent, BackgroundTransform;
        Data.UserDataController User;
        MapIcon[] CreatedIcons;
        GameObject[] CreatedLines;
        Vector3[] MapLinePoints;
        [SerializeField] LineRenderer MapLine;
        bool Subscribed;
        
        void Start()
        {
            CreateMap();
        }
        
        void CreateMap()
        {
            CreateLines();
            CreateMarks();
            SetupLine();
            SetupBackground();
            ProcessUserData();
        }
        
        void CreateLines()
        {
            CreatedLines = new GameObject[Mathf.CeilToInt(IconsShown/(float)IconsPerLine)];
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
            CreatedIcons = new MapIcon[IconsShown];
            for (int i = 0; i < IconsShown; i++)
            {
                CreatedIcons[i] = new MapIcon(IconSample, CreatedLines[LineId].transform, IconStatus.Locked);
                if ((!Reverse && IconsOnLine == 0) || (Reverse && IconsOnLine == 1))
                {
                    CreatedIcons[i].SetAnchoredX(0.3f);
                }   
                else
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
            MapLinePoints = new Vector3[IconsShown];
            for (int i = 0; i < IconsShown; i++)
            {
                MapLinePoints[i] = LineTransform.InverseTransformPoint(CreatedIcons[i].GetCenterPoint());
            }
            MapLine.positionCount = IconsShown;
            MapLine.SetPositions(MapLinePoints);
        }
        
        void SetupBackground()
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(ContentParent);
            BackgroundTransform.offsetMax = Vector2.up * Mathf.Max(Screen.height, ContentParent.rect.height);
        }
        
        async void ProcessUserData()
        {
            User = Services.DI.Single<Data.UserDataController>();
            while (!User.isDataLoaded) await Task.Delay(100);
            RefreshByUserData();
            User.Data.OnLevelsChanged += RefreshByUserData;
        }
        
        void RefreshByUserData()
        {
            for (int i=0; i< User.Data.Levels.Length; i++)
            {
                CreatedIcons[i].UpdateStatus(User.Data.Levels[i]);
                if (User.Data.Levels[i] != IconStatus.Locked)
                {
                    int LevelNumber = i;
                    CreatedIcons[i].ProcessOnClicked(()=> RunLevel(LevelNumber));
                }
            }
        }
        
        void RunLevel(int LevelNumber)
        {
            Services.DI.Single<Gameplay.GameplayController>().StartLevel(LevelNumber);
            Services.DI.Single<Services.BackButton>().Cleanup();
            Services.DI.Single<UI.Menu.MainMenu>().HideAll();
        }
    }
}