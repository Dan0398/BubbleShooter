using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay{
    
    [System.Serializable]
    public class GameField
    {
        public const int BubblesCountPerLine = 9;
        [field:SerializeField] public float BubbleSize {get; private set;}
        [SerializeField] bool UsingShift;
        [SerializeField] Vector2 GameFieldSize;
        [SerializeField] Vector2 LinesStartPoint;
        [SerializeField] LineOfBubbles[] CreatedLines;
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
            CleanupLines();
        }
        
        public void ApplyPool(Pool pool) => Pool = pool;
        
        public void CreateFieldEndless()
        {
            CleanupLines();
            FillLinesByBubbles(6);
        }
        
        void CleanupLines()
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
                if (UsingShift) LastLineShifted = !LastLineShifted;
                CreatedLines[i].TryApplyShift(LastLineShifted, Vector3.right * BubbleSize * 0.5f);
                CreatedLines[i].SetVerticalPos(BubbleSize * (i+0.5f));    
            }
        }
        
        void FillLinesByBubbles(int LinesCount)
        {
            for (int i=0; i<LinesCount; i++)
            {
                for (int k=0; k< BubblesCountPerLine; k++)
                {
                    CreatedLines[i].SetBubbleAtPlace(Pool.GiveMeBubble(), k);
                }
            }
        }
        
        struct FieldPlace
        {
            public int Line, Place;
            public FieldPlace(int Line, int Place)
            {
                this.Line = Line;
                this.Place = Place;
            }
        };
        FieldPlace[] NeighbourPlaces;
        
        public void CollectBubble(Bubble NewBubble)
        {
            int LineNumber, BubblePlace;
            GetPos();
            if (!IsCleanNeighboursSuccess()) return;
            CleanUnconnected();
            
            void GetPos()
            {
                LineNumber = GetLineNumberForBubble(NewBubble);
                BubblePlace = GetPlaceNumblerForBubble(NewBubble, CreatedLines[LineNumber].ShiftedRight);
                if (BubblePlace < 0|| BubblePlace >= BubblesCountPerLine)
                {
                    LineNumber++;
                    BubblePlace = GetPlaceNumblerForBubble(NewBubble, CreatedLines[LineNumber].ShiftedRight);
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
            
            bool IsCleanNeighboursSuccess()
            {
                if (NeighbourPlaces == null || (UsingShift && NeighbourPlaces.Length != 6))
                {
                    NeighbourPlaces = new FieldPlace[6];
                }
                else if (NeighbourPlaces == null || (!UsingShift && NeighbourPlaces.Length != 4))
                {
                    NeighbourPlaces = new FieldPlace[4];
                }
                var Color = NewBubble.MyColor;
                var AllNeighbours = CollectSameColored();
                if (AllNeighbours.Count<3) return false;
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
                            FoundNew = true;
                            if (!CreatedLines[Neighbour.Line].IsColorMatch(Color, Neighbour.Place)) continue; 
                            foreach(var Pos in Result)
                            {
                                if (Pos.Line == Neighbour.Line && Pos.Place == Neighbour.Place)
                                {
                                    FoundNew = false;
                                    break;
                                }
                            }
                            if (FoundNew) Result.Add(Neighbour); 
                        }
                    }
                    return Result;
                }
            }
            
            void GetNeighbourPlaces(FieldPlace Center)
            {
                AddIsValid(Center.Line+1, Center.Place, 0);
                AddIsValid(Center.Line-1, Center.Place, 1);
                AddIsValid(Center.Line, Center.Place+1, 2);
                AddIsValid(Center.Line, Center.Place-1, 3);
                
                if (UsingShift) 
                {
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
                }
                
                void AddIsValid(int Line, int Place, int Number)
                {
                    if (Line < 0 || Place < 0 || Line > CreatedLines.Length || Place >= BubblesCountPerLine) return;
                    NeighbourPlaces[Number].Line = Line;
                    NeighbourPlaces[Number].Place = Place;
                }
            }
            
            void CleanUnconnected()
            {
                List<FieldPlace> RootChank;
                SetupRootChunk();
                GrowRootChunk();
                
                var UnconnectedBubbles = CollectBubbles(GetNonRootChankActivePlaces());
                Pool.HideBubbleToPool(UnconnectedBubbles);
                
                void SetupRootChunk()
                {
                    RootChank = new List<FieldPlace>(BubblesCountPerLine);
                    for (int i = 0; i < BubblesCountPerLine;i++)
                    {
                        if (!CreatedLines[0].IsBubbleExist(i)) continue;
                        RootChank.Add(new FieldPlace(0, i));
                    }
                }
                
                void GrowRootChunk()
                {
                    bool FoundNew = true;
                    int NN;
                    for(int i=0; i < RootChank.Count; i++)
                    {
                        GetNeighbourPlaces(RootChank[i]);
                        for (NN=0; NN< NeighbourPlaces.Length; NN++)
                        {
                            FoundNew = true;
                            if (!CreatedLines[NeighbourPlaces[NN].Line].IsBubbleExist(NeighbourPlaces[NN].Place)) continue; 
                            for (int k=0; k< RootChank.Count; k++)
                            {
                                if (RootChank[k].Line == NeighbourPlaces[NN].Line && RootChank[k].Place == NeighbourPlaces[NN].Place)
                                {
                                    FoundNew = false;
                                    break;
                                }
                            }
                            if (FoundNew)
                            {
                                RootChank.Add(NeighbourPlaces[NN]);
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