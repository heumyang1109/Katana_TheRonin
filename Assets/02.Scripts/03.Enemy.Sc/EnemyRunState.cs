using UnityEngine;

public class EnemyRunState : EnemyState
{
    public EnemyRunState(EnemyController enemy) : base(enemy) { }

    // 달리기 상태 
    public override void Enter()
    {
        enemy.animator.SetBool("Run", true); // 달리기 애니메이션 활성화
    }

    // 매 프레임마다 상태 체크
    public override void Update()
    {
        // 죽으면 달리기 상태를 멈춤 
        if (enemy.IsDead)
        {
            return; 
        }

        // 플레이어가 자신의 위치보다 높이 있다면, 플레이어의 위치쪽을 바라봄
        float heightDiff = enemy.player.position.y - enemy.transform.position.y;
        if (heightDiff > enemy.heightThreshold)
        {
            enemy.ChangeState(enemy.idleState); // 대기 상태로
            return;
        }

        //  감지 범위를 벗어나면 원래 자리로 돌아가기
        float distToPlayer = Vector2.Distance(enemy.player.position, enemy.spawnPoint);
        if (distToPlayer > enemy.returnDistance)
        {
            enemy.MoveTo(enemy.spawnPoint); // 스폰 지점으로 이동

            // 스폰 지점에 있을시 대기 상태로 전환
            if (Vector2.Distance(enemy.transform.position, enemy.spawnPoint) < 0.1f)
            {
                enemy.ChangeState(enemy.idleState);
            }
            return;
        }

        // 공격 범위 안에 들어오면 공격 상태로 전환
        if (Vector2.Distance(enemy.transform.position, enemy.player.position) < enemy.attackRange)
        {
            enemy.ChangeState(enemy.attackState);
            return;
        }

        // 그 외의 경우 플레이어를 추격
        enemy.MoveTo(enemy.player.position);
    }

    //  달리기 상태 종료
    public override void Exit()
    {
        enemy.animator.SetBool("Run", false); // 달리기 애니메이션 끔
    }
}
