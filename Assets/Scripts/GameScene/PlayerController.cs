using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float sprintSpeedMultiplier;
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

        velocityYBackup = playerRigidbody.velocity.y;
        playerRigidbody.velocity = transform.forward * movementSpeed * sprint * getVertical + transform.right * movementSpeed * getHorizontal;
        playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, velocityYBackup, playerRigidbody.velocity.z);
    }
}
