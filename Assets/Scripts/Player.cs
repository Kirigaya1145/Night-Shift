using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5;
    Vector2 rawInput = Vector2.zero;
    Rigidbody rb;
    Animator animator;

    public Transform cameraTransform;
    public float rotateSpeed = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 worldMove = camForward * rawInput.y + camRight * rawInput.x;

        rb.linearVelocity = new Vector3(worldMove.x * moveSpeed, rb.linearVelocity.y, worldMove.z * moveSpeed);

        if (animator != null) animator.SetBool("Run", worldMove.sqrMagnitude > 0.01f);

        if (worldMove.sqrMagnitude > 0.01f && rawInput.y > 0.05f)
        {
            Quaternion targetRot = Quaternion.LookRotation(worldMove.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.fixedDeltaTime);
        }
    }

    public void OnMove(InputValue input)
    {
        rawInput = input.Get<Vector2>();
    }
}