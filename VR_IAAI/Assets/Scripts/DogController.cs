using System.Collections.Generic;
//using System.Numerics;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class DogController : MonoBehaviour
{
    public InputActionReference beckonAction;
    public InputActionReference ballAction;
    [SerializeField] private GameObject dog;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject ball;
    private NavMeshAgent agent;
    [SerializeField] private Animator dogAnimator;
    private bool isRunning = false; // True when dog is running
    private bool hasBall = false;
    private GameObject currentDest;
    private BallController ballController;
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
        ballController = GetComponent<BallController>();
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
        if (context.performed && !isRunning)
        {
            isRunning = true;
            currentDest = player;
        }
    }

    public void RunToBall()
    {
        if (!isRunning)
        {
            isRunning = true;
            currentDest = ball;
        }
    }

    public void DestinationHandler()
    {
        Debug.Log("Dog is running towards " + currentDest.name + " at position " + currentDest.transform.position);
        ApproachTarget(currentDest.transform.position);
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
                if (hasBall)
                {
                    ballController.DetachObject();
                    hasBall = false;
                }
                agent.velocity = new Vector3(0,0,0);
                isRunning = false;
                dogAnimator.SetBool("isRunning", false);
                if (currentDest == ball)
                {
                    //play pickup anim
                    ballController.AttachObject();
                    currentDest = player;
                    isRunning = true;
                    hasBall = true;
                } 
            }
        } 
    }
    public void PetResponse()
    {
        if (isPet)
        {
            //set animator

            //maybe set timer for pet duration?
            
            isPet = false;
        }
    }

    public void IdleRoam()
    {
        
    }
}
