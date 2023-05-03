using System;
using Unity.Netcode;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : NetworkBehaviour {
    public Animator animator;
    public float walkSpeed = 10f;
    public float runSpeed = 20f;
    public Vector3 direction;
    private float currentSpeed;

    public float worldSize = 100;
    public GameManager gameManager;
    private Rigidbody _rb;
    public GameObject other;
    public float hitRadius;
    
    public Vector3[] clientPlacements;
    public Vector3[] hostPlacements;

    private static readonly int Walk = Animator.StringToHash("WALK");
    private static readonly int Run = Animator.StringToHash("RUN");
    private static readonly int Idle = Animator.StringToHash("IDLE");
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        _rb = GetComponent<Rigidbody>();
        Disable();
    }

    private void Update ()
    {
        if (!IsOwner) return;
        MoveHandler();
        LocationHandler();
    }

    private void MoveHandler()
    {

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement direction and speed
        direction = new Vector3(horizontal, 0f, vertical);
        direction.Normalize();
        print(direction);
        if (Vector3.zero == direction)
        {
            animator.SetBool(Idle,true);
            animator.SetBool(Run,false);
            animator.SetBool(Walk,false);
        }
        else
        {
            animator.SetBool(Idle,false);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed = runSpeed;
                animator.SetBool(Run,true);
                animator.SetBool(Walk,false);
            }
            else
            {
                currentSpeed = walkSpeed;
                animator.SetBool(Walk,true);
                animator.SetBool(Run,false);
            }
            
            // Move the player
            transform.position += direction * (currentSpeed * Time.deltaTime);
            if (Vector3.zero != direction)
            {
            
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 40f);
            }
        }
    }

    private void LocationHandler()
    {
        // Check if the character is outside the bounds
        // Get the position of the player or AI
        Vector3 position = transform.position;

        // Check if the player or AI has gone beyond the X boundaries
        if (position.x < -worldSize / 2f)
        {
            position.x = worldSize / 2f;
        }
        else if (position.x > worldSize / 2f)
        {
            position.x = -worldSize / 2f;
        }

        // Check if the player or AI has gone beyond the Y boundaries
        if (position.y < -worldSize / 2f)
        {
            position.y = worldSize / 2f;
        }
        else if (position.y > worldSize / 2f)
        {
            position.y = -worldSize / 2f;
        }

        // Check if the player or AI has gone beyond the Z boundaries
        if (position.z < -worldSize / 2f)
        {
            position.z = worldSize / 2f;
        }
        else if (position.z > worldSize / 2f)
        {
            position.z = -worldSize / 2f;
        }

        // Update the position of the player or AI
        transform.position = position;

        //
        // // Check distance for hit
        // if (CompareTag("Player") && false)
        // {
        //     if (Vector3.Distance(other.transform.position, transform.position) < hitRadius)
        //     {
        //         _animator.SetTrigger(Near);
        //     }
        // }
        // else if (CompareTag("AI"))
        // {
        //     if (Vector3.Distance(other.transform.position, transform.position) < hitRadius)
        //     {
        //         _animator.SetTrigger(Near);
        //     }
        // }
    }

    public void Disable()
    {
        enabled = false;
    }
    
    public void Enable()
    {
        enabled = true;
    }
}