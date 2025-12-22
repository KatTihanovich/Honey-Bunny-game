using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GameAPIManager : MonoBehaviour
{
    private const string BASE_URL = "http://localhost:8080/api";
    private string jwtToken;
    private long currentUserId;

    public static GameAPIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator Register(string userNickname, string userPassword, int userAge, Action<bool, string> callback)
{
    RegisterRequest data = new RegisterRequest
    {
        nickname = userNickname,
        password = userPassword,
        age = userAge
    };

    string json = JsonUtility.ToJson(data);
    yield return PostRequest("/users/register", json, (success, response) =>
    {
        if (success)
        {
            AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(response);
            jwtToken = authResponse.token;
            currentUserId = authResponse.user.id; // ← Изменено
            
            Debug.Log("Регистрация успешна! userId: " + currentUserId);
            
            PlayerPrefs.SetString("JWT_Token", jwtToken);
            PlayerPrefs.SetInt("User_ID", (int)currentUserId);
            PlayerPrefs.Save();
        }
        callback(success, response);
    });
}

public IEnumerator Login(string userNickname, string userPassword, Action<bool, string> callback)
{
    LoginRequest data = new LoginRequest
    {
        nickname = userNickname,
        password = userPassword
    };

    string json = JsonUtility.ToJson(data);
    yield return PostRequest("/users/login", json, (success, response) =>
    {
        if (success)
        {
            AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(response);
            jwtToken = authResponse.token;
            currentUserId = authResponse.user.id; // ← Изменено
            
            Debug.Log("Логин успешен! userId: " + currentUserId);
            
            PlayerPrefs.SetString("JWT_Token", jwtToken);
            PlayerPrefs.SetInt("User_ID", (int)currentUserId);
            PlayerPrefs.Save();
        }
        callback(success, response);
    });
}


    // Сохранение прогресса
    public IEnumerator SaveProgress(long levelId, int killedEnemies, int solvedPuzzles, 
                                     string timeSpent, int stars, Action<bool, string> callback)
    {
        Progress data = new Progress
        {
            userId = currentUserId,
            levelId = levelId,
            killedEnemiesNumber = killedEnemies,
            solvedPuzzlesNumber = solvedPuzzles,
            timeSpent = timeSpent,
            stars = stars,
            createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
        };

        string json = JsonUtility.ToJson(data);
        yield return PostRequestWithAuth($"/progress?userId={currentUserId}", json, callback);
    }

    // Получение последнего прогресса по уровню
    public IEnumerator GetLatestProgress(long levelId, Action<bool, Progress> callback)
    {
        yield return GetRequestWithAuth($"/progress/{currentUserId}/level/{levelId}/latest", (success, response) =>
        {
            if (success)
            {
                Progress progress = JsonUtility.FromJson<Progress>(response);
                callback(true, progress);
            }
            else
            {
                callback(false, null);
            }
        });
    }

    // Получение статистики игрока
    public IEnumerator GetUserStatistics(Action<bool, UserStatistics> callback)
    {
        string endpoint = $"/statistics/{currentUserId}";
            Debug.Log("=== СТАТИСТИКА ===");
            Debug.Log("currentUserId: " + currentUserId);
            Debug.Log("endpoint: " + endpoint);
            Debug.Log("Полный URL: " + BASE_URL + endpoint);
        yield return GetRequestWithAuth(endpoint, (success, response) =>
        {
            
            if (success)
            {
                UserStatistics stats = JsonUtility.FromJson<UserStatistics>(response);
                callback(true, stats);
            }
            else
            {
                callback(false, null);
            }
        });
    }

    // Получение достижений пользователя
public IEnumerator GetUserAchievements(Action<bool, Achievement[]> callback)
{
    string endpoint = $"/achievements/user/{currentUserId}";
    Debug.Log("=== ДОСТИЖЕНИЯ ===");
    Debug.Log("currentUserId: " + currentUserId);
    Debug.Log("Полный URL: " + BASE_URL + endpoint);
    
    yield return GetRequestWithAuth(endpoint, (success, response) =>
    {
        Debug.Log("Success: " + success);
        Debug.Log("Response: " + response);
        
        if (success)
        {
            Debug.Log("Попытка парсинга достижений...");
            string wrappedJson = "{\"achievements\":" + response + "}";
            Debug.Log("Wrapped JSON: " + wrappedJson);
            
            AchievementsWrapper wrapper = JsonUtility.FromJson<AchievementsWrapper>(wrappedJson);
            Debug.Log("Количество достижений: " + (wrapper.achievements != null ? wrapper.achievements.Length : 0));
            
            callback(true, wrapper.achievements);
        }
        else
        {
            Debug.LogError("Ошибка получения достижений: " + response);
            callback(false, null);
        }
    });
}

    // POST запрос без аутентификации
    private IEnumerator PostRequest(string endpoint, string jsonData, Action<bool, string> callback)
    {
        UnityWebRequest request = new UnityWebRequest(BASE_URL + endpoint, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            callback(true, request.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"Error: {request.error}");
            callback(false, request.error);
        }
    }

    // POST запрос с JWT токеном
    private IEnumerator PostRequestWithAuth(string endpoint, string jsonData, Action<bool, string> callback)
    {
        UnityWebRequest request = new UnityWebRequest(BASE_URL + endpoint, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + jwtToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            callback(true, request.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"Error: {request.error}");
            callback(false, request.error);
        }
    }

    // GET запрос с JWT токеном
    private IEnumerator GetRequestWithAuth(string endpoint, Action<bool, string> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(BASE_URL + endpoint);
        request.SetRequestHeader("Authorization", "Bearer " + jwtToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            callback(true, request.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"Error: {request.error}");
            callback(false, request.error);
        }
    }

    public long GetCurrentUserId() => currentUserId;
    public string GetJWTToken() => jwtToken;
}