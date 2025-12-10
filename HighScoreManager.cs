using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ScoreEntry
{
    public string playerName;
    public int score;

    public ScoreEntry(string name, int s)
    {
        playerName = name;
        score = s;
    }
}

[System.Serializable]
public class HighScoreData
{
    public List<ScoreEntry> scores = new List<ScoreEntry>();
}

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance { get; private set; }

    public List<ScoreEntry> HighScores { get; private set; } = new List<ScoreEntry>();
    const int MaxScores = 5;
    const string SaveKey = "HighScoresV2";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScores();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadScores()
    {
        string json = PlayerPrefs.GetString(SaveKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            HighScoreData data = JsonUtility.FromJson<HighScoreData>(json);
            if (data != null && data.scores != null)
                HighScores = data.scores;
        }
    }

    void SaveScores()
    {
        HighScoreData data = new HighScoreData();
        data.scores = HighScores;
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public bool TryAddScore(string playerName, int score)
    {
        if (score <= 0) return false;

        int insertIndex = -1;
        for (int i = 0; i < HighScores.Count; i++)
        {
            if (score > HighScores[i].score)
            {
                insertIndex = i;
                break;
            }
        }

        if (insertIndex == -1 && HighScores.Count < MaxScores)
            insertIndex = HighScores.Count;

        if (insertIndex >= 0 && insertIndex < MaxScores)
        {
            HighScores.Insert(insertIndex, new ScoreEntry(playerName, score));
            if (HighScores.Count > MaxScores)
                HighScores.RemoveAt(HighScores.Count - 1);
            SaveScores();
            return true;
        }

        return false;
    }

    public bool IsHighScore(int score)
    {
        if (HighScores.Count < MaxScores) return score > 0;
        return score > HighScores[HighScores.Count - 1].score;
    }

    public void ClearScores()
    {
        HighScores.Clear();
        PlayerPrefs.DeleteKey(SaveKey);
        LoadScores();
    }
}
