using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [SerializeField] private Slider coinSlider;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private int maxCoins = 100;

    public int totalCoins = 0;

    // Хранит список собранных монет для каждого уровня
    private Dictionary<string, HashSet<string>> _collectedCoinsPerLevel = new Dictionary<string, HashSet<string>>();
    private const string _collectedCoinsKey = "CollectedCoinsPerLevel";

    // Хранит общее количество монет, собранных на каждом уровне
    private Dictionary<string, int> _levelCoinCounts = new Dictionary<string, int>();
    private const string _levelCoinsKey = "LevelCoins";

    // Имя текущего уровня
    private string CurrentLevel => SceneManager.GetActiveScene().name;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadCollectedCoins(); // Загружаем ранее собранные монеты
            LoadLevelCoinCounts(); // Загружаем количество монет по уровням

            // Если на этом уровне уже были монеты — восстанавливаем их количество
            if (_levelCoinCounts.TryGetValue(CurrentLevel, out int levelCoins))
            {
                totalCoins = levelCoins;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (coinSlider != null)
        {
            coinSlider.maxValue = maxCoins;
            coinSlider.value = totalCoins;
        }
        UpdateCoinText();
    }

    // Увеличивает количество монет и обновляет прогресс UI
    public void AddCoins(int amount)
    {
        totalCoins += amount;
        UpdateSlider();
        UpdateCoinText();

        // Обновляем количество монет для текущего уровня и сохраняем
        _levelCoinCounts[CurrentLevel] = totalCoins;
        SaveLevelCoinCounts();
    }

    private void UpdateSlider()
    {
        if (coinSlider != null)
        {
            coinSlider.value = totalCoins;
        }
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = $"{totalCoins}/{maxCoins}";
        }
    }

    // Проверяет, была ли уже собрана монета с указанным ID на этом уровне
    public bool IsCoinCollected(string coinId)
    {
        if (_collectedCoinsPerLevel.TryGetValue(CurrentLevel, out var collectedSet))
        {
            return collectedSet.Contains(coinId);
        }
        return false;
    }

    // Помечает монету как собранную и сохраняет данные
    public void MarkCoinCollected(string coinId)
    {
        if (!_collectedCoinsPerLevel.ContainsKey(CurrentLevel))
        {
            _collectedCoinsPerLevel[CurrentLevel] = new HashSet<string>();
        }

        // Добавляем монету и сохраняем, только если её ещё не было
        if (_collectedCoinsPerLevel[CurrentLevel].Add(coinId))
        {
            SaveCollectedCoins();
        }
    }

    // Сохраняет список собранных монет по уровням в PlayerPrefs
    private void SaveCollectedCoins()
    {
        var data = new CollectedCoinData();

        foreach (var kvp in _collectedCoinsPerLevel)
        {
            data.levels.Add(new LevelCoinSet
            {
                levelName = kvp.Key,
                collectedCoinIds = new List<string>(kvp.Value)
            });
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(_collectedCoinsKey, json);
        PlayerPrefs.Save();
    }

    // Загружает список собранных монет из PlayerPrefs
    private void LoadCollectedCoins()
    {
        if (PlayerPrefs.HasKey(_collectedCoinsKey))
        {
            string json = PlayerPrefs.GetString(_collectedCoinsKey);
            var data = JsonUtility.FromJson<CollectedCoinData>(json);

            _collectedCoinsPerLevel.Clear();
            foreach (var levelData in data.levels)
            {
                _collectedCoinsPerLevel[levelData.levelName] = new HashSet<string>(levelData.collectedCoinIds);
            }
        }
    }

    // Сохраняет количество монет, собранных на каждом уровне
    private void SaveLevelCoinCounts()
    {
        var data = new LevelCoinDataWrapper
        {
            levelCoins = new List<LevelCoinEntry>()
        };

        foreach (var kvp in _levelCoinCounts)
        {
            data.levelCoins.Add(new LevelCoinEntry { levelName = kvp.Key, coinCount = kvp.Value });
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(_levelCoinsKey, json);
        PlayerPrefs.Save();
    }

    // Загружает количество монет по уровням
    private void LoadLevelCoinCounts()
    {
        if (PlayerPrefs.HasKey(_levelCoinsKey))
        {
            string json = PlayerPrefs.GetString(_levelCoinsKey);
            var data = JsonUtility.FromJson<LevelCoinDataWrapper>(json);
            _levelCoinCounts.Clear();
            foreach (var entry in data.levelCoins)
            {
                _levelCoinCounts[entry.levelName] = entry.coinCount;
            }
        }
    }

    // Классы-обёртки для сериализации данных о собранных монетах
    [System.Serializable]
    private class CollectedCoinData
    {
        public List<LevelCoinSet> levels = new List<LevelCoinSet>();
    }

    [System.Serializable]
    private class LevelCoinSet
    {
        public string levelName;
        public List<string> collectedCoinIds;
    }

    // Классы-обёртки для сериализации количества монет по уровням
    [System.Serializable]
    private class LevelCoinDataWrapper
    {
        public List<LevelCoinEntry> levelCoins;
    }

    [System.Serializable]
    private class LevelCoinEntry
    {
        public string levelName;
        public int coinCount;
    }
}
