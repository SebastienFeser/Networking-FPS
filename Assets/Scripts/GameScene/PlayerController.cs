using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float sprintSpeedMultiplier;
    [SerializeField] Camera camera;
    private float sprint = 1;

    Rigidbody playerRigidbody;

    float velocityYBackup;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
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
        if (Input.GetButtonDown("Shoot"))
        {
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
}
