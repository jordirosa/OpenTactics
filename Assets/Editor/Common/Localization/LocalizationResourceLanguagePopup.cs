using UnityEditor;
using UnityEngine;

public class LocalizationResourceLanguagePopup : PopupWindowContent
{
    #region Localization
    private const string LOCALIZATION_RESOURCE_FILE = "LocalizationResourceLanguagePopup";

    private const string LOCALIZATION_TEXT = "Text";
    private const string LOCALIZATION_LANGUAGE = "Language";
    private const string LOCALIZATION_OK = "OK";
    private const string LOCALIZATION_CANCEL = "Cancel";

    private LocalizationData LOCALIZATION_DATA;
    #endregion

    public const int POPUP_WIDTH = 300;
    public const int POPUP_HEIGHT = 120;

    private string language;

    private LocalizationEditorWindow localizationWindow;

    public LocalizationResourceLanguagePopup(LocalizationEditorWindow localizationWindow)
    {
        this.localizationWindow = localizationWindow;

        this.LOCALIZATION_DATA = new LocalizationData(Application.dataPath, LOCALIZATION_RESOURCE_FILE, LocalizationEditorWindow.getLanguage());
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(POPUP_WIDTH, POPUP_HEIGHT);
    }

    public override void OnOpen()
    {
        language = null;
    }

    public override void OnGUI(Rect rect)
    {
        GUIStyle style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_TEXT], style, GUILayout.Height(50));
        language = EditorGUILayout.TextField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_LANGUAGE], language);
        if (GUILayout.Button(LOCALIZATION_DATA.getEntries()[LOCALIZATION_OK]))
        {
            if (language != null)
            {
                localizationWindow.addLanguageToResource(language);
                editorWindow.Close();
            }
        }
        if (GUILayout.Button(LOCALIZATION_DATA.getEntries()[LOCALIZATION_CANCEL]))
        {
            editorWindow.Close();
        }
        GUILayout.Space(70);
    }
}
