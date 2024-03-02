// 싱글톤 뼈대.

using System;
using UnityEngine;

namespace BaseRPG_V1
{
    public class BaseSingleton<T> : MonoBehaviour where T : BaseSingleton<T>
    {
        // 종료 여부 확인.
        private static bool isQuit = false;

        private static T m_Instance = null;

        public static T Instance
        {
            get
            {
                if (m_Instance == null && isQuit == false)
                {
                    Initialzation();
                }
                
                return m_Instance;
            }
        }

        // 싱글톤 초기화.
        private static void Initialzation()
        {
            // 타입.
            Type type = typeof(T);

            // 오브젝트 생성.
            GameObject obj = new GameObject();

            // 오브젝트 이름 재정의.
            obj.name = type.ToString();

            // 싱글톤 스크립트 생성 후 추가.
            m_Instance = obj.AddComponent<T>();
        }

        private void Awake() 
        {
            DontDestroyOnLoad(this);
        }

        public void OnApplicationQuit()
        {
            isQuit = true;
        }
    }
}