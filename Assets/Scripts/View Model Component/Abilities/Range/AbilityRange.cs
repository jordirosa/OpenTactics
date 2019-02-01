using System.Collections.Generic;

using UnityEngine;

public abstract class AbilityRange : MonoBehaviour
{
    public const int MAX_HORIZONTAL_RANGE = 99;
    public const int MAX_VERTICAL_RANGE = 99;

    public int horizontal = 1;
    public int vertical = MAX_VERTICAL_RANGE;

    public virtual bool directionOriented { get { return false; } }

    protected Unit unit { get { return GetComponentInParent<Unit>(); } }

    public abstract List<Tile> getTilesInRange(Board board);

    public abstract string getAbilityRangeDescription();
}