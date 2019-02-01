using System.Collections.Generic;

using UnityEngine;

public class ExpansiveWaveAbilityArea : AbilityArea
{
    public int horizontal;
    public int vertical;
    Tile tile;

    public override List<Tile> getTilesInArea(Board board, Point pos)
    {
        tile = board.GetTile(pos);
        return board.Search(tile, expandSearch);
    }

    bool expandSearch(Tile from, Tile to)
    {
        return (from.distance + 1) <= horizontal && Mathf.Abs(to.height - tile.height) <= vertical;
    }

    public override string getAbilityAreaDescription()
    {
        return string.Format("Onda ({0}/{1})", horizontal.ToString(), vertical.ToString());
    }
}