using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySteerSeek : MonoBehaviour
{
    public float moveSpeed = 5;
    Vector3 moveVec = Vector3.zero;
    Rigidbody rb;
    public GameObject target;

    public float satisfactionRadius = 3;
    public float slowRadius = 10;
    public float maxSpeed = 7;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // DEBUG: cek dua penyebab paling umum kalau script diam total
        if (rb == null)
        {
            Debug.LogError("EnemySteerSeek: Rigidbody TIDAK ketemu di object ini!");
        }
        if (target == null)
        {
            Debug.LogError("EnemySteerSeek: Target belum di-assign di Inspector!");
        }
    }

    void FixedUpdate()
    {
        SteeringSeek();

        rb.AddForce(moveVec * moveSpeed);

        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        flatVelocity = Vector3.ClampMagnitude(flatVelocity, maxSpeed);
        rb.linearVelocity = new Vector3(flatVelocity.x, rb.linearVelocity.y, flatVelocity.z);

        // DEBUG: hapus/comment baris ini lagi kalau udah selesai debugging
        Debug.Log("moveVec=" + moveVec + " | moveSpeed=" + moveSpeed +
                   " | velocity=" + rb.linearVelocity + " | pos=" + transform.position);
    }

    void SteeringSeek()
    {
        // Calculate direction
        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0;

        // Arriving
        // Check Satisfaction Radius
        if (direction.magnitude < satisfactionRadius)
        {
            direction = Vector3.zero;
            moveSpeed = 0;
        }
        // Check Slow Radius
        else if (direction.magnitude < slowRadius)
        {
            moveSpeed = maxSpeed * direction.magnitude / slowRadius;
        }
        else
        {
            moveSpeed = maxSpeed;
        }

        // Normalize direction to get Unit vector
        moveVec = direction.normalized;
    }
}