using UnityEngine;

public class Hit_Lazer : MonoBehaviour
{
    Transform tr; // 플레이어 참조
    Vector2 MousePos; // 마우스 위치
    float Speed = 50f; // 레이저 이동 속도
    Vector3 dir; // 플레이어에서 마우스의 방향 벡터 

    float angle; // 회전 각도
    Vector3 dirNo; // 방향 벡터 정규화 

    void Start()
    {
        // 플레이어의 트랜스 폼 참조 
        tr = GameObject.Find("Player").GetComponent<Transform>();

        // 마우스의 화면 좌표를 월드 좌표로 변환
        MousePos = Input.mousePosition;
        MousePos = Camera.main.ScreenToWorldPoint(MousePos);
        Vector3 Pos = new Vector3(MousePos.x, MousePos.y, 0);

        // 플레이어의 위치에서 마우스의 위치 방향 벡터 계산
        dir = Pos - tr.position;

        // 방향 벡터를 각도로 변환
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 노멀 라이즈 단위 벡터
        dirNo = new Vector3(dir.x, dir.y, 0).normalized;

        // 4초후 히트레이저 삭제 
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        // 플레이어의 마우스 방향으로 레이저 회전
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 레이저의 위치, 방향 * 속도 * 시간 만큼 이동  
        transform.position += dirNo * Speed * Time.deltaTime;
    }
}
