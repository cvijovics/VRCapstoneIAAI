using System.Collections.Generic;
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

    public void AttachObject()
    {
        ball.transform.SetParent(ballPosition);
        ball.transform.localPosition = Vector3.zero;
        ball.transform.localRotation = Quaternion.identity;
        ball.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void DetachObject()
    {
        ball.transform.parent = null;
        ball.GetComponent<Rigidbody>().isKinematic = false;
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
