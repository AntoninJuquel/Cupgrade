using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamageable
{
    [SerializeField] int maxHealth;
    int health;
    enum State
    {
        Roaming,
        ChaseTarget,
        AttackTarget,
        Dead
    }

    [SerializeField] float targetRange = 5f;
    [SerializeField] float stopChaseRange = 10f;

    [SerializeField] State state;

    EnemyMovements enemyMovements;
    EnemyAttack enemyAttack;

    Vector3 roamingPosition;

    float distance;

    [SerializeField] float noMovementThreshold = 0.0001f;

    [SerializeField] Animator anim;

    private const int noMovementFrames = 3;
    Vector3[] previousLocations = new Vector3[noMovementFrames];
    private bool isMoving;

    private void Awake()
    {
        enemyAttack = GetComponent<EnemyAttack>();
        enemyMovements = GetComponent<EnemyMovements>();

        for (int i = 0; i < previousLocations.Length; i++)
        {
            previousLocations[i] = Vector3.zero;
        }

        health = maxHealth;
    }
    private void Start()
    {
        roamingPosition = GetRoamingPosition();
        state = State.Roaming;
    }
    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Roaming:
                enemyMovements.MoveTo(roamingPosition);

                distance = Vector3.Distance(transform.position, roamingPosition);
                if (distance < 3f || !isMoving)
                    roamingPosition = GetRoamingPosition();
                
                FindTarget();
                CheckMovements();
                break;
            case State.ChaseTarget:
                anim.SetBool("isAttacking", false);
                enemyAttack.Aim();
                enemyMovements.MoveTo(PlayerController.Instance.GetPosition());

                if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) < enemyAttack.GetAttackRange())
                    state = State.AttackTarget;
                else if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) > stopChaseRange)
                    state = State.Roaming;

                break;
            case State.AttackTarget:
                enemyAttack.Aim();
                enemyAttack.Attack();
                anim.SetBool("isAttacking",true);
                if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) > enemyAttack.GetAttackRange())
                {
                    enemyAttack.ResetAttack();
                    state = State.ChaseTarget;
                }
                if (health < maxHealth / 2)
                    anim.SetBool("isAngry", true);
                break;
            case State.Dead:
                break;
        }
    }
    void CheckMovements()
    {
        for (int i = 0; i < previousLocations.Length - 1; i++)
        {
            previousLocations[i] = previousLocations[i + 1];
        }
        previousLocations[previousLocations.Length - 1] = transform.position;

        for (int i = 0; i < previousLocations.Length - 1; i++)
        {
            if (Vector3.Distance(previousLocations[i], previousLocations[i + 1]) >= noMovementThreshold)
            {
                isMoving = true;

                break;
            }
            else
            {
                isMoving = false;
            }
        }
        anim.SetBool("isRunning", isMoving);
    }
    Vector3 GetRoamingPosition()
    {
        return transform.position + GetRandomDir() * Random.Range(10f, 15f);
    }

    Vector3 GetRandomDir()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    void FindTarget()
    {
        if (Vector3.Distance(transform.position, PlayerController.Instance.GetPosition()) < targetRange)
        {
            state = State.ChaseTarget;
        }
    }

    public void DecreaseHealth(int damage)
    {
        health -= damage;
        if (health <= 0)
            Die();
    }
    public void IncreaseHealth(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
    }
    public void Die()
    {
        LootManager.Instance.LootPowerUp(transform.position);
        GameManager.Instance.RemoveEnemy(gameObject, true);
        gameObject.layer = 2;
        enemyAttack.LastAttack();
        anim.SetBool("isAttacking", true);
        anim.SetBool("isDead", true);
        StartCoroutine(enemyMovements.BumpBody());
        state = State.Dead;
    }
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    public int GetHealth()
    {
        return health;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, stopChaseRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, targetRange);
    }
}
