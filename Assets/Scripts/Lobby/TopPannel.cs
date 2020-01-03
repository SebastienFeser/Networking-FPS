using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class TopPannel : MonoBehaviour
{
    private string connectionStatusMessage = "Connection Status: ";

    [SerializeField] TextMeshProUGUI statusText;
    /*public TextMeshProUGUI StatusText
    {
        get => statusText;
        set => statusText = value;
    }*/

    private void Update()
    {
        statusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
    }
}
