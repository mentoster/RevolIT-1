using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Connetion : MonoBehaviourPunCallbacks
{
    [SerializeField] string appVer = "0.1";
    [SerializeField] byte maxPlayers = 4;
    [SerializeField] GameObject player;
    [SerializeField] GameObject pistol;
    [SerializeField] GameObject SceneCamera;
    [SerializeField] Transform[] LobbySpawnPoints;
    [SerializeField] Transform[] PistolSpawnPoints;
    [SerializeField] Transform[] ShootingSpawnPoints;
    [SerializeField] SceneController sceneController;
    Settings settings;
    [SerializeField] string AppId = "";


    void Start()
    {
        PhotonNetwork.GameVersion = appVer;
        settings = new Settings("", 0);
        if (settings.serverAddress != "" && settings.serverPort != 0)
        {
            Debug.Log("Connect to local server");
            PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = false;
            PhotonNetwork.PhotonServerSettings.AppSettings.Server = settings.serverAddress;
            PhotonNetwork.PhotonServerSettings.AppSettings.Port = settings.serverPort;
        }
        else
        {
            Debug.Log("Connect to remote server");
            PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
            PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = AppId;
            PhotonNetwork.PhotonServerSettings.AppSettings.Server = "";
            PhotonNetwork.PhotonServerSettings.AppSettings.Port = 0;
        }
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected");
        RoomOptions opt = new RoomOptions();
        opt.MaxPlayers = maxPlayers;
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("There is no rooms, creating...");
        PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = maxPlayers});
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom.");
        int players = PhotonNetwork.CurrentRoom.PlayerCount;
        GameObject localplayer = PhotonNetwork.Instantiate(player.name, LobbySpawnPoints[players - 1].position, LobbySpawnPoints[players - 1].rotation);
        GameObject localpistol = PhotonNetwork.Instantiate(pistol.name, PistolSpawnPoints[players - 1].position, PistolSpawnPoints[players - 1].rotation);
        sceneController.localPlayer = localplayer;
        localplayer.GetComponent<playerHandler>().MakeLocal(ShootingSpawnPoints[players - 1], players - 1);
        localpistol.GetComponent<PistolHandler>().spawn("player", players - 1);
        SceneCamera.SetActive(false);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom.");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Payer Connected");
        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(cause);
    }
}
