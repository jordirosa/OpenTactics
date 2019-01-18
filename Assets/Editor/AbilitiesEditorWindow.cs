using System;
using System.Collections.Generic;
using System.Xml;

using UnityEditor;
using UnityEngine;

public class AbilitiesEditorWindow : EditorWindow
{
    #region Localization
    private const string LOCALIZATION_RESOURCE = "AbilitiesEditorWindow";

    private const string LOCALIZATION_WINDOW_TITLE = "WindowTitle";
    private const string LOCALIZATION_SAVE = "Save";
    private const string LOCALIZATION_GENERATE = "Generate";
    private const string LOCALIZATION_ABILITIES_LIST_FOLDOUT = "AbilitiesListFoldout";
    private const string LOCALIZATION_ADD = "Add";
    private const string LOCALIZATION_NO_ABILITY_SELECTED = "NoAbilitySelected";
    private const string LOCALIZATION_GENERAL_DATA = "GeneralData";
    private const string LOCALIZATION_COST_DATA = "CostData";
    private const string LOCALIZATION_EDIT = "Edit";
    private const string LOCALIZATION_NAME = "Name";
    private const string LOCALIZATION_CATEGORY = "Category";
    private const string LOCALIZATION_DESCRIPTION = "Description";
    private const string LOCALIZATION_MENTAL_COST = "MentalCost";
    private const string LOCALIZATION_PHYSICAL_COST = "PhysicalCost";
    private const string LOCALIZATION_GENERATING_PREFABS = "GeneratingPrefabs";
    private const string LOCALIZATION_GENERATING_PREFAB = "GeneratingPrefab";
    #endregion

    public const int WINDOW_WIDTH = 600;
    public const int WINDOW_HEIGHT = 600;

    static GUIStyle boldFoldout;

    private static LocalizationData LOCALIZATION_DATA;

    [MenuItem("Custom Tools/Abilities editor")]
    public static void ShowWindow()
    {
        AbilitiesEditorWindow window = (AbilitiesEditorWindow)EditorWindow.GetWindow(typeof(AbilitiesEditorWindow), false);

        LOCALIZATION_DATA = new LocalizationData(Application.dataPath, LOCALIZATION_RESOURCE, LocalizationEditorWindow.getLanguage());
        window.titleContent = new GUIContent(LOCALIZATION_DATA.getEntries()[LOCALIZATION_WINDOW_TITLE]);
        window.position = new Rect((Screen.currentResolution.width / 2) - (WINDOW_WIDTH / 2), (Screen.currentResolution.height / 2) - (WINDOW_HEIGHT / 2), WINDOW_WIDTH, WINDOW_HEIGHT);

        window.selectedAbilityData = null;
        window.abilitiesDataList = loadAbilities();

        if (boldFoldout == null)
        {
            boldFoldout = new GUIStyle(EditorStyles.foldout);
            boldFoldout.fontStyle = FontStyle.Bold;
        }
    }

    public void OnGUI()
    {
        drawAbilitiesList();
    }

    #region Abilities list
    private class cAbilityData
    {
        public string name;
        public AbilityGeneralData.Category category;
        public string description;

        public int mentalCost;
        public int physicalCost;
    }

    private bool abilitiesListVisible = true;

    private const float ABILITIES_COLUMN_WIDTH_1 = 45.0f;
    private const float ABILITIES_COLUMN_WIDTH_2 = 150.0f;
    private const float ABILITIES_COLUMN_WIDTH_3 = 100.0f;
    private const float ABILITIES_COLUMN_WIDTH_4 = 150.0f;
    private const float ABILITIES_COLUMN_WIDTH_5 = 150.0f;
    private const float ABILITIES_COLUMN_WIDTH_6 = 150.0f;

    private Vector2 abilitiesListPositionScroll;

    private bool toggleGeneralData = true;
    private bool toggleCostData = true;

    private List<cAbilityData> abilitiesDataList = new List<cAbilityData>();
    private cAbilityData selectedAbilityData = null;

