using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallShoot : MonoBehaviourPunCallbacks
{
    float time = 0;
    int hitPlayerIndex = -1;
    public int shooterActorNumber = -2;
    [SerializeField] float timeToDestroy;

    private void Start()
    {
        time = 0;
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time >= timeToDestroy)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall")
        {
            Destroy(gameObject);
        }
        else if (other.tag == "PlayerParent")
        {
            if (other.GetComponent<PhotonView>().IsMine)
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber == shooterActorNumber)
                {
                }
                else
                {
                    if (other.GetComponent<PlayerController>().isInvincible)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        Debug.Log("BallShoot Actor Number = " + shooterActorNumber);
                        other.GetComponent<PlayerController>().Die(shooterActorNumber);
                        Destroy(gameObject);
                    }
                }
            }
            
        }
        /*if (photonView.IsMine)
        {
            if (other.tag == "Wall")
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else if (other.tag == "Player")
            {
                if (other.GetComponentInParent<PlayerData>().Index == currentPlayerIndex)
                {
                    Debug.Log("IsCurrentPlayer");
                }
                else
                {
                    hitPlayerIndex = other.GetComponentInParent<PlayerData>().Index;
                    photonView.RPC("KillTargetPlayer", RpcTarget.All, hitPlayerIndex);
                    photonView.RPC("IncreaseTargetPlayerPoints", RpcTarget.All, currentPlayerIndex);
                }
            }
        }*/
    }

    /*[PunRPC]
    void KillTargetPlayer(int index)
    {
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("PlayerParent"))
        {
            if (player.GetComponentInChildren<Camera>().enabled)
            {
                if (hitPlayerIndex == player.GetComponentInParent<PlayerData>().Index)
                {
                    Debug.Log("Killed: " + index);
                    PhotonNetwork.Destroy(player);
                }
                break;
            }
        }
    }

    [PunRPC]
    void IncreaseTargetPlayerPoints(int index)
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("PlayerParent"))
        {
            if (player.GetComponentInChildren<Camera>().enabled)
            {
                if (hitPlayerIndex == player.GetComponentInParent<PlayerData>().Index)
                {
                    Debug.Log("WinPoints: " + index);
                }
                break;
            }
        }
    }*/
}
