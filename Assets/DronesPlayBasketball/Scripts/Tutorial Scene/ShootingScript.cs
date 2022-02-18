using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    public Transform target;
    public Transform firePos;
    public GameObject bulletPrefab;
    public bool canFire;

    private void Start()
    {
        ShootTarget();
    }

    void Update()
    {
        transform.LookAt(target);
    }

    public void ShootTarget()
    {
        if (GameUIController.CanFireBullet)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePos.position, firePos.rotation);
            bullet.SetActive(true);
        }
        Invoke("ShootTarget", 0.25f);
    }
}