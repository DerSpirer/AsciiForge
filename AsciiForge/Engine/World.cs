﻿using AsciiForge.Resources;

namespace AsciiForge.Engine
{
    public class World
    {
        private readonly List<RoomResource> _rooms;
        public List<RoomResource> rooms { get { return _rooms; } }
        public int currRoom { get; private set; } = 0;
        private readonly List<Entity> _entities = new List<Entity>();
        public List<Entity> entities { get { return _entities; } }

        private long? _lastUpdateTime = null;

        internal World(List<RoomResource> rooms)
        {
            if (rooms == null || rooms.Count <= 0)
            {
                Logger.Error("Trying to instantiate a world with no rooms");
                throw new Exception("Trying to instantiate a world with no rooms");
            }
            _rooms = rooms;
        }

        internal async Task Start()
        {
            await LoadRoom(0);
        }
        internal async Task Update()
        {
            long currTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (_lastUpdateTime == null)
            {
                _lastUpdateTime = currTime;
            }
            float deltaTime = (currTime - ((long)_lastUpdateTime)) / 1000.0f;
            foreach (Entity entity in _entities)
            {
                await entity.Update(deltaTime);
            }

            _lastUpdateTime = currTime;
        }
        public async Task Draw(Canvas canvas)
        {
            foreach (Entity entity in _entities)
            {
                await entity.Draw(canvas);
            }
        }
        internal async Task Destroy()
        {
            await UnloadRoom();
        }

        public async Task LoadRoom(int index)
        {
            if (index < 0 || index >= rooms.Count)
            {
                return;
            }

            await UnloadRoom();
            currRoom = index;
            foreach (InstanceResource instanceResource in rooms[currRoom].instances)
            {
                await Instantiate(instanceResource);
            }
        }
        private async Task UnloadRoom()
        {
            List<Entity> tempEntities = new List<Entity>(_entities);
            foreach (Entity entity in tempEntities)
            {
                await entity.Destroy();
            }
            _entities.Clear();
        }

        public Entity? FindEntity(string species) => _entities.FirstOrDefault(e => e.species == species);
        public List<Entity> FindEntities(string species) => _entities.Where(e => e.species == species).ToList();
        public async Task<Entity> Instantiate()
        {
            Entity entity = new Entity(string.Empty);
            _entities.Add(entity);
            await entity.Awake();
            return entity;
        }
        public async Task<Entity?> Instantiate(string species, Vector3? position = null)
        {
            if (string.IsNullOrEmpty(species))
            {
                Logger.Error("Failed to instantiate entity with no species");
                return null;
            }
            if (!ResourceManager.entities.ContainsKey(species))
            {
                Logger.Error($"Failed to instantiate entity of non-existing species: {species}");
                return null;
            }
            Entity entity = await Instantiate(ResourceManager.entities[species]);
            if (position != null)
            {
                entity.transform.position = position;
            }
            _entities.Add(entity);
            await entity.Awake();
            return entity;
        }
        private async Task<Entity> Instantiate(EntityResource entityResource)
        {
            Entity entity = new Entity(entityResource.species);
            foreach (ComponentResource componentResource in entityResource.components)
            {
                await entity.AddComponent(componentResource);
            }
            return entity;
        }
        private async Task<Entity> Instantiate(InstanceResource instanceResource)
        {
            EntityResource entityResource = ResourceManager.entities[instanceResource.species];
            Entity entity = await Instantiate(entityResource);
            foreach (ComponentResource componentResource in instanceResource.components)
            {
                await entity.AddComponent(componentResource);
            }
            _entities.Add(entity);
            await entity.Awake();
            return entity;
        }
    }
}
