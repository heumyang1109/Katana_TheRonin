using UnityEngine;

public class Portal : MonoBehaviour
{
    public string targetSceneName;       // 이동할 씬 이름
    private bool allEnemiesDead = false; // 적이 전부 죽었는지 체크

    private void Update()
    {
        // "Enemy" 태그 가진 오브젝트를 모두 찾음
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        allEnemiesDead = enemies.Length == 0; // 적이 하나도 없으면 true
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어 태그가 아니면 무시
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        // 이동할 씬 이름이 없으면 무시
        if (string.IsNullOrEmpty(targetSceneName))
        {
            return;
        }

        // 적이 전부 죽었으면 씬 전환
        if (allEnemiesDead)
        {
            GameSceneManager sceneManager = FindObjectOfType<GameSceneManager>();

            if (sceneManager != null)
            {
                sceneManager.LoadScene(targetSceneName); // 지정한 씬으로 이동
            }
        }
    }
}

