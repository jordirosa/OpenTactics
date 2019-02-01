using UnityEngine;
using UnityEngine.UI;

public class AbilityPanel : MonoBehaviour
{
    public GameObject panel;

    public Text nameLabel;
    public Text categoryLabel;
    public Text descriptionLabel;
    public GameObject levelPanel;
    public Text levelLabel;
    public GameObject CentralPanelSeparator;
    public Text bonusLabel;

    public GameObject costPanel;
    public GameObject mentalCostPanel;
    public GameObject physicalCostPanel;
    public GameObject separatorPanel;
    public Text mentalCostLabel;
    public Text physicalCostLabel;
    public Text rangeTypeLabel;
    public Text areaTypeLabel;
    //public Text powerLabel;

    public GameObject effectsPanel;
    public GameObject effectPanelPrefab;

    public void display(GameObject obj)
    {
        bool levelDataFound = false;
        bool bonusFound = false;

        string levelText = "";
        string bonusText = "";

        AbilityGeneralData generalData = obj.GetComponent<AbilityGeneralData>();
        if (generalData)
        {
            nameLabel.text = generalData.abilityName;
            categoryLabel.text = generalData.abilityCategoryText;
            descriptionLabel.text = generalData.abilityDescription;
        }

        AbilityCostData costData = obj.GetComponent<AbilityCostData>();
        if (costData)
        {
            int costPanels = 0;

            if (costData.abilityMentalCost == 0 && costData.abilityPhysicalCost == 0)
            {
                costPanel.SetActive(false);
            }
            else
            {
                costPanel.SetActive(true);
            }

            if (costData.abilityMentalCost > 0)
            {
                mentalCostPanel.SetActive(true);
                costPanels++;
            }
            else
            {
                mentalCostPanel.SetActive(false);
            }

            if (costData.abilityPhysicalCost > 0)
            {
                physicalCostPanel.SetActive(true);
                costPanels++;
            }
            else
            {
                physicalCostPanel.SetActive(false);
            }

            if (costPanels == 0)
            {
                costPanel.SetActive(false);
            }
            else
            {
                costPanel.SetActive(true);

                if (costPanels > 1)
                {
                    separatorPanel.SetActive(true);
                }
                else
                {
                    separatorPanel.SetActive(false);
                }
            }

            mentalCostLabel.text = costData.abilityMentalCost.ToString();
            physicalCostLabel.text = costData.abilityPhysicalCost.ToString();
        }

        AbilityRange abilityRange = obj.GetComponent<AbilityRange>();
        if (abilityRange)
        {
            rangeTypeLabel.text = abilityRange.getAbilityRangeDescription();
        }

        AbilityArea abilityArea = obj.GetComponent<AbilityArea>();
        if (abilityArea)
        {
            areaTypeLabel.text = abilityArea.getAbilityAreaDescription();
        }

        for (int i = 0; i < obj.transform.childCount; ++i)
        {
            Transform child = obj.transform.GetChild(i);

            if (child.name == "Bonus")
            {
                string levelChildName;

                AbilityLevelData abilityLevelData = child.GetComponent<AbilityLevelData>();

                if(abilityLevelData != null)
                {
                    levelDataFound = true;
                    levelText = "Nivel " + abilityLevelData.level.ToString();

                    levelChildName = "Level " + abilityLevelData.level;

                    for(int j = 0; j < child.transform.childCount; ++j)
                    {
                        Transform bonusChild = child.transform.GetChild(j);

                        if (bonusChild.name == levelChildName)
                        {
                            AbilityBonus[] abilityBonusList = bonusChild.GetComponents<AbilityBonus>();
                            foreach (AbilityBonus abilityBonus in abilityBonusList)
                            {
                                bonusFound = true;

                                if (bonusText != "")
                                {
                                    bonusText += "\n";
                                }
                                bonusText += abilityBonus.getAbilityBonusDescription();
                            }

                            break;
                        }
                    }
                }
            }

            foreach (Transform effectChild in effectsPanel.transform)
            {
                Destroy(effectChild.gameObject);
            }
            if (child.name == "Effects")
            {
                for (int j = 0; j < child.childCount; ++j)
                {
                    Transform childEffect = child.transform.GetChild(j);

                    GameObject effectPanelObject = Instantiate(effectPanelPrefab);
                    effectPanelObject.transform.SetParent(effectsPanel.transform);
                    effectPanelObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    effectPanelObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    EffectPanel effectPanel = effectPanelObject.GetComponent<EffectPanel>();

                    if(effectPanel)
                    {
                        AbilityPower abilityPower = childEffect.GetComponent<AbilityPower>();
                        if (abilityPower)
                        {
                            effectPanel.powerLabel.gameObject.SetActive(true);

                            effectPanel.powerLabel.text = abilityPower.getAbilityPower().ToString() + "%";
                        }
                        else
                        {
                            effectPanel.powerLabel.gameObject.SetActive(false);
                        }

                        AbilityHitRate abilityHitRate = childEffect.GetComponent<AbilityHitRate>();
                        if (abilityHitRate)
                        {
                            effectPanel.hitRateLabel.gameObject.SetActive(true);

                            effectPanel.hitRateLabel.text = abilityHitRate.getAbilityHitRateDescription();
                        }
                        else
                        {
                            effectPanel.hitRateLabel.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        if(levelDataFound)
        {
            levelPanel.gameObject.SetActive(true);
            levelLabel.text = levelText;
        }
        else
        {
            levelPanel.gameObject.SetActive(false);
        }

        if(bonusFound)
        {
            CentralPanelSeparator.SetActive(true);
            bonusLabel.gameObject.SetActive(true);
            bonusLabel.text = bonusText;
        }
        else
        {
            CentralPanelSeparator.SetActive(false);
            bonusLabel.gameObject.SetActive(false);
            bonusLabel.text = "";
        }
    }
}
