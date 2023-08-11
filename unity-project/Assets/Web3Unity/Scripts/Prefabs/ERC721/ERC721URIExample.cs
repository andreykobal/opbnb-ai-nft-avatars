using UnityEngine;
using Web3Unity.Scripts.Library.ETHEREUEM.EIP;
using Web3Unity.Scripts.Library.Ethers.Providers;

public class ERC721URIExample : MonoBehaviour
{
    async void Start()
    {
        string contract = "0x191F03d5c7f18C8Cce0f6105Ac3c5D7a95Bad747";
        string tokenId = "1";

        string uri = await ERC721.URI(contract, tokenId);
        Debug.LogError(uri);
    }
}
