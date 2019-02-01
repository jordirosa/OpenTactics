using System.Collections.Generic;

public class SelfAbilityRange : AbilityRange
{
    public override List<Tile> getTilesInRange(Board board)
    {
        List<Tile> retValue = new List<Tile>(1);

        retValue.Add(unit.tile);

        return retValue;
    }

    public override string getAbilityRangeDescription()
    {
        return "Propio";
    }
}