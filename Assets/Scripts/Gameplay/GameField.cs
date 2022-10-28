using System.Collections.Generic;
using UnityEngine;

namespace Gameplay{
    
    [System.Serializable]
    public class GameField
    {
        struct FieldPlace
        {
            public bool Valid;
            public int Line, Place;
            public FieldPlace(int Line, int Place)
            {
                Valid = true;
                this.Line = Line;
                this.Place = Place;
            }
        };
        public const int BubblesCountPerLine = 9;
        [field:SerializeField] public float BubbleSize {get; private set;}
        [SerializeField] Vector2 GameFieldSize;
        [SerializeField] Vector2 LinesStartPoint;
        [SerializeField] LineOfBubbles[] CreatedLines;
        System.Action OnSuccessEnded;
        FieldPlace[] NeighbourPlaces;
        bool LastLineShifted = false;
        Transform LinesParent;
        Pool Pool;
        
        public void Init(Transform Parent)
        {
            var LinesObj = new GameObject("Lines");
            LinesParent = LinesObj.transform;
            LinesParent.transform.SetParent(Parent);
            LinesParent.localPosition = Vector3.zero;
            BubbleSize = GameFieldSize.x / (float)(BubblesCountPerLine+0.5f);
            CreatedLines = new LineOfBubbles[Mathf.FloorToInt(GameFieldSize.y/BubbleSize)];
            if (NeighbourPlaces == null || NeighbourPlaces.Length != 6)
            {
                NeighbourPlaces = new FieldPlace[6];
            }
            CleanupLines();
        }
        
        public void ApplyPool(Pool pool) => Pool = pool;
        
        public void CreateLevelField(Content.Level Data, System.Action OnSuccess)
        {
            CleanupLines();
            FillLinesWithLevelData(Data);
            OnSuccessEnded = OnSuccess;
        }
        
        public void CleanupLines()
        {
            LastLineShifted = false;
            for (int i=0; i< CreatedLines.Length; i++)
            {
                if (CreatedLines[i] != null)
                {
                    CreatedLines[i].CleanupLine(Pool);
                    continue;
                }
                CreatedLines[i] = new LineOfBubbles(LinesParent, LinesStartPoint);
                LastLineShifted = !LastLineShifted;
                CreatedLines[i].TryApplyShift(LastLineShifted, Vector3.right * BubbleSize * 0.5f);
                CreatedLines[i].SetVerticalPos(BubbleSize * (i+0.5f));    
            }
        }
        
        void FillLinesWithLevelData(Content.Level Data)
        {
            int MaxBubble = 0;
            for (int i=0; i< Data.Lines.Length; i++)
            {
                MaxBubble = Mathf.Max(Data.Lines[i].Colors.Length, BubblesCountPerLine);
                for (int k=0; k < MaxBubble; k++)
                {
                    if (Data.Lines[i].Colors[k] == Bubble.BubbleColor.None) continue;
                    var bubble = Pool.GiveMeBubble();
                    bubble.ChangeColor(Data.Lines[i].Colors[k]);
                    CreatedLines[i].SetBubbleAtPlace(bubble, k);
                }
            }
        }
        
        public void CreateFieldEndless()
        {
            CleanupLines();
            FillLinesByBubbles(6);
            OnSuccessEnded = CreateFieldEndless;
        }
        
        void FillLinesByBubbles(int LinesCount)
        {
            for (int i=0; i<LinesCount; i++)
            {
                for (int k=0; k< BubblesCountPerLine; k++)
                {
                    var Bubble = Pool.GiveMeBubble();
                    Bubble.RandomizeColor();
                    CreatedLines[i].SetBubbleAtPlace(Bubble, k);
                }
            }
        }
        
