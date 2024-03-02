
// 플레이어 기본 뼈대.

/*
    - 구현 목록 -

    1. WSAD 이동 구현.
    2. 마우스를 통해 좌, 우 회전 구현.
    3. m_Speed 변수를 통해 이동속도 조절 가능.
    4. 좌클릭 함수 추가.
    5. 인벤토리 추가.
    6. 장비 장착 추가.

    Move함수와 ThreeView함수, LeftClick 함수는 상속받으면 Update문에 선언해줄 것.

    ex)
    public class Player : BasePlayerCharacter
    {
        private void Update()
        {
            ThreeView();
            Move();
        }
    }
 
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseRPG_V1
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasePlayerCharacter : BaseCharacter
    {
        // 애니메이터 이넘.
        public enum kMOVE
        {
            None = 0,
            Forward,
            Attack
        }

        // 이동속도.
        [SerializeField]
        protected int m_Speed;

        // 자신 리지드바디.
        [SerializeField]
        protected Rigidbody m_Rigidbody;

        public override void Initialization()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        public override void DisposeObject()
        {   
        }

        // 캐릭터 이동 WSAD.
        public virtual void Move()
        {
            // 키를 꾹 입력 했을 시.
            if (Input.GetKey(KeyCode.W))
            {
                W_Key();
            }

            if (Input.GetKey(KeyCode.S))
            {
                S_Key();
            }

            if (Input.GetKey(KeyCode.A))
            {
                A_Key();
            }

            if (Input.GetKey(KeyCode.D))
            {
                D_Key();
            }



            // 키를 떼었을 경우.
            if (Input.GetKeyUp(KeyCode.W))
            {
                W_KeyUp();
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                S_KeyUp();
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                A_KeyUp();
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                D_KeyUp();
            }
        }

        // 조이스틱 이동.
        protected void JoystickMove(float x, float z, Action callback)
        {
            Vector3 moveVec = new Vector3(x, 0.0f, z) * m_Speed * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position + moveVec);

            // 움직이는 중인가?.
            if (moveVec.sqrMagnitude == 0)
            {
                return;
            }

            Quaternion dirQuqt = Quaternion.LookRotation(moveVec);
            Quaternion moveQuqt = Quaternion.Slerp(m_Rigidbody.rotation, dirQuqt, 0.3f);
            m_Rigidbody.MoveRotation(moveQuqt);

            if (callback == null)
            {
                return;
            }

            // 이동 후 콜백.
            callback();
        }

        // 3인칭 마우스 회전.
        public virtual void ThreeView()
        {
            // 좌, 우 회전을 위해 마우스 회전 값 구해옴.
            float posX = Input.GetAxis("Mouse X");

            // 마우스 좌우 회전.
            Quaternion qt = transform.rotation;
            qt.eulerAngles = new Vector3(qt.eulerAngles.x, qt.eulerAngles.y + posX, qt.eulerAngles.z);

            transform.rotation = qt;
        }

        // 좌클릭 함수.
        public virtual void LeftClick()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                OnLeftClick();   
            }
        }

        // 좌클릭 이벤트 구현 함수.
        public virtual void OnLeftClick()
        {
        }

        // 앞 움직임.
        public virtual void W_Key()
        {
            Vector3 moveVec = new Vector3(0.0f, 0.0f, 1.0f) * m_Speed * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position + moveVec);            
        }

        // 앞 움직임 키 뗐을 시.
        public virtual void W_KeyUp()
        {
            m_Rigidbody.velocity = Vector3.zero;
        }

        // 뒤 움직임.
        public virtual void S_Key()
        {
            Vector3 moveVec = new Vector3(0.0f, 0.0f, -1.0f) * m_Speed * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position + moveVec);
        }

        // 뒤 움직임 키 뗏을 시.
        public virtual void S_KeyUp()
        {
            m_Rigidbody.velocity = Vector3.zero;
        }

        // 왼쪽 움직임.
        public virtual void A_Key()
        {
            Vector3 moveVec = new Vector3(-1.0f, 0.0f, 0.0f) * m_Speed * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position + moveVec);
        }

        // 왼쪽 움직임 키 뗏을 시.
        public virtual void A_KeyUp()
        {
            m_Rigidbody.velocity = Vector3.zero;
        }

        // 오른쪽 움직임.
        public virtual void D_Key()
        {
            Vector3 moveVec = new Vector3(1.0f, 0.0f, 0.0f) * m_Speed * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position + moveVec);
        }

        // 오른쪽 움직임 키 뗏을 시.
        public virtual void D_KeyUp()
        {
            m_Rigidbody.velocity = Vector3.zero;
        }
    }
}