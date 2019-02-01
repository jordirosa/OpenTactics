using UnityEngine;

public class Stats : MonoBehaviour
{
    public enum StatTypes
    {
        PHYSICAL_ATTACK,
        PHYSICAL_DEFENSE,
        PHYSICAL_ACCURACY,
        PHYSICAL_EVASION,
        MAGICAL_ATTACK,
        MAGICAL_DEFENSE,
        MAGICAL_ACCURACY,
        MAGICAL_EVASION
    }

    private int physicalAttack;
    private int physicalDefense;
    private int physicalAccuracy;
    private int physicalEvasion;
    private int magicalAttack;
    private int magicalDefense;
    private int magicalAccuracy;
    private int magicalEvasion;

    public void setValue(StatTypes type, int value)
    {
        switch(type)
        {
            case StatTypes.PHYSICAL_ATTACK:
                physicalAttack = value;
                break;
            case StatTypes.PHYSICAL_DEFENSE:
                physicalDefense = value;
                break;
            case StatTypes.PHYSICAL_ACCURACY:
                physicalAccuracy = value;
                break;
            case StatTypes.PHYSICAL_EVASION:
                physicalEvasion = value;
                break;
            case StatTypes.MAGICAL_ATTACK:
                magicalAttack = value;
                break;
            case StatTypes.MAGICAL_DEFENSE:
                magicalDefense = value;
                break;
            case StatTypes.MAGICAL_ACCURACY:
                magicalAccuracy = value;
                break;
            case StatTypes.MAGICAL_EVASION:
                magicalEvasion = value;
                break;
        }
    }
}