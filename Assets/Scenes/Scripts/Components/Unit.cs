using Unity.Entities;

namespace Scenes.Components
{
    public struct Unit : IComponentData
    {
        public float health;
        public int damage;
    }
}