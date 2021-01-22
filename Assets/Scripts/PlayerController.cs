
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]private PlayerStats playerStats;


    
    public Animator animator;
    public Animation dashButtonCDAnim;
    private Rigidbody2D mainPlayerRb;
    private Collider2D playerCollider2D;
    private SpriteRenderer sr;
    public Transform attackPointR;
    public Transform attackPointL;
    public Joystick joystick;
    public ParticleSystem firePs;

    public List<ParticleSystem> CloudAttackList;

    public float movementSpeed = 0f;
    public float jumpForce;
    private int currentHealth;
    private int comboCounter;
    public bool isDashing = false;

    public float attackRate = 2f;
    private float nextAttackTime;

    public LayerMask EnemyLayers;
    public LayerMask PlatformLayer;

    void Start()
    {

        mainPlayerRb = GetComponent<Rigidbody2D>();
        playerCollider2D = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = playerStats.PlayerHP;
        
    }


    void Update()
    {
        if (dashButtonCDAnim.isPlaying) // checking if the dash is on cooldown for enemy AI
        {
            isDashing = false;
        }
    }


    //using in event animation under the "Attack" animation
    public void Attack()
    {
        Collider2D[] hitEnemies;

        //Attack range
        if (sr.flipX)
        {
            hitEnemies = Physics2D.OverlapCircleAll(attackPointR.position, playerStats.BasicAttackRange, EnemyLayers);
        }
        else
        {
            hitEnemies = Physics2D.OverlapCircleAll(attackPointL.position, playerStats.BasicAttackRange, EnemyLayers);
        }

        //deal dmg
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit!");
            enemy.GetComponent<Enemy>().TakeDamage(playerStats.PlayerDMG);
        }
    }

    void FixedUpdate()
    {
        //ps.transform.position = Vector3.MoveTowards(ps.transform.position,transform.position, 1000f);

        Movement();
        IsJumping();
        
    }

    void Movement()
    {
        if (joystick.Horizontal >= 0.2f)
        {
            movementSpeed = 8f;
            sr.flipX = true;
            animator.SetFloat("Speed", movementSpeed);
            transform.Translate(movementSpeed * Time.deltaTime, 0, 0);

        }
        else if (joystick.Horizontal <= -0.5f)
        {
            movementSpeed = 8f;
            sr.flipX = false;
            animator.SetFloat("Speed", movementSpeed);
            transform.Translate(movementSpeed * Time.deltaTime * -1, 0, 0);

        }
        else
        {
            movementSpeed = 0f;
            animator.SetFloat("Speed", movementSpeed);
        }

    }

    void OnDrawGizmosSelected()
    {
        if (attackPointR == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPointR.position, playerStats.BasicAttackRange);
    }

    private bool IsGrounded()
    {
        float extraHeightText = 0.1f;
        RaycastHit2D raycasthit = Physics2D.BoxCast(playerCollider2D.bounds.center, playerCollider2D.bounds.size, 0f, Vector2.down, extraHeightText, PlatformLayer);

        return raycasthit.collider != null;
    }

    private void IsJumping()
    {
        float verticalMove = joystick.Vertical;
        //Checking jump, if the player can jump, will do the jump
        if (verticalMove >= .5f && IsGrounded())
        {
            mainPlayerRb.AddForce(Vector2.up * jumpForce);
        }

        if (IsGrounded())
        {
            animator.SetBool("IsJumping", false);
        }
        else
        {
            animator.SetBool("IsJumping", true);

        }

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    public void Die()
    {
        //die animation
        animator.SetBool("Died", true);
        
        this.enabled = false;
    }

    
    public void basicAttackButton()
    {
        
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("MainPlayer_Attack"))
        {
            animator.SetTrigger("Attack");
        }

    }

    public void DashButton()
    {
        if (!dashButtonCDAnim.isPlaying)
        {
            isDashing = true;
            LeanTween.followLinear(firePs.transform, gameObject.transform, LeanProp.localX, 7f);
            firePs.transform.position = transform.position;
            firePs.Play();
            if (joystick.Horizontal >= 0.2f)
            {
                sr.flipX = true;
                animator.SetFloat("Speed", movementSpeed);
                transform.Translate(2, 0, 0);
            }
            else if (joystick.Horizontal <= -0.5f)
            {
                sr.flipX = false;
                animator.SetFloat("Speed", movementSpeed);
                transform.Translate(2 * -1, 0, 0);
            }

            dashButtonCDAnim.Play();
        }

    }


    public void PlayerCloudAttackButton()
    {
        foreach (ParticleSystem cloudAttack in CloudAttackList)
        {
            cloudAttack.Play();
        }
       

    }
}

    
