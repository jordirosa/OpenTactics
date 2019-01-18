using UnityEngine;

public class AbilityCostData : MonoBehaviour
{
    public const int MAX_COST = 99;

    Ability owner;

    public int abilityMentalCost;
    public int abilityPhysicalCost;

    #region MonoBehaviour
    void Awake()
    {
        owner = GetComponent<Ability>();
    }
    #endregion
}
