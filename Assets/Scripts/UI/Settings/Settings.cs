using UnityEngine;

namespace UI.Settings{
    public class Settings : MonoBehaviour, Services.IService
    {
        [SerializeField] GameObject RestartButton, ToMainMenuButton;
        public bool isGameplay;
        
        void Start()
        {
            SelfRegister();
            HideWindow();
        }
        
        void SelfRegister()
        {
            Services.DI.Register<UI.Settings.Settings>(this);
        }
        
        void HideWindow()
        {
            Services.DI.Single<UI.Menu.MainMenu>().gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
        
        public void ShowSettings()
        {
            RestartButton.SetActive(isGameplay);
            ToMainMenuButton.SetActive(isGameplay);
            gameObject.SetActive(true);
            Services.DI.Single<UI.Menu.MainMenu>().gameObject.SetActive(false);
            Services.DI.Single<Services.BackButton>().RegisterBackAction(HideWindow);
        }
        
        public void PickContinueButton()
        {
            Services.DI.Single<Services.BackButton>().GoBack();
        }
        
        public void PickRestartButton()
        {
            Services.DI.Single<Gameplay.GameplayController>().RestartGameplay();
            PickContinueButton();
        }
        
        public void PickToMainMenuButton()
        {
            Services.DI.Single<Gameplay.GameplayController>().StopGameplay();
            PickContinueButton();
        }
        
        public void PickQuitButton()
        {
            Application.Quit();
        }
    }
}