using UnityEngine;

public class Stairs : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    private Animator animator;

    public float slopeThreshold = 0.1f; // 경사 판단 기준

    private void Awake()
    {
        // 씬 시작 시 Player 참조
        FindPlayer();
    }

    private void OnEnable()
    {
        // 씬 전환 시에도 Player 재 참조
        FindPlayer();
    }

    private void FindPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                rb = player.GetComponent<Rigidbody2D>();
                animator = player.GetComponent<Animator>();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {    
        // 플레이어의 참조가 없다면 
        if (player == null) 
        {
            FindPlayer();  // 플레이어 찾기

            // 찾았는데도 없다면 함수 종료
            if (player == null)  
            {
                return;
            } 
        }

        // 충돌한 오브젝트가 플레이어라면 
        if (collision.gameObject == player)
        {
            bool isOnSlope = false;

            // 충돌 지점의 노멀 값으로 경사 판단 
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (Mathf.Abs(contact.normal.x) > slopeThreshold) // 경사의 기준을 초과하면 
                {
                    isOnSlope = true; // 경사로 판정
                    break;
                }
            }

            // 경사 위면 
            if (isOnSlope) 
            {
                //  플레이어의 리지드 바디의 프리즈 x 를 활성화  (미끄러짐 방지) 
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                rb.gravityScale = 0; // 경사 위일때 플레이어가 올라가기 쉽도록 중력제거 

                if (animator != null)
                {
                    animator.SetBool("Jump", false);
                    animator.SetBool("Run", false);
                }
            }

            // 경사가 아니면
            else
            {
                rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 플레이어의 리지드 바디의 프리즈 x를 끔 
                rb.gravityScale = 1; // 중력을 1로 되돌림 
            }
        }
    }

    // 플레이어가 경사를 벗어났을때 호출
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (player == null)
        {
            FindPlayer();
            if (player == null) return;
        }

        if (collision.gameObject == player)
        {
            // X축 고정 해제, 중력 복원
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 1;
        }
    }
}
