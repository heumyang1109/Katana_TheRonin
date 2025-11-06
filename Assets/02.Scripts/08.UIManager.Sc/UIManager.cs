using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI 패널")]
    public GameObject mainMenuPanel; // Start, Exit 패널
    public GameObject inGameHUD;  // 인게임 HUD (슬로우모드 게이지)
    public GameObject youDiedPanel; // 사망 시 표시
    public GameObject gameClearPanel; // 게임 클리어 시 표시

    [Header("슬로우모드 UI")]
    public Slider slowModeSlider; // 슬로우모드 게이지

    private bool isGameClear = false; // 게임 클리어 여부 
    private bool isPlayerDead = false; // 플레이어 사망 여부 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환시에도 유지 
        }
        else
        {
            Destroy(gameObject); // 중복 방지 
        }
    }

    private void Start()
    {
        Time.timeScale = 1f; // 시간 초기화 

        // UI 초기 상태 설정 
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }

        if (inGameHUD != null)
        {
            inGameHUD.SetActive(false);
        }

        if (youDiedPanel != null)
        {
            youDiedPanel.SetActive(false);
        }

        if (gameClearPanel != null)
        {
            gameClearPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // Esc 키 입력시 게임 종료 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }

        UpdateSlowModeUI();

        //  플레이어 사망 후 R키로 리스폰
        if (isPlayerDead && Input.GetKeyDown(KeyCode.R))
        {
            if (GameManager.Instance != null)
            {
                youDiedPanel.SetActive(false);
                isPlayerDead = false;
                Time.timeScale = 1f;

                // 리스폰 후 게임 매니저에서 플레이어 제어 활성화 
                if (GameManager.Instance.currentPlayer != null)
                {
                    GameManager.Instance.currentPlayer.enabled = true;
                }

                // 모든 오브젝트 리스폰 
                GameManager.Instance.RespawnAll();

                // 리스폰 후 인게임 칸바스 재활성화 
                Transform canvas = transform.Find("InGameCanvas");
                if (canvas != null)
                {
                    canvas.gameObject.SetActive(true);
                }
            }
        }

        // 게임  클리어 후 Esc로 종료
        if (isGameClear && Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    // 게임 시작 버튼 
    public void StartGame()
    {
        if (mainMenuPanel != null)
        {
            // 메인메뉴 패널 활성화 
            mainMenuPanel.SetActive(false);
        }

        if (inGameHUD != null)
        {
            // 인게임 HUD  활성화 
            inGameHUD.SetActive(true);
        }

        Time.timeScale = 1f; // 시간 초기화

        // Bgm 재생 
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM();
        }
    }

    // 게임 종료 
    public void QuitGame()
    {
        Application.Quit();
    }
    
    // 슬로우 모드 UI 업데이트 
    private void UpdateSlowModeUI()
    {
        if (slowModeSlider == null || TimeManager.Instance == null)
        {
            return;
        }

        var tm = TimeManager.Instance;

        // 슬로우 모드가 활성화 되어 있는 경우 
        if (tm.IsSlowing())
        {
            // 슬로우 모드의 남은 비율을 계산 
            float remaining = Mathf.Clamp01(1f - (tm.timer / tm.slowDuration));
            
            // 비율을 슬라이더로 표시 
            slowModeSlider.value = remaining;
        }
        else
        {  
            // 슬로우 모드가 비활성화 상태면 다시 차오름
            slowModeSlider.value = Mathf.MoveTowards(slowModeSlider.value, 1f, Time.deltaTime * 0.5f);
        }
    }

    // 플레이어 사망 시 UI 및 상태처리 
    public void OnPlayerDied()
    {
        isPlayerDead = true;  // 플레이어 사망 여부 
        Time.timeScale = 0f; // 인게임 일시정지 

        // 플레이어가 사망 했다면 사망 패널 활성화 
        if (youDiedPanel != null)
        {
            youDiedPanel.SetActive(true);
        }

        // 사망시 인게임 HUD 는 숨김 
        if (inGameHUD != null)
        {
            inGameHUD.SetActive(false);
        }

      

        // 사망시 플레이어의 제어를  차단
        if (GameManager.Instance != null && GameManager.Instance.currentPlayer != null)
        {
            GameManager.Instance.currentPlayer.enabled = false;
        }
    }

    // 게임 클리어 UI 처리
    public void OnGameClear()
    {
        if (isGameClear)
        {
            return;
        }
        isGameClear = true;

        Time.timeScale = 0f; // 게임 클리어시 인 게임 일시정지 

        // 게임 클리어 UI 패널 활성화 
        if (gameClearPanel != null)
        {
            gameClearPanel.SetActive(true);
        }

        // 인게임 HUD 숨김 
        if (inGameHUD != null)
        {
            inGameHUD.SetActive(false);
        }

        // 게임 클리어시 Bgm을 멈춤 
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopBGM();
        }

        // 게임 클리어시 플레이어의 제어를 차단 
        if (GameManager.Instance != null && GameManager.Instance.currentPlayer != null)
        {
            GameManager.Instance.currentPlayer.enabled = false;
        }
    }
}
