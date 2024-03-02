using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialization : MonoBehaviour
{
    void Awake()
    {
        Screen.SetResolution(1280, 720, false);
        Application.targetFrameRate = 60;

        PoolManager.Instance.Create<VariableJoystick>("Prefabs/Variable Joystick");
        PoolManager.Instance.Create<CameraController>("Prefabs/Main Camera");

        BGMManager.Instance.Initialization();
    }

    // Start is called before the first frame update
    void Start()
    {        
        PoolManager.Instance.Pop<CameraController>();
    }
}