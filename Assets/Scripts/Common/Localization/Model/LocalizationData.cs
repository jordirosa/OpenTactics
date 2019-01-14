using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class LocalizationData
{
    public const string LOCALIZATION_SUBPATH = "Assets/Resources";
    public const string LOCALIZATION_FOLDER = "Localization";
    public const string LOCALIZATION_EXTENSION = "I18N";

    public static string LOCALIZATION_FULL_PATH
    {
        get
        {
            return LocalizationData.LOCALIZATION_SUBPATH + "/" + LocalizationData.LOCALIZATION_FOLDER;
        }
    }

    public string language { get; set; }

    private Dictionary<string, string> data;

    public LocalizationData()
    {
        data = new Dictionary<string, string>();
    }

    public LocalizationData(string applicationPath, string resourceName, bool unescaped = true) : this(applicationPath, resourceName, null, unescaped)
    {

    }

    public LocalizationData(string applicationPath, string resourceName, string language, bool unescaped = true) : this()
    {
        string filePath = buildResourcePath(applicationPath, resourceName, language);
        if (!File.Exists(filePath))
        {
            filePath = buildResourcePath(applicationPath, resourceName);
        }

        StreamReader file = new StreamReader(filePath);

        if (file != null)
        {
            while (!file.EndOfStream)
            {
                string entry = file.ReadLine();

                Regex regex = new Regex("(?:\")(.*?)(?:\":\")(.*?)(?:\")");
                MatchCollection matches = regex.Matches(entry);

                if (unescaped)
                {
                    data.Add(matches[0].Groups[1].Value, Regex.Unescape(matches[0].Groups[2].Value));
                }
                else
                {
                    data.Add(matches[0].Groups[1].Value, matches[0].Groups[2].Value);
                }
            }

            file.Close();
        }
    }

    public Dictionary<string, string> getEntries()
    {
        return data;
    }

    public static string buildResourcePath(string applicationPath, string resourceName)
    {
       return string.Format("{0}/{1}/{2}.{3}", applicationPath + "/..", LocalizationData.LOCALIZATION_FULL_PATH, resourceName, LocalizationData.LOCALIZATION_EXTENSION);
    }

    public static string buildResourcePath(string applicationPath, string resourceName, string language)
    {
        if (language != null)
        {
            return string.Format("{0}/{1}/{2}.{3}.{4}", applicationPath + "/..", LocalizationData.LOCALIZATION_FULL_PATH, resourceName, language, LocalizationData.LOCALIZATION_EXTENSION);
        }
        else
        {
            return buildResourcePath(applicationPath, resourceName);
        }
    }

    public static List<LocalizationData> loadAllLanguagesLocalizationData(string applicationDataPath, string resourceName, bool unescaped)
    {
        List<LocalizationData> localizationDataList = null;

        LocalizationData localizationData;

        string[] files = Directory.GetFiles(string.Format("{0}/{1}", applicationDataPath + "/..", LocalizationData.LOCALIZATION_FULL_PATH), resourceName + "*.I18N");

        if (files.Length > 0)
        {
            localizationDataList = new List<LocalizationData>();

            foreach (string file in files)
            {
                if (file.Substring(file.Length - 8, 1) == ".")
                {
                    string language = file.Substring(file.Length - 7, 2);

                    localizationData = new LocalizationData(applicationDataPath, resourceName, language, unescaped);
                    localizationData.language = language;
                    localizationDataList.Add(localizationData);
                }
                else
                {
                    localizationData = new LocalizationData(applicationDataPath, resourceName, unescaped);
                    localizationDataList.Insert(0, localizationData);
                }
            }
        }

        return localizationDataList;
    }
}
