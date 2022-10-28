using System.Collections.Generic;

namespace Services{
    public class BackButton: Services.IService
    {
        Stack<System.Action> BackActions;
        
        public BackButton()
        {
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
                BackActions.Peek()?.Invoke();
            }
            else 
            {
                UnityEngine.Debug.Log("Вызов меню подтверждения выхода");
            }
        }
    }
}