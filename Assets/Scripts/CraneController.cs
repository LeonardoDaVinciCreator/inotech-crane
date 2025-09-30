using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{    
    [SerializeField, Range(0, 10)]
    private float _moveSpeed;
    [SerializeField]
    private GameObject _trolley;
    [SerializeField]
    private Transform _hookTransform;

    [Space(10)]
    [Header("Animation")]
    [SerializeField]
    private InputActionReference _moveAction;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private string _completeness = "completeness";

    [Space(10)]
    [Header("Animation Settings")]
    [SerializeField, Range(0.1f, 2f)]
    private float _lerpSpeed = 0.5f;
    [SerializeField, Range(0f, 1f)]
    private float _minCompleteness = 0f;
    [SerializeField, Range(0f, 1f)]
    private float _maxCompleteness = 1f;

    private float _currentCompleteness = 0f;

    private void OnEnable()
    {
        if (_moveAction != null) _moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (_moveAction != null) _moveAction.action.Disable();
    }

    private void Awake()
    {
        _currentCompleteness = _animator.GetFloat(_completeness);
    }
    

    private void FixedUpdate()
    {
        MoveTrolley();
    }

    private void MoveTrolley()
    {
        if (_moveAction == null) return;

        Vector2 move = _moveAction.action.ReadValue<Vector2>();
        float direction = Mathf.Sign(move.x);//не зависит от силы нажатия джойстика
        float movement = move.x * _moveSpeed * 0.01f;

        float targetCompleteness = _currentCompleteness + movement;
        targetCompleteness = Mathf.Clamp(targetCompleteness, _minCompleteness, _maxCompleteness);        
        _currentCompleteness = Mathf.Lerp(_currentCompleteness, targetCompleteness, _lerpSpeed);
        
        _animator.SetFloat(_completeness, _currentCompleteness);
    }
}
