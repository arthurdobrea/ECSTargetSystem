using Scenes.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Scenes
{
    // public class FindTargetSystemCF : ComponentSystem
    // {
    //     protected override void OnUpdate()
    //     {
    //         Entities.WithNone<HasTarget>().WithAll<Unit>().ForEach((Entity entity, ref Translation unitTransalation) =>
    //         {
    //             Team yourTeam = World.Active.EntityManager.GetComponentData<Team>(entity);
    //
    //             float3 unitPosition = unitTransalation.Value;
    //             Entity closestTargetEntity = Entity.Null;
    //             float3 closestTargetPosition = float3.zero;
    //
    //             Entities.WithAll<Unit>().ForEach((Entity targetEntity, ref Translation targetTranslation) =>
    //             {
    //                 Team enemyTeam = World.Active.EntityManager.GetComponentData<Team>(targetEntity);
    //
    //                 if (enemyTeam.team != yourTeam.team)
    //                 {
    //                     if (closestTargetEntity == Entity.Null)
    //                     {
    //                         closestTargetEntity = targetEntity;
    //                         closestTargetPosition = targetTranslation.Value;
    //                     }
    //                     else
    //                     {
    //                         if (math.distance(unitPosition, targetTranslation.Value) <
    //                             math.distance(unitPosition, closestTargetPosition))
    //                         {
    //                             closestTargetEntity = targetEntity;
    //                             closestTargetPosition = targetTranslation.Value;
    //                         }
    //                     }
    //                 }
    //             });
    //             if (closestTargetEntity != Entity.Null)
    //             {
    //                 PostUpdateCommands.AddComponent(entity,
    //                     new HasTarget {targetEnity = closestTargetEntity});
    //             }
    //
    //             if (!World.Active.EntityManager.Exists(closestTargetEntity))
    //             {
    //                 PostUpdateCommands.RemoveComponent(entity, typeof(HasTarget));
    //             }
    //         });
    //     }
    // }
}

public class FindTargetJobSystem : JobComponentSystem
{
    public struct EntityWithPosition
    {
        public Entity entity;
        public float3 position;
    }

    [RequireComponentTag(typeof(Unit))]
    [ExcludeComponent(typeof(HasTarget))]
    [BurstCompile]
    public struct FindTargetJob : IJobForEachWithEntity<Translation>
    {
        
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<EntityWithPosition> targetArray;
        public EntityCommandBuffer.Concurrent entityCommandBuffer;
        public void Execute(Entity entity, int index, [ReadOnly]ref Translation entityTranslation)
        {
            Team yourTeam = World.Active.EntityManager.GetComponentData<Team>(entity);

            float3 unitPosition = entityTranslation.Value;
            Entity closestTargetEntity = Entity.Null;
            float3 closestTargetPosition = float3.zero;

            for (int i = 0; i < targetArray.Length; i++)
            {
                EntityWithPosition targetEntityWithPosition = targetArray[i];
                Team enemyTeam = World.Active.EntityManager.GetComponentData<Team>(targetEntityWithPosition.entity);

                if (enemyTeam.team != yourTeam.team)
                {
                    if (closestTargetEntity == Entity.Null)
                    {
                        closestTargetEntity = targetEntityWithPosition.entity;
                        closestTargetPosition = targetEntityWithPosition.position;
                    }
                    else
                    {
                        if (math.distance(unitPosition, targetEntityWithPosition.position) <
                            math.distance(unitPosition, closestTargetPosition))
                        {
                            closestTargetEntity = targetEntityWithPosition.entity;
                            closestTargetPosition =targetEntityWithPosition.position;
                        }
                    }
                }
            }
            if (closestTargetEntity != Entity.Null)
            {
                //entityCommandBuffer.AddComponent(index,entity, new HasTarget {targetEnity = closestTargetEntity});
            }

            if (!World.Active.EntityManager.Exists(closestTargetEntity))
            {
                entityCommandBuffer.RemoveComponent(index,entity, typeof(HasTarget));
            }
        }
    }

    private EndSimulationEntityCommandBufferSystem entityCommandBufferSystem;

    protected override void OnCreate()
    {
        entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityQuery entityQueries = GetEntityQuery(typeof(Unit), ComponentType.ReadOnly<Translation>());
        
        NativeArray<Entity> targetEntityArray = entityQueries.ToEntityArray(Allocator.TempJob);
        NativeArray<Translation> targetTranslationArray = entityQueries.ToComponentDataArray<Translation>(Allocator.TempJob);
        
        NativeArray<EntityWithPosition> targetArray = new NativeArray<EntityWithPosition>(targetEntityArray.Length ,Allocator.TempJob);

        for (int i = 0; i < targetEntityArray.Length; ++i)
        {
            targetArray[i] = new EntityWithPosition
            {
                entity = targetEntityArray[i],
                position = targetTranslationArray[i].Value
            };
        }

        targetEntityArray.Dispose();
        targetTranslationArray.Dispose();

        FindTargetJob targetJob = new FindTargetJob
        {
            targetArray = targetArray,
            entityCommandBuffer = entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(), 
        };
        JobHandle jobHandle = targetJob.Schedule(this, inputDeps);
        entityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}