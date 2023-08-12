using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

public class AutonomousAIChat : MonoBehaviour
{
    private const string apiEndpoint = "https://api.openai.com/v1/chat/completions";
    private const string apiKey = "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"; // Make sure to insert your own API key here
    private const string model = "gpt-3.5-turbo";
    private const int maxContextLength = 4096; 

    public TextToSpeech speechSynthesizer;

    private string characterName;
    private string characterBio;

    public string conversationContext = "";

    private Queue<string> sentenceQueue = new Queue<string>();

    private bool isSpeaking = false;

    private void Start()
    {
        // Fetch data from PlayerPrefs
        characterName = PlayerPrefs.GetString("avatarName");
        characterBio = PlayerPrefs.GetString("avatarDescription");
        string voice = PlayerPrefs.GetString("avatarVoice");
        speechSynthesizer.SetSynthesizeVoiceConfig(voice);
        
        StartCoroutine(GeneratePeriodicPhrases());
        Debug.Log("Starting generating periodic phrases...");
        isSpeaking = true; // Set the flag to true

    }
    
    private void Update()
    {
        // If 'V' is pressed and the GeneratePeriodicPhrases coroutine is not already running
        if (Input.GetKeyDown(KeyCode.V) && !isSpeaking)
        {
        }
    }
    
    private IEnumerator GeneratePeriodicPhrases()
    {
        while (true)
        {
            StartCoroutine(SendMessage("Tell a random fact about yourself"));
        
            // Wait for the response to finish speaking
            yield return new WaitUntil(() => !speechSynthesizer.isSpeaking);

            // Then wait for an additional 20 seconds
            yield return new WaitForSeconds(20);        
        }
    }

    private IEnumerator SendMessage(string message)
    {
        string systemMessage = "Give a very short one-line response to the user's prompt. Be witty, concise, and hilarious!, crack jokes, and offer quick game insights. Think short, snappy! " + "You are " + characterName + ", the very character the player is controlling in an adventure game. Your biography: " + characterBio;

        if (message.Length > maxContextLength)
        {
            Debug.LogError("Error: The message length exceeds the maximum context length allowed by GPT.");
            yield break;
        }

        ChatRequest chatRequest = new ChatRequest
        {
            model = model,
            messages = new Message[]
            {
                new Message { role = "system", content = systemMessage },
                new Message { role = "user", content = conversationContext + "\n\n USER: " + message }
            }
        };

        string jsonPayload = JsonConvert.SerializeObject(chatRequest);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        UnityWebRequest request = new UnityWebRequest(apiEndpoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            ChatResponse chatResponse = JsonConvert.DeserializeObject<ChatResponse>(jsonResponse);
            string reply = chatResponse.choices[0].message.content;
            
            // Extract the character name from the system message
            string[] nameParts = characterName.Split(' ');
            string firstName = nameParts[0];
    
            // Construct a regex pattern to match various prefixes
            string pattern = @"^("
                             + Regex.Escape(firstName).ToUpper() + @"(:\s*)"   // Case for "JOHN: "
                             + "|"
                             + Regex.Escape(characterName) + @"(:\s+)"        // Case for "John Doe: "
                             + "|"
                             + Regex.Escape(firstName) + @"(:\s+)"            // Case for "John: "
                             + ")+";                                        // The '+' at the end will handle patterns like "JOHN: John Doe: "
        
            reply = Regex.Replace(reply, pattern, "");

            
            conversationContext += "USER: " + message + "\n";
            conversationContext += characterName + ": " + reply + "\n";
            
            TrimConversationContext();
            
            Regex regex = new Regex(@"(.*?[.!?])\s*");
            MatchCollection matches = regex.Matches(reply);
            foreach (Match match in matches)
            {
                sentenceQueue.Enqueue(match.Groups[1].Value.Trim());
            }
            
            StartCoroutine(ProcessSentenceQueue());
        }
    }
    
    private IEnumerator ProcessSentenceQueue()
    {
        while (sentenceQueue.Count > 0)
        {
            if (!speechSynthesizer.isSpeaking)
            {
                string nextSentence = sentenceQueue.Dequeue();
                speechSynthesizer.synthesizeSpeech(nextSentence);
                yield return new WaitUntil(() => !speechSynthesizer.isSpeaking);
            }
            else
            {
                yield return null;
            }
        }
    }

    private void TrimConversationContext()
    {
        while (conversationContext.Length > maxContextLength)
        {
            int newLineIndex = conversationContext.IndexOf('\n');
            if (newLineIndex >= 0)
            {
                conversationContext = conversationContext.Substring(newLineIndex + 1);
            }
            else
            {
                conversationContext = conversationContext.Substring(0, maxContextLength);
                break;
            }
        }
    }

    private class Message
    {
        public string role;
        public string content;
    }

    private class ChatRequest
    {
        public string model;
        public Message[] messages;
    }

    private class ChatResponse
    {
        public Choice[] choices;
    }

    private class Choice
    {
        public Message message;
    }
}
