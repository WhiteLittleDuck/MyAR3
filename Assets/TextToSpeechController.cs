using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;


[RequireComponent(typeof(AudioSource))]
public class TextToSpeechController : MonoBehaviour
{
    public GameObject textt;


    public string subKey;
    public string region;
    public string resourceName;

    public AudioClip explodedClip;
    public AudioClip diffusedClip;

    private string accessToken;
    public AudioSource audioSource;

    public static TextToSpeechController instance;
    // Start is called before the first frame update
    void Start(){
    }

    // Update is called once per frame
    void Update(){}
    void Awake(){
        instance=this;
        audioSource=GetComponent<AudioSource>();
        StartCoroutine(GetAccessToken());
    }

    IEnumerator GetAccessToken(){
        // create a web request and set the method to POST
        UnityWebRequest webReq = new UnityWebRequest(string.Format("https://{0}.api.cognitive.microsoft.com/sts/v1.0/issuetoken", region));
        webReq.method = UnityWebRequest.kHttpVerbPOST;              
        // create a download handler to receive the access token
        webReq.downloadHandler = new DownloadHandlerBuffer();
        // set the header
        webReq.SetRequestHeader("Ocp-Apim-Subscription-Key", subKey);
        // send the request and wait for a response
        yield return webReq.SendWebRequest();

        // if we got an error - log it and return
        if(webReq.isHttpError){  
            Debug.Log(webReq.error);
            yield break;
        }
        // token generate success
        // otherwise set the access token
        this.accessToken = webReq.downloadHandler.text;
        Debug.Log("token:");
        Debug.Log(accessToken);
        // Debug.Log(webReq.downloadHandler.text);
    }

    public void readText(string text){
        // if(accessToken == null){
        StartCoroutine(GetSpeech(text));
        // }
    }

    public IEnumerator GetSpeech(string text){
        string body = @"<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='en-US'>
              <voice name='Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)'>" + text + "</voice></speak>";

        // create a web request and set the method to POST
        UnityWebRequest webReq = new UnityWebRequest(string.Format("https://{0}.tts.speech.microsoft.com/cognitiveservices/v1", region));        
        webReq.method = UnityWebRequest.kHttpVerbPOST;

        // create a download handler to receive the audio data
        webReq.downloadHandler = new DownloadHandlerBuffer();

        // set the body to be uploaded
        webReq.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
        webReq.uploadHandler.contentType = "application/ssml+xml";       
        
        // set the headers
        webReq.SetRequestHeader("Authorization", "Bearer " + accessToken);
        webReq.SetRequestHeader("User-Agent", resourceName);
        webReq.SetRequestHeader("X-Microsoft-OutputFormat", "riff-24khz-16bit-mono-pcm");

        // send the request and wait for a response
        yield return webReq.SendWebRequest();

        // if there's a problem - return
        if (webReq.isHttpError){
            Debug.Log(webReq.error);
            textt.GetComponent<Text>().text=webReq.error;
            yield break;
        }
        // textt.GetComponent<Text>().text="0";

        // play the audio
        // StartCoroutine(PlayTTS(webReq.downloadHandler.data));
        PlayTTS2(webReq.downloadHandler.data);
    }

    // converts the audio data and plays the cli

    // converts the audio data and plays the clip
    // IEnumerator PlayTTS (byte[] audioData){
    //     textt.GetComponent<Text>().text="1";
    //     // save the audio data temporarily as a .wav file
    //     string tempPath = Application.persistentDataPath + "/tts.wav";
    //     textt.GetComponent<Text>().text="2";
    //     System.IO.File.WriteAllBytes(tempPath, audioData);
    //     textt.GetComponent<Text>().text="3";
    //     // load that file in
    //     UnityWebRequest loader = UnityWebRequestMultimedia.GetAudioClip(tempPath, AudioType.WAV);
    //     textt.GetComponent<Text>().text="3";
    //     yield return loader.SendWebRequest();
    //     try{
    //     textt.GetComponent<Text>().text="4";
    //     // convert it to an audio clip
    //     AudioClip ttsClip = DownloadHandlerAudioClip.GetContent(loader);
    //     textt.GetComponent<Text>().text="success in set audio";
    //     // play it
    //     audioSource.PlayOneShot(ttsClip);
    //     }catch(Exception ex){
    //         textt.GetComponent<Text>().text=ex.Message+"";
    //     }


    // }
    private void PlayTTS2(byte[] audioData){
        audioSource.Stop();
        AudioClip ttsClip = AudioClip.Create("mySound", audioData.Length, 1, 24000, false,false);
        ttsClip.SetData(bytesToFloat(audioData), 0);
        audioSource.PlayOneShot(ttsClip);

    }

    public void explode(){
        audioSource.Stop();
        audioSource.PlayOneShot(explodedClip);
    }

    public void diffuse(){
        audioSource.Stop();
        audioSource.PlayOneShot(diffusedClip);
    }

    public static float[] bytesToFloat(byte[] byteArray)//byte[]数组转化为AudioClip可读取的float[]类型
    {
        float[] sounddata = new float[byteArray.Length / 2];
        for (int i = 0; i < sounddata.Length; i++)
        {
            sounddata[i] = bytesToFloat(byteArray[i * 2], byteArray[i * 2 + 1]);
        }
        return sounddata;
    }
      static float bytesToFloat(byte firstByte, byte secondByte) {
             // convert two bytes to one short (little endian)
	         //小端和大端顺序要调整
			 short s;
			 if (BitConverter.IsLittleEndian)
			             s = (short)((secondByte << 8) | firstByte);
			 else
			             s = (short)((firstByte<< 8) | secondByte );
			             // convert to range from -1 to (just below) 1
			             return s / 32768.0F;
         }
}
