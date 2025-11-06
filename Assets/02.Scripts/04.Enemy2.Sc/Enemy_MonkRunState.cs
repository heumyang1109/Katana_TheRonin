using UnityEngine;

public class Enemy_MonkRunState : Enemy_MonkState
{
    private bool isReturning = false; // 플레이어 탐지 범위를 벗어나면 시작 위치로 돌아감

    public Enemy_MonkRunState(Enemy_MonkController enemy) : base(enemy) { }

    public override void Enter()
    {
        // 적이 달리는 상태로 전환될 때 실행
        enemy.animator.SetBool("Run", true);
    }

    public override void Update()
    {
        if (enemy.IsDead) return; // 적이 이미 죽었다면 아무 행동도 하지 않음

        // 플레이어가 없거나 탐지 범위를 벗어나면 원 위치로 복귀 
        if (isReturning || enemy.player == null || Vector2.Distance(enemy.transform.position, enemy.player.position) > enemy.detectRange)
        {
            // 원위치로 이동
            enemy.MoveTo(enemy.startPos);

            // 원위치 방향으로 스프라이트 좌우 반전 처리
            Vector2 dir = enemy.startPos - (Vector2)enemy.transform.position; // 시작 위치까지 방향 계산
            Vector3 scale = enemy.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (dir.x > 0 ? 1 : -1); // 오른쪽이면 양수, 왼쪽이면 음수로 반전
            enemy.transform.localScale = scale;

            // 원위치에 거의 도착하면 복귀를 종료 후 Idle 상태로 전환
            if (Vector2.Distance(enemy.transform.position, enemy.startPos) < 0.1f)
            {
                isReturning = false; // 복귀 초기화
                enemy.ChangeState(enemy.idleState); // Idle 상태로 전환
            }

            return; 
        }

        // 플레이어가  범위 내에 존재하면 복귀 모드 종료
        isReturning = false;

        // 플레이어와 적 사이 거리 계산
        float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);
        
        // 높이 차이 계산
        float yDiff = Mathf.Abs(enemy.player.position.y - enemy.transform.position.y);

        // 플레이어 방향으로 바라보기 
        enemy.LookAtPlayer();

        // 플레이어가 공격 범위 안에 들어오면 공격 상태로 전환
        if (distance <= enemy.attackRange)
        {
            enemy.ChangeState(enemy.attackState);
        }

        // 플레이어와의 높이 차이가 허용 범위 안이면 이동
        else if (yDiff < enemy.heightThreshold)
        {
            enemy.MoveTo(enemy.player.position);
        }
    }

    // 게임 매니저에서 호출  적이 시작 위치로 돌아가도록 설정
    public void MoveToStart()
    {
        isReturning = true; // 복귀 활성화
    }

    public override void Exit()
    {
        // Run 상태 종료 시 호출
        enemy.animator.SetBool("Run", false);
    }
}
