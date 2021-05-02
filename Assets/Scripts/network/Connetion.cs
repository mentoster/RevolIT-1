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

    void Start()
    {
        PhotonNetwork.GameVersion = appVer;
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
        localpistol.GetComponent<PistolHandler>().spawn("player", players - 1);
        localplayer.GetComponent<MeshRenderer>().enabled = false;
        localplayer.GetComponent<AudioListener>().enabled = false;
        localplayer.GetComponent<Camera>().enabled = true;
        localplayer.GetComponent<FlyCamera>().enabled = true;
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
