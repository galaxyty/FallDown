using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private readonly byte kMAX_PLAYER = 2;

    [SerializeField]
    private InputField m_InputOfName;

    [SerializeField]
    private GameObject m_Panel;

    [SerializeField]
    private Toggle m_Male;

    [SerializeField]
    private GameObject m_ObjectOfLoading;

    [SerializeField]
    private Text m_TextOfLoading;

    [SerializeField]
    private GameObject m_ObjectOfMessage;

    [SerializeField]
    private Text m_TextOfMessage;

    // 플레이어 생성 위치.
    private readonly Vector3 m_VectorOfCharacter = new Vector3(0.0f, 3.0f, 0.0f);

    // 카메라 생성 위치.
    private readonly Vector3 m_VectoroOfCamera = new Vector3(0.0f, 8.0f, 0.0f);

    void Awake()
    {
        LoadingManager.Instance.Init(m_ObjectOfLoading, m_TextOfLoading);
        MessageManager.Instance.Init(m_ObjectOfMessage, m_TextOfMessage);
        GameManager.Instance.STATE = GameManager.kSTATE.NONE;
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.AutomaticallySyncScene = true;

        // 마스터 서버 연결.
        PhotonNetwork.ConnectUsingSettings();
        LoadingManager.Instance.LoadingOn("서버 접속 중...");
        MessageManager.Instance.MessageOff();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            // 룸 이탈.
            PhotonNetwork.LeaveRoom();
        }
    }

    private void Spawn()
    {
        // 맵 생성. (생성은 마스터 클라이언트에게서 생성, 그 이후 접속한 클라이언트는 동기화로 자동 생성).
        PhotonNetwork.InstantiateRoomObject("Prefabs/Map", Vector3.zero, Quaternion.identity);

        // 캐릭터 생성.
        GameObject character = null;

        if (m_Male.isOn == true)
        {
            // 남성 캐릭터.
            character = PhotonNetwork.Instantiate("Prefabs/PlayerMale", m_VectorOfCharacter, Quaternion.identity);
        }
        else
        {
            // 여성 캐릭터.
            character = PhotonNetwork.Instantiate("Prefabs/PlayerFemale", m_VectorOfCharacter, Quaternion.identity);
        }

        // 카메라 생성 후 셋팅.
        CameraController camera = PoolManager.Instance.GetObject<CameraController>();
        camera.ResetLocalPosition();
        camera.transform.localPosition = m_VectoroOfCamera;

        // 카메라 이동 대상 셋팅.
        camera.SettingTarget(character);

        // BGM 재생.
        BGMManager.Instance.Play("Room");
    }

    // 매칭 버튼.
    public void OnClickMatching()
    {
        if (m_InputOfName.text == null || m_InputOfName.text == "")
        {
            return;
        }

        LoadingManager.Instance.LoadingOn("매칭 중...");

        // 닉네임.
        PhotonNetwork.LocalPlayer.NickName = m_InputOfName.text;

        // 룸 옵션.
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = kMAX_PLAYER;       // 룸 인원 수.
        // TODO :: 룸 입장 조건은 없다. 따라서 해당 부분은 필요 없으니 주석 처리.
        // 룸 조건.
        // roomOptions.CustomRoomProperties = new Hashtable()
        // {
        //     {
        //         "Player",
        //         "All"
        //     }
        // };
        // roomOptions.CustomRoomPropertiesForLobby = new string[] { "Player" };  // 로비에게 이 룸은 해당 속성(키)을 가진 유저만 접속 가능하게 전달.

        // 랜덤 참가.
        PhotonNetwork.JoinRandomOrCreateRoom(
            // TODO :: 위에 룸 입장 조건이 쓰이지 않으니 필요없어서 주석 처리, expected... 아마 참가할 때와 관련된 부분인 거 같다. 잘은 모르겠다.
            // expectedCustomRoomProperties: roomOptions.CustomRoomProperties,        // 참가할 때의 기준.
            // expectedMaxPlayers: kMAX_PLAYER,             // 인원 수.
            roomOptions: roomOptions                        // 생성할 때의 기준.
        );
    }

    #region Photon Fucntion
    
    // 다른 유저가 룸에 참가할 시 호출.
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} 플레이어가 룸에 참가했습니다");

        MessageManager.Instance.MessageOn($"플레이어 대기 중 ({PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers})");

        // 최대 인원 수 도달 시 게임 시작.
        if (PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            GameManager.Instance.STATE = GameManager.kSTATE.INIT_PLAY;

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            Debug.Log("게임 시작 준비...");
            return;
        }
    }

    // 다른 유저가 룸에 이탈할 시 호출.
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} 플레이어가 룸에서 이탈했습니다");

        MessageManager.Instance.MessageOn($"플레이어 대기 중 ({PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers})");
    }

    // 방에 참가 했을 시 호출.
    public override void OnJoinedRoom()
    {
        Debug.Log("룸 참가 성공");

        GameManager.Instance.STATE = GameManager.kSTATE.IDLE;

        LoadingManager.Instance.LoadingOff();

        Spawn();

        if (m_Panel == null)
        {
            return;
        }
        
        m_Panel.SetActive(false);
        MessageManager.Instance.MessageOn($"플레이어 대기 중 ({PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers})");

        // 최대 인원 수 도달 시 게임 시작.
        if (PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            GameManager.Instance.STATE = GameManager.kSTATE.INIT_PLAY;

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            Debug.Log("게임 시작 준비...");
            return;
        }
    }    

    // 로비 접속 시 호출.
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속 성공");
    }

    // 룸 생성 실패 시.
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("룸 생성 실패 returnCode : " + returnCode + " / message : " + message);

        LoadingManager.Instance.LoadingOff();
    }

    // 룸 랜덤으로 참가 실패 시.
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("룸 랜덤 참가 실패 returnCode : " + returnCode + " / message : " + message);

        LoadingManager.Instance.LoadingOff();
    }    

    // 룸에 참가 실패 시.
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("룸 참가 실패 returnCode : " + returnCode + " / message : " + message);

        LoadingManager.Instance.LoadingOff();
    }

    // 마스터 서버에 연결 성공 시 호출.
    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버 접속 성공");

        LoadingManager.Instance.LoadingOff();
    }
    
    // 룸 나갈 시 호출.
    public override void OnLeftRoom()
    {
        Debug.Log("룸 이탈");

        BGMManager.Instance.Stop();

        GameManager.Instance.STATE = GameManager.kSTATE.NONE;

        if (m_Panel == null)
        {
            return;
        }

        m_Panel.SetActive(true);
        MessageManager.Instance.MessageOff();
    }

    // 로비 나갈 시 호출.
    public override void OnLeftLobby()
    {
        Debug.Log("로비 이탈");
    }

    // 포톤 서버 연결 끊길 때 호출.
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("포톤 서버 연결 끊김");

        GameManager.Instance.STATE = GameManager.kSTATE.NONE;
    }

    #endregion Photon Fucntion
}
