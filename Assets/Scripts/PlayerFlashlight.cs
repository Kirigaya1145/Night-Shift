using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{
    public bool HasFlashLight {  get; private set; }
    public float RemainingTime { get; private set; }
    public Light flashLight;

    void Start()
    {
        if (flashLight != null) flashLight.enabled = false;
    }
    public void PickUp(float duration)
    {
        HasFlashLight = true;
        RemainingTime = duration;
        if (flashLight != null) flashLight.enabled = true;
    }
    void Update()
    {
        if (!HasFlashLight) return;
        RemainingTime -= Time.deltaTime;
        if (RemainingTime <= 0f)
        {
            HasFlashLight = false;
            RemainingTime = 0f;
            if (flashLight != null) flashLight.enabled = false;
        }
    }
}
