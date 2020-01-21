using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

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

    [SerializeField] Material[] playerColors; 

    [SerializeField] int killScore = 100;

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

    List<int> playerScore = new List<int>();

    [SerializeField] AudioSource deathSource;
    [SerializeField] AudioSource killSource;

    enum GameState
    {
        WAITING_TO_START,
        GAME,
        END_GAME

    }
    GameState gameState;
    bool hasLoaded = false;
    bool hasStartedCountDownCoroutine = false;
    bool hasRespawned = false;

    [SerializeField] float bulletVelocity;
    public float BulletVelocity
    {
        get => bulletVelocity;
        set => bulletVelocity = value;
    }


    [SerializeField] float invincibleTimeWhenRespawned = 3f;
    [SerializeField] int victoryScore = 1000;

    int playersInRoomLastFrame;

    string winnerName = "Error";

    private void Start()
    {
        gameState = GameState.WAITING_TO_START;
        playersInRoomLastFrame = PhotonNetwork.PlayerList.Length;
    }

    private void Update()
    {
        if (gameState == GameState.WAITING_TO_START)
        {
            WaitingToStart();
        }
        else if (gameState == GameState.GAME)
        {
            scoreText.text = "Your Score = " + playerScore[PhotonNetwork.LocalPlayer.ActorNumber - 1];
            if (playerScore[PhotonNetwork.LocalPlayer.ActorNumber - 1] >= victoryScore)
            {
                photonView.RPC("EndGame", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
            }
        }
        else if (gameState == GameState.END_GAME)
        {
            LocalPlayerController.IsInGameState = false;
        }

        if (playersInRoomLastFrame != PhotonNetwork.PlayerList.Length)
        {
            if (PhotonNetwork.PlayerList.Length == 1)
            {
                photonView.RPC("EndGame", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
            }
        }
        playersInRoomLastFrame = PhotonNetwork.PlayerList.Length;

    }

    private void OnDrawGizmos()
    {
        foreach(Vector3 spawnPoint in initialSpawnPoints)
        {
            Gizmos.DrawSphere(spawnPoint, 1);
        }
    }

    #region Methods
    void WaitingToStart()
    {
        if (!hasLoaded)
        {
            SpawnPlayer();
            hasLoaded = true;
        }

        if (FindObjectsOfType<NewPlayerController>().Length == PhotonNetwork.PlayerList.Length && !hasStartedCountDownCoroutine)
        {
            hasStartedCountDownCoroutine = true;
            gameState = GameState.GAME;
            StartCoroutine("GameStartCoroutine");
            StartScore();
        }
    }

    void SpawnPlayer()
    {
        GameObject currentPlayer;
        currentPlayer = PhotonNetwork.Instantiate("Player", initialSpawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1], Quaternion.identity);
        
        NewPlayerController currentPlayerController = currentPlayer.GetComponent<NewPlayerController>();
        currentPlayerController.enabled = true;
        for (int i = 0; i < currentPlayerController.PlayerMeshRenderers.Length; i++)
        {
            currentPlayerController.PlayerMeshRenderers[i].materials = new Material[] { playerColors[PhotonNetwork.LocalPlayer.ActorNumber - 1] };
        }
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

    void StartScore()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playerScore.Add(0);
        }
    }

    IEnumerator HasKilled()
    {
        killSource.Play();
        centralScreenText.text = "Has killed " + playerKilledOrKillerName;
        yield return new WaitForSeconds(hasKilledTextTime);
        centralScreenText.text = "";
    }

    IEnumerator HasBeenKilled()
    {
        deathSource.Play();
        centralScreenText.text = "Has been killed by " + playerKilledOrKillerName;
        yield return new WaitForSeconds(hasBeenKilledTextTime);
        centralScreenText.text = "";
    }

    IEnumerator GameStartCoroutine()
    {
        centralScreenText.text = "Ready?";
        yield return new WaitForSeconds(3f);
        centralScreenText.text = "3";
        yield return new WaitForSeconds(1f);
        centralScreenText.text = "2";
        yield return new WaitForSeconds(1f);
        centralScreenText.text = "1";
        yield return new WaitForSeconds(1f);
        centralScreenText.text = "GOOO";
        localPlayerController.IsInGameState = true;
        yield return new WaitForSeconds(2f);
        centralScreenText.text = "";
    }

    IEnumerator Victory()
    {
        centralScreenText.text = "Stoooop!";
        yield return new WaitForSeconds(3f);
        centralScreenText.text = "And the winner is...";
        yield return new WaitForSeconds(3f);
        centralScreenText.text = winnerName;
        yield return new WaitForSeconds(5f);
        centralScreenText.text = "Thanks for playing my game!";
        yield return new WaitForSeconds(5f);
        PhotonNetwork.Disconnect();

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("GameLauncher");
    }
    #endregion

    #region RPCS

    [PunRPC]
    void GivePointsToKiller(int killerActorNumber, int killedActorNumber)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == killerActorNumber)
        {
            playerKilledOrKillerName = PhotonNetwork.CurrentRoom.GetPlayer(killedActorNumber).NickName;
            StartCoroutine("HasKilled");
        }
        playerScore[killerActorNumber - 1] += killScore;
    }

    [PunRPC]
    void EndGame(string winnerNameInRPC)
    {
        winnerName = winnerNameInRPC;
        gameState = GameState.END_GAME;
        StartCoroutine("Victory");
    }
    #endregion
}
