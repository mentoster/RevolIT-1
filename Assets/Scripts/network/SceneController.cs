using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SceneController : MonoBehaviour
{
    public GameObject localPlayer;
    public bool[] ready_players;
    bool waiting = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (waiting)
        {
            int counter = 0;
            for (int i = 0; i < ready_players.Length - 1; i++)
            {
                if (ready_players[i] == true)
                {
                    counter += 1;
                }
            }
            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (counter == PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    waiting = false;
                    localPlayer.GetComponent<playerHandler>().StartGame();
                }
            }
        }
    }

    [PunRPC]
    public void ready(int index)
    {
        ready_players[index] = true;
        Debug.Log(index);
    }
}
