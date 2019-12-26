using Unity.Entities;

namespace Scenes.Components
{
    public struct HasTarget : IComponentData
    {
        public Entity targetEnity;
    }
}