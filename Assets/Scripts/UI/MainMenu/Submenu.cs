using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    [System.Serializable]
    public class Submenu
    {
        [SerializeField] RectTransform OnScene;
        KeyValuePair<MaskableGraphic, Color>[] BaseColors;
        
        public void Init()
        {
            SetupColors();
        }
        
        void SetupColors()
        {
            var AllMaskables = OnScene.GetComponentsInChildren<MaskableGraphic>();
            BaseColors = new KeyValuePair<MaskableGraphic, Color>[AllMaskables.Length];
            for (int i = 0; i < BaseColors.Length; i++)
            {
                BaseColors[i] = new KeyValuePair<MaskableGraphic, Color>(AllMaskables[i], AllMaskables[i].color);
            }
        }
        
        public void ShowAnimated(CancellationToken? token = null) => Animate(true, token);
        
        public void HideAnimated(CancellationToken? token = null) => Animate(false, token);
        
        async void Animate(bool isInvoking, CancellationToken? token)
        {
            const int AnimationSteps = 40;
            float Lerp = 0;
            if (isInvoking) OnScene.gameObject.SetActive(true);
            for (int i = 0; i <= AnimationSteps; i++)
            {
                if (!Application.isPlaying) return;
                if (token.Value.IsCancellationRequested) break;
                Lerp = i / (float)AnimationSteps;
                if (isInvoking) Lerp = 1 - Lerp;
                RefreshView(Lerp);
                await Task.Delay(16);
            }
            RefreshView(isInvoking? 0: 1);
            if (!isInvoking) OnScene.gameObject.SetActive(false);
            
            
            void RefreshView(float Lerp)
            {
                OnScene.localScale = Vector3.one * (1 + Lerp * 0.4f);
                foreach(var Maskable in BaseColors)
                {
                    Maskable.Key.color = Maskable.Value - Color.black * Maskable.Value.a * Lerp;
                }
            }
        }
    }
}