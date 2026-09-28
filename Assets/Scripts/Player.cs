using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5;
    Vector3 moveVec = Vector3.zero;
    Rigidbody rb;
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(moveVec.x * moveSpeed, rb.linearVelocity.y, moveVec.z * moveSpeed);

        Vector3 flat = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        // Bool "Run": true kalau sedang bergerak
        if (animator != null) animator.SetBool("Run", flat.sqrMagnitude > 0.1f);

        // Putar model menghadap arah gerak
        if (flat.sqrMagnitude > 0.05f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flat.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.fixedDeltaTime);
        }
    }

    // Dipanggil otomatis oleh Player Input component (Behavior: Send Messages)
    // saat action "Move" di Input Actions asset ter-trigger
    public void OnMove(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();
        // Input Y (dari WASD/stick) dipetakan ke sumbu Z dunia 3D,
        // sumbu Y dibiarkan 0 karena itu urusan gravity/Rigidbody, bukan input gerak
        moveVec = new Vector3(inputVec.x, 0, inputVec.y);
    }
}
