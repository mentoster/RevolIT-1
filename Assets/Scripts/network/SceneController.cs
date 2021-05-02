using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SceneController : MonoBehaviour
{
    public bool[] ready_players;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int counter = 0;
        for (int i = 0; i < ready_players.Length - 1; i++)
        {
            if (ready_players[i] == true)
            {
                counter += 1;
            }
        }
        if (counter == ready_players.Length - 1)
        {
            Debug.Log("spawn");
        }
    }

    [PunRPC]
    public void ready(int index)
    {
        ready_players[index] = true;
    }
}
