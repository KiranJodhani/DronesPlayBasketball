using UnityEngine;
using Photon.Pun;

public class BulletInstance : MonoBehaviourPun
{
    private string CollideObjectTag;
    public GameObject explosionPrefab;
    public float destroyTime = 3;
    public float bulletForce = 15f;

    void Start()
    {
        Invoke("DestroyBullet", destroyTime);
    }

    private void OnEnable()
    {
        transform.GetComponent<Rigidbody>().velocity = transform.forward * bulletForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        CollideObjectTag = collision.gameObject.tag;

        if (CollideObjectTag == "Ball")
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
            Invoke("DestroyBullet", 0.1f);
        }
        else
        {
            DestroyBullet();
        }
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}