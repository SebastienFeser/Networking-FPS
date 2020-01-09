using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerData : MonoBehaviour, Photon.Pun.IPunObservable
{
    [SerializeField] int index;
    public int Index
    {
        get => index;
        set => index = value;
    }

    int score;
    public int Score
    {
        get => score;
        set => score = value;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(index);
            stream.SendNext(score);
        }
        else
        {
            index = (int)stream.ReceiveNext();
            score = (int)stream.ReceiveNext();
        }
    }
}
