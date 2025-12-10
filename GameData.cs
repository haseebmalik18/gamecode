using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    public int CurrentScore { get; set; }
    public int HighestScoreThisSession { get; set; }
    public int TotalJokersPopped { get; set; }
    public int GamesPlayed { get; set; }
    public string PlayerName { get; set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadData()
    {
        HighestScoreThisSession = 0;
        TotalJokersPopped = PlayerPrefs.GetInt("TotalJokersPopped", 0);
        GamesPlayed = PlayerPrefs.GetInt("GamesPlayed", 0);
        PlayerName = PlayerPrefs.GetString("PlayerName", "BATMAN");
        CurrentScore = 0;
    }

    public void StartNewGame()
    {
        CurrentScore = 0;
        GamesPlayed++;
        PlayerPrefs.SetInt("GamesPlayed", GamesPlayed);
        PlayerPrefs.Save();
    }

    public void AddScore(int points)
    {
        CurrentScore += points;
        if (CurrentScore > HighestScoreThisSession)
            HighestScoreThisSession = CurrentScore;
    }

    public void AddJokerPopped()
    {
        TotalJokersPopped++;
        PlayerPrefs.SetInt("TotalJokersPopped", TotalJokersPopped);
        PlayerPrefs.Save();
    }

    public void SetPlayerName(string name)
    {
        PlayerName = name;
        PlayerPrefs.SetString("PlayerName", name);
        PlayerPrefs.Save();
    }

    public void GameOver()
    {
        HighScoreManager.Instance?.TryAddScore(PlayerName, CurrentScore);
    }
}
