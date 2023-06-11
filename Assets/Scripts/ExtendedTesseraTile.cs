using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Tessera;

public class ExtendedTesseraTile : TesseraTile
{
    [SerializeField]
    public TileBase tile;

    [SerializeField]
    public Tilemap setOfTiles;


    public Vector3Int blockOffset;

    public List<TileBase> possibleUnderTiles = new List<TileBase>();

    public bool isSingleSprite = false;
    public List<Sprite> possibleSprites = new List<Sprite>();
}
