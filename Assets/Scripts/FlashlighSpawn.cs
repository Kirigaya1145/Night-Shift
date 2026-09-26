using UnityEngine;

public class FlashlighSpawn : MonoBehaviour
{
    public static FlashlighSpawn Instance {  get; private set; }

    public GameObject flashlightPrefab;
    public Transform[] spawnPoints;
    public float respawnDelay = 20f;

    [SerializeField] float respawnTimer = -1f;
    bool isWaitingRespawn = false;
    void Awake()
    { 
        Instance = this;
    }
    void Start()
    {
        
    }
    void Update()
    {
        if (!isWaitingRespawn) return;
        respawnTimer -= Time.deltaTime;
        if (respawnTimer <= 0f) 
        {
            isWaitingRespawn = false;
            SpawnFlashLight();
        }
    }
    public void NotifyPickUp()
    {
        respawnTimer = respawnDelay;
        isWaitingRespawn=true;
    }
    void SpawnFlashLight()
    {
        if (spawnPoints.Length == 0 || flashlightPrefab == null) return;
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(flashlightPrefab, point.position, Quaternion.identity);
    }
}
