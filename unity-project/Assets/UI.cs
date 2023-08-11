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
        var provider = RPC.GetInstance.Provider();
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
        checkNFTOwnership.CheckNFTBalance("0x643D5cf6fdd9Faa3825e194e925D07E290E993D2", 0);
        checkNFTOwnership.CheckNFTBalance("0xEDD3E9751334eC1E4605DC02a0484D6acFe0F69e", 1);
        checkNFTOwnership.CheckNFTBalance("0xEDD3E9751334eC1E4605DC02a0484D6acFe0F69e", 2);
        
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
        if (checkNFTOwnership.HasAvatar(avatarIndex))
        {
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
