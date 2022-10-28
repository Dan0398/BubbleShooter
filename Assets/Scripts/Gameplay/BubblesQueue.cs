using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay{
    [System.Serializable]
    public class BubblesQueue
    {
        [SerializeField] Vector2 PosOnScreen;
        Transform OnScene;
        Bubble[] CreatedBubbles;
        Pool BubblesPool;
        float BubbleSize;
        
        public void Init(Transform Parent, Pool Pool, float BubbleSize)
        {
            CreateRootObject(Parent);
            BubblesPool = Pool;
            this.BubbleSize = BubbleSize;
            CreatedBubbles = new Bubble[5];
            for (int i=0; i< CreatedBubbles.Length; i++)
            {
                CreatedBubbles[i] = BubblesPool.GiveMeBubble();
                CreatedBubbles[i].RandomizeColor();
            }
            RefreshTransforms();
        }
        
        void CreateRootObject(Transform Parent)
        {
            if (OnScene != null) return;
            var Obj = new GameObject("Queue");
            OnScene = Obj.transform;
            OnScene.position = PosOnScreen;
            OnScene.SetParent(Parent);
        }
        
        void RefreshTransforms()
        {
            for (int i=0; i< CreatedBubbles.Length; i++)
            {
                CreatedBubbles[i].MyTransform.SetParent(OnScene);
                CreatedBubbles[i].MyTransform.localPosition = Vector2.right * BubbleSize * 1.2f * i;
            }
        }
        
        public Bubble SwitchBubbles(Bubble Switchable)
        {
            var Result = CreatedBubbles[0];
            CreatedBubbles[0] = Switchable;
            RefreshTransforms();
            return Result;
        }
        
        public Bubble GiveMeBubble()
        {
            var Result = CreatedBubbles[0];
            for (int i=0; i< CreatedBubbles.Length-1; i++)
            {
                CreatedBubbles[i] = CreatedBubbles[i+1]; 
            }
            CreatedBubbles[CreatedBubbles.Length-1] = BubblesPool.GiveMeBubble();
            CreatedBubbles[CreatedBubbles.Length-1].RandomizeColor();
            RefreshTransforms();
            return Result;
        }
    }
}