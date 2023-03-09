using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DaiwaRentalGD.Gui.Utilities.WorkingJsonFile
{
    public abstract class GDOutputJson<T>
    {
        protected T Data { get; set; }
        public GDOutputJson()
        {

        }
        public abstract void GetData();

        public void SaveJson(string path)
        {
            string json = JsonConvert.SerializeObject(Data);
            System.IO.File.WriteAllText(path, json);
        }
    }
}
