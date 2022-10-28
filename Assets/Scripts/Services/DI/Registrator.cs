using UnityEngine;
using DIService = Services.DI;

namespace Services
{
    public class Registrator : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        static void RegisterAllSystems()
        {
            DIService.Register<Content.LevelsHolder>(new Content.LevelsHolder());
            DIService.Register<Data.UserDataController>(new Data.UserDataController());
            DIService.Register<BackButton>(new BackButton());
        }
    }
}