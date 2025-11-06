using System.Collections;
using UnityEngine;

public class Slash : MonoBehaviour
{
    private GameObject player; // 플레이어를 참조
    private Vector3 dir; // 플레이어에서 마우스쪽으로 가는 방향 벡터
    private float angle; 

    void Start()
    {
        // 씬에서 플레이어 태그를 지닌 오브젝트를 찾음
        player = GameObject.FindGameObjectWithTag("Player");

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 마우스의 화면 좌표를 월드 좌표로 변환
        mousePos.z = 0; // 2D 이므로 마우스의 z축은 0으로 고정
        dir = mousePos - player.transform.position; // 플레이어의 위치에서 마우스의 위치를 뺀 방향 벡터

        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // atan2로 방향 벡터 각도 계산, 라디안 각도 단위 변환
    }

    void Update()
    {
        transform.position = player.transform.position; // 항상 플레이어의 위치와 동일하게 유지
        transform.rotation = Quaternion.Euler(0f, 0f, angle); // 공격 방향으로 회전 
    }

    public void Des()
    {
        Destroy(gameObject); // AttSlash 함수의 애니메이션 이벤트 길이에 맞춰서 삭제
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IEnemy enemy = collision.GetComponent<IEnemy>(); //충돌한 오브젝트가 IEnemy를 상속받은 오브젝트인지 확인
        
        // 충돌한 오브젝트가 IEnemy 를 상속받은 적이고 아직 죽지 않았다면
        if (enemy != null && !enemy.IsDead)
        {
            enemy.Die(); // 적의 Die 함수 호출

            // 적의 사망 애니메이션이 먼저 나오도록 딜레이를 주고 0.5초 뒤 적 오브젝트 삭제 
            StartCoroutine(DestroyEnemyAfterDelay(collision.gameObject, 0.5f)); 
        }
    }

    private IEnumerator DestroyEnemyAfterDelay(GameObject enemyObj, float delay)
    {
        // 딜레이의 시간 만큼 대기
        yield return new WaitForSeconds(delay);

        // 적 오브젝트가 존재 하지 않는다면
        if (enemyObj != null) 
        {
            Destroy(enemyObj); // 적 오브젝트 제거 
        }
            
    }
}
