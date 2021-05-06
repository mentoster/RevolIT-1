using System.IO;
using UnityEngine;

[System.Serializable]
public class Settings
{
    public string serverAddress;
    public int serverPort;


    public Settings(string serverAddress, int serverPort)
    {
        string path = Path.Combine(Application.dataPath, "settings.json");

#if UNITY_EDITOR
        path = Path.Combine(Directory.GetCurrentDirectory(), "settings.json");
#endif

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Settings buffSettings = JsonUtility.FromJson<Settings>(json);
            this.serverAddress = buffSettings.serverAddress;
            this.serverPort = buffSettings.serverPort;
        }
        else
        {
            this.serverPort = serverPort;
            this.serverAddress = serverAddress;
            File.WriteAllText(path, JsonUtility.ToJson(this));
        }
    }
}