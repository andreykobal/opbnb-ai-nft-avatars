using UnityEngine;
using UnityEngine.UIElements;

public class DiamondCollector : MonoBehaviour
{
    public int diamondCount = 10;
    public AudioClip diamondCollectSound;
    public UIDocument uiDocument;

    private int collectedDiamonds = 0;
    private AudioSource audioSource;
    private VisualElement claimElement;
    private Label diamondsLabel;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        claimElement = uiDocument.rootVisualElement.Q("Claim");
        claimElement.visible = false;

        diamondsLabel = uiDocument.rootVisualElement.Q<Label>("Diamonds");
        UpdateDiamondsLabel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Diamond"))
        {
            Destroy(other.gameObject);
            collectedDiamonds++;

            // Play the diamond collect sound
            if (audioSource != null && diamondCollectSound != null)
            {
                audioSource.PlayOneShot(diamondCollectSound);
            }

            UpdateDiamondsLabel();

            if (collectedDiamonds == diamondCount)
            {
                Debug.Log("Win!");

                // Show the #Claim element
                claimElement.visible = true;

                // Unlock the cursor
                UnityEngine.Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    private void UpdateDiamondsLabel()
    {
        diamondsLabel.text = $"{collectedDiamonds}/{diamondCount}";
    }
}