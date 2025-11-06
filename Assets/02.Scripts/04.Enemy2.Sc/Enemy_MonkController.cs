using UnityEngine;

// 적 몽크(Enemy Monk) 컨트롤러 클래스
public class Enemy_MonkController : MonoBehaviour, IEnemy
{
    [Header("설정")]
    public float speed = 3f;  // 이동 속도 (변수: speed)
    public float detectRange = 7f; // 플레이어 탐지 범위 
    public float attackRange = 5f;  // 공격 범위 
    public float heightThreshold = 1f;  // 높이 차이 기준
    public Transform player;    // 플레이어 Transform 참조 
    public GameObject kunaiPrefab;  // 쿠나이 프리팹 
    public Transform firePoint;  // 쿠나이 발사 위치

    public bool IsDead { get; private set; } = false; // 적 사망 여부 
    public Animator animator; // 애니메이터 참조 
    public Vector2 startPos; // 초기 위치

    // 상태 인스턴스 변수
    public Enemy_MonkIdleState idleState;    
    public Enemy_MonkRunState runState;      
    public Enemy_MonkAttackState attackState; 
    public Enemy_MonkDeadState deadState;    

    private Enemy_MonkState currentState;    // 현재 상태 
    private bool hasThrownKunai = false;     // 쿠나이 공격 여부 체크 

    private void Awake()
    {
        animator = GetComponent<Animator>(); // Animator 컴포넌트 가져오기
        startPos = transform.position;       // 초기 위치 저장

        // 각 상태 초기화 및 참조 전달
        idleState = new Enemy_MonkIdleState(this);
        runState = new Enemy_MonkRunState(this);
        attackState = new Enemy_MonkAttackState(this);
        deadState = new Enemy_MonkDeadState(this);
    }

    private void Start()
    {
        ChangeState(idleState); // 게임 시작 시 Idle 상태 진입

        // 초기 방향 설정 (왼쪽 바라보기)
        Vector3 scale = transform.localScale;
        scale.x = -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void OnEnable()
    {
        // 플레이어 사망 이벤트 등록
        PlayerController.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        // 플레이어 사망 이벤트 해제
        PlayerController.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void Update()
    {
        if (!IsDead)
        {
            // 현재 상태 업데이트
            currentState?.Update();
        }
    }

    // 상태 전환 함수
    public void ChangeState(Enemy_MonkState newState)
    {
        // 사망 상태 이후에는 DeadState만 유지
        if (IsDead && !(newState is Enemy_MonkDeadState))
            return;

        currentState?.Exit();    // 이전 상태 종료
        currentState = newState; // 새로운 상태 설정
        currentState?.Enter();   // 새로운 상태 진입
    }

    // 목표 위치로 이동
    public void MoveTo(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    // 플레이어를 바라보도록 회전
    public void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 dir = player.position - transform.position; // 플레이어 방향 계산
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (dir.x > 0 ? 1 : -1); // x축 반전
        transform.localScale = scale;

        // 발사 포인트도 동일
        if (firePoint != null)
        {
            Vector3 fireLocal = firePoint.localPosition;
            fireLocal.x = Mathf.Abs(fireLocal.x) * (dir.x > 0 ? 1 : -1);
            firePoint.localPosition = fireLocal;
        }
    }

    // 쿠나이 발사
    public void FireKunai()
    {
        if (IsDead || kunaiPrefab == null || firePoint == null || player == null)
        {
            return;
        }

        if (hasThrownKunai) 
        {
            return;
        }

        hasThrownKunai = true;

        GameObject kunai = Instantiate(kunaiPrefab, firePoint.position, Quaternion.identity); // 쿠나이 생성
        Vector2 dir = (player.position - firePoint.position).normalized; // 방향 계산

        Kunai kScript = kunai.GetComponent<Kunai>();
        if (kScript != null)
        {
            kScript.SetDirection(dir); // 쿠나이 방향 설정
            kScript.SetOwner(this); // 발사 주체 설정
        }
    }

    // 쿠나이 던지기 상태 초기화
    public void ResetKunaiThrow()
    {
        hasThrownKunai = false;
    }

    // 적 사망 처리
    public void Die()
    {
        if (IsDead) 
        {
            return;  
        } 

        IsDead = true;  // 사망 상태로 전환
        ChangeState(deadState); // DeadState로 상태 전환

        Invoke(nameof(CheckAfterDeath), 0.25f); // 사망 후 Game Clear 체크
    }

    // 사망 후 게임 클리어 확인
    private void CheckAfterDeath()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CheckGameClear();
        }
    }

    // 플레이어 사망 처리 이벤트
    private void HandlePlayerDeath()
    {
        if (IsDead) 
        {
            return;
        } 

        ChangeState(runState); // RunState로 복귀
        runState.MoveToStart();
    }
}

