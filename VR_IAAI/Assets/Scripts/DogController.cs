using System.Collections;
//using System.Numerics;




//using System.Numerics;
using UnityEngine.SceneManagement;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class DogController : MonoBehaviour
{
    public InputActionReference beckonAction;
    public InputActionReference ballAction;
    [SerializeField] private AudioController audioController;
    [SerializeField] private AudioClip whistleClip;
    [SerializeField] private GameObject dog;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject ball;
    [SerializeField] private AudioClip[] walkClips;
    [SerializeField] private AudioClip[] runClips;
    private NavMeshAgent agent;
    [SerializeField] private Animator dogAnimator;
    private bool isRunning = false; // True when dog is running
    private bool isRotating = false;
    private bool isRoaming = false;
    private bool isPickingUp = false;
    private bool isDropping = false;
    private bool isSitting = false;
    private bool isStandingUp = false;
    private bool soundPlayed = false;
    private bool hasBall = false;
    private float idleTimer = 0f;
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
        
        if (agent.speed < 2f)
        {
            idleTimer += Time.deltaTime;
        } else
        {
            idleTimer = 0f;
        }

        if (idleTimer > 5f)
        {
            isSitting = true;   
        }

        stateMachine.Update();

        if (isRunning)
        {
            if (agent.velocity.sqrMagnitude > 0.1f)
            {
                dogAnimator.SetFloat("direction", GetDogAngle());
            }
            dogAnimator.SetFloat("speed", agent.velocity.magnitude);
               
        }
        if (isRunning && !isRoaming && !soundPlayed && !isRotating)
        {
            soundPlayed = true;
            StartCoroutine(StepWait(0.5f, runClips));
        }

        if (isRoaming && isRunning && !soundPlayed && !isRotating)
        {
            soundPlayed = true;
            StartCoroutine(StepWait(0.5f, walkClips));
        }
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    IEnumerator StepWait(float waitTime, AudioClip[] audioClips)
    {
        audioController.PlaySFXRandom(audioClips, dog.transform, 0.4f);
        yield return new WaitForSeconds(waitTime);
        soundPlayed = false;
    }

    public void RunToPlayer(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            audioController.PlaySFX(whistleClip, player.transform, 0.3f);
            InterruptRoam();
            if (isSitting)
            {
                StartCoroutine(StandUp(() => {
                    currentDest = player;
                    isRunning = true;
                    agent.speed = 2f; // Set running speed
                }));
                
            } else
            {
                currentDest = player;
                isRunning = true;
                agent.speed = 2f; // Set running speed
            }
            
            //dogAnimator.SetBool("hasObjective", true);
        }
    }

    public void RunToBall()
    {

        InterruptRoam();
        if (isSitting)
        {
            StartCoroutine(StandUp(() => {
                currentDest = ball;
                isRunning = true;
                agent.speed = 2f; // Set running speed
                
            }));
        } 
        else
        {
            currentDest = ball;
            agent.speed = 2f; // Set running speed
            isRunning = true;
        }        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger hit by: {other.name} on GameObject: {other.gameObject.name}, tag: {other.gameObject.tag}");

        if(other.CompareTag("Player"))
        {
            if (!isPet)
            {
                isPet = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //nothing
    }

    public void DestinationHandler()
    {
        if (currentDest == null)
        {
            isRunning = false;
            return;
        }
        Debug.Log("Dog is running towards " + currentDest.name + " at position " + currentDest.transform.position);
        float angleToTarget = GetAngleToTarget();

        if (Mathf.Abs(angleToTarget) > 120f && !isRotating)
        {
            StartCoroutine(TurnAngle(angleToTarget));
        }
        else
        {
            ApproachTarget(currentDest.transform.position);
        }
    }

    public void ApproachTarget(Vector3 targetPos)
    {
        if (isPickingUp || isDropping || isRotating || isSitting)
        {
            return; // Don't move while picking up or dropping the ball
        }

        if (currentDest == null)
        {
            isRunning = false;
            return;
        }

        dogAnimator.SetBool("isRunning", true);
        agent.destination = targetPos;
        if (!agent.pathPending)
        {

            Debug.Log("remaining distance: " + Vector3.Distance(agent.destination, agent.transform.position));
            Debug.Log("stopping distance: " + agent.stoppingDistance);
            if ( Vector3.Distance( agent.destination, agent.transform.position) <= agent.stoppingDistance)
            {
                agent.isStopped = true;
                agent.velocity = new Vector3(0,0,0);
                dogAnimator.SetFloat("direction", 0);
                dogAnimator.SetFloat("speed", 0);
                if (hasBall)
                {
                    StartCoroutine(DropWait());
                } else if (currentDest == ball)
                {              
                    
                    StartCoroutine(PickupWait());
                } else {
                    agent.velocity = new Vector3(0,0,0);
                    isRunning = false;
                    isRoaming = false;
                    agent.isStopped = false;
                    dogAnimator.SetBool("hasObjective", false);
                    dogAnimator.SetBool("isRoaming", false);
                    dogAnimator.SetBool("isRunning", false);
                    if (currentDest.name == "RandomPoint")
                    {
                        Destroy(currentDest);
                    }
                }
            }
        } 
    }

    private Coroutine roamCoroutine;
    void InterruptRoam()
    {
        if (roamCoroutine != null)
        {
            StopCoroutine(roamCoroutine);
            roamCoroutine = null;
        }

        if (currentDest != null && currentDest.name == "RandomPoint")
        {
            Destroy(currentDest);
            currentDest = null;
        }

        agent.velocity = new Vector3(0,0,0);
        isRunning = false;
        isRoaming = false;
        agent.isStopped = false;
        agent.updateRotation = true;
        dogAnimator.SetBool("hasObjective", false);
        dogAnimator.SetBool("isRoaming", false);
        dogAnimator.SetBool("isRunning", false);
        if (currentDest != null && currentDest.name == "RandomPoint")
        {
            Destroy(currentDest);
        }
    }

    private float smoothedAngle = 0f;
    public float GetDogAngle()
    {
        Vector3 desiredDirection = agent.desiredVelocity;
        if (desiredDirection.sqrMagnitude < 0.01f)
        {
            smoothedAngle = Mathf.Lerp(smoothedAngle, 0f, Time.deltaTime * 5f); // Smoothly return to 0 when not moving
            return smoothedAngle;
        }

        float targetAngle = Vector3.SignedAngle(transform.forward, desiredDirection, Vector3.up);

        targetAngle = Mathf.Clamp(targetAngle, -179f, 179f); // Limit the angle to prevent extreme values
        smoothedAngle = Mathf.Lerp(smoothedAngle, targetAngle, Time.deltaTime * 5f); // Smoothly interpolate towards the target angle

        Debug.Log("Target Angle: " + targetAngle + ", Smoothed Angle: " + smoothedAngle);
        return smoothedAngle;
    }

    public float GetAngleToTarget()
    {
        if (currentDest == null)
        {
            return 0f;
        }

        Vector3 directionToTarget = currentDest.transform.position - transform.position;
        directionToTarget.y = 0; // Ignore vertical difference
        float targetAngle = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
        
        return Mathf.Clamp(targetAngle, -140f, 140f); // Limit the angle to prevent extreme values
    }
    
    // Used some genAI for this cause it was a bit confusing
    IEnumerator TurnAngle(float targetAngle)
    {
        yield return null; // Wait one frame to ensure this doesn't interfere with any ongoing movement logic

        isRotating = true;
        agent.updateRotation = false; // Disable automatic rotation
        agent.isStopped = true; // Stop the agent while rotating

        dogAnimator.SetBool("isRotatingLeft", targetAngle < 0);
        dogAnimator.SetBool("isRotatingRight", targetAngle > 0);
        yield return null; // Wait one frame for the animation state to update
        
        

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + targetAngle, 0);

        float rotateSpeed = 140f;
        float duration = Mathf.Abs(targetAngle) / rotateSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            //smoothing
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, smoothT);

            yield return null;
        }

        transform.rotation = targetRotation; 

        dogAnimator.SetBool("isRotatingLeft", false);
        dogAnimator.SetBool("isRotatingRight", false);
        yield return null; // Wait one frame for the animation state to update

        isRotating = false;
        agent.updateRotation = true; // Re-enable automatic rotation
        agent.isStopped = false; // Resume the agent after rotating

        ApproachTarget(currentDest.transform.position); // Continue towards the target after rotating
    }

    IEnumerator StandUp(System.Action onComplete)
    {
        isStandingUp = true;
        dogAnimator.SetBool("isSitting", false);
        yield return null; // Wait one frame for the animation state to update

        yield return new WaitUntil(() => 
            dogAnimator.GetCurrentAnimatorStateInfo(0).IsName("SitEnd"));
        
        float clipLength = dogAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(clipLength); // Wait for the stand up animation to finish

        isSitting = false;
        isStandingUp = false;
        idleTimer = 0f;
        agent.isStopped = false;

        onComplete?.Invoke();
    }

    public void PetResponse()
    {
        if (isPet && !isRunning && !isPickingUp && !isDropping)
        {
            StartCoroutine(PetWait());
        }
        
    }

    //basically a debug function to make sure the dog stops doing anything related to the ball if the ball is taken away mid-action
    public void OnBallTaken()
    {
        if (hasBall || isPickingUp || isDropping)
        {
            StopAllCoroutines();
            isPickingUp = false;
            isRoaming = false;
            isRotating = false;
            isDropping = false;
            soundPlayed = false;
            hasBall = false;
            agent.isStopped = false;
            agent.ResetPath();
            agent.velocity = new Vector3(0,0,0);
            
            if (currentDest != null && currentDest.name == "RandomPoint")
            {
                Destroy(currentDest);
            }
            currentDest = null;
            dogAnimator.SetBool("hasObjective", false);
            dogAnimator.SetBool("isRunning", false);
            dogAnimator.SetBool("isRotatingLeft", false);
            dogAnimator.SetBool("isRotatingRight", false);
            dogAnimator.ResetTrigger("pickup");
            dogAnimator.ResetTrigger("drop");
            RunToBall();

        }
    }

    IEnumerator PickupWait()
    {
        
        isPickingUp = true;
        isRunning = false;
        dogAnimator.ResetTrigger("pickup");
        dogAnimator.SetTrigger("pickup");
        yield return null; // Wait one frame for the trigger to register and the animation to start

        dogAnimator.SetBool("isRunning", false);
        yield return null; // Wait one frame for the animation state to update

        yield return new WaitUntil(() => 
            dogAnimator.GetCurrentAnimatorStateInfo(0).IsName("Pickup Ball"));

        float clipLength = dogAnimator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(clipLength*0.3f); // Wait until the point in the animation where the ball should be picked up
        ballController.AttachObject();

        yield return new WaitForSeconds(clipLength*0.7f); // Wait for the rest of the animation to finish
        currentDest = player;
        agent.isStopped = false;
        agent.speed = 2f; // Set running speed
        isRunning = true;
        hasBall = true;
        dogAnimator.SetBool("hasObjective", true);
        isPickingUp = false;
    }
    IEnumerator DropWait()
    {
        isRunning = false;
        dogAnimator.ResetTrigger("drop");
        dogAnimator.SetTrigger("drop");
        yield return null;

        yield return new WaitUntil(() => 
            dogAnimator.GetCurrentAnimatorStateInfo(0).IsName("Drop Ball"));

        float clipLength = dogAnimator.GetCurrentAnimatorStateInfo(0).length;
        float dropTime = clipLength * 0.5f; 
        
        yield return new WaitForSeconds(clipLength*0.3f); // Wait until the point in the animation where the ball should be dropped
        ballController.DetachObject();

        yield return new WaitForSeconds(clipLength*0.7f); // Wait for the rest of the animation to finish
        hasBall = false;
        isDropping = false;
        isRunning = false;
        isRoaming = false;
        dogAnimator.SetBool("hasObjective", false);
        dogAnimator.SetBool("isRoaming", false);
        dogAnimator.SetBool("isRunning", false);
        agent.isStopped = false;
    }

    IEnumerator PetWait()
    {
        isSitting = false;
        isStandingUp = false;
        idleTimer = 0f;
        agent.isStopped = true;
        dogAnimator.SetBool("isPet", true);

        yield return new WaitUntil(() => 
            dogAnimator.GetCurrentAnimatorStateInfo(0).IsName("Pet"));
        
        float clipLength = dogAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(clipLength);

        isPet = false;
        dogAnimator.SetBool("isPet", false);
        agent.isStopped = false;
    }

    public void IdleRoam()
    {
        if (isSitting && !isRunning && !isStandingUp)
        {
            agent.isStopped = true;
            dogAnimator.SetBool("isSitting", true);
            if (roamCoroutine != null)
            {
                StopCoroutine(roamCoroutine);
            }
            return;
        } else if (!isStandingUp)
        {
            agent.isStopped = false;
            dogAnimator.SetBool("isSitting", false);
        }
        if (!isRoaming && !isRunning && !isSitting && SceneManager.GetActiveScene().name != "TutorialScene")
        {
            roamCoroutine = StartCoroutine(RoamWait(Random.Range(2f, 5f)));
        }
        
    }

    public Vector3 GetRandomPoint(float radius)
    {
        var randomDirection = Random.insideUnitSphere * radius;
        randomDirection.y = 0; // Keep the point on the ground
        var randomPoint = transform.position + randomDirection;

        NavMeshHit hit; // stores result of navmesh sample position
        Vector3 finalPosition = transform.position;

        if (NavMesh.SamplePosition(randomPoint, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    IEnumerator RoamWait(float waitTime)
    {
        isRoaming = true;
        yield return new WaitForSeconds(waitTime);
        
        if (isRunning)
        {
            isRoaming = false;
            yield break;
        }
        agent.speed = 1f; // Set roaming speed
        Vector3 randomPoint = GetRandomPoint(10f);
        currentDest = new GameObject("RandomPoint") { transform = { position = randomPoint } };
        isRunning = true;
        isRoaming = false;

        


    }
}
