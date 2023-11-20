using Godot;
using System;
using System.Collections.Generic;

public partial class WorldMap : Node2D
{
    public Godot.Collections.Dictionary<int, MapPoint> mapPoints;

    public override void _Ready()
    {
        mapPoints = new Godot.Collections.Dictionary<int, MapPoint> ();
        //toggle accessibility on worlds
        foreach (Node node in this.GetChildren())
        {
            if (node is not MapPoint) continue;
            MapPoint point = node as MapPoint;
            GD.Print("Checking world node " + point.worldNumber);
            try
            {
                point.Accessible = GameManager.SaveFile.GetWorldUnlocked(point.worldNumber);
            }
            catch (Exception e)
            {
                point.Accessible = false;
            }
            mapPoints.Add(point.worldNumber, point);
        }
        PositionCursor();
    }

    public void PositionCursor()
    {
        //place cursor at the right place
        if (GameManager.SaveFile.currentWorld > 0)
        {
            GetNode<WorldMapCursor>("%Cursor").SwapPoint(mapPoints[GameManager.SaveFile.currentWorld]);
        }
        else
        {
            GetNode<WorldMapCursor>("%Cursor").Initialize();
        }
    }
}
