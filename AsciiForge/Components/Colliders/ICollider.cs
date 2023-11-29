using AsciiForge.Engine;
using AsciiForge.Engine.Ecs;

namespace AsciiForge.Components.Colliders
{
    public interface ICollider
    {
        public bool PointMeeting(ICollider other, Vector2 position);
        public bool PointMeeting(string species, Vector2 position)
        {
            List<Entity> entities = Game.world.FindEntities(species);
            foreach (Entity entity in entities)
            {
                List<ICollider> colliders = entity.FindComponents<ICollider>();
                foreach (ICollider collider in colliders)
                {
                    if (PointMeeting(collider, position))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
