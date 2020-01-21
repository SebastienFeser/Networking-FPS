using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MaterialObservable : MonoBehaviourPunCallbacks, IPunObservable
{
    Color syncColor;
    Vector3 tempColor;

    [SerializeField] MeshRenderer[] playerMeshRenderersToObserve;

    private void Update()
    {
        if(!photonView.IsMine)
        {
            foreach (MeshRenderer renderer in playerMeshRenderersToObserve)
            {
                renderer.material.color = syncColor;
            }
            return;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            tempColor = new Vector3(playerMeshRenderersToObserve[0].material.color.r, playerMeshRenderersToObserve[0].material.color.g, playerMeshRenderersToObserve[0].material.color.b);

            stream.Serialize(ref tempColor);
        }
        else
        {
            stream.Serialize(ref tempColor);

            syncColor = new Color(tempColor.x, tempColor.y, tempColor.z, 1.0f);
        }
    }
}
