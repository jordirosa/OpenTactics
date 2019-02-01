using UnityEngine;

public class AbilityPowerAbilityBonus : AbilityBonus
{
    public const int MIN_POWER_BONUS = 1;
    public const int MAX_POWER_BONUS = 999;

    public int powerBonus;

    private void OnEnable()
    {
        setNotificacionsPowerCalculation(levelData.level, true);
    }

    private void OnDisable()
    {
        setNotificacionsPowerCalculation(levelData.level, false);
    }

    public void setNotificacionsPowerCalculation(int level, bool enable)
    {
        if (transform.name != "Level " + level)
        {
            return;
        }

        for (int i = 0; i < transform.parent.parent.childCount; ++i)
        {
            Transform child = transform.parent.parent.GetChild(i);
            if (child.name == "Effects")
            {
                for (int j = 0; j < child.childCount; ++j)
                {
                    Transform childEffect = child.transform.GetChild(j);

                    
                    AbilityPower abilityPower = childEffect.GetComponent<AbilityPower>();

                    if (enable)
                    {
                        this.AddObserver(onGetPowerCalculation, AbilityPower.getPowerCalculation, abilityPower);
                    }
                    else
                    {
                        this.RemoveObserver(onGetPowerCalculation, AbilityPower.getPowerCalculation, abilityPower);
                    }
                }
            }
        }
    }

    public override void onLevelChanged(object sender, AbilityLevelData.LevelChangedData levelChangedData)
    {
        setNotificacionsPowerCalculation(levelChangedData.oldLevel, false);
        setNotificacionsPowerCalculation(levelChangedData.newLevel, true);
    }

    public void onGetPowerCalculation(object sender, object args)
    {
        ValueCalculationException exc = (ValueCalculationException)args;

        AddValueModifier modifier = new AddValueModifier(0, powerBonus);
        exc.addModifier(modifier);
    }

    public override string getAbilityBonusDescription()
    {
        return powerBonus.ToString() + "% bono poder.";
    }
}
