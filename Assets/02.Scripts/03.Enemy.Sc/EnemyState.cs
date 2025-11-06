using UnityEngine;

public abstract class EnemyState
{
    protected EnemyController enemy; // 상태가 적용 될 컨트롤러 참조  

    public EnemyState(EnemyController enemy)
    {
        this.enemy = enemy; // 적 건트롤러 연결 
    }

    // 상태 진입 시 호출 
    public virtual void Enter() { }

    // 상태 유지 시 호출 
    public virtual void Update() { }

    // 상태 종료 시 호출 
    public virtual void Exit() { }
}
