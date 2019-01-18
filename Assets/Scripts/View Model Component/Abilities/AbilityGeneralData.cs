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
    public string abilityCategoryText
    {
        get
        {
            return getAbilityCategoryName(abilityCategory);
        }
    }
    public string abilityDescription;

    #region MonoBehaviour
    void Awake()
    {
        owner = GetComponent<Ability>();
    }
    #endregion

    public static string getAbilityCategoryName(Category category)
    {
        switch (category)
        {
            case Category.COMMON:
                return "Común";
            default:
                return "";
        }
    }
}