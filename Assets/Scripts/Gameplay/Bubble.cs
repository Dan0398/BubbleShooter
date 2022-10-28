using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gameplay{
    [System.Serializable]
    public class Bubble
    {
        public static Color GetColorByEnum(BubbleColor RequiredColor)
        {
            if (RequiredColor == BubbleColor.Red) return Color.red;
            else if (RequiredColor == BubbleColor.Green) return Color.green;
            else if (RequiredColor == BubbleColor.Yellow) return Color.yellow;
            else if (RequiredColor == BubbleColor.Purple) return new Color32(193,0,255,255);
            else return Color.black;
        }
        
        public static BubbleColor GetRandomColor()
        {
            return (BubbleColor) Random.Range(0,System.Enum.GetValues(typeof(BubbleColor)).Length);
        }
        
        public enum BubbleColor
        {
            Red, Green, Yellow, Purple
        }
        
        public Transform MyTransform {get; private set;}
        public BubbleColor MyColor {get; private set;}
        GameObject OnScene;
        Collider2D Col;
        float Size;
        
        public Bubble(GameObject Sample, Transform Parent, float BubbleSize, BubbleColor Color)
        {
            OnScene = GameObject.Instantiate(Sample);
            OnScene.SetActive(true);
            MyTransform = OnScene.transform;
            MyTransform.SetParent(Parent);
            Col = OnScene.GetComponent<Collider2D>();
            Size = BubbleSize;
            MyTransform.localScale = Vector3.one * BubbleSize;
            ChangeColor(Color);
        }
        
        public void ChangeColor(BubbleColor NewColor)
        {
            MyColor = NewColor;
            OnScene.GetComponent<SpriteRenderer>().color = GetColorByEnum(MyColor);
        }
        
        public void RandomizeColor() => ChangeColor(GetRandomColor());
        
        public void PlaceInLine(Transform LineTransform, int IdOnLine)
        {
            MyTransform.SetParent(LineTransform);
            MyTransform.localPosition = Vector3.right * (-GameField.BubblesCountPerLine/2f + IdOnLine+0.25f) * Size;
        }
        
        public void DeactivateCollisions() => Col.enabled = false;
        
        public void ActivateCollisions() => Col.enabled = true;
    }
}