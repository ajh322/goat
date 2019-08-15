using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MicrophoneListener : MonoBehaviour
{
    [SerializeField] private Text TextVol;
    public float sensitivity = 100;
    public float loudness = 0;
    public float pitch = 0;
    AudioSource _audio;
    public float RmsValue;
    public float DbValue;
    public float PitchValue;
    private const int QSamples = 1024;
    private const float RefValue = 0.1f;
    private const float Threshold = 0.02f;
    float[] _samples;
    private float[] _spectrum;
    private float _fSample;
     public bool startMicOnStartup = true;
     public bool stopMicrophoneListener = false;
     public bool startMicrophoneListener = false;
     private bool microphoneListenerOn = false;
     public bool disableOutputSound = false; 
     AudioSource src;
     public AudioMixer masterMixer;
     float timeSinceRestart = 0;
     void Start() {        
         //start the microphone listener
         if (startMicOnStartup) {
             RestartMicrophoneListener ();
             StartMicrophoneListener ();
             
             _audio = GetComponent<AudioSource> ();
             _audio.clip = Microphone.Start (null, true, 10, 44100);
             _audio.loop = true;
             while (!(Microphone.GetPosition(null) > 0))  {}
             _audio.Play();
             _samples = new float[QSamples];
             _spectrum = new float[QSamples];
             _fSample = AudioSettings.outputSampleRate;
         }
     }
 
     void Update(){    
 
         if (stopMicrophoneListener) {
             StopMicrophoneListener ();
         }
         if (startMicrophoneListener) {
             StartMicrophoneListener ();
         }
         stopMicrophoneListener = false;
         startMicrophoneListener = false;
 
         MicrophoneIntoAudioSource (microphoneListenerOn);
 
         DisableSound (!disableOutputSound);
 
         loudness = GetAveragedVolume() * sensitivity;
         //loudness는 소리의 크기
         //Pitch는 소리의 높낮이라고 생각하면 된다. PitchValue 값을 가져다가 사용하면 된다. 
         GetPitch();
         
          //아래는 커스텀 한 소스로 플레이어 스크립트의 CurrentVolume값을 계속해서 바꿔준다.
         GameManager.Instance.CurrentVolume = loudness;
         TextVol.text = "MikeVolume::" + loudness;
     }
 

    float GetAveragedVolume() {
        float[] data = new float[256];
        float a = 0;
        _audio.GetOutputData (data, 0);
        foreach(float s in data) 
        {
            a+=Mathf.Abs(s);
        }
        return a/256;
    }

    void GetPitch() {
        GetComponent<AudioSource>().GetOutputData(_samples, 0); // fill array with samples
        int i;
        float sum = 0;
        for (i = 0; i < QSamples; i++)
        {
            sum += _samples[i] * _samples[i]; // sum squared samples
        }
        RmsValue = Mathf.Sqrt(sum / QSamples); // rms = square root of average
        DbValue = 20 * Mathf.Log10(RmsValue / RefValue); // calculate dB
        if (DbValue < -160) DbValue = -160; // clamp it to -160dB min
        // get sound spectrum
        GetComponent<AudioSource>().GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0;
        var maxN = 0;
        for (i = 0; i < QSamples; i++)
        { // find max 
            if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
                continue;
            maxV = _spectrum[i];
            maxN = i; // maxN is the index of max
        }
        float freqN = maxN; // pass the index to a float variable
        if (maxN > 0 && maxN < QSamples - 1)
        { // interpolate index using neighbours
            var dL = _spectrum[maxN - 1] / _spectrum[maxN];
            var dR = _spectrum[maxN + 1] / _spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        PitchValue = freqN * (_fSample / 2) / QSamples; // convert index to frequency
    }
 
     //stops everything and returns audioclip to null
     public void StopMicrophoneListener(){
         //stop the microphone listener
         microphoneListenerOn = false;
         //reenable the master sound in mixer
         disableOutputSound = false;
         //remove mic from audiosource clip
         src.Stop ();
         src.clip = null;
 
         Microphone.End (null);
     }
 
 
     public void StartMicrophoneListener(){
         //start the microphone listener
         microphoneListenerOn = true;
         //disable sound output (dont want to hear mic input on the output!)
         disableOutputSound = true;
         //reset the audiosource
         RestartMicrophoneListener ();
     }
     
     
     //controls whether the volume is on or off, use "off" for mic input (dont want to hear your own voice input!) 
     //and "on" for music input
     public void DisableSound(bool SoundOn){
         
         float volume = 0;
         
         if (SoundOn) {
             volume = 0.0f;
         } else {
             volume = -80.0f;
         }
         
         masterMixer.SetFloat ("MasterVolume", volume);
     }
 
 
 
     // restart microphone removes the clip from the audiosource
     public void RestartMicrophoneListener(){
 
         src = GetComponent<AudioSource>();
         
         //remove any soundfile in the audiosource
         src.clip = null;
 
         timeSinceRestart = Time.time;
 
     }
 
     //puts the mic into the audiosource
     void MicrophoneIntoAudioSource (bool MicrophoneListenerOn){
 
         if(MicrophoneListenerOn){
             //pause a little before setting clip to avoid lag and bugginess
             if (Time.time - timeSinceRestart > 0.5f && !Microphone.IsRecording (null)) {
                 src.clip = Microphone.Start (null, true, 10, 44100);
                 
                 //wait until microphone position is found (?)
                 while (!(Microphone.GetPosition (null) > 0)) {
                 }
                 
                 src.Play (); // Play the audio source
             }
         }
     }}
