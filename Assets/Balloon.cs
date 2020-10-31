using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public float DespawnHeight = 30f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.y > DespawnHeight)
        {
            
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{   
    //}

    //private void OnCollisionStay(Collision collision)
    //{

    //}

    //private void OnCollisionExit(Collision collision)
    //{

    //}
}