        public void PlaceUserBubble(Bubble NewBubble)
        {
            List<FieldPlace> RootChank;
            int LineNumber = 0, BubblePlace = 0;
            GetPos();
            if (NewBubble == null) return;
            if (!IsCleanNeighboursSuccess()) return;
            SetupRootChunk();
            if (RootChank.Count == 0)
            {
                OnSuccessEnded?.Invoke();
                return;
            }
            CleanUnconnected();
            
            void GetPos()
            {
                LineNumber = GetLineNumberForBubble(NewBubble);
                if (LineNumber < 0)
                {
                    Pool.HideBubbleToPool(NewBubble);
                    NewBubble = null;
                    return;
                }
                BubblePlace = Mathf.Clamp(GetPlaceNumblerForBubble(NewBubble, CreatedLines[LineNumber].ShiftedRight), 0, BubblesCountPerLine-1);
                if (CreatedLines[LineNumber].IsBubbleExist(BubblePlace))
                {
                    GetNeighbourPlaces(new FieldPlace(LineNumber, BubblePlace));
                    foreach(var Neighbour in NeighbourPlaces)
                    {
                        if (!Neighbour.Valid) continue;
                        if (!CreatedLines[Neighbour.Line].IsBubbleExist(Neighbour.Place))
                        {
                            LineNumber = Neighbour.Line;
                            BubblePlace = Neighbour.Place;
                            break;
                        }
                    }
                }
                CreatedLines[LineNumber].SetBubbleAtPlace(NewBubble, BubblePlace);
            }
        
            int GetLineNumberForBubble(Bubble NewBubble)
            {
                float Height = NewBubble.MyTransform.position.y;
                return Mathf.FloorToInt((LinesStartPoint.y - Height)/BubbleSize);
            }
            
            int GetPlaceNumblerForBubble(Bubble NewBubble, bool Shifted)
            {
                float XPos = NewBubble.MyTransform.position.x;
                if (Shifted) XPos -= BubbleSize * 0.5f;
                return Mathf.FloorToInt((GameFieldSize.x/2f + XPos)/BubbleSize);
            }
            
            void GetNeighbourPlaces(FieldPlace Center)
            {
                AddIsValid(Center.Line+1, Center.Place, 0);
                AddIsValid(Center.Line-1, Center.Place, 1);
                AddIsValid(Center.Line, Center.Place+1, 2);
                AddIsValid(Center.Line, Center.Place-1, 3);
                
                if (CreatedLines[Center.Line].ShiftedRight)
                {
                    AddIsValid(Center.Line+1, Center.Place+1, 4);
                    AddIsValid(Center.Line-1, Center.Place+1, 5);
                }
                else 
                {
                    AddIsValid(Center.Line+1, Center.Place-1, 4);
                    AddIsValid(Center.Line-1, Center.Place-1, 5);
                }
                
                void AddIsValid(int Line, int Place, int Number)
                {
                    if (Line < 0 || Place < 0 || Line > CreatedLines.Length || Place >= BubblesCountPerLine)
                    {
                        NeighbourPlaces[Number].Valid = false;
                        return;
                    } 
                    NeighbourPlaces[Number].Valid = true;
                    NeighbourPlaces[Number].Line = Line;
                    NeighbourPlaces[Number].Place = Place;
                }
            }
            
            bool IsCleanNeighboursSuccess()
            {
                var Color = NewBubble.MyColor;
                var AllNeighbours = CollectSameColored();
                if (AllNeighbours == null || AllNeighbours.Count<3) return false;
                Pool.HideBubbleToPool(CollectBubbles(AllNeighbours));
                return true;
                
                List<FieldPlace> CollectSameColored()
                {
                    List<FieldPlace> Result = new List<FieldPlace>();
                    Result.Add(new FieldPlace(LineNumber, BubblePlace));
                    for (int i=0; i < Result.Count; i++)
                    {
                        GetNeighbourPlaces(Result[i]);
                        bool FoundNew = true;
                        foreach(var Neighbour in NeighbourPlaces)
                        {
                            if (!Neighbour.Valid) continue;
                            if (!CreatedLines[Neighbour.Line].IsColorMatch(Color, Neighbour.Place)) continue; 
                            FoundNew = true;
                            foreach(var Pos in Result)
                            {
                                if (Pos.Line == Neighbour.Line && Pos.Place == Neighbour.Place)
                                {
                                    FoundNew = false;
                                    break;
                                }
                            }
                            if (FoundNew) 
                            {
                                Result.Add(Neighbour); 
                            }
                        }
                    }
                    return Result;
                }
            }
            
            void SetupRootChunk()
            {
                RootChank = new List<FieldPlace>(BubblesCountPerLine);
                for (int i = 0; i < BubblesCountPerLine;i++)
                {
                    if (!CreatedLines[0].IsBubbleExist(i)) continue;
                    RootChank.Add(new FieldPlace(0, i));
                }
            }
            
            void CleanUnconnected()
            {
                GrowRootChunk();
                var UnconnectedBubbles = CollectBubbles(GetNonRootChankActivePlaces());
                Pool.HideBubbleToPool(UnconnectedBubbles);
                
                void GrowRootChunk()
                {
                    bool FoundNew = true;
                    for(int i=0; i < RootChank.Count; i++)
                    {
                        GetNeighbourPlaces(RootChank[i]);
                        foreach(var Neighbour in NeighbourPlaces)
                        {
                            if (!Neighbour.Valid) continue;
                            if (!CreatedLines[Neighbour.Line].IsBubbleExist(Neighbour.Place)) continue; 
                            FoundNew = true;
                            for (int k=0; k< RootChank.Count; k++)
                            {
                                if (RootChank[k].Line == Neighbour.Line && RootChank[k].Place ==Neighbour.Place)
                                {
                                    FoundNew = false;
                                    break;
                                }
                            }
                            if (FoundNew)
                            {
                                RootChank.Add(Neighbour);
                            }
                        }
                    }
                }
                
                List<FieldPlace> GetNonRootChankActivePlaces()
                {
                    bool NonRoot = true;
                    List<FieldPlace> Result = new List<FieldPlace>();
                    for (int Line = 0; Line< CreatedLines.Length; Line++)
                    {
                        for (int Place = 0; Place < BubblesCountPerLine; Place++)
                        {
                            if (!CreatedLines[Line].IsBubbleExist(Place)) continue;
                            NonRoot = true;
                            foreach(var RootPlace in RootChank)
                            {
                                if (RootPlace.Line == Line && RootPlace.Place == Place)
                                {
                                    NonRoot = false;
                                    break;
                                }
                            }
                            if (NonRoot)
                            {
                                Result.Add(new FieldPlace(Line, Place));
                            }
                        }
                    }
                    return Result;
                }
            }
            
            Bubble[] CollectBubbles(List<FieldPlace> PlaceOfBubbles)
            {
                Bubble[] Result = new Bubble[PlaceOfBubbles.Count];
                for(int i=0; i< Result.Length; i++)
                {
                    Result[i] = CreatedLines[PlaceOfBubbles[i].Line].GiveMeBubbleAtPlace(PlaceOfBubbles[i].Place);
                }
                return Result;
            }
        }
    }
}