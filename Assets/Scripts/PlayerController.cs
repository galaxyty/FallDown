using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseRPG_V1;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerController : BasePlayerCharacter, IPunObservable
{
    public static int playerCount;

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

    private bool isPlayerCount = false;

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

        if (GameManager.Instance.STATE == GameManager.kSTATE.PLAY)
        {
            // 인원 넣기.
            if (isPlayerCount == false)
            {
                playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

                isPlayerCount = true;
            }
        }

        if (GameManager.Instance.STATE == GameManager.kSTATE.PLAY && playerCount == 1)
        {
            GameManager.Instance.STATE = GameManager.kSTATE.WINNER;
            isPlayerCount = false;
            Debug.Log("게임 승리 작동을 시작합니다");
        }

        if (transform.localPosition.y <= Constants.kGAMEOVER_POSITION)
        {
            // 게임 오버.
            if (GameManager.Instance.STATE == GameManager.kSTATE.PLAY)
            {
                GameManager.Instance.STATE = GameManager.kSTATE.END_PLAY;
                isPlayerCount = false;
                
                --playerCount;                
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 데이터를 네트워크에 전송합니다
            stream.SendNext(playerCount);
            Debug.Log("sssssssss 전송 : " + playerCount + " / " + transform.name);
        }
        else
        {
            // 데이터를 네트워크에서 수신합니다
            playerCount = (int)stream.ReceiveNext();
            Debug.Log("sssssssss 수신 : " + playerCount + " / " + transform.name);
        }
    }
}
