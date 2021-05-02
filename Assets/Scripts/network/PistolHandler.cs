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
            if (owner == "player")
            {
                take();
            }
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
        PhotonView scene_PV =  GameObject.Find("SceneController").GetComponent<PhotonView>();
        scene_PV.RPC("ready", RpcTarget.AllBuffered, pistol_index);
    }
}
