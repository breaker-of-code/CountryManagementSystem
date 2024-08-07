using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class CountryManagerWindow : EditorWindow
{
    private CountryManager countryManager;
    private Vector2 scrollPos;
    private string newCountryName = "";
    private Sprite newCountryFlag;
    private RegionType newRegionType = RegionType.None;
    private List<State> newStates = new List<State>();
    private List<Province> newProvinces = new List<Province>();
    private List<County> newCounties = new List<County>();
    private int selectedTab = 0;

    [MenuItem("Country Manager/Manage Data")]
    public static void ShowWindow()
    {
        GetWindow<CountryManagerWindow>("Country Manager");
    }

    private void OnEnable()
    {
        countryManager = FindObjectOfType<CountryManager>();
        if (countryManager == null)
        {
            GameObject manager = new GameObject("CountryManager");
            countryManager = manager.AddComponent<CountryManager>();
        }
    }

    private void OnGUI()
    {
        if (countryManager == null) return;

        if (countryManager.countryData == null)
        {
            EditorGUILayout.HelpBox("No CountryData ScriptableObject assigned.", MessageType.Warning);
            countryManager.countryData = (CountryData)EditorGUILayout.ObjectField("Country Data",
                countryManager.countryData, typeof(CountryData), false);
            return;
        }

        selectedTab = GUILayout.Toolbar(selectedTab, new string[] { "Add Data", "Show Data" });

        switch (selectedTab)
        {
            case 0:
                ShowAddDataTab();
                break;
            case 1:
                ShowShowDataTab();
                break;
        }
    }

    private void ShowAddDataTab()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.LabelField("Add New Country", EditorStyles.boldLabel);
        newCountryName = EditorGUILayout.TextField("Country Name", newCountryName);
        newCountryFlag = (Sprite)EditorGUILayout.ObjectField("Flag", newCountryFlag, typeof(Sprite), false);

        EditorGUILayout.LabelField("Region Type:");
        newRegionType = (RegionType)EditorGUILayout.EnumPopup(newRegionType);

        if (newRegionType == RegionType.State)
        {
            DisplayRegions<State>(newStates, "State", "Add State");
        }

        if (newRegionType == RegionType.Province)
        {
            DisplayRegions<Province>(newProvinces, "Province", "Add Province");
        }

        if (newRegionType == RegionType.County)
        {
            DisplayRegions<County>(newCounties, "County", "Add County");
        }

        EditorGUILayout.EndScrollView();

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Save Country", GUILayout.Height(40)))
        {
            if (string.IsNullOrEmpty(newCountryName) || newCountryFlag == null)
            {
                EditorUtility.DisplayDialog("Error", "Country Name and Flag are required.", "OK");
            }
            else
            {
                Country newCountry = new Country
                {
                    countryName = ToCamelCase(newCountryName),
                    flag = newCountryFlag,
                    regionType = newRegionType,
                    states = FormatStates(newStates),
                    provinces = FormatProvinces(newProvinces),
                    counties = FormatCounties(newCounties)
                };
                countryManager.countryData.countries.Add(newCountry);

                // Reset fields
                newCountryName = "";
                newCountryFlag = null;
                newRegionType = RegionType.None;
                newStates = new List<State>();
                newProvinces = new List<Province>();
                newCounties = new List<County>();
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(countryManager.countryData);
        }
    }

    private void DisplayRegions<T>(List<T> regions, string regionLabel, string addButtonLabel) where T : new()
    {
        if (GUILayout.Button(addButtonLabel))
        {
            regions.Add(new T());
        }

        for (int i = 0; i < regions.Count; i++)
        {
            var region = regions[i];
            EditorGUILayout.BeginHorizontal();
            if (region is State)
            {
                var state = region as State;
                state.stateName = EditorGUILayout.TextField($"{regionLabel} {i + 1}", state.stateName);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    regions.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
                DisplayCities(state.cities);
            }
            else if (region is Province)
            {
                var province = region as Province;
                province.provinceName = EditorGUILayout.TextField($"{regionLabel} {i + 1}", province.provinceName);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    regions.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
                DisplayCities(province.cities);
            }
            else if (region is County)
            {
                var county = region as County;
                county.countyName = EditorGUILayout.TextField($"{regionLabel} {i + 1}", county.countyName);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    regions.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
                DisplayCities(county.cities);
            }

            GUILayout.Space(10); // Add vertical space between regions
        }
    }

    private void DisplayCities(List<City> cities)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Cities", GUILayout.Width(50));
        if (GUILayout.Button("+", GUILayout.Width(20)))
        {
            cities.Add(new City());
        }

        EditorGUILayout.EndHorizontal();

        for (int j = 0; j < cities.Count; j++)
        {
            EditorGUILayout.BeginHorizontal();
            cities[j].cityName = EditorGUILayout.TextField($"City {j + 1}", cities[j].cityName);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                cities.RemoveAt(j);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void ShowShowDataTab()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        if (countryManager.countryData.countries.Count == 0)
        {
            EditorGUILayout.LabelField("No data available.");
        }
        else
        {
            foreach (Country country in countryManager.countryData.countries)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Country Name:", country.countryName);
                EditorGUILayout.ObjectField("Flag", country.flag, typeof(Sprite), false);

                if (country.regionType == RegionType.State)
                {
                    DisplayRegionData<State>(country.states, "States");
                }

                if (country.regionType == RegionType.Province)
                {
                    DisplayRegionData<Province>(country.provinces, "Provinces");
                }

                if (country.regionType == RegionType.County)
                {
                    DisplayRegionData<County>(country.counties, "Counties");
                }

                if (GUILayout.Button("Delete Country"))
                {
                    if (EditorUtility.DisplayDialog("Confirm Delete", "Are you sure you want to delete this country?",
                            "Yes", "No"))
                    {
                        countryManager.countryData.countries.Remove(country);
                        break;
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }

        EditorGUILayout.EndScrollView();
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

    private string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        str = str.ToLower();
        return char.ToUpper(str[0]) + str.Substring(1);
    }

    private List<State> FormatStates(List<State> states)
    {
        foreach (var state in states)
        {
            state.stateName = ToCamelCase(state.stateName);
            foreach (var city in state.cities)
            {
                city.cityName = ToCamelCase(city.cityName);
            }
        }

        return states;
    }

    private List<Province> FormatProvinces(List<Province> provinces)
    {
        foreach (var province in provinces)
        {
            province.provinceName = ToCamelCase(province.provinceName);
            foreach (var city in province.cities)
            {
                city.cityName = ToCamelCase(city.cityName);
            }
        }

        return provinces;
    }
    
    private List<County> FormatCounties(List<County> counties)
    {
        foreach (var county in counties)
        {
            county.countyName = ToCamelCase(county.countyName);
            foreach (var city in county.cities)
            {
                city.cityName = ToCamelCase(city.cityName);
            }
        }
        return counties;
    }
}