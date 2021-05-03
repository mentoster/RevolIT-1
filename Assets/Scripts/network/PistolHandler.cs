using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class PistolHandler : MonoBehaviour
{
    string owner;
    int pistol_index;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            take();
        }
    }

    public void spawn(string player, int index)
    {
        owner = player;
        pistol_index = index;
        Debug.Log(index);
    }

    public void take()
    {
        if (owner == "player")
        {
            PhotonView scene_PV = GameObject.Find("SceneController").GetComponent<PhotonView>();
            scene_PV.RPC("ready", RpcTarget.AllBuffered, pistol_index);
        }
    }
}
