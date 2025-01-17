using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GoToDevInfo : MonoBehaviour
{
    public void ChangeScene()
    {
        SceneManager.LoadSceneAsync(3);
    }
}
