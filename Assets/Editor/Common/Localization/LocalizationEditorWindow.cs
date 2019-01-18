using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEngine;

public class LocalizationEditorWindow : EditorWindow
{
    #region Localization
    private const string LOCALIZATION_RESOURCE_FILE = "LocalizationEditorWindow";

    private const string LOCALIZATION_WINDOW_TITLE = "WindowTitle";
    private const string LOCALIZATION_RESOURCE_NAME = "ResourceName";
    private const string LOCALIZATION_SAVE = "Save";
    private const string LOCALIZATION_DELETE_RESOURCE = "DeleteResource";
    private const string LOCALIZATION_NEW_ENTRY = "NewEntry";
    private const string LOCALIZATION_LANGUAGE = "Language";
    private const string LOCALIZATION_ADD_LANGUAGE = "AddLanguage";
    private const string LOCALIZATION_DELETE_LANGUAGE = "DeleteLanguage";
    private const string LOCALIZATION_RESOURCE = "Resource";
    private const string LOCALIZATION_DEFAULT = "Default";
    private const string LOCALIZATION_KEY = "Key";
    private const string LOCALIZATION_VALUE = "Value";
    private const string LOCALIZATION_NEW = "New";

    private static LocalizationData LOCALIZATION_DATA;
    #endregion

    public const int WINDOW_WIDTH = 400;
    public const int WINDOW_HEIGHT = 600;

    private static GUIContent SAVE_BUTTON;
    private static GUIContent DELETE_BUTTON;

    #region Language selector
    public static string getLanguage()
    {
        string language = "en";

        if(Menu.GetChecked("Custom Tools/Common/Language/English"))
        {
            language = "en";
        }

        if (Menu.GetChecked("Custom Tools/Common/Language/Español"))
        {
            language = "es";
        }

        return language;
    }

    [MenuItem("Custom Tools/Common/Language/English")]
    public static void English()
    {
        Menu.SetChecked("Custom Tools/Common/Language/English", true);
        Menu.SetChecked("Custom Tools/Common/Language/Español", false);
    }

    [MenuItem("Custom Tools/Common/Language/Español")]
    public static void Spanish()
    {
        Menu.SetChecked("Custom Tools/Common/Language/English", false);
        Menu.SetChecked("Custom Tools/Common/Language/Español", true);
    }
    #endregion

    [MenuItem("Custom Tools/Common/Localization editor")]
    public static void ShowWindow()
    {
        LocalizationEditorWindow window = (LocalizationEditorWindow)EditorWindow.GetWindow(typeof(LocalizationEditorWindow), false);

        LOCALIZATION_DATA = new LocalizationData(Application.dataPath, LOCALIZATION_RESOURCE_FILE, LocalizationEditorWindow.getLanguage());
        window.titleContent = new GUIContent(LOCALIZATION_DATA.getEntries()[LOCALIZATION_WINDOW_TITLE]);
        window.position = new Rect((Screen.currentResolution.width / 2) - (WINDOW_WIDTH / 2), (Screen.currentResolution.height / 2) - (WINDOW_HEIGHT / 2), WINDOW_WIDTH, WINDOW_HEIGHT);

        SAVE_BUTTON = new GUIContent(LOCALIZATION_DATA.getEntries()[LOCALIZATION_SAVE]);
        DELETE_BUTTON = new GUIContent(LOCALIZATION_DATA.getEntries()[LOCALIZATION_DELETE_RESOURCE]);

        NEW_ENTRY_BUTTON = new GUIContent(LOCALIZATION_DATA.getEntries()[LOCALIZATION_NEW_ENTRY]);
        ADD_LANGUAGE_BUTTON = new GUIContent(LOCALIZATION_DATA.getEntries()[LOCALIZATION_ADD_LANGUAGE]);
        DELETE_LANGUAGE_BUTTON = new GUIContent(LOCALIZATION_DATA.getEntries()[LOCALIZATION_DELETE_LANGUAGE]);

        window.resourcesList = window.buildResourcesList();
        window.createResource();
    }

    public void OnGUI()
    {
        drawLocalizationData();
    }

    #region Localization data
    private class cLocalizationResource
    {
        public string name;
        public bool modified;

        public List<LocalizationData> localizationDataList;
    }

    private struct sLocalizationDataChanged
    {
        public string oldKey;
        public string newKey;
        public string oldValue;
        public string newValue;
        public bool delete;
    }

    private static GUIContent NEW_ENTRY_BUTTON;
    private static GUIContent ADD_LANGUAGE_BUTTON;
    private static GUIContent DELETE_LANGUAGE_BUTTON;

