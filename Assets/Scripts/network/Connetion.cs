using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Connetion : MonoBehaviourPunCallbacks
{
    [SerializeField] private string appVer = "0.1";
    [SerializeField] private byte maxPlayers = 4;
    private bool inSerchOfMatch = false;


    public static bool gameStart { get; private set; }

    private void Awake()
    {
        gameStart = false;
    }

    private void Start()
    {
        PhotonNetwork.GameVersion = appVer;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void EndOfMatch(bool isWin)
    {
        PhotonNetwork.LeaveRoom();
    }

    private void CheckGameStatus()
    {
        if (maxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            gameStart = true;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    #region Btns actions
    public void SearchMatch()
    {
        if (inSerchOfMatch)
        {
            PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = maxPlayers });
        }
        else
        {
            inSerchOfMatch = true;
            PhotonNetwork.JoinRandomRoom();
        }
    }
    #endregion

    #region Fails
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning("No rooms avalible. A new room will be created.");
        SearchMatch();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to create room");
        inSerchOfMatch = false;
    }
    #endregion

    #region Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom.");
        CheckGameStatus();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom.");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        CheckGameStatus();
    }

    public override void OnLeftRoom()
    {
        
    }
    #endregion
}
