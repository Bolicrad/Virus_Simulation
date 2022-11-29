using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Plaza : MonoBehaviour
{
    public GameObject individualPrefab;
    private ObjectPool<GameObject> _individualPool;

    public VirusPreset preset;

    private void Awake()
    {
        _individualPool =
            new ObjectPool<GameObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, true, 100);
    }

    private void ActionOnDestroy(GameObject obj)
    {
        //Do nothing yet
    }

    private void ActionOnRelease(GameObject obj)
    {
        obj.transform.SetParent(null);
    }

    private void ActionOnGet(GameObject obj)
    {
        obj.GetComponent<Individual>().Reset();
    }

    private GameObject CreateFunc()
    {
        var individual = Instantiate(individualPrefab, transform, true);
        return individual;
    }

    private void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
