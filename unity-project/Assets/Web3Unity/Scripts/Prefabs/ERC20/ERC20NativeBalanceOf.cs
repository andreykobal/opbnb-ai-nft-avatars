using UnityEngine;
using Web3Unity.Scripts.Library.Ethers.Providers;

public class ERC20NativeBalanceOf : MonoBehaviour
{
    async void Start()
    {
        string account = "0x98000edf79B0eb598085721D57d93B5865c87751";
        var provider = RPC.GetInstance.Provider();
        var getBalance = await provider.GetBalance(account);
        Debug.LogError("Account Balance: " + getBalance);
    }
}