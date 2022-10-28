using Newtonsoft.Json;

namespace Data{
    public class UserData: AbstractData
    {
        [System.Serializable]
        public enum LevelStatus
        {
            Locked = -1,
            Available = 0,
            Complete1Star = 1,
            Complete2Star = 2,
            Complete3Star = 3
        }
        
        [JsonRequired] LevelStatus[] levels;
        [JsonIgnore] public LevelStatus[] Levels
        {
            get => levels;
            set 
            {
                levels = value;
                OnLevelsChanged?.Invoke();
            }
        }
        [JsonIgnore] public System.Action OnLevelsChanged;
        
        public UserData(): base()
        {
            Levels = new LevelStatus[5];
        }

        protected override void SetCustomDefaults()
        {
            Levels[0] = LevelStatus.Available;
            for (int i = 1; i < Levels.Length; i ++)
            {
                Levels[i] = LevelStatus.Locked;
            }
        }
    }
}