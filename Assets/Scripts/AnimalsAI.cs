using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(SpriteRenderer))]
public class AnimalAI : MonoBehaviour
{
    [Header("Настройки")]
    public float detectionRadius = 6f;
    public float fleeDistance = 10f;
    public float minIdleTime = 2f;
    public float maxIdleTime = 5f;
    public float roamingRadius = 15f;

    [Header("Скорости")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    [Header("Ссылки")]
    public Transform player;
    public LayerMask obstacleMask = 1; // Default layer

    // Приватные переменные
    private NavMeshAgent agent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector3 spawnPosition;
    private float idleTimer;
    private float currentIdleTime;
    private Vector3 lastPlayerPosition;

    // Отладочные переменные
    private bool playerVisible;
    private Vector3 currentDestination;

    private enum AnimalState { Idle, Roaming, Fleeing }
    private AnimalState currentState = AnimalState.Idle;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnPosition = transform.position;

        // Критические настройки для 2D
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = walkSpeed;

        // Автопоиск игрока если не назначен
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null) Debug.LogError("Игрок не найден!");
        }

        currentIdleTime = Random.Range(minIdleTime, maxIdleTime);
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        if (CheckPlayerVisibility())
        {
            StartFleeing();
        }

        FixTransform();
        UpdateStateMachine();
        UpdateAnimation();
    }

    void FixTransform()
    {
        // Фиксируем позицию и поворот
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        transform.rotation = Quaternion.identity;
    }

    void UpdateStateMachine()
    {
        playerVisible = CheckPlayerVisibility();

        switch (currentState)
        {
            case AnimalState.Idle:
                UpdateIdleState();
                break;

            case AnimalState.Roaming:
                UpdateRoamingState();
                break;

            case AnimalState.Fleeing:
                UpdateFleeingState();
                break;
        }
    }

    bool CheckPlayerVisibility()
    {
        return Vector3.Distance(transform.position, player.position) <= detectionRadius;
    }

    void UpdateIdleState()
    {
        idleTimer += Time.deltaTime;

        if (playerVisible)
        {
            StartFleeing();
            return;
        }

        if (idleTimer >= currentIdleTime)
        {
            StartRoaming();
        }
    }

    void UpdateRoamingState()
    {
        if (playerVisible)
        {
            StartFleeing();
            return;
        }

        if (agent.remainingDistance < 0.2f)
        {
            ReturnToIdle();
        }
    }

    void UpdateFleeingState()
    {
        if (playerVisible)
        {
            FleeFromPlayer();
        }
        else if (Vector3.Distance(transform.position, lastPlayerPosition) > detectionRadius * 1.5f)
        {
            ReturnToIdle();
        }
    }

    void StartFleeing()
    {
        currentState = AnimalState.Fleeing;
        agent.speed = runSpeed;
        FleeFromPlayer();
    }

    void FleeFromPlayer()
    {

        Vector3 fleeDirection = (transform.position - player.position).normalized;
        Vector3 fleeTarget = transform.position + fleeDirection * fleeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleeTarget, out hit, fleeDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);

            if (agent.pathStatus != NavMeshPathStatus.PathComplete)
            {
                Debug.LogWarning($"Путь неполный! Статус: {agent.pathStatus}");
            }
        }
        else
        {
            Debug.LogError("Не найдена точка для бегства на NavMesh!");
        }
    }

    void StartRoaming()
    {
        currentState = AnimalState.Roaming;
        agent.speed = walkSpeed;

        Vector2 randomDirection = Random.insideUnitCircle.normalized * Random.Range(2f, roamingRadius);
        Vector3 roamTarget = spawnPosition + new Vector3(randomDirection.x, randomDirection.y, 0);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(roamTarget, out hit, roamingRadius, NavMesh.AllAreas))
        {
            currentDestination = hit.position;
            agent.SetDestination(hit.position);
        }
    }

    void ReturnToIdle()
    {
        currentState = AnimalState.Idle;
        agent.speed = walkSpeed;
        currentIdleTime = Random.Range(minIdleTime, maxIdleTime);
        idleTimer = 0f;
    }

    void UpdateAnimation()
    {
        float speed = agent.velocity.magnitude / runSpeed;
        animator.SetFloat("Speed", speed);

        if (agent.velocity.x > 0.1f) spriteRenderer.flipX = false;
        else if (agent.velocity.x < -0.1f) spriteRenderer.flipX = true;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying || player == null) return;

        // Линия к игроку
        Gizmos.color = playerVisible ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, player.position);

        // Текущий путь
        if (agent.hasPath)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < agent.path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(agent.path.corners[i], agent.path.corners[i + 1]);
            }
        }
    }
}