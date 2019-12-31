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
            Entities.WithAll<EnemyAttackData>().ForEach((Entity targetEntity, ref EnemyAttackData attackData) =>
                {
                    attackData.timer += Time.deltaTime;

                    Entity attacker = attackData.source;
                    Entity target = attackData.target;

                    HealthData componentData = activeEntityManager.GetComponentData<HealthData>(target);

                    if (attackData.timer >= attackData.frequency)
                    {
                        attackData.timer = 0f;

                        float newHp = componentData.health - attackData.damage;
                        
                        activeEntityManager.SetComponentData(target, new HealthData{health = newHp});
                    }
                });
        }
    }
}