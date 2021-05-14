using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameObject localPlayer;
    [SerializeField] bool[] ready_players;
    [SerializeField] bool[] alive_players;
    [SerializeField] bool waiting = true;
    [SerializeField] bool game_started = false;

    void Start()
    {

    }

    void Update()
    {
        if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
        {
            //if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            //{
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
                    if (counter == PhotonNetwork.CurrentRoom.PlayerCount)
                    {
                        waiting = false;
                        game_started = true;
                        alive_players = ready_players;
                        localPlayer.GetComponent<playerHandler>().StartGame();
                    }

                }
            //}
        }
        if (game_started)
        {
            int counter = 0;
            for (int i = 0; i < alive_players.Length - 1; i++)
            {
                if (alive_players[i] == true)
                {
                    counter += 1;
                }
            }
            if (counter == 0)
            {
                PhotonNetwork.Disconnect();
                SceneManager.LoadScene(0);
                PhotonNetwork.ConnectUsingSettings();
            }
        }
    }

    [PunRPC]
    public void ready(int index)
    {
        ready_players[index] = true;
    }

    [PunRPC]
    public void dead(int index)
    {
        alive_players[index] = false;
    }
}
