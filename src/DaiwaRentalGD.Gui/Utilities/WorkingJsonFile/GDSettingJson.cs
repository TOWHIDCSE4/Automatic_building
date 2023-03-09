using DaiwaRentalGD.Gui.Utilities.WorkingJsonFile.JsonModel;
using Newtonsoft.Json;
using System.IO;

namespace DaiwaRentalGD.Gui.Utilities.WorkingJsonFile
{
    public abstract class GDSettingJson<T>
    {
        protected string JsonPath { get; set; }

        public GDSettingJson(string jsonPath)
        {
            JsonPath = jsonPath;
        }

        protected T ReadJsonFile()
        {
            string jsonString = File.ReadAllText(JsonPath);
            T obj = JsonConvert.DeserializeObject<T>(jsonString);
            return obj;
        }

        public abstract void Process(); 
    }
}
