using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseRPG_V1;
using Photon.Realtime;
using Photon.Pun;

public class CreateMap : BaseObject
{
    [SerializeField]
    private GameObject m_ObjectOfFloor;

    // X축 생성 갯수.
    [SerializeField]
    private int countX = 0;

    // Z축 생성 갯수.
    [SerializeField]
    private int countZ = 0;

    // 발판 하나 당 거리.
    private const float POS_X = 2.0f;

    public override void Initialization()
    {
        
    }

    public override void DisposeObject()
    {
        
    }

    void Start()
    {
        CreateFloor();
    }

    // 바닥 생성.
    private void CreateFloor()
    {
        if (m_ObjectOfFloor == null)
        {
            throw new Exception("m_ObjectOfFloor 오브젝트가 비어 있습니다.");
        }

        // z번째 줄에 생성 시킬 준비.
        for (int z = 0; z < countZ; z++)
        {
            // z번째 줄에서 x갯수만큼 바닥 생성.
            float posX = 0;
            for (int x = 0; x < countX; x++)
            {
                GameObject floor = PhotonNetwork.InstantiateRoomObject("Prefabs/Floor", new Vector3(posX, 0.0f, z * POS_X), Quaternion.identity);

                if (floor == null)
                {
                    continue;
                }
                
                floor.transform.SetParent(transform);

                // 다음 좌표 값.
                posX += POS_X;
            }
        }
    }
}
