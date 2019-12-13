using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class SpawnSystem : ComponentSystem
{
    [SerializeField] private Material unitMaterial;
    [SerializeField] private Material targetMaterial;
    [SerializeField] private Mesh quadMesh;
    
    private EntityManager _entityManager;
    
    protected override void OnCreate()
    {
        _entityManager = World.Active.EntityManager;
    }

    protected override void OnUpdate()
    {
        Entities.WithAll<HasSpawn>().ForEach((Entity entity,ref HasSpawn hasSpawn, ref Translation translation) =>
        {
            float3 position = translation.Value;

            if (hasSpawn.timer == 2000)
            {
                createEntity();
            }
            else
            {
                hasSpawn.timer += 1;
            }

        });
    }

    private void createEntity()
    {
        Entity entity = _entityManager.CreateEntity(
            typeof(Translation),
            typeof(LocalToWorld),
            typeof(RenderMesh),
            typeof(Scale),
            typeof(Unit),
            typeof(Team)
        );
        
        SetEntityComponentData(entity, new float3(1, 1, 0), quadMesh,
            targetMaterial);
        _entityManager.SetComponentData(entity, new Scale {Value = .5f});
        _entityManager.SetComponentData(entity, new Team {team = 1});
        _entityManager.SetComponentData(entity, new Unit {health = 100, attackDamage = 20, atackspeed = 2, timer = 0});
    }
    private void SetEntityComponentData(Entity entity, float3 spawnPosition, Mesh mesh, Material material)
    {
        _entityManager.SetSharedComponentData<RenderMesh>(entity,
            new RenderMesh
            {
                material = material,
                mesh = mesh,
            }
        );

        _entityManager.SetComponentData<Translation>(entity,
            new Translation
            {
                Value = spawnPosition
            }
        );
    }
}