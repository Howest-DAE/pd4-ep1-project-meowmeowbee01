using UnityEngine;

namespace Assets.Scripts.Singleton
{
	public abstract class MonobehaviourSingleton<T> : MonoBehaviour where T : MonobehaviourSingleton<T>
	{
		private static T _instance;
		public static T Instance
		{
			get
			{

				if (_instance == null) //Lazy instantiation
				{
					GameObject gameObject = new(typeof(T).Name);
					_instance = gameObject.AddComponent<T>();
					DontDestroyOnLoad(gameObject);
				}
				return _instance;
			}
		}

		protected virtual void Awake()
		{
			if (_instance != null && _instance != this) //prevent other instances (could be embedded in scene)
			{
				Destroy(gameObject);
				return;
			}
			_instance = this as T;
			DontDestroyOnLoad(gameObject);
		}
	}

}
