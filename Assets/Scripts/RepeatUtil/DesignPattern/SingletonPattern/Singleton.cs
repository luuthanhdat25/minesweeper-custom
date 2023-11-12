using UnityEngine;

namespace RepeatUtil.DesignPattern.SingletonPattern
{
    /// <summary>
    /// Generic Singleton class used for creating a single instance of a MonoBehaviour-derived class
    /// </summary>
    /// <typeparam name="T">The type of the MonoBehaviour-derived class</typeparam>
    public class Singleton<T> : RepeatMonoBehaviour where T : Component
    {
        protected static T instance;

        /// <summary>
        /// Get the single instance of the class
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance != null) return instance;
                return SetUpInstance();
            }
        }

        private static T SetUpInstance()
        {
            instance = (T)FindObjectOfType(typeof(T));
            if (instance != null) return instance;
            
            GameObject gameObject = new GameObject();
            gameObject.name = typeof(T).Name;
            instance = gameObject.AddComponent<T>();
            DontDestroyOnLoad(gameObject);
            return instance;
        }

        /// <summary>
        /// Overrides the Awake method to ensure there is only one instance of the Singleton.
        /// </summary>
        protected override void Awake()
        {
            RemoveDuplicates();
            base.Awake();
        }
        
        protected virtual void RemoveDuplicates()
        {
            if(instance != null) { Destroy(gameObject); return; }
            
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }
}