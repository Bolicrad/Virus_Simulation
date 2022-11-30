using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Plaza : MonoBehaviour
{
    public GameObject individualPrefab;
    private ObjectPool<GameObject> _individualPool;

    public VirusPreset preset;
    private List<Individual> _crowd;
    public List<Button> ChooseButtons;
    public Button StartDayButton;
    
    
    
    private int _dayCount = 0;

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
        obj.transform.SetParent(transform);
    }

    private GameObject CreateFunc()
    {
        var individual = Instantiate(individualPrefab, transform, true);
        return individual;
    }

    private void Start()
    {
        _crowd = new List<Individual>();
        
    }

    public void ChoosePreset(VirusPreset virusPreset)
    {
        preset = virusPreset;
        foreach (var chooseButton in ChooseButtons)
        {
            chooseButton.gameObject.SetActive(false);
        }
        
        StartSimulate();
    }

    public void StartSimulate()
    {
        for (var i = 0; i < 1000; i++)
        {
            _crowd.Add(_individualPool.Get().GetComponent<Individual>());
        }

        var patientZero = _crowd[Random.Range(0, _crowd.Count)];
        patientZero.GetInfect(preset.CreateVirusInstance());
        
        StartDayButton.gameObject.SetActive(true);
        StartDayButton.interactable = true;
    }

    public void StartDay()
    {
        StartDayButton.interactable = false;
        _dayCount++;
        foreach (var individual in _crowd)
        {
            individual.StartDay();
        }
        Transmission();
        EndDay();
    }

    public void Transmission()
    {
        var sources = _crowd.FindAll(e => (e.CanTransmit && e.isOutSide));
        var targets = _crowd.FindAll(e => e.isOutSide);
        foreach (var source in sources)
        {
            source.Transmit(targets);
        }
    }

    public void EndDay()
    {
        foreach (var individual in _crowd)
        {
            individual.EndDay();   
        }

        StartDayButton.interactable = true;
        
        Debug.Log($"At the End of Day {_dayCount}");

        if (_crowd.TrueForAll(e => e.infectStatus is InfectStatus.Recovered or InfectStatus.Death or InfectStatus.Healthy))
        {
            StartDayButton.interactable = false;
            var deaths = _crowd.FindAll(individual => individual.infectStatus == InfectStatus.Death);
            Debug.Log($"After the pandemic, {deaths.Count} People died, and {_crowd.Count-deaths.Count} recovered.");
        }
    }

    public void Reset()
    {
        _dayCount = 0;
        var childList = GetComponentsInChildren<Individual>();
        
        foreach (var individual in childList)
        {
            _individualPool.Release(individual.gameObject);
        }
        _crowd.Clear();
        StartDayButton.gameObject.SetActive(false);
        foreach (var chooseButton in ChooseButtons)
        {
            chooseButton.gameObject.SetActive(true);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
