using UnityEngine;

public class Bullet : BasePooledObject
{
    [SerializeField] private float _lifeTime = 3f;
    private float _timer;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        _timer = 0f;
        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _lifeTime)
        {
            OnDespawn();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnDespawn();
    }
}