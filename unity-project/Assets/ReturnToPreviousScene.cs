using UnityEngine;
using UnityEngine.InputSystem;

public class ReturnToPreviousScene : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ReturnToPreviousSceneInBuildSettings();
        }
    }

    private void ReturnToPreviousSceneInBuildSettings()
    {
        int previousSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1;

        if (previousSceneIndex >= 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(previousSceneIndex);
        }
    }
}