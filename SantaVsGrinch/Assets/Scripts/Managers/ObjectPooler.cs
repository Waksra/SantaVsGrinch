using System.Collections.Generic;
using Gameplay;
using UnityEngine;

namespace Managers
{
    public class ObjectPooler : MonoBehaviour
    {
        private Dictionary<int, Stack<GameObject>> indexToPoolMap;
        private Dictionary<string, int> nameToIndexMap;
        private Dictionary<int, GameObject> indexToPrefab;

        private new Transform transform;

        private static ObjectPooler _instance;
        
        public static GameObject GetObject(int index)
        {
            ObjectPooler instance = GetInstance();

            if (!instance.indexToPoolMap.ContainsKey(index))
                return null;

            Stack<GameObject> pool = instance.indexToPoolMap[index];
            
            if(pool.Count == 0)
            {
                Poolable poolable = instance.indexToPrefab[index].GetComponent<Poolable>();
                int amount = poolable.InitialPoolSize / 2;
                instance.IncreasePool(index, amount);
            }
            
            GameObject go = pool.Pop();
            return go;
        }

        public static void ReturnToPool(GameObject go)
        {
            ObjectPooler instance = GetInstance();

            if(!instance.nameToIndexMap.TryGetValue(go.name, out int index))
                return;

            ReturnToPool(go, index);
        }

        public static void ReturnToPool(GameObject go, int index)
        {
            ObjectPooler instance = GetInstance();

            if(!instance.indexToPoolMap.TryGetValue(index, out Stack<GameObject> pool))
                return;

            instance.ResetObject(go);
            pool.Push(go);
        }

        public static int GetIndex(string name)
        {
            ObjectPooler instance = GetInstance();

            if (!instance.nameToIndexMap.TryGetValue(name, out int index))
                return -1;
            
            return index;
        }

        public static int AddObject(GameObject go)
        {
            return GetInstance().InternalAddObject(go);
        }
        
        //Pool
        private int InternalAddObject(GameObject go)
        {
            if (nameToIndexMap.ContainsKey(go.name))
                return nameToIndexMap[go.name];

            if (!go.TryGetComponent(out Poolable poolable))
            {
                Debug.LogError($"{go.name} has no Poolable component.");
                return -1;
            }

            int index = nameToIndexMap.Count;
            nameToIndexMap.Add(go.name, index);

            indexToPrefab.Add(index, go);

            Stack<GameObject> newPool = new Stack<GameObject>(poolable.InitialPoolSize);
            indexToPoolMap.Add(index, newPool);
            IncreasePool(index, poolable.InitialPoolSize);

            return index;
        }
        
        private void IncreasePool(int index, int amount)
        {
            Stack<GameObject> pool = indexToPoolMap[index];
            
            for (int i = 0; i < amount; i++)
            {
                GameObject go = CreateNewObject(index);
                pool.Push(go);
            }
        }

        private GameObject CreateNewObject(int index)
        {
            GameObject go = Instantiate(indexToPrefab[index], transform);
            ResetObject(go);
            go.GetComponent<Poolable>().PoolIndex = index;

            return go;
        }
        
        //GameObject
        private void ResetObject(GameObject go)
        {
            go.SetActive(false);
        }
        
        //Instance
        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(this);

            transform = GetComponent<Transform>();
        }

        private static ObjectPooler GetInstance()
        {
            if (_instance == null)
                _instance = CreateNewInstance();

            return _instance;
        }

        private static ObjectPooler CreateNewInstance()
        {
            GameObject newGameObject = new GameObject("ObjectPooler");
            ObjectPooler newInstance = newGameObject.AddComponent<ObjectPooler>();
            newInstance.Initialize();

            return newInstance;
        }

        private void Initialize()
        {
            indexToPoolMap = new Dictionary<int, Stack<GameObject>>();
            nameToIndexMap = new Dictionary<string, int>();
            indexToPrefab = new Dictionary<int, GameObject>();
        }
    }
}