using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySteerWander : MonoBehaviour
{
    public float moveSpeed = 3;
    public float maxSpeed = 7;
    Vector3 moveVec = Vector3.zero;
    Rigidbody rb;

    public float wanderCooldown = 1;
    float lastX = 0;
    float lastZ = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        wanderCooldown -= Time.fixedDeltaTime;
        // Simple timer effect, ganti arah random tiap sekian detik
        if (wanderCooldown <= 0)
        {
            Wandering();
            wanderCooldown = 1;
        }

        // Adds force to the object (Y dibiarkan 0 biar nggak ganggu gravity)
        rb.AddForce(moveVec * moveSpeed);

        // Limits the maximum velocity, cuma di bidang X-Z
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        flatVelocity = Vector3.ClampMagnitude(flatVelocity, maxSpeed);
        rb.linearVelocity = new Vector3(flatVelocity.x, rb.linearVelocity.y, flatVelocity.z);
    }

    void Wandering()
    {
        // Randomly calculate offset (pakai float, bukan int, biar variatif)
        float x = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);

        // Apply the offset to last direction, biar transisi arah nggak patah-patah
        x += lastX;
        z += lastZ;

        Vector3 direction = new Vector3(x, 0, z);

        // Normalize direction to get Unit vector
        moveVec = direction.normalized;
        moveSpeed = maxSpeed;

        // Save last x and z for next steering
        lastX = x;
        lastZ = z;
    }
}