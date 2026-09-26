using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{
    public bool HasFlashLight {  get; private set; }
    public float RemainingTime { get; private set; }

    public void PickUp(float duration)
    {
        HasFlashLight = true;
        RemainingTime = duration;
    }
    void Update()
    {
        if (!HasFlashLight) return;
        RemainingTime -= Time.deltaTime;
        if (RemainingTime <= 0f)
        {
            HasFlashLight = false;
            RemainingTime = 0f;
        }
    }
}
