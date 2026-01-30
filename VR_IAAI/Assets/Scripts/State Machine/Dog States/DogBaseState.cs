using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
public abstract class DogBaseState : IState
{
    
    protected readonly DogController dog;
    protected readonly Animator dogAnimator; 
    protected DogBaseState(DogController dog, Animator dogAnimator)
    {
        this.dog = dog;
        this.dogAnimator = dogAnimator;
    }
    public virtual void OnEnter()
    {
        
    }

    public virtual void Update()
    {
        
    }

    public virtual void FixedUpdate()
    {
        
    }

    public virtual void OnExit()
    {
        
    }
}
