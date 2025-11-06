using UnityEngine;

public class Kunai : MonoBehaviour
{
    [Header("투사체 속성")]
    public float speed = 9f;         // 쿠나이 이동 속도
    public float lifetime = 0.5f;    // 쿠나이 존재 시간 (0.5초 후 자동 삭제)

    private Vector2 direction;       // 쿠나이가 이동할 방향 벡터 
    private bool isReflected = false; // 쿠나이가 반사되었는지 여부

    [Header("참조")]
    public Enemy_MonkController owner; // 쿠나이를 던진 원래 적 (Monk) 를 참조  

    private void Start()
    {
        // lifetime 의 시간 후 쿠나이 자동 삭제
        Destroy(gameObject, lifetime);
    }

    // 쿠나이 이동 방향과 회전 설정
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized; // 방향 벡터 정규화
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // 벡터를 각도로 변환
        transform.rotation = Quaternion.Euler(0, 0, angle); // 회전 적용
    }

    // 쿠나이를 던진 원래의 적을 참조 
    public void SetOwner(Enemy_MonkController enemy)
    {
        owner = enemy;
    }

    // 플레이어가 쿠나이를 반사했을때
    public void Reflect()
    {
        if (owner != null)
        {
            // 적의 위치를 향해 방향 전환
            Vector2 reflectDir = ((Vector2)owner.transform.position - (Vector2)transform.position).normalized;
            direction = reflectDir;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle); // 회전 적용

            isReflected = true; // 반사 상태로 표시

            //  반사 후 쿠나이을 던진 원래의 적이 죽지 않았다면 
            if (!owner.IsDead)
            {
                owner.Die(); //  적 죽음 
            }
        }
    }

    private void Update()
    {
    
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 쿠나을 반사 하지 못한 경우
        if (collision.CompareTag("Player") && !isReflected)
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Hit(); // 플레이어 피해 처리
            }
            Destroy(gameObject); // 쿠나이 삭제
        }

        // 쿠나이가 적에게 반사된 경우
        if (collision.CompareTag("Enemy") && isReflected)
        {
            Enemy_MonkController enemy = collision.GetComponent<Enemy_MonkController>();
            if (enemy != null && !enemy.IsDead)
            {
                enemy.Die(); // 적 제거
            }
            Destroy(gameObject); // 쿠나이 삭제
        }

        // 지면 또는 장애물과 충돌 시
        if (collision.CompareTag("Ground") || collision.CompareTag("Obstacle"))
        {
            Destroy(gameObject); // 쿠나이 삭제
        }
    }
}
