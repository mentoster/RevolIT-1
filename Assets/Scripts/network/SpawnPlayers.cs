using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Vector3 offset;

    public override void OnJoinedRoom()
    {
        GameObject localPlayer = PhotonNetwork.Instantiate(_player.name, Vector3.zero, Quaternion.identity);
    }
}
