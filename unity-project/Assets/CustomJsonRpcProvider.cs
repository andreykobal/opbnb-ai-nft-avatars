using Web3Unity.Scripts.Library.Ethers.Providers;
using System;
using System.Threading.Tasks;


public class CustomJsonRpcProvider : JsonRpcProvider
{
    private readonly ulong _customChainId;

    public CustomJsonRpcProvider(string url, string customChainId) : base(url)
    {
        if (ulong.TryParse(customChainId, out ulong chainId))
        {
            _customChainId = chainId;
        }
        else
        {
            throw new ArgumentException("Invalid customChainId format. Must be a valid ulong value.");
        }
    }

    public override Task<Web3Unity.Scripts.Library.Ethers.Network.Network> DetectNetwork()
    {
        var network = new Web3Unity.Scripts.Library.Ethers.Network.Network
        {
            ChainId = _customChainId
        };
        return Task.FromResult(network);
    }
}
