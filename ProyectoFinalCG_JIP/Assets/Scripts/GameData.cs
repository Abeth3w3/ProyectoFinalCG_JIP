using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // Puntuación y progreso
    public int totalScore;
    public int highScore;
    public int totalDeaths;
    public int enemiesKilled;

    // Tiempos
    public float totalPlayTime;
    public float currentLevelTime;
    public List<LevelTime> levelTimes;

    // Estadísticas de juego
    public int burgersThrown;
    public int drinksUsed;
    public int levelsCompleted;

    // Inventario
    public int burgerCount;
    public int drinkCount;
    public int coins;

    // Configuración
    public float musicVolume;
    public float sfxVolume;
    public bool tutorialCompleted;

    // Metadata
    public string lastSaveDate;
    public string currentLevel;
}

[System.Serializable]
public class LevelTime
{
    public string levelName;
    public float completionTime;
    public string dateCompleted;
}