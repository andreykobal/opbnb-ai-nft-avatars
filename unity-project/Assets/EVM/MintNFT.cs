using System.Collections;
using Web3Unity.Scripts.Library.Ethers.Contracts;
using Web3Unity.Scripts.Library.Ethers.Providers;
using Web3Unity.Scripts.Library.Web3Wallet;

using UnityEngine;

public class MintNFT : MonoBehaviour
{
    public CheckNFTOwnership checkNFTOwnership;

    string[] contractAddresses = 
    {
        "0xd68B7C666b269B3FC9daAc7a3a446bE32999920E",
        "0x5dA076A7a10560E0d597E131489fDd0Dc28c7951",
        "0x84797dA18F5C8eD3F04783412568784C6eCa68dB"
    };

    async public void mintItem(int avatarIndex)
    {
        string chainId = "5611";
        var tokenURI = "https://bafkreiczapdwomdlotjqt4yaojyizlgarn4kq57smi3ptkwn5lug5yz7yu.ipfs.nftstorage.link/";

        string contractAbi = ContractAbi.SingleChainAbi;
        string contractAddress = contractAddresses[avatarIndex];
        string method = "mintItem";
        
        var provider = new JsonRpcProvider("https://opbnb-testnet.nodereal.io/v1/64a9df0874fb4a93b9d0a3849de012d3");
        var contract = new Contract(contractAbi, contractAddress, provider);
        Debug.Log("Contract: " + contract);
        
        var calldata = contract.Calldata(method, new object[]
        {
            tokenURI
        });
        
        string response = await Web3Wallet.SendTransaction(chainId, contractAddress, "0", calldata, "", "");

        Debug.LogError(response);
        
        StartCoroutine(WaitAndCheckBalance(avatarIndex));
    }
    
    IEnumerator WaitAndCheckBalance(int avatarIndex)
    {
        // Wait for 15 seconds
        yield return new WaitForSeconds(20);

        // Check the balance again
        checkNFTOwnership.CheckNFTBalance(contractAddresses[avatarIndex], avatarIndex);
    }
}