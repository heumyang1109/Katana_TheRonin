using UnityEngine;

public class Enemy_MonkDeadState : Enemy_MonkState
{
    public Enemy_MonkDeadState(Enemy_MonkController enemy) : base(enemy) { }

    public override void Enter()
    {
        // Dead 상태 진입 시 사망 애니메이션 트리거
        enemy.animator.SetTrigger("Dead");

        // 적의 모든 코루틴 중지 
        enemy.StopAllCoroutines();

        // Rigidbody2D, Collider2D 참조
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        Collider2D col = enemy.GetComponent<Collider2D>();

        if (rb != null)
        {
            // 물리 연산 비활성화
            rb.simulated = false;
        }

        if (col != null)
        {
            // 충돌 비활성화 
            col.enabled = false; // Collider 비활성화
        }

        // 0.5초 후 게임 오브젝트 제거
        Object.Destroy(enemy.gameObject, 0.5f);
    }
}


