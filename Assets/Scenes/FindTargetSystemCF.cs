using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Scenes
{
    public class FindTargetSystemCF : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithNone<HasTarget>().WithAll<Unit>().ForEach((Entity entity, ref Translation unitTransalation) =>
            {
                float3 unitPosition = unitTransalation.Value;
                Entity closestTargetEntity = Entity.Null;
                float3 closestTargetPosition = float3.zero;

                Entities.WithAll<Target>().ForEach((Entity targetEntity, ref Translation targetTranslation) =>
                {
                    if (closestTargetEntity == Entity.Null)
                    {
                        closestTargetEntity = targetEntity;
                        closestTargetPosition = targetTranslation.Value;
                    }
                    else
                    {
                        if (math.distance(unitPosition, targetTranslation.Value) <
                            math.distance(unitPosition, closestTargetPosition))
                        {
                            closestTargetEntity = targetEntity;
                            closestTargetPosition = targetTranslation.Value;
                        }
                    }
                });
                if (closestTargetEntity != Entity.Null)
                {
                    PostUpdateCommands.AddComponent(entity, new HasTarget {targetEnity = closestTargetEntity});
                }
            });
        }
    }
}