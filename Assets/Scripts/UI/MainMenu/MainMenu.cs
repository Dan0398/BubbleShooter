using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace UI.Menu
{
    public class MainMenu : MonoBehaviour, Services.IService
    {
        [SerializeField] Submenu StartMenu, GameTypesMenu, LevelsMenu;
        Submenu[] SubMenus => new Submenu[] { StartMenu, GameTypesMenu, LevelsMenu};
        CancellationTokenSource AnimBreaker;
        
        void Start()
        {
            SelfRegister();
            InitSubMenu();
        }
        
        void SelfRegister()
        {
            Services.DI.Register<UI.Menu.MainMenu>(this);
        }
        
        void InitSubMenu()
        {
            foreach(var Menu in SubMenus) Menu.Init();
        }
        
        void RefreshAnimBreaker()
        {
            AnimBreaker?.Cancel();
            AnimBreaker = new CancellationTokenSource();
        }
        
        public void ShowGameTypesScreen()
        {
            RefreshAnimBreaker();
            StartMenu.HideAnimated(AnimBreaker.Token);
            GameTypesMenu.ShowAnimated(AnimBreaker.Token);
            Services.DI.Single<Services.BackButton>().RegisterBackAction(()=>
            {
                RefreshAnimBreaker();
                StartMenu.ShowAnimated(AnimBreaker.Token);
                GameTypesMenu.HideAnimated(AnimBreaker.Token);
            });
        }
        
        public void ShowLevelsScreen()
        {
            RefreshAnimBreaker();
            GameTypesMenu.HideAnimated(AnimBreaker.Token);
            LevelsMenu.ShowAnimated(AnimBreaker.Token);
            Services.DI.Single<Services.BackButton>().RegisterBackAction(()=>
            {
                RefreshAnimBreaker();
                GameTypesMenu.ShowAnimated(AnimBreaker.Token);
                LevelsMenu.HideAnimated(AnimBreaker.Token);
            });
        }
    }
}