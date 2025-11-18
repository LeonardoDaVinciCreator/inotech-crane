using System.Collections.Generic;
using UnityEngine;

public class Gun : BaseWeapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 50f;
    [SerializeField] private int poolSize = 20;

    private List<GameObject> bulletPool;
    private int poolIndex = 0;

    private void Awake()
    {
        bulletPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Add(bullet);
        }
    }

    protected override void Fire()
    {
        GameObject bullet = bulletPool[poolIndex];
        poolIndex = (poolIndex + 1) % poolSize;

        bullet.SetActive(true);
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;

        if (bullet.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }
    }
}