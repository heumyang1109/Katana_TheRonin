using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float speed = 6f; // 이동 속도 값 
    public float jumpUp = 11f; // 점프 하는 힘 
    public Vector3 direction; // 입력 방향 벡터

    [Header("공격 관련")]
    public float power = 4f; // 공격시 앞으로 이동하는 값
    public GameObject slash; // 슬래시 이펙트 프리팹 
    public GameObject hit_lazer; // 히트 레이저 프리팹

    [Header("랜드 더스트")]
    public GameObject landDust; // 이동 시 발생하는 먼지 프리팹

    [Header("벽 점프 관련")]
    public Transform wallChk; // 벽 체크
    public float wallchkDistance; // 벽을 체크하는 거리
    public LayerMask wLayer; // 벽 감지 레이어
    bool isWall; // 벽 감지 여부
    public float slidingSpeed; // 벽에 붙어 있을때 미끄러 지는 속도 
    public float wallJumpPower; // 벽 점프시 가하는 힘
    public bool isWallJump; // 벽 점프 중인지 판단 여부
    float isRight = 1; // 플레이어가 바라보는 방향 (오른쪽 1, 왼쪽 -1) 

    [Header("점프 공격 각도")]
    public float jumpAngleThreshold = 10f; // 점프 공격이 판정되는 각도

    [Header("패링 설정")]
    public float parryWindow = 0.2f;  // 패링 가능 시간 

    [Header("사운드 설정")]
    public AudioSource audioSource;
    public AudioClip slashSound; // 슬래쉬 효과음
    public AudioClip parrySound; // 패링 효과음
    public AudioClip deathSound; // 캐릭터 사망 효과음

    [Header("슬로우 모드 색상")]
    private Color originalColor; // 기본 스프라이트 컬러
    public Color slowColor = Color.yellow; // 슬로우 모드 변경되는 스프라이트 컬러

    private bool isDead = false; // 캐릭터 사망 여부
    public bool IsDead => isDead; // 외부 접근용 프로퍼티

    public static event Action OnPlayerDeath; // 플레이어 사망 이벤트

    // 참조할 컴포넌트 
    private Animator pAnimator; 
    private Rigidbody2D pRig2D;
    private SpriteRenderer sp;

    private bool isParryActive = false; // 패링 판단 여부
    private float parryStartTime = 0f; // 패링 시작 시간

    void Awake()
    {
        // 씬 전환시 파괴 방지
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // 주요 컴포넌트 초기화 
        pAnimator = GetComponent<Animator>();
        pRig2D = GetComponent<Rigidbody2D>();
        direction = Vector2.zero;
        sp = GetComponent<SpriteRenderer>();
        originalColor = sp.color;

        // 오디오 소스 자동 추가
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // 캐릭터가 사망 상태면 업데이트를 무시  
        if (isDead)
        {
            return; // 사망 시 함수 종료 
        }

        AttemptParry(); // 패링 판정 함수

        if (!isWallJump) // 벽 점프 중이 아니면
        {
            KeyInput(); // 입력 처리 
            Move(); // 이동 처리
        }

        // 벽 감지 레이캐스트) 
        isWall = Physics2D.Raycast(wallChk.position, Vector2.right * isRight, wallchkDistance, wLayer);
        pAnimator.SetBool("Grab", isWall); // 벽 점프시 잡기 애니메이션 함수

        // 점프 입력 처리
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!pAnimator.GetBool("Jump"))
            {
                Jump();
                pAnimator.SetBool("Jump", true);
            }
        }

        // 벽 점프 처리
        if (isWall)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                isWallJump = true;
                Invoke("FrezzeX", 0.3f); //  벽 점프 중일 때 0.3초 동안 키 입력을 막음  
                pRig2D.velocity = new Vector2(-isRight * wallJumpPower, 0.9f * wallJumpPower); // 벽을 기준으로 반대 방향으로 점프
                sp.flipX = !sp.flipX; // 벽 점프시 캐릭터 방향 반전
                isRight = -isRight; // 캐릭터가 바라보는 방향 값 
            }
        }

        // 슬로우 모드 시 색상 변경 
        if (TimeManager.Instance != null)
        {
            if (TimeManager.Instance.IsSlowing())
            {
                sp.color = slowColor;
            }
            else
            {
                sp.color = originalColor;
            }
        }
    }

    void FixedUpdate()
    {
        // 캐릭터가 사망 상태일때 물리연산 무시
        if (isDead)
        {
            return; // 사망 시 함수 종료 
        }

        // 바닥 감지 레이어
        RaycastHit2D rayHit = Physics2D.Raycast(pRig2D.position, Vector3.down, 1, LayerMask.GetMask("Ground"));

        // 하강 중일 때 땅인지 판정
        if (pRig2D.velocity.y < 0)
        {
            // 바닥과 충돌한 경우, 그리고 일정 거리(0.7f) 이내라면 '착지'로 판단
            if (rayHit.collider != null && rayHit.distance < 0.7f)
            {
                pAnimator.SetBool("Jump", false); // 점프 상태 해제
            }
            // 그게 아니라면
            else
            {
                pAnimator.SetBool("Jump", !isWall); // 벽이 아니면 점프
                pAnimator.SetBool("Grab", isWall); // 벽이라면 그랩
            }
        }
    }

    void KeyInput()
    {
        // 캐릭터가 사망 상태라면 키 입력을 무시 
        if (isDead)
        {
            return; // 사망시 함수 종료
        }

        direction.x = Input.GetAxisRaw("Horizontal");

        // 좌측 이동
        if (direction.x < 0)
        {
            sp.flipX = true;
            pAnimator.SetBool("Run", true);
            isRight = -1;
        }

        // 우측 이동
        else if (direction.x > 0)
        {
            sp.flipX = false;
            pAnimator.SetBool("Run", true);
            isRight = 1;
        }

        // 입력이 없다면 Run 애니메이션 종료 Idle 상태로 복귀 
        else
        {
            pAnimator.SetBool("Run", false);
        }

        // 마우스 좌 클릭시 공격
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 공격시 마우스 화면 좌표에서 월드 좌표로 변환
            Vector3 toMouse = mouseWorldPos - transform.position; // 플레이어 위치에서 마우스 방향 벡터 계산

            if (toMouse.x < 0) // 마우스의 위치가 왼쪽에 있으면
            {
                // 왼쪽으로 플립
                sp.flipX = true;
                isRight = -1;
            }

            // 반대의 경우 
            else
            {
                // 오른쪽으로 플립
                sp.flipX = false;
                isRight = 1;
            }

            float angle = Vector2.Angle(Vector2.right * isRight, new Vector2(toMouse.x, toMouse.y)); // 공격 방향 각도 계산
            if (toMouse.y > 0) // 마우스가 플레이어 위쪽에 있으면 (점프 공격 가능 조건)
            {
                if (angle > jumpAngleThreshold && !pAnimator.GetBool("Jump")) // 각도가 점프 공격 기준 이상이고 아직 점프 중이 아니면
                {
                    // 점프 실행 
                    Jump();
                    pAnimator.SetBool("Jump", true);
                }
            }

            pAnimator.SetTrigger("Attack"); // 공격 애니메이션 실행

            // 히트 레이저 이펙트 및 사운드
            if (hit_lazer != null) // 히트 레이저 프리팹이 존재하면
            {
                // 플레이어의 위치에 히트 레이저 생성
                Instantiate(hit_lazer, transform.position, Quaternion.identity);
            }

            if (slashSound != null && audioSource != null)
            {
                // 공격시 효과음을 한번씩만 재생
                audioSource.PlayOneShot(slashSound);
            }

            isParryActive = true; // 공격 시 잠깐동안 패링 판정
            parryStartTime = Time.time; // 패링이 가능한 시간 판정
        }

        // 왼쪽 쉬프트 키를 누르면 타임매니저에 의해 슬로우 모드 발동
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.ActivateSlowMotion();
            }
        }
    }

    void AttemptParry()
    {
        // 패링 상태가 아니라면
        if (!isParryActive)
        {
            return;
        }

        // 패링 가능 시간이 지났다면 비활성화 
        if (Time.time - parryStartTime > parryWindow)
        {
            isParryActive = false;
            return;
        }

        // 현재 씬에 존재하는 모든 쿠나이 오브젝트 탐색
        Kunai[] kunais = FindObjectsOfType<Kunai>();
        foreach (var k in kunais)
        {
            // 플레이어와 쿠나이 사이의 거리 계산
            float distance = Vector2.Distance(k.transform.position, transform.position);
            
            // 거리가 2 이상이면 패링 범위 밖으로 판단 
            if (distance > 2f)
            {
                continue;
            }

            // 쿠나이가 플레이어가 바라보는 방향에 있는지 판단
            Vector2 toKunai = (k.transform.position - transform.position).normalized; // 플레이어 위치에서 쿠나이 위치로 향하는 벡터
            bool facingKunai = (sp.flipX && toKunai.x < 0) || (!sp.flipX && toKunai.x > 0); // 좌,우 바라보는 방향에 따라 패링 성공 여부 판정

            if (facingKunai) // 쿠나이가 플레이어가 바라보는 방향에 있으면
            {
                k.Reflect(); //쿠나이 반환

                if (landDust != null)
                {
                    LandDust(landDust); // 쿠나이 패링시 발밑 먼지 이펙트 생성
                }

                if (parrySound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(parrySound); // 패링 성공시 한번씩만 효과음 재생
                }

                isParryActive = false; // 패링판정 종료 
                return; 
            }
        }
    }

    // 벽 점프 시 X축의 이동을 잠시 막음
    void FrezzeX() 
    {
        isWallJump = false;
        Invoke("WallJump", 0.1f);
    }

    // 벽 점프 후 착지 판정
    void WallJump()
    {
        // 레이캐스트로 바닥 감지
        RaycastHit2D groundCheck = Physics2D.Raycast(pRig2D.position, Vector2.down, 1f, LayerMask.GetMask("Ground"));
        if (groundCheck.collider != null)
        {
            // 일정 거리 이내면 착지로 판단
            if (groundCheck.distance < 0.7f)
            {
                pAnimator.SetBool("Jump", false); // 점프 상태 해제
                pAnimator.SetBool("Grab", false); // 벽 잡기 해제 
            }
        }
    }

    // 점프 함수
    public void Jump()
    {
        // 캐릭터 사망시 점프 무시 
        if (isDead)
        {
            return;
        }

        pRig2D.velocity = Vector3.zero; // 기존 속도 초기화
        pRig2D.AddForce(new Vector2(0, jumpUp), ForceMode2D.Impulse);
    }

    // 이동 함수
    public void Move()
    {
        // 캐릭터 사망 시 이동 무시 
        if (isDead)
        {
            return;
        }

        transform.position += direction * speed * Time.deltaTime;
    }

    // 땅 먼지 생성
    public void LandDust(GameObject dust)
    {
        // 캐릭터의 발밑 기준점으로 먼지를 생성
        Instantiate(dust, transform.position + new Vector3(0, -0.43f, 0), Quaternion.identity);
    }

    // 슬래쉬 이펙트 애니메이션 함수
    public void AttSlash()
    {
        // 캐릭터 사망시 공격 무시 
        if (isDead)
        {
            return;
        }

        // 캐릭터가 바라보는 방향으로  공격시 앞으로 가는 힘을 가함
        Vector2 force = sp.flipX ? Vector2.left * power : Vector2.right * power;
        pRig2D.AddForce(force, ForceMode2D.Impulse);

        if (slash != null) // 슬래쉬 이펙트 프리팹이 존재하면
        {
            Instantiate(slash, transform.position, Quaternion.identity); // 슬래쉬 이펙트 생성
        }
    }

    // 피격 처리
    public void Hit()
    {   
        // 사망 시 피격 무시 
        if (isDead)
        {
            return;
        }

        Die(); // 사망 처리
    }

    // 사망 처리
    private void Die()
    {
        // 사망 중복 처리 방지
        if (isDead)
        {
            return; 
        }

        isDead = true;
        pAnimator.SetTrigger("Dead"); // 사망 애니메이션 재생

        if (deathSound != null)
        {
            GameObject audioObj = new GameObject("DeathSound"); // 사운드 전용 임시 오브젝트 생성
            AudioSource tempSource = audioObj.AddComponent<AudioSource>();
            tempSource.clip = deathSound;
            tempSource.Play();
            Destroy(audioObj, deathSound.length); // 사운드의 길이 만큼 재생 후 삭제 
        }

        direction = Vector3.zero; // 이동 정지
        pRig2D.velocity = Vector2.zero; // 물리 속도 초기화

        // 캐릭터 사망 시 Bgm은 끄지 않음 (사운드 매니저와 연동)
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.KeepBGMPlaying();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnPlayerDied();
        }
        
        // 외부 이벤트 호출 
        OnPlayerDeath?.Invoke();

        // 게임 클리어시 플레이어 컨트롤러 비활성화 (UI 매니저와 연동)
        this.enabled = false;
    }

    // 벽 점프 확인용 기즈모
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (wallChk != null)
        {
            Gizmos.DrawRay(wallChk.position, Vector2.right * isRight * wallchkDistance);
        }
    }
}
