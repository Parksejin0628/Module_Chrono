using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] prefabs;
    private List<GameObject>[] pools;

    // Start is called before the first frame update
    void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];
        for(int i=0; i<prefabs.Length; i++)
        {
            pools[i] = new List<GameObject>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject Spawn(int index)
    {
        GameObject select = null;

        foreach (GameObject temp in pools[index])
        {
            if(temp.activeSelf == false)
            {
                temp.SetActive(true);
                select = temp;
                break;
            }
        }

        if(select == null)
        {
            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
        }


        return select;
    }
}


