using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviour
{
    float time = 0;
    int hitPlayerIndex = -1;
    int killerActorNumber = -2;
    public int KillerActorNumber
    {
        get => killerActorNumber;
        set => killerActorNumber = value;
    }
    [SerializeField] float timeToDestroy;
    [SerializeField] AudioSource fireBurnSource;

    private void Start()
    {
        time = 0;
        fireBurnSource.loop = true;
        fireBurnSource.Play();
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
                if (PhotonNetwork.LocalPlayer.ActorNumber == killerActorNumber)
                {
                }
                else
                {
                    if (other.GetComponent<PlayerController>().IsInvincible)
                    {
                        //Destroy(gameObject);
                    }
                    else
                    {
                        FindObjectOfType<GameManager>().Die(killerActorNumber);
                        //Destroy(gameObject);
                    }
                }
            }
        }
    }
}
