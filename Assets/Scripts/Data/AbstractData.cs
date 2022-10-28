using Newtonsoft.Json;

namespace Data{
    public abstract class AbstractData
    {
        [JsonRequired] public bool isNewData;
        
        public AbstractData()
        {
            isNewData = true;
        }
        
        public void SetDefaults()
        {
            isNewData = false;
            SetCustomDefaults();
        }
        
        protected abstract void SetCustomDefaults();
    }
}