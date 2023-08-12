using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Public variable to set the UI Document from the inspector
    public UIDocument uiDocument;

    private void OnEnable()
    {
        // Access the root visual element
        var root = uiDocument.rootVisualElement;

        // Find the button called "Play"
        var playButton = root.Q<Button>("Play");
        if (playButton != null)
        {
            // Add click event listener
            playButton.clicked += LoadNextScene;
        }
    }

    private void Update()
    {
        // Check for the Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadPreviousScene();
        }
    }

    // Function to load the next scene in the build settings
    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
    }

    // Function to load the previous scene in the build settings
    private void LoadPreviousScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int previousSceneIndex = currentSceneIndex - 1;
        if (previousSceneIndex < 0)
        {
            previousSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
        }
        SceneManager.LoadScene(previousSceneIndex);
    }
}