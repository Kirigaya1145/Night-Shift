using System.Collections;
using UnityEngine;

public class HearingSensor : MonoBehaviour
{
    public float radius = 10f;      // jangkauan pendengaran (bola, tanpa arah)
    public LayerMask noiseMask;     // layer "Noise"

    // Data yang dibaca oleh Decision Making
    public bool HeardSomething { get; private set; }
    public Vector3 LastHeardPosition { get; private set; }
    public float TimeSinceLastHeard => Time.time - lastHeardTime;

    float lastHeardTime = -999f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SensoryRoutine());
    }

    IEnumerator SensoryRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return wait;
            HearingCheck();
        }
    }

    void HearingCheck()
    {
        // Suara dimodelkan sebagai bola (tidak butuh sudut, tidak terhalang tembok)
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, noiseMask, QueryTriggerInteraction.Collide);

        if (hits.Length > 0)
        {
            // Ambil suara yang paling dekat
            Collider nearest = hits[0];
            float bestDist = (nearest.transform.position - transform.position).sqrMagnitude;
            for (int i = 1; i < hits.Length; i++)
            {
                float d = (hits[i].transform.position - transform.position).sqrMagnitude;
                if (d < bestDist)
                {
                    bestDist = d;
                    nearest = hits[i];
                }
            }

            HeardSomething = true;
            LastHeardPosition = nearest.transform.position;
            lastHeardTime = Time.time;
            return;
        }

        HeardSomething = false;
    }
}