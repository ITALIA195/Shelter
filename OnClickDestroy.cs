using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnClickDestroy : Photon.MonoBehaviour
{
    public bool DestroyByRpc;

    [RPC]
    public void DestroyRpc()
    {
        Destroy(gameObject);
        PhotonNetwork.UnAllocateViewID(photonView.viewID);
    }

    private void OnClick()
    {
        if (!this.DestroyByRpc)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            photonView.RPC("DestroyRpc", PhotonTargets.AllBuffered, new object[0]);
        }
    }
}

