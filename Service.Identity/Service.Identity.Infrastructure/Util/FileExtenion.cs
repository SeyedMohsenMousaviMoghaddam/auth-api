using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Identity.Infrastructure.Util;
public static class FileExtenion
{
    public static T DeserializeJsonFromFile<T>(string path)
    {
        string fullPath, appDomain = AppDomain.CurrentDomain.BaseDirectory;
        if (appDomain.Contains("bin"))
        {
            string rootDirectory = appDomain.Substring(0, appDomain.Length - 18);
            fullPath = $"{rootDirectory}/{path}";
        }
        else
        {
            fullPath = path;
        }

        using (StreamReader file = File.OpenText(fullPath))
        {
            string json = file.ReadToEnd();
            //var settings = new JsonSerializerSettings
            //{
            //    ContractResolver = new PrivateSetterContractResolver(),
            //};

            return JsonConvert.DeserializeObject<T>(json);
        }
    }

}