    public void drawAbilitiesList()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(EditorStyles.toolbar.fixedHeight), GUILayout.ExpandWidth(true));
        if (GUILayout.Button(LOCALIZATION_DATA.getEntries()[LOCALIZATION_SAVE], EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
        {
            saveAbilities(abilitiesDataList);
        }
        if (GUILayout.Button(LOCALIZATION_DATA.getEntries()[LOCALIZATION_GENERATE], EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
        {
            generateAbilityPrefabs(abilitiesDataList);
        }
        EditorGUILayout.EndHorizontal();

        abilitiesListVisible = EditorGUILayout.Foldout(abilitiesListVisible, LOCALIZATION_DATA.getEntries()[LOCALIZATION_ABILITIES_LIST_FOLDOUT], true, boldFoldout);
        if (abilitiesListVisible)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(EditorStyles.toolbar.fixedHeight), GUILayout.ExpandWidth(true));
            if (GUILayout.Button(LOCALIZATION_DATA.getEntries()[LOCALIZATION_ADD], EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                abilitiesDataList.Add(new cAbilityData());
            }
            EditorGUILayout.EndHorizontal();

            abilitiesListPositionScroll = EditorGUILayout.BeginScrollView(abilitiesListPositionScroll, GUILayout.Height(150));
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width(ABILITIES_COLUMN_WIDTH_1));
            EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_NAME], EditorStyles.boldLabel, GUILayout.Width(ABILITIES_COLUMN_WIDTH_2));
            EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_CATEGORY], EditorStyles.boldLabel, GUILayout.Width(ABILITIES_COLUMN_WIDTH_3));
            EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_DESCRIPTION], EditorStyles.boldLabel, GUILayout.Width(ABILITIES_COLUMN_WIDTH_4));
            EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_MENTAL_COST], EditorStyles.boldLabel, GUILayout.Width(ABILITIES_COLUMN_WIDTH_5));
            EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_PHYSICAL_COST], EditorStyles.boldLabel, GUILayout.Width(ABILITIES_COLUMN_WIDTH_6));
            EditorGUILayout.EndHorizontal();

            foreach (cAbilityData abilityData in abilitiesDataList)
            {
                EditorGUILayout.BeginHorizontal();
                if(GUILayout.Button(LOCALIZATION_DATA.getEntries()[LOCALIZATION_EDIT], GUILayout.Width(ABILITIES_COLUMN_WIDTH_1)))
                {
                    selectedAbilityData = abilityData;
                }
                abilityData.name = EditorGUILayout.TextField(abilityData.name, GUILayout.Width(ABILITIES_COLUMN_WIDTH_2));
                abilityData.category = (AbilityGeneralData.Category)EditorGUILayout.EnumPopup(abilityData.category, GUILayout.Width(ABILITIES_COLUMN_WIDTH_3));
                abilityData.description = EditorGUILayout.TextField(abilityData.description, GUILayout.Width(ABILITIES_COLUMN_WIDTH_4));
                abilityData.mentalCost = EditorGUILayout.IntSlider(abilityData.mentalCost, 0, AbilityCostData.MAX_COST, GUILayout.Width(ABILITIES_COLUMN_WIDTH_5));
                abilityData.physicalCost = EditorGUILayout.IntSlider(abilityData.physicalCost, 0, AbilityCostData.MAX_COST, GUILayout.Width(ABILITIES_COLUMN_WIDTH_6));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndScrollView();

            #region General Data
            string generalDataText = LOCALIZATION_DATA.getEntries()[LOCALIZATION_GENERAL_DATA];
            if (selectedAbilityData != null)
            {
                generalDataText += " " + selectedAbilityData.name;
            }
            toggleGeneralData = EditorGUILayout.Foldout(toggleGeneralData, generalDataText, true, boldFoldout);
            if (toggleGeneralData)
            {
                if (selectedAbilityData != null)
                {
                    EditorGUI.indentLevel++;
                    selectedAbilityData.name = EditorGUILayout.TextField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_NAME], selectedAbilityData.name, GUILayout.ExpandWidth(true));
                    selectedAbilityData.category = (AbilityGeneralData.Category)EditorGUILayout.EnumPopup(LOCALIZATION_DATA.getEntries()[LOCALIZATION_CATEGORY], selectedAbilityData.category, GUILayout.ExpandWidth(true));
                    EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_DESCRIPTION]);
                    selectedAbilityData.description = EditorGUILayout.TextArea(selectedAbilityData.description, GUILayout.ExpandWidth(true), GUILayout.Height(100));
                    EditorGUI.indentLevel--;
                }
                else
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_NO_ABILITY_SELECTED]);
                    EditorGUI.indentLevel--;
                }
            }
            #endregion

            #region Cost Data
            string costDataText = LOCALIZATION_DATA.getEntries()[LOCALIZATION_COST_DATA];
            if (selectedAbilityData != null)
            {
                generalDataText += " " + selectedAbilityData.name;
            }
            toggleCostData = EditorGUILayout.Foldout(toggleCostData, costDataText, true, boldFoldout);
            if (toggleCostData)
            {
                if (selectedAbilityData != null)
                {
                    EditorGUI.indentLevel++;
                    selectedAbilityData.mentalCost = EditorGUILayout.IntSlider(LOCALIZATION_DATA.getEntries()[LOCALIZATION_MENTAL_COST], selectedAbilityData.mentalCost, 0, AbilityCostData.MAX_COST, GUILayout.ExpandWidth(true));
                    selectedAbilityData.physicalCost = EditorGUILayout.IntSlider(LOCALIZATION_DATA.getEntries()[LOCALIZATION_PHYSICAL_COST], selectedAbilityData.physicalCost, 0, AbilityCostData.MAX_COST, GUILayout.ExpandWidth(true));
                    EditorGUI.indentLevel--;
                }
                else
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_NO_ABILITY_SELECTED]);
                    EditorGUI.indentLevel--;
                }
            }
            #endregion
        }
    }
    #endregion

    #region XML Constants
    const string XML_TAG_ABILITIES = "Abilities";
    const string XML_TAG_ABILITY = "Ability";
    const string XML_TAG_NAME = "Name";
    const string XML_TAG_CATEGORY = "Category";
    const string XML_TAG_DESCRIPTION = "Description";
    const string XML_TAG_MENTAL_COST = "MentalCost";
    const string XML_TAG_PHYSICAL_COST = "PhysicalCost";
    #endregion

    #region XML logic
    const string xmlPath = "Data";
    const string xmlName = "Abilities.xml";

    private static void CreateDirectories()
    {
        if (!AssetDatabase.IsValidFolder(string.Format("{0}/{1}", Application.dataPath, xmlPath)))
            AssetDatabase.CreateFolder("", xmlPath);
    }

    #region XML save
    private static void saveAbilities(List<cAbilityData> abilitiesDataList)
    {
        CreateDirectories();

        XmlDocument xmlDocument = new XmlDocument();
        XmlNode xmlNode = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
        xmlDocument.AppendChild(xmlNode);
        XmlNode xmlHabilidades = xmlDocument.CreateElement(XML_TAG_ABILITIES);
        xmlDocument.AppendChild(xmlHabilidades);
        foreach (cAbilityData abilityData in abilitiesDataList)
        {
            XmlNode xmlHabilidad = xmlDocument.CreateElement(XML_TAG_ABILITY);
            xmlHabilidades.AppendChild(xmlHabilidad);

            XmlNode xmlHabilidadNombre = xmlDocument.CreateElement(XML_TAG_NAME);
            xmlHabilidadNombre.AppendChild(xmlDocument.CreateTextNode(abilityData.name));
            xmlHabilidad.AppendChild(xmlHabilidadNombre);

            XmlNode xmlHabilidadCategoria = xmlDocument.CreateElement(XML_TAG_CATEGORY);
            xmlHabilidadCategoria.AppendChild(xmlDocument.CreateTextNode(abilityData.category.ToString()));
            xmlHabilidad.AppendChild(xmlHabilidadCategoria);

            XmlNode xmlHabilidadDescription = xmlDocument.CreateElement(XML_TAG_DESCRIPTION);
            xmlHabilidadDescription.AppendChild(xmlDocument.CreateTextNode(abilityData.description));
            xmlHabilidad.AppendChild(xmlHabilidadDescription);

            XmlNode xmlHabilidadMentalCost = xmlDocument.CreateElement(XML_TAG_MENTAL_COST);
            xmlHabilidadMentalCost.AppendChild(xmlDocument.CreateTextNode(abilityData.mentalCost.ToString()));
            xmlHabilidad.AppendChild(xmlHabilidadMentalCost);

            XmlNode xmlHabilidadPhysicalCost = xmlDocument.CreateElement(XML_TAG_PHYSICAL_COST);
            xmlHabilidadPhysicalCost.AppendChild(xmlDocument.CreateTextNode(abilityData.physicalCost.ToString()));
            xmlHabilidad.AppendChild(xmlHabilidadPhysicalCost);
        }
        xmlDocument.Save(string.Format("{0}/{1}/{2}", Application.dataPath, xmlPath, xmlName));
    }
    #endregion

    #region XML load
    private static List<cAbilityData> loadAbilities()
    {
        List<cAbilityData> abilitesDataList = new List<cAbilityData>();

        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(string.Format("{0}/{1}/{2}", Application.dataPath, xmlPath, xmlName));
        if (xmlDocument.DocumentElement.Name == XML_TAG_ABILITIES)
        {
            foreach (XmlNode abilityNode in xmlDocument.DocumentElement.ChildNodes)
            {
                if (abilityNode.Name == XML_TAG_ABILITY)
                {
                    cAbilityData abilityData = new cAbilityData();

                    foreach (XmlNode abilityNodeChild in abilityNode.ChildNodes)
                    {
                        if (abilityNodeChild.Name == XML_TAG_NAME)
                        {
                            abilityData.name = abilityNodeChild.InnerText;
                        }

                        if (abilityNodeChild.Name == XML_TAG_CATEGORY)
                        {
                            abilityData.category = (AbilityGeneralData.Category)Enum.Parse(typeof(AbilityGeneralData.Category), abilityNodeChild.InnerText);
                        }

                        if (abilityNodeChild.Name == XML_TAG_DESCRIPTION)
                        {
                            abilityData.description = abilityNodeChild.InnerText;
                        }

                        if (abilityNodeChild.Name == XML_TAG_MENTAL_COST)
                        {
                            abilityData.mentalCost = int.Parse(abilityNodeChild.InnerText);
                        }

                        if (abilityNodeChild.Name == XML_TAG_PHYSICAL_COST)
                        {
                            abilityData.physicalCost = int.Parse(abilityNodeChild.InnerText);
                        }
                    }

                    abilitesDataList.Add(abilityData);
                }
            }
        }

        return abilitesDataList;
    }
    #endregion

    #region Prefab generation
    public const string ABILITIES_SUBPATH = "Assets/Resources";
    public const string ABILITIES_FOLDER = "Abilities";
    public const string ABILITIES_EXTENSION = "prefab";

    const string prefabsPath = "Assets/Resources/Abilities";

    private static void generateAbilityPrefabs(List<cAbilityData> abilitiesDataList)
    {
        if (!AssetDatabase.IsValidFolder(string.Format("{0}/{1}", ABILITIES_SUBPATH, ABILITIES_FOLDER)))
        {
            AssetDatabase.CreateFolder(ABILITIES_SUBPATH, ABILITIES_FOLDER);
        }

        int actual = 0;
        int total = abilitiesDataList.Count;
        float progress = 0.0f;
        foreach (cAbilityData abilityData in abilitiesDataList)
        {
            string categoryName = AbilityGeneralData.getAbilityCategoryName(abilityData.category);
            if (actual > 0)
            {
                progress = (float)actual / (float)total;
            }
            EditorUtility.DisplayProgressBar(LOCALIZATION_DATA.getEntries()[LOCALIZATION_GENERATING_PREFABS] + "...", string.Format("{0} {1} ({2}/{3})...", LOCALIZATION_DATA.getEntries()[LOCALIZATION_GENERATING_PREFAB], categoryName + "/" + abilityData.name, actual + 1, total), progress);

            string fullPath = string.Format("{0}/{1}/{2}.prefab", prefabsPath, categoryName, abilityData.name);
            if (!AssetDatabase.IsValidFolder(string.Format("{0}/{1}/{2}", ABILITIES_SUBPATH, ABILITIES_FOLDER, categoryName)))
            {
                AssetDatabase.CreateFolder(string.Format("{0}/{1}", ABILITIES_SUBPATH, ABILITIES_FOLDER), categoryName);
            }
            AssetDatabase.DeleteAsset(fullPath);

            GameObject obj = new GameObject("temp");

            Ability ability = obj.AddComponent<Ability>();

            AbilityGeneralData abilityGeneralData = obj.AddComponent<AbilityGeneralData>();
            abilityGeneralData.abilityName = abilityData.name;
            abilityGeneralData.abilityCategory = abilityData.category;
            abilityGeneralData.abilityDescription = abilityData.description;

            AbilityCostData abilityCostData = obj.AddComponent<AbilityCostData>();
            abilityCostData.abilityMentalCost = abilityData.mentalCost;
            abilityCostData.abilityPhysicalCost = abilityData.physicalCost;

            GameObject prefab = PrefabUtility.CreatePrefab(fullPath, obj);
            DestroyImmediate(obj);

            actual++;
        }

        EditorUtility.ClearProgressBar();
    }
    #endregion
    #endregion
}