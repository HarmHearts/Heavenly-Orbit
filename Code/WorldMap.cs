using Godot;
using System;
using System.Collections.Generic;

public partial class WorldMap : Node2D
{
    [Export]
    public int worldToUnlock;

    private PackedScene explosion = GD.Load<PackedScene>("res://Scenes/Constructs/planet_unlock.tscn");

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
                point.Accessible = GameManager.GameSave.IsWorldUnlocked(point.worldNumber);
            }
            catch (Exception e)
            {
                point.Accessible = false;
            }
            mapPoints.Add(point.worldNumber, point);
        }
        PositionCursor();
        if(worldToUnlock > 0 && worldToUnlock < mapPoints.Count)
        {
            GetNode<WorldMapCursor>("%Cursor").canMove = false;
            mapPoints[worldToUnlock].Accessible = false;
            GetNode<Timer>("%UnlockTimer").Timeout += UnlockPlanet;
            GetNode<Timer>("%UnlockTimer").Start();
        }
    }

    public void PositionCursor()
    {
        //place cursor at the right place
        if (GameManager.GameSave.CurrentWorld > 0)
        {
            GetNode<WorldMapCursor>("%Cursor").SwapPoint(mapPoints[GameManager.GameSave.CurrentWorld]);
        }
        else
        {
            GetNode<WorldMapCursor>("%Cursor").Initialize();
        }
    }

    public void UnlockPlanet()
    {
        mapPoints[worldToUnlock].AddChild(explosion.Instantiate());
        mapPoints[worldToUnlock].Accessible = true;
        GetNode<WorldMapCursor>("%Cursor").canMove = true;
        GetNode<Timer>("%UnlockTimer").Timeout -= UnlockPlanet;
        AudioSystem.PlaySFX("Explode");
    }
}
