using UnityEngine;

public class DogIdleState : DogBaseState
{
    public DogIdleState(DogController dog, Animator animator) : base(dog, animator) { }
    public override void OnEnter()
    {
        Debug.Log("Idle state entered");
    }

    public override void Update()
    {
        //dog.IdleRoam();
    }

}
