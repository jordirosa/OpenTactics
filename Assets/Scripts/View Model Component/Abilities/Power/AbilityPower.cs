using UnityEngine;

public abstract class AbilityPower : MonoBehaviour
{
    public const int MAX_POWER = 999;
    public const int MIN_POWER = 0;

    public const string getPowerCalculation = "AbilityPower.getAbilityPowerCalculation";

    public int basePower;

    public int getAbilityPower()
    {
        int calculatedPower = 0;

        ValueCalculationException exc = new ValueCalculationException(basePower);
        this.PostNotification(getPowerCalculation, exc);
        calculatedPower = Mathf.FloorToInt(exc.getModifiedValue());

        if (exc.toggle == false)
        {
            return basePower;
        }

        return calculatedPower;
    }
}