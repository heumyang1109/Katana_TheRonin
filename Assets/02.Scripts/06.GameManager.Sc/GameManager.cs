using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 

    [Header("플레이어")]
    public PlayerController currentPlayer; // 현재 플레이어 컨트롤러 참조
    [HideInInspector] public Transform respawnPoint; // 씬마다 StartPoint 자동 참조, 플레이어 리스폰 기준점

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject); // 씬 전환에도 파괴되지 않음
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 이벤트 등록
        }
        else
        {
            Destroy(gameObject); // 중복된 GameManager 제거
        }
    }

    private void Update()
    {
        // R키 입력 시 플레이어와 적 모두 리스폰
        if (Input.GetKeyDown(KeyCode.R))
        {
            RespawnAll();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentPlayer == null)
        {
            // 씬 내 플레이어 컨트롤러를 가진 오브젝트 검색
            currentPlayer = FindObjectOfType<PlayerController>();
        }

        // 씬 내 StartPoint 객체를 찾아 respawnPoint 설정
        StartPoint sp = FindObjectOfType<StartPoint>();
        if (sp != null)
        {
            respawnPoint = sp.transform;
        }

        // 씬 로드 직후 플레이어와 적 초기 리스폰
        RespawnAll();
    }

    // 플레이어와 적 모두 리스폰
    public void RespawnAll()
    {
        if (currentPlayer == null)
        {
            currentPlayer = FindObjectOfType<PlayerController>(); 
        }

        if (respawnPoint == null)
        {
            StartPoint sp = FindObjectOfType<StartPoint>();
            if (sp != null)
            {
                respawnPoint = sp.transform; 
            }
        }

        ResetPlayer(); // 플레이어 초기화
        RespawnEnemies(); // 적 초기화
    }

    // 플레이어 상태 초기화
    private void ResetPlayer()
    {
        if (currentPlayer == null || respawnPoint == null)
        {
            return;
        }

        currentPlayer.gameObject.SetActive(true); // 플레이어 활성화
        currentPlayer.transform.position = respawnPoint.position; //  플레이어 리스폰 위치 초기화

        // private bool isDead 초기화
        typeof(PlayerController)
            .GetField("isDead", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(currentPlayer, false);

        // 애니메이션 초기화
        Animator anim = currentPlayer.GetComponent<Animator>();
        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }
    }

    // 씬 내 모든 에너미 스포너 초기화 후 적 리스폰
    private void RespawnEnemies()
    {
        EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>(); // 씬에 있는 모든 스포너 검색
        foreach (var spawner in spawners)
        {
            spawner.Clear(); // 기존 적 제거

            if (currentPlayer != null)
            {
                spawner.Spawn(currentPlayer.transform); // 새 적 생성
            }
        }
    }

    // 게임 클리어 판정
    public void CheckGameClear()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // 메인 씬2 에서만 클리어 판정
        if (currentScene != "MainScene2")
        {
            return;
        }

        // 씬 내 모든 에너미 스포너  확인
        EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();
        if (spawners == null || spawners.Length == 0)
        {
            return;
        }

        //  씬에 살아 있는 적 수 카운트
        int aliveCount = 0;

        foreach (var spawner in spawners)
        {
            // 스포너에 적이 없으면 다음 스포너로 넘어감
            if (spawner.spawnedEnemy == null)
            {
                continue;
            }

            // 일반 적과 원거리적 (Monk) 구분
            EnemyController ec = spawner.spawnedEnemy.GetComponent<EnemyController>();
            Enemy_MonkController monk = spawner.spawnedEnemy.GetComponent<Enemy_MonkController>();

            // 일반 적이 존재하고 살아있으면 aliveCount 증가
            if (ec != null && !ec.IsDead)
            {
                aliveCount++;
            }

            // 원거리 적 (Monk)이 존재하고 살아있으면 aliveCount 증가
            else if (monk != null && !monk.IsDead)
            {
                aliveCount++;
            }
        }

        // 살아 있는 적이 없으면 게임 클리어 처리
        if (aliveCount == 0)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OnGameClear(); // UI에 게임 클리어 표시
            }
        }
    }
}

