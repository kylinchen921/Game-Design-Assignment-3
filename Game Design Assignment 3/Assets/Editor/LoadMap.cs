using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class LoadMap
{
    private static Grid grid;


    // Add a new menu item under an existing menu


    [MenuItem("Tools/MapLoad/Load")]
    private static void Load()
    {
        EditorApplication.playmodeStateChanged = HandleOnPlayModeChanged;

        var go = GameObject.FindGameObjectWithTag("Grid");
        grid = go.GetComponent<Grid>();
        grid.ReadLevel("Level1");

        int wi = grid.cells.GetLength(0);
        int hi = grid.cells.GetLength(1);
        grid.I = new Serialiseable2DArray(wi,hi);
        var c = 0;
        for (int x = 0; x < hi; x++)
        {
            for (int y = 0; y < wi; y++)
            {
                grid.I[y, x] = grid.cells[y, x].GameObject;
            }
        }
    }


    [MenuItem("Tools/MapLoad/Save")]
    private static void Save()
    {

    }

    static void HandleOnPlayModeChanged()
    {
        // This method is run whenever the playmode state is changed.
        Debug.Log("on Play");
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.Log("on Play play");

            //Object.DestroyImmediate(grid.CellParent);
        }
    }
}