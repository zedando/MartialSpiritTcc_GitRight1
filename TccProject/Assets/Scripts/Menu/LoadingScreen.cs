using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar;
    private float targetProgress = 0f;

    void Start()
    {
        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        if (string.IsNullOrEmpty(Loader.SceneToLoad))
        {
            Debug.LogError("SceneToLoad is empty! Please set it before loading.");
            yield break;
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(Loader.SceneToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            targetProgress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = Mathf.MoveTowards(progressBar.value, targetProgress, Time.deltaTime);

            if (progressBar.value >= 0.99f && operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
