using Unity.Entities;

namespace Scenes.Components
{
    public struct EnemyAttackData : IComponentData
    {
        public float timer;
        public float frequency;
        public float damage;
        public Entity source;
        public Entity target;
    }
}