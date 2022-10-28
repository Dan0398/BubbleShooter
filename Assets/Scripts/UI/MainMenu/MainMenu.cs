using System.Threading;
using UnityEngine;

namespace UI.Menu
{
    public class MainMenu : MonoBehaviour, Services.IService
    {
        [SerializeField] Submenu StartMenu, GameTypesMenu, LevelsMenu;
        Submenu Actual;
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
        
        public void ShowBaseWindow()
        {
            RefreshAnimBreaker();
            Actual = StartMenu;
            Actual.ShowAnimated(AnimBreaker.Token);
            LevelsMenu.HideAnimated(AnimBreaker.Token);
            GameTypesMenu.HideAnimated(AnimBreaker.Token);
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
            Actual = GameTypesMenu;
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
            Actual = LevelsMenu;
            LevelsMenu.ShowAnimated(AnimBreaker.Token);
            Services.DI.Single<Services.BackButton>().RegisterBackAction(()=>
            {
                RefreshAnimBreaker();
                GameTypesMenu.ShowAnimated(AnimBreaker.Token);
                LevelsMenu.HideAnimated(AnimBreaker.Token);
            });
        }
        
        public void RunEndless()
        {
            HideAll();
            Services.DI.Single<Services.BackButton>().Cleanup();
            Services.DI.Single<Gameplay.GameplayController>().StartEndless();
        }
        
        public void HideAll()
        {
            foreach(var Menu in SubMenus)
            {
                if (Menu == Actual) Menu.HideAnimated();
                else Menu.Hide();
            }
        }
        
        public void PickBackButton()
        {
            Services.DI.Single<Services.BackButton>().GoBack();
        }
    }
}