using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Handles all scene loading logic (async, additive, etc.)
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [SerializeField] private string mapSceneName = "2DMap";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // persists across scenes

            // Automatically load the 2D map additively as background
            StartCoroutine(LoadSceneAsync(mapSceneName, true));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName, bool additive = false)
    {
        Debug.Log($"[SceneLoader] Loading scene: {sceneName}");
        AsyncOperation op;

        if (additive)
            op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        else
            op = SceneManager.LoadSceneAsync(sceneName);

        while (!op.isDone)
            yield return null;

        Debug.Log($"[SceneLoader] Scene loaded: {sceneName}");
    }
}
