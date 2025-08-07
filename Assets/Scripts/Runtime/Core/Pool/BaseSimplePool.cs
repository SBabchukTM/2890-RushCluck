using Core.Factory;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class BaseSimplePool
    {
        private readonly ILogger _logger;
        private readonly GameObjectFactory _factory;
        private List<Sprite> _sprites;

        private List<GameObject> _items;
        private GameObject _prefab;
        private bool _initialized;

        public BaseSimplePool(ILogger logger,
            GameObjectFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }

        public void Initialize(int size, GameObject prefab, List<Sprite> sprites)
        {
            _sprites = sprites;

            if (_initialized)
                return;

            _items = new(size);
            _prefab = prefab;

            for (int i = 0; i < size; i++)
                CreateNewItem();

            _initialized = true;
        }

        public void Initialize(int size, GameObject prefab)
        {
            if (_initialized)
                return;

            _items = new(size);
            _prefab = prefab;
            for (int i = 0; i < size; i++)
                CreateNewItem();

            _initialized = true;
        }

        public GameObject Get()
        {
            if (!_initialized)
            {
                _logger.Error($"Pool not initialized");
                return null;
            }

            if (_items.Count == 0)
                CreateNewItem();

            var item = _items[0];
            _items.Remove(item);

            item.SetActive(true);
            return item;
        }

        public GameObject GetCar()
        {
            if (!_initialized)
            {
                _logger.Error($"Pool not initialized");
                return null;
            }

            if (_items.Count == 0)
                CreateNewItem();

            var item = _items[0];
            _items.Remove(item);

            var spriteRenderer = item.GetComponent<SpriteRenderer>();
            var id = Random.Range(0, _sprites.Count);
            spriteRenderer.sprite = _sprites[id];

            item.SetActive(true);
            return item;
        }

        public void Return(GameObject item)
        {
            if (!_initialized)
            {
                _logger.Error($"Pool not initialized");
                return;
            }

            item.SetActive(false);
            _items.Add(item);
        }

        public void ClearAll()
        {
            _items.Clear();
            _initialized = false;
        }

        private void CreateNewItem()
        {
            GameObject item = _factory.Create(_prefab);
            item.SetActive(false);

            _items.Add(item);

            if (_initialized)
            {
                _logger.Error(
                    $"Object created after initialization, possible memory leak");
            }
        }
    }
}