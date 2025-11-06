using UnityEngine;

public class EnemyController : MonoBehaviour, IEnemy
{
    [Header("기본 설정")]
    public float speed = 2f; // 이동속도 
    public float attackRange = 1.5f; // 공격 범위 
    public float returnDistance = 8f;  // 플레이어 감지 및 추적 범위
    public float heightThreshold = 1.5f; // 플레이어와의 높이 차이 기준 값 
    public Transform player;

    [HideInInspector] public Vector3 spawnPoint; // 스폰 위치 저장
    [HideInInspector] public bool isDead; // 사망 여부
    public bool IsDead => isDead; // IEnemy 참조용

    [HideInInspector] public Animator animator;
    private Rigidbody2D rb;

    // 상태 인스턴스
    [HideInInspector] public EnemyIdleState idleState;
    [HideInInspector] public EnemyRunState runState;
    [HideInInspector] public EnemyAttackState attackState;
    [HideInInspector] public EnemyDeadState deadState;

    private EnemyState currentState; 

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spawnPoint = transform.position;

        // 왼쪽을 바라보도록 초기화 
        Vector3 scale = transform.localScale;
        scale.x = -Mathf.Abs(scale.x);
        transform.localScale = scale;

        // 상태생성 
        idleState = new EnemyIdleState(this);
        runState = new EnemyRunState(this);
        attackState = new EnemyAttackState(this);
        deadState = new EnemyDeadState(this);

        currentState = idleState; // 시작 상태 
        currentState.Enter();
    }

    void Update()
    {
        // 플레이어가 없다면 
        if (player == null) 
        {
            StopMoving(); // 이동 중지 
            if (currentState != idleState) // 현재 상태가 Idle 이 아니면
            {
                ChangeState(idleState); // Idle  상태로 전환
            }
            return;
        }

        // 현재 상태의 업데이트 실행
        currentState.Update();
    }

    // 현재 상태 종료 후 새 상태 시작
    public void ChangeState(EnemyState newState)
    {
        if (currentState == null)
        {
            return;
        }
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    // 플레이어가 감지 범위 안에 있는지 확인
    public bool IsPlayerInRange()
    {
        return player != null && Vector2.Distance(transform.position, player.position) < returnDistance;
    }

    // 타겟 위치로 이동 
    public void MoveTo(Vector3 target)
    {
        if (isDead)
        {
            return;
        }
        Vector2 dir = (target - transform.position).normalized; // 타겟으로 향하는 이동 방향 계산 
        rb.velocity = dir * speed; // 속도 적용 
        Flip(dir.x); // 이동 방향에 맞게 캐릭터 반전 
    }

    public void StopMoving()
    {
        rb.velocity = Vector2.zero;
    }

    // x 축 방향에 따라 캐릭터 반전 
    public void Flip(float dirX)
    {
        if ((dirX > 0 && transform.localScale.x < 0) ||
            (dirX < 0 && transform.localScale.x > 0))
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // 플레이어의 방향 바라보기 
    public void LookAtPlayer()
    {
        if (player == null)
        {
            return;
        }
        Flip(player.position.x - transform.position.x);
    }

    public void Die()
    { 
        // 이미 사망한 상태면 무시 
        if (isDead)
        {
            return;
        }

        isDead = true;
        ChangeState(deadState); // 사망 상태로 전환 

        //  플레이어가 적을 다 죽였을때의 게임 클리어 조건 처리 
        Invoke(nameof(CheckAfterDeath), 0.25f);
    }

    // 게임 매니저에서 현재 적이 모두 죽었는지 확인, 모든 적이 죽었다면, 씬 2에서 게임 클리어처리  
    private void CheckAfterDeath()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CheckGameClear();
        }
    }

    // 공격 애니메이션에서 호출되는 히트 이벤트 처리 
    public void OnAttackHit()
    {
        attackState?.HandleAttackHitFromAnimation();
    }
}

