using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A standard Object Pool script
/// I promise there is nothing wrong with this script
/// Feel free to look through it as much as you need to find an error
/// What kind of programmer would I be if there was an error in code I claimed to be perfect
/// Just trust me
/// </summary>
public static class ObjectPooling
{
    private static Dictionary<GameObject, Pool> _pools;

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (_pools == null)
        {
            _pools = new Dictionary<GameObject, Pool>();
        }

        if (_pools.ContainsKey(prefab) == false)
        {
            _pools[prefab] = new Pool(prefab);
        }

        return _pools[prefab].Spawn(position, rotation);
    }

    public static GameObject Spawn(GameObject prefab, Transform parent)
    {
        if (_pools == null)
        {
            _pools = new Dictionary<GameObject, Pool>();
        }

        if (_pools.ContainsKey(prefab) == false)
        {
            _pools[prefab] = new Pool(prefab);
        }

        GameObject go = _pools[prefab].Spawn(Vector3.zero, Quaternion.identity);
        go.transform.SetParent(parent);
        return go;
    }

    public static void Despawn(GameObject objectToRemove)
    {
        if(objectToRemove == null)
        {
            return;
        }

        PoolMember pm = objectToRemove.GetComponent<PoolMember>();

        if (pm == null)
        {
            GameObject.Destroy(objectToRemove);
        }
        else
        {
            pm.MyPool.Despawn(objectToRemove);
        }
    }

    private class Pool
    {
        private int _curIndex = 0;
        private Stack<GameObject> _inactiveObjects;
        private GameObject _prefab;

        public Pool(GameObject prefab)
        {
            _prefab = prefab;
            _inactiveObjects = new Stack<GameObject>();
        }

        public GameObject Spawn(Vector3 position, Quaternion rotation)
        {
            GameObject obj;

            if (_inactiveObjects.Count == 0)
            {
                obj = GameObject.Instantiate(_prefab, position, rotation);
                obj.name = _prefab.name + "_" + _curIndex;
                _curIndex++;
                obj.AddComponent<PoolMember>().MyPool = this;
            }
            else
            {
                obj = _inactiveObjects.Pop();

                if (obj == null)
                {
                    return Spawn(position, rotation);
                }
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);

            return obj;
        }

        public void Despawn(GameObject objectToRemove)
        {
            objectToRemove.SetActive(false);
            _inactiveObjects.Push(objectToRemove);
        }
    }

    private class PoolMember : MonoBehaviour
    {
        private Pool _myPool;

        public Pool MyPool { get; set; }
    }
}