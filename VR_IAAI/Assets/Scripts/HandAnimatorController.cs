using UnityEngine;
using UnityEngine.InputSystem;
public class HandAnimatorController : MonoBehaviour
{

    [SerializeField] private InputActionReference gripAction;
    [SerializeField] private InputActionReference triggerAction;

    [SerializeField] private Animator handAnimator;
    // Update is called once per frame

    void Update()
    {
        float triggerVal = triggerAction.action.ReadValue<float>();
        float gripVal = gripAction.action.ReadValue<float>();

        handAnimator.SetFloat("Trigger", triggerVal);
        handAnimator.SetFloat("Grip", gripVal);
    }
}
