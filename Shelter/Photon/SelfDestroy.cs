using Photon;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class SelfDestroy : Photon.MonoBehaviour
{
    public float CountDown = 5f;

    private void Update()
    {
        this.CountDown -= Time.deltaTime;
        if (this.CountDown <= 0f)
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
            {
                Destroy(gameObject);
            }
            else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
            {
                if (photonView != null)
                {
                    if (photonView.viewID == 0)
                    {
                        Destroy(gameObject);
                    }
                    else if (photonView.isMine)
                    {
                        PhotonNetwork.Destroy(gameObject);
                    }
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}

