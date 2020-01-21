using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldMineController : MonoBehaviour
{
    [SerializeField] float rayDistance;
    [SerializeField] Vector3 rayDirection;
    [SerializeField] Vector3 rayStartPosition;
    [SerializeField] Collider explosionTrigger;
    
    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(rayStartPosition, rayDirection, out hit, rayDistance))
        {
            if (hit.transform.tag == "Player")
            {
                Debug.Log("Has hit player");
                //if (explosionTrigger.bounds.Contains)
            }
        }
    }
}
