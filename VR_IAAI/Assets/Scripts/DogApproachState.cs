using UnityEngine;

public class DogApproachState : DogBaseState
{
    private Vector3 targetPos;
    public DogApproachState(DogController dog, Animator animator, Vector3 targetPos) : base(dog, animator)
    {
        this.targetPos = targetPos;
    }
    public override void OnEnter()
    {
        // Fade animation from one to another
    }

    public override void Update()
    {
        // Call dog's approach logic
        dog.ApproachTarget(this.targetPos);
    }
}
