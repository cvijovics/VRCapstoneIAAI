using UnityEngine;

public class DogApproachState : DogBaseState
{
    private Vector3 targetPos;
    public DogApproachState(DogController dog, Animator animator) : base(dog, animator) { }
    public override void OnEnter()
    {
        Debug.Log("Approach state entered");
        // Fade animation from one to another
    }

    public override void Update()
    {
        // Call dog's approach logic
        
    }

    public override void FixedUpdate()
    {
        dog.DestinationHandler();
    }

    public override void OnExit()
    {
        
    }


}
