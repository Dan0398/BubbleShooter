using UnityEngine;
using DIService = Services.DI;

namespace Services
{
    public class Registrator : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        static void RegisterAllSystems()
        {
            DIService.Register<BackButton>(new BackButton());
        }
    }
}