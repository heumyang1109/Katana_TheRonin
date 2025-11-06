using UnityEngine;

public class StartPoint : MonoBehaviour
{
    
    private void Awake()
    {
        if (GameManager.Instance != null) // 게임매니저가 존재하면
        {
            GameManager.Instance.respawnPoint = transform; // 플레이어가 리스폰 될때 이 위치를 기준으로 사용함.
        }
    }
}

