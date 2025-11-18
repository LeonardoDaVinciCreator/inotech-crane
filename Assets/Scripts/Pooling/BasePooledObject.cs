using UnityEngine;

public abstract class BasePooledObject : MonoBehaviour
{
    public virtual void OnSpawn()
    {
        gameObject.SetActive(true);
    }

    public virtual void OnDespawn()
    {
        gameObject.SetActive(false);
    }
}