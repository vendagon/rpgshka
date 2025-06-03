using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float acceleration = 10f;

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int damage = 20;
    [SerializeField] private bool attackPointActiveByDefault = false;

    [Header("Audio Settings")]
    [SerializeField] public AudioClip attackSound;

    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private HealthSystem _healthSystem;

    private Vector2 _moveInput;
    private float _currentSpeed;
    private float _lastAttackTime;
    private bool _isRunning;
    private bool _haveSword;
    private bool _isAttacking;
    private bool _controlsEnabled = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _healthSystem = GetComponent<HealthSystem>();

        if (attackPoint != null)
            attackPoint.gameObject.SetActive(attackPointActiveByDefault);
    }

    private void Start()
    {
        _currentSpeed = walkSpeed;
    }

    private void Update()
    {
        if (!_controlsEnabled) return;

        HandleInput();
        HandleAttack();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        if (!_controlsEnabled) return;

        HandleMovement();
    }

    private void HandleInput()
    {
        _moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")).normalized;

        _isRunning = Input.GetKey(KeyCode.LeftShift);
        _currentSpeed = Mathf.Lerp(_currentSpeed, _isRunning ? runSpeed : walkSpeed, acceleration * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleSword();
        }
    }

    private void ToggleSword()
    {
        _haveSword = !_haveSword;
        _animator.SetBool("HaveSword", _haveSword);
    }

    private void HandleMovement()
    {
        _rb.linearVelocity = _moveInput * _currentSpeed;
    }

    private void UpdateAnimations()
    {
        float scaleX = Mathf.Abs(transform.localScale.x);
        _animator.SetFloat("Horizontal", _moveInput.x);
        _animator.SetFloat("Vertical", _moveInput.y);
        _animator.SetFloat("Speed", _moveInput.sqrMagnitude);
        _animator.SetBool("IsRunning", _isRunning);

        if (_moveInput.x != 0)
        {
            transform.localScale = new Vector3(_moveInput.x > 0 ? -scaleX : scaleX, transform.localScale.y, transform.localScale.z);
        }
    }

    private void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1") && CanAttack() && _haveSword)
        {
            Attack();
        }
    }

    private bool CanAttack()
    {
        return Time.time >= _lastAttackTime + attackCooldown && !_isAttacking;
    }

    private void Attack()
    {
        _isAttacking = true;
        _lastAttackTime = Time.time;
        _animator.SetTrigger("Attack");

        if (attackPoint != null)
            attackPoint.gameObject.SetActive(true);
    }

    public void OnAttackHit()
    {
        if (attackPoint == null || !attackPoint.gameObject.activeSelf) return;

        AudioManager.Instance.PlaySFX(attackSound);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Health>()?.TakeDamage(damage);
        }
    }

    public void OnAttackEnd()
    {
        _isAttacking = false;
        if (attackPoint != null)
            attackPoint.gameObject.SetActive(false);
    }

    public void SetPositionAfterLoad(Vector3 position)
    {
        StartCoroutine(SetPositionNextFrame(position));
    }

    private IEnumerator SetPositionNextFrame(Vector3 position)
    {
        yield return null; // Ждем один кадр, чтобы все успело инициализироваться
        transform.position = position;
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
    public void SetControlsEnabled(bool enabled)
    {
        _controlsEnabled = enabled;

        // Останавливаем движение при отключении управления
        if (!enabled)
        {
            _rb.linearVelocity = Vector2.zero;
            _animator.SetFloat("Speed", 0f);
        }
    }
}