using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PrimitiveSpawner : MonoBehaviour
{
    public float spawnInterval = 1.5f;   // раз в сколько секунд спавнить
    public int minObjectsPerSpawn = 2;   // минимум объектов за один раз
    public int maxObjectsPerSpawn = 3;   // максимум объектов за один раз
    public float spawnHeight = 5f;       // насколько выше зоны спавна появятся объекты
    public float fallMultiplier = 2f;    // множитель скорости падения

    private BoxCollider spawnArea;

    void Start()
    {
        spawnArea = GetComponent<BoxCollider>();
        spawnArea.isTrigger = true;
        InvokeRepeating(nameof(SpawnObjects), 1f, spawnInterval);
    }

    void SpawnObjects()
    {
        int count = Random.Range(minObjectsPerSpawn, maxObjectsPerSpawn + 1);

        for (int i = 0; i < count; i++)
        {
            SpawnPrimitive();
        }
    }

    void SpawnPrimitive()
    {
        // случайный тип примитива
        PrimitiveType[] types = { PrimitiveType.Cube, PrimitiveType.Sphere, PrimitiveType.Capsule, PrimitiveType.Cylinder };
        PrimitiveType randomType = types[Random.Range(0, types.Length)];

        // случайная точка в пределах зоны
        Vector3 localPos = new Vector3(
            Random.Range(-spawnArea.size.x / 2, spawnArea.size.x / 2),
            0,
            Random.Range(-spawnArea.size.z / 2, spawnArea.size.z / 2)
        );

        Vector3 worldPos = spawnArea.transform.TransformPoint(localPos);
        worldPos.y += spawnHeight;

        // создаём объект
        GameObject obj = GameObject.CreatePrimitive(randomType);
        obj.transform.position = worldPos;

        // добавляем Rigidbody и ускоряем падение
        Rigidbody rb = obj.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.mass = 1f;
        rb.linearDamping = 0f; // меньше сопротивление воздуха

        // увеличиваем силу падения (ускорение вниз)
        rb.linearVelocity = Vector3.down * 9.81f * fallMultiplier;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        BoxCollider col = GetComponent<BoxCollider>();
        if (col != null)
        {
            Gizmos.matrix = col.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(col.center + Vector3.up * spawnHeight, col.size);
        }
    }
}
