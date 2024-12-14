using UnityEngine;

[System.Serializable]
public class ComboAttack
{
    public string Name;
    public AttackType Attack;
    public string AnimationTrigger;
    public float ComboTime;
}

public class ComboManager : MonoBehaviour
{
    [Header("Animator Settings")]
    [SerializeField] private Animator _animator;

    [Header("VFX Manager")]
    [SerializeField] private SwordVFXManager _vfxManager;

    [Header("Combo Settings")]
    [SerializeField]
    private ComboAttack[] _combos = new ComboAttack[]
    {
        // new ComboAttack { Name = "FireAttack", Attack = new FireAttack(), AnimationTrigger = "Attack1", ComboTime = 0.5f },
        // new ComboAttack { Name = "IceAttack", Attack = new IceAttack(), AnimationTrigger = "Attack2", ComboTime = 0.5f },
        // new ComboAttack { Name = "LightningAttack", Attack = new LightningAttack(), AnimationTrigger = "Attack3", ComboTime = 0.5f }
    };

    private int _currentComboIndex = -1;
    private float _comboTimer = 0f;
    private bool _isComboActive = false;
    private string _currentAnimationTrigger = null;
    private string _currentComboType = null;

    private void Update()
    {
        if (IsGameActive())
        {
            HandleInput();
            UpdateComboTimer();
        }
    }

    private bool IsGameActive()
    {
        return GameManager.Instance != null && GameManager.Instance.IsGameActive;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TriggerAttack(0);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            TriggerAttack(1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            TriggerAttack(2);
        }
    }

    private void TriggerAttack(int attackIndex)
    {
        if (_isComboActive && attackIndex == _currentComboIndex + 1)
        {
            ContinueCombo();
        }
        else
        {
            StartCombo(attackIndex);
        }
    }


    public bool IsAttacking()
    {
        Debug.Log("isAttacking " + _isComboActive);
        return _isComboActive;
    }

    public bool CanDestroyEnemy(EnemyType enemyType)
    {
        foreach (string attackType in enemyType.canDestroyedBy)
        {
            if (_currentComboType == attackType)
                return true;
        }

        return false;
    }

    private void StartCombo(int attackIndex)
    {
        _currentComboIndex = attackIndex;
        _isComboActive = true;
        PlayComboAnimation();
    }


    private void ContinueCombo()
    {
        if (_currentComboIndex + 1 < _combos.Length)
        {
            _currentComboIndex++;
            PlayComboAnimation();
        }
        else
        {
            EndCombo();
        }
    }
    private void PlayComboAnimation()
    {
        if (_animator == null)
        {
            Debug.LogError("Animator is not assigned.");
            return;
        }

        ComboAttack currentCombo = _combos[_currentComboIndex];

        if (_currentAnimationTrigger == currentCombo.AnimationTrigger)
        {
            Debug.Log($"Animation {currentCombo.AnimationTrigger} is already playing. Ignoring input.");
            return;
        }

        _animator.SetTrigger(currentCombo.AnimationTrigger);
        _currentAnimationTrigger = currentCombo.AnimationTrigger;
        _currentComboType = currentCombo.Name;
        currentCombo.Attack.TriggerAttack(_vfxManager);

        _comboTimer = currentCombo.ComboTime;
    }
    private void UpdateComboTimer()
    {
        if (_isComboActive)
        {
            _comboTimer -= Time.deltaTime;
            if (_comboTimer <= 0f)
            {
                EndCombo();
            }
        }
    }

    private void EndCombo()
    {
        _isComboActive = false;
        _currentComboIndex = -1;
        _comboTimer = 0f;

        if (_animator != null)
        {
            _animator.SetTrigger("move");
        }

        _currentAnimationTrigger = null;
    }
}
