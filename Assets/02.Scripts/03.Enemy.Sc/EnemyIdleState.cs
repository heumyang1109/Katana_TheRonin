using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.animator.SetBool("Run", false);
    }

    public override void Update()
    {
        // 죽으면 대기 상태를 멈춤
        if (enemy.IsDead) 
        {
            return; 
        } 

        if (enemy.IsPlayerInRange())
        {
            // 플레이어의 위치가  적의 위치보다 높다면
            float heightDiff = enemy.player.position.y - enemy.transform.position.y;
            if (heightDiff > enemy.heightThreshold)
            {
                enemy.LookAtPlayer(); // 플레이어 바라보기 
                return;
            }

            enemy.ChangeState(enemy.runState); // 플레어의 위치가 적의 위치와 같다면 달리기 상태로 
        }
    }
}
