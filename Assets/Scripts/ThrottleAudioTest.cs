using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrottleAudioTest : MonoBehaviour
{
    public AudioSource ThrottleAudio;
    public AudioSource IdleAudio;
    public AudioSource LowRpm;
    public AudioSource LowMidRpm;
    public AudioSource MidRpm;
    public AudioSource HighRpm;
    public AudioSource Backfire;

    [SerializeField]
    public List<EngineSoundRule> EngineSoundRules;

    // Works with regular fields
    //[DebugGUIGraph(min: 0, max: 1, r: 0, g: 1, b: 0, autoScale: true)]
    private float throttleVal;

    public float AccelerationRate = 10f;
    public float DecelerationRate = 10f;
    public float BackFireProbability = 0.1f;
    private float rpms = 1000;
    private float maxRpms = 10000;

    // Start is called before the first frame update
    void Start()
    {
        maxRpms = GetMaxRpms();
        LowRpm.volume = 0;
        LowRpm.Play();
        LowMidRpm.volume = 0;
        LowMidRpm.Play();
        MidRpm.volume = 0;
        MidRpm.Play();
        HighRpm.volume = 0;
        HighRpm.Play();
        IdleAudio.volume = 1;
        IdleAudio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        var throttle = Input.GetAxis("Vertical");
        if (throttle < throttleVal) { DoBackfire(); }
        throttleVal = throttle;
        if (throttle != 0)
        {
            rpms += Mathf.Abs(Input.GetAxis("Vertical")) * AccelerationRate * Time.deltaTime;
            if (rpms >= maxRpms) { rpms = maxRpms; }
        }
        else
        {

            rpms -= DecelerationRate * Time.deltaTime;
            //idle
            if (rpms < 1000) { rpms = 1000; IdleAudio.pitch = 1; IdleAudio.volume = 1; }


        }
        Debug.Log(rpms);
        SetSoundLevels(rpms);
    }

    private float GetMaxRpms()
    {
        var maxRpms = Mathf.NegativeInfinity;
        foreach (var esr in EngineSoundRules) { 
        foreach(var sl in esr.SoundLevels)
            {
                if(sl.EndRpms > maxRpms)
                {
                    maxRpms = sl.EndRpms;
                }
            }
        }
        Debug.Log($"Max rpms: {maxRpms}");
        return maxRpms;
    }

    private void DoBackfire()
    {
        if (UnityEngine.Random.Range(0.000f, 1.000f) <= BackFireProbability && !Backfire.isPlaying){Backfire.Play(); };
    }

    public void SetSoundLevels(float rpms)
    {
        foreach (var esr in EngineSoundRules)
        {
            foreach (var sl in esr.SoundLevels)
            {
                if (sl.StartRpms <= rpms && sl.EndRpms >= rpms)
                {
                    //rpms 1500 rpm start:1000 end: 3000. 500 / 1000 = 0.5 volume
                    //rpms 2500 rpm start:1000 end: 3000 1500 /
                    esr.EngineSound.pitch = (Mathf.Abs((sl.EndPitch - sl.StartPitch)) * (rpms-sl.StartRpms) / (sl.EndRpms - sl.StartRpms)) + sl.StartPitch;
                    esr.EngineSound.volume = (-(sl.StartVolume - sl.EndVolume) * (rpms - sl.StartRpms) / (sl.EndRpms - sl.StartRpms)) + sl.StartVolume;
                    break;
                }
                else {
                    esr.EngineSound.volume = 0;
                }
            }
        }
    }

}

[Serializable]
public class EngineSoundRule
{
    public AudioSource EngineSound;
    public List<EngineSoundLevel> SoundLevels;
}
[Serializable]
public class EngineSoundLevel
{
    public float StartRpms = 0f;
    public float EndRpms = 0f;
    public float StartVolume = 0f;
    public float EndVolume = 0f;
    public float StartPitch = 0f;
    public float EndPitch = 0f;
}
