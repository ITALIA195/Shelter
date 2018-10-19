using UnityEngine;
using Extensions = Photon.Extensions;

public class LevelBottom : MonoBehaviour
{
    public GameObject link;
    public BottomType type;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (this.type == BottomType.Die)
            {
                if (other.gameObject.GetComponent<HERO>() != null)
                {
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multiplayer)
                    {
                        if (Extensions.GetPhotonView(other.gameObject).isMine)
                        {
                            other.gameObject.GetComponent<HERO>().netDieLocal(rigidbody.velocity * 50f, false, -1, string.Empty, true);
                        }
                    }
                    else
                    {
                        other.gameObject.GetComponent<HERO>().die(other.gameObject.rigidbody.velocity * 50f, false);
                    }
                }
            }
            else if (this.type == BottomType.Teleport)
            {
                if (this.link != null)
                {
                    other.gameObject.transform.position = this.link.transform.position;
                }
                else
                {
                    other.gameObject.transform.position = Vector3.zero;
                }
            }
        }
    }

    private void Start()
    {
    }

    private void Update()
    {
    }
}
