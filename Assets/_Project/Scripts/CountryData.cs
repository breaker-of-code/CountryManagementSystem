using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CountryData", menuName = "Country Manager/Country Data", order = 1)]
public class CountryData : ScriptableObject
{
    public List<Country> countries = new();
}