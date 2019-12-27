using Unity.Entities;
using UnityEngine;

namespace Scenes.Components
{
    public class AttackSystem : ComponentSystem
    {
        private EntityManager activeEntityManager;

        protected override void OnCreate()
        {
            activeEntityManager = World.Active.EntityManager;
        }

        protected override void OnUpdate()
        {
            Entities.WithAll<AttackFaze>().ForEach((Entity targetEntity, ref HasTarget hasTarget) =>
            {
                Debug.Log("enemy health left - ");
                Unit targetEntityData = activeEntityManager.GetComponentData<Unit>(targetEntity);
                Unit enemyEntityData = activeEntityManager.GetComponentData<Unit>(hasTarget.targetEnity);
                float unitDataDamage = targetEntityData.damage;

                // enemyEntityData.health -= unitDataDamage;

                if (enemyEntityData.health >= 0)
                {
                    Debug.Log("enemy health left - " + enemyEntityData.health);
                    PostUpdateCommands.SetComponent(targetEntity,new MovingFaze{isActive = true});
                    PostUpdateCommands.RemoveComponent(targetEntity, typeof(HasTarget));
                    PostUpdateCommands.RemoveComponent(targetEntity, typeof(AttackFaze));
                    PostUpdateCommands.DestroyEntity(hasTarget.targetEnity);
                }
            });
        }
    }
}