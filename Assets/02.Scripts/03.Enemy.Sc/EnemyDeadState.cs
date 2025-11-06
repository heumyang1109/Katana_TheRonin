public class EnemyDeadState : EnemyState
{
    public EnemyDeadState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.animator.SetTrigger("Dead"); // 사망 애니메이션 재생 
        enemy.StopMoving(); // 적 움직임 멈춤
        UnityEngine.Object.Destroy(enemy.gameObject, 0.5f); // 0.5초 뒤 삭제 
    }
}
