using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum InfectStatus{
    Healthy,
    Incubation,
    Ill,
    Hospital,
    Death,
    Recovered
}

public class Individual : MonoBehaviour
{
    #region Parameters
    
    public int age;
    public int daysOfInfected;
    public bool isOutSide;

    private float InterestOfGoOutside
    {
        get
        {
            switch (infectStatus)
            {
                case InfectStatus.Recovered:
                case InfectStatus.Incubation:
                case InfectStatus.Healthy: return Random.Range(0.4f, 0.9f); 
                case InfectStatus.Ill: return Random.Range(0.1f, 0.4f);
                case InfectStatus.Hospital:
                case InfectStatus.Death:
                default: return -0.1f;
            }
        }
    }

    private bool IsInfected => _virusInstance != null;
    public bool CanTransmit => infectStatus is InfectStatus.Incubation or InfectStatus.Ill;

    private float RealDeathRate
    {
        get
        {
            if (!IsInfected) return -0.1f;
            else
            {
                var result = _virusInstance.DeathRate;
                if (age is <= 18 or >= 65)
                {
                    result *= 2;
                }

                if (infectStatus == InfectStatus.Hospital)
                {
                    result *= 0.2f;
                }

                return result;
            }
        }
    }

    private Virus _virusInstance;
    public InfectStatus infectStatus;
    
    #endregion

    #region Components

    public SpriteRenderer spriteRenderer;

    #endregion

    #region Transmission

    public InfectStatus GetInfect(Virus virus)
    {
        if (IsInfected) return infectStatus;
        
        _virusInstance = new Virus(virus);
        infectStatus = InfectStatus.Incubation;
        daysOfInfected = 0;
        return infectStatus;
    }

    public void Transmit(List<Individual> targets)
    {
        foreach (var individual in targets)
        {
            individual.GetInfect(_virusInstance);
        }
    }

    private void GoOutSide()
    {
        //Go out side
        isOutSide = true;
    }

    private void Ill()
    {
        //ill
        infectStatus = InfectStatus.Ill;
    }

    private void GoHospital()
    {
        //hospitalized
        infectStatus = InfectStatus.Hospital;
    }

    private void Die()
    {
        //AS is
        infectStatus = InfectStatus.Death;
        Debug.Log($"Individual {GetInstanceID()} Died at age of ${age}, after {daysOfInfected} days of infection of the virus:\n");
        _virusInstance.DebugPrint();
    }

    private void Recover()
    {
        infectStatus = InfectStatus.Recovered;
        Debug.Log(
            $"Individual {GetInstanceID()} Recovered from the disease, after {daysOfInfected} days of infection of the virus:\n");
        _virusInstance.DebugPrint();
    }

    #endregion

    #region Game Loop

    public void StartDay()
    {
        if (Random.Range(0f, 1f) < InterestOfGoOutside)
        {
            GoOutSide();
        }
    }

    public void EndDay()
    {
        if (infectStatus is InfectStatus.Healthy or InfectStatus.Death or InfectStatus.Recovered) return;
        daysOfInfected++;
        if (infectStatus == InfectStatus.Incubation)
        {
            if (daysOfInfected > _virusInstance.DaysOfIncubation)
            {
                Ill();
            }
            return;
        }

        if (infectStatus != InfectStatus.Hospital)
        {
            if (Random.Range(0f, 1f) < _virusInstance.HospitalRate)
            {
                GoHospital();
            }
        }
        if (Random.Range(0f,1f) < RealDeathRate)
        {
            Die();
            return;
        }
        
        //survived today
        if (daysOfInfected > _virusInstance.DaysOfIncubation + _virusInstance.DaysToRecover)
        {
            Recover();
        }

    }

    #endregion

    // Start is called before the first frame update
    public void Reset()
    {
        age = Random.Range(0, 80);
        infectStatus = InfectStatus.Healthy;
        _virusInstance = null;
    }

    private void Awake()
    {
        Reset();
    }
}
