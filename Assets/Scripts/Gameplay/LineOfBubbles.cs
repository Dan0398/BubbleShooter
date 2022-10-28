using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gameplay{
    [System.Serializable]
    public class LineOfBubbles
    {
        Transform OnScene;
        Bubble[] Bubbles;
        Vector3 RootPoint;
        int MyLineNumber = 0;
        public bool ShiftedRight {get; private set;}
        
        public LineOfBubbles(Transform Parent, Vector3 StartPoint)
        {
            var Obj = new GameObject("Bubble line");
            OnScene = Obj.transform;
            OnScene.SetParent(Parent);
            OnScene.localPosition = StartPoint;
            RootPoint = StartPoint;
            Bubbles = new Bubble[GameField.BubblesCountPerLine];
        }
        
        public void CleanupLine(Pool BubblesPool)
        {
            for (int i=0; i< Bubbles.Length; i++)
            {
                if (Bubbles[i] == null) continue;
                BubblesPool.HideBubbleToPool(Bubbles[i]);
                Bubbles[i] = null;
            }
        }
        
        public void SetBubbleAtPlace(Bubble NewBubble, int Place)
        {
            if (Bubbles[Place] != null)
            {
                Debug.Log("Место для пузырька занято");
                return;
            }
            Bubbles[Place] = NewBubble;
            NewBubble.PlaceInLine(OnScene, Place);
        }
        
        public void TryApplyShift(bool Shifted, Vector3 ShiftDelta)
        {
            if (ShiftedRight == Shifted) return;
            ShiftedRight = Shifted;
            OnScene.localPosition += ShiftDelta * (Shifted? 1: -1);
        }
        
        public void SetVerticalPos(float Height, bool Animated = false)
        {
            if (!Animated)
            {
                OnScene.localPosition = RootPoint + Vector3.down * Height + Vector3.right * OnScene.localPosition.x;
            }
            else 
            {
                ShiftVecticalAnimated(Height);
            }
        }
        
        async void ShiftVecticalAnimated(float NewHeight)
        {
            float OldHeight = OnScene.localPosition.y;
            float XPos = OnScene.localPosition.x;
            for (int i=0; i<= 20; i++)
            {
                OnScene.localPosition = Vector3.down * Mathf.Lerp(OldHeight, NewHeight, i/20f);
                OnScene.localPosition += Vector3.right * XPos;
                await System.Threading.Tasks.Task.Delay(16);
            }
        }
        
        public bool IsColorMatch(Bubble.BubbleColor col, int BubblePlace)
        {
            if (Bubbles[BubblePlace] == null) return false; 
            return Bubbles[BubblePlace].MyColor == col;
        }
        
        public Bubble GiveMeBubbleAtPlace(int Place)
        {
            var Result = Bubbles[Place];
            Bubbles[Place] = null;
            return Result;
        }
        
        public bool IsBubbleExist(int Place) => Bubbles[Place] != null;
    }
}