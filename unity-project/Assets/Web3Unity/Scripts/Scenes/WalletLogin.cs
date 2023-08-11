using System.Text;
using Nethereum.Signer;
using Nethereum.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Web3Unity.Scripts.Library.Web3Wallet;

public class WalletLogin : MonoBehaviour
{
    public UIDocument uiDocument; // Reference to the UIDocument of the new UI Toolkit
    private Toggle rememberMe;
    private Button loginButton;
    ProjectConfigScriptableObject projectConfigSO = null;

    private void Start()
    {
        var root = uiDocument.rootVisualElement;

        // Fetch UI Elements
        rememberMe = root.Q<Toggle>("RememberMe");
        loginButton = root.Q<Button>("Login");

        // Setup Event Handlers
        loginButton.clicked += OnLogin;

        Web3Wallet.url = "https://chainsafe.github.io/game-web3wallet/";
        projectConfigSO = (ProjectConfigScriptableObject)Resources.Load("ProjectConfigData", typeof(ScriptableObject));
        PlayerPrefs.SetString("ProjectID", projectConfigSO.ProjectID);
        PlayerPrefs.SetString("ChainID", projectConfigSO.ChainID);
        PlayerPrefs.SetString("Chain", projectConfigSO.Chain);
        PlayerPrefs.SetString("Network", projectConfigSO.Network);
        PlayerPrefs.SetString("RPC", projectConfigSO.RPC);

        if (PlayerPrefs.HasKey("RememberMe") && PlayerPrefs.HasKey("Account"))
            if (PlayerPrefs.GetInt("RememberMe") == 1 && PlayerPrefs.GetString("Account") != "")
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public async void OnLogin()
    {
        var timestamp = (int)System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds;
        var expirationTime = timestamp + 60;
        var message = expirationTime.ToString();
        var signature = await Web3Wallet.Sign(message);
        var account = SignVerifySignature(signature, message);
        var now = (int)System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds;

        if (account.Length == 42 && expirationTime >= now)
        {
            PlayerPrefs.SetString("Account", account);
            if (rememberMe.value)
                PlayerPrefs.SetInt("RememberMe", 1);
            else
                PlayerPrefs.SetInt("RememberMe", 0);
            Debug.Log("Account: " + account);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    public string SignVerifySignature(string signatureString, string originalMessage)
    {
        var msg = "\x19" + "Ethereum Signed Message:\n" + originalMessage.Length + originalMessage;
        var msgHash = new Sha3Keccack().CalculateHash(Encoding.UTF8.GetBytes(msg));
        var signature = MessageSigner.ExtractEcdsaSignature(signatureString);
        var key = EthECKey.RecoverFromSignature(signature, msgHash);
        return key.GetPublicAddress();
    }
}
