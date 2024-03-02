
// 풀매니저에 영향을 받을 오브젝트.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace BaseRPG_V1
{
    public abstract class BaseObject : MonoBehaviourPunCallbacks
    {
        // 풀 오브젝트 생성 시 실행.
        public abstract void Initialization();

        // 풀 오브젝트 Push 할 때마다 실행.
        public abstract void DisposeObject();

        // 로컬 포지션 초기화.
        public void ResetLocalPosition()
        {
            transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }
}