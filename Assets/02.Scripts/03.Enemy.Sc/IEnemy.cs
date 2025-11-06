public interface IEnemy
{
    bool IsDead { get; } // 적의 사망 여부 
    void Die(); // 적 사망 처리 함수 
}
