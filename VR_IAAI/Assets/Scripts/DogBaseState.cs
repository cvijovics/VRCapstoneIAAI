using UnityEngine;

public abstract class DogBaseState
{
    public abstract void EnterState(DogStateManager dog);

    public abstract void UpdateState(DogStateManager dog);

    public abstract void OnCollisionEnter(DogStateManager dog);
}
