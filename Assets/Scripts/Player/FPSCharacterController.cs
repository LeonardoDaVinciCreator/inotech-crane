using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSCharacterController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Child object that holds the camera — rotates vertically (up/down)")]
    [SerializeField] private Transform _cameraPivot;
    [SerializeField] private Transform _gunPivot;

    [Header("Movement")]
    [SerializeField, Range(1f, 20f)] private float _walkSpeed = 5f;
    [SerializeField, Range(1f, 20f)] private float _runSpeed = 10f;
    [SerializeField, Range(1f, 10f)] private float _jumpHeight = 2f;
    [SerializeField, Range(0.1f, 10f)] private float _gravity = 9.81f;
    [SerializeField, Range(0.01f, 1f)] private float _speedSmoothTime = 0.1f;

    [Header("Mouse Look")]
    [SerializeField, Range(1f, 10f)] private float _mouseSensitivity = 3f;
    [SerializeField, Range(5f, 20f)] private float _lookSmoothness = 12f;
    [SerializeField] private float _minLookAngle = -89f;
    [SerializeField] private float _maxLookAngle = 89f;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _lookAction;
    [SerializeField] private InputActionReference _jumpAction;
    [SerializeField] private InputActionReference _runAction;

    private CharacterController _controller;
    private Vector3 _velocity;
    private float _currentSpeed;

    private float _currentCameraVerticalAngle = 0f;
    private float _targetCameraVerticalAngle = 0f;
    private float _currentHorizontalRotation = 0f;
    private float _targetHorizontalRotation = 0f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();

        if (_cameraPivot == null)
            Debug.LogError("Assign the Camera Pivot (child object with Camera) in the inspector!");

        _currentHorizontalRotation = transform.eulerAngles.y;
        _targetHorizontalRotation = _currentHorizontalRotation;

        _currentCameraVerticalAngle = _cameraPivot != null ? _cameraPivot.localEulerAngles.x : 0f;
        _targetCameraVerticalAngle = _currentCameraVerticalAngle;

        if (_currentCameraVerticalAngle > 180f)
            _currentCameraVerticalAngle -= 360f;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        EnableInput();
    }

    private void OnDisable()
    {
        DisableInput();
    }

    private void EnableInput()
    {
        if (_moveAction?.action != null) _moveAction.action.Enable();
        if (_lookAction?.action != null) _lookAction.action.Enable();
        if (_jumpAction?.action != null)
        {
            _jumpAction.action.Enable();
            _jumpAction.action.performed += OnJump;
        }
        if (_runAction?.action != null) _runAction.action.Enable();
    }

    private void DisableInput()
    {
        if (_moveAction?.action != null) _moveAction.action.Disable();
        if (_lookAction?.action != null) _lookAction.action.Disable();
        if (_jumpAction?.action != null)
        {
            _jumpAction.action.Disable();
            _jumpAction.action.performed -= OnJump;
        }
        if (_runAction?.action != null) _runAction.action.Disable();
    }

    private void Update()
    {
        HandleLook();
        UpdateGunRotation();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleLook()
    {
        if (_lookAction?.action == null) return;

        Vector2 lookInput = _lookAction.action.ReadValue<Vector2>();

        float mouseX = lookInput.x * _mouseSensitivity;
        _targetHorizontalRotation += mouseX;
        _currentHorizontalRotation = Mathf.LerpAngle(_currentHorizontalRotation, _targetHorizontalRotation, _lookSmoothness * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, _currentHorizontalRotation, 0f);

        float mouseY = lookInput.y * _mouseSensitivity;
        _targetCameraVerticalAngle -= mouseY;
        _targetCameraVerticalAngle = Mathf.Clamp(_targetCameraVerticalAngle, _minLookAngle, _maxLookAngle);
        _currentCameraVerticalAngle = Mathf.Lerp(_currentCameraVerticalAngle, _targetCameraVerticalAngle, _lookSmoothness * Time.deltaTime);

        if (_cameraPivot != null)
        {
            _cameraPivot.localRotation = Quaternion.Euler(_currentCameraVerticalAngle, 0f, 0f);
        }
    }

    private void UpdateGunRotation()
    {
        if (_gunPivot != null)
        {
            _gunPivot.rotation = Quaternion.Euler(_currentCameraVerticalAngle, _currentHorizontalRotation, 0f);
        }
    }

    private void HandleMovement()
    {
        if (_moveAction?.action == null) return;

        Vector2 moveInput = _moveAction.action.ReadValue<Vector2>();
        bool isRunning = _runAction?.action != null && _runAction.action.ReadValue<float>() > 0.1f;

        float targetSpeed = (isRunning ? _runSpeed : _walkSpeed) * moveInput.magnitude;
        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, _speedSmoothTime);

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        // Handle gravity and grounding
        if (_controller.isGrounded)
        {
            if (_velocity.y < 0f)
                _velocity.y = 0f;
        }
        else
        {
            _velocity.y -= _gravity * Time.fixedDeltaTime;
        }

        _controller.Move(moveDirection * _currentSpeed * Time.fixedDeltaTime + _velocity * Time.fixedDeltaTime);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (_controller.isGrounded)
        {
            _velocity.y = Mathf.Sqrt(2f * _jumpHeight * _gravity);
        }
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        _mouseSensitivity = Mathf.Clamp(sensitivity, 1f, 10f);
    }

    public void SetLookSmoothness(float smoothness)
    {
        _lookSmoothness = Mathf.Clamp(smoothness, 5f, 20f);
    }
}