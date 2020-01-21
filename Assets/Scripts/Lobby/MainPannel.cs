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
    [SerializeField] GameObject roomSelectionPannel;
    [SerializeField] GameObject insideRoomPannel;

    [SerializeField] GameObject roomUIPrefab;

    [SerializeField] TextMeshProUGUI[] playerList;


    [SerializeField] TopPannel topPannel;

    Dictionary<string, RoomInfo> roomList = new Dictionary<string, RoomInfo>();
    Dictionary<string, GameObject> roomListPrefabs = new Dictionary<string, GameObject>();

    [SerializeField] GameObject roomListContent;

    string roomName = "RandomRoom";
    byte amountOfPlayers = 4;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

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
        roomSelectionPannel.SetActive(true);
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
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("GameScene");
        }
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
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListPrefabs();
        UpdateRoomInfo(roomList);
        UpdateRoomListPrefabs();
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
        roomSelectionPannel.SetActive(false);
        insideRoomPannel.SetActive(false);
    }

    void RenamePlayers()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playerList[i].text = PhotonNetwork.PlayerList[i].NickName;
        }
    }

    void UpdateRoomInfo(List<RoomInfo> roomListNetworked)
    {
        //Remove room if it got closed, became invisible or was removed
        foreach (RoomInfo info in roomListNetworked)
        {
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (roomListPrefabs.ContainsKey(info.Name))
                {
                    roomListPrefabs.Remove(info.Name);
                }

                continue;
            }

            //Updata prefab list info
            if (roomList.ContainsKey(info.Name))
            {
                roomList[info.Name] = info;
            }
            else
            {
                //Add new room
                roomList.Add(info.Name, info);
            }
        }


    }

    void ClearRoomListPrefabs()
    {
        foreach (GameObject room in roomListPrefabs.Values)
        {
            Destroy(room.gameObject);
        }
        roomListPrefabs.Clear();
    }

    void UpdateRoomListPrefabs()
    {
        foreach (RoomInfo info in roomList.Values)
        {
            GameObject room = Instantiate(roomUIPrefab);
            room.transform.SetParent(roomListContent.transform);
            room.transform.localScale = Vector3.one;
            room.GetComponent<RoomInRoomList>().Initialize(info.Name, (byte)info.PlayerCount, (byte)info.MaxPlayers);

            roomListPrefabs.Add(info.Name, room);
        }
    }

}
