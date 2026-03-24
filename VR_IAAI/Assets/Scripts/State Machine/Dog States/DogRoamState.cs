using UnityEngine;

public class DogRoamState : DogBaseState
{
    public DogRoamState(DogController dog, Animator animator) : base(dog, animator) { }
    public override void OnEnter()
    {
        Debug.Log("Roam state entered");
    }

    public override void Update()
    {
        dog.IdleRoam();
    }

}
