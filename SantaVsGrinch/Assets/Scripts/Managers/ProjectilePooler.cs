using System.Collections.Generic;
using Gameplay;
using UnityEngine;

namespace Managers
{
    public class ProjectilePooler : MonoBehaviour
    {
        private Dictionary<int, Stack<Projectile>> indexToPoolMap;
        private Dictionary<string, int> nameToIndexMap;
        private Dictionary<int, Projectile> indexToPrefab;

        private new Transform transform;

        private static ProjectilePooler _instance;

        public static Projectile GetProjectile(int index)
        {
            ProjectilePooler instance = GetInstance();

            if (!instance.indexToPoolMap.ContainsKey(index))
                return null;

            Stack<Projectile> pool = instance.indexToPoolMap[index];
            if(pool.Count == 0)
                instance.IncreasePool(index, instance.indexToPrefab[index].InitialPoolAmount / 2);
            
            Projectile projectile = pool.Pop();
            return projectile;
        }

        public static void ReturnToPool(Projectile projectile)
        {
            ProjectilePooler instance = GetInstance();

            if(!instance.nameToIndexMap.TryGetValue(projectile.name, out int index))
                return;

            ReturnToPool(projectile, index);
        }

        public static void ReturnToPool(Projectile projectile, int index)
        {
            ProjectilePooler instance = GetInstance();

            if(!instance.indexToPoolMap.TryGetValue(index, out Stack<Projectile> pool))
                return;

            instance.ResetProjectile(projectile);
            pool.Push(projectile);
        }

        public static int GetIndex(string name)
        {
            ProjectilePooler instance = GetInstance();

            if (!instance.nameToIndexMap.TryGetValue(name, out int index))
                return -1;
            
            return index;
        }

        public static int AddProjectile(Projectile projectile)
        {
            return GetInstance().InternalAddProjectile(projectile);
        }

        //Pool
        private int InternalAddProjectile(Projectile projectile)
        {
            if (nameToIndexMap.ContainsKey(projectile.name))
                return nameToIndexMap[projectile.name];

            int index = nameToIndexMap.Count;
            nameToIndexMap.Add(projectile.name, index);

            indexToPrefab.Add(index, projectile);

            Stack<Projectile> newPool = new Stack<Projectile>(projectile.InitialPoolAmount);
            indexToPoolMap.Add(index, newPool);
            IncreasePool(index, projectile.InitialPoolAmount);

            return index;
        }

        private void IncreasePool(int index, int amount)
        {
            Stack<Projectile> pool = indexToPoolMap[index];
            for (int i = 0; i < amount; i++)
            {
                Projectile newProjectile = CreateNewProjectile(index);
                pool.Push(newProjectile);
            }
        }

        private Projectile CreateNewProjectile(int index)
        {
            Projectile newProjectile = Instantiate(indexToPrefab[index], transform);
            ResetProjectile(newProjectile);
            newProjectile.PoolIndex = index;

            return newProjectile;
        }

        //Projectile

        private void ResetProjectile(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);
            projectile.Reset();
        }

        //Instance
        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(this);

            transform = GetComponent<Transform>();
        }

        private static ProjectilePooler GetInstance()
        {
            if (_instance == null)
                _instance = CreateNewInstance();

            return _instance;
        }

        private static ProjectilePooler CreateNewInstance()
        {
            GameObject newGameObject = new GameObject("ProjectilePooler");
            ProjectilePooler newInstance = newGameObject.AddComponent<ProjectilePooler>();
            newInstance.Initialize();

            return newInstance;
        }

        private void Initialize()
        {
            indexToPoolMap = new Dictionary<int, Stack<Projectile>>();
            nameToIndexMap = new Dictionary<string, int>();
            indexToPrefab = new Dictionary<int, Projectile>();
        }
    }
}