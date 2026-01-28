using UnityEngine;

public class DogStateManager : MonoBehaviour
{
    DogBaseState currentState;
    public DogIdleState idleState = new DogIdleState();
    public DogRunState runState = new DogRunState();
    public DogCatchState catchState = new DogCatchState();
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = idleState;
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(DogBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }
}
