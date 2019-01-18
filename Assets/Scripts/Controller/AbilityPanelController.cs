using UnityEngine;
using UnityEngine.UI;

public class AbilityPanelController : MonoBehaviour
{
    //Temporal code <--
    public GameObject gameObject;

    [ContextMenu("testing")]
    public void test()
    {
        abilityPanel.display(gameObject);
    }
    //Temporal code <--
    public AbilityPanel abilityPanel;

    public void display(GameObject obj)
    {
        abilityPanel.display(obj);
    }
}
