using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class NewGameManager : MonoBehaviourPunCallbacks
{
    #region UI
    [SerializeField] TextMeshProUGUI centralScreenText;
    public TextMeshProUGUI CentralScreenText
    {
        get => centralScreenText;
        set => centralScreenText = value;
    }

    [SerializeField] TextMeshProUGUI scoreText;
    public TextMeshProUGUI ScoreText
    {
        get => scoreText;
        set => scoreText = value;
    }

    [SerializeField] TextMeshProUGUI timeText;
    public TextMeshProUGUI TimeText
    {
        get => timeText;
        set => timeText = value;
    }

    [SerializeField] Image reloadBar;
    public Image ReloadBar
    {
        get => reloadBar;
        set => reloadBar = value;
    }

    [SerializeField] GameObject reloadingPannel;
    public GameObject ReloadingPannel
    {
        get => reloadingPannel;
        set => reloadingPannel = value;
    }

    string playerKilledOrKillerName = "Error";
    #endregion

    [SerializeField] Vector3[] initialSpawnPoints;              //4 different spawn
    public Vector3[] InitialSpawnPoints
    {
        get => initialSpawnPoints;
        set => initialSpawnPoints = value;
    }

    public NewPlayerController myPlayerController;

    enum GameState
    {
        WAITING_TO_START,
        GAME,
        END_GAME

    }
    GameState gameState;
    bool masterClientLoaded = false;
    bool hasIncreasedLoadedList = false;
    int playerHasLoaded = 0;

    bool hasStartedCountDownCoroutine = false;

    private void Start()
    {
        gameState = GameState.WAITING_TO_START;
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("MasterClientLoaded", RpcTarget.All);
        }
    }

    private void Update()
    {
        if (gameState == GameState.WAITING_TO_START)
        {

        }
        else if (gameState == GameState.GAME)
        {

        }
        else if (gameState == GameState.END_GAME)
        {

        }
    }

    #region Methods
    void WaitingToStart()
    {
        if (masterClientLoaded && !hasIncreasedLoadedList)
        {
            hasIncreasedLoadedList = true;
        }
        if (PhotonNetwork.PlayerList.Length == playerHasLoaded && !hasStartedCountDownCoroutine)
        {
            hasStartedCountDownCoroutine = true;
        }
    }

    void SpawnPlayer()
    {
        GameObject currentPlayer;
        currentPlayer = PhotonNetwork.Instantiate("Player", initialSpawnPoints[PhotonNetwork.LocalPlayer.ActorNumber], Quaternion.identity);
        currentPlayer.GetComponent<NewPlayerController>().enabled = true;
        currentPlayer.GetComponentInChildren<Camera>().enabled = true;
        currentPlayer.GetComponentInChildren<AudioListener>().enabled = true;
        currentPlayer.GetComponentInChildren<NewPlayerCamera>().enabled = true;
    }

    void GivePointsToKiller(int killerActorNumber)
    {
        photonView.RPC("GivePointsToKiller", RpcTarget.All, killerActorNumber, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    void Die()
    {

    }

    void IncreasePoints()
    {

    }
    #endregion

    #region RPCS
    [PunRPC]
    void IncreasePlayerHasLoaded()
    {
        playerHasLoaded += 1;
    }

    [PunRPC]
    void InformMasterHasLoaded()
    {
        masterClientLoaded = true;
    }

    [PunRPC]
    void GivePointsToKiller(int killerActorNumber, int killedActorNumber)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == killerActorNumber)
        {
            playerKilledOrKillerName = PhotonNetwork.CurrentRoom.GetPlayer(killedActorNumber).NickName;
        }
    }
    #endregion
}
