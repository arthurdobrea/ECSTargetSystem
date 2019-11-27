using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameHadnlerCF : MonoBehaviour
{
    [SerializeField] private Material unitMaterial;
    [SerializeField] private Material targetMaterial;
    [SerializeField] private Mesh quadMesh;


    private static EntityManager entityManager;


    private void Start()
    {
        entityManager = World.Active.EntityManager;

        for (int i = 0; i < 1; i++)
        {
            SpawnUnitEntity();
        }

        for (int i = 0; i < 3; i++)
        {
            SpawnTargetEntity();
        }
    }

    private float spawnTargetTimer;

    private void Update()
    {
//        spawnTargetTimer -= Time.deltaTime;
//        if (spawnTargetTimer < 0)
//        {
//            spawnTargetTimer = .1f;
//
//            for (int i = 0; i < 10; i++)
//            {
//                SpawnTargetEntity();
//            }
//        }
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
            typeof(Unit)
        );
        SetEntityComponentData(entity, position, quadMesh, unitMaterial);
        entityManager.SetComponentData(entity, new Scale {Value = 1.5f});
    }

    private void SpawnTargetEntity()
    {
        Entity entity = entityManager.CreateEntity(
            typeof(Translation),
            typeof(LocalToWorld),
            typeof(RenderMesh),
            typeof(Scale),
            typeof(Target)
        );
        SetEntityComponentData(entity,
            new float3(Random.Range(-8, +8f), Random.Range(-5, +5f), 0), quadMesh,
            targetMaterial);
        entityManager.SetComponentData(entity, new Scale {Value = .5f});
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
}

public struct Target : IComponentData
{
}

public struct HasTarget : IComponentData
{
    public Entity targetEnity;
}

public class HasTargetDebug : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref Translation translation, ref HasTarget hasTarget) =>
        {
            if (World.Active.EntityManager.Exists(hasTarget.targetEnity))
            {
                Translation targetTranslation =
                    World.Active.EntityManager.GetComponentData<Translation>(hasTarget.targetEnity);
                Debug.DrawLine(translation.Value, targetTranslation.Value);
            }
        });
    }
}