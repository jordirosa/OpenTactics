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
    private const string LOCALIZATION_RANGE_DATA = "RangeData";
    private const string LOCALIZATION_AREA_DATA = "AreaData";
    private const string LOCALIZATION_LEVEL_DATA = "LevelData";
    private const string LOCALIZATION_EFFECT_DATA = "EffectData";
    private const string LOCALIZATION_EDIT = "Edit";
    private const string LOCALIZATION_NAME = "Name";
    private const string LOCALIZATION_CATEGORY = "Category";
    private const string LOCALIZATION_DESCRIPTION = "Description";
    private const string LOCALIZATION_MENTAL_COST = "MentalCost";
    private const string LOCALIZATION_PHYSICAL_COST = "PhysicalCost";
    private const string LOCALIZATION_RANGE_TYPE = "RangeType";
    private const string LOCALIZATION_RANGE_TYPE_SELF = "RangeTypeSelf";
    private const string LOCALIZATION_RANGE_TYPE_CONSTANT = "RangeTypeConstant";
    private const string LOCALIZATION_RANGE_HORIZONTAL = "RangeHorizontal";
    private const string LOCALIZATION_RANGE_VERTICAL = "RangeVertical";
    private const string LOCALIZATION_AREA_TYPE = "AreaType";
    private const string LOCALIZATION_AREA_TYPE_SINGLE_UNIT = "AreaTypeSingleUnit";
    private const string LOCALIZATION_AREA_TYPE_EXPANSIVE_WAVE = "AreaTypeExpansiveWave";
    private const string LOCALIZATION_AREA_TYPE_FULL = "AreaTypeFull";
    private const string LOCALIZATION_AREA_HORIZONTAL = "AreaHorizontal";
    private const string LOCALIZATION_AREA_VERTICAL = "AreaVertical";
    private const string LOCALIZATION_GENERATING_PREFABS = "GeneratingPrefabs";
    private const string LOCALIZATION_GENERATING_PREFAB = "GeneratingPrefab";
    private const string LOCALIZATION_ADD_LEVEL = "AddLevel";
    private const string LOCALIZATION_DELETE_LEVEL = "DeleteLevel";
    private const string LOCALIZATION_LEVEL = "Level";
    private const string LOCALIZATION_ADD_BONUS = "AddBonus";
    private const string LOCALIZATION_DELETE_BONUS = "DeleteBonus";
    private const string LOCALIZATION_LEVEL_BONUS_TYPE = "LevelBonusType";
    private const string LOCALIZATION_LEVEL_BONUS_TYPE_ABILITY_POWER = "LevelBonusTypeAbilityPower";
    private const string LOCALIZATION_LEVEL_BONUS_TEXT_ABILITY_POWER = "LevelBonusTextAbilityPower";
    private const string LOCALIZATION_LEVEL_BONUS_POWER_BONUS = "LevelBonusPowerBonus";
    private const string LOCALIZATION_ADD_EFFECT = "AddEffect";
    private const string LOCALIZATION_DELETE_EFFECT = "DeleteEffect";
    private const string LOCALIZATION_EFFECT_POWER_TYPE = "EffectPowerType";
    private const string LOCALIZATION_EFFECT_POWER_TYPE_PHYSICAL = "EffectPowerTypePhysical";
    private const string LOCALIZATION_EFFECT_POWER_TYPE_MAGICAL = "EffectPowerTypeMagical";
    private const string LOCALIZATION_EFFECT_POWER = "EffectPower";
    private const string LOCALIZATION_EFFECT_HITRATE_TYPE = "EffectHitRateType";
    private const string LOCALIZATION_EFFECT_HITRATE_TYPE_PHYSICAL = "EffectHitRateTypePhysical";
    private const string LOCALIZATION_EFFECT_HITRATE_TYPE_MAGICAL = "EffectHitRateTypeMagical";
    private const string LOCALIZATION_EFFECT_HITRATE_TYPE_FULL = "EffectHitRateTypeFull";
    private const string LOCALIZATION_EFFECT_TYPE = "EffectType";
    private const string LOCALIZATION_EFFECT_TYPE_DAMAGE = "EffectTypeDamage";
    private const string LOCALIZATION_EFFECT_TEXT_DAMAGE = "EffectTextDamage";
    #endregion

    public const int WINDOW_WIDTH = 700;
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
    private enum ePowerTypes
    {
        PHYSICAL = 0,
        MAGICAL = 1,
    }

    private enum eHitRateTypes
    {
        PHYSICAL = 0,
        MAGICAL = 1,
        FULL = 2,
    }

    private abstract class cAbilityLevelBonus
    {
        public bool foldout = true;

        public int type;

        public cAbilityLevelBonus(int type)
        {
            this.type = type;
        }

        public abstract string getBonusName();
        public abstract cAbilityLevelBonus copy();
        public abstract void drawBonusGUI();
        public abstract void saveToXML(XmlDocument xmlDocument, XmlNode parentNode);
        public abstract void loadFromXML(XmlNode parentNode);

        protected bool openAbilityLevelGUI()
        {
            foldout = EditorGUILayout.Foldout(foldout, getBonusName(), true, boldFoldout);
            if (foldout)
            {
                EditorGUI.indentLevel++;
                this.type = EditorGUILayout.Popup(LOCALIZATION_DATA.getEntries()[LOCALIZATION_LEVEL_BONUS_TYPE], this.type, buildLevelBonusTypePopup(), GUILayout.ExpandWidth(true));
                EditorGUI.indentLevel--;
            }

            return foldout;
        }
    }

    private class cAbilityLevelBonusAbilityPower : cAbilityLevelBonus
    {
        private const string XML_TAG_POWER_BONUS = "PowerBonus";

        public int powerBonus = AbilityPowerAbilityBonus.MIN_POWER_BONUS;

        public cAbilityLevelBonusAbilityPower() : base(0)
        {
        }

        public override string getBonusName()
        {
            return LOCALIZATION_DATA.getEntries()[LOCALIZATION_LEVEL_BONUS_TEXT_ABILITY_POWER];
        }

        public override cAbilityLevelBonus copy()
        {
            cAbilityLevelBonusAbilityPower abilityLevelBonus = new cAbilityLevelBonusAbilityPower();

            abilityLevelBonus.powerBonus = powerBonus;

            return abilityLevelBonus;
        }

        public override void drawBonusGUI()
        {
            bool foldout = openAbilityLevelGUI();

            if(foldout)
            {
                EditorGUI.indentLevel++;
                this.powerBonus = EditorGUILayout.IntSlider(LOCALIZATION_DATA.getEntries()[LOCALIZATION_LEVEL_BONUS_POWER_BONUS], this.powerBonus, AbilityPowerAbilityBonus.MIN_POWER_BONUS, AbilityPowerAbilityBonus.MAX_POWER_BONUS, GUILayout.ExpandWidth(true));
                EditorGUI.indentLevel--;
            }
        }

        public override void saveToXML(XmlDocument xmlDocument, XmlNode parentNode)
        {
            XmlNode xmlPowerBonusNode = xmlDocument.CreateElement(XML_TAG_POWER_BONUS);
            xmlPowerBonusNode.AppendChild(xmlDocument.CreateTextNode(powerBonus.ToString()));
            parentNode.AppendChild(xmlPowerBonusNode);
        }

        public override void loadFromXML(XmlNode parentNode)
        {
            foreach (XmlNode xmlPowerBonusNode in parentNode.ChildNodes)
            {
                if (xmlPowerBonusNode.Name == XML_TAG_POWER_BONUS)
                {
                    powerBonus = int.Parse(xmlPowerBonusNode.InnerText);
                }
            }
        }
    }

    private class cAbilityLevelData
    {
        public bool foldout = true;

        public List<cAbilityLevelBonus> bonusList = new List<cAbilityLevelBonus>();
    }

    private abstract class cAbilityEffect
    {
        public bool foldout = true;

        public ePowerTypes powerType;
        public int power;
        public eHitRateTypes hitRateType;
        public int type;

        public cAbilityEffect(int type)
        {
            this.type = type;
        }

        public abstract string getEffectName();
        public abstract void drawEffectGUI();
        public abstract void saveToXML(XmlDocument xmlDocument, XmlNode parentNode);
        public abstract void loadFromXML(XmlNode parentNode);

        protected bool openAbilityEffectGUI()
        {
            foldout = EditorGUILayout.Foldout(foldout, getEffectName(), true, boldFoldout);
            if (foldout)
            {
                EditorGUI.indentLevel++;
                this.powerType = (ePowerTypes)EditorGUILayout.Popup(LOCALIZATION_DATA.getEntries()[LOCALIZATION_EFFECT_POWER_TYPE], (int)this.powerType, buildEffectPowerTypePopup(), GUILayout.ExpandWidth(true));
                this.power = EditorGUILayout.IntSlider(LOCALIZATION_DATA.getEntries()[LOCALIZATION_EFFECT_POWER], this.power, AbilityPower.MIN_POWER, AbilityPower.MAX_POWER, GUILayout.ExpandWidth(true));
                this.hitRateType = (eHitRateTypes)EditorGUILayout.Popup(LOCALIZATION_DATA.getEntries()[LOCALIZATION_EFFECT_HITRATE_TYPE], (int)this.hitRateType, buildEffectHitRateTypePopup(), GUILayout.ExpandWidth(true));
                this.type = EditorGUILayout.Popup(LOCALIZATION_DATA.getEntries()[LOCALIZATION_EFFECT_TYPE], this.type, buildEffectTypePopup(), GUILayout.ExpandWidth(true));
                EditorGUI.indentLevel--;
            }

            return foldout;
        }
    }

    private class cAbilityEffectDamageEffect : cAbilityEffect
    {
        public cAbilityEffectDamageEffect() : base(0)
        {
        }

        public override string getEffectName()
        {
            return LOCALIZATION_DATA.getEntries()[LOCALIZATION_EFFECT_TEXT_DAMAGE];
        }

        public override void drawEffectGUI()
        {
            bool foldout = openAbilityEffectGUI();
        }

        public override void saveToXML(XmlDocument xmlDocument, XmlNode parentNode)
        {
        }

        public override void loadFromXML(XmlNode parentNode)
        {
        }
    }

    private class cAbilityData
    {
        public string name;
        public AbilityGeneralData.Category category;
        public string description;

        public int mentalCost;
        public int physicalCost;

        public int rangeType;
        public int horizontalRange = 1;
        public int verticalRange = AbilityRange.MAX_VERTICAL_RANGE;

        public int areaType;
        public int horizontalArea = 0;
        public int verticalArea = 0;

        public Dictionary<int, cAbilityLevelData> levelBonusList = new Dictionary<int, cAbilityLevelData>();
        public List<cAbilityEffect> effectsList = new List<cAbilityEffect>();
    }

    private bool abilitiesListVisible = true;

    private const float ABILITIES_COLUMN_WIDTH_1 = 45.0f;
    private const float ABILITIES_COLUMN_WIDTH_2 = 150.0f;
    private const float ABILITIES_COLUMN_WIDTH_3 = 100.0f;
    private const float ABILITIES_COLUMN_WIDTH_4 = 150.0f;
    private const float ABILITIES_COLUMN_WIDTH_5 = 150.0f;
    private const float ABILITIES_COLUMN_WIDTH_6 = 150.0f;
    private const float ABILITIES_COLUMN_WIDTH_7 = 100.0f;
    private const float ABILITIES_COLUMN_WIDTH_8 = 150.0f;
    private const float ABILITIES_COLUMN_WIDTH_9 = 150.0f;
    private const float ABILITIES_COLUMN_WIDTH_10 = 100.0f;
    private const float ABILITIES_COLUMN_WIDTH_11 = 150.0f;
    private const float ABILITIES_COLUMN_WIDTH_12 = 150.0f;

    private Vector2 abilitiesListPositionScroll;
    private Vector2 abilitiesDetailPositionScroll;

    private bool toggleGeneralData = true;
    private bool toggleCostData = true;
    private bool toggleRangeData = true;
    private bool toggleAreaData = true;
    private bool toggleLevelData = true;
    private bool toggleEffectData = true;

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
            EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_RANGE_TYPE], EditorStyles.boldLabel, GUILayout.Width(ABILITIES_COLUMN_WIDTH_7));
            EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_RANGE_HORIZONTAL], EditorStyles.boldLabel, GUILayout.Width(ABILITIES_COLUMN_WIDTH_8));
            EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_RANGE_VERTICAL], EditorStyles.boldLabel, GUILayout.Width(ABILITIES_COLUMN_WIDTH_9));
            EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_AREA_TYPE], EditorStyles.boldLabel, GUILayout.Width(ABILITIES_COLUMN_WIDTH_10));
            EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_AREA_HORIZONTAL], EditorStyles.boldLabel, GUILayout.Width(ABILITIES_COLUMN_WIDTH_11));
            EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_AREA_VERTICAL], EditorStyles.boldLabel, GUILayout.Width(ABILITIES_COLUMN_WIDTH_12));
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
                abilityData.rangeType = EditorGUILayout.Popup(abilityData.rangeType, buildRangeTypePopoup(), GUILayout.Width(ABILITIES_COLUMN_WIDTH_7));
                if(abilityData.rangeType == 0)
                {
                    GUI.enabled = false;
                }
                abilityData.horizontalRange = EditorGUILayout.IntSlider(abilityData.horizontalRange, 0, AbilityRange.MAX_HORIZONTAL_RANGE, GUILayout.Width(ABILITIES_COLUMN_WIDTH_8));
                abilityData.verticalRange = EditorGUILayout.IntSlider(abilityData.verticalRange, 0, AbilityRange.MAX_VERTICAL_RANGE, GUILayout.Width(ABILITIES_COLUMN_WIDTH_9));
                if (abilityData.rangeType == 0)
                {
                    GUI.enabled = true;
                }
                abilityData.areaType = EditorGUILayout.Popup(abilityData.areaType, buildAreaTypePopoup(), GUILayout.Width(ABILITIES_COLUMN_WIDTH_10));
                if (abilityData.areaType == 0 || abilityData.areaType == 2)
                {
                    GUI.enabled = false;
                }
                abilityData.horizontalArea = EditorGUILayout.IntSlider(abilityData.horizontalArea, 0, AbilityArea.MAX_HORIZONTAL_AREA, GUILayout.Width(ABILITIES_COLUMN_WIDTH_11));
                abilityData.verticalArea = EditorGUILayout.IntSlider(abilityData.verticalArea, 0, AbilityArea.MAX_VERTICAL_AREA, GUILayout.Width(ABILITIES_COLUMN_WIDTH_12));
                if (abilityData.areaType == 0 || abilityData.areaType == 2)
                {
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndScrollView();

            abilitiesDetailPositionScroll = EditorGUILayout.BeginScrollView(abilitiesDetailPositionScroll);

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
                costDataText += " " + selectedAbilityData.name;
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

            #region Range Data
            string rangeDataText = LOCALIZATION_DATA.getEntries()[LOCALIZATION_RANGE_DATA];
            if (selectedAbilityData != null)
            {
                rangeDataText += " " + selectedAbilityData.name;
            }
            toggleRangeData = EditorGUILayout.Foldout(toggleRangeData, rangeDataText, true, boldFoldout);
            if (toggleRangeData)
            {
                if (selectedAbilityData != null)
                {
                    EditorGUI.indentLevel++;
                    selectedAbilityData.rangeType = EditorGUILayout.Popup(LOCALIZATION_DATA.getEntries()[LOCALIZATION_RANGE_TYPE], selectedAbilityData.rangeType, buildRangeTypePopoup(), GUILayout.ExpandWidth(true));
                    if (selectedAbilityData.rangeType == 0)
                    {
                        GUI.enabled = false;
                    }
                    selectedAbilityData.horizontalRange = EditorGUILayout.IntSlider(LOCALIZATION_DATA.getEntries()[LOCALIZATION_RANGE_HORIZONTAL], selectedAbilityData.horizontalRange, 0, AbilityRange.MAX_HORIZONTAL_RANGE, GUILayout.ExpandWidth(true));
                    selectedAbilityData.verticalRange = EditorGUILayout.IntSlider(LOCALIZATION_DATA.getEntries()[LOCALIZATION_RANGE_VERTICAL], selectedAbilityData.verticalRange, 0, AbilityRange.MAX_VERTICAL_RANGE, GUILayout.ExpandWidth(true));
                    if (selectedAbilityData.rangeType == 0)
                    {
                        GUI.enabled = true;
                    }
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

            #region Area Data
            string areaDataText = LOCALIZATION_DATA.getEntries()[LOCALIZATION_AREA_DATA];
            if (selectedAbilityData != null)
            {
                areaDataText += " " + selectedAbilityData.name;
            }
            toggleAreaData = EditorGUILayout.Foldout(toggleAreaData, areaDataText, true, boldFoldout);
            if (toggleAreaData)
            {
                if (selectedAbilityData != null)
                {
                    EditorGUI.indentLevel++;
                    selectedAbilityData.areaType = EditorGUILayout.Popup(LOCALIZATION_DATA.getEntries()[LOCALIZATION_AREA_TYPE], selectedAbilityData.areaType, buildAreaTypePopoup(), GUILayout.ExpandWidth(true));
                    if (selectedAbilityData.areaType == 0 || selectedAbilityData.areaType == 2)
                    {
                        GUI.enabled = false;
                    }
                    selectedAbilityData.horizontalArea = EditorGUILayout.IntSlider(LOCALIZATION_DATA.getEntries()[LOCALIZATION_AREA_HORIZONTAL], selectedAbilityData.horizontalArea, 0, AbilityArea.MAX_HORIZONTAL_AREA, GUILayout.ExpandWidth(true));
                    selectedAbilityData.verticalArea = EditorGUILayout.IntSlider(LOCALIZATION_DATA.getEntries()[LOCALIZATION_AREA_VERTICAL], selectedAbilityData.verticalArea, 0, AbilityArea.MAX_VERTICAL_AREA, GUILayout.ExpandWidth(true));
                    if (selectedAbilityData.areaType == 0 || selectedAbilityData.areaType == 2)
                    {
                        GUI.enabled = true;
                    }
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

            #region Level Data
            string levelDataText = LOCALIZATION_DATA.getEntries()[LOCALIZATION_LEVEL_DATA];
            if (selectedAbilityData != null)
            {
                levelDataText += " " + selectedAbilityData.name;
            }
            toggleLevelData = EditorGUILayout.Foldout(toggleLevelData, levelDataText, true, boldFoldout);
            if (toggleLevelData)
            {
                if (selectedAbilityData != null)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(EditorStyles.toolbar.fixedHeight), GUILayout.ExpandWidth(true));
                    if (GUILayout.Button(LOCALIZATION_DATA.getEntries()[LOCALIZATION_ADD_LEVEL], EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
                    {
                        cAbilityLevelData abilityLevelData = new cAbilityLevelData();

                        if (selectedAbilityData.levelBonusList.Count > 0)
                        {
                            foreach(cAbilityLevelBonus abilityBonus in selectedAbilityData.levelBonusList[selectedAbilityData.levelBonusList.Count].bonusList)
                            {
                                abilityLevelData.bonusList.Add(abilityBonus.copy());
                            }
                        }

                        selectedAbilityData.levelBonusList.Add(selectedAbilityData.levelBonusList.Count + 1, abilityLevelData);
                    }
                    if (GUILayout.Button(LOCALIZATION_DATA.getEntries()[LOCALIZATION_DELETE_LEVEL], EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
                    {
                        selectedAbilityData.levelBonusList.Remove(selectedAbilityData.levelBonusList.Count);
                    }
                    EditorGUILayout.EndHorizontal();

                    foreach (KeyValuePair<int, cAbilityLevelData> levelBonusKeyPair in selectedAbilityData.levelBonusList)
                    {
                        levelBonusKeyPair.Value.foldout = EditorGUILayout.Foldout(levelBonusKeyPair.Value.foldout, LOCALIZATION_DATA.getEntries()[LOCALIZATION_LEVEL] + " " + levelBonusKeyPair.Key, true, boldFoldout);
                        if(levelBonusKeyPair.Value.foldout)
                        {
                            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(EditorStyles.toolbar.fixedHeight), GUILayout.ExpandWidth(true));
                            if (GUILayout.Button(LOCALIZATION_DATA.getEntries()[LOCALIZATION_ADD_BONUS], EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
                            {
                                levelBonusKeyPair.Value.bonusList.Add(new cAbilityLevelBonusAbilityPower());
                            }
                            if (GUILayout.Button(LOCALIZATION_DATA.getEntries()[LOCALIZATION_DELETE_BONUS], EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
                            {

                            }
                            EditorGUILayout.EndHorizontal();

                            foreach(cAbilityLevelBonus abilityLevelBonus in levelBonusKeyPair.Value.bonusList)
                            {
                                abilityLevelBonus.drawBonusGUI();
                            }
                        }
                    }
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

            #region Effect Data
            string effectDataText = LOCALIZATION_DATA.getEntries()[LOCALIZATION_EFFECT_DATA];
            if (selectedAbilityData != null)
            {
                effectDataText += " " + selectedAbilityData.name;
            }
            toggleEffectData = EditorGUILayout.Foldout(toggleEffectData, effectDataText, true, boldFoldout);
            if (toggleEffectData)
            {
                if (selectedAbilityData != null)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(EditorStyles.toolbar.fixedHeight), GUILayout.ExpandWidth(true));
                    if (GUILayout.Button(LOCALIZATION_DATA.getEntries()[LOCALIZATION_ADD_EFFECT], EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
                    {
                        selectedAbilityData.effectsList.Add(new cAbilityEffectDamageEffect());
                    }
                    if (GUILayout.Button(LOCALIZATION_DATA.getEntries()[LOCALIZATION_DELETE_EFFECT], EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
                    {
                        //selectedAbilityData.levelBonusList.Remove(selectedAbilityData.levelBonusList.Count);
                    }
                    EditorGUILayout.EndHorizontal();

                    foreach(cAbilityEffect abilityEffect in selectedAbilityData.effectsList)
                    {
                        abilityEffect.drawEffectGUI();
                    }
                }
                else
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_NO_ABILITY_SELECTED]);
                    EditorGUI.indentLevel--;
                }
            }
            #endregion

            EditorGUILayout.EndScrollView();
        }
    }

    private static string[] buildRangeTypePopoup()
    {
        string[] options = new string[2];

        options[0] = LOCALIZATION_DATA.getEntries()[LOCALIZATION_RANGE_TYPE_SELF];
        options[1] = LOCALIZATION_DATA.getEntries()[LOCALIZATION_RANGE_TYPE_CONSTANT];

        return options;
    }

    private static string[] buildAreaTypePopoup()
    {
        string[] options = new string[3];

        options[0] = LOCALIZATION_DATA.getEntries()[LOCALIZATION_AREA_TYPE_SINGLE_UNIT];
        options[1] = LOCALIZATION_DATA.getEntries()[LOCALIZATION_AREA_TYPE_EXPANSIVE_WAVE];
        options[2] = LOCALIZATION_DATA.getEntries()[LOCALIZATION_AREA_TYPE_FULL];

        return options;
    }

    private static string[] buildLevelBonusTypePopup()
    {
        string[] options = new string[1];

        options[0] = LOCALIZATION_DATA.getEntries()[LOCALIZATION_LEVEL_BONUS_TYPE_ABILITY_POWER];

        return options;
    }

    private static string[] buildEffectPowerTypePopup()
    {
        string[] options = new string[2];

        options[(int)ePowerTypes.PHYSICAL] = LOCALIZATION_DATA.getEntries()[LOCALIZATION_EFFECT_POWER_TYPE_PHYSICAL];
        options[(int)ePowerTypes.MAGICAL] = LOCALIZATION_DATA.getEntries()[LOCALIZATION_EFFECT_POWER_TYPE_MAGICAL];

        return options;
    }

    private static string[] buildEffectHitRateTypePopup()
    {
        string[] options = new string[3];

        options[(int)eHitRateTypes.PHYSICAL] = LOCALIZATION_DATA.getEntries()[LOCALIZATION_EFFECT_HITRATE_TYPE_PHYSICAL];
        options[(int)eHitRateTypes.MAGICAL] = LOCALIZATION_DATA.getEntries()[LOCALIZATION_EFFECT_HITRATE_TYPE_MAGICAL];
        options[(int)eHitRateTypes.FULL] = LOCALIZATION_DATA.getEntries()[LOCALIZATION_EFFECT_HITRATE_TYPE_FULL];

        return options;
    }

    private static string[] buildEffectTypePopup()
    {
        string[] options = new string[1];

        options[0] = LOCALIZATION_DATA.getEntries()[LOCALIZATION_EFFECT_TYPE_DAMAGE];

        return options;
    }
    #endregion

    #region XML logic
    #region XML Constants
    const string XML_TAG_ABILITIES = "Abilities";
    const string XML_TAG_ABILITY = "Ability";
    const string XML_TAG_TYPE = "Type";
    const string XML_TAG_HORIZONTAL = "Horizontal";
    const string XML_TAG_VERTICAL = "Vertical";
    const string XML_TAG_NAME = "Name";
    const string XML_TAG_CATEGORY = "Category";
    const string XML_TAG_DESCRIPTION = "Description";
    const string XML_TAG_MENTAL_COST = "MentalCost";
    const string XML_TAG_PHYSICAL_COST = "PhysicalCost";
    const string XML_TAG_RANGE = "Range";
    const string XML_TAG_AREA = "Area";
    const string XML_TAG_LEVELS = "Levels";
    const string XML_TAG_LEVEL = "Level";
    const string XML_TAG_VALUE = "Value";
    const string XML_TAG_BONUS_LIST = "BonusList";
    const string XML_TAG_BONUS = "Bonus";
    const string XML_TAG_EFFECTS = "Effects";
    const string XML_TAG_EFFECT = "Effect";
    const string XML_TAG_POWER = "Power";
    const string XML_TAG_HITRATE = "HitRate";

    const string xmlPath = "Data";
    const string xmlName = "Abilities.xml";
    #endregion

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
        XmlNode xmlAbiltiesNode = xmlDocument.CreateElement(XML_TAG_ABILITIES);
        xmlDocument.AppendChild(xmlAbiltiesNode);
        foreach (cAbilityData abilityData in abilitiesDataList)
        {
            XmlNode xmlAbilityNode = xmlDocument.CreateElement(XML_TAG_ABILITY);
            xmlAbiltiesNode.AppendChild(xmlAbilityNode);

            XmlNode xmlAbilityName = xmlDocument.CreateElement(XML_TAG_NAME);
            xmlAbilityName.AppendChild(xmlDocument.CreateTextNode(abilityData.name));
            xmlAbilityNode.AppendChild(xmlAbilityName);

            XmlNode xmlAbilityCategory = xmlDocument.CreateElement(XML_TAG_CATEGORY);
            xmlAbilityCategory.AppendChild(xmlDocument.CreateTextNode(abilityData.category.ToString()));
            xmlAbilityNode.AppendChild(xmlAbilityCategory);

            XmlNode xmlAbilityDescription = xmlDocument.CreateElement(XML_TAG_DESCRIPTION);
            xmlAbilityDescription.AppendChild(xmlDocument.CreateTextNode(abilityData.description));
            xmlAbilityNode.AppendChild(xmlAbilityDescription);

            XmlNode xmlAbilityMentalCost = xmlDocument.CreateElement(XML_TAG_MENTAL_COST);
            xmlAbilityMentalCost.AppendChild(xmlDocument.CreateTextNode(abilityData.mentalCost.ToString()));
            xmlAbilityNode.AppendChild(xmlAbilityMentalCost);

            XmlNode xmlAbilityPhysicalCost = xmlDocument.CreateElement(XML_TAG_PHYSICAL_COST);
            xmlAbilityPhysicalCost.AppendChild(xmlDocument.CreateTextNode(abilityData.physicalCost.ToString()));
            xmlAbilityNode.AppendChild(xmlAbilityPhysicalCost);

            XmlNode xmlAbilityRange = xmlDocument.CreateElement(XML_TAG_RANGE);
            XmlAttribute xmlRangeType = xmlDocument.CreateAttribute(XML_TAG_TYPE);
            xmlRangeType.Value = abilityData.rangeType.ToString();
            xmlAbilityRange.Attributes.SetNamedItem(xmlRangeType);
            xmlAbilityNode.AppendChild(xmlAbilityRange);

            if (abilityData.rangeType != 0)
            {
                XmlNode xmlAbilityRangeHorizontal = xmlDocument.CreateElement(XML_TAG_HORIZONTAL);
                xmlAbilityRangeHorizontal.AppendChild(xmlDocument.CreateTextNode(abilityData.horizontalRange.ToString()));
                xmlAbilityRange.AppendChild(xmlAbilityRangeHorizontal);

                XmlNode xmlAbilityRangeVertical = xmlDocument.CreateElement(XML_TAG_VERTICAL);
                xmlAbilityRangeVertical.AppendChild(xmlDocument.CreateTextNode(abilityData.verticalRange.ToString()));
                xmlAbilityRange.AppendChild(xmlAbilityRangeVertical);
            }

            XmlNode xmlAbilityArea = xmlDocument.CreateElement(XML_TAG_AREA);
            XmlAttribute xmlAreaType = xmlDocument.CreateAttribute(XML_TAG_TYPE);
            xmlAreaType.Value = abilityData.areaType.ToString();
            xmlAbilityArea.Attributes.SetNamedItem(xmlAreaType);
            xmlAbilityNode.AppendChild(xmlAbilityArea);

            if (abilityData.areaType != 0 && abilityData.areaType != 2)
            {
                XmlNode xmlAbilityAreaHorizontal = xmlDocument.CreateElement(XML_TAG_HORIZONTAL);
                xmlAbilityAreaHorizontal.AppendChild(xmlDocument.CreateTextNode(abilityData.horizontalArea.ToString()));
                xmlAbilityArea.AppendChild(xmlAbilityAreaHorizontal);

                XmlNode xmlAbilityAreaVertical = xmlDocument.CreateElement(XML_TAG_VERTICAL);
                xmlAbilityAreaVertical.AppendChild(xmlDocument.CreateTextNode(abilityData.verticalArea.ToString()));
                xmlAbilityArea.AppendChild(xmlAbilityAreaVertical);
            }

            if(abilityData.levelBonusList.Count > 0)
            {
                XmlNode xmlLevelsNode = xmlDocument.CreateElement(XML_TAG_LEVELS);
                xmlAbilityNode.AppendChild(xmlLevelsNode);

                foreach(KeyValuePair<int, cAbilityLevelData> abilityLevelData in abilityData.levelBonusList)
                {
                    XmlNode xmlLevelNode = xmlDocument.CreateElement(XML_TAG_LEVEL);
                    XmlAttribute xmlLevelValue = xmlDocument.CreateAttribute(XML_TAG_VALUE);
                    xmlLevelValue.Value = abilityLevelData.Key.ToString();
                    xmlLevelNode.Attributes.SetNamedItem(xmlLevelValue);
                    xmlLevelsNode.AppendChild(xmlLevelNode);

                    if (abilityLevelData.Value.bonusList.Count > 0)
                    {
                        XmlNode xmlBonusListNode = xmlDocument.CreateElement(XML_TAG_BONUS_LIST);
                        xmlLevelNode.AppendChild(xmlBonusListNode);

                        foreach(cAbilityLevelBonus abilityLevelBonus in abilityLevelData.Value.bonusList)
                        {
                            XmlNode xmlBonusNode = xmlDocument.CreateElement(XML_TAG_BONUS);
                            XmlAttribute xmlBonusType = xmlDocument.CreateAttribute(XML_TAG_TYPE);
                            xmlBonusType.Value = abilityLevelBonus.type.ToString();
                            xmlBonusNode.Attributes.SetNamedItem(xmlBonusType);
                            xmlBonusListNode.AppendChild(xmlBonusNode);

                            abilityLevelBonus.saveToXML(xmlDocument, xmlBonusNode);
                        }
                    }
                }
            }

            if(abilityData.effectsList.Count > 0)
            {
                XmlNode xmlEffectsNode = xmlDocument.CreateElement(XML_TAG_EFFECTS);
                xmlAbilityNode.AppendChild(xmlEffectsNode);

                foreach(cAbilityEffect abilityEffect in abilityData.effectsList)
                {
                    XmlNode xmlEffectNode = xmlDocument.CreateElement(XML_TAG_EFFECT);
                    XmlAttribute xmlEffectType = xmlDocument.CreateAttribute(XML_TAG_TYPE);
                    xmlEffectType.Value = abilityEffect.type.ToString();
                    xmlEffectNode.Attributes.SetNamedItem(xmlEffectType);
                    xmlEffectsNode.AppendChild(xmlEffectNode);

                    XmlNode xmlEffectPowerNode = xmlDocument.CreateElement(XML_TAG_POWER);
                    xmlEffectPowerNode.AppendChild(xmlDocument.CreateTextNode(abilityEffect.power.ToString()));
                    XmlAttribute xmlPowerType = xmlDocument.CreateAttribute(XML_TAG_TYPE);
                    xmlPowerType.Value = abilityEffect.powerType.ToString();
                    xmlEffectPowerNode.Attributes.SetNamedItem(xmlPowerType);
                    xmlEffectNode.AppendChild(xmlEffectPowerNode);

                    XmlNode xmlEffectHitRateNode = xmlDocument.CreateElement(XML_TAG_HITRATE);
                    XmlAttribute xmlHitRateType = xmlDocument.CreateAttribute(XML_TAG_TYPE);
                    xmlHitRateType.Value = abilityEffect.hitRateType.ToString();
                    xmlEffectHitRateNode.Attributes.SetNamedItem(xmlHitRateType);
                    xmlEffectNode.AppendChild(xmlEffectHitRateNode);

                    abilityEffect.saveToXML(xmlDocument, xmlEffectNode);
                }
            }
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

                        if (abilityNodeChild.Name == XML_TAG_RANGE)
                        {
                            foreach (XmlAttribute nodeTipoRangoAttr in abilityNodeChild.Attributes)
                            {
                                if (nodeTipoRangoAttr.Name == XML_TAG_TYPE)
                                {
                                    abilityData.rangeType = int.Parse(nodeTipoRangoAttr.Value);
                                }
                            }

                            if (abilityData.rangeType != 0)
                            {
                                foreach (XmlNode nodeTipoRangoHijo in abilityNodeChild.ChildNodes)
                                {
                                    if (nodeTipoRangoHijo.Name == XML_TAG_HORIZONTAL)
                                    {
                                        abilityData.horizontalRange = int.Parse(nodeTipoRangoHijo.InnerText);
                                    }

                                    if (nodeTipoRangoHijo.Name == XML_TAG_VERTICAL)
                                    {
                                        abilityData.verticalRange = int.Parse(nodeTipoRangoHijo.InnerText);
                                    }
                                }
                            }
                        }

                        if (abilityNodeChild.Name == XML_TAG_AREA)
                        {
                            foreach (XmlAttribute nodeTipoRangoAttr in abilityNodeChild.Attributes)
                            {
                                if (nodeTipoRangoAttr.Name == XML_TAG_TYPE)
                                {
                                    abilityData.areaType = int.Parse(nodeTipoRangoAttr.Value);
                                }
                            }

                            if (abilityData.areaType != 0 && abilityData.areaType != 2)
                            {
                                foreach (XmlNode nodeTipoRangoHijo in abilityNodeChild.ChildNodes)
                                {
                                    if (nodeTipoRangoHijo.Name == XML_TAG_HORIZONTAL)
                                    {
                                        abilityData.horizontalArea = int.Parse(nodeTipoRangoHijo.InnerText);
                                    }

                                    if (nodeTipoRangoHijo.Name == XML_TAG_VERTICAL)
                                    {
                                        abilityData.verticalArea = int.Parse(nodeTipoRangoHijo.InnerText);
                                    }
                                }
                            }
                        }

                        if (abilityNodeChild.Name == XML_TAG_LEVELS)
                        {
                            foreach (XmlNode nodeLevel in abilityNodeChild.ChildNodes)
                            {
                                if (nodeLevel.Name == XML_TAG_LEVEL)
                                {
                                    cAbilityLevelData abilityLevelData = new cAbilityLevelData();

                                    foreach (XmlAttribute nodeLevelAttr in nodeLevel.Attributes)
                                    {
                                        if (nodeLevelAttr.Name == XML_TAG_VALUE)
                                        {
                                            abilityData.levelBonusList.Add(int.Parse(nodeLevelAttr.Value), abilityLevelData);
                                        }
                                    }

                                    foreach (XmlNode nodeLevelHijo in nodeLevel.ChildNodes)
                                    {
                                        if (nodeLevelHijo.Name == XML_TAG_BONUS_LIST)
                                        {
                                            foreach (XmlNode nodeBonus in nodeLevelHijo.ChildNodes)
                                            {
                                                if (nodeBonus.Name == XML_TAG_BONUS)
                                                {
                                                    foreach (XmlAttribute nodeBonusAttr in nodeBonus.Attributes)
                                                    {
                                                        if (nodeBonusAttr.Name == XML_TAG_TYPE)
                                                        {
                                                            cAbilityLevelBonus abilityLevelBonus = null;

                                                            switch (int.Parse(nodeBonusAttr.Value))
                                                            {
                                                                case 0:
                                                                    abilityLevelBonus = new cAbilityLevelBonusAbilityPower();
                                                                    abilityLevelBonus.loadFromXML(nodeBonus);
                                                                    break;
                                                            }

                                                            if (abilityLevelBonus != null)
                                                            {
                                                                abilityLevelData.bonusList.Add(abilityLevelBonus);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (abilityNodeChild.Name == XML_TAG_EFFECTS)
                        {
                            foreach (XmlNode nodeEffect in abilityNodeChild.ChildNodes)
                            {
                                if (nodeEffect.Name == XML_TAG_EFFECT)
                                {
                                    cAbilityEffect abilityEffect = null;

                                    foreach (XmlAttribute effectAttr in nodeEffect.Attributes)
                                    {
                                        if (effectAttr.Name == XML_TAG_TYPE)
                                        {
                                            switch (int.Parse(effectAttr.Value))
                                            {
                                                case 0:
                                                    abilityEffect = new cAbilityEffectDamageEffect();
                                                    abilityEffect.loadFromXML(nodeEffect);
                                                    break;
                                            }
                                        }
                                    }

                                    foreach (XmlNode nodeEffectHijo in nodeEffect.ChildNodes)
                                    {
                                        if (nodeEffectHijo.Name == XML_TAG_POWER)
                                        {
                                            abilityEffect.power = int.Parse(nodeEffectHijo.InnerText);

                                            foreach (XmlAttribute powerAttr in nodeEffectHijo.Attributes)
                                            {
                                                if (powerAttr.Name == XML_TAG_TYPE)
                                                {
                                                    abilityEffect.powerType = (ePowerTypes)Enum.Parse(typeof(ePowerTypes), powerAttr.Value);
                                                }
                                            }
                                        }

                                        if (nodeEffectHijo.Name == XML_TAG_HITRATE)
                                        {
                                            foreach (XmlAttribute hitRateAttr in nodeEffectHijo.Attributes)
                                            {
                                                if (hitRateAttr.Name == XML_TAG_TYPE)
                                                {
                                                    abilityEffect.hitRateType = (eHitRateTypes)Enum.Parse(typeof(eHitRateTypes), hitRateAttr.Value);
                                                }
                                            }
                                        }
                                    }

                                    if (abilityEffect != null)
                                    {
                                        abilityData.effectsList.Add(abilityEffect);
                                    }
                                }
                            }
                        }
                    }

                    abilitesDataList.Add(abilityData);
                }
            }
        }

        return abilitesDataList;
    }
    #endregion
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
                progress = (float)actual / (float)(total - 1);
            }
            EditorUtility.DisplayProgressBar(LOCALIZATION_DATA.getEntries()[LOCALIZATION_GENERATING_PREFABS] + "...", string.Format("{0} {1} ({2}/{3})...", LOCALIZATION_DATA.getEntries()[LOCALIZATION_GENERATING_PREFAB], categoryName + "/" + abilityData.name, actual + 1, total), progress);

            string fullPath = string.Format("{0}/{1}/{2}.prefab", prefabsPath, abilityData.category.ToString(), abilityData.name);
            if (!AssetDatabase.IsValidFolder(string.Format("{0}/{1}/{2}", ABILITIES_SUBPATH, ABILITIES_FOLDER, abilityData.category.ToString())))
            {
                AssetDatabase.CreateFolder(string.Format("{0}/{1}", ABILITIES_SUBPATH, ABILITIES_FOLDER), abilityData.category.ToString());
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

            AbilityRange abilityRange = null;
            switch (abilityData.rangeType)
            {
                case 0:
                    abilityRange = obj.AddComponent<SelfAbilityRange>();
                    abilityRange.horizontal = 0;
                    abilityRange.vertical = 0;
                    break;
                case 1:
                    abilityRange = obj.AddComponent<ConstantAbilityRange>();
                    abilityRange.horizontal = abilityData.horizontalRange;
                    abilityRange.vertical = abilityData.verticalRange;
                    break;
            }

            AbilityArea abilityArea = null;
            switch (abilityData.areaType)
            {
                case 0:
                    abilityArea = obj.AddComponent<SingleUnitAbilityArea>();
                    break;
                case 1:
                    abilityArea = obj.AddComponent<ExpansiveWaveAbilityArea>();
                    ((ExpansiveWaveAbilityArea)abilityArea).horizontal = abilityData.horizontalArea;
                    ((ExpansiveWaveAbilityArea)abilityArea).vertical = abilityData.verticalArea;
                    break;
                case 2:
                    abilityArea = obj.AddComponent<FullAbilityArea>();
                    break;
            }

            if(abilityData.levelBonusList.Count > 0)
            {
                GameObject gameObjectBonus = new GameObject("Bonus");
                gameObjectBonus.transform.SetParent(obj.transform);

                AbilityLevelData abilityLevelData = gameObjectBonus.AddComponent<AbilityLevelData>();

                foreach (KeyValuePair<int, cAbilityLevelData> keyPairLevelData in abilityData.levelBonusList)
                {
                    GameObject gameObjectLevel = new GameObject("Level " + keyPairLevelData.Key);
                    gameObjectLevel.transform.SetParent(gameObjectBonus.transform);

                    foreach (cAbilityLevelBonus levelBonus in keyPairLevelData.Value.bonusList)
                    {
                        switch (levelBonus.type)
                        {
                            case 0:
                                AbilityPowerAbilityBonus abilityBonus = gameObjectLevel.AddComponent<AbilityPowerAbilityBonus>();
                                abilityBonus.powerBonus = ((cAbilityLevelBonusAbilityPower)levelBonus).powerBonus;
                                break;
                        }
                    }
                }
            }

            if (abilityData.effectsList.Count > 0)
            {
                GameObject gameObjectEffects = new GameObject("Effects");
                gameObjectEffects.transform.SetParent(obj.transform);

                foreach (cAbilityEffect abilityEffect in abilityData.effectsList)
                {
                    GameObject gameObjectEffect = new GameObject(abilityEffect.getEffectName());
                    gameObjectEffect.transform.SetParent(gameObjectEffects.transform);

                    switch(abilityEffect.powerType)
                    {
                        case ePowerTypes.PHYSICAL:
                            PhysicalAbilityPower physicalAbilityPower = gameObjectEffect.AddComponent<PhysicalAbilityPower>();
                            physicalAbilityPower.basePower = abilityEffect.power;
                            break;
                        case ePowerTypes.MAGICAL:
                            MagicalAbilityPower magicalAbilityPower = gameObjectEffect.AddComponent<MagicalAbilityPower>();
                            magicalAbilityPower.basePower = abilityEffect.power;
                            break;
                    }

                    switch(abilityEffect.hitRateType)
                    {
                        case eHitRateTypes.PHYSICAL:
                            gameObjectEffect.AddComponent<PhysicalAbilityHitRate>();
                            break;
                        case eHitRateTypes.MAGICAL:
                            gameObjectEffect.AddComponent<MagicalAbilityHitRate>();
                            break;
                        case eHitRateTypes.FULL:
                            gameObjectEffect.AddComponent<FullAbilityHitRate>();
                            break;
                    }

                    switch (abilityEffect.type)
                    {
                        case 0:
                            DamageAbilityEffect damageEffect = gameObjectEffect.AddComponent<DamageAbilityEffect>();
                            break;
                    }
                }
            }

            GameObject prefab = PrefabUtility.CreatePrefab(fullPath, obj);
            DestroyImmediate(obj);

            actual++;
        }

        EditorUtility.ClearProgressBar();
    }
    #endregion
}