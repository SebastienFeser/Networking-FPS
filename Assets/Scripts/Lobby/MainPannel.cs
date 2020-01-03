using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class MainPannel : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject loginPannel;
    [SerializeField] TMP_InputField loginNameInputField;


    [SerializeField] GameObject selectionPannel;
    [SerializeField] GameObject createRoomPannel;
    [SerializeField] GameObject joinRandomRoomPannel;
    [SerializeField] GameObject lobbySelectionPannel;
    [SerializeField] GameObject insideRoomPannel;

    [SerializeField] TextMeshProUGUI[] playerList;


    [SerializeField] TopPannel topPannel;

    Dictionary<string, RoomInfo> roomList;
    Dictionary<string, GameObject> roomListPrefabs;

    string roomName = "RandomRoom";
    byte amountOfPlayers = 4;


    #region UI
    public void LoginButton()
    {
        
        string loginPlayerName = loginNameInputField.text;
        if (loginPlayerName == "")
        {
            loginPlayerName = "No_One";
        }
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NickName = loginPlayerName;
        DesactivateAllPannels();
    }

    public void JoinRandomRoomSelectionButton()
    {
        PhotonNetwork.JoinRandomRoom();
        DesactivateAllPannels();
    }

    public void CreateRoomSelectionButton()
    {
        DesactivateAllPannels();
        createRoomPannel.SetActive(true);
    }

    public void RoomListSelectionButton()
    {
        DesactivateAllPannels();
    }

    public void CreateRoomButton()
    {
        DesactivateAllPannels();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = amountOfPlayers;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void StartGame()
    {
        DesactivateAllPannels();
        PhotonNetwork.LoadLevel("GameScene");
    }

    public void QuitGame()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region Pun Callbacks

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        selectionPannel.SetActive(true);
        Debug.Log("test");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RenamePlayers();
    }

    public override void OnLeftLobby()
    {
        
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create Room Failed");
        selectionPannel.SetActive(true);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created");
        insideRoomPannel.SetActive(true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoomButton();
    }

    public override void OnJoinedRoom()
    {
        insideRoomPannel.SetActive(true);
        RenamePlayers();
    }

    public override void OnLeftRoom()
    {

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RenamePlayers();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        
    }

    #endregion

    void DesactivateAllPannels()
    {
        loginPannel.SetActive(false);
        selectionPannel.SetActive(false);
        createRoomPannel.SetActive(false);
        joinRandomRoomPannel.SetActive(false);
        lobbySelectionPannel.SetActive(false);
        insideRoomPannel.SetActive(false);
    }

    void RenamePlayers()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playerList[i].text = PhotonNetwork.PlayerList[i].NickName;
        }
    }

}
