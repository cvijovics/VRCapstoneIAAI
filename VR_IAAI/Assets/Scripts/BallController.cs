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
}
