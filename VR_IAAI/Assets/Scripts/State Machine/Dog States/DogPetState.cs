using UnityEngine;

public class DogPetState : DogBaseState
{
    public DogPetState(DogController dog, Animator animator) : base(dog, animator) { }
    public override void OnEnter()
    {
        Debug.Log("Pet state entered!");
        // Fade animation
    }

    public override void Update()
    {
        // Play pet logic
    }

    public override void FixedUpdate()
    {
        dog.PetResponse();
    }

}
