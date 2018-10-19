using UnityEngine;
using Extensions = Photon.Extensions;

public class TitanTrigger : MonoBehaviour
{
    public bool isCollide;

    private void OnTriggerEnter(Collider other)
    {
        if (!this.isCollide)
        {
            GameObject gameObject = other.transform.root.gameObject;
            if (gameObject.layer == 8)
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
                {
                    if (Extensions.GetPhotonView(gameObject).isMine)
                    {
                        this.isCollide = true;
                    }
                }
                else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                {
                    GameObject obj3 = Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().main_object;
                    if (obj3 != null && obj3 == gameObject)
                    {
                        this.isCollide = true;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (this.isCollide)
        {
            GameObject gameObject = other.transform.root.gameObject;
            if (gameObject.layer == 8)
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
                {
                    if (Extensions.GetPhotonView(gameObject).isMine)
                    {
                        this.isCollide = false;
                    }
                }
                else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Singleplayer)
                {
                    GameObject obj3 = Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().main_object;
                    if (obj3 != null && obj3 == gameObject)
                    {
                        this.isCollide = false;
                    }
                }
            }
        }
    }
}
