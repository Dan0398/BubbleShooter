using UnityEngine;
#if UNITY_WEBGL
    using Task = Cysharp.Threading.Tasks.UniTask;
    using Cysharp.Threading.Tasks;
#else
    using System.Threading.Tasks;
    using System.IO;
#endif
using Newtonsoft.Json;

namespace Services
{
    public static class IO
    {
        public static void SaveData(object Serialized, bool Important = false)
        {
            var JsonString = JsonConvert.SerializeObject(Serialized);
            WriteData(Serialized.GetType().ToString(), JsonString, Important);
        }
        
#if UNITY_WEBGL
        public static async UniTask<TResult> LoadData<TResult>(bool Important = false)
        {
            var RawData = await Services.WebCatcher.GetDataFromServer(typeof(TResult).ToString());
            if (RawData == null || string.IsNullOrEmpty(RawData))
            {
                return (TResult)System.Activator.CreateInstance(typeof(TResult));
            }
            return (TResult) JsonConvert.DeserializeObject(RawData, typeof(TResult));
        }

        static void WriteData(string Name, string Data, bool Important)
        {
            Services.WebCatcher.SendDataToServer(Name, Data, Important);
        }
#endif
    
#if !UNITY_WEBGL

        public static async Task<TResult> LoadData<TResult>(bool Important = false)
        {
            var RawData = await GetData(typeof(TResult).ToString());
            if (RawData == null)
            {
                return (TResult)System.Activator.CreateInstance(typeof(TResult));
            }
            var JsonData = System.Text.Encoding.UTF8.GetString(RawData);
            return (TResult) JsonConvert.DeserializeObject(JsonData, typeof(TResult));
        }
        
        async static Task<byte[]> GetData(string Name)
        {
            string Path = GetFullPath(Name);
            if (!File.Exists(Path))
            {
                return null;
            }
            byte[] Result = null;
            using (var Reader = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Result = new byte[Reader.Length];
                await Reader.ReadAsync(Result, 0, Result.Length);
                Reader.Close();
                Reader.Dispose();
            }
            return Result;
        }
            
        static async void WriteData(string Name, string Data, bool Important)
        {
            var RawData = System.Text.Encoding.UTF8.GetBytes(Data);
            string Path = GetFullPath(Name);
            using (var Writer = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                await Writer.WriteAsync(RawData,0, Data.Length);
                Writer.Close();
                Writer.Dispose();
            }
        }
        
        static string GetFullPath(string FileName) => Application.persistentDataPath + "/" + FileName + ".json";
    #endif
    }
}