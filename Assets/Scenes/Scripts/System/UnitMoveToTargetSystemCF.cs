using Scenes.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class UnitMoveToTargetSystemCF : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity targetUnitEntity, ref HasTarget hasTarget, ref Translation translation) =>
        {
            MovingFaze targetUnitEntityData = World.Active.EntityManager.GetComponentData<MovingFaze>(targetUnitEntity);
            if (World.Active.EntityManager.Exists(hasTarget.targetEnity) && targetUnitEntityData.isActive)
            {
                Translation targetTranslation =
                    World.Active.EntityManager.GetComponentData<Translation>(hasTarget.targetEnity);

                float3 targetDir = math.normalize(targetTranslation.Value - translation.Value);
                float moveSpeed = 5f;
                translation.Value += targetDir * moveSpeed * Time.deltaTime;

                if (math.distance(translation.Value, targetTranslation.Value) < .2f)
                {
                    PostUpdateCommands.AddComponent(targetUnitEntity, typeof(AttackFaze));
                    PostUpdateCommands.SetComponent(targetUnitEntity, new MovingFaze{isActive = false} );
                }
            }
            else
            {
                PostUpdateCommands.RemoveComponent(targetUnitEntity, typeof(HasTarget));
            }
        });
    }
}