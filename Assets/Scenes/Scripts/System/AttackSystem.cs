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
                if (activeEntityManager.Exists(attackData.target))
                {
                    attackData.timer += Time.deltaTime;

                    Entity source = attackData.source;
                    Entity target = attackData.target;

                    HealthData componentData = activeEntityManager.GetComponentData<HealthData>(target);

                    if (attackData.timer >= attackData.frequency)
                    {
                        attackData.timer = 0f;

                        float newHp = componentData.health - attackData.damage;

                        if (newHp <= 0)
                        {
                            activeEntityManager.RemoveComponent(source, typeof(EnemyAttackData));
                            activeEntityManager.DestroyEntity(target);
                        }
                        else
                        {
                            activeEntityManager.SetComponentData(target, new HealthData {health = newHp});
                        }
                    }
                }
                else
                {
                    activeEntityManager.RemoveComponent(attackData.source, typeof(EnemyAttackData));
                }
            });
        }
    }
}