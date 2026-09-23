using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySteerFlee : MonoBehaviour
{
    public float moveSpeed = 5;
    Vector3 moveVec = Vector3.zero;
    Rigidbody rb;
    public GameObject target;

    public float maxSpeed = 7;
    // Jarak minimal supaya dianggap "aman", di luar jarak ini nggak perlu lari lagi
    // (opsional, boleh dibiarkan sangat besar kalau mau selalu lari selama target ada di scene)
    public float panicDistance = 15;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SteeringFlee();

        // Adds force to the object (Y dibiarkan 0 biar nggak ganggu gravity)
        rb.AddForce(moveVec * moveSpeed);

        // Limits the maximum velocity, cuma di bidang X-Z
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        flatVelocity = Vector3.ClampMagnitude(flatVelocity, maxSpeed);
        rb.linearVelocity = new Vector3(flatVelocity.x, rb.linearVelocity.y, flatVelocity.z);
    }

    void SteeringFlee()
    {
        // Calculate direction (kebalikan dari Seek: dari target ke diri sendiri)
        Vector3 direction = transform.position - target.transform.position;
        direction.y = 0;

        // Kalau target sudah cukup jauh (di luar panicDistance), berhenti lari
        if (direction.magnitude > panicDistance)
        {
            moveVec = Vector3.zero;
            moveSpeed = 0;
            return;
        }

        moveSpeed = maxSpeed;

        // Normalize direction to get Unit vector
        moveVec = direction.normalized;
    }
}