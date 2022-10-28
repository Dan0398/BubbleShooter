using System.Collections.Generic;

namespace Services{
    public class BackButton: Services.IService
    {
        Stack<System.Action> BackActions;
        
        public BackButton()
        {
            var Inputs = new Controls();
            Inputs.Enable();
            Inputs.BaseMap.Back.performed += (s) => GoBack();
            BackActions = new Stack<System.Action>();
        }
        
        public void RegisterBackAction(System.Action OnBackClicked)
        {
            BackActions.Push(OnBackClicked);
        }
        
        public void GoBack()
        {
            if (BackActions.Count > 0)
            {
                BackActions.Pop()?.Invoke();
            }
            else 
            {
                Services.DI.Single<UI.Settings.Settings>().ShowSettings();
            }
        }
        
        public void Cleanup()
        {
            BackActions.Clear();
        }
    }
}