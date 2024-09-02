using System;
using System.Collections.Generic;
using UnityEngine;
using BaseRPG_V1;

public class PoolManager : BaseSingleton<PoolManager>
{
    // 풀 오브젝트 비활성화 리스트.
    private List<BaseObject> m_PoolDisable = new List<BaseObject>();

    // 풀 오브젝트 활성화 리스트.
    private List<BaseObject> m_PoolEnable = new List<BaseObject>();

    // 풀 오브젝트 생성.
    public void Create<T>(string path, int count = 1) where T : BaseObject
    {
        // 재귀함수 종료.
        if (count-- <= 0)
            return;

        BaseObject component = null;

        // 번들에서 프리팹 생성.
        // BundleManager.Instance.Instantiate(name, (GameObject obj) => 
        // {
        //     // null 체크.
        //     if (obj == null)
        //     {
        //         Debug.Log("@@ " + name + "오브젝트가 존재하지 않아 생성할 수 없습니다");
        //         return;
        //     }
            
        //     // 오브젝트 풀매니저로 기본 부모 설정.
        //     obj.transform.SetParent(transform);
            
        //     // 오브젝트 비활성화.
        //     obj.SetActive(false);

        //     // 컴포넌트 가져옴.
        //     component = obj.GetComponent<T>();

        //     // 컴포넌트 null 체크.
        //     if (component == null)
        //     {
        //         Debug.Log("@@ " + obj.name + " 프리팹에서 컴포넌트가 존재하지 않아 생성할 수 없습니다");
        //         return;
        //     }

        //     // 생성 시 함수 실행.
        //     component.Initialization();

        //     // 비활성화 리스트 추가.
        //     m_PoolDisable.Add(component);

        //     // 갯수만큼 생성.
        //     Create<T>(name, count);
        // });

        T resource = Resources.Load<T>(path);

        if (resource == null)
        {
            throw new Exception("@@ " + path + " 경로에 리소스가 존재하지 않아 생성할 수 없습니다");
        }

        T obj = Instantiate(resource);

        // null 체크.
        if (obj == null)
        {
            throw new Exception("@@ " + resource.name + "오브젝트가 존재하지 않아 생성할 수 없습니다");
        }

        // 오브젝트 풀매니저로 기본 부모 설정.
        obj.transform.SetParent(transform);

        // 오브젝트 비활성화.
        obj.gameObject.SetActive(false);

        // 컴포넌트 가져옴.
        component = obj.GetComponent<T>();

        // 컴포넌트 null 체크.
        if (component == null)
        {
            Debug.Log("@@ " + obj.name + " 프리팹에서 컴포넌트가 존재하지 않아 생성할 수 없습니다");
            return;
        }

        // 생성 시 함수 실행.
        component.Initialization();

        // 비활성화 리스트 추가.
        m_PoolDisable.Add(component);

        // 갯수만큼 생성.
        Create<T>(path, count);
    }

    // 풀 오브젝트에서 가져온다.
    public T Pop<T>(Transform parent = null) where T : BaseObject
    {
        // 풀 활성화에 담을 컴포넌트.
        T component = default(T);

        // 타입.
        Type type = typeof(T);

        // 풀 활성화에 담을 컴포넌트를 찾는다.
        component = m_PoolDisable.Find(foundData => foundData.GetType() == type).GetComponent<T>();

        // null 체크.
        if (component == null)
        {
            Debug.Log("@@ " + type.ToString() + " 스크립트를 풀 오브젝트에 할당(Create) 해주세요");
            return null;
        }

        // 활성화 리스트 추가.
        m_PoolEnable.Add(component);

        // 비활성화 리스트 제거.
        m_PoolDisable.Remove(component);

        // 부모 설정.
        component.transform.SetParent(parent);

        // 위치랑 크기 원래대로.
        ReturnTransform(component.GetComponent<RectTransform>());

        // Trasform 달린 객체는 회전 값도 원래대로.
        ReturnTransform(component.GetComponent<Transform>());

        component.gameObject.SetActive(true);        

        return component;
    }

