using Scenes.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class FindTargetJobSystem : JobComponentSystem
{
    public struct EntityWithPosition
    {
        public Entity entity;
        public float3 position;
    }

    [RequireComponentTag(typeof(Team1))]
    [BurstCompile]
    [ExcludeComponent(typeof(HasTarget))]
    public struct Team1FindTargetJob : IJobForEachWithEntity<Translation>
    {
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<EntityWithPosition> targetArray;
        public NativeArray<Entity> closestTargetNativeArray;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation entityTranslation)
        {
            float3 unitPosition = entityTranslation.Value;
            Entity closestTargetEntity = Entity.Null;
            float3 closestTargetPosition = float3.zero;

            for (int i = 0; i < targetArray.Length; i++)
            {
                EntityWithPosition targetEntityWithPosition = targetArray[i];

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
                        closestTargetPosition = targetEntityWithPosition.position;
                    }
                }
            }

            if (closestTargetEntity != Entity.Null)
            {
                //entityCommandBuffer.AddComponent(index, entity, new HasTarget {targetEnity = closestTargetEntity});
            }

            if (!World.Active.EntityManager.Exists(closestTargetEntity))
            {
                //entityCommandBuffer.RemoveComponent(index, entity, typeof(HasTarget));
            }
            
            closestTargetNativeArray[index] = closestTargetEntity;
        }
       
    }
    

    [RequireComponentTag(typeof(Team2))]
    [ExcludeComponent(typeof(HasTarget))]
    public struct Team2FindTargetJob : IJobForEachWithEntity<Translation>
    {
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<EntityWithPosition> targetArray;
        public NativeArray<Entity> closestTargetNativeArray;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation entityTranslation)
        {
            float3 unitPosition = entityTranslation.Value;
            Entity closestTargetEntity = Entity.Null;
            float3 closestTargetPosition = float3.zero;

            for (int i = 0; i < targetArray.Length; i++)
            {
                EntityWithPosition targetEntityWithPosition = targetArray[i];

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
                        closestTargetPosition = targetEntityWithPosition.position;
                    }
                }
            }

            if (closestTargetEntity != Entity.Null)
            {
               // entityCommandBuffer.AddComponent(index, entity, new HasTarget {targetEnity = closestTargetEntity});
            }

            if (!World.Active.EntityManager.Exists(closestTargetEntity))
            {
                //entityCommandBuffer.RemoveComponent(index, entity, typeof(HasTarget));
            }
            closestTargetNativeArray[index] = closestTargetEntity;
        }
    }

    private EndSimulationEntityCommandBufferSystem entityCommandBufferSystem;

    protected override void OnCreate()
    {
        entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    private struct AddCopmonentJob :IJobForEachWithEntity<Translation>
    {
        [DeallocateOnJobCompletion] [ReadOnly]  public NativeArray<Entity> closestNativeArray;
        public EntityCommandBuffer.Concurrent entityCommandBuffer;
        public void Execute(Entity entity, int index, ref Translation c0)
        {
            if (closestNativeArray[index] != Entity.Null)
            {
                entityCommandBuffer.AddComponent(index,entity,new HasTarget {targetEnity = closestNativeArray[index]});
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityQuery team1entityQueries = GetEntityQuery(typeof(Team1), ComponentType.ReadOnly<Translation>());
        NativeArray<Entity> team1targetEntityArray = team1entityQueries.ToEntityArray(Allocator.TempJob);
        NativeArray<Translation> team1targetTranslationArray = team1entityQueries.ToComponentDataArray<Translation>(Allocator.TempJob);
        NativeArray<EntityWithPosition> team1targetArray = new NativeArray<EntityWithPosition>(team1targetEntityArray.Length, Allocator.TempJob);

        EntityQuery team2entityQueries = GetEntityQuery(typeof(Team2), ComponentType.ReadOnly<Translation>());
        NativeArray<Entity> team2targetEntityArray = team2entityQueries.ToEntityArray(Allocator.TempJob);
        NativeArray<Translation> team2targetTranslationArray = team2entityQueries.ToComponentDataArray<Translation>(Allocator.TempJob);
        NativeArray<EntityWithPosition> team2targetArray = new NativeArray<EntityWithPosition>(team2targetEntityArray.Length, Allocator.TempJob);

        for (int i = 0; i < team1targetEntityArray.Length; ++i)
        {
            team1targetArray[i] = new EntityWithPosition
            {
                entity = team1targetEntityArray[i],
                position = team1targetTranslationArray[i].Value
            };
        }

        for (int i = 0; i < team2targetEntityArray.Length; ++i)
        {
            team2targetArray[i] = new EntityWithPosition
            {
                entity = team2targetEntityArray[i],
                position = team2targetTranslationArray[i].Value
            };
        }

        team1targetEntityArray.Dispose();
        team1targetTranslationArray.Dispose();

        team2targetEntityArray.Dispose();
        team2targetTranslationArray.Dispose();

        EntityQuery team1query = GetEntityQuery(typeof(Team1), ComponentType.Exclude<HasTarget>());
        NativeArray<Entity> closestTeam1EntityArray = new NativeArray<Entity>(team1query.CalculateEntityCount(),Allocator.TempJob);
        Team1FindTargetJob target1Job = new Team1FindTargetJob
        {
            targetArray = team1targetArray,
            closestTargetNativeArray = closestTeam1EntityArray
        };

        AddCopmonentJob addCopmonentJobTeam1 = new AddCopmonentJob
        {
            closestNativeArray = closestTeam1EntityArray,
            entityCommandBuffer = entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        EntityQuery team2query = GetEntityQuery(typeof(Team2), ComponentType.Exclude<HasTarget>());
        NativeArray<Entity> closestTeam2EntityArray = new NativeArray<Entity>(team2query.CalculateEntityCount(),Allocator.TempJob);
        Team2FindTargetJob target2Job = new Team2FindTargetJob
        {
            targetArray = team2targetArray,
            closestTargetNativeArray = closestTeam2EntityArray
        };
        
        AddCopmonentJob addCopmonentJobTeam2 = new AddCopmonentJob
        {
            closestNativeArray = closestTeam2EntityArray,
            entityCommandBuffer = entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };
        
        JobHandle jobHandle = target1Job.Schedule(this, inputDeps);
        jobHandle = addCopmonentJobTeam1.Schedule(this, jobHandle);
        
        // jobHandle = target2Job.Schedule(this, jobHandle);
        // jobHandle = addCopmonentJobTeam2.Schedule(this, jobHandle);
        
        entityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}