using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // Player
    public float smoothSpeed = 8f; 

    Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Offset diambil dari posisi kamera sekarang di Scene,
        // jadi tinggal atur posisi kamera sesuka hati sebelum Play
        offset = transform.position - target.position;
    }

    // LateUpdate dipanggil setelah semua gerakan selesai, supaya kamera tidak bergetar
    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        if (smoothSpeed <= 0f)
        {
            transform.position = desiredPosition;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
        // Rotasi sengaja tidak diubah, jadi sudut pandang tetap
    }
}