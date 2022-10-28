using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Content{
    public class LevelsHolder : Services.IService
    {
        public Level[] Levels;
        public LevelsHolder()
        {
            PreloadData();
        }
        
        void PreloadData()
        {
            Levels = Resources.LoadAll<Level>("Levels/") as Level[];
        }
    }
}