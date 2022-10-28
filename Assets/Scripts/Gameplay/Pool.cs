using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay{
    public class Pool
    {
        Transform OnScene;
        Queue<Bubble> PooledBubbles;
        GameObject Sample;
        float BubbleSize;
        
        public Pool(GameObject BubbleSample, float BubbleSize, Transform Parent)
        {
            Sample = BubbleSample;
            this.BubbleSize = BubbleSize;
            PooledBubbles = new Queue<Bubble>();
            CreateSceneObj(Parent);
        }
        
        public void CreateSceneObj(Transform Parent)
        {
            var Obj = new GameObject("Bubbles Pool");
            Obj.SetActive(false);
            OnScene = Obj.transform;
            OnScene.SetParent(Parent);
        }
        
        public Bubble GiveMeBubble()
        {
            if (PooledBubbles.Count > 0)
            {
                return PooledBubbles.Dequeue();
            }
            return new Bubble(Sample, OnScene, BubbleSize, Bubble.GetRandomColor());
        }
        
        public void HideBubbleToPool(Bubble UselessBubble)
        {
            PooledBubbles.Enqueue(UselessBubble);
            UselessBubble.MyTransform.SetParent(OnScene);
        }
        
        public void HideBubbleToPool(Bubble[] UselessBubbles)
        {
            foreach(var Bubble in UselessBubbles) HideBubbleToPool(Bubble);
        }
    }
}