using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    CharacterController controller;
    Animator animator;
    Vector3 enemyVelocity;
    [SerializeField] GameObject player;
    [SerializeField] float rotationAdjustment;
    Vector3 playerOffset;
    bool isCircling = false;
    bool isWalking = false;
    bool specialMovement = false;
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float specialMovementSpeed = 15f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // get the vector between the player and the enemy
        playerOffset = transform.position - player.transform.position;

        if (specialMovement) {
            controller.Move(enemyVelocity * Time.deltaTime);
            return;
        }

        if (isCircling) {
            circlePlayer();
        } 

        else if (isWalking) {
            Walk();
        }

        else {
            enemyVelocity = Vector3.zero;
        }

        enemyVelocity.y = 0f;
        controller.Move(enemyVelocity * Time.deltaTime);
    }

    public void setIsCircling(bool circle, bool direction) {
        isCircling = circle;
        if (direction) {
            rotationAdjustment *= -1;
        }
    }

    public void setIsWalking(bool walking, bool walkForward) {
        isWalking = walking;
        // check the walk direction bool and walk speed and change to ensure it is moving in the right direciton
        if ((!walkForward && walkSpeed > 0) || (walkForward && walkSpeed < 0)) {
            walkSpeed *= -1;
        }
    }

    void circlePlayer() {
        // adjust the value of the rotation speed based on the distance from the player
        float circleRotationSpeed;
            circleRotationSpeed = 1/playerOffset.magnitude * rotationAdjustment;

        // get the destination for the next frame to move in a circle
        Vector3 destination = Quaternion.Euler(0, circleRotationSpeed, 0) * playerOffset + player.transform.position;

        transform.LookAt(player.transform.position);
        enemyVelocity = destination - transform.position;

        // set the animation speed/direction
        float animatorVelocity =  -circleRotationSpeed/8f;
        animator.SetFloat("HorizontalInput", animatorVelocity);
    }

    void Walk() {
        transform.LookAt(player.transform.position);
        enemyVelocity = transform.forward * walkSpeed;

        // set the animator movement values
        animator.SetFloat("VerticalInput", walkSpeed);
    }

    public void cancelMovement()
    {
        specialMovement = false;

        isWalking = false;
        animator.SetFloat("VerticalInput", 0f);

        isCircling = false;
        animator.SetFloat("HorizontalInput", 0f);

        enemyVelocity = Vector3.zero;
    }

    // TODO: MAYBE CHANGE THIS TO MOVE TO A CENTRAL POSITION
    // TODO: ALTERNATIVELY, CALCULATE THE JUMP DEPENDING ON THE PLAYER AND ENEMY POSITIONS
    public void jumpBack() {
        specialMovement = true;
        enemyVelocity = transform.forward * -1 * specialMovementSpeed;
    }
}
