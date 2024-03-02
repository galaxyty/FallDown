using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseRPG_V1;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerController : BasePlayerCharacter
{
    public enum kANI_STATE
    {
        None,
        Idle,
        Move
    }

    public kANI_STATE Ani_State = kANI_STATE.None;

    private VariableJoystick m_Joystick;

    [SerializeField]
    private PhotonView PV;

    [SerializeField]
    private Text m_TextOfNickname;

    [SerializeField]
    private Animator m_Animator;

    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            m_Joystick = PoolManager.Instance.Pop<VariableJoystick>("Canvas");
        }

        m_TextOfNickname.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        m_TextOfNickname.color = PV.IsMine ? Color.green : Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Joystick == null)
        {
            return;
        }

        Ani_State = kANI_STATE.Idle;

        JoystickMove(m_Joystick.Horizontal, m_Joystick.Vertical, () =>
        {
            Ani_State = kANI_STATE.Move;
        });

        m_Animator.SetInteger("State", (int)Ani_State);

        if (transform.localPosition.y <= Constants.kGAMEOVER_POSITION)
        {
            if (GameManager.Instance.STATE == GameManager.kSTATE.PLAY)
            {
                GameManager.Instance.STATE = GameManager.kSTATE.DIS_PLAY;
            }
        }
    }
}
