using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public GameObject localPlayer;                              // локальный игрок
    [SerializeField] Light Candle;                              // затухающая свеча
    [SerializeField] GameObject[] Lamps;                        // лампы мешающие начальной сцене
    [SerializeField] GameObject Panel;                          // затухающая панель
    [SerializeField] bool[] ready_players;                      // готовые игроки
    [SerializeField] bool[] alive_players;                      // живые игроки
    [SerializeField] bool waiting = true;                       // ожидание начала игры
    [SerializeField] bool game_started = false;                 // игра в процессе

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
                    StartCoroutine("StartGame");
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

    IEnumerator StartGame()
    {
        while (Candle.range > 0.4f)
        {
            Candle.range -= Time.deltaTime * 1.5f;
            yield return new WaitForSeconds(0.05f);
        }
        Lamp_Turner(true);
        Panel.SetActive(true);
        localPlayer.GetComponent<playerHandler>().StartGame();
    }

    public void Lamp_Turner(bool state)
    {
        for (int i = 0; i < Lamps.Length; i++)
        {
            Lamps[i].SetActive(state);
        }
    }
}
