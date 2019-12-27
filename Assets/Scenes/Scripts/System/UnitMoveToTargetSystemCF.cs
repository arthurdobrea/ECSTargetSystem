﻿using Scenes.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class UnitMoveToTargetSystemCF : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity unitEntity, ref HasTarget hasTarget, ref Translation translation) =>
        {
            if (World.Active.EntityManager.Exists(hasTarget.targetEnity))
            {
                Translation targetTranslation =
                    World.Active.EntityManager.GetComponentData<Translation>(hasTarget.targetEnity);

                float3 targetDir = math.normalize(targetTranslation.Value - translation.Value);
                float moveSpeed = 5f;
                translation.Value += targetDir * moveSpeed * Time.deltaTime;

                if (math.distance(translation.Value, targetTranslation.Value) < .2f)
                {
                    World.Active.EntityManager.SetComponentData(unitEntity,new AttackFaze{});
                    PostUpdateCommands.DestroyEntity(hasTarget.targetEnity);
                    PostUpdateCommands.RemoveComponent(unitEntity, typeof(HasTarget));
                }
            }
            else
            {
                PostUpdateCommands.RemoveComponent(unitEntity, typeof(HasTarget));
            }
        });
    }
}