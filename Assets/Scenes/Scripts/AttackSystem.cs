using Unity.Entities;
using Unity.Transforms;

public class AttackSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll(typeof(AttackFaze)).ForEach(
            (Entity alliedUnit, ref Translation translation, ref HasTarget hasTarget) =>
            {
                PostUpdateCommands.DestroyEntity(hasTarget.targetEnity);
                PostUpdateCommands.RemoveComponent(alliedUnit, typeof(HasTarget));
            });
    }
}