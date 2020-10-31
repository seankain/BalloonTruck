using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonLauncher : MonoBehaviour
{
    public Transform BalloonLaunchLocation;
    public GameObject BalloonPrefab;
    private bool firing = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Fire1") > 0 && !firing)
        {
            firing = true;
            Instantiate(BalloonPrefab, BalloonLaunchLocation.position, Quaternion.identity, null);
            return;
        }
        if (Input.GetAxis("Fire1") == 0 && firing)
        {
            firing = false;
        }
    }
}
