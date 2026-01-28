using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class DogController : MonoBehaviour
{
    public InputActionReference beckonAction;
    [SerializeField] private GameObject dog;
    [SerializeField] private GameObject player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator dogAnimator;
    private bool isRunning = false; // True when dog is running to player

    StateMachine stateMachine;

    void Awake()
    {
        // State machine
        stateMachine = new StateMachine();
    

        // Declare states
        var approachState = new DogApproachState(this, dogAnimator);
        var petState = new DogPetState(this, dogAnimator);
        var idleState = new DogIdleState(this, dogAnimator);

        // Define transitions

        // Set initial state
        stateMachine.SetState(idleState);

    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void RunToPlayer(InputAction.CallbackContext context)
    {
        Debug.Log("Dog is running to player at position: " + player.transform.position);
        if (context.performed && !isRunning)
        {
            isRunning = true;
            dogAnimator.SetBool("isRunning", true);
        }
    }

    public void ApproachTarget(Vector3 targetPos)
    {
        dogAnimator.SetBool("isRunning", true);
        agent.destination = targetPos;
        if (!agent.pathPending)
        {
            Debug.Log("remaining distance: " + Vector3.Distance(agent.destination, agent.transform.position));
            Debug.Log("stopping distance: " + agent.stoppingDistance);
            if ( Vector3.Distance( agent.destination, agent.transform.position) <= agent.stoppingDistance)
            {
                dog.transform.LookAt(player.transform);
                agent.velocity = new Vector3(0,0,0);
                dogAnimator.SetBool("isRunning", false);
            }
        } 
    }

    public void petResponse()
    {
        // Do pet response anim, stop moving, etc
    }

    public void idleRoam()
    {
        
    }
}
