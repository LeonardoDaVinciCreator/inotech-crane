using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CraneCableController : MonoBehaviour
{
    [SerializeField]
    private List<ConfigurableJoint> _joints;

    [Space(10)]
    [Header("Settings")]
    [SerializeField]
    private float lenghtFactor = 10f;
    [SerializeField]
    private float _minLenght = 0.2f;
    [SerializeField]
    private float _maxLenght = 5f;

    [Space(5)]
    [SerializeField]
    private InputActionReference _moveAction;

    private void OnEnable()
    {
        if (_moveAction != null) _moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (_moveAction != null) _moveAction.action.Disable();
    }

    private void FixedUpdate()
    {
        AdjustCableLength();
    }

    private void AdjustCableLength()
    {
        if (_moveAction == null) return;

        float move = _moveAction.action.ReadValue<float>();
        Debug.Log("Move input: " + move);
        float delta = move * lenghtFactor * Time.fixedDeltaTime;

        foreach (ConfigurableJoint joint in _joints)
        {
            SoftJointLimit limit = joint.linearLimit;
            
            float newLimit = Mathf.Clamp(limit.limit - delta, _minLenght, _maxLenght);

            if (Mathf.Approximately(newLimit, limit.limit))
                continue;

            limit.limit = newLimit;
            joint.linearLimit = limit;
        }
    }
}
