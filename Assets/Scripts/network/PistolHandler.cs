using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PistolHandler : MonoBehaviour
{
    public string owner;
    int pistol_index;
    public bool ready = false;
    [SerializeField]PhotonView PV;


    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            PhotonView PV = GameObject.Find("SceneController").GetComponent<PhotonView>();
            PV.RPC("ready", RpcTarget.AllBuffered, pistol_index);
        }
    }

    public void spawn(string player, int index)
    {
        owner = player;
        pistol_index = index;
    }

    public void take()
    {
        PV.RPC("tale_pistol", RpcTarget.AllBuffered, owner);
    }

    [PunRPC]
    public void take_pistol(string player)
    {
        if (owner == player)
        {
            ready = true;
        }
    }
}
