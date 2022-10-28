using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay{
    public class GameplayController : MonoBehaviour, Services.IService
    {
        [SerializeField] bool TestSample;
        [SerializeField] GameObject BubbleSample;
        [SerializeField] GameField Field;
        [SerializeField] Pool Pool;
        [SerializeField] PlayerActions Actions;
        [SerializeField] BubblesQueue Queue;
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
            if (TestSample)
            {
                StartEndless();
            }
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
            Inputs.BaseMap.SwitchBubbles.performed += (s) => Actions.SwitchBubble();
        }
        
        public void StartEndless()
        {
            Inputs.Enable();
            Field.CreateFieldEndless();
            Actions.StartGameplay();
        }
    }
}