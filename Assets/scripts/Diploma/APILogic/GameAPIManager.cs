using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GameAPIManager : MonoBehaviour
{
    private const string BASE_URL = "https://game-progress-api.onrender.com/api";
    private const string NO_INTERNET_MESSAGE = "No internet connection";
    private const int ZERO_CODE = 0;

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

    public IEnumerator Register(string userNickname, string userPassword, int userAge, Action<bool, string, long> callback)
{
    RegisterRequest data = new RegisterRequest
    {
        nickname = userNickname,
        password = userPassword,
        age = userAge
    };

    string json = JsonUtility.ToJson(data);
    yield return PostRequest("/users/register", json, (success, response, code) =>
    {
        if (success)
        {
            AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(response);
            jwtToken = authResponse.token;
            currentUserId = authResponse.user.id; 
            
            Debug.Log("Регистрация успешна! userId: " + currentUserId);
            
            PlayerPrefs.SetString("JWT_Token", jwtToken);
            PlayerPrefs.SetInt("User_ID", (int)currentUserId);
            PlayerPrefs.Save();
        }
        callback(success, response, code);
    });
}

public IEnumerator Login(string userNickname, string userPassword, Action<bool, string, long> callback)
{
    LoginRequest data = new LoginRequest
    {
        nickname = userNickname,
        password = userPassword
    };

    string json = JsonUtility.ToJson(data);
    yield return PostRequest("/users/login", json, (success, response, code) =>
    {
        if (success)
        {
            AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(response);
            jwtToken = authResponse.token;
            currentUserId = authResponse.user.id;
            
            Debug.Log("Логин успешен! userId: " + currentUserId);
            
            PlayerPrefs.SetString("JWT_Token", jwtToken);
            PlayerPrefs.SetInt("User_ID", (int)currentUserId);
            PlayerPrefs.Save();
        }
        callback(success, response, code);
    });
}


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

    public IEnumerator GetLatestProgress(long levelId, Action<bool, Progress> callback)
    {
        yield return GetRequestWithAuth($"/progress/{currentUserId}/level/{levelId}/latest", (success, response, code) =>
        {
            if (success)
            {
                Debug.Log("Success: " + success);
                Debug.Log("Response: " + response);
                Debug.Log("Response Code: " + code);
                
                Progress progress = JsonUtility.FromJson<Progress>(response);
                callback(true, progress);
            }
            else
            {
                callback(false, null);
            }
        });
    }

    public IEnumerator GetUserStatistics(Action<bool, UserStatistics> callback)
    {
        string endpoint = $"/statistics/{currentUserId}";

        yield return GetRequestWithAuth(endpoint,
            (success, response, code) =>
            {
                Debug.Log("Success: " + success);
                Debug.Log("Response: " + response);
                Debug.Log("Response Code: " + code);

                if (success)
                {
                    UserStatistics stats =
                        JsonUtility.FromJson<UserStatistics>(response);

                    callback(true, stats);
                }
                else
                {
                    if (code == 404)
                    {
                        callback(true, null);
                    }
                    else
                    {
                        callback(false, null);
                    }
                }
            });
    }

public IEnumerator GetUserAchievements(Action<bool, Achievement[]> callback)
{
    string endpoint = $"/achievements/user/{currentUserId}";
    Debug.Log("=== ДОСТИЖЕНИЯ ===");
    Debug.Log("currentUserId: " + currentUserId);
    
    yield return GetRequestWithAuth(endpoint, (success, response, code) =>
    {
        Debug.Log("Success: " + success);
        Debug.Log("Response: " + response);
        Debug.Log("Response Code: " + code);
        
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

    private IEnumerator PostRequest(string endpoint, string jsonData, Action<bool, string, long> callback)
    {
        UnityWebRequest request = new UnityWebRequest(BASE_URL + endpoint, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            callback(true, request.downloadHandler.text, request.responseCode);
        }
        else
            {
                if (IsNoInternet(request))
                {
                    Debug.LogError("No internet connection");
                    callback(false, NO_INTERNET_MESSAGE, ZERO_CODE);
                }
                else
                {
                    Debug.LogError($"Error: {request.error}");
                    callback(false, request.error, request.responseCode);
                }
            }

    }

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
            if (IsNoInternet(request))
            {
                Debug.LogError("No internet connection");
                callback(false, NO_INTERNET_MESSAGE);
            }
            else
            {
                Debug.LogError($"Error: {request.error}");
                callback(false, request.error);
            }
        }
            
    }

    private IEnumerator GetRequestWithAuth(
        string endpoint,
        Action<bool, string, long> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(BASE_URL + endpoint);
        request.SetRequestHeader("Authorization", "Bearer " + jwtToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            callback(true, request.downloadHandler.text, request.responseCode);
        }
        else
        {
            if (IsNoInternet(request))
            {
                Debug.LogError("No internet connection");
                callback(false, NO_INTERNET_MESSAGE, ZERO_CODE);
            }
            else
            {
                Debug.LogError($"Error: {request.error}");
                callback(false, request.error, request.responseCode);
            }
        }
    }
    // Delete user account
    public IEnumerator DeleteUser(Action<bool, string> callback)
    {
        string endpoint = $"/users/{currentUserId}";
        
        yield return DeleteRequestWithAuth(endpoint, (success, response) =>
        {
            if (success)
            {
                Debug.Log("User deleted successfully");
                
                // Clear saved credentials
                PlayerPrefs.DeleteKey("JWT_Token");
                PlayerPrefs.DeleteKey("User_ID");
                PlayerPrefs.Save();
                
                jwtToken = null;
                currentUserId = 0;
            }
            callback(success, response);
        });
    }

    // Get stars progress statistics
    public IEnumerator GetStarsProgress(Action<bool, StarsProgress> callback)
    {
        string endpoint = $"/statistics/{currentUserId}/stars-progress";
        
        yield return GetRequestWithAuth(endpoint, (success, response, code) =>
        {
            Debug.Log("Success: " + success);
            Debug.Log("Response: " + response);
            Debug.Log("Response Code: " + code);
            
            if (success)
            {
                StarsProgress starsProgress = JsonUtility.FromJson<StarsProgress>(response);
                callback(true, starsProgress);
            }
            else
            {
                callback(false, null);
            }
        });
    }

    // Update user profile
    public IEnumerator UpdateUser(string newNickname, string newPassword, Action<bool, string> callback)
    {
        UpdateUserRequest data = new UpdateUserRequest
        {
            nickname = newNickname,
            password = newPassword
        };
        
        string json = JsonUtility.ToJson(data);
        string endpoint = $"/users/{currentUserId}";
        
        yield return PutRequestWithAuth(endpoint, json, callback);
    }

    // Helper method for DELETE requests with authentication
    private IEnumerator DeleteRequestWithAuth(string endpoint, Action<bool, string> callback)
    {
        UnityWebRequest request = UnityWebRequest.Delete(BASE_URL + endpoint);
        request.downloadHandler = new DownloadHandlerBuffer();
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

    // Helper method for PUT requests with authentication
    private IEnumerator PutRequestWithAuth(string endpoint, string jsonData, Action<bool, string> callback)
    {
        UnityWebRequest request = new UnityWebRequest(BASE_URL + endpoint, "PUT");
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

    private bool IsNoInternet(UnityWebRequest request)
    {
        return request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.DataProcessingError;
    }

    public long GetCurrentUserId() => currentUserId;
    public string GetJWTToken() => jwtToken;
}