using Unity.Entities;

namespace Scenes.Components
{
    public struct Unit : IComponentData
    {
        public bool isActive;
        public float health;
        public int damage;
    }
}