using UnityEditor;
using UnityEngine;

public class AbilitiesEditorWindow : EditorWindow
{
    private const string LOCALIZATION_RESOURCE = "AbilitiesEditorWindow";

    private const string LOCALIZATION_ABILITIES_LIST_FOLDOUT = "AbilitiesListFoldout";

    private LocalizationData localizationData;

    [MenuItem("Custom Tools/Abilities editor")]
    public static void ShowWindow()
    {
        AbilitiesEditorWindow window = (AbilitiesEditorWindow)EditorWindow.GetWindow(typeof(AbilitiesEditorWindow), false);

        window.localizationData = new LocalizationData(Application.dataPath, LOCALIZATION_RESOURCE, "es");
    }

    public void OnGUI()
    {
        drawAbilitiesList();
    }

    #region Abilities list
    private bool abilitiesListVisible = true;

    private const float ABILITIES_COLUMN_WIDTH_1 = 45.0f;
    private const float ABILITIES_COLUMN_WIDTH_2 = 150.0f;
    private const float ABILITIES_COLUMN_WIDTH_3 = 100.0f;

    public void drawAbilitiesList()
    {
        abilitiesListVisible = EditorGUILayout.Foldout(abilitiesListVisible, localizationData.getEntries()[LOCALIZATION_ABILITIES_LIST_FOLDOUT], true);
        if (abilitiesListVisible)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width(ABILITIES_COLUMN_WIDTH_1));
            EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.Width(ABILITIES_COLUMN_WIDTH_2));
            EditorGUILayout.LabelField("Category", EditorStyles.boldLabel, GUILayout.Width(ABILITIES_COLUMN_WIDTH_3));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Button("Edit", GUILayout.Width(ABILITIES_COLUMN_WIDTH_1));
            EditorGUILayout.TextField("Name", GUILayout.Width(ABILITIES_COLUMN_WIDTH_2));
            EditorGUILayout.EnumPopup(AbilityGeneralData.Category.COMMON,GUILayout.Width(ABILITIES_COLUMN_WIDTH_3));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }
    }
    #endregion
}