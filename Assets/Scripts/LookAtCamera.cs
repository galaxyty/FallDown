using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Camera;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = m_Camera.transform.rotation;
    }
}
