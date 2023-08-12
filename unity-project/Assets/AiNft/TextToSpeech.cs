//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
// <code>
using System;
using System.Threading;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.UIElements;
using Microsoft.CognitiveServices.Speech;

public class TextToSpeech : MonoBehaviour
{
    // Hook up the three properties below with a Text, InputField and Button object in your UI.
    //public Text outputText;
    //public InputField inputField;
    //public Button speakButton;
    public AudioSource audioSource;
    public UIDocument uiDocument;
    private Label outputText;

    // Replace with your own subscription key and service region (e.g., "westus").
    private const string SubscriptionKey = "c7d785b3bf134446adf40fc5d036ce32";
    private const string Region = "eastus";

    private const int SampleRate = 24000;

    private object threadLocker = new object();
    private bool waitingForSpeak;
    private bool audioSourceNeedStop;
    private string message;

    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;
    
    private string femaleVoice = "en-US-SaraNeural";
    private string maleVoice = "en-US-BrandonNeural";
    private string robotVoice = "en-GB-OliviaNeural";

    public Animator avatarAnimator;

    public bool isSpeaking = false;

    public void SetSynthesizeVoiceConfig(string voice)
    {
        if (voice == "Female" || voice == "female")
        {
            speechConfig.SpeechSynthesisVoiceName = femaleVoice;
        } else if (voice == "Male" || voice == "male")
        {
            speechConfig.SpeechSynthesisVoiceName = maleVoice;
        } else
        {
            speechConfig.SpeechSynthesisVoiceName = robotVoice;
        }
        Debug.Log("SetSynthesizeVoiceConfig: " + speechConfig.SpeechSynthesisVoiceName);
        synthesizer = new SpeechSynthesizer(speechConfig, null);
        
        synthesizer.SynthesisCanceled += (s, e) =>
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(e.Result);
            message = $"CANCELED:\nReason=[{cancellation.Reason}]\nErrorDetails=[{cancellation.ErrorDetails}]\nDid you update the subscription info?";
        };
    }
    
    public void synthesizeSpeech(string speechMessage)
    {
        lock (threadLocker)
        {
            waitingForSpeak = true;
        }

        outputText.text = speechMessage;
        
        string newMessage = null;
        var startTime = DateTime.Now;

        // Starts speech synthesis, and returns once the synthesis is started.
        using (var result = synthesizer.StartSpeakingTextAsync(speechMessage).Result)
        {
            // Native playback is not supported on Unity yet (currently only supported on Windows/Linux Desktop).
            // Use the Unity API to play audio here as a short term solution.
            // Native playback support will be added in the future release.
            var audioDataStream = AudioDataStream.FromResult(result);
            var isFirstAudioChunk = true;
            var audioClip = AudioClip.Create(
                "Speech",
                SampleRate * 600, // Can speak 10mins audio as maximum
                1,
                SampleRate,
                true,
                (float[] audioChunk) =>
                {
                    var chunkSize = audioChunk.Length;
                    var audioChunkBytes = new byte[chunkSize * 2];
                    var readBytes = audioDataStream.ReadData(audioChunkBytes);
                    if (isFirstAudioChunk && readBytes > 0)
                    {
                        var endTime = DateTime.Now;
                        var latency = endTime.Subtract(startTime).TotalMilliseconds;
                        newMessage = $"Speech synthesis succeeded!\nLatency: {latency} ms.";
                        isFirstAudioChunk = false;
                    }

                    for (int i = 0; i < chunkSize; ++i)
                    {
                        if (i < readBytes / 2)
                        {
                            audioChunk[i] = (short)(audioChunkBytes[i * 2 + 1] << 8 | audioChunkBytes[i * 2]) / 32768.0F;
                        }
                        else
                        {
                            audioChunk[i] = 0.0f;
                        }
                    }

                    if (readBytes == 0)
                    {
                        Thread.Sleep(200); // Leave some time for the audioSource to finish playback
                        audioSourceNeedStop = true;

                    }
                });

            audioSource.clip = audioClip;
            audioSource.PlayWithEvent(); }

        lock (threadLocker)
        {
            if (newMessage != null)
            {
                message = newMessage;
            }

            waitingForSpeak = false;
        }
    }

    void Start()
    {
        var root = uiDocument.rootVisualElement;
        outputText = root.Q<Label>("Response");
        
        if (outputText == null)
        {
            UnityEngine.Debug.LogError("outputText property is null! Assign a UI Text element to it.");
        }
        /*else if (inputField == null)
        {
            message = "inputField property is null! Assign a UI InputField element to it.";
            UnityEngine.Debug.LogError(message);
        }
        else if (speakButton == null)
        {
            message = "speakButton property is null! Assign a UI Button to it.";
            UnityEngine.Debug.LogError(message);
        }*/
        else
        {
            // Continue with normal initialization, Text, InputField and Button objects are present.
            //inputField.text = "Enter text you wish spoken here.";
            message = "Click button to synthesize speech";
            //speakButton.onClick.AddListener(ButtonClick);

            // Creates an instance of a speech config with specified subscription key and service region.
            speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, Region);

            // The default format is RIFF, which has a riff header.
            // We are playing the audio in memory as audio clip, which doesn't require riff header.
            // So we need to set the format to raw (24KHz for better quality).
            speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);

            // Creates a speech synthesizer.
            // Make sure to dispose the synthesizer after use!
            //synthesizer = new SpeechSynthesizer(speechConfig, null);

            
        }
    }

    void Update()
    {
        /*if (audioSource.isPlaying && avatarAnimator != null)
        {
            isSpeaking = true;
            avatarAnimator.SetBool("isTalking", true); 
        } else if (!audioSource.isPlaying && avatarAnimator != null)
        {
            isSpeaking = false;
            avatarAnimator.SetBool("isTalking", false); 
        }*/
        lock (threadLocker)
        {
            /*if (speakButton != null)
            {
                speakButton.interactable = !waitingForSpeak;
            }*/

            /*if (outputText != null)
            {
                outputText.text = message;
            }*/

            if (audioSourceNeedStop)
            {
                audioSource.StopWithEvent();
                audioSourceNeedStop = false;
            }
        }
    }
    
    private void OnEnable() 
    {
        AudioSourceExtensions.OnAudioStateChanged += HandleAudioStateChange;
    }

    private void OnDisable() 
    {
        AudioSourceExtensions.OnAudioStateChanged -= HandleAudioStateChange;
    }

    private void HandleAudioStateChange(AudioSource changedAudioSource) 
    {
        if (changedAudioSource == audioSource) 
        {
            if (audioSource.isPlaying && avatarAnimator != null)
            {
                isSpeaking = true;
                avatarAnimator.SetBool("isTalking", true); 
            } 
            else if (!audioSource.isPlaying && avatarAnimator != null)
            {
                isSpeaking = false;
                avatarAnimator.SetBool("isTalking", false); 
            } 
            else if (audioSource.isPlaying && avatarAnimator == null)
            {
                isSpeaking = true;
            } 
            else if (!audioSource.isPlaying && avatarAnimator == null)
            {
                isSpeaking = false;
            }
        }
    }

    void OnDestroy()
    {
        if (synthesizer != null)
        {
            synthesizer.Dispose();
        }
    }
}
// </code>
