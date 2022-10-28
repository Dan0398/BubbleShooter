using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class DI : MonoBehaviour
    {
        static Dictionary<System.Type, IService> AllSystems;
        
        static DI()
        {
            AllSystems = new Dictionary<System.Type, IService>();//
        }
        
        public static TService Single<TService>() where TService: IService
        {
            if (AllSystems.TryGetValue(typeof(TService), out IService Value))
            {
                return (TService) Value;
            }
            Debug.LogError("Попытка вызвать систему \"" + typeof(TService).ToString() + "\". Система не найдена.");
            return default(TService);
        }
        
        public static void Register<TService>(IService Service) where TService: IService
        {
            if (AllSystems.TryGetValue(typeof(TService), out IService Value))
            {
                AllSystems[typeof(TService)] = Service;
            }
            else 
            {
                AllSystems.Add(typeof(TService), Service);
            }
        }
    }
}