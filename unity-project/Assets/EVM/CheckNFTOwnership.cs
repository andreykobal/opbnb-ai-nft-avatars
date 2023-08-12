using Web3Unity.Scripts.Library.Ethers.Contracts;
using Newtonsoft.Json;
using Web3Unity.Scripts.Library.Ethers.Providers;
using UnityEngine;

public class CheckNFTOwnership : MonoBehaviour
{
    public UI ui;

    private int[] avatarBalances = new int[3];

    async public void CheckNFTBalance(string contractAddress, int avatarIndex)
    {
        string contractAbi = ContractAbi.SingleChainAbi;
        string method = "getBalance";

        var provider = new CustomJsonRpcProvider("https://opbnb-testnet.nodereal.io/v1/64a9df0874fb4a93b9d0a3849de012d3", "5611");
        var contract = new Contract(contractAbi, contractAddress, provider);
        Debug.Log("Contract: " + contract);


        // Call the getBalance method
        var walletAddress = PlayerPrefs.GetString("Account");
        Debug.Log("Wallet address is: " + walletAddress);
        var calldata = await contract.Call(method, new object[]
        {
            // Pass the wallet address for which you want to retrieve the balance
            walletAddress
        });
        avatarBalances[avatarIndex] = int.Parse(calldata[0].ToString());
        print("ERC-721 Token Balance for avatar " + (avatarIndex + 1) + ": " + avatarBalances[avatarIndex]);

        string buttonText = avatarBalances[avatarIndex] > 0 ? "Select" : "Mint";
        ui.UpdateButtonLabel(avatarIndex, buttonText);
    }

    public bool HasAvatar(int avatarIndex)
    {
        return avatarBalances[avatarIndex] > 0;
    }
}