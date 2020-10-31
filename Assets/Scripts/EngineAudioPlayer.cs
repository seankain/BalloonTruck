using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineAudioPlayer : MonoBehaviour
{
    public List<AudioSource> IdleSounds;
    public List<AudioSource> RevSounds;
    public List<AudioSource> AccelerationSounds;
    public List<AudioSource> RunningSounds;
    public float InputDeltaThreshold = 0.3f;
    public float DeltaFrameSpan = 5;
    private int elapsedFrames = 0;
    private float throttleDelta = 0;
    private float prevThrottle = 0;
    private ThrottleState throttleState = ThrottleState.Idle;
    private AudioSource activeAudioSource;
    private SimpleCarController carController;
    private Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponent<SimpleCarController>();
        rb = GetComponent<Rigidbody>();
        activeAudioSource = IdleSounds[0];
        activeAudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        var currentThrottle = Input.GetAxis("Vertical");

        Debug.Log(rb.velocity);
        
        //Idling or rev down
        if (currentThrottle == 0)
        {
            if (prevThrottle > 0)
            {
                throttleState = ThrottleState.RevOff;
                Debug.Log("RevOff");
            }
            else
            {
                //We're playing idle
                if (throttleState == ThrottleState.Idle)
                {
                    return;
                }
                //Still idle, if reving off let audio complete
                if (throttleState == ThrottleState.RevOff)
                {
                    //if (!activeAudioSource.isPlaying)
                    //{
                    //    Debug.Log("RevOff Done");
                    //    throttleState = ThrottleState.Idle;
                    //}
                    Debug.Log("RevOff to idle");
                    throttleState = ThrottleState.Idle;
                }
       
                //else
                //{
                //    if (throttleState == ThrottleState.Idle)
                //    {
                //        return;
                //    }
                //    else
                //    {
                //        throttleState = ThrottleState.Idle;
                //        //prevThrottle = currentThrottle;
                //        Debug.Log("Idle");
                //    }
                //    //return;
                //}
               // throttleState = ThrottleState.Idle;
               // Debug.Log("Idle");
               // return;
            }
        }
        //Keeping a constant speed
        if (currentThrottle != 0 && Mathf.Abs(currentThrottle - prevThrottle) <= InputDeltaThreshold)
        { 
            //Already playing running which loops
            if (throttleState == ThrottleState.Running)
            {
                return;
            }
            Debug.Log("Running");
            throttleState = ThrottleState.Running;
        }
        //Accelerating
        if (currentThrottle > prevThrottle) {
            if (throttleState == ThrottleState.Accelerating) 
            {
                return; 
            }
            Debug.Log("Accelerating");
            throttleState = ThrottleState.Accelerating;
        }
        //state changed play sound
        //Debug.Log(currentThrottle);
        prevThrottle = currentThrottle;
        SelectSound();

    }

    private void SelectSound()
    {
        Debug.Log(throttleState);
        if (throttleState == ThrottleState.Idle)
        {
            Debug.Log("Select idle");
            PlayRandom(this.IdleSounds);
        }
        if (throttleState == ThrottleState.Accelerating)
        {
            PlayRandom(this.AccelerationSounds);
        }
        if (throttleState == ThrottleState.Running)
        {
            PlayRandom(this.RunningSounds);
        }
        if (throttleState == ThrottleState.RevOff)
        {
            PlayRandom(this.RevSounds);
        }
    }

    private void PlayRandom(List<AudioSource> sources)
    {
        var idx = Random.Range(0, sources.Count - 1);
        if (sources[idx] != null)
        {
            activeAudioSource.Stop();
            activeAudioSource = sources[idx];
            activeAudioSource.Play();
        }


    }
}
