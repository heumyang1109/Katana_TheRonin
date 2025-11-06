
using UnityEngine;

public class LandDust : MonoBehaviour
{
    // 땅먼지 유지시간 
    public float lifetime = 0.5f;

    private void Awake()
    {
        // 라이프 타임의 초 만큼 기다렸다 삭제
        Destroy(gameObject,lifetime);
    }
    
}
