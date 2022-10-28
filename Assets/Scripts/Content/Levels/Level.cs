using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Content
{
    [CreateAssetMenu(fileName = "NewLevel", menuName = "Content/Levels", order = 250)]
    public class Level : ScriptableObject
    {
        [System.Serializable]
        public class LineData
        {
            public Gameplay.Bubble.BubbleColor[] Colors;
        }
        
        public LineData[] Lines;
        public Gameplay.Bubble.BubbleColor[] PlayerPool;
    }
}