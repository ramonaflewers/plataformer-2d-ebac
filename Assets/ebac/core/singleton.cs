using UnityEngine;
using System.Collections.Generic;

namespace ebac.core.singleton{

    public class singleton<T> : MonoBehaviour where T: MonoBehaviour
    {
        public static T Instance;

        private void Awake(){
            if (Instance == null)
                Instance = GetComponent<T>();
            else
                Destroy(gameObject);
        }
    }
}