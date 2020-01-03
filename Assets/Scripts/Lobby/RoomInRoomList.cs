using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomInRoomList : MonoBehaviour
{
    [SerializeField] Text roomNameText;
    public Text RoomNameText
    {
        get => roomNameText;
        set => roomNameText = value;
    }

    [SerializeField] Text roomPlayersText;
    public Text RoomPlayersText
    {
        get => roomPlayersText;
        set => roomPlayersText = value;
    }

    [SerializeField] Button joinRoomButton;
    public Button JoinRoomButton
    {
        get => joinRoomButton;
        set => joinRoomButton = value;
    }

    string roomName;

    public void InitializeRoom(string name, byte currentAmountOfPlayers, byte maxPlayers)
    {
        roomName = name;

        roomNameText.text = name;
        roomPlayersText.text = currentAmountOfPlayers + "/" + maxPlayers;
    }
}
