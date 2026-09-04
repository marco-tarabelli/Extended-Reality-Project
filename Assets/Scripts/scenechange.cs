using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
    Debug.Log($"Try to load the scene: {sceneName}");
    SceneManager.LoadScene(sceneName);
    }
    

}
