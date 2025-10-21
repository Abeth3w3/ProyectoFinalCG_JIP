using System;
using System.IO;
using UnityEngine;

[Serializable]
public class PlayerStats
{
    public float tiempoJugado; // En minutos
    public int score;

    private static string filePath => Application.persistentDataPath + "/playerStats.json";

    public static void Save(PlayerStats stats)
    {
        string json = JsonUtility.ToJson(stats, true);
        File.WriteAllText(filePath, json);
    }

    public static PlayerStats Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<PlayerStats>(json);
        }
        return new PlayerStats();
    }
}
