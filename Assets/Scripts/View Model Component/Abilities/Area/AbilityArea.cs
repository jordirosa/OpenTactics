using System.Collections.Generic;

using UnityEngine;

public abstract class AbilityArea : MonoBehaviour
{
    public const int MAX_HORIZONTAL_AREA = 99;
    public const int MAX_VERTICAL_AREA = 99;

    public abstract List<Tile> getTilesInArea(Board board, Point pos);

    public abstract string getAbilityAreaDescription();
}