using UnityEngine;

public abstract class Enemy_MonkState
{
    protected Enemy_MonkController enemy;

    public Enemy_MonkState(Enemy_MonkController enemy) // Enemy_MonkController 참조 
    {
        this.enemy = enemy;
    }

    // 상태 진입 시 호출 
    public virtual void Enter() { }

    // 상태 유지 시 호출 
    public virtual void Update() { }

    // 상태 종료 시 호출 
    public virtual void Exit() { }
}
