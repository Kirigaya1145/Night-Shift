using System.Collections;
using UnityEngine;

public class SightSensor : MonoBehaviour
{
    public float radius = 14f;
    [Range(0, 360)] public float angle = 90f;   // lebar kerucut pandang (derajat)
    public float eyeHeight = 1.5f;              // tinggi mata dari pivot (kaki)

    public LayerMask targetMask;                // layer target (Player)
    public LayerMask obstacleMask;              // layer penghalang (Wall, Door)

    // Data yang dibaca oleh Decision Making
    public Transform Target { get; private set; }
    public bool TargetSensed { get; private set; }
    public Vector3 LastSensedPosition { get; private set; }
    public float TimeSinceLastSensed => Time.time - lastSensedTime;

    float lastSensedTime = -999f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SensoryRoutine());
    }

    // Dicek 5 kali per detik, bukan tiap frame (lebih hemat)
    IEnumerator SensoryRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return wait;
            SensorCheck();
        }
    }

    void SensorCheck()
    {
        // 1. Ada target di dalam radius bola?
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, targetMask);
        if (hits.Length > 0)
        {
            Transform t = hits[0].transform;

            // 2. Apakah target ada di dalam kerucut pandang? (sudut dihitung di bidang X-Z)
            Vector3 flatDir = t.position - transform.position;
            flatDir.y = 0;
            flatDir.Normalize();

            if (Vector3.Angle(transform.forward, flatDir) < angle / 2f)
            {
                // 3. Apakah ada penghalang di antara mata dan target?
                Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
                Vector3 targetPos = t.position + Vector3.up * eyeHeight;
                Vector3 toTarget = targetPos - eyePos;

                if (!Physics.Raycast(eyePos, toTarget.normalized, toTarget.magnitude, obstacleMask))
                {
                    Target = t;
                    TargetSensed = true;
                    LastSensedPosition = t.position;
                    lastSensedTime = Time.time;
                    return;
                }
            }
        }

        // Semua kasus lain = gagal merasakan target
        TargetSensed = false;
    }
}