using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform _player;       // Reference to the player
    [SerializeField] private Vector3 _offset = new Vector3(0, 5, -10); // Offset from the player
    [SerializeField] private float _rotationSpeed = 5f; // Speed for rotating the camera around the player
    [SerializeField] private float _smoothSpeed = 0.125f; // Speed for smoothing the camera's position

    private float _currentRotationAngle;

    private void Start()
    {
        FindPlayer();
    }

    private void LateUpdate()
    {
        if (!IsPlayerAssigned())
            return;

        RotateCamera();
        UpdateCameraPosition();
    }

    private void FindPlayer()
    {
        if (_player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                _player = playerObject.transform;
            }
            else
            {
                Debug.LogError("No object with the Player tag found in the scene!");
            }
        }
    }

    private bool IsPlayerAssigned()
    {
        if (_player != null)
            return true;

        Debug.LogError("Player is not assigned to the CameraController!");
        return false;
    }

    private void RotateCamera()
    {
        float horizontalInput = GetHorizontalInput();
        UpdateRotationAngle(horizontalInput);
        UpdateOffsetWithRotation();
    }

    private float GetHorizontalInput()
    {
        return Input.GetAxis("Mouse X") * _rotationSpeed;
    }

    private void UpdateRotationAngle(float horizontalInput)
    {
        _currentRotationAngle += horizontalInput;
    }

    private void UpdateOffsetWithRotation()
    {
        Quaternion rotation = Quaternion.Euler(0, _currentRotationAngle, 0);
        _offset = rotation * new Vector3(0, _offset.y, -Mathf.Abs(_offset.z));
    }

    private void UpdateCameraPosition()
    {
        Vector3 desiredPosition = CalculateDesiredPosition();
        Vector3 smoothedPosition = SmoothPosition(desiredPosition);
        UpdateCameraTransform(smoothedPosition);
    }

    private Vector3 CalculateDesiredPosition()
    {
        return _player.position + _offset;
    }

    private Vector3 SmoothPosition(Vector3 desiredPosition)
    {
        return Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed);
    }

    private void UpdateCameraTransform(Vector3 smoothedPosition)
    {
        transform.position = smoothedPosition;
        LookAtPlayer();
    }

    private void LookAtPlayer()
    {
        Vector3 lookTarget = _player.position + Vector3.up * 1.5f;
        transform.LookAt(lookTarget);
    }
}