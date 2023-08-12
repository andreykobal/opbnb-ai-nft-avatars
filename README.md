# Interactive AI NFT Avatars on opBNB Chain

![ai-nft-bnb](https://github.com/andreykobal/opbnb-ai-nft-avatars/assets/19206978/b0bce338-c907-4b68-88c9-8f2c77fe88c3)


Welcome to the **Interactive AI NFT Avatars on opBNB Chain** GitHub repository! This repository contains the Unity game project along with the smart contracts used in the game. Interactive AI NFT Avatars is an exciting 3D game where players use NFT avatars, complete quests, collect diamonds, and explore magical worlds.

## Project Setup

To set up the Interactive AI NFT Avatars on opBNB Chain project, please follow these steps:

1. Clone the repository to your local machine using the following command:
   ```
   git clone https://github.com/andreykobal/opbnb-ai-nft-avatars.git
   ```

2. Open the Unity game project that is located in `/unity-project` using Unity Hub or your preferred Unity editor.

3. Install the necessary dependencies and packages required for the project.

4. Configure the Unity project settings as needed and ensure that the project is properly configured for your target platform.

5. Set up the Metamask extension in your preferred browser and create a wallet.

6. Configure the Metamask wallet by connecting it to the opBNB network and obtaining the necessary testnet tokens [opBNB Getting Started]([https://docs..network/develop/](https://docs.bnbchain.org/opbnb-docs/docs/build-on-opbnb/getting-started)). 

7. Build and run the game to start playing Interactive AI NFT Avatars!

## Smart Contract Deployment

To deploy the smart contracts using Hardhat, please follow these steps:

* Clone this and run `npm i` in terminal.
* Add .env file with:
```
MNEMONIC=privatekey. not the seedphrase
```
* Edit the deploy script to pass in your name and ticker/uri depending on standard
* Edit the contractUri method in the contract and add your collection metadata URI 
* Edit the mint script and add your token uri, contract address and account address of the account you want to mint to.
* Compile the smart contracts with `npx hardhat compile`
* Deploy with `npx hardhat run --network opbnb scripts/deploy721.js` for ERC-721
* Deploy with `npx hardhat run --network opbnb scripts/deploy1155.js` for ERC-1155
* Mint 721 with `npx hardhat run --network opbnb scripts/mint.js`
* Mint 1155 with `npx hardhat run --network opbnb scripts/mint1155.js`
* Check balance of ERC-1155 with `npx hardhat run --network opbnb scripts/balances.js`
* Get metadata of ERC-721 with `npx hardhat run --network opbnb scripts/getTokensOfOwner.js`


Contract code inspired from Opensea: https://github.com/ProjectOpenSea/meta-transactions/blob/main/contracts/ERC721MetaTransactionMaticSample.sol
This is for gas-less transactions when transferring assets. Users dont have to pay that extra gas, and get a better experience.

## Smart Contract Code Highlights

### GameItem.sol

The `GameItem.sol` contract is responsible for managing the 721 non-fungible token (NFT) avatars in Interactive AI NFT Avatars. Players can purchase avatars and own them using this contract.

Important code snippets from `GameItem.sol`:

- `mintItem` function: This function mints a new avatar NFT with the provided token URI and assigns it to the caller's address.
  ```solidity
  function mintItem(string memory tokenURI) public payable {
      // Mint a new avatar NFT and assign it to the caller's address
      // ...
  }
  ```

### GameItems.sol

The `GameItems.sol` contract handles the 1155 semi-fungible token (SFT) diamonds in Interactive AI NFT Avatars. Players can collect these diamonds and use them to claim rewards.

Important code snippets from `GameItems.sol`:

- `batchMint` function: This function allows the batch minting of diamond tokens with the provided token URIs and assigns them to the caller's address.
  ```solidity
  function batchMint(string[] memory tokenURIs) public {
      // Batch mint diamond tokens and assign them to the caller's address
      // ...
  }
  ```

- `getTotalBalance` function: This function returns the total balance of diamonds owned by a specific address.
  ```solidity
  function getTotalBalance(address account) public view returns (uint256) {
      // Get the total balance of diamonds owned by the specified address
      // ...
  }
  ```

## Game Code Highlights

Here are some C# code highlights from the Interactive AI NFT Avatars project:

### MintNFT.cs

```csharp
async public void mintItem(int avatarIndex)
{
    string chainId = "5611";
    var tokenURI = "https://bafkreiczapdwomdlotjqt4yaojyizlgarn4kq57smi3ptkwn5lug5yz7yu.ipfs.nftstorage.link/";

    string contractAbi = "";
    string contractAddress = contractAddresses[avatarIndex];
    string method = "mintItem";
    
    var provider = new JsonRpcProvider("https://opbnb-testnet.nodereal.io/v1/64a9df0874fb4a93b9d0a3849de012d3");
    var contract = new Contract(contractAbi, contractAddress, provider);
    Debug.Log("Contract: " + contract);
    
    var calldata = contract.Calldata(method, new object[]
    {
        tokenURI
    });
    
    string response = await Web3Wallet.SendTransaction(chainId, contractAddress, "500000000000000000", calldata, "", "");

    Debug.LogError(response);
    
    StartCoroutine(WaitAndCheckBalance(avatarIndex));
}
```

### CheckNFTOwnership.cs

```csharp
async public void CheckNFTBalance(string contractAddress, int avatarIndex)
{
    string contractAbi = "";
    string method = "getBalance";

    var provider = new JsonRpcProvider("https://opbnb-testnet.nodereal.io/v1/64a9df0874fb4a93b9d0a3849de012d3");
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

    string buttonText = avatarBalances[avatarIndex] > 0 ? "Select" : "Buy 0.5 $SKL";
    ui.UpdateButtonLabel(avatarIndex, buttonText);
}
```

### Check1155Balance.cs

```csharp
async public void Check1155TotalBalance()
{
    string contractAbi = "";
    string contractAddress = "0x6721De8B1865A6cD98C64165305611B1f28B95e4";
    string method = "getTotalBalance";
    
    var walletAddress = PlayerPrefs.GetString("Account");
    var provider = new JsonRpcProvider("https://opbnb-testnet.nodereal.io/v1/64a9df0874fb4a93b9d0a3849de012d3");
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
```

### MintDiamonds.cs

```csharp
private async void Start()
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
    string contractAddress = "0x6721De8B1865A6cD98C64165305611B1f28B95e4";
    string value = "0";
    string abi = "";
    string method = "batchMint";
    string gasLimit = "";
    string gasPrice = "";
    string[] uri = { "https://bafkreiczapdwomdlotjqt4yaojyizlgarn4kq57smi3ptkwn5lug5yz7yu.ipfs.nftstorage.link/" };
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
```

### GetTokensMetadata.cs
This code defines a Unity script named `GetTokensMetadata` that interacts with an Ethereum smart contract and retrieves metadata for NFTs (Non-Fungible Tokens) owned by a specific address, including information like title, description, URLs for the 3D model and icon, network, and token ID.

The most important code snippet is the `GetRequest` method, responsible for making a web request to retrieve JSON metadata for a specific NFT token URI, then parsing the JSON and creating an `NftData` object to store the metadata.

```csharp
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
```

This method fetches metadata from a given URI, processes the retrieved JSON data to create an `NftData` object, and adds it to the `nftDataList` collection for later use.

## AI Integration Code Highlights

AI integration in this project involved the use of the OpenAI API for natural language processing, as well as the Microsoft Cognitive Services Speech SDK for text-to-speech synthesis; you can explore further details through the links below:

OpenAI API Reference for Models: [OpenAI API Models](https://platform.openai.com/docs/api-reference/models)

Microsoft Cognitive Services Speech SDK on GitHub: [Cognitive Services Speech SDK](https://github.com/Azure-Samples/cognitive-services-speech-sdk/tree/master)

### OpenAIChat.cs

This code defines a Unity script named `OpenAIChat` that implements a chatbot interaction using OpenAI's API, where the chatbot engages in a conversation with users and responds to their messages.

The most important code snippet is the `SendMessage` method, responsible for sending a user's message to the OpenAI API, receiving a response, processing it, updating the conversation context, and initiating speech synthesis for the chatbot's reply:

```csharp
private IEnumerator SendMessage(string message)
{
    // ...

    // Build the chat request
    ChatRequest chatRequest = new ChatRequest
    {
        model = model,
        messages = new Message[]
        {
            // Include both the user message and the system response in the conversation context
            new Message { role = "system", content = systemMessage },
            new Message { role = "user", content = conversationContext + "\n\n USER: " + message }
        }
    };

    // ...

    // Send the request to OpenAI API
    yield return request.SendWebRequest();

    if (request.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError("Error: " + request.error);
    }
    else
    {
        // Process the API response
        string jsonResponse = request.downloadHandler.text;
        ChatResponse chatResponse = JsonConvert.DeserializeObject<ChatResponse>(jsonResponse);
        string reply = chatResponse.choices[0].message.content;
        Debug.Log("Completion: " + reply);

        // ...

        // Update conversation context with user and chatbot messages
        conversationContext += "USER: " + message + "\n";
        conversationContext += characterName + ": " + reply + "\n";

        // Check and adjust the conversation context length
        TrimConversationContext();

        // Enqueue sentences for speech synthesis
        Regex regex = new Regex(@"(.*?[.!?])\s*");
        MatchCollection matches = regex.Matches(reply);
        foreach (Match match in matches)
        {
            sentenceQueue.Enqueue(match.Groups[1].Value.Trim());
        }

        // Start processing the sentence queue for speech synthesis
        StartCoroutine(ProcessSentenceQueue());
    }
}
```

This method sends the user message along with the conversation context to the OpenAI API and handles the chatbot's response. It updates the conversation context and enqueues sentences for speech synthesis to simulate a more natural conversation flow.

### SpeechToText.cs

This script, named `SpeechToText`, is used in Unity to perform speech recognition using Microsoft's Cognitive Services Speech API, and it also interacts with an `OpenAIChat` script to integrate with a chatbot.

The most important code snippet is the `ButtonClick` method, where the speech recognition process is initiated:

```csharp
public async void ButtonClick()
{
    // Creates an instance of a speech config with specified subscription key and service region.
    var config = SpeechConfig.FromSubscription(SubscriptionKey, Region);

    using (var recognizer = new SpeechRecognizer(config))
    {
        lock (threadLocker)
        {
            waitingForReco = true;
        }

        // Starts speech recognition, and returns after a single utterance is recognized.
        var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

        // Checks result.
        string newMessage = string.Empty;
        if (result.Reason == ResultReason.RecognizedSpeech)
        {
            newMessage = result.Text;
        }
        else if (result.Reason == ResultReason.NoMatch)
        {
            newMessage = "NOMATCH: Speech could not be recognized.";
        }
        else if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = CancellationDetails.FromResult(result);
            newMessage = $"CANCELED: Reason={cancellation.Reason} ErrorDetails={cancellation.ErrorDetails}";
        }

        lock (threadLocker)
        {
            message = newMessage;
            waitingForReco = false;
            MainThreadDispatcher.RunOnMainThread(() => {
                if (outputText != null)
                {
                    outputText.text = message;
                }
                openAIChat.OnRecieveSpeech(message);
            });

        }
    }
}
```

This method initializes a speech recognizer, starts speech recognition, and handles the recognition result. If recognized, the recognized text is processed and passed to the `OpenAIChat` script for further interaction.

### TextToSpeech.cs

This script, named `TextToSpeech`, handles text-to-speech synthesis in Unity using Microsoft's Cognitive Services Speech API, and it integrates with UI elements and an avatar animation.

The most important code snippet is the `synthesizeSpeech` method, which performs text-to-speech synthesis and plays the generated audio:

```csharp
public void synthesizeSpeech(string speechMessage)
{
    // ...

    using (var result = synthesizer.StartSpeakingTextAsync(speechMessage).Result)
    {
        // ...
        
        audioSource.clip = audioClip;
        audioSource.PlayWithEvent();
    }

    lock (threadLocker)
    {
        // ...
    }
}
```

This method uses the `SpeechSynthesizer` to synthesize speech from the provided message. It creates an audio clip from the synthesized audio data and plays it through the `audioSource`. The method also handles playback control and synchronization with the main Unity thread.


## Contributing

Thank you for your interest in contributing

 to Interactive AI NFT Avatars! If you would like to contribute, please follow these guidelines:

- Fork the repository and create a new branch for your contributions.
- Make your changes and ensure that the code adheres to the project's coding style and conventions.
- Test your changes thoroughly.
- Submit a pull request describing your changes and the rationale behind them.

We appreciate your contributions to make Interactive AI NFT Avatars an even more exciting game!

## License

This project is licensed under the [MIT License](LICENSE). Feel free to use, modify, and distribute the code for personal or commercial purposes.

## Acknowledgements

We would like to express our gratitude to the open-source community for their valuable contributions and the resources that helped in the development of Interactive AI NFT Avatars.

If you have any questions or encounter any issues, please don't hesitate to reach out. Enjoy playing Interactive AI NFT Avatars!

