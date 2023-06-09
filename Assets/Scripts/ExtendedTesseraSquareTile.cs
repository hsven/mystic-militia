using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Tessera;

public class ExtendedTesseraSquareTile : TesseraSquareTile
{
    [SerializeField]
    public TileBase tile;

    [SerializeField]
    public Tilemap setOfTiles;

    public Vector3Int blockOffset;

    public List<TileBase> possibleUnderTiles = new List<TileBase>();
}
