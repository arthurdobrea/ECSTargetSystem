using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameHadnlerCF : MonoBehaviour
{
    [SerializeField] private Material buildingMaterial;
    [SerializeField] private Material targetMaterial;
    [SerializeField] private Mesh quadMesh;
    
    private static EntityManager entityManager;

    private void Start()
    {
        entityManager = World.Active.EntityManager; 
        CreateSpawner(new float3(Random.Range(-8, +8f), Random.Range(-5, +5f), 0));
        return;
        
        for (int i = 0; i < 1; i++)
        {
            SpawnUnitEntity();
        }

        for (int i = 0; i < 1; i++)
        {
            SpawnTargetEntity();
        }
    }

    private float spawnTargetTimer;

    private void Update()
    {
        return;
        spawnTargetTimer -= Time.deltaTime;
        if (spawnTargetTimer < 0)
        {
            spawnTargetTimer = .1f;

            for (int i = 0; i < 1; i++)
            {
                SpawnUnitEntity();
                SpawnTargetEntity();
            }
        }
    }

    private void SpawnUnitEntity()
    {
        SpawnUnitEntity(new float3(Random.Range(-8, +8f), Random.Range(-5, +5f), 0));
    }

    private void SpawnUnitEntity(float3 position)
    {
        Entity entity = entityManager.CreateEntity(
            typeof(Translation),
            typeof(LocalToWorld),
            typeof(RenderMesh),
            typeof(Scale),
            typeof(Unit),
            typeof(Team)
        );
        SetEntityComponentData(entity, position, quadMesh, buildingMaterial);
        entityManager.SetComponentData(entity, new Scale {Value = 1.5f});
        entityManager.SetComponentData(entity, new Team {team = 2});
        entityManager.SetComponentData(entity, new Unit {health = 200, attackDamage = 10, atackspeed = 2, timer = 0});
    }

    private void CreateSpawner(float3 position)
    {
        Entity entity = entityManager.CreateEntity(
            typeof(Translation),
            typeof(LocalToWorld),
            typeof(RenderMesh),
            typeof(Scale),
            typeof(HasSpawn)
        );
        SetEntityComponentData(entity, position, quadMesh, buildingMaterial);
        entityManager.SetComponentData(entity,new HasSpawn());
    }

    private void SpawnTargetEntity()
    {
        Entity entity = entityManager.CreateEntity(
            typeof(Translation),
            typeof(LocalToWorld),
            typeof(RenderMesh),
            typeof(Scale),
            typeof(Unit),
            typeof(Team)
        );
        SetEntityComponentData(entity, new float3(Random.Range(-8, +8f), Random.Range(-5, +5f), 0), quadMesh,
            targetMaterial);
        entityManager.SetComponentData(entity, new Scale {Value = .5f});
        entityManager.SetComponentData(entity, new Team {team = 1});
        entityManager.SetComponentData(entity, new Unit {health = 100, attackDamage = 20, atackspeed = 2, timer = 0});
    }

    private void SetEntityComponentData(Entity entity, float3 spawnPosition, Mesh mesh, Material material)
    {
        entityManager.SetSharedComponentData<RenderMesh>(entity,
            new RenderMesh
            {
                material = material,
                mesh = mesh,
            }
        );

        entityManager.SetComponentData<Translation>(entity,
            new Translation
            {
                Value = spawnPosition
            }
        );
    }
}

public struct Unit : IComponentData
{
    public float health;
    public float attackDamage;
    public float timer;
    public float atackspeed;
}

public struct Team : IComponentData
{
    public int team;
}

public struct Target : IComponentData
{
}

public struct HasTarget : IComponentData
{
    public Entity targetEnity;
}

public struct AttackFaze : IComponentData
{
    
}

public struct HasSpawn : IComponentData
{
    public float timer;
}