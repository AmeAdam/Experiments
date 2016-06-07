using System;
using System.IO;
using Newtonsoft.Json;

namespace AmeCommon.Settings
{
    public interface ICommonSettingsProvider
    {
        T GetSettings<T>(string name = null) where T : new();
        void SaveUserSettings<T>(T settings, string name = null);
        T GetUserSettings<T>(string name = null) where T : new();
    }

    public class CommonSettingsProvider : ICommonSettingsProvider
    {
        protected string BaseDir => AppDomain.CurrentDomain.BaseDirectory;
        protected string UserDir => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AME");

        public T GetSettings<T>(string name = null) where T : new()
        {
            name = name ?? typeof(T).Name;
            var path = Path.Combine(BaseDir, name + ".json");
            return GetSettingsInternal<T>(path);
        }

        public void SaveUserSettings<T>(T settings, string name = null) 
        {
            name = name ?? typeof(T).Name;
            var path = Path.Combine(UserDir, name + ".json");
            SaveSettingsInternal(path, settings);
        }

        public T GetUserSettings<T>(string name = null) where T : new()
        {
            name = name ?? typeof(T).Name;
            var path = Path.Combine(UserDir, name + ".json");
            return GetSettingsInternal<T>(path);
        }

        protected T GetSettingsInternal<T>(string filePath) where T : new()
        {
            if (!File.Exists(filePath))
                return new T();
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
        }

        protected void SaveSettingsInternal<T>(string filePath, T settings)
        {
            var content = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(filePath, content);
        }
    }
}