using UnityEngine;

namespace Gameplay{
    public class GameplayController : MonoBehaviour, Services.IService
    {
        [SerializeField] GameObject BubbleSample;
        [SerializeField] GameField Field;
        [SerializeField] Pool Pool;
        [SerializeField] PlayerActions Actions;
        [SerializeField] BubblesQueue Queue;
        System.Action OnSuccessEnded;
        System.Action OnRestart;
        Controls Inputs;
        
        void Start()
        {
            SelfRegister();
            Field.Init(transform);
            Pool = new Pool(BubbleSample, Field.BubbleSize,  transform);
            Field.ApplyPool(Pool);
            Queue.Init(transform, Pool, Field.BubbleSize);
            Actions.Init(Field.BubbleSize, Queue, Field);
            ApplyInputs();
        }
        
        void SelfRegister()
        {
            Services.DI.Register<GameplayController>(this);
        }
        
        void ApplyInputs()
        {
            Inputs = new Controls();
            Inputs.BaseMap.ClickPosition.performed += (s) => Actions.ApplyPointerPos(s.ReadValue<Vector2>());
            Inputs.BaseMap.Clicked.canceled += (s) => Actions.ShootBubble();
            Inputs.BaseMap.SwitchBubbles.performed += (s) => Actions.SwitchBubbleToSecondary();
        }
        
        public void StartEndless()
        {
            OnRestart = ()=> 
            {
                Inputs.Enable();
                Field.CreateFieldEndless();
                Queue.SetRandomize();
                Actions.StartGameplay();
                Services.DI.Single<UI.Settings.Settings>().isGameplay = true;
            };
            RestartGameplay();
        }
        
        public void StartLevel(int LevelNumber)
        {
            OnSuccessEnded = ()=> 
            {
                Debug.Log("On success level invoked");
                StopGameplay();
                var User = Services.DI.Single<Data.UserDataController>();
                User.Data.Levels[LevelNumber] = Data.UserData.LevelStatus.Complete1Star;
                if (LevelNumber < User.Data.Levels.Length-1)
                {
                    User.Data.Levels[LevelNumber+1] = Data.UserData.LevelStatus.Available;
                }
                User.Data.OnLevelsChanged?.Invoke();
            };
            var LevelData = Services.DI.Single<Content.LevelsHolder>().Levels[LevelNumber];
            OnRestart = ()=> 
            {
                Inputs.Enable();
                Field.CreateLevelField(LevelData, OnSuccessEnded);
                Queue.SetLevelColorCycle(LevelData);
                Actions.StartGameplay();
                Services.DI.Single<UI.Settings.Settings>().isGameplay = true;    
            };
            RestartGameplay();
        }
        
        public void RestartGameplay()
        {
            OnRestart?.Invoke();
        }
        
        public void StopGameplay()
        {
            Inputs.Disable();
            Field.CleanupLines();
            Queue.Deactivate();
            Actions.StopGameplay(Pool);
            Services.DI.Single<UI.Menu.MainMenu>().ShowBaseWindow();
            Services.DI.Single<UI.Settings.Settings>().isGameplay = false;
        }
    }
}