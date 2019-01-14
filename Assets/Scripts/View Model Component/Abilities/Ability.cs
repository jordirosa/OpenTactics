using UnityEngine;

public class Ability : MonoBehaviour
{
    public const string DidPerformNotification = "Ability.DidPerformNotification";

    public void Perform()
    {
        this.PostNotification(DidPerformNotification);
    }
}
