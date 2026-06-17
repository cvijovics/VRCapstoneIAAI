using System.Collections.Generic;
using System.Collections;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class BallController : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private Transform ballPosition;
    [SerializeField] private AudioController audioController;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip dropClip;
    [SerializeField] private DogController dogController;
    private bool isAttached = false;
    public void Update()
    {
            if (isAttached)
            {
                ball.transform.position = ballPosition.position;
                ball.transform.rotation = ballPosition.rotation;
            }
    }
    public void AttachObject()
    {
        ball.transform.SetParent(ballPosition);
        ball.transform.localPosition = Vector3.zero;
        ball.transform.localRotation = Quaternion.identity;
        ball.GetComponent<Rigidbody>().isKinematic = true;
        isAttached = true;
    }

    public void DetachObject()
    {
        ball.transform.SetParent(null, true);
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        ballRb.isKinematic = false;
        ballRb.useGravity = true;
        isAttached = false;
    }

    public bool IsAttached => isAttached;

    public void ForceDetach()
    {
        Debug.Log($"ForceDetach called. isAttached: {isAttached}");
        StartCoroutine(ForceDetachAfterDelay());
    }

    IEnumerator ForceDetachAfterDelay()
    {
        yield return null;
        DetachObject();
        yield return null;
        ball.transform.SetParent(null, true); //again for good measure
        if (dogController != null)
        {
            dogController.OnBallTaken();
        }
    }


    public void PlayPickupSound()
    {
        audioController.PlaySFX(pickupClip, ball.transform, 0.4f);
    }

    public void PlayDropSound()
    {
        audioController.PlaySFX(dropClip, ball.transform, 0.4f);
    }
}
