using System.Collections.Generic;

using UnityEngine;

public class AbilityLevelData : MonoBehaviour
{
    //Temporal code <--
    public int testLevel;

    [ContextMenu("setLevel")]
    public void setLevel()
    {
        this.level = testLevel;
    }
    //Temporal code <--
    public const string levelChanged = "AbilityLevelData.levelChanged";

    public class LevelChangedData
    {
        public int oldLevel;
        public int newLevel;
    }

    private int mLevel;

    public int level
    {
        set
        {
            LevelChangedData levelChangedData = new LevelChangedData();
            levelChangedData.oldLevel = mLevel;
            levelChangedData.newLevel = value;

            mLevel = value;

            this.PostNotification(levelChanged, levelChangedData);
        }

        get
        {
            return mLevel;
        }
    }
}
