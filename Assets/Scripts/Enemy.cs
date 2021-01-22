
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    private int currentHealth;
    public float speed;
    public float attackPlayerRange = 0.5f;

    
    private Animator enemyAnimator;
    public Transform attackR;
    public Transform attackL;
    public Transform playerRangePoint;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public Transform startingRangePoint;
    public PlayerController playerCS; // playerController script
    

    public LayerMask PlayerLayer;


    private Vector2 target;
    private Vector2 newPosition;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        enemyAnimator = GetComponent<Animator>();
        currentHealth = playerStats.PlayerHP;

    }

    void Update()
    {
        EnemyMovement();
        EnemyAttack();
        LookAtPlayer();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        enemyAnimator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    public void BasicAttack()
    {
        Collider2D[] hitPlayers;

        //Attack range
        if (!sr.flipX)
        {
            hitPlayers = Physics2D.OverlapCircleAll(attackR.position, playerStats.BasicAttackRange, PlayerLayer);
        }
        else
        {
            hitPlayers = Physics2D.OverlapCircleAll(attackL.position, playerStats.BasicAttackRange, PlayerLayer);
        }

        //deal dmg
        foreach (Collider2D player in hitPlayers)
        {
            Debug.Log("Hit!");
            player.GetComponent<PlayerController>().TakeDamage(playerStats.PlayerDMG);
        }
    }

    public void Die()
    {
        Debug.Log("enemy died");
        //die animation
        enemyAnimator.SetBool("Died", true);
        //cancle the enemy
        this.enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    public void EnemyMovement()
    {
        //Priorety level -> 1.top, 2.medium, 3.low.

        ShouldMove(); // 1
        ShouldJump(); // 2

    }

    public void ShouldJump()
    {
        //algorithem if enemy should jump.
        //When should he jump - > PlayerDash == true >> ShouldJump returns true aswell (only if chance rate is >= X (numb)
        if (playerCS.isDashing && Random.Range(0,100) >= 50) // have 50% chance to jump while dashing
        {
            
            rb.AddForce(Vector2.up * playerCS.jumpForce * 90 * Time.fixedDeltaTime);
        }
    }

    public void ShouldMove() 
    {
        if (!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("HeavyBandit_Attack"))
        {
            target = new Vector2(playerRangePoint.transform.position.x, rb.position.y);
            newPosition = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            LookAtPlayer();
            //rb.MovePosition(newPosition);
            if (!sr.flipX)
            {
                transform.Translate(speed * -1f * Time.deltaTime ,0,0);
            }
            else
            {
                transform.Translate(speed * Time.deltaTime, 0, 0);
            }
        }
    }
    
    public void LookAtPlayer()
    {
        if (transform.position.x > playerRangePoint.position.x)
        {
            sr.flipX = false;
        }
        else
        {
            sr.flipX = true;
        }
    }

    public void EnemyAttack()
    {
            if (Vector2.Distance(playerRangePoint.position, startingRangePoint.position) <= attackPlayerRange)
            {
                enemyAnimator.SetTrigger("Attack");
            }
    }
    void OnDrawGizmosSelected()
    {
        if (attackL == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(startingRangePoint.position, attackPlayerRange);
    }

   
}



   