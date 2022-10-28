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
        Bubble.BubbleColor[] LevelColors;
        int NumberInCycle;
        
        public void Init(Transform Parent, Pool Pool, float BubbleSize)
        {
            BubblesPool = Pool;
            this.BubbleSize = BubbleSize;
            CreateRootObject(Parent);
        }
        
        void CreateRootObject(Transform Parent)
        {
            if (OnScene != null) return;
            var Obj = new GameObject("Queue");
            OnScene = Obj.transform;
            OnScene.position = PosOnScreen;
            OnScene.SetParent(Parent);
        }
        
        void RefreshBubbles()
        {
            if (CreatedBubbles == null)
            {
                CreatedBubbles = new Bubble[5];
            }
            for (int i=0; i< CreatedBubbles.Length; i++)
            {
                SetupBubbleFromPool(i);
            }
            RefreshTransforms();
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
            SetupBubbleFromPool(CreatedBubbles.Length-1);
            RefreshTransforms();
            return Result;
        }
        
        void SetupBubbleFromPool(int NumberInQueue)
        {
            CreatedBubbles[NumberInQueue] = BubblesPool.GiveMeBubble();
            if (NumberInCycle == -1)
            {
                CreatedBubbles[NumberInQueue].RandomizeColor();
            }
            else 
            {
                CreatedBubbles[NumberInQueue].ChangeColor(LevelColors[NumberInCycle]);
                NumberInCycle++;
                if (NumberInCycle>= LevelColors.Length) NumberInCycle = 0;
            }
        }
        
        public void SetRandomize()
        {
            NumberInCycle = -1;
            OnScene.gameObject.SetActive(true);
            RefreshBubbles();
        }
        
        public void SetLevelColorCycle(Content.Level LevelData)
        {
            NumberInCycle = 0;
            LevelColors = LevelData.PlayerPool;
            OnScene.gameObject.SetActive(true);
            RefreshBubbles();
        }
        
        public void Deactivate()
        {
            for (int i=0; i< CreatedBubbles.Length; i++)
            {
                if (CreatedBubbles[i] == null) continue;
                BubblesPool.HideBubbleToPool(CreatedBubbles[i]);
                CreatedBubbles[i] = null;
            }
            OnScene.gameObject.SetActive(false);
        }
    }
}