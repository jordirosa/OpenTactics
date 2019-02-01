using UnityEngine;

public abstract class AbilityBonus : MonoBehaviour
{
    public AbilityLevelData levelData;

    public abstract void onLevelChanged(object sender, AbilityLevelData.LevelChangedData levelChangedData);
    public abstract string getAbilityBonusDescription();

    #region MonoBehaviour
    void Awake()
    {
        levelData = transform.parent.GetComponent<AbilityLevelData>();

        this.AddObserver(onLevelChangedHandler, AbilityLevelData.levelChanged, levelData);
    }
    #endregion

    public void onLevelChangedHandler(object sender, object args)
    {
        AbilityLevelData.LevelChangedData levelChangedData = (AbilityLevelData.LevelChangedData)args;

        onLevelChanged(sender, levelChangedData);
    }
}
