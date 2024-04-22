// Copyright (C) 2017-2018 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;
using Mirror;

namespace GameVanilla.Core
{
	/// <summary>
	/// Object pooling is a common game development technique that helps reduce the amount of garbage generated
	/// at runtime when creating and destroying a lot of objects. We use it for all the tile entities and their
	/// associated particle effects in the game.
	///
	/// You can find an official tutorial from Unity about object pooling here:
	/// https://unity3d.com/learn/tutorials/topics/scripting/object-pooling
	/// </summary>
    public class ObjectPool : MonoBehaviour
    {
        public GameObject prefab;
        public int initialSize;

        /// <summary>
        /// Unity's Awake method.
        /// </summary>
        private void Awake()
        {
            Assert.IsNotNull(prefab);
        }


        public GameObject GetObject()
        {
            var obj = CreateInstance();
            obj.SetActive(true);
            NetworkServer.Spawn(obj);
            return obj;
        }

        /// <summary>
        /// Returns the specified game object to the pool where it came from.
        /// </summary>
        /// <param name="obj">The object to return to its origin pool.</param>
        public void ReturnObject(GameObject obj)
        {
            var pooledObject = obj.GetComponent<PooledObject>();
            Assert.IsNotNull(pooledObject);
            Assert.IsTrue(pooledObject.pool == this);

            obj.SetActive(false);
            NetworkServer.Destroy(obj);
        }

        /// <summary>
        /// Resets the object pool to its initial state.
        /// </summary>
        public void Reset()
        {
            var objectsToReturn = new List<GameObject>();
            foreach (var instance in transform.GetComponentsInChildren<PooledObject>())
            {
                if (instance.gameObject.activeSelf)
                {
                    objectsToReturn.Add(instance.gameObject);
                }
            }
            foreach (var instance in objectsToReturn)
            {
                ReturnObject(instance);
            }
        }

        /// <summary>
        /// Creates a new instance of the pooled object type.
        /// </summary>
        /// <returns>A new instance of the pooled object type.</returns>
        private GameObject CreateInstance()
        {
            var obj = Instantiate(prefab);
            var pooledObject = obj.AddComponent<PooledObject>();
            pooledObject.pool = this;
            obj.transform.SetParent(transform);
            return obj;
        }
    }

    /// <summary>
    /// Utility class to identify the pool of a pooled object.
    /// </summary>
    public class PooledObject : MonoBehaviour
    {
        public ObjectPool pool;
    }
}
