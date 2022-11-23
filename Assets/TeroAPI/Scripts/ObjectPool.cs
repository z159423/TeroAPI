using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPool
{
    //오브젝트 풀
    Queue<GameObject> pool = new Queue<GameObject>();
    //생성되어 있는 오브젝트 리스트
    List<GameObject> generatedObject = new List<GameObject>();
    //오브젝트 프리팹
    [SerializeField] private GameObject objectPrefabs;
    //오브젝트를 생성할 부모위치
    [SerializeField] public Transform objectParent;

    /// <summary>
    /// 오브젝트 풀에서 오브젝트 가져오기 만약 풀 안에 오브젝트가 없다면 새로운 오브젝트를 생성합니다.
    /// </summary>   
    public GameObject DequeueObject(Vector3 position)
    {
        if (pool.Count > 0)
        {
            var obj = pool.Dequeue();
            obj.transform.SetParent(objectParent);
            obj.transform.position = position;

            obj.SetActive(true);

            generatedObject.Add(obj);

            return obj;
        }
        else
        {
            var obj = GameObject.Instantiate(objectPrefabs, objectParent);
            obj.transform.position = position;

            generatedObject.Add(obj);

            return obj;
        }
    }

    /// <summary>
    /// 다시 오브젝트를 풀에 넣는 함수
    /// </summary>  
    public void EnqueueObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);

        generatedObject.Remove(obj);
    }
}
