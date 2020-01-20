using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NewPlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private float movementSpeed;
    [SerializeField] GameObject ball;
    [SerializeField] Camera playerCamera;
    public Camera PlayerCamera
    {
        get => playerCamera;
        set => playerCamera = value;
    }

    NewGameManager gameManager;

    Rigidbody playerRigidbody;
    bool canShoot = true;
    [SerializeField] float reloadTime;
    float reloadCurrentTime;

    bool isInvincible = false;
    public bool IsInvincible
    {
        get => isInvincible;
        set => isInvincible = value;
    }

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<NewGameManager>();
        gameManager.LocalPlayerController = this;
    }

    private void Update()
    {
        float getHorizontal = Input.GetAxisRaw("Horizontal");
        float getVertical = Input.GetAxisRaw("Vertical");
        Shooting();

        float velocityYBackup = playerRigidbody.velocity.y;
        playerRigidbody.velocity = transform.forward * movementSpeed * getVertical + transform.right * movementSpeed * getHorizontal;
        playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, velocityYBackup, playerRigidbody.velocity.z);
    }

    void Shooting()
    {
        if (canShoot)
        {
            if (Input.GetButtonDown("Shoot"))
            {
                photonView.RPC("SpawnBall", RpcTarget.All, playerCamera.transform.position, playerCamera.transform.forward * gameManager.BulletVelocity, PhotonNetwork.LocalPlayer.ActorNumber);
                reloadCurrentTime = 0;
                canShoot = false;
                gameManager.ReloadingPannel.SetActive(true);
            }
        }
        else
        {
            reloadCurrentTime += Time.deltaTime;
            if (reloadCurrentTime >= reloadTime)
            {
                reloadCurrentTime = reloadTime;
                canShoot = true;
            }
            gameManager.ReloadBar.fillAmount = reloadCurrentTime / reloadTime;
        }
    }

    [PunRPC]
    void SpawnBall(Vector3 position, Vector3 velocity, int killerActorNumber)
    {
        GameObject ballInstantiated = Instantiate(ball, position, Quaternion.identity);
        ballInstantiated.GetComponent<Rigidbody>().velocity = velocity;
        ballInstantiated.GetComponent<NewBullet>().KillerActorNumber = killerActorNumber;
    }

}
