using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuildings.Utilities
{
    /// <summary>
    /// シングルトンを実装
    /// </summary>
    /// <typeparam name="T">コンポーネントの型</typeparam>
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        /// <summary>シングルトンインスタンス</summary>
        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if(instance == null)
                    {
                        Debug.LogWarning(typeof(T) + "is nothing");
                    }
                }

                return instance;
            }
        }

        /// <summary>インスタンス</summary>
        private static T instance = null;
        /// <summary>初期化完了フラグ</summary>
		private bool inited = false;


        protected virtual void Awake()
        {
            CheckInstance();

            if(this.inited == false)
            {
                this.inited = true;
            }
        }

        /// <summary>
        /// インスタンスの検索・確認
        /// </summary>
        /// <returns>インスタンスの検索が出来たかどうか</returns>
        private bool CheckInstance()
        {
            if(instance == null)
            {
                instance = (T)this;
                return true;
            }
            else if(Instance == this)
            {
                return true;
            }

            Destroy(this);
            return false;
        }
    }
}
