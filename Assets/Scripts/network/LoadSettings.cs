using System.IO;
using UnityEngine;

[System.Serializable]
public class Settings
{
    public string serverAddress;
    public int serverPort;

    /// <summary>
    /// Get settings from .json file or generate it with default params.
    /// </summary>
    /// <param name="serverAddress">Standart index.html url for first generate .json</param>
    /// <param name="serverPort">Standart "pong" hub url for first generate .json</param>
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