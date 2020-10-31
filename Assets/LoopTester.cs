using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopTester : MonoBehaviour
{
    public List<AudioSource> Sources;
    public float AccelerationRate = 2f;
    private float rpms = 1000;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        rpms += Mathf.Abs(Input.GetAxis("Vertical")) * AccelerationRate * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Alpha1)) { PlaySound(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { PlaySound(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { PlaySound(2); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { PlaySound(3); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { PlaySound(4); }
    }

    private void PlaySound(int idx)
    {
        for (var i = 0; i < Sources.Count; i++)
        {
            if (i == idx)
            {
                if (!Sources[i].isPlaying)
                {
                    Sources[i].Play();
                }
            }
            else
            {
                Sources[i].Stop();
            }
        }
    }
}

