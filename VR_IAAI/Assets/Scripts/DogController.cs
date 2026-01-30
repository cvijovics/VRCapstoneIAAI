using System.Collections.Generic;
//using System.Numerics;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class DogController : MonoBehaviour
{
    public InputActionReference beckonAction;
    [SerializeField] private GameObject dog;
    [SerializeField] private GameObject player;
    // Ball object here
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator dogAnimator;
    private bool isRunning = false; // True when dog is running to player
    private bool isPet = false; // True when player has touched dog physically

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
        At(idleState, approachState, new FuncPredicate(() => isRunning));
        At(approachState, idleState, new FuncPredicate(() => !isRunning));

        At(idleState, petState, new FuncPredicate(() => isPet));
        At(petState, idleState, new FuncPredicate(() => !isPet));
        Any(idleState, new FuncPredicate(ReturnToIdleState));

        // Set initial state
        stateMachine.SetState(idleState);

    }

    bool ReturnToIdleState()
    {
        return !isRunning 
            && !isPet;
    }

    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        stateMachine.Update();
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
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

    public void destinationHandler()
    {
        Vector3 targetPos = new Vector3();
        // if running to player

        
        // if running to ball
        targetPos = player.transform.position; // change logic later
        ApproachTarget(targetPos);
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
                agent.velocity = new Vector3(0,0,0);
                isRunning = false;
                dogAnimator.SetBool("isRunning", false);
            }
        } 
    }

    public void petResponse()
    {
        if (isPet)
        {
            //set animator
            //maybe set timer for pet duration?
        }
    }

    public void idleRoam()
    {
        
    }
}
