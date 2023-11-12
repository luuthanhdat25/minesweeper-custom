using System.Collections.Generic;
using UnityEngine;

namespace RepeatUtil.DesignPattern.ObjectPooling
{
    public abstract class ListObjectPooling<T> : RepeatMonoBehaviour
    {
        [SerializeField] protected Transform prefabsManagerTransform;
        [SerializeField] protected Transform poolTransform;
        [SerializeField] protected List<Transform> prefabList;

        protected override void LoadComponents()
        {
            this.LoadPrefabsManager();
            this.LoadPoolTransform();
        }

        private void LoadPrefabsManager()
        {
            if (prefabsManagerTransform == null)
            {
                this.prefabsManagerTransform = transform.Find("PrefabsManager");
                if (prefabsManagerTransform == null) Debug.LogError("PrefabsManager not found");
            }

            this.LoadPrefabsToPool();
        }

        private void LoadPrefabsToPool()
        {
            if (prefabList.Count > 0) return;

            foreach (Transform prefab in prefabsManagerTransform)
                this.prefabList.Add(prefab);

            this.HideAllPrefab();
        }

        private void HideAllPrefab()
        {
            foreach (Transform prefab in prefabList)
                prefab.gameObject.SetActive(false);
        }

        private void LoadPoolTransform()
        {
            if (this.poolTransform != null) return;
            this.poolTransform = transform.Find("Pool");
        }

        public virtual Transform GetTransform<I>(I instance) where I : T
        {
            Transform prefab = this.GetPrefabByInstanceFromPrefabList(instance);
            if (prefab == null)
            {
                Debug.LogWarning("Prefab not found: " + prefab);
                return null;
            }

            Transform newPrefab = GetObjectFromPool(prefab);
            newPrefab.SetParent(poolTransform);
            newPrefab.gameObject.SetActive(true);
            return newPrefab;
        }

        public virtual Transform GetPrefabByInstanceFromPrefabList<I>(I instance) where I : T
        {
            foreach (Transform prefab in this.prefabList)
                if (prefab.GetComponent<I>().Equals(instance))
                    return prefab;

            return null;
        }

        protected virtual Transform GetObjectFromPool(Transform prefab)
        {
            foreach (Transform objectFromPool in poolTransform)
            {
                if (!objectFromPool.gameObject.activeSelf && prefab.name == objectFromPool.name)
                    return objectFromPool;
            }

            Transform newPrefab = Instantiate(prefab);
            newPrefab.name = prefab.name;
            return newPrefab;
        }

        public virtual void Despawn(Transform obj) 
            => obj.gameObject.SetActive(false);

        public virtual void DespawnAllPool()
        {
            foreach (Transform trans in poolTransform)
                Despawn(trans);
        }

        public void PushObjectToPool(Transform transform)
            => transform.parent = this.poolTransform;

        public virtual List<Transform> GetPrefabList() => prefabList;
    }
}