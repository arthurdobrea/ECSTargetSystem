using Scenes.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class UnitMoveToTargetSystemCF : ComponentSystem
{
    private Entity enemyAttackEntity;
    private EntityManager entityManager = World.Active.EntityManager;
    private EntityArchetype enemyAttackArchetype;

    protected override void OnCreate()
    {
        enemyAttackArchetype = entityManager.CreateArchetype(typeof(EnemyAttackData));
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity unitEntity, ref HasTarget hasTarget, ref Translation translation) =>
        {
            if (entityManager.Exists(hasTarget.targetEnity))
            {
                Translation targetTranslation =
                    entityManager.GetComponentData<Translation>(hasTarget.targetEnity);

                float3 targetDir = math.normalize(targetTranslation.Value - translation.Value);
                float moveSpeed = 5f;

                translation.Value += targetDir * moveSpeed * Time.deltaTime;

                if (math.distance(translation.Value, targetTranslation.Value) < .2f)
                {
                    enemyAttackEntity = entityManager.CreateEntity(enemyAttackArchetype);
                    entityManager.SetComponentData(enemyAttackEntity, new EnemyAttackData
                    {
                        timer = 0f,
                        frequency = 1f,
                        damage = 10,
                        source = unitEntity,
                        target = hasTarget.targetEnity
                    });
                }
            }
            else
            {
                PostUpdateCommands.RemoveComponent(unitEntity, typeof(HasTarget));
            }
        });
    }
}