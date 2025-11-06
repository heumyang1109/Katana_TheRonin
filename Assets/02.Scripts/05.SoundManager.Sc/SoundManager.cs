using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance; 

    [Header("BGM Clips")]
    public AudioClip mainSceneBGM;   // 메인씬에서 재생할 Bgm
    public AudioClip mainScene2BGM;  // 메인씬2에서 재생할 Bgm

    private AudioSource audioSource; // Bgm 재생용 오디오 소스 
    private Coroutine fadeCoroutine; // 페이드 처리용 코루틴

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환시에도 사운드 매니저 유지
        }
        else
        {
            Destroy(gameObject); // 중복된 사운드 매니저  제거
            return;
        }

        // AudioSource 세팅
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;          // 루프 재생
        audioSource.playOnAwake = false;  // Awake 시 자동 재생 금지
        audioSource.volume = 1f;          // 초기 볼륨

        // 씬 로드 시 자동으로 Bgm 이 재생되도록 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드 후 해당 씬의 Bgm 재생
        PlayBGMForScene(scene.name);
    }

    // 현재 씬을 기준으로 Bgm 재생
    public void PlayBGM()
    {
        PlayBGMForScene(SceneManager.GetActiveScene().name);
    }

    // 씬 이름에 따른 Bgm을 재생 
    private void PlayBGMForScene(string sceneName)
    {
        AudioClip newClip = null;

        // 씬 이름에 따른 Bgm 할당
        switch (sceneName)
        {
            case "MainScene":
                {
                    newClip = mainSceneBGM; // 메인 씬 에서는 메인 씬 Bgm 재생
                    break;
                }
            case "MainScene2":
                {
                    newClip = mainScene2BGM; // 메인 씬2에서는 메인 씬2 Bgm 재생
                    break;
                }
            default:
                {
                    newClip = null; // 지정된 씬이 없으면 Bgm 재생하지 않음
                    break;
                }
        }


        // 새 Bgm이 현재 Bgm과 같거나 null이 아닌 경우 재생
        if (newClip != null && audioSource.clip != newClip)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine); // 이전 페이드 코루틴 중지
            }

            fadeCoroutine = StartCoroutine(FadeToNewBGM(newClip, 1f)); // 1초페이드
        }
        else
        {
            // 동일한 BGM이면 재생 중이 아니면 바로 재생
            if (!audioSource.isPlaying && newClip != null)
            {
                audioSource.clip = newClip;
                audioSource.Play();
            }
        }
    }

    // 기존 Bgm에서 새로운 Bgm으로 페이드 전환
    private IEnumerator FadeToNewBGM(AudioClip newClip, float fadeDuration)
    {
        // 새 Bgm 이 재생중이면 현재 Bgm 감소 
        if (audioSource.isPlaying)
        {
            for (float t = 0f; t < fadeDuration; t += Time.unscaledDeltaTime)
            {
                audioSource.volume = Mathf.Lerp(1f, 0f, t / fadeDuration); // 볼륨 감소
                yield return null;
            }
        }

        // Bgm 교체 후 재생
        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        // 볼륨 서서히 증가
        for (float t = 0f; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = 1f; // 최종 볼륨 1로 고정
        fadeCoroutine = null;     // 코루틴 참조 초기화
    }

    // 게임 클리어 시 Bgm 종료 처리
    public void StopBGM(bool fadeOut = true, float fadeDuration = 1f)
    {
        if (audioSource == null || !audioSource.isPlaying)
        {
            return; // 재생 중이 아니면 무시
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine); // 기존 페이드 코루틴 중단
        }

        if (fadeOut)
        {
            fadeCoroutine = StartCoroutine(FadeOutAndStop(fadeDuration)); // 페이드 아웃
        }
        else
        {
            audioSource.Stop();  // 즉시 정지
            audioSource.volume = 1f; // 볼륨 초기화
        }
    }

    // 페이드 아웃 후 Bgm 정지
    private IEnumerator FadeOutAndStop(float fadeDuration)
    {
        float startVolume = audioSource.volume; // 현재 볼륨 저장

        for (float t = 0f; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration); // 볼륨 감소
            yield return null;
        }

        audioSource.Stop();      // 재생 종료
        audioSource.volume = 1f; // 볼륨 초기화
        fadeCoroutine = null;     // 코루틴 초기화
    }

    // 캐릭터 사망시 Bgm 유지 
    public void KeepBGMPlaying()
    {
        if (audioSource != null)
        {
            if (!audioSource.isPlaying && audioSource.clip != null)
            {
                audioSource.Play(); // 이미 재생 중이라면 무시
            }
        }
    }
}

