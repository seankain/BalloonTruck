using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSpin : MonoBehaviour
{
    // Start is called before the first frame update
    public List<VisualWheelInfo> WheelInfos;

    // Update is called once per frame
    void Update()
    {
        foreach (var w in WheelInfos)
        {
            //w.transform.Rotate(Vector3.forward, -RotationSpeed * Time.deltaTime);
            // w.WheelMeshObject.transform.Rotate(Vector3.forward, w.SyncedCollider.rpm * 60 * Time.deltaTime);
            ApplyLocalPositionToVisuals(w);
        }
    }

    public void ApplyLocalPositionToVisuals(VisualWheelInfo wheelInfo)
    {
        Vector3 position;
        Quaternion rotation;
        wheelInfo.SyncedCollider.GetWorldPose(out position, out rotation);

        //might want this for uneven terrain but the colliders are wider than the tire positions
        //wheelInfo.WheelMeshObject.transform.position = position;
        wheelInfo.WheelMeshObject.transform.rotation = rotation * Quaternion.Euler(0, 270f, 0);
    }

}
[System.Serializable]
public class VisualWheelInfo
{
    public GameObject WheelMeshObject;
    public WheelCollider SyncedCollider;
}
