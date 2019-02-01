using System.Collections.Generic;

public class SingleUnitAbilityArea : AbilityArea
{
    public override List<Tile> getTilesInArea(Board board, Point pos)
    {
        List<Tile> retValue = new List<Tile>();

        Tile tile = board.GetTile(pos);
        if (tile != null)
        {
            retValue.Add(tile);
        }

        return retValue;
    }

    public override string getAbilityAreaDescription()
    {
        return "Único";
    }
}