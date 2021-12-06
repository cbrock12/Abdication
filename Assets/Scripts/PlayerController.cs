using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    // Player's rigidbody
    public Rigidbody2D rb;

    // Player's movement speeds
    public float speed;
    public float jumpSpeed;

    // Player's orientation
    private Vector3 charScale;
    private float scaleX;

    // Information related to player being grounded
    public LayerMask ground;
    public Transform footing;
    public float radius;
    private bool isGrounded;

    // Player's jumping information
    public float jumpTime;
    private float jumpTimer;
    private bool isJumping;

    // Player's attack information
    private bool isAttacking;
    private int attackCounter;
    public float attackCooldownTime;
    private float attackCooldownTimer;
    public float attackComboTime;
    private float attackComboTimer;
    public Transform attackZone;
    public float attackRange;
    public int attackDamage;
    public LayerMask enemies;

    // Player's health
    public int health;
    private int startingHealth;
    public Transform bar;
    private Vector3 barScale;

    // Player's scene of death
    private static readonly string level = "level";

    // Player's stunned information
    private bool stunned;
    public float stunnedTime;
    private float stunnedTimer;

    // Player's animator
    public Animator animator;

    // Player's audio sources
    public AudioSource clips;
    public List<AudioClip> effects = new List<AudioClip>(3);

    // Start is called before the first frame update
    void Start()
    {

        // Retrieve player's original x scale
        charScale = transform.localScale;
        scaleX = charScale.x;

        // Set attack state and counter to zero
        isAttacking = false;
        attackCounter = 0;

        // Set starting health
        startingHealth = health;
        barScale = bar.localScale;

        // Set player's stunned state to false
        stunned = false;

        // Set player's current level
        PlayerPrefs.SetFloat(level, SceneManager.GetActiveScene().buildIndex);
    }

    // Update is called once per frame
    void Update()
    {

        // Continue if stunned
        if (stunned == true)
        {

            // Count stunned timer down
            stunnedTimer -= Time.deltaTime;

            // Continue if stunned timer is finished
            if (stunnedTimer <= 0)
            {

                // Stun state is no longer applied
                stunned = false;
            }
        }

        else
        {

            // Continue if there is horizontal movement
            if (Input.GetAxis("Horizontal") != 0)
            {

                // Run orientate method
                Orientate();
            }

            // Continue if there is no horizontal movement
            else
            {

                // Stop run animation
                animator.SetBool("Run", false);
            }

            // Player's grounded state
            isGrounded = Physics2D.OverlapCircle(footing.position, radius, ground);

            // Continue if player is grounded and presses w
            if (isGrounded == true && Input.GetKeyDown("w"))
            {

                // Run jump method
                Jump();
            }

            // Continue if player is still holding w after jumping
            if (Input.GetKey("w") && isJumping == true)
            {

                // Run extended jump method
                ExtJump();
            }

            // Continue if player lets go of w
            if (Input.GetKeyUp("w"))
            {

                // Player is no longer jumping
                isJumping = false;
            }

            // Continue if grounded
            if (isGrounded == true)
            {

                // Stop falling animation
                animator.SetBool("Fall", false);
            }

            // Continue if player moves downward
            else
            {

                animator.SetBool("Fall", true);
            }

            // Count cooldown timer down
            attackCooldownTimer -= Time.deltaTime;

            // Continue if player left clicked
            if (Input.GetMouseButtonDown(0) && isGrounded == true && attackCooldownTimer <= 0)
            {

                // Run animation
                animator.Play("Attack1");

                // Run attack method
                Attack();
            }

            // Continue if player is currently attacking
            if (isAttacking == true)
            {

                // Run attack combo method
                AttackCombo();
            }
        }
    }

    // Method for attacking
    void Attack()
    {

        // Play sword swing audio
        clips.PlayOneShot(effects[0], 1);

        // Locate and attack enemy colliders in front of player
        Collider2D[] targets = Physics2D.OverlapCircleAll(attackZone.position, attackRange, enemies);

        // Set attack counter and timers
        isAttacking = true;
        attackCounter += 1;
        attackComboTimer = attackComboTime;
        attackCooldownTimer = attackCooldownTime;

        // Continue for each enemy found
        foreach (Collider2D enemy in targets)
        {

            // Enemy takes damage
            enemy.GetComponent<EnemyController>().TakeDamage(attackDamage);
        }
    }

    // Method for attack combo
    void AttackCombo()
    {

        // Continue if combo still has time
        if (attackComboTimer > 0)
        {

            // Count combo timer down
            attackComboTimer -= Time.deltaTime;

            // Continue if player left clicked
            if (Input.GetMouseButtonDown(0) && isGrounded == true && attackComboTimer <= (attackComboTime - .2))
            {

                // Continue if one attack has occured
                if (attackCounter == 1)
                {

                    // Run animation
                    animator.Play("Attack2");

                    // Run attack method
                    Attack();
                }

                // Continue if more than one attack has occured
                else
                {

                    // Run animation
                    animator.Play("Attack3");

                    // Run attack method
                    Attack();

                    // Combo ended
                    isAttacking = false;
                    attackCounter = 0;
                }
            }
        }

        // Continue if combo timer has ran out
        else
        {

            // Combo ended
            isAttacking = false;
            attackCounter = 0;
        }
    }

    // Method for jumping
    void Jump()
    {

        // Player jumps
        rb.velocity = Vector2.up * jumpSpeed;
        isJumping = true;
        jumpTimer = jumpTime;
        animator.SetTrigger("Jump");

        // Allow player to move left or right in air
        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y);
    }

    // Method for extended jump
    void ExtJump()
    {

        // Animate Fall
        animator.SetBool("Fall", true);

        // Continue if the jump timer hasn't ran out
        if (jumpTimer > 0)
        {

            // Player continues ascent and jump timer counts down
            rb.velocity = Vector2.up * jumpSpeed;
            jumpTimer -= Time.deltaTime;

            // Allow player to move left or right in air
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y);
        }

        // Continue if the jump timer has ran out
        else
        {

            // Player is no longer jumping
            isJumping = false;
        }
    }

    // Method for orientation
    void Orientate()
    {

        // Player's horizontal movement
        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y);

        // Continue if horizontal movement goes right
        if (Input.GetAxis("Horizontal") > 0)
        {

            // Set orientation facing right
            charScale.x = scaleX;
        }

        // Continue if horizontal movement goes left
        else if (Input.GetAxis("Horizontal") < 0)
        {

            // Set orientation facing left
            charScale.x = -scaleX;
        }

        // Set player's orientation
        transform.localScale = charScale;

        // Animate run
        animator.SetBool("Run", true);
    }

    // Method for taking damage
    public void TakeDamage(int damage)
    {

        // Make player health 0 if damage is too much
        if (damage > health)
        {

            // Player is dead
            health = 0;
        }

        // Continue if player health is above 0
        if (health > 0)
        {

            // Play hit audio
            clips.PlayOneShot(effects[1], 1);

            // Player takes a hit and becomes stunned
            health -= damage;
            animator.SetTrigger("Hit");
            stunned = true;
            stunnedTimer = stunnedTime;

            // Health bar becomes lowered
            float var1 = (float)damage;
            float var2 = (float)startingHealth;
            float subtract = var1 / var2;
            barScale.x -= subtract;
            bar.localScale = barScale;
        }

        // Continue if player health is 0 or less
        if (health == 0)
        {

            // Play death audio
            clips.PlayOneShot(effects[2], 1);

            // Run death animation
            animator.SetTrigger("Death");

            // Delay for death animation
            StartCoroutine(Die(animator.GetCurrentAnimatorStateInfo(0).length));
        }
    }

    // Method for death delay
    IEnumerator Die(float delay)
    {

        // Wait for animation to end
        yield return new WaitForSeconds(delay - .2f);

        // Switch to post death scene
        SceneManager.LoadScene(6);
    }
}
