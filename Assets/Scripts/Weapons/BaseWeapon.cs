using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BaseWeapon : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected float fireRate = 0.2f;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected InputActionReference fireAction;

    protected float lastFireTime;

    protected virtual void OnEnable()
    {
        fireAction?.action.Enable();
        fireAction.action.performed += OnFire;
    }

    protected virtual void OnDisable()
    {
        fireAction?.action.Disable();
        fireAction.action.performed -= OnFire;
    }

    protected virtual void OnFire(InputAction.CallbackContext context)
    {
        if (Time.time - lastFireTime < fireRate)
            return;

        lastFireTime = Time.time;
        Fire();
    }

    protected abstract void Fire();
}