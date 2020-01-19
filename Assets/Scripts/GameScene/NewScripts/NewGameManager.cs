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
    [SerializeField] float hasBeenKilledTextTime = 3f;
    [SerializeField] float hasKilledTextTime = 3f;
    #endregion

    [SerializeField] Vector3[] initialSpawnPoints;              //4 different spawn
    public Vector3[] InitialSpawnPoints
    {
        get => initialSpawnPoints;
        set => initialSpawnPoints = value;
    }

    private NewPlayerController localPlayerController;
    public NewPlayerController LocalPlayerController
    {
        get => localPlayerController;
        set => localPlayerController = value;
    }

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
    bool hasRespawned = false;

    [SerializeField] float bulletVelocity;
    public float BulletVelocity
    {
        get => bulletVelocity;
        set => bulletVelocity = value;
    }


    [SerializeField] float invincibleTimeWhenRespawned = 3f;

    private void Start()
    {
        gameState = GameState.WAITING_TO_START;
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("InformMasterHasLoaded", RpcTarget.All);
            photonView.RPC("InformMasterHasLoaded", RpcTarget.All);
        }
    }

    private void Update()
    {
        if (gameState == GameState.WAITING_TO_START)
        {
            WaitingToStart();
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
            photonView.RPC("IncreasePlayerHasLoaded", RpcTarget.All);
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

    public void GivePointsToKiller(int killerActorNumber)
    {
        photonView.RPC("GivePointsToKiller", RpcTarget.All, killerActorNumber, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public void Die(int killerActorNumber)
    {
        playerKilledOrKillerName = PhotonNetwork.CurrentRoom.GetPlayer(killerActorNumber).NickName;
        GivePointsToKiller(killerActorNumber);
        StartCoroutine("HasBeenKilled");
        RespawnPlayer();
    }

    void RespawnPlayer()
    {
        float maxDistance = 0;
        Vector3 respawnPoint = new Vector3(0, 0, 0);
        foreach(Vector3 spawnPoint in InitialSpawnPoints)
        {
            if (Vector3.Distance(spawnPoint, localPlayerController.transform.position) > maxDistance)
            {
                maxDistance = Vector3.Distance(spawnPoint, localPlayerController.transform.position);
                respawnPoint = spawnPoint;
            }
        }
        localPlayerController.transform.position = respawnPoint;
        hasRespawned = true;
    }

    public void IncreasePoints()
    {

    }

    IEnumerator HasKilled()
    {
        centralScreenText.text = "Has killed " + playerKilledOrKillerName;
        yield return new WaitForSeconds(hasKilledTextTime);
        centralScreenText.text = "";
    }

    IEnumerator HasBeenKilled()
    {
        centralScreenText.text = "Has been killed by " + playerKilledOrKillerName;
        yield return new WaitForSeconds(hasBeenKilledTextTime);
        centralScreenText.text = "";
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
            StartCoroutine("HasKilled");
        }
    }
    #endregion
}
