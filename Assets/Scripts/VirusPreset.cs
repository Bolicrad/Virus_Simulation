using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "ScriptableObjects/VirusPreset", order = 1)]
public class VirusPreset : ScriptableObject
{
    public string virusName;
    public float r0;
    public int daysOfIncubation;
    public int daysToRecover;
    public float hospitalRate;
    public float deathRate;

    public Virus CreateVirusInstance()
    {
        return new Virus(virusName, r0, daysOfIncubation, daysToRecover, hospitalRate, deathRate);
    }
}
