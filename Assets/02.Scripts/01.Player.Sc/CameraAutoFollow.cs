using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CameraAutoFollow : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCam; // 시네머신 카메라 참조 

    void Awake()
    {
        virtualCam = GetComponent<CinemachineVirtualCamera>(); // 가상 카메라 컴포넌트 가져오기
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 시 플레이어 재할당 
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 이벤트 해제 
    }

    void Start()
    {
        AssignPlayer(); // 씬 전환 후 플레이어 할당 
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignPlayer(); // 게임 시작시 플레이어 할당 
    }

    private void AssignPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player"); // 플레이어 태그를 지닌 오브젝트 찾기
        if (player != null)
        {
            virtualCam.Follow = player.transform;  // 카메라가 플레이어를 따라가도록 지정 
        }
    }
}
