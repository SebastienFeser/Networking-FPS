using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NewPlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private float movementSpeed;
    [SerializeField] GameObject ball;
    [SerializeField] Camera camera;

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
            photonView.RPC("SpawnBall", RpcTarget.All, camera.transform.position, camera.transform.forward * gameManager.BulletVelocity, PhotonNetwork.LocalPlayer.ActorNumber);
            reloadCurrentTime = 0;
            canShoot = false;
            gameManager.ReloadingPannel.SetActive(true);
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

}
