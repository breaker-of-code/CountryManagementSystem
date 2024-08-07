using System.Collections.Generic;
using UnityEngine;

public class CountryManager : MonoBehaviour
{
    public CountryData countryData;
    [HideInInspector] public List<Country> countries = new();

    private void Awake()
    {
        if (countryData != null)
        {
            countries = countryData.countries;
        }
    }
}

[System.Serializable]
public class Country
{
    public string countryName;
    public Sprite flag;
    public RegionType regionType;
    public List<State> states = new();
    public List<Province> provinces = new();
    public List<County> counties = new();
}

[System.Serializable]
public class State
{
    public string stateName;
    public List<City> cities = new();
}

[System.Serializable]
public class Province
{
    public string provinceName;
    public List<City> cities = new();
}

[System.Serializable]
public class County
{
    public string countyName;
    public List<City> cities = new();
}

[System.Serializable]
public class City
{
    public string cityName;
}

public enum RegionType
{
    None,
    State,
    Province,
    County
}