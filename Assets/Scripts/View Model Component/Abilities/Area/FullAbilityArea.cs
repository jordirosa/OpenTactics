using System.Collections.Generic;

public class FullAbilityArea : AbilityArea
{
    public override List<Tile> getTilesInArea(Board board, Point pos)
    {
        AbilityRange ar = GetComponent<AbilityRange>();
        return ar.getTilesInRange(board);
    }

    public override string getAbilityAreaDescription()
    {
        return "Completa";
    }
}