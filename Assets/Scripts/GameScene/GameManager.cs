using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Vector3[] initialSpawnPoints;              //4 different spawn

    bool masterHasLoadedScene = false;

    enum GameState
    {
        WAITING_MASTER_LOADED_SCENE,
        MASTER_HAS_LOADED_SCENE,
        WAITING_FOR_EVERYONE_LOADED_SCENE,
        EVERYONE_LOADED_SCENE,
        START,
    }

    GameState gameState = GameState.WAITING_MASTER_LOADED_SCENE;

    int playerHasLoaded = 0;

    int playerIndex;

    float gameTimer;
    float GameTimer
    {
        get => gameTimer;
        set => gameTimer = value;
    }

    private void Awake()
    {
        
    }

    private void OnDrawGizmos()
    {
        foreach (Vector3 spawnPoint in initialSpawnPoints)
        {
            Gizmos.DrawSphere(spawnPoint, 0.1f);
        }
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.WAITING_MASTER_LOADED_SCENE:
                WaitingMasterConnected();
                break;
            case GameState.WAITING_FOR_EVERYONE_LOADED_SCENE:
                WaitingForEveryoneLoadedScene();
                break;
            case GameState.EVERYONE_LOADED_SCENE:
                EveryOneHasLoadedScene();
                break;
        }
    }

    void WaitingMasterConnected()
    {
        Debug.Log("WaitMasterConnected");
        if (PhotonNetwork.IsMasterClient)
        {

            photonView.RPC("IncreasePlayerHasLoaded", RpcTarget.All);
            photonView.RPC("InformTheMasterHasLoadedScene", RpcTarget.All);
            gameState = GameState.WAITING_FOR_EVERYONE_LOADED_SCENE;

        }
        else if (masterHasLoadedScene)
        {
            photonView.RPC("IncreasePlayerHasLoaded", RpcTarget.All);
            gameState = GameState.WAITING_FOR_EVERYONE_LOADED_SCENE;
        }
    }

    void WaitingForEveryoneLoadedScene()
    {
        Debug.Log("WaitEveryoneConnected");
        if (PhotonNetwork.IsMasterClient)
        {
            if (playerHasLoaded == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                photonView.RPC("EveryoneHasLoadedSceneGameState", RpcTarget.All);
                Debug.Log(gameState);
            }
        }
    }

    void EveryOneHasLoadedScene()
    {
        Debug.Log("Everyone loaded");
        GetPlayerIndex();
        SpawnPlayer();
        gameState = GameState.START;
    }

    void GetPlayerIndex()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
            {
                playerIndex = i;
            }
        }
    }

    void SpawnPlayer()
    {
        Debug.Log("Spawn Player");
        GameObject currentPlayer;
        currentPlayer = PhotonNetwork.Instantiate("Player", initialSpawnPoints[playerIndex], Quaternion.identity);
        currentPlayer.GetComponent<PlayerController>().enabled = true;
        currentPlayer.GetComponentInChildren<Camera>().enabled = true;
        currentPlayer.GetComponentInChildren<AudioListener>().enabled = true;
        currentPlayer.GetComponentInChildren<PlayerCamera>().enabled = true;
    }


    struct PlayerData
    {
        int score;
        int deaths;
    }

    [PunRPC]
    void IncreasePlayerHasLoaded()
    {
        playerHasLoaded += 1;
    }

    [PunRPC]
    void InformTheMasterHasLoadedScene()
    {
        masterHasLoadedScene = true;
    }

    [PunRPC]
    void EveryoneHasLoadedSceneGameState()
    {
        gameState = GameState.EVERYONE_LOADED_SCENE;
    }
}
