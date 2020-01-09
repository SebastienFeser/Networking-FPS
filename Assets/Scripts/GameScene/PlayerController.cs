using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float sprintSpeedMultiplier;
    [SerializeField] Camera camera;
    GameManager gameManager;


    private float sprint = 1;

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
        float getHorizontal = Input.GetAxisRaw("Horizontal");
        float getVertical = Input.GetAxisRaw("Vertical");

        /*if (Input.GetButton("Sprint"))
        {
            sprint = sprintSpeedMultiplier;
        }
        else
        {
            sprint = 1;
        }*/

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
}
