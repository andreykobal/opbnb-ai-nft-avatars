using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ClaimController : MonoBehaviour
{
    private const string BaseUrl = "https://skale-sfuel-77334b37033d.herokuapp.com";

    private void Start() {
        string currentAccount = PlayerPrefs.GetString("Account");
        Claim(currentAccount);
    }

    public void Claim(string ethereumAddress)
    {
        string url = $"{BaseUrl}/claim/{ethereumAddress}";
        StartCoroutine(SendRequest(url));
    }

    private IEnumerator SendRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("sFuel Claim Request successful");
                Debug.Log(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError("sFuel Claim Request failed");
                Debug.LogError(webRequest.error);
            }
        }
    }
}
