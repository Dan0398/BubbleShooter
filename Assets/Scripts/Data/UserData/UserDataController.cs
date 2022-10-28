#if UNITY_WEBGL
    using Task = Cysharp.Threading.Tasks.UniTask;
#else
    using Task = System.Threading.Tasks.Task;
#endif

namespace Data{
    public class UserDataController : BaseDataController
    {
        public UserData Data {get; private set; }
        protected override bool IsDataImportant => true;
        protected override AbstractData MyData => Data; 
        public UserDataController(): base()
        {
            
        }
        
        protected override void SubscribeOnChanges()
        {
            Data.OnLevelsChanged += base.SaveData;
        }
        
        protected override async Task LoadData()
        {
            Data = await Services.IO.LoadData<UserData>(IsDataImportant);
            if (Data == null)
            {
                Data = new UserData();
            }
        }
    }
}