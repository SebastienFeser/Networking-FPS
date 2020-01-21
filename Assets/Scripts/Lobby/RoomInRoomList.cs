using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomInRoomList : MonoBehaviour
{
    public void Initialize(string name, byte playerCount, byte maxPlayers)
    {
        roomNameText.text = name;
        roomPlayersText.text = playerCount + "/" + maxPlayers;
    }

    [SerializeField] TextMeshProUGUI roomNameText;
    public TextMeshProUGUI RoomNameText
    {
        get => roomNameText;
        set => roomNameText = value;
    }

    [SerializeField] TextMeshProUGUI roomPlayersText;
    public TextMeshProUGUI RoomPlayersText
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
