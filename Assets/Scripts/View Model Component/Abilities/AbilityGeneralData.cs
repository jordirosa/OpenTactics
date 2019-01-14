using UnityEngine;

public class AbilityGeneralData : MonoBehaviour
{
    public enum Category
    {
        COMMON,
    }

    Ability owner;

    public string abilityName;
    public Category abilityCategory;

    #region MonoBehaviour
    void Awake()
    {
        owner = GetComponent<Ability>();
    }
    #endregion
}
