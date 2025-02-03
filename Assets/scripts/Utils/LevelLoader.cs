using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AsyncSceneLoaderWithCheck : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private Vector3 scenePosition = Vector3.zero;

    private bool isLoading = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isLoading && !IsSceneLoaded(sceneToLoad))
        {
            Debug.Log("Начинаем загрузку сцены: " + sceneToLoad);
            StartCoroutine(LoadSceneAsync(sceneToLoad));
        }
        else if (IsSceneLoaded(sceneToLoad))
        {
            Debug.Log("Сцена уже загружена!");
        }
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        isLoading = true;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            Debug.Log($"Загрузка: {operation.progress * 100}%");
            yield return null;
        }

        operation.allowSceneActivation = true;
        yield return new WaitForSeconds(0.1f);

        MoveSceneObjects(sceneName, scenePosition);
        isLoading = false;
    }

    void MoveSceneObjects(string sceneName, Vector3 newPosition)
    {
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        if (!loadedScene.IsValid())
        {
            Debug.LogError("Ошибка: сцена не найдена!");
            return;
        }

        foreach (GameObject obj in loadedScene.GetRootGameObjects())
        {
            obj.transform.position += newPosition;
        }

        Debug.Log("Сцена перемещена на координаты: " + newPosition);
    }

    bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName)
                return true;
        }
        return false;
    }
}
