using UnityEngine;
using Photon.Pun;

public class Destroyer : MonoBehaviourPun
{
    public float destroyTime;

    void Start()
    {
        Invoke("DestroyMe", destroyTime);
    }

    void DestroyMe()
    {
        //PhotonNetwork.Destroy(gameObject);
        photonView.RPC("DestroyRPC", RpcTarget.All);
    }

    [PunRPC]
    void DestroyRPC()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            //CancelInvoke();
            DestroyMe();
        }
    }
}
