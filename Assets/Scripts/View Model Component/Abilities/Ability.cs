using UnityEngine;

public class Ability : MonoBehaviour
{
    public const string didPerformNotification = "Ability.DidPerformNotification";

    public void Perform()
    {
        this.PostNotification(didPerformNotification);
    }
}
