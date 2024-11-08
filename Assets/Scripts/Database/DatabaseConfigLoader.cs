using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class DatabaseConfigLoader
{
    public static JObject LoadConfig()
    {
        string configPath = Path.Combine(Application.streamingAssetsPath, "config.json");
        string jsonContent = File.ReadAllText(configPath);
        return JObject.Parse(jsonContent);
    }
}