using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseRPG_V1;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : BaseSingleton<GameManager>
{
    public enum kSTATE
    {
        NONE,           // 기본 상태.
        IDLE,           // 대기 중.
        INIT_PLAY,      // 게임 셋팅.
        PLAY,           // 게임 진행 중.
        END_PLAY,       // 게임 진행 끝.
        GAMEOVER,       // 게임 오버.
        WINNER,         // 게임 승리.
    }

    public kSTATE STATE = kSTATE.NONE;

    void Update()
    {
        switch (STATE)
        {
            case kSTATE.INIT_PLAY:
                StartCoroutine(EWait());
                STATE = kSTATE.NONE;

                break;
            case kSTATE.PLAY:
                break;
            
            case kSTATE.END_PLAY:
                StartCoroutine(ERoomLeave());
                STATE = kSTATE.GAMEOVER;
                break;
            
            case kSTATE.GAMEOVER:
                break;
            
            case kSTATE.WINNER:
                StartCoroutine(EWinRoomLeave());
                Debug.Log("게임 승리 작동 완료");
                break;
            
            default:
                break;
        }
    }

    IEnumerator EWait()
    {
        Debug.Log("3");
        MessageManager.Instance.MessageOn("게임 시작 3초 전...");
        yield return new WaitForSeconds(1.0f);

        Debug.Log("2");
        MessageManager.Instance.MessageOn("게임 시작 2초 전...");
        yield return new WaitForSeconds(1.0f);

        Debug.Log("1");
        MessageManager.Instance.MessageOn("게임 시작 1초 전...");
        yield return new WaitForSeconds(1.0f);

        Debug.Log("GO!");
        MessageManager.Instance.MessageOn("게임 시작!");
        yield return new WaitForSeconds(1.0f);

        MessageManager.Instance.MessageOff();
        STATE = kSTATE.PLAY;
    }

    // 게임 오버. 일정 시간 후 룸 이탈 됨.
    IEnumerator ERoomLeave()
    {
        LoadingManager.Instance.LoadingOn("당신은 탈락하였습니다. 3초후 룸으로 이동합니다.");
        yield return new WaitForSeconds(3.0f);
        PhotonNetwork.LeaveRoom();
    }

    // 게임 승리.
    IEnumerator EWinRoomLeave()
    {
        LoadingManager.Instance.LoadingOn("최종 승리하셨습니다!!! 3초후 룸으로 이동합니다.");
        yield return new WaitForSeconds(3.0f);
        PhotonNetwork.LeaveRoom();
    }
}
