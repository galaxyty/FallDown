using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseRPG_V1;
using UnityEngine.UI;

public class MessageManager : BaseSingleton<MessageManager>
{
    private GameObject m_Panel;

    private Text m_TextOfMessage;

    public void Init(GameObject obj, Text text)
    {
        m_Panel = obj;
        m_TextOfMessage = text;
    }

    public void MessageOn(string text)
    {
        m_Panel.SetActive(true);
        m_TextOfMessage.text = text;
    }

    public void MessageOff()
    {
        m_Panel.SetActive(false);
    }
}
