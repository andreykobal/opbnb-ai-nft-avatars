using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AvatarChanger : MonoBehaviour
{
    public Avatar[] avatars; // The avatars to be assigned
    public GameObject[] avatarModelPrefabs; // The avatar model prefabs

    private Animator animator;
    private GameObject currentAvatarModel;
    public int desiredAvatarIndex = 0;

    private void Start()
    {
        animator = GetComponent<Animator>();
        InstantiateAvatarModel(UI.DesiredAvatarIndex);
        UpdateAvatar(UI.DesiredAvatarIndex);
        SetActiveAvatarModel(UI.DesiredAvatarIndex);
    }

    private void InstantiateAvatarModel(int avatarIndex)
    {
        if (currentAvatarModel != null)
        {
            Destroy(currentAvatarModel);
        }

        currentAvatarModel = Instantiate(avatarModelPrefabs[avatarIndex], transform.position, transform.rotation, transform);
    }

    private void SetActiveAvatarModel(int activeIndex)
    {
        for (int i = 0; i < avatarModelPrefabs.Length; i++)
        {
            if (i == activeIndex)
            {
                currentAvatarModel.SetActive(true);
            }
            else
            {
                Destroy(avatarModelPrefabs[i]);
            }
        }
    }

    private void UpdateAvatar(int avatarIndex)
    {
        // Assign the new avatar to the Animator component
        animator.avatar = avatars[avatarIndex];
    }

    private void Update()
    {
        // Check for Escape key press using the new Input System
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ReturnToPreviousScene();
        }
    }

    private void ReturnToPreviousScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int previousSceneIndex = (currentSceneIndex - 1 + SceneManager.sceneCountInBuildSettings) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(previousSceneIndex);
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }
}