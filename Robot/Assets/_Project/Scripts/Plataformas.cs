using UnityEngine;

public class Plataformas : MonoBehaviour
{
   [Header("Configuración de Plataforma")]
    public float disableTime = 0.5f;
    
    [Header("Tamaño de Triggers")]
    public float topAreaHeight = 0.5f;
    public float bottomAreaHeight = 1.5f;
    
    private Collider platformCollider;
    private GameObject topTrigger;
    private GameObject bottomTrigger;
    
    void Start()
    {
        platformCollider = GetComponent<Collider>();
        CreateTriggers();
    }
    
    void Update()
    {
        // Verificar inputs si los triggers están activos
        if (IsPlayerInTopTrigger() && Input.GetKey(KeyCode.S))
        {
            TogglePlatform(false);
        }
        
        if (IsPlayerInBottomTrigger() && Input.GetButtonDown("Jump"))
        {
            TogglePlatform(false);
        }
    }
    
    void CreateTriggers()
    {
        Vector3 platformSize = GetComponent<Collider>().bounds.size;
        
        // Trigger superior
        topTrigger = CreateTrigger("TopTrigger", 
            new Vector3(0, platformSize.y / 2 + topAreaHeight / 2, 0),
            new Vector3(platformSize.x, topAreaHeight, platformSize.z));
        
        // Trigger inferior
        bottomTrigger = CreateTrigger("BottomTrigger",
            new Vector3(0, -7  - bottomAreaHeight / 2, 0),
            new Vector3(platformSize.x, bottomAreaHeight, platformSize.z));
    }
    
    GameObject CreateTrigger(string name, Vector3 localPosition, Vector3 size)
    {
        GameObject trigger = new GameObject(name);
        trigger.transform.SetParent(transform);
        trigger.transform.localPosition = localPosition;
        
        BoxCollider collider = trigger.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = size;
        
        trigger.AddComponent<SimplePlatformTrigger>().parentPlatform = this;
        
        return trigger;
    }
    
    bool IsPlayerInTopTrigger()
    {
        return topTrigger.GetComponent<SimplePlatformTrigger>().playerInside;
    }
    
    bool IsPlayerInBottomTrigger()
    {
        return bottomTrigger.GetComponent<SimplePlatformTrigger>().playerInside;
    }
    
    void TogglePlatform(bool enabled)
    {
        platformCollider.enabled = enabled;
        if (!enabled)
        {
            Invoke("EnablePlatform", disableTime);
        }
    }
    
    void EnablePlatform()
    {
        platformCollider.enabled = true;
    }
}

public class SimplePlatformTrigger : MonoBehaviour
{
    public Plataformas parentPlatform;
    public bool playerInside = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}
