using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{

    // Enemy's rigidbody and transform
    public Rigidbody2D rb;
    public Transform trans;

    // Player's transform
    public Transform target;

    // Enemy's movement speeds
    public float speed;

    // Enemy's orientation
    private Vector3 charScale;
    private float scaleX;

    // Information related to player being grounded
    public LayerMask ground;
    public Transform footing;
    public float radius;
    private bool isGrounded;

    // Enemy's attack information
    public float attackCooldownTime;
    private float attackCooldownTimer;
    public Transform attackZone;
    public float attackRange;
    public int attackDamage;
    public LayerMask player;
    private int addAttack;

    // Enemy's health
    public int health;
    private int startingHealth;
    public Transform bar;
    private Vector3 barScale;
    public float deathDelay;

    // Enemy's stunned information
    private bool stunned;
    public float stunnedTime;
    private float stunnedTimer;

    // Enemy's animator
    public Animator animator;

    // Enemy's audio sources
    public AudioSource clips;
    public List<AudioClip> effects = new List<AudioClip>(3);

    // Holds the value for the grid and node movement
    public GridMaker grid;
    private int move;

    // Holds enemy type
    public int type;

    // Holds the arrow rigid body
    public Rigidbody2D arrow;

    // Start is called before the first frame update
    void Start()
    {

        // Retrieve enemy's original x scale
        charScale = transform.localScale;
        scaleX = charScale.x;

        // Continue if bar exist
        if (bar != null)
        {

            // Set starting health
            startingHealth = health;
            barScale = bar.localScale;
        }

        // Set enemy's stunned state to false
        stunned = false;

        // Set node movement to zero
        move = 0;

        // Set additional attack to zero
        addAttack = 0;
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

        // Continue if not stunned
        else
        {

            // Stop once dead
            if (health > 0)
            {

                // Check if we are out of attack range
                if (Vector2.Distance(trans.position, target.position) > (attackRange * 2))
                {

                    // Lock in enemy and player position on node grid
                    Vector2 pos1 = trans.position - grid.transform.position;
                    Vector2 pos2 = target.position - grid.transform.position;

                    // Go through pathing and movement
                    Pathing(pos1, pos2);
                    Movement();
                }

                // Check if we are in attack range
                else
                {

                    // Fall check
                    if (rb.velocity.y == 0)
                    {

                        // Count cooldown timer down
                        attackCooldownTimer -= Time.deltaTime;

                        // Continue if timer runs out
                        if (attackCooldownTimer <= 0)
                        {

                            // Check for archer
                            if (type != 3)
                            {

                                // Run animation
                                animator.Play("Attack");
                            }

                            // Run attack method
                            Attack();

                            // Check for boss
                            if (type == 5)
                            {

                                // Mark additional attack
                                addAttack = 1;
                            }
                        }

                        // Continue additional attack after cooldown reaches half way mark
                        if (addAttack == 1 && attackCooldownTimer <= (attackCooldownTime / 2))
                        {

                            // Run animation
                            animator.Play("Attack 2");

                            // Run attack method
                            Attack();

                            // Set additional attack to 0
                            addAttack = 0;
                        }
                    }
                }

                // Run orientate method
                Orientate();
            }
        }
    }

    // Method for enemy movement
    private void Movement()
    {

        // Enemy's grounded state
        isGrounded = Physics2D.OverlapCircle(footing.position, radius, ground);

        // Continue if grid path isn't null
        if (grid.path != null && grid.path.Count > 0)
        {

            move = grid.path.Count - 1;

            // Get the direction to the target
            Vector2 steering = grid.path[move].position - (Vector2)trans.position;

            // Continue if enemy is in air
            if (isGrounded == false)
            {

                steering.y = -5;
            }

            // Continue if enemy is on ground
            else
            {

                steering.y = 0;
            }

            if (type != 3)
            {

                // Continue if the distance from the enemy to the final path node is greater than the attack range
                if (Vector2.Distance(trans.position, grid.path[move].position) > (attackRange))
                {

                    // If this is too fast, clip it to the max speed
                    if (steering.magnitude > speed)
                    {

                        steering.Normalize();
                        steering *= speed;
                    }

                    // Move towards target
                    rb.velocity = steering;
                }
            }

            else
            {

                // Continue if the distance from the enemy to the final path node is greater than the attack range
                if (Vector2.Distance(trans.position, target.position) > (attackRange * 2))
                {

                    // If this is too fast, clip it to the max speed
                    if (steering.magnitude > speed)
                    {

                        steering.Normalize();
                        steering *= speed;
                    }

                    // Move towards target
                    rb.velocity = steering;
                }

                else
                {

                    // Velocity set to zero
                    rb.velocity = Vector2.zero;
                }
            }
        }
    }

    // Method for attacking
    void Attack()
    {

        // Set attack timer
        attackCooldownTimer = attackCooldownTime;

        // Continue if not archer
        if (type != 3)
        {

            // Play attack audio
            clips.PlayOneShot(effects[0], 1);

            // Locate and attack player collider in front of enemy
            Collider2D[] targets = Physics2D.OverlapCircleAll(attackZone.position, attackRange, player);

            // Continue for each enemy found
            foreach (Collider2D player in targets)
            {

                // Enemy takes damage
                player.GetComponent<PlayerController>().TakeDamage(attackDamage);
            }
        }

        // Continue if archer
        else
        {
            // Run animation
            animator.Play("Attack");

            // Delay for arrow release and audio
            StartCoroutine(Shoot(animator.GetCurrentAnimatorStateInfo(0).length));
        }
    }

    // Method for orientation
    void Orientate()
    {

        // Continue if horizontal movement goes right
        if (trans.position.x - target.position.x > 0)
        {

            // Set orientation facing right
            charScale.x = scaleX;
        }

        // Continue if horizontal movement goes left
        else if (trans.position.x - target.position.x < 0)
        {

            // Set orientation facing left
            charScale.x = -scaleX;
        }

        // Set enemy's orientation
        transform.localScale = charScale;

        // Check for sufficient movement
        if (rb.velocity.x > .1f || rb.velocity.x < -.1f)
        {

            // Specific enemies can run
            if (type == 1 || type == 2 || type == 3)
            {

                // Animate run
                animator.SetBool("Run", true);
            }
        }

        // Continue if there isn't sufficient movement
        else
        {

            // Specific enemies can run
            if (type == 1 || type == 2 || type == 3)
            {

                // Stop running
                animator.SetBool("Run", false);
            }
        }
    }

    // Method for taking damage
    public void TakeDamage(int damage)
    {

        // Continue if health is greater than zero
        if (health > 0)
        {

            // Play hit audio
            clips.PlayOneShot(effects[1], 1);

            // Enemy takes a hit and becomes stunned
            health -= damage;
            stunned = true;
            stunnedTimer = stunnedTime;

            // Make sure enemy can be hit
            if (type == 1 || type == 4)
            {

                // Animate hit
                animator.SetTrigger("Hit");
            }
        }

        // Continue if bar exist
        if (bar != null && health >= 0)
        {

            // Health bar becomes lowered
            float var1 = (float)damage;
            float var2 = (float)startingHealth;
            float subtract = var1 / var2;
            barScale.x -= subtract;
            bar.localScale = barScale;
        }

        // Continue if enemy health is 0
        if (health <= 0)
        {

            // Play death audio
            clips.PlayOneShot(effects[2], 1);

            // Run death animation
            animator.SetTrigger("Dead");

            // Delay for death animation
            StartCoroutine(Die(animator.GetCurrentAnimatorStateInfo(0).length));
        }
    }

    // Method for death delay
    IEnumerator Die(float delay)
    {

        // Wait for animation to end
        yield return new WaitForSeconds(delay + deathDelay);

        // Continue for boss
        if (type == 5)
        {

            // Switch to final caption
            SceneManager.LoadScene(5);
        }

        // Remove enemy
        DestroyImmediate(this.gameObject, true);
    }

    // Method for the A* pathfinding
    private void Pathing(Vector3 start, Vector3 end)
    {

        // Set start and end nodes using given positions
        Node startNode = grid.NodeFromPosition(start);
        Node endNode = grid.NodeFromPosition(end);

        // Create the open and closed lists
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        // Add start node to the open list
        openList.Add(startNode);

        // Continue while the open list contains at least 1 node
        while (openList.Count > 0)
        {

            // Set current node to the first node in the open list
            Node currentNode = openList[0];

            // Run through the open list excluding the current node
            for (int i = 1; i < openList.Count; i++)
            {

                // Continue if current node has a higher f cost than the iterated node
                // Or if current node has an equal f cost and a higher h cost than the iterated node
                if (currentNode.F() > openList[i].F() || currentNode.F() == openList[i].F() && currentNode.h > openList[i].h)
                {

                    // Set current node to iterated node
                    currentNode = openList[i];
                }
            }

            // Remove the current node from the open list and add it to the closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // Continue if current node is equal to the end node
            if (currentNode == endNode)
            {

                // Run the FinalPath method using the start and end nodes
                FinalPath(startNode, endNode);
                return;
            }

            // Run through for each node found to be a neighbor of the current node
            foreach (Node neighbor in grid.Neighbors(currentNode))
            {

                // Continue if the neighbor is in the closed list
                if (closedList.Contains(neighbor))
                {

                    // Skip past this neighbor
                    continue;
                }

                // Calculate cost using the current node's g cost and ManhattenDistance method
                int cost = currentNode.g + ManhattenDistance(currentNode, neighbor);

                // Continue if calculated cost is less than the neighbor's g cost
                // Or if the open list does not contain neighbor node
                if (cost < neighbor.g || !openList.Contains(neighbor))
                {

                    // Neighbor node's g cost is now equal to the calculated cost
                    neighbor.g = cost;

                    // Neighbor node's h cost is equal to the distance calculated
                    // Using the ManhattenDistance method
                    neighbor.h = ManhattenDistance(neighbor, endNode);

                    // Neighbor node's parent node is assigned to the current node
                    neighbor.parent = currentNode;

                    // Continue if the openlist does not contain the neighbor node
                    if (!openList.Contains(neighbor))
                    {

                        // Add the neighbor node to the open list
                        openList.Add(neighbor);
                    }
                }
            }
        }
    }

    // Method for recieving the path to destination
    private void FinalPath(Node node1, Node node2)
    {

        // List containing the path of nodes needed
        List<Node> path = new List<Node>();

        // Setting the current node to goal node
        Node current = node2;

        // Continue while current node does not equal starting node
        while (current != node1)
        {

            // Add nodes to path list in reverse order
            path.Add(current);
            current = current.parent;
        }

        // Flip the path list so node 1 is the start and node 2 is the end
        path.Reverse();

        // Set the path on the grid to the path list created
        grid.path = path;
    }

    // Method for calculating the distance from the starting node to the goal node
    private int ManhattenDistance(Node node1, Node node2)
    {

        // Recieve x and y differences
        int x = Mathf.Abs(node1.gridX - node2.gridX);
        int y = Mathf.Abs(node1.gridY - node2.gridY);

        // Return distance
        return x + y;
    }

    // Method for arrow delay
    IEnumerator Shoot(float delay)
    {

        // Wait for animation to end
        yield return new WaitForSeconds(delay);

        // Play attack audio
        clips.PlayOneShot(effects[0], 1);

        // Continue if facing left
        if (charScale.x < 0)
        {

            // Set new vector, instantiate arrow shot, activate shot, and let it fly
            Vector2 newVector = new Vector2(trans.position.x - 2f, trans.position.y + .6f);
            Rigidbody2D shot = Instantiate(arrow, newVector, trans.rotation);
            shot.gameObject.SetActive(true);
            shot.velocity = trans.TransformDirection(Vector2.left * 8);
        }

        // Continue if facing right
        else
        {

            // Set new vector, instantiate arrow shot, activate shot, and let it fly (rotate right)
            Vector2 newVector = new Vector2(trans.position.x + 2f, trans.position.y + .6f);
            Rigidbody2D shot = Instantiate(arrow, newVector, trans.rotation);
            shot.gameObject.SetActive(true);
            shot.velocity = trans.TransformDirection(Vector2.right * 8);
            shot.transform.Rotate(0, 180, 0);
        }
    }
}
