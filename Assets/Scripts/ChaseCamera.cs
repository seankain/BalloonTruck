using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseCamera : MonoBehaviour
{

    public GameObject ChaseObject;
    public float ChaseDistance = 10f;
    public float ChaseHeight = 10f;
    public float ChaseSpeed = 4f;
    private Camera cam;


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.LookAt(ChaseObject.transform);
        if (Vector3.Distance(cam.transform.position, ChaseObject.transform.position) > ChaseDistance) 
        {
            cam.transform.position = Vector3.MoveTowards(cam.transform.position, ChaseObject.transform.position, ChaseSpeed * Time.deltaTime);
        }
    }
}
