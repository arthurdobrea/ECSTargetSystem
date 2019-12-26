using Unity.Entities;

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
                Unit unitData = activeEntityManager.GetComponentData<Unit>(targetEntity);
                Unit enemyData = activeEntityManager.GetComponentData<Unit>(hasTarget.targetEnity);
                float enemyDataHealth = enemyData.health;
                float unitDataDamage = unitData.damage;
                
                if (enemyDataHealth >= 0)
                {
                        
                }
            });
        }
    }
}