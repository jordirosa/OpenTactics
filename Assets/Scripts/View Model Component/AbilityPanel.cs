using UnityEngine;
using UnityEngine.UI;

public class AbilityPanel : MonoBehaviour
{
    public GameObject panel;

    public Text nameLabel;
    public Text categoryLabel;
    public Text descriptionLabel;

    public GameObject costPanel;
    public GameObject mentalCostPanel;
    public GameObject physicalCostPanel;
    public GameObject separatorPanel;
    public Text mentalCostLabel;
    public Text physicalCostLabel;

    public void display(GameObject obj)
    {
        AbilityGeneralData generalData = obj.GetComponent<AbilityGeneralData>();
        if (generalData)
        {
            nameLabel.text = generalData.abilityName;
            categoryLabel.text = generalData.abilityCategoryText;
            descriptionLabel.text = generalData.abilityDescription;
        }

        AbilityCostData costData = obj.GetComponent<AbilityCostData>();
        if(costData)
        {
            int costPanels = 0;

            if(costData.abilityMentalCost == 0 && costData.abilityPhysicalCost == 0)
            {
                costPanel.SetActive(false);
            }
            else
            {
                costPanel.SetActive(true);
            }

            if(costData.abilityMentalCost > 0)
            {
                mentalCostPanel.SetActive(true);
                costPanels++;
            }
            else
            {
                mentalCostPanel.SetActive(false);
            }

            if(costData.abilityPhysicalCost > 0)
            {
                physicalCostPanel.SetActive(true);
                costPanels++;
            }
            else
            {
                physicalCostPanel.SetActive(false);
            }

            if(costPanels == 0)
            {
                costPanel.SetActive(false);
            }
            else
            {
                costPanel.SetActive(true);

                if(costPanels > 1)
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
    }
}
