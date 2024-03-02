using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseRPG_V1;
using UnityEngine.UI;

public class LoadingManager : BaseSingleton<LoadingManager>
{
    private GameObject m_Panel;
    
    private Text m_TextOfLoading;

    public void Init(GameObject obj, Text text)
    {
        m_Panel = obj;
        m_TextOfLoading = text;
    }

    public void LoadingOn(string text)
    {
        m_Panel.SetActive(true);
        m_TextOfLoading.text = text;
    }

    public void LoadingOff()
    {
        m_Panel.SetActive(false);
    }
}
