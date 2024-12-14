using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _laneSwitchSpeed = 10f;
    [SerializeField] private float gravity = -9.81f; // Gravity force

    [Header("Appearance Settings")]
    [SerializeField] private Renderer _swordMeshRenderer;
    [SerializeField] private SwordVFXManager _swordVFXManager;
    [SerializeField] private PlayerAnimator _playerAnimator;
    [SerializeField] private ComboManager _comboManager;

    [Header("Lane Settings")]
    [SerializeField] private LaneManager _laneManager;

    private CharacterController _controller;
    private Rigidbody[] _ragdollRigidbodies;
    private Collider[] _ragdollColliders;

    private static readonly Color[] _colors = { Color.red, Color.blue, Color.green };
    private int _currentColorIndex;
    private int _currentLane = 1;
    private Vector3 _targetPosition;
    private Vector3 _lastPosition;
    private Vector3 _startPosition;

    public Vector3 Velocity { get; private set; }
    private float verticalVelocity = 0f; // Tracks vertical movement

    private void Start()
    {
        InitializeComponents();
        ApplyColor(0);
    }



    private void InitializeComponents()
    {
        _controller = GetComponent<CharacterController>();
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        _ragdollColliders = GetComponentsInChildren<Collider>();

        if (_laneManager == null)
        {
            Debug.LogError("LaneManager is not assigned in the Inspector.");
        }

        _startPosition = transform.position;
        _targetPosition = transform.position;
        DisableRagdoll();
        InitializeSword();
    }

    private void InitializeSword()
    {
        Sword sword = GetComponentInChildren<Sword>();
        if (sword != null)
        {
            sword.ComboManager = GetComponent<ComboManager>();
        }
    }

    private void Update()
    {
        if (IsGameActive())
        {

            HandleInput();
            HandleColorChange();
        };

    }

    private void FixedUpdate()
    {
        if (IsGameActive())
        {
            HandleMovement();
        };

    }

    private bool IsGameActive()
    {
        return GameManager.Instance != null && GameManager.Instance.IsGameActive;
    }



    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) && _laneManager.CanSwitchLaneLeft(_currentLane))
        {
            _currentLane--;
            UpdateTargetPosition();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && _laneManager.CanSwitchLaneRight(_currentLane))
        {
            _currentLane++;
            UpdateTargetPosition();
        }
    }

    private void UpdateTargetPosition()
    {
        float laneX = _laneManager.GetLanePosition(_currentLane);
        _targetPosition = new Vector3(laneX, transform.position.y, transform.position.z);
    }

    private void HandleMovement()
    {
        // Calculate forward movement
        Vector3 forwardMovement = Vector3.forward * _moveSpeed * Time.deltaTime;

        // Apply gravity
        if (_controller.isGrounded)
        {
            verticalVelocity = 0f; // Reset vertical velocity when on the ground
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; // Apply gravity over time
        }

        // Smoothly move horizontally to the target lane position
        Vector3 lateralMovement = new Vector3(
            Mathf.Lerp(transform.position.x, _targetPosition.x, _laneSwitchSpeed * Time.deltaTime),
            verticalVelocity * Time.deltaTime, // Include vertical movement due to gravity
            transform.position.z
        );

        // Combine forward and lateral movement
        Vector3 movement = forwardMovement + (lateralMovement - transform.position);

        _playerAnimator.PlayWalkingAnimation(1f); // Ensure animations are triggered correctly

        MovePlayer(movement);
    }
    private void MovePlayer(Vector3 movement)
    {
        if (_controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0f; // Reset vertical velocity if grounded
        }

        movement.y = verticalVelocity * Time.deltaTime;
        _controller.Move(movement); // Move the character
    }

    private void HandleColorChange()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ChangeColor();
        }
    }

    private void ChangeColor()
    {
        _currentColorIndex = (_currentColorIndex + 1) % _colors.Length;
        ApplyColor(_currentColorIndex);
    }

    private void ApplyColor(int colorIndex)
    {
        _swordMeshRenderer.material.color = _colors[colorIndex];
        TriggerVFX();
    }

    private void TriggerVFX()
    {
        // _swordVFXManager?.PlayVFX();
    }

    public void StartGame()
    {
        DisableRagdoll();
        ResetPlayerPosition();
    }

    public void GameOver()
    {
        _playerAnimator.StopWalkingAnimation();
    }

    public void ResetPlayerPosition()
    {
        transform.position = _startPosition;
    }

    public void EnableRagdoll(Vector3 force, Vector3 hitPoint)
    {
        _playerAnimator.DisableAnimator();
        _controller.enabled = false;

        foreach (var rb in _ragdollRigidbodies)
        {
            if (!rb.gameObject.CompareTag("Sword"))
            {
                rb.isKinematic = false;
            }
        }

        foreach (var col in _ragdollColliders)
        {
            if (!col.gameObject.CompareTag("Sword"))
            {
                col.enabled = true;
            }
        }

        Rigidbody closestRigidbody = GetClosestRigidbody(hitPoint);
        closestRigidbody?.AddForce(force, ForceMode.Impulse);
    }

    public void DisableRagdoll()
    {
        foreach (var rb in _ragdollRigidbodies)
        {
            if (!rb.gameObject.CompareTag("Sword"))
            {
                rb.isKinematic = true;
            }
        }

        foreach (var col in _ragdollColliders)
        {
            if (!col.gameObject.CompareTag("Sword"))
            {
                col.enabled = false;
            }
        }

        _controller.enabled = true;
        _playerAnimator.EnableAnimator();
    }

    private Rigidbody GetClosestRigidbody(Vector3 hitPoint)
    {
        Rigidbody closest = null;
        float closestDistance = float.MaxValue;

        foreach (var rb in _ragdollRigidbodies)
        {
            float distance = Vector3.Distance(rb.position, hitPoint);
            if (distance < closestDistance)
            {
                closest = rb;
                closestDistance = distance;
            }
        }

        return closest;
    }

    public Color GetRendererColor()
    {
        return _swordMeshRenderer.material.color;
    }
}