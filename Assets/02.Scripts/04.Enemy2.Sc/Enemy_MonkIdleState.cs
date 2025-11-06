using UnityEngine;

public class Enemy_MonkIdleState : Enemy_MonkState
{
    public Enemy_MonkIdleState(Enemy_MonkController enemy) : base(enemy) { }

    public override void Enter()
    {
        // Idle 상태로 진입 시 달리기 애니메이션 종료
        enemy.animator.SetBool("Run", false);
    }

    public override void Update()
    {
        if (enemy.IsDead)
        {
            // 적이 이미 사망했으면 아무 행동도 하지 않음
            return;
        }

        if (enemy.player == null)
        {
            // 플레이어가 존재하지 않으면 Idle 상태 유지
            return;
        }

        // 플레이어와 적 사이의 거리 계산
        float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);

        // 플레이어가 탐지 범위 내에 들어오면 
        if (distance <= enemy.detectRange)
        {
            // 플레이어와 적 사이의  높이 차이 계산
            float yDiff = Mathf.Abs(enemy.player.position.y - enemy.transform.position.y);

            // 높이 차이가 허용 범위를 벗어났다면 
            if (yDiff >= enemy.heightThreshold)
            {
                enemy.LookAtPlayer(); // 플레이어 방향으로 바라보기
                return; // 이동하지 않고 Idle 유지
            }

            // 플레이어가 탐지 범위 내에 있고 높이 차이가 허용 범위 내이면 Run 상태로 전환
            enemy.ChangeState(enemy.runState);
        }
    }
}

