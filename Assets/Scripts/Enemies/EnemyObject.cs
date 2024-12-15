using UnityEngine;
using System;

public class EnemyObject : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private Transform _player;
    [SerializeField] private float _playerHeight = 1.5f;
    [SerializeField] private float _playerZOffset = 2f;

    [Header("Throw Settings")]
    [SerializeField] private float _throwHeight = 2f;

    [Header("Visual Settings")]
    [SerializeField] private Renderer _objectRenderer;

    [Header("Managers")]
    [SerializeField] private SliceManager _sliceManager;


    private Rigidbody _rigidbody;
    private EnemyType _enemyType;
    public Color ObjectColor { get; set; }
    public event Action<EnemyObject> OnDestroyed;
    private bool _hasCollided = false;

    public void DestroyEnemy()
    {
        OnDestroyed?.Invoke(this);
        gameObject.SetActive(false);
    }

    public void InitializeObject(EnemyType enemyType, Transform playerTransform)
    {
        _enemyType = enemyType;

        ObjectColor = enemyType.enemyColor;
        _player = playerTransform;
        InitializeComponents();
        StartAction();
    }


    public void StartAction()
    {
        ApplyThrowForce();
    }

    private void InitializeComponents()
    {
        _rigidbody = GetComponent<Rigidbody>();

        if (_rigidbody == null)
        {
            LogMissingComponent("Rigidbody");
        }

        SetObjectColor();
    }

    private void ApplyThrowForce()
    {
        if (!IsPlayerAssigned())
            return;

        Vector3 predictedPlayerPosition = PredictPlayerPosition();
        Vector3 targetPosition = CalculateTargetPosition(predictedPlayerPosition);
        Vector3 throwVelocity = CalculateThrowVelocity(transform.position, targetPosition, _throwHeight);

        _rigidbody?.AddForce(throwVelocity, ForceMode.VelocityChange);
        _rigidbody.useGravity = true;
    }

    private bool IsPlayerAssigned()
    {
        if (_player != null)
            return true;

        Debug.LogError("Player Transform is not assigned!");
        return false;
    }

    private Vector3 PredictPlayerPosition()
    {
        PlayerController playerController = _player.GetComponent<PlayerController>();
        Vector3 predictedPosition = _player.position;

        if (playerController != null)
        {
            Vector3 playerVelocity = playerController.Velocity;
            float timeToLand = CalculateTimeToLand(_throwHeight);
            predictedPosition += playerVelocity * timeToLand;
        }

        return predictedPosition;
    }

    private Vector3 CalculateTargetPosition(Vector3 predictedPlayerPosition)
    {
        return new Vector3(
            transform.position.x,
            _playerHeight,
            predictedPlayerPosition.z + _playerZOffset
        );
    }

    private float CalculateTimeToLand(float height)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);
        return Mathf.Sqrt(2 * height / gravity);
    }

    private Vector3 CalculateThrowVelocity(Vector3 start, Vector3 target, float height)
    {
        float gravity = Physics.gravity.y;
        float displacementY = target.y - start.y;
        Vector3 displacementXZ = new Vector3(target.x - start.x, 0, target.z - start.z);

        float time = Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (displacementY - height) / gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);
        Vector3 velocityXZ = displacementXZ / time;

        return velocityXZ + velocityY;
    }

    private void SetObjectColor()
    {
        if (_objectRenderer != null)
        {
            _objectRenderer.material.color = ObjectColor;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameActive)
            return;

        if (_hasCollided)
            return;

        _hasCollided = true;

        Debug.Log("OnCollisionEnter " + collision.gameObject);
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision);
        }
        else if (collision.gameObject.CompareTag("Sword"))
        {
            HandleSwordCollision(collision);
        }
        else if (collision.gameObject.CompareTag("Floor"))
        {
            TriggerGameOver();
        }
    }

    private void HandlePlayerCollision(Collision collision)
    {
        TriggerPlayerRagdoll(collision);
        TriggerGameOver();



        // Disables cause WIP
        // if (IsPlayerColorMatching(collision.gameObject))
        // {
        //     GameManager.Instance.AddScore(1);
        //     _sliceManager.SliceObject(gameObject, collision);
        // }
        // else
        // {
        //     TriggerPlayerRagdoll(collision);
        //     TriggerGameOver();
        // }
    }

    private bool IsPlayerColorMatching(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            LogMissingComponent("Player Renderer");
            return false;
        }
        return playerController.GetRendererColor() == ObjectColor;
    }

    private void TriggerPlayerRagdoll(Collision collision)
    {
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            Vector3 impactForce = _rigidbody.linearVelocity.normalized * _rigidbody.mass * 10f;
            playerController.EnableRagdoll(impactForce, collision.contacts[0].point);
        }
    }

    private void HandleSwordCollision(Collision collision)
    {
        Sword sword = collision.gameObject.GetComponent<Sword>();
        if (sword == null)
        {
            LogMissingComponent("Sword");
        }

        if (sword.ComboManager == null)
        {
            LogMissingComponent("sword.ComboManager");

        }


        if (sword != null && sword.ComboManager != null && sword.ComboManager.IsAttacking() && sword.ComboManager.CanDestroyEnemy(_enemyType))
        {
            GameManager.Instance.AddScore(1);
            _sliceManager.SliceObject(gameObject, collision);
        }
        else
        {
            TriggerGameOver();
        }
    }


    private void TriggerGameOver()
    {
        GameManager.Instance.GameOver();
    }

    private void LogMissingComponent(string componentName)
    {
        Debug.LogError($"{componentName} component missing on {gameObject.name}");
    }
}
