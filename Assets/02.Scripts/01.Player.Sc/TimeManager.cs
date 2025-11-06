using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("슬로우모드 설정")]
    public float slowTimeScale = 0.45f; // 슬로우 모드 시간 비율 
    public float slowDuration = 1.5f; // 슬로우 모드 유지 시간 

    public bool isSlowing = false; // 슬로우 모드 활성 여부
    public float originalTimeScale = 1f; // 원래의 시간 
    public float timer = 0f; // 슬로우 모드 경과 시간 

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (isSlowing)
        {
            // 실제 시간을 기준으로 경과시간 누적
            timer += Time.unscaledDeltaTime; 

            if (timer >= slowDuration) 
            {
                ResumeTime(); // 슬로우 모드가 끝나면 원래 속도로 복구
            }
               
        }
    }

    public void ActivateSlowMotion()
    {
        // 슬로우 모드가 활성화 되어 있으면 무시 
        if (isSlowing) 
        {
            return;
        } 
            

        isSlowing = true;
        originalTimeScale = Time.timeScale; // 원래의 속도 
        Time.timeScale = slowTimeScale; // 게임 속도를 느리게 적용 
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // 물리연산 속도도 맞춰서 느리게 
        timer = 0f; // 슬로우 모드 경과 시간 초기화 
    }

    public void ResumeTime()
    {
        isSlowing = false; // 슬로우 모드 끔 
        Time.timeScale = originalTimeScale; // 원래 속도로 초기화 
        Time.fixedDeltaTime = 0.02f; //물리 연산 속도 초기화 
    }

    // 슬로우 모드 상태 반환 
    public bool IsSlowing() => isSlowing;
}
