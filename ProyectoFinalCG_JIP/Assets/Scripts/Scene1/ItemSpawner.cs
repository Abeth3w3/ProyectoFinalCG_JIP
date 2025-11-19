using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableItem
    {
        public GameObject itemPrefab;
        public float spawnWeight = 1f;
    }

    [Header("Configuración de Spawn")]
    public List<SpawnableItem> spawnableItems = new List<SpawnableItem>();
    public int maxItems = 10;
    public float spawnInterval = 5f;
    public float collisionCheckRadius = 0.5f;

    [Header("Límites del Mapa (Relativos al Spawner)")]
    public float minX = -5f;  // Ahora son relativos a la posición del spawner
    public float maxX = 5f;
    public float minZ = -5f;
    public float maxZ = 5f;
    public float spawnHeight = 2f;

    [Header("Visualización Gizmos")]
    public Color gizmoColor = Color.green;
    public bool showSpawnArea = true;
    public bool showSpawnPoints = false;

    [Header("Opciones Avanzadas")]
    public LayerMask itemCollisionMask;

    private List<GameObject> spawnedItems = new List<GameObject>();
    private float timer = 0f;

    void Start()
    {
        int initialAmount = Mathf.FloorToInt(maxItems * 0.5f);
        for (int i = 0; i < initialAmount; i++)
        {
            SpawnItem();
        }
    }

    void Update()
    {
        spawnedItems.RemoveAll(item => item == null);

        timer += Time.deltaTime;

        if (timer >= spawnInterval && spawnedItems.Count < maxItems)
        {
            SpawnItem();
            timer = 0f;
        }
    }

    void SpawnItem()
    {
        if (spawnableItems.Count == 0)
        {
            Debug.LogWarning("ItemSpawner: No hay ítems configurados.");
            return;
        }

        GameObject itemToSpawn = GetRandomItem();
        if (itemToSpawn == null) return;

        Vector3 spawnPosition = GetSafeSpawnPosition();
        if (spawnPosition == Vector3.negativeInfinity)
        {
            Debug.LogWarning("ItemSpawner: No se pudo encontrar un lugar seguro para spawnear.");
            return;
        }

        GameObject newItem = Instantiate(itemToSpawn, spawnPosition, Quaternion.identity);
        spawnedItems.Add(newItem);

        Debug.Log($"Item '{itemToSpawn.name}' generado en {spawnPosition}");
    }

    GameObject GetRandomItem()
    {
        float totalWeight = 0f;
        foreach (var item in spawnableItems)
            totalWeight += item.spawnWeight;

        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var item in spawnableItems)
        {
            currentWeight += item.spawnWeight;
            if (randomValue <= currentWeight)
                return item.itemPrefab;
        }

        return spawnableItems[0].itemPrefab;
    }

    Vector3 GetSafeSpawnPosition()
    {
        int attempts = 20;

        while (attempts > 0)
        {
            // Calcular posición RELATIVA al spawner
            float x = transform.position.x + Random.Range(minX, maxX);
            float z = transform.position.z + Random.Range(minZ, maxZ);

            Vector3 pos = new Vector3(x, spawnHeight, z);

            if (!Physics.CheckSphere(pos, collisionCheckRadius, itemCollisionMask))
                return pos;

            attempts--;
        }

        return Vector3.negativeInfinity;
    }

    void OnDrawGizmos()
    {
        if (!showSpawnArea) return;

        Gizmos.color = gizmoColor;

        // Calcular centro RELATIVO al spawner
        Vector3 center = transform.position + new Vector3(
            (minX + maxX) / 2f,
            spawnHeight,
            (minZ + maxZ) / 2f
        );

        Vector3 size = new Vector3(
            Mathf.Abs(maxX - minX),
            0.1f,
            Mathf.Abs(maxZ - minZ)
        );

        // Dibujar área principal
        Gizmos.DrawWireCube(center, size);

        // Dibujar área semitransparente
        Color semiTransparent = gizmoColor;
        semiTransparent.a = 0.1f;
        Gizmos.color = semiTransparent;
        Gizmos.DrawCube(center, size);

        // Dibujar líneas desde el spawner hasta las esquinas
        Gizmos.color = Color.red;
        Vector3[] corners = new Vector3[]
        {
            transform.position + new Vector3(minX, spawnHeight, minZ),
            transform.position + new Vector3(minX, spawnHeight, maxZ),
            transform.position + new Vector3(maxX, spawnHeight, maxZ),
            transform.position + new Vector3(maxX, spawnHeight, minZ)
        };

        // Dibujar línea desde el spawner hasta el centro del área
        Gizmos.DrawLine(transform.position, center);

        // Dibujar el contorno del área
        Gizmos.color = gizmoColor;
        for (int i = 0; i < corners.Length; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i + 1) % corners.Length]);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualización adicional cuando está seleccionado
        Gizmos.color = Color.cyan;

        Vector3 center = transform.position + new Vector3(
            (minX + maxX) / 2f,
            spawnHeight,
            (minZ + maxZ) / 2f
        );

        Vector3 size = new Vector3(
            Mathf.Abs(maxX - minX),
            0.1f,
            Mathf.Abs(maxZ - minZ)
        );

        Gizmos.DrawWireCube(center, size);

        // Dibujar esferas en las esquinas
        float cornerSphereRadius = 0.3f;
        Vector3[] corners = new Vector3[]
        {
            transform.position + new Vector3(minX, spawnHeight, minZ),
            transform.position + new Vector3(minX, spawnHeight, maxZ),
            transform.position + new Vector3(maxX, spawnHeight, maxZ),
            transform.position + new Vector3(maxX, spawnHeight, minZ)
        };

        foreach (Vector3 corner in corners)
        {
            Gizmos.DrawSphere(corner, cornerSphereRadius);
        }
    }
}