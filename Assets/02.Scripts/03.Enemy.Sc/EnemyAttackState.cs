using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private bool hasHitThisAttack = false; // 한 공격 시점에 플레이어가 이미 맞았는지 여부

    public EnemyAttackState(EnemyController enemy) : base(enemy) { }

    // 공격 상태 시작 호출
    public override void Enter()
    {
        hasHitThisAttack = false; // 공격 판정 초기화 
        enemy.animator.SetTrigger("Attack"); // 공격 애니메이션 실행
        enemy.StopMoving(); // 공격시엔 이동하지 않음 
    }

    public override void Update()
    {
        if (enemy.IsDead)
        {
            return;
        }

        // 플레이어가 공격 범위 밖으로 나가면 추적 상태로 변환 
        if (Vector2.Distance(enemy.transform.position, enemy.player.position) > enemy.attackRange)
        {
            enemy.ChangeState(enemy.runState);
        }
    }

    // 공격애니메이션에서 호출되는 이벤트 
    public void HandleAttackHitFromAnimation()
    {
        if (hasHitThisAttack) 
        {
            return;
        } 
        TryHitPlayer(); // 공격 범위 안에 플레이어가 있다면 피해 적용 
        hasHitThisAttack = true; // 공격 판정 완료 
    }

    private void TryHitPlayer()
    {
        // 플레이어가 없다면 상태 종료
        if (enemy.player == null) 
        {
            return;
        } 

        //적과 플레이어 사이의 거리 계산 
        float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);

        // 플레이어가 적의 공격 범위 안이면 
        if (distance <= enemy.attackRange)
        {
            // 플레이어 컨트롤러에서 Hit 함수를 호출
            PlayerController pc = enemy.player.GetComponent<PlayerController>();
            pc?.Hit();
        }
    }

    // 공격 상태 종료시 호출
    public override void Exit()
    {
        hasHitThisAttack = false; // 공격 판정 초기화 
    }
}
