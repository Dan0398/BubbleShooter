using UnityEngine;
#if UNITY_WEBGL
    using Task = Cysharp.Threading.Tasks.UniTask;
#else
    using Task = System.Threading.Tasks.Task;
#endif

namespace Data{
    public abstract class BaseDataController : Services.IService
    {
        protected abstract bool IsDataImportant {get;}
        public bool isDataLoaded {get; private set;}
        protected abstract AbstractData MyData {get;}
        bool SavingInvoked;
        
        public BaseDataController()
        {
            SavingInvoked = false;
            PreloadData();
        }
        
        async void PreloadData()
        {
            await LoadData();
            if (MyData.isNewData)
            {
                MyData.SetDefaults();
                SaveData();
            }
            SubscribeOnChanges();
            isDataLoaded = true;
        }
        
        protected abstract Task LoadData();
        
        protected abstract void SubscribeOnChanges();
        
        protected async void SaveData()
        {
            if (SavingInvoked) return;
            SavingInvoked = true;
            await Task.Delay(3000);
            Services.IO.SaveData(MyData, IsDataImportant);
            SavingInvoked = false;
        }
    }
}