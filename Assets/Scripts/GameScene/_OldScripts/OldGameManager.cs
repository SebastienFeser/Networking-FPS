﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class OldGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Vector3[] initialSpawnPoints;              //4 different spawn
    public OldPlayerController myPlayerController;
    public Vector3[] InitialSpawnPoints
    {
        get => initialSpawnPoints;
        set => initialSpawnPoints = value;
    }

    [SerializeField] Image reloadBar;
    public Image ReloadBar
    {
        get => reloadBar;
        set => reloadBar = value;
    }

    [SerializeField] GameObject reloadingPannel;
    public  GameObject ReloadingPannel
    {
        get => reloadingPannel;
        set => reloadingPannel = value;
    }

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
        currentPlayer.GetComponent<OldPlayerController>().enabled = true;
        currentPlayer.GetComponentInChildren<Camera>().enabled = true;
        currentPlayer.GetComponentInChildren<AudioListener>().enabled = true;
        currentPlayer.GetComponentInChildren<OldPlayerCamera>().enabled = true;
        currentPlayer.GetComponent<OldPlayerData>().Index = playerIndex;
    }

    public void GivePointsToKiller(int killerActorNumber)
    {
        photonView.RPC("GivePointsToKiller", RpcTarget.All, killerActorNumber, PhotonNetwork.LocalPlayer.ActorNumber);
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

    [PunRPC]
    void GivePointsToKiller(int killerActorNumber, int killedActorNumber)
    {
        Debug.Log("Give Points To Killer Actor Number = " + killerActorNumber);
        Debug.Log("OwnerActorNr = " + PhotonNetwork.LocalPlayer.ActorNumber + " and killerActorNumber = " + killerActorNumber);
        if (PhotonNetwork.LocalPlayer.ActorNumber == killerActorNumber)
        {

            myPlayerController.PlayerKilledOrKillerName = PhotonNetwork.CurrentRoom.GetPlayer(killedActorNumber).NickName;
            myPlayerController.PlayerHasKilled();
        }
    }
}
