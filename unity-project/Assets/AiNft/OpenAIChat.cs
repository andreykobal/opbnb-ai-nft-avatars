using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.UIElements;


public class OpenAIChat : MonoBehaviour
{
    private const string apiEndpoint = "https://api.openai.com/v1/chat/completions";
    private const string apiKey = "";
    private const string model = "gpt-3.5-turbo";
    private const int maxContextLength = 4096; // Maximum context length allowed by GPT

    public TextToSpeech speechSynthesizer;
    public string characterName = "Aurelia Everwood";
    public string characterBio = "Aurelia is a skilled inventor and engineer hailing from the city of ARTIMUS.";
    
    public UIDocument uiDocument;
    private TextField userInputField;
    private Button sendButton;
    private Label messageText;

    public string conversationContext = "";
    
    private Queue<string> sentenceQueue = new Queue<string>();


    private void Start()
    {
        var root = uiDocument.rootVisualElement;
        userInputField = root.Q<TextField>("Input");
        sendButton = root.Q<Button>("Send");
        messageText = root.Q<Label>("TextMessage");
        
        Debug.Log($"UserInputField: {userInputField}, SendButton: {sendButton}, MessageText: {messageText}");

        

        // Set up the button click event to send user messages
        sendButton.clicked += OnSendButtonClick;
        
        // Add the event listener for FocusInEvent
        userInputField.RegisterCallback<FocusInEvent>(OnInputFieldFocused);


    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnSendButtonClick();
        }
    }
    
    private void OnInputFieldFocused(FocusInEvent evt)
    {
        if (userInputField.text == "Send a message")
        {
            userInputField.SetValueWithoutNotify(""); // Clear the input field
        }
    }

    public void initialGreeting()
    {
        StartCoroutine(SendMessage("Introduce yourself very shortly. Start with I'm..."));
    }
    
    public void OnRecieveSpeech(string message)
    {
        Debug.Log("OnRecieveSpeech: " + message);
        StartCoroutine(SendMessage(message));
    }
    
    

    private void OnSendButtonClick()
    {
        string userMessage = userInputField.text;
        messageText.text = userMessage;
        if (!string.IsNullOrEmpty(userMessage))
        {
            StartCoroutine(SendMessage(userMessage));
            userInputField.SetValueWithoutNotify(""); // Clear the input field after sending the message
        }
    }

    private IEnumerator SendMessage(string message)
    {
        string systemMessage = "You are " + characterName + ". " + characterBio + ". \n \n Keep the conversation going, respond to user's last message, take into account the conversation context. Act like a character of a game, sound naturally, keep your responses short and easy to understand. ";

        // Check if the message length exceeds the maximum context length
        if (message.Length > maxContextLength)
        {
            Debug.LogError("Error: The message length exceeds the maximum context length allowed by GPT.");
            yield break;
        }

        Debug.Log("conversationContext: " + conversationContext);

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
            Debug.Log("Completion: " + reply);

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

            // Update conversation context with both user message and chatbot's response
            conversationContext += "USER: " + message + "\n";
            conversationContext += characterName + ": " + reply + "\n";

            // Check and adjust the conversation context length
            TrimConversationContext();

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
    
    private IEnumerator ProcessSentenceQueue()
    {
        while (sentenceQueue.Count > 0)
        {
            if (!speechSynthesizer.isSpeaking)
            {
                string nextSentence = sentenceQueue.Dequeue();
                speechSynthesizer.synthesizeSpeech(nextSentence);
            
                // Wait until the synthesizer finishes speaking before dequeuing the next sentence
                yield return new WaitUntil(() => !speechSynthesizer.isSpeaking);
            }
            else
            {
                yield return null;  // Wait for the next frame
            }
        }
    }


    private void TrimConversationContext()
    {
        // Check if the conversation context length exceeds the maximum allowed
        while (conversationContext.Length > maxContextLength)
        {
            // Find the first index of newline character to remove the oldest message from the context
            int newLineIndex = conversationContext.IndexOf('\n');
            if (newLineIndex >= 0)
            {
                conversationContext = conversationContext.Substring(newLineIndex + 1);
            }
            else
            {
                // If there's no newline character (only one message in the context), simply truncate the context
                conversationContext = conversationContext.Substring(0, maxContextLength);
                break;
            }
        }
    }

    public void setVoiceConfig(string voice)
    {
        speechSynthesizer.SetSynthesizeVoiceConfig(voice);
    }

    // Classes to handle the JSON data
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
