using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5;
    Vector3 moveVec = Vector3.zero;
    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(moveVec.x * moveSpeed, rb.linearVelocity.y, moveVec.z * moveSpeed);
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
