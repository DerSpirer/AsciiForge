using AsciiForge.Components;
using AsciiForge.Resources;
using System.ComponentModel;
using System.Reflection;

namespace AsciiForge.Engine
{
    public class Entity
    {
        private bool _started = false;
        private bool _destroyed = false;

        public readonly string species;
        public readonly List<Component> components;
        private Transform _transform;
        public Transform transform
        {
            get
            {
                return _transform;
            }
        }

        internal Entity(string species)
        {
            this.species = species;
            this.components = new List<Component>();
            Transform transform = new Transform();
            this._transform = transform;
            components.Add(transform);
            transform._entity = this;
        }

        public async Task RemoveComponent(Component component)
        {
            if (component == null || !components.Contains(component))
            {
                return;
            }
            if (component.isEnabled)
            {
                MethodInfo? destroyMethod = component.GetType().GetMethod("Destroy", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance, new Type[0]);
                if (destroyMethod != null)
                {
                    object? result = destroyMethod.Invoke(component, new object[0]);
                    if (result != null && result is Task)
                    {
                        await (Task)result;
                    }
                }
            }
            components.Remove(component);
        }
        public async Task AddComponent(Component component)
        {
            if (component == null || components.Contains(component))
            {
                return;
            }
            if (component.entity != null)
            {
                Logger.Warning($@"Failed to add a '{component.GetType().FullName}' component to an entity of the species '{this.species}' because it is already assigned to a different entity of the species '{component.entity.species}'");
                return;
            }
            if (components.Any(c => c.GetType() == component.GetType()))
            {
                await RemoveComponent(components.Find(c => c.GetType() == component.GetType())!);
                Logger.Info($"Replacing '{component.GetType().FullName}' component of an entity of the species '{this.species}' with a different component of this type");
            }
            if (component.GetType() == typeof(Transform))
            {
                this._transform = (Transform)component;
            }
            components.Add(component);
            component._entity = this;
        }
        internal async Task AddComponent(ComponentResource componentResource)
        {
            Component component = (Component)Activator.CreateInstance(componentResource.type)!;
            foreach ((string key, object? value) in componentResource.properties)
            {
                PropertyInfo property = componentResource.type.GetProperty(key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
                property.SetValue(component, value);
            }
            await AddComponent(component);
        }
        public T? FindComponent<T>()
            where T : class
        {
            return components.FirstOrDefault(c => c is T) as T;
        }
        public List<T> FindComponents<T>()
            where T : class
        {
            return components.Where(c => c is T).Select(c => (c as T)!).ToList();
        }

        internal async Task Awake()
        {
            foreach (Component component in components.Where(c => c.isEnabled))
            {
                MethodInfo? awakeMethod = component.GetType().GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance, new Type[0]);
                if (awakeMethod != null)
                {
                    object? result = awakeMethod.Invoke(component, new object[0]);
                    if (result != null && result is Task)
                    {
                        await (Task)result;
                    }
                }
            }
        }
        private async Task Start()
        {
            foreach (Component component in components.Where(c => c.isEnabled))
            {
                MethodInfo? startMethod = component.GetType().GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance, new Type[0]);
                if (startMethod != null)
                {
                    object? result = startMethod.Invoke(component, new object[0]);
                    if (result != null && result is Task)
                    {
                        await (Task)result;
                    }
                }
            }
        }
        internal async Task Update(float deltaTime)
        {
            if (!_started)
            {
                _started = true;
                await Start();
            }
            foreach (Component component in components.Where(c => c.isEnabled))
            {
                MethodInfo? updateMethod = component.GetType().GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance, new Type[1] { typeof(float) });
                if (updateMethod != null)
                {
                    object? result = updateMethod.Invoke(component, new object[1] { deltaTime });
                    if (result != null && result is Task)
                    {
                        await (Task)result;
                    }
                }
            }
        }
        public async Task Draw(Canvas canvas)
        {
            foreach (Component component in components.Where(c => c.isEnabled))
            {
                MethodInfo? drawMethod = component.GetType().GetMethod("Draw", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance, new Type[1] { typeof(Canvas) });
                if (drawMethod != null)
                {
                    object? result = drawMethod.Invoke(component, new object[1] { canvas });
                    if (result != null && result is Task)
                    {
                        await (Task)result;
                    }
                }
            }
        }
        public async Task Destroy()
        {
            if (_destroyed || !Game.world.entities.Contains(this))
            {
                return;
            }
            List<Component> tempComponents = new List<Component>(components);
            foreach (Component component in tempComponents)
            {
                await RemoveComponent(component);
            }
            Game.world.entities.Remove(this);
            _destroyed = true;
        }
    }
}
