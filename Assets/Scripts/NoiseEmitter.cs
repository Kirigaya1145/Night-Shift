using UnityEngine;

// Tempel di object Player (yang punya Rigidbody)
public class NoiseEmitter : MonoBehaviour
{
    public float noiseInterval = 0.5f;      // jeda antar suara langkah
    public float noiseLifetime = 0.6f;      // suara bertahan sebentar supaya sempat terdeteksi sensor (cek tiap 0.2 detik)
    public float minSpeed = 0.5f;           // kecepatan minimal supaya dianggap berjalan
    public string noiseLayerName = "Noise";

    Rigidbody rb;
    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (LayerMask.NameToLayer(noiseLayerName) == -1)
        {
            Debug.LogError("NoiseEmitter: layer '" + noiseLayerName + "' belum dibuat di Tags and Layers!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVelocity.magnitude > minSpeed)
        {
            EmitNoise();
            timer = noiseInterval;
        }
    }

    // Bisa dipanggil dari script lain (misal saat mengambil senter)
    public void EmitNoise()
    {
        int layer = LayerMask.NameToLayer(noiseLayerName);
        if (layer == -1) return;

        GameObject noise = new GameObject("Noise");
        noise.transform.position = transform.position;
        noise.layer = layer;

        SphereCollider col = noise.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 0.3f;

        Destroy(noise, noiseLifetime);
    }
}