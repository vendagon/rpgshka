using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private State startingState;
    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamingDistanceMin = 3f;
    [SerializeField] private float roamingTimerMax = 2f;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float roamSpeed = 2f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float lostPlayerDelay = 1.5f;
    private float lastAttackTime = -999f;
    private float lostPlayerTimer = 0f;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private State state;
    private float roamingTime;
    private Vector3 startingPosition;
    private bool isDead = false;
    private float distanceToPlayer;
    public bool isAttacking = false;
    private State previousState;
    private Health health;

    private enum State
    {
        Idle,
        Roaming,
        Chasing,
        Attack,
        Die
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        startingPosition = transform.position;

        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDamaged += HandleDamaged;
            health.OnDeath += HandleDeath;
        }

        state = startingState;
    }

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (isDead) return;

        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (state != previousState)
        {
            OnStateExit(previousState);
            previousState = state;
        }

        switch (state)
        {
            case State.Roaming: RoamingBehavior(); break;
            case State.Chasing: ChasingBehavior(); break;
            case State.Attack: AttackingBehavior(); break;
            default: IdleBehavior(); break;
        }

        CheckPlayerDetection();
        UpdateAnimations();
    }

    private void OnStateExit(State oldState)
    {
        if (oldState == State.Attack)
        {
            isAttacking = false;
            animator.ResetTrigger("Attack");
            navMeshAgent.isStopped = false;
        }
    }

    private void RoamingBehavior()
    {
        animator.SetBool("IsMoving", true);
        roamingTime -= Time.deltaTime;

        if (roamingTime <= 0 || navMeshAgent.remainingDistance < 0.1f)
        {
            Vector3 roamPosition = GetRoamingPosition();
            ChangeFacingDirection(transform.position, roamPosition);
            navMeshAgent.speed = roamSpeed;
            navMeshAgent.SetDestination(roamPosition);
            roamingTime = roamingTimerMax;
        }
    }

    private void ChasingBehavior()
    {
        animator.SetBool("IsMoving", true);
        navMeshAgent.speed = chaseSpeed;
        navMeshAgent.SetDestination(player.position);
        ChangeFacingDirection(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            state = State.Attack;
            lostPlayerTimer = 0f;
        }
        else if (distanceToPlayer > chaseRange * 1.5f)
        {
            lostPlayerTimer += Time.deltaTime;
            if (lostPlayerTimer >= lostPlayerDelay)
            {
                state = State.Roaming;
                lostPlayerTimer = 0f;
            }
        }
        else
        {
            lostPlayerTimer = 0f;
        }
    }

    private void AttackingBehavior()
    {
        if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
        {
            navMeshAgent.isStopped = true;
            animator.SetTrigger("Attack");
            isAttacking = true;
            lastAttackTime = Time.time;
        }
        else if (distanceToPlayer > attackRange * 1.2f)
        {
            isAttacking = false;
            navMeshAgent.isStopped = false;
            state = State.Chasing;
        }
    }

    private void IdleBehavior()
    {
        animator.SetBool("IsMoving", false);
    }

    private void CheckPlayerDetection()
    {
        if (state == State.Attack) return; // Не прерываем текущую атаку

        if (distanceToPlayer <= attackRange)
        {
            state = State.Attack;
        }
        else if (distanceToPlayer <= chaseRange)
        {
            state = State.Chasing;
        }
        else if (state == State.Chasing)
        {
            state = State.Roaming;
        }
    }

    private void UpdateAnimations()
    {
        // Рассчитываем движение
        bool isActuallyMoving = navMeshAgent.velocity.magnitude > 0.1f;
        bool shouldShowMoveAnimation = (state == State.Roaming || state == State.Chasing) && isActuallyMoving;

        // Управляем анимациями
        animator.SetBool("IsMoving", shouldShowMoveAnimation);
        animator.SetBool("IsChasing", state == State.Chasing);
        animator.SetFloat("MoveSpeed", navMeshAgent.velocity.magnitude / roamSpeed);
    }

    private Vector3 GetRoamingPosition()
    {
        return startingPosition + UnityEngine.Random.insideUnitSphere * Random.Range(roamingDistanceMin, roamingDistanceMax);
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        transform.rotation = Quaternion.Euler(0, sourcePosition.x > targetPosition.x ? -180 : 0, 0);
    }

    // Вызывается из анимации атаки
    public void OnAttackHit()
    {
        if (distanceToPlayer <= attackRange * 1.2f)
        {
            // Нанесение урона игроку
            player.GetComponent<HealthSystem>()?.TakeDamage(attackDamage);
        }
    }

    public void OnAttackEnd()
    {
        isAttacking = false;

        if (distanceToPlayer > attackRange * 1.2f)
        {
            state = State.Chasing;
            navMeshAgent.isStopped = false;
        }
    }
    private void HandleDamaged()
    {
        if (!isDead)
        {
            isAttacking = false;
            animator.ResetTrigger("Attack"); // Сброс атаки
            animator.SetTrigger("Hit"); // Запуск анимации урона
            navMeshAgent.isStopped = false; // На всякий случай включаем движение
        }
    }
    private void HandleDeath()
    {
        isDead = true;
        state = State.Die;
        navMeshAgent.isStopped = true;
        animator.SetTrigger("Die");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}