using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Numerics;

using UnityEngine;
using Web3Unity.Scripts.Library.Ethers.Providers;


public class UI : MonoBehaviour
{
    private static int desiredAvatarIndex = 0;

    public static int DesiredAvatarIndex
    {
        get { return desiredAvatarIndex; }
        set { desiredAvatarIndex = value; }
    }

    public CheckNFTOwnership checkNFTOwnership;
    public MintNFT mintNFT;

    private Button buttonSelect1;
    private Button buttonSelect2;
    private Button buttonSelect3;

    private Label wallet;
    private Label balance; 

    private async void Start()
    {
        string account = PlayerPrefs.GetString("Account");
        var provider = new CustomJsonRpcProvider("https://opbnb-testnet.nodereal.io/v1/64a9df0874fb4a93b9d0a3849de012d3", "5611");
        var getBalance = await provider.GetBalance(account);

        BigInteger weiValue = BigInteger.Parse(getBalance.ToString()); // Example Wei value
        BigInteger divisor = BigInteger.Pow(10, 18);
        decimal etherValue = decimal.Divide((decimal)weiValue, (decimal)divisor);
        
        Debug.LogError("Account Balance in Wei: " + getBalance);
        Debug.Log("$MNT: " + etherValue.ToString("0.0"));


        
        
        

        string formattedString = account.Substring(0, 6) + "..." + account.Substring(account.Length - 4);

        Debug.Log(formattedString);
        
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        wallet = root.Q<Label>("Wallet");
        balance = root.Q<Label>("Balance");

        wallet.text = formattedString;
        balance.text = etherValue.ToString("0.0000");
        

        // Begin checking NFT balances on start
        checkNFTOwnership.CheckNFTBalance("0xd68B7C666b269B3FC9daAc7a3a446bE32999920E", 0);
        checkNFTOwnership.CheckNFTBalance("0x5dA076A7a10560E0d597E131489fDd0Dc28c7951", 1);
        checkNFTOwnership.CheckNFTBalance("0x84797dA18F5C8eD3F04783412568784C6eCa68dB", 2);
        
    }

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        buttonSelect1 = root.Q<Button>("ButtonSelect1");
        buttonSelect2 = root.Q<Button>("ButtonSelect2");
        buttonSelect3 = root.Q<Button>("ButtonSelect3");
        

        buttonSelect1.clicked += () => HandleButtonClick(0);
        buttonSelect2.clicked += () => HandleButtonClick(1);
        buttonSelect3.clicked += () => HandleButtonClick(2);
    }

    private void HandleButtonClick(int avatarIndex)
    {
        string avatarName = "";
        string avatarDescription = "";
        string avatarVoice = "";

        switch (avatarIndex)
        {
            case 0:
                avatarName = "Isabella";
                avatarDescription = "Brilliant hacker and cyberpunk rebel.";
                avatarVoice = "Female";
                break;
            case 1:
                avatarName = "Kaito";
                avatarDescription = "Suave international thief and enigmatic anti-hero.";
                avatarVoice = "Male";
                break;
            case 2:
                avatarName = "Zara";
                avatarDescription = "Skilled private investigator with a no-nonsense attitude.";
                avatarVoice = "Female";
                break;
            default:
                break;
        }

        if (checkNFTOwnership.HasAvatar(avatarIndex))
        {
            PlayerPrefs.SetString("avatarName", avatarName);
            PlayerPrefs.SetString("avatarDescription", avatarDescription);
            PlayerPrefs.SetString("avatarVoice", avatarVoice);

            Debug.Log("Selected Character " + (avatarIndex + 1));
            DesiredAvatarIndex = avatarIndex;
            LoadNextScene();
        }
        else
        {
            Debug.Log("Minting Avatar " + (avatarIndex + 1));
            mintNFT.mintItem(avatarIndex);
        }
    }

    public void UpdateButtonLabel(int avatarIndex, string text)
    {
        Button button = GetButtonForAvatarIndex(avatarIndex);
        button.text = text;

         if (text == "Mint")
        {
            // Set the background color to rgb(68, 31, 132)
            button.style.backgroundColor = new Color(68f / 255f, 31f / 255f, 132f / 255f);
        }
    }

    private Button GetButtonForAvatarIndex(int avatarIndex)
    {
        switch (avatarIndex)
        {
            case 0:
                return buttonSelect1;
            case 1:
                return buttonSelect2;
            case 2:
                return buttonSelect3;
            default:
                return null;
        }
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
    }
}
