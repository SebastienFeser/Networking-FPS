using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float sprintSpeedMultiplier;
    [SerializeField] Camera camera;
    [SerializeField] float BulletVelocity;
    [SerializeField] PlayerData playerData;
    [SerializeField] GameObject ball;
    [SerializeField] float invincibleTimeWhenRespawned = 3f;
    public bool isInvincible = false;
    bool hasRespawned = true;
    GameManager gameManager;


    private float sprint = 1;

    private float respawnInvincibleTimeCurrent = 0;

    Rigidbody playerRigidbody;

    float velocityYBackup;
    bool canShoot = true;

    [SerializeField] float reloadTime;
    float reloadCurrentTime;


    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();

    }


    private void Update()
    {
        if (!hasRespawned)
        {
            respawnInvincibleTimeCurrent = 0;
            hasRespawned = true;
            isInvincible = true;
        }

        if (isInvincible)
        {
            respawnInvincibleTimeCurrent += Time.deltaTime;
            if (respawnInvincibleTimeCurrent > invincibleTimeWhenRespawned)
            {
                isInvincible = false;
            }
        }

        float getHorizontal = Input.GetAxisRaw("Horizontal");
        float getVertical = Input.GetAxisRaw("Vertical");

        Shooting();

        velocityYBackup = playerRigidbody.velocity.y;
        playerRigidbody.velocity = transform.forward * movementSpeed * sprint * getVertical + transform.right * movementSpeed * getHorizontal;
        playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, velocityYBackup, playerRigidbody.velocity.z);
    }

    void Shooting()
    {
        if (canShoot)
        {
            if (Input.GetButtonDown("Shoot"))
            {
                photonView.RPC("SpawnBall", RpcTarget.All, camera.transform.position, camera.transform.forward * BulletVelocity, photonView.OwnerActorNr);
                reloadCurrentTime = 0;
                canShoot = false;
                gameManager.ReloadingPannel.SetActive(true);
                RaycastHit hit;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "Player")
                    {
                        Debug.Log("Shoot");
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
        else
        {
            reloadCurrentTime += Time.deltaTime;
            if (reloadCurrentTime >= reloadTime)
            {
                reloadCurrentTime = reloadTime;
                canShoot = true;
                gameManager.ReloadingPannel.SetActive(false);
            }
            gameManager.ReloadBar.fillAmount = reloadCurrentTime / reloadTime;

        }
    }

    /*void SpawnBall()
    {
        GameObject ballInstantiated = PhotonNetwork.Instantiate("Ball", camera.transform.position, Quaternion.identity);
        ballInstantiated.GetComponent<Rigidbody>().velocity = camera.transform.forward * BulletVelocity;
        ballInstantiated.GetComponent<BallShoot>().currentPlayerIndex = playerData.Index;
    }*/

    public void Die(int killerActorNumber)
    {
        Debug.Log("You were killed By " + PhotonNetwork.CurrentRoom.GetPlayer(killerActorNumber).NickName);
        photonView.RPC("GivePointsToKiller", RpcTarget.All, killerActorNumber);
        float maxDistance = 0;
        Vector3 respawnPoint = new Vector3(0,0,0);
        foreach(Vector3 spawnPoint in gameManager.InitialSpawnPoints)
        {
            if (Vector3.Distance(spawnPoint, transform.position) > maxDistance)
            {
                maxDistance = Vector3.Distance(spawnPoint, transform.position);
                respawnPoint = spawnPoint;
            }
            gameObject.transform.position = respawnPoint;
            hasRespawned = false;

        }
    }


    [PunRPC]
    void SpawnBall(Vector3 position, Vector3 velocity, int ownerActorNumber)
    {
        GameObject ballInstantiated = Instantiate(ball, position, Quaternion.identity);
        ballInstantiated.GetComponent<Rigidbody>().velocity = velocity;
        ballInstantiated.GetComponent<BallShoot>().shooterActorNumber = ownerActorNumber;
    }

    [PunRPC]
    void GivePointsToKiller(int killerActorNumber)
    {
        if (photonView.OwnerActorNr == killerActorNumber)
        {
            Debug.Log("You get your points");
        }
    }
    /*void Mines()
    {
        //CastRay with distance for C4, Middle screen, from camera
        //If raycast touches wall && Mines > 0
            //Spawn Mine Ghost
            //If leftclick
                //SpawnMine
                //MineColor = PlayerColor
    }*/
}
