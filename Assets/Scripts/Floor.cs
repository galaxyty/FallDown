using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseRPG_V1;
using Photon.Realtime;
using Photon.Pun;

public class Floor : BaseObject
{
    [SerializeField]
    private float m_ScaleX;

    [SerializeField]
    private float m_ScaleY;

    [SerializeField]
    private float m_ScaleZ;

    [SerializeField]
    private Rigidbody m_Rigidbody;

    [SerializeField]
    private PhotonView m_PV;

    public override void Initialization()
    {
        m_Rigidbody.isKinematic = true;
    }

    public override void DisposeObject()
    {
        m_Rigidbody.isKinematic = false;
    }

    void Start()
    {
        transform.localScale = new Vector3(m_ScaleX, m_ScaleY, m_ScaleZ);
        m_Rigidbody.isKinematic = true;
    }

    // 닿으면 해제.
    [PunRPC]
    private void DownFloor()
    {
        m_Rigidbody.isKinematic = false;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (GameManager.Instance.STATE != GameManager.kSTATE.PLAY)  
        {
            return;
        }
        
        if (collision.transform.tag.Contains("Player"))
        {
            m_PV.RPC("DownFloor", RpcTarget.All);
        }
    }
}
