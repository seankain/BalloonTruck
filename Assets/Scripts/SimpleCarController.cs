using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    public float CurrentMotor { get { return motor; } }
    private float motor = 0;
    public void FixedUpdate()
    {
        motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
        }
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}

public enum ThrottleState
{
    Idle,
    RevOn,
    RevOff,
    Accelerating,
    Running
}

public class ThrottleStateProcessor
{

    public int WindowSize { get; set; } = 30;
    public List<float> window = new List<float>();
    public ThrottleState CurrentThrottleState { get; private set; } = ThrottleState.Idle;
    private int currentPos = 0;
    private List<float> RevShape;
    private List<float> ConstantSpeedShape;
    private List<float> AccelerateShape;
    private List<float> IdleShape;

    [DebugGUIGraph(min: 0, max: 1, r: 0, g: 1, b: 0, autoScale: true)]
    private float revGraphVal;
    [DebugGUIGraph(min: 0, max: 1, r: 0, g: 1, b: 0, autoScale: true)]
    private float accelerateGraphVal;


    public ThrottleStateProcessor(int windowSize)
    {
        WindowSize = windowSize;
        window.AddRange(Enumerable.Repeat(0f, windowSize));
        RevShape = new RevInputShape().GenerateValues(windowSize);
        AccelerateShape = new FullAccelerateInputShape2().GenerateValues(windowSize);
        ConstantSpeedShape = new ConstantSpeedInputShape().GenerateValues(windowSize);
        IdleShape = new IdleInputShape().GenerateValues(windowSize);
    }

    public void AppendInputValue(float inputValue)
    {
        window[currentPos] = inputValue;
        currentPos++;
        if (currentPos == WindowSize - 1)
        {
            currentPos = 0;
            UpdateThrottleState();
            UpdateDebugGraphs();
        }
    }

    private void UpdateDebugGraphs()
    {
        accelerateGraphVal = AccelerateShape[currentPos];
        revGraphVal = RevShape[currentPos];

    }

    private void UpdateThrottleState()
    {
        var revDist = ThrottleInputShape.GetDistance(window, RevShape);
        var accDist = ThrottleInputShape.GetDistance(window, AccelerateShape);
        var conDist = ThrottleInputShape.GetDistance(window, ConstantSpeedShape);
        var idlDist = ThrottleInputShape.GetDistance(window, IdleShape);
        var lowest = revDist;
        if(accDist < lowest) { lowest = accDist; }
        if(conDist < lowest) { lowest = conDist; }
        if(idlDist < lowest) { lowest = idlDist; }
        if(revDist == lowest) { CurrentThrottleState = ThrottleState.RevOn; };
        if(accDist == lowest) { CurrentThrottleState = ThrottleState.Accelerating; }
        if(conDist == lowest) { CurrentThrottleState = ThrottleState.Running; }
        if(idlDist == lowest) { CurrentThrottleState = ThrottleState.Idle; }
        Debug.Log($"Rev:{revDist} | Acc:{accDist} | Run: {conDist} | Idl: {idlDist}");
        Debug.Log(CurrentThrottleState);
    }

}

public abstract class ThrottleInputShape
{
    public abstract List<float> GenerateValues(int windowSize);

    public static float GetDistance(List<float> inputWindow,List<float> shape) 
    {
        var windowMax = inputWindow.Max();
        var relativeShape = shape.Select(v => v * windowMax / 1).ToList();
        var sum = 0f;
        //Trying MSE
        for (var i = 0; i < inputWindow.Count; i++)
        {
            sum += Mathf.Pow(inputWindow[i] - relativeShape[i], 2);
        }
        return (1f / inputWindow.Count) * sum;
    }

}

public class RevInputShape : ThrottleInputShape
{
    public override List<float> GenerateValues(int windowSize)
    {
        List<float> values = new List<float>();
        for (var i = 0; i < windowSize; i++) 
        {
            values.Add(-(Mathf.Pow(i, 2) - windowSize * i) / 225);
        }
        return values;
    }
}

public class FullAccelerateInputShape : ThrottleInputShape
{
    public override List<float> GenerateValues(int windowSize)
    {
        List<float> values = new List<float>();
        //(x-15)/(1 + |x-15|) + 1
        for (var i = 0; i < windowSize; i++)
        {
            values.Add(((i - windowSize / 2f) / (1 + Mathf.Abs(i - (windowSize / 2f))) + 1));
        }
        return values;
    }
}

public class FullAccelerateInputShape2 : ThrottleInputShape
{
    public override List<float> GenerateValues(int windowSize)
    {
        List<float> values = new List<float>();
        for (var i = 0; i < windowSize; i++)
        {
            values.Add((1/windowSize)*i);
        }
        return values;
    }
}

public class ConstantSpeedInputShape : ThrottleInputShape
{
    public override List<float> GenerateValues(int windowSize)
    {
        List<float> values = new List<float>();
        for (var i = 0; i < windowSize; i++)
        {
            values.Add(1);
        }
        return values;
    }
}

public class IdleInputShape : ThrottleInputShape
{
    public override List<float> GenerateValues(int windowSize)
    {
        List<float> values = new List<float>();
        for (var i = 0; i < windowSize; i++)
        {
            values.Add(0);
        }
        return values;
    }
}
