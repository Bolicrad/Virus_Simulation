using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Virus
{
    public string VirusName;
    public float R0;
    public int DaysOfIncubation;
    public int DaysToRecover;
    public float HospitalRate;
    public float DeathRate;

    private readonly int _transmissionCount;
    private int _version;

    public Virus(string virusName,float r0, int daysOfIncubation, int daysToRecover, float hospitalRate, float deathRate)
    {
        VirusName = virusName;
        R0 = r0;
        DaysOfIncubation = daysOfIncubation;
        DaysToRecover = daysToRecover;
        HospitalRate = hospitalRate;
        DeathRate = deathRate;

        _version = 1;
    }

    public Virus(Virus previous)
    {
        VirusName = previous.VirusName;
        R0 = previous.R0;
        DaysOfIncubation = previous.DaysOfIncubation;
        DaysToRecover = previous.DaysToRecover;
        HospitalRate = previous.HospitalRate;
        DeathRate += previous.DeathRate;

        _transmissionCount = previous._transmissionCount + 1;
        if (_transmissionCount >= 10)
        {
            Mutant();
        }
    }

    public void Transmit(Individual target)
    {
        target.GetInfect(this);
    }

    public void DebugPrint()
    {
        Debug.Log($"Virus Information:\n" +
                  $"Name: {VirusName}, " +
                  $"Version: {_version}, " +
                  $"r0: {R0}, " +
                  $"Days Of Incubation: {DaysOfIncubation}, " +
                  $"Days To Recover: {DaysToRecover}, " +
                  $"Hospitalization Rate: {HospitalRate}, " +
                  $"Death Rate: {DeathRate}."
                  );
    }

    private void Mutant()
    {
        R0 += Random.Range(-0.1f, 0.1f);
        DaysOfIncubation += Random.Range(-1, 2);
        DaysToRecover += Random.Range(-1, 2);
        HospitalRate += Random.Range(-0.005f, 0.005f);
        DeathRate += Random.Range(-0.001f, 0.001f);
        _version++;
    }
}