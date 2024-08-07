using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(CountryManager))]
public class CountryManagerEditor : Editor
{
    private CountryManager countryManager;
    private Vector2 scrollPos;

    private void OnEnable()
    {
        countryManager = (CountryManager)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Country Manager", EditorStyles.boldLabel);

        countryManager.countryData = (CountryData)EditorGUILayout.ObjectField("Country Data", countryManager.countryData, typeof(CountryData), false);

        if (countryManager.countryData == null)
        {
            EditorGUILayout.HelpBox("No CountryData ScriptableObject assigned.", MessageType.Warning);
            return;
        }

        EditorGUILayout.Space();

        if (countryManager.countryData.countries.Count == 0)
        {
            EditorGUILayout.LabelField("No data available.");
        }
        else
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            foreach (Country country in countryManager.countryData.countries)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Country Name:", country.countryName);
                EditorGUILayout.ObjectField("Flag", country.flag, typeof(Sprite), false);

                if (country.regionType == RegionType.State)
                {
                    DisplayRegionData<State>(country.states, "States");
                }
                else if (country.regionType == RegionType.Province)
                {
                    DisplayRegionData<Province>(country.provinces, "Provinces");
                }
                else if (country.regionType == RegionType.County)
                {
                    DisplayRegionData<County>(country.counties, "Counties");
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();
        }
    }

    private void DisplayRegionData<T>(List<T> regions, string regionLabel) where T : class
    {
        EditorGUILayout.LabelField(regionLabel);
        EditorGUI.indentLevel++;
        foreach (var region in regions)
        {
            if (region is State)
            {
                var state = region as State;
                EditorGUILayout.LabelField(state.stateName);
                DisplayCityData(state.cities);
            }
            else if (region is Province)
            {
                var province = region as Province;
                EditorGUILayout.LabelField(province.provinceName);
                DisplayCityData(province.cities);
            }
            else if (region is County)
            {
                var county = region as County;
                EditorGUILayout.LabelField(county.countyName);
                DisplayCityData(county.cities);
            }
        }
        EditorGUI.indentLevel--;
    }

    private void DisplayCityData(List<City> cities)
    {
        EditorGUI.indentLevel++;
        foreach (var city in cities)
        {
            EditorGUILayout.LabelField(city.cityName);
        }
        EditorGUI.indentLevel--;
    }
}
