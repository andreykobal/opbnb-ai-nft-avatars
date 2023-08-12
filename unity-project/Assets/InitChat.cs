using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitChat : MonoBehaviour
{
    public OpenAIChat openAIChat;
    
    private string avatarName;
    private string avatarDescription;
    private string avatarVoice;

    void Start()
    {
        avatarName = PlayerPrefs.GetString("avatarName");
        avatarDescription = PlayerPrefs.GetString("avatarDescription");
        avatarVoice = PlayerPrefs.GetString("avatarVoice");
        
        initAiAvatar();
    }

    public void initAiAvatar()
    {

        openAIChat.conversationContext = "";
        
        // if avatarName and avatarDescrition length bigger then 0 null then initAiAvatar with them else jane doe
        if (avatarName.Length > 0 && avatarDescription.Length > 0 && avatarVoice.Length > 0)
        {
            openAIChat.characterName = avatarName;
            openAIChat.characterBio = avatarDescription;
            openAIChat.setVoiceConfig(avatarVoice);
        }
        else
        {
            openAIChat.characterName = "Jane Doe";
            openAIChat.characterBio = "Jane Doe Lives in Ailand";
            openAIChat.setVoiceConfig("Female"); 
        }
        openAIChat.initialGreeting();
        //log 
        Debug.Log("Avatar name: " + avatarName + " Avatar description: " + avatarDescription + "Voice: " + avatarVoice);
    }
}
