using UnityEngine;
public class Enemy_MonkAttackState : Enemy_MonkState
{
    public float attackCooldown = 0.25f; // 공격 대기 시간
    public float initialDelay = 0.15f; // 공격 상태 진입 후 첫 공격까지의 지연 시간
    private float stateEnterTime;  // 상태에 들어온 시간 기록
    private float lastAttackTime;  // 마지막 공격이 발생한 시간
    private bool firstAttackDone = false; // 첫 공격 여부 체크

    public Enemy_MonkAttackState(Enemy_MonkController enemy) : base(enemy) { }

    // 상태 진입 시 초기화
    public override void Enter()
    {
        stateEnterTime = Time.time;  // 현재 공격한 시간을 기록
        lastAttackTime = 0f;  // 마지막 공격 시간을 초기화
        firstAttackDone = false;  // 첫 공격 완료 여부 초기화
    }

    public override void Update()
    {
        if (enemy.IsDead) // 적이 죽었다면 공격 상태 반환 
        {
            return;
        }

        if (enemy.player == null) // 플레이어가 없다면 공격 상태 반환
        {
            return;
        }

        // 적과 플레이어 사이의 거리 계산
        float distance = Vector2.Distance(enemy.transform.position, enemy.player.position);

        // 플레이어를 바라보게 함
        enemy.LookAtPlayer();

        // 공격 범위를 벗어나면 이동 상태로 전환
        if (distance > enemy.attackRange)
        {
            enemy.ChangeState(enemy.runState);
            return;
        }

        //  첫 공격이 끝나고 다음 공격을 하기 위한 처리 
        if (!firstAttackDone && Time.time - stateEnterTime >= initialDelay)
        {
            enemy.animator.SetTrigger("Attack"); 
            enemy.FireKunai(); 
            firstAttackDone = true;  
            lastAttackTime = Time.time; // 마지막 공격 시간 갱신
        }
        // 쿨 다임 이후 연속 공격 처리 
        else if (firstAttackDone && Time.time - lastAttackTime >= attackCooldown)
        {
            enemy.animator.SetTrigger("Attack"); 
            enemy.FireKunai();                    
            lastAttackTime = Time.time; // 마지막 공격 시간 갱신
        }
    }
}
