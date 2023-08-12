using Web3Unity.Scripts.Library.Ethers.Contracts;
using Web3Unity.Scripts.Library.Ethers.Providers;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

public class GetTokensMetadata : MonoBehaviour
{
    private List<NftData> nftDataList = new List<NftData>();

    async public void CheckTokensOwned()
    {
        nftDataList.Clear();

        string contractAddress = "0xd68B7C666b269B3FC9daAc7a3a446bE32999920E";
        var provider = new CustomJsonRpcProvider("https://opbnb-testnet.nodereal.io/v1/64a9df0874fb4a93b9d0a3849de012d3", "5611");
        await GetAndProcessRequests(contractAddress, provider, "opbnb");
        
    }

    async Task GetAndProcessRequests(string contractAddress, JsonRpcProvider provider, string network)
    {
        string contractAbi = ContractAbi.SingleChainAbi;
        var contract = new Contract(contractAbi, contractAddress, provider);

        string ownerAddress = PlayerPrefs.GetString("Account"); // Get the address of the owner whose tokens you want to check.

        try
        {
            var calldata = await contract.Call("getTokensOfOwner", new object[] { ownerAddress });
            List<string> tokenURIs = calldata[0] as List<string>;

            if (tokenURIs != null)
            {
                foreach (string tokenURI in tokenURIs)
                {
                    await GetRequest(tokenURI, network, null);
                }
            }
            else
            {
                Debug.LogError("Could not cast returned data to List<string>.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Exception when calling getTokensOfOwner: " + e.Message);
        }
    }

    async Task GetRequest(string uri, string network, string tokenId)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            await webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(uri + ": Error: " + webRequest.error);
            }
            else
            {
                JObject json = JObject.Parse(webRequest.downloadHandler.text);

                if (json != null)
                {
                    try
                    {
                        NftData nftData = new NftData
                        {
                            Title = json["name"].ToString(),
                            Description = json["description"].ToString(),
                            UrlModel = json["animation_url"].ToString(),
                            UrlIcon = json["image"].ToString(),
                            Network = network,
                            TokenId = tokenId
                            // If you wish to include attributes, add them here
                        };

                        nftDataList.Add(nftData);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error while parsing and creating NftData: " + e);
                    }
                }
                else
                {
                    Debug.LogError("Parsed JSON is null.");
                }
            }
        }
    }
}
