using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor.Tilemaps;
using System;
using Tessera;
using System.Runtime.InteropServices;
using static UnityEditor.Progress;

public class MapGenerator : MonoBehaviour
{
    [Serializable]
    public class TileData {
        public TileBase tile;
        public int value;

        public List<int> adjacencyTiles;
    }

    [Serializable]
    public class TileModule {
        public GameObject module;
        public int value;

        public List<int> adjacencyTiles;
    }

    [Serializable]
    public class TilemapLayer
    {
        public Tilemap tilemap;
        public TesseraGenerator generator;
        public List<TesseraPinned> pinnedTiles;
    }

    struct TileCount {
        public TileBase tile;
        public int count;

        public TileCount(TileBase tile, int count)
        {
            this.tile = tile;
            this.count = count;
        }
    }

    [SerializeField]
    bool useWFC = false;

    [SerializeField]
    List<TilemapLayer> tilemapLayers;

    [SerializeField]
    GameObject tileStorage;
    [SerializeField]
    Transform spawnedPinnedTiles;

    public Vector2Int mapSize = new Vector2Int(25, 25);

    [Header("For Perlin generation")]
    [Range(0f, 1f)]
    public float spawnThreshold = 0.5f;
    public Tilemap ground;

    public List<TileData> tilePalette;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap() {
        CleanPreviousMap();
        tileStorage.SetActive(true);

        if(!useWFC)
            PerlinGeneration();
        else
            WFCGeneration();

        tileStorage.SetActive(false);
    }

    void CleanPreviousMap()
    {
        foreach (var layer in tilemapLayers)
        {
            layer.generator.Clear();
            layer.tilemap.ClearAllTiles();

        }

        for (int i = 0; i < spawnedPinnedTiles.childCount; i++)
        {
            Destroy(spawnedPinnedTiles.GetChild(i).gameObject);
        }
    }

    void PerlinGeneration() {
        // get current grid location
        Vector3Int centerCell = ground.WorldToCell(transform.position);
        Vector3Int currentCell = centerCell;

        Vector3Int topBoundPoint = centerCell + new Vector3Int(mapSize.x / 2, mapSize.y / 2, 0);
        Vector3Int lowerBoundPoint = centerCell - new Vector3Int(mapSize.x / 2, mapSize.y / 2, 0);

        Debug.Log(centerCell);

        int totalSquares = mapSize.x * mapSize.y;
        for (int i = lowerBoundPoint.x; i < topBoundPoint.x; i++)
        {
            for (int j = lowerBoundPoint.y; j < topBoundPoint.y; j++)
            {

                var gridPos = new Vector3Int(i, j, 0);

                int newNoise = UnityEngine.Random.Range(0,10000);
                float xPos = (float) i / (float) mapSize.x;
                float yPos = (float) j / (float) mapSize.y;

                var adjustedPos = new Vector2(xPos, yPos);

                float perlinVal = Mathf.PerlinNoise(xPos + newNoise, yPos+ newNoise);
                Debug.Log("In pos: " + adjustedPos + ", has perlin: " + perlinVal);
                // set the new tile
                if(perlinVal > spawnThreshold) {
                    ground.SetTile(gridPos, GetWeightedRandomTile());
                }

            }
        }

    }

    void WFCGeneration() {
        //Activate all the pinned objects for map computation
        foreach (var layer in tilemapLayers)
        {
            layer.pinnedTiles.ForEach(x => x.gameObject.SetActive(true));
        }

        //Generate map
        foreach (var layer in tilemapLayers)
        {
            GenerateTilemap(layer);
        }

        //Deactivate all the pinned objects so they are invisible
        foreach (var layer in tilemapLayers)
        {
            layer.pinnedTiles.ForEach(x => x.gameObject.SetActive(false));
        }
    }

    void GenerateTilemap(TilemapLayer layer)
    {
        layer.generator.size = new Vector3Int(mapSize.x, 1, mapSize.y);

        var generationResult = layer.generator.Regenerate();
        var tiles = generationResult.tileInstances;

        PlaceTilesInTilemap(layer.generator, layer.tilemap, tiles);


        //TODO: With certain sizes, the pinnedTile may be half-block off
        foreach (var pin in layer.pinnedTiles)
        {
            var pos = pin.transform.position;
            Sylves.Cell gridCell;
            layer.generator.GetGrid().FindCell(pos, out gridCell);
            var adjustedFirstPos = new Vector3Int(gridCell.x, gridCell.z, 0);
            //Debug.Log("Target position in cells: " + adjustedFirstPos);


            var pinCopy = Instantiate(pin.gameObject, spawnedPinnedTiles);
            //Debug.Log("Target position in world: " + layer.tilemap.CellToWorld(adjustedFirstPos));
            pinCopy.transform.position = layer.tilemap.CellToWorld(adjustedFirstPos);
        }

        layer.generator.Clear();
    }


    void PlaceTilesInTilemap(TesseraGenerator gen, Tilemap tileMap, IList<TesseraTileInstance> tiles)
    {
        foreach (var item in tiles)
        {
            ExtendedTesseraTile tileData = (ExtendedTesseraTile)item.Tile;

            if (tileData.setOfTiles != null)
            {
                BoundsInt bounds = tileData.setOfTiles.cellBounds;
                TileBase[] allTiles = tileData.setOfTiles.GetTilesBlock(bounds);

                int counter = 0;
                //var minPos = item.Cell;
                for (int x = 0; x < bounds.size.x; x++)
                {
                    for (int y = 0; y < bounds.size.y; y++)
                    {
                        if (allTiles[x + y * bounds.size.x] != null)
                        {
                            var adjustedFirstPos = new Vector3Int(item.Cell.x, item.Cell.z, 0);
                            var adjustedCurrentPos = new Vector3Int(item.Cells[counter].x, item.Cells[counter].z, 0);

                            var normalizedCellPosition = (adjustedCurrentPos - adjustedFirstPos) - tileData.blockOffset;
                            var tile = tileData.setOfTiles.GetTile(normalizedCellPosition);
                            ground.SetTile(adjustedCurrentPos, tile);
                            counter++;
                        }
                    }
                }
            }
            else
            {
                var adjustedPos = new Vector3Int(item.Cell.x, item.Cell.z, 0);
                tileMap.SetTile(adjustedPos, tileData.tile);
            }
        }
    }

    public TileBase GetWeightedRandomTile()
    {
        int weightSum = 0;
        foreach (var item in tilePalette)
        {
            weightSum += item.value;
        }
        // tilePalette.Values.ToArray();
        int randomWeight = UnityEngine.Random.Range(0, weightSum);
        for (int i = 0;i < tilePalette.Count; i++)
        {
            var tileEntry = tilePalette[i];
            randomWeight -= tileEntry.value;
            if (randomWeight < 0)
            {
                return tileEntry.tile;
            }
        }

        return tilePalette[0].tile;
    }
}
