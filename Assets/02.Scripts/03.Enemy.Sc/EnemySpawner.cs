using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;       // Inspector에서 스폰할 적 프리팹 지정
    [HideInInspector] public GameObject spawnedEnemy; // 현재 스폰된 적을 참조

    
    public void Spawn(Transform player)
    {
        // 프리팹이 있고, 아직 스폰된 적이 없으면
        if (enemyPrefab != null && spawnedEnemy == null)
        {
            // 적 생성, 현재 적이 스폰 된 위치
            spawnedEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation);

            // 생성된 적이 EnemyController를 가지고 있으면 플레이어 정보 연결
            EnemyController ec = spawnedEnemy.GetComponent<EnemyController>();
            if (ec != null)
            {
                ec.player = player;
            }

            // 생성된 적이 Enemy_MonkController를 가지고 있으면 플레이어 정보 연결
            Enemy_MonkController monk = spawnedEnemy.GetComponent<Enemy_MonkController>();
            if (monk != null)
            {
                monk.player = player;
            }
        }
    }

    // 현재 스폰된 적 제거 함수
    public void Clear()
    {
        if (spawnedEnemy != null)
        {
            Destroy(spawnedEnemy); // 적 오브젝트 삭제
            spawnedEnemy = null;    // 참조 및 초기화
        }
    }
}
