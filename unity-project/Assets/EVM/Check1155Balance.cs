using Web3Unity.Scripts.Library.Ethers.Contracts;
using Newtonsoft.Json;
using Web3Unity.Scripts.Library.Ethers.Providers;
using UnityEngine;
using UnityEngine.UIElements;

public class Check1155Balance : MonoBehaviour
{
    public UIDocument uiDocument;
    private Label diamondsTotalLabel;
    async public void Check1155TotalBalance()
    {
        string contractAbi = ContractAbi.Abi1155;
        string contractAddress = "0x9f109bD4cC26357184CF6b1f87cFaadd8233E432";
        string method = "getTotalBalance";
        
        var walletAddress = PlayerPrefs.GetString("Account");
        var provider = new CustomJsonRpcProvider("https://opbnb-testnet.nodereal.io/v1/64a9df0874fb4a93b9d0a3849de012d3", "5611");
        var contract = new Contract(contractAbi, contractAddress, provider);
        var calldata = await contract.Call(method, new object[]
        {
            // Pass the account address as the parameter
            walletAddress
        });
        var totalDiamonds = calldata[0];
        
        print("Contract 1155 Balance (Diamonds) Total: " + totalDiamonds);
        
        diamondsTotalLabel = uiDocument.rootVisualElement.Q<Label>("DiamondsTotal");
        diamondsTotalLabel.text = totalDiamonds.ToString();
    }

    void Start()
    {
        Check1155TotalBalance();
    }
}
