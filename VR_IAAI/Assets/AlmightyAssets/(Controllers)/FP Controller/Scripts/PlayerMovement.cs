using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [Header("Layer Mask")]
    [Tooltip("Which Layers can be walked on?")]
    public LayerMask walkableMask;
    
    [Header("Movement")]
    public float speed = 6f;
    public float jumpHeight = 1.5f;
    
    [Header("Auto Run")]
    public bool useAutoRun = true;
    public KeyCode autoRunKey = KeyCode.R;
    
    [Header("Sprint")]
    public bool useSprint = true;
    public KeyCode sprintKey = KeyCode.LeftShift;

    CharacterController controller;
    Transform groundCheck;
    Vector3 velocity;
    float sprint, gravity;
    bool isGrounded, isRunning;

    void Start() {
        Physics.gravity = Vector3.down * 20;
        controller = GetComponent<CharacterController>();
        groundCheck = transform.Find("GroundCheck");
        gravity = Physics.gravity.y;
        Cursor.visible = false;
    }

    void Update() {
        sprint = Input.GetKey(sprintKey) ? 10f : 0;
        if(useAutoRun && Input.GetKeyDown(autoRunKey)) { isRunning = !isRunning; }
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, walkableMask);
        if(isGrounded && velocity.y < 0) { velocity.y = -2f; }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if(useAutoRun && isRunning) { z = 1f; }
        Vector3 motion = transform.right * x + transform.forward * z;
        controller.Move(motion * (speed + sprint) * Time.deltaTime);
        if(Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

}
