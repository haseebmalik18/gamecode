using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float EnemySpeedMultiplier { get; private set; } = 1f;
    public float GrowthMultiplier { get; private set; } = 1f;
    public float PoliceSpeedMultiplier { get; private set; } = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ApplyDifficulty();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = savedVolume;
    }

    void ApplyDifficulty()
    {
        int difficulty = PlayerPrefs.GetInt("Difficulty", 1);

        switch (difficulty)
        {
            case 0:
                EnemySpeedMultiplier = 0.7f;
                GrowthMultiplier = 0.7f;
                PoliceSpeedMultiplier = 0.7f;
                break;
            case 1:
                EnemySpeedMultiplier = 1f;
                GrowthMultiplier = 1f;
                PoliceSpeedMultiplier = 1f;
                break;
            case 2:
                EnemySpeedMultiplier = 1.5f;
                GrowthMultiplier = 1.5f;
                PoliceSpeedMultiplier = 1.5f;
                break;
        }
    }
}
