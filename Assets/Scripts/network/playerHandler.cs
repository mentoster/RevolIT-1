using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class playerHandler : MonoBehaviour
{
    Transform spawn;
    int player_index; // todo
    [SerializeField] [Tooltip("Камера игрока")] Camera playerCamera;
    [SerializeField] [Tooltip("скрипт игрока (пока нет нормального)")] FlyCamera playerControl;
    [SerializeField] [Tooltip("AudioListener игрока")] AudioListener AL;
    [SerializeField] [Tooltip("Отключаемые части игрока")] MeshRenderer[] playerRenderers;

    public void MakeLocal(Transform spawnPoint, int index)
    {
        player_index = index;
        spawn = spawnPoint;
        playerCamera.enabled = true;
        playerControl.enabled = true;
        AL.enabled = true;
        for (int i = 0; i < playerRenderers.Length; i++)
        {
            playerRenderers[i].enabled = false;
        }
    }

    public void StartGame()
    {
        gameObject.transform.position = spawn.position;
        gameObject.transform.rotation = spawn.rotation;
    }
}
