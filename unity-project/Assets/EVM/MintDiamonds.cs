using UnityEngine;
using Web3Unity.Scripts.Library.Ethers.Contracts;
using Web3Unity.Scripts.Library.Web3Wallet;
using UnityEngine.UIElements;

public class MintDiamonds : MonoBehaviour
{
    private VisualElement claimElement;

    void Start()
    {
        // Find the "Claim" button in the UI document
        Button claimButton = GetComponent<UIDocument>().rootVisualElement.Q<Button>("ClaimButton");
        claimElement = GetComponent<UIDocument>().rootVisualElement.Q("Claim");

        // Add a click event handler to the "Claim" button
        claimButton.clicked += Mint1155Diamonds;
    }

    async public void Mint1155Diamonds()
    {
        string chainId = "5611";
        string contractAddress = "0x9f109bD4cC26357184CF6b1f87cFaadd8233E432";
        string value = "0";
        string abi = ContractAbi.Abi1155;
        string method = "batchMint";
        string gasLimit = "";
        string gasPrice = "";
        string[] uri = { "https://gnfd-testnet-sp1.bnbchain.org/view/ailand-testnet/metadata.json" };
        var contract = new Contract(abi, contractAddress);
        var calldata = contract.Calldata(method, new object[]
        {
            uri
        });

        string response = await Web3Wallet.SendTransaction(chainId, contractAddress, value, calldata, gasLimit, gasPrice);
        print(response);

        // Check if there is a response
        if (!string.IsNullOrEmpty(response))
        {
            claimElement.visible = false;
        }
    }
}