    private const float LOCALIZATION_DATA_COLUMN_WIDTH_1 = 20.0f;
    private const float LOCALIZATION_DATA_COLUMN_WIDTH_2 = 100.0f;
    private const float LOCALIZATION_DATA_COLUMN_WIDTH_3 = 200.0f;

    private List<cLocalizationResource> resourcesList;

    private int selectedResource;
    private int selectedLanguage;

    private Vector2 localizationDataScrollPosition;

    public void drawLocalizationData()
    {
        int newLanguage;
        int newResource;

        List<sLocalizationDataChanged> localizationDataChanged = new List<sLocalizationDataChanged>();

        localizationDataScrollPosition = EditorGUILayout.BeginScrollView(localizationDataScrollPosition);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        newResource = EditorGUILayout.Popup(LOCALIZATION_DATA.getEntries()[LOCALIZATION_RESOURCE], selectedResource, buildResourcesPopupArray());
        if (newResource != selectedResource)
        {
            selectedLanguage = 0;
            selectedResource = newResource;

            loadResource();
        }
        if (GUILayout.Button(SAVE_BUTTON, GUILayout.ExpandWidth(false)))
        {
            handleSaveButton();
        }
        if (GUILayout.Button(DELETE_BUTTON, GUILayout.ExpandWidth(false)))
        {
            handleDeleteButton();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        newLanguage = EditorGUILayout.Popup(LOCALIZATION_DATA.getEntries()[LOCALIZATION_LANGUAGE], selectedLanguage, buildLanguagesPopupArray());
        if(newLanguage != selectedLanguage)
        {
            changeSelectedLanguage(newLanguage);
        }
        if (GUILayout.Button(ADD_LANGUAGE_BUTTON, GUILayout.ExpandWidth(false)))
        {
            handleAddLanguageButton();
        }
        if (GUILayout.Button(DELETE_LANGUAGE_BUTTON, GUILayout.ExpandWidth(false)))
        {
            handleDeleteLanguageButton();
        }
        EditorGUILayout.EndHorizontal();

        #region Main toolbar
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(EditorStyles.toolbar.fixedHeight), GUILayout.ExpandWidth(true));
        if (GUILayout.Button(NEW_ENTRY_BUTTON, EditorStyles.toolbarButton, GUILayout.Width(EditorStyles.toolbarButton.CalcSize(NEW_ENTRY_BUTTON).x)))
        {
            addEntry(GUID.Generate().ToString(), "");
        }
        EditorGUILayout.EndHorizontal();
        #endregion

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.Width(LOCALIZATION_DATA_COLUMN_WIDTH_1));
        EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_KEY], EditorStyles.boldLabel, GUILayout.Width(LOCALIZATION_DATA_COLUMN_WIDTH_2));
        EditorGUILayout.LabelField(LOCALIZATION_DATA.getEntries()[LOCALIZATION_VALUE], EditorStyles.boldLabel, GUILayout.Width(LOCALIZATION_DATA_COLUMN_WIDTH_3));
        EditorGUILayout.EndHorizontal();

        foreach (KeyValuePair<string, string> entry in resourcesList[selectedResource].localizationDataList[selectedLanguage].getEntries())
        {
            string newKey;
            string newValue;
            bool delete = false;

            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("X", GUILayout.Width(LOCALIZATION_DATA_COLUMN_WIDTH_1)))
            {
                delete = true;
            }
            newKey = EditorGUILayout.TextField(entry.Key, GUILayout.Width(LOCALIZATION_DATA_COLUMN_WIDTH_2));
            newValue = EditorGUILayout.TextField(entry.Value, GUILayout.Width(LOCALIZATION_DATA_COLUMN_WIDTH_3));

            if(newKey != entry.Key || newValue != entry.Value || delete)
            {
                sLocalizationDataChanged localizationDataChangedEntry = new sLocalizationDataChanged();
                localizationDataChangedEntry.oldKey = entry.Key;
                localizationDataChangedEntry.newKey = newKey;
                localizationDataChangedEntry.oldValue = entry.Value;
                localizationDataChangedEntry.newValue = newValue;
                localizationDataChangedEntry.delete = delete;

                localizationDataChanged.Add(localizationDataChangedEntry);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndScrollView();

        updateLocalizationDataChanged(localizationDataChanged);
    }

    public void createResource()
    {
        cLocalizationResource localizationResource = new cLocalizationResource();
        localizationResource.modified = true;
        resourcesList.Add(localizationResource);

        selectedLanguage = 0;
        selectedResource = resourcesList.Count - 1;

        LocalizationData localizationData = new LocalizationData();
        resourcesList[selectedResource].localizationDataList = new List<LocalizationData>();
        resourcesList[selectedResource].localizationDataList.Add(localizationData);
    }

    public void loadResource()
    {
        if (resourcesList[selectedResource].localizationDataList == null)
        {
            loadLocalizationResource(resourcesList[selectedResource].name);
        }
    }

    public void addLanguageToResource(string language)
    {
        if (language == null)
        {
            return;
        }

        foreach (LocalizationData localizationDataTMP in resourcesList[selectedResource].localizationDataList)
        {
            if (localizationDataTMP.language == language)
            {
                return;
            }
        }

        LocalizationData localizationData = new LocalizationData();
        localizationData.language = language;
        foreach (KeyValuePair<string, string> entry in resourcesList[selectedResource].localizationDataList[0].getEntries())
        {
            localizationData.getEntries().Add(entry.Key, entry.Value);
        }
        resourcesList[selectedResource].localizationDataList.Add(localizationData);

        selectedLanguage = resourcesList[selectedResource].localizationDataList.Count - 1;
    }

    private List<cLocalizationResource> buildResourcesList()
    {
        string[] files = Directory.GetFiles(string.Format("{0}/{1}", Application.dataPath + "/..", LocalizationData.LOCALIZATION_FULL_PATH), "*.I18N");

        List<cLocalizationResource> resourcesList = new List<cLocalizationResource>();

        foreach (string file in files)
        {
            if (file.Substring(file.Length - 8, 1) != ".")
            {
                FileInfo info = new FileInfo(file);

                cLocalizationResource localizationResource = new cLocalizationResource();
                localizationResource.name = info.Name.Substring(0, info.Name.Length - 5);
                localizationResource.modified = false;

                resourcesList.Add(localizationResource);
            }
        }

        return resourcesList;
    }

    private string[] buildResourcesPopupArray()
    {
        int i = 0;
        string[] array = new string[resourcesList.Count];

        foreach (cLocalizationResource resource in resourcesList)
        {
            array[i] = (resource.name == null) ? "<<" + LOCALIZATION_DATA.getEntries()[LOCALIZATION_NEW] + ">>" : resource.name;
            if(resource.modified)
            {
                array[i] = "*" + array[i];
            }

            i++;
        }

        return array;
    }

    private string[] buildLanguagesPopupArray()
    {
        int i = 0;
        string[] array = new string[resourcesList[selectedResource].localizationDataList.Count];

        foreach (LocalizationData localizationData in resourcesList[selectedResource].localizationDataList)
        {
            array[i] = (localizationData.language == null) ? LOCALIZATION_DATA.getEntries()[LOCALIZATION_DEFAULT] : localizationData.language;

            i++;
        }

        return array;
    }

    private void changeSelectedLanguage(int newLanguage)
    {
        selectedLanguage = newLanguage;
    }

    private void addEntry(string key, string value)
    {
        foreach(LocalizationData localizationData in resourcesList[selectedResource].localizationDataList)
        {
            localizationData.getEntries().Add(key, value);
        }

        resourcesList[selectedResource].modified = true;
    }

    private void modifyEntryKey(string oldKey, string newKey)
    {
        foreach (LocalizationData localizationData in resourcesList[selectedResource].localizationDataList)
        {
            string value = localizationData.getEntries()[oldKey];

            localizationData.getEntries().Remove(oldKey);
            localizationData.getEntries().Add(newKey, value);
        }

        resourcesList[selectedResource].modified = true;
    }

    private void modifyEntryValue(string key, string newValue)
    {
        resourcesList[selectedResource].localizationDataList[selectedLanguage].getEntries()[key] = newValue;

        resourcesList[selectedResource].modified = true;
    }

    private void deleteEntry(string key)
    {
        foreach (LocalizationData localizationData in resourcesList[selectedResource].localizationDataList)
        {
            localizationData.getEntries().Remove(key);
        }

        resourcesList[selectedResource].modified = true;
    }

    private void updateLocalizationDataChanged(List<sLocalizationDataChanged> localizationDataChanged)
    {
        foreach (sLocalizationDataChanged localizationDataChangedEntry in localizationDataChanged)
        {
            if (localizationDataChangedEntry.oldValue != localizationDataChangedEntry.newValue)
            {
                modifyEntryValue(localizationDataChangedEntry.oldKey, localizationDataChangedEntry.newValue);
            }

            if (localizationDataChangedEntry.oldKey != localizationDataChangedEntry.newKey)
            {
                modifyEntryKey(localizationDataChangedEntry.oldKey, localizationDataChangedEntry.newKey);
            }

            if (localizationDataChangedEntry.delete)
            {
                deleteEntry(localizationDataChangedEntry.newKey);
            }
        }
    }

    private void handleSaveButton()
    {
        if (resourcesList[selectedResource].name == null)
        {
            Rect popupPosition = new Rect();
            popupPosition.x = (position.width / 2) - (LocalizationResourceNamePopup.POPUP_WIDTH / 2);
            LocalizationResourceNamePopup popup = new LocalizationResourceNamePopup(this);
            PopupWindow.Show(popupPosition, popup);
        }
        else
        {
            saveLocalizationResource(resourcesList[selectedResource].name);
        }
    }

    public void handleDeleteButton()
    {
        deleteLocalizationResource(resourcesList[selectedResource].name);
    }

    private void handleAddLanguageButton()
    {
        Rect popupPosition = new Rect();
        popupPosition.x = (position.width / 2) - (LocalizationResourceLanguagePopup.POPUP_WIDTH / 2);
        LocalizationResourceLanguagePopup popup = new LocalizationResourceLanguagePopup(this);
        PopupWindow.Show(popupPosition, popup);
    }

    private void handleDeleteLanguageButton()
    {
        resourcesList[selectedResource].localizationDataList.RemoveAt(selectedLanguage);

        selectedLanguage = 0;
    }
    #endregion

    #region Delete Localization Resource
    public void deleteLocalizationResource(string resourceName)
    {
        string[] existingFiles = Directory.GetFiles(Application.dataPath + "/../" + LocalizationData.LOCALIZATION_FULL_PATH, resourceName + "*" + LocalizationData.LOCALIZATION_EXTENSION);
        foreach (string existingFile in existingFiles)
        {
            File.Delete(existingFile);
        }

        resourcesList.RemoveAt(selectedResource);
        if (resourcesList.Count == 0)
        {
            createResource();
        }
        else
        {
            selectedLanguage = 0;
            selectedResource = 0;

            loadResource();
        }
    }
    #endregion

    #region Save Localization Resource
    public void saveLocalizationResource(string resourceName)
    {
        StreamWriter file;

        string language;
        bool languageFound;

        foreach(LocalizationData localizationData in resourcesList[selectedResource].localizationDataList)
        {
            createDirectory();
            if (localizationData.language == null || localizationData.language == "")
            {
                file = createLocalizationFile(resourceName);
            }
            else
            {
                file = createLocalizationFile(resourceName, localizationData.language);
            }

            foreach (KeyValuePair<string, string> entry in localizationData.getEntries())
            {
                file.WriteLine(string.Format("\"{0}\":\"{1}\"", entry.Key, entry.Value));
            }

            file.Close();
        }

        string[] existingFiles = Directory.GetFiles(Application.dataPath + "/../" + LocalizationData.LOCALIZATION_FULL_PATH, resourceName + "*" + LocalizationData.LOCALIZATION_EXTENSION);
        foreach (string existingFile in existingFiles)
        {
            if (existingFile.Substring(existingFile.Length - 8, 1) == ".")
            {
                language = existingFile.Substring(existingFile.Length - 7, 2);
            }
            else
            {
                language = null;
            }

            languageFound = false;
            foreach (LocalizationData localizationData in resourcesList[selectedResource].localizationDataList)
            {
                if (localizationData.language == language)
                {
                    languageFound = true;
                }
            }

            if(!languageFound)
            {
                File.Delete(existingFile);
            }
        }

        resourcesList[selectedResource].name = resourceName;
        resourcesList[selectedResource].modified = false;
    }

    private void createDirectory()
    {
        if (!AssetDatabase.IsValidFolder(LocalizationData.LOCALIZATION_FULL_PATH))
        {
            AssetDatabase.CreateFolder(LocalizationData.LOCALIZATION_SUBPATH, LocalizationData.LOCALIZATION_FOLDER);
        }
    }

    private StreamWriter createLocalizationFile(string resourceName)
    {
        return new StreamWriter(LocalizationData.buildResourcePath(Application.dataPath, resourceName));
    }

    private StreamWriter createLocalizationFile(string resourceName, string language)
    {
        return new StreamWriter(LocalizationData.buildResourcePath(Application.dataPath, resourceName, language));
    }
    #endregion

    #region Load Localization Resource
    public void loadLocalizationResource(string resourceName)
    {
        resourcesList[selectedResource].localizationDataList = LocalizationData.loadAllLanguagesLocalizationData(Application.dataPath, resourceName, false);

        selectedLanguage = 0;
    }
    #endregion
}