    // 풀 오브젝트에서 가져온다.
    public T Pop<T>(string tag) where T : BaseObject
    {
        // 풀 활성화에 담을 컴포넌트.
        T component = default(T);

        // 타입.
        Type type = typeof(T);

        // 풀 활성화에 담을 컴포넌트를 찾는다.
        component = m_PoolDisable.Find(foundData => foundData.GetType() == type).GetComponent<T>();

        // null 체크.
        if (component == null)
        {
            Debug.Log("@@ " + type.ToString() + " 스크립트를 풀 오브젝트에 할당 해주세요");
            return null;
        }

        // 활성화 리스트 추가.
        m_PoolEnable.Add(component);

        // 비활성화 리스트 제거.
        m_PoolDisable.Remove(component);

        // string으로 해당 태그 오브젝트 찾아온다.
        Transform parent = GameObject.FindWithTag(tag).transform;

        // 태그 존재 null 체크.
        if (parent == null)
        {
            Debug.Log("@@ " + tag + " 태그가 존재하지 않습니다");
            return null;
        }

        // 부모 설정.
        component.transform.SetParent(parent);

        // 위치랑 크기 원래대로.
        ReturnTransform(component.GetComponent<RectTransform>());
        
        component.gameObject.SetActive(true);

        return component;
    }

    // 풀 오브젝트로 돌려보낸다.
    public void Push(BaseObject obj)
    {
        // 포톤 함수로 인하여 풀매니저가 null인 상태에서 이게 호출 되서 Missing 오류 뜨는 경우가 있기에 조건문 넣어줌.
        if (this == null)
        {
            return;
        }

        // UI 기본 좌표.
        Vector3 vector = Vector3.zero;

        // UI Rect.
        RectTransform rect = obj.GetComponent<RectTransform>();

        // Rect Transform 좌표 미리 담아둠.
        if (rect)
        {
            vector = rect.anchoredPosition;
        }

        obj.DisposeObject();
        obj.gameObject.SetActive(false);

        // 부모 변경.
        obj.gameObject.transform.SetParent(transform);

        // 부모 변경 시 좌표가 바뀌므로 담아둔 좌표로 되돌림.
        if (rect)
        {
            rect.localPosition = vector;
        }
        
        // 비활성화 리스트로 보냄.
        m_PoolDisable.Add(obj);

        // 활성화 리스트에서 제거.
        m_PoolEnable.Remove(obj);
    }

    // 리스트에 담아진 오브젝트들을 풀로 돌려보낸다.
    public void PushList<T>(List<T> list) where T : BaseObject
    {
        list.ForEach((slot) => Push(slot));
        list.Clear();
    }

    // 풀 오브젝트 활성화 리스트에서 T 타입 가져온다.
    public T GetObject<T>()
    {
        // 풀 오브젝트.
        T component = default(T);

        // 타입.
        Type type = typeof(T);

        // 풀 활성화에 담아진 오브젝트 중에 풀 오브젝트 가져온다.
        var data = m_PoolEnable.Find(foundData => foundData.GetType() == type);

        // null 체크.
        if (data == null)
            return component;

        // 컴포넌트 가져옴.
        component = data.GetComponent<T>();

        return component;
    }

    // 다른 부모로 옮겨서 어긋난 AnchoredPosition이랑 Scale을 원래대로 돌려놓은다 (UI용).
    private void ReturnTransform(RectTransform transform)
    {
        // null 체크.
        if (transform == null)
            return;

        // 해상도 크기에 따른 UI 크기 조정.
        if ((transform.anchorMin.x == transform.anchorMax.x) && (transform.anchorMin.y == transform.anchorMax.y))
        {
            // stretch가 아닐 때, 부모가 바뀌면 AnchoredPosition을 다시 원래 값으로 변경.
            transform.anchoredPosition = transform.position;
        }
        else
        {
            // stretch 일 경우, 상하좌우 크기 0.
            transform.offsetMin = new Vector2(0.0f, 0.0f);
            transform.offsetMax = new Vector2(0.0f, 0.0f);
        }

        // 부모가 바뀌면 Scale도 원래 값으로 변경. 이는 해상도에 따라 크기가 달라지는걸 막기 위함.
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    // 다른 부모로 옮겨서 어긋난 Position, Scale, EulerAngle값을 원래대로 돌려놓은다 (3D 오브젝트용).
    private void ReturnTransform(Transform transform)
    {
        // null 체크.
        if (transform == null)
            return;

        // 부모가 바뀌면 Position을 다시 원래 값으로 변경.
        transform.localPosition = transform.position;

        // transform 달린 객체는 회전 값도 원래 값으로 변경.
        transform.localEulerAngles = transform.eulerAngles;

        // 부모가 바뀌면 Scale도 원래 값으로 변경. 이는 해상도에 따라 크기가 달라지는걸 막기 위함.
        transform.localScale = new Vector3(1.0f, transform.localScale.y, 1.0f);
    }
}
