using System.Collections.Generic;
using Scenes;
using Scenes.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class SpawnSystem : ComponentSystem
{
    private EntityQuery spawnerQuery;
    private static EntityManager entityManager;

    private Material unitMaterial;
    private Material tragetMaterial;
    private Mesh mesh;
    private EnemySpawnModel[] spawner;


    //??
    private readonly List<float> time = new List<float>();


    protected override void OnCreate()
    {
        entityManager = World.Active.EntityManager;
        spawnerQuery = GetEntityQuery(ComponentType.ReadOnly<EnemySpawnModel>());
    }

    protected override void OnUpdate()
    {
        if(spawner == null) spawner = spawnerQuery.ToComponentArray<EnemySpawnModel>();
        
        unitMaterial = spawner[0].UnitMaterial;
        tragetMaterial = spawner[0].TargetMaterial;
        mesh = spawner[0].mesh;
        
        float deltaTime = Time.deltaTime;

        for (int i = 0; i < spawner.Length; i++)
        {
            //??
            if (time.Count < i + 1)
                time.Add(0f);

            time[i] += deltaTime;

            Debug.Log("before spawning");
            if (time[i] >= spawner[i].SpawnTime)
            {
                Debug.Log("instantiate");
                SpawnUnitEntity();
                SpawnTargetEntity();
                time[i] = 0f;
            }
        }
    }

    private void SpawnUnitEntity()
    {
        SpawnUnitEntity(new float3(0, 0, 0));
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
        SetEntityComponentData(entity, position, unitMaterial);
        entityManager.SetComponentData(entity, new Scale {Value = 1.5f});
        entityManager.SetComponentData(entity, new Team {team = 2});
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
        SetEntityComponentData(entity, new float3(5, 5, 0), tragetMaterial);
        entityManager.SetComponentData(entity, new Scale {Value = .5f});
        entityManager.SetComponentData(entity, new Team {team = 1});
    }

    private void SetEntityComponentData(Entity entity, float3 spawnPosition, Material material)
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