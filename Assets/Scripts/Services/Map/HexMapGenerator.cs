using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Hexagon.Services.Map
{
    public class HexMapGenerator : MonoBehaviour
    {
        public enum HemisphereMode
        {
            Both,
            North,
            South
        }

        private static readonly float[] temperatureBands = {0.1f, 0.3f, 0.6f};
        private static readonly float[] moistureBands = {0.12f, 0.28f, 0.85f};

        private static readonly Biome[] biomes =
        {
            new Biome(0, 0), new Biome(4, 0), new Biome(4, 0), new Biome(4, 0),
            new Biome(0, 0), new Biome(2, 0), new Biome(2, 1), new Biome(2, 2),
            new Biome(0, 0), new Biome(1, 0), new Biome(1, 1), new Biome(1, 2),
            new Biome(0, 0), new Biome(1, 1), new Biome(1, 2), new Biome(1, 3)
        };

        private int cellCount, landCells;

        [Range(20, 200)] public int chunkSizeMax = 100;

        [Range(20, 200)] public int chunkSizeMin = 30;

        private List<ClimateData> climate = new List<ClimateData>();

        [Range(6, 10)] public int elevationMaximum = 8;

        [Range(-4, 0)] public int elevationMinimum = -2;

        [Range(0, 100)] public int erosionPercentage = 50;

        [Range(0f, 1f)] public float evaporationFactor = 0.5f;

        [Range(0f, 1f)] public float extaLakeProbability = 0.25f;

        private readonly List<HexDirection> flowDirections = new List<HexDirection>();

        public HexGrid grid;

        public HemisphereMode hemisphere;

        [Range(0f, 1f)] public float highRiseProbability = 0.25f;

        [Range(0f, 1f)] public float highTemperature = 1f;

        [Range(0f, 0.5f)] public float jitterProbability = 0.25f;

        [Range(5, 95)] public int landPercentage = 50;

        [Range(0f, 1f)] public float lowTemperature = 0f;

        [Range(0, 10)] public int mapBorderX = 5;

        [Range(0, 10)] public int mapBorderZ = 5;

        private List<ClimateData> nextClimate = new List<ClimateData>();

        [Range(0f, 1f)] public float precipitationFactor = 0.25f;

        [Range(0, 10)] public int regionBorder = 5;

        [Range(1, 4)] public int regionCount = 1;

        private List<MapRegion> regions;

        [Range(0, 20)] public int riverPercentage = 10;

        [Range(0f, 1f)] public float runoffFactor = 0.25f;

        private HexCellPriorityQueue searchFrontier;
        private int searchFrontierPhase;

        public int seed;

        [Range(0f, 1f)] public float seepageFactor = 0.125f;

        [Range(0f, 0.4f)] public float sinkProbability = 0.2f;

        [Range(0f, 1f)] public float startingMoisture = 0.1f;

        [Range(0f, 1f)] public float temperatureJitter = 0.1f;

        private int temperatureJitterChannel;

        public bool useFixedSeed;

        [Range(1, 5)] public int waterLevel = 3;

        public HexDirection windDirection = HexDirection.NW;

        [Range(1f, 10f)] public float windStrength = 4f;

        public void GenerateMap(int x, int z)
        {
            var originalRandomState = Random.state;
            if (!useFixedSeed)
            {
                seed = Random.Range(0, int.MaxValue);
                seed ^= (int) DateTime.Now.Ticks;
                seed ^= (int) Time.unscaledTime;
                seed &= int.MaxValue;
            }

            Random.InitState(seed);

            cellCount = x * z;
            grid.CreateMap(x, z);
            if (searchFrontier == null) searchFrontier = new HexCellPriorityQueue();
            for (var i = 0; i < cellCount; i++) grid.GetCell(i).WaterLevel = waterLevel;
            CreateRegions();
            CreateLand();
            ErodeLand();
            CreateClimate();
            CreateRivers();
            SetTerrainType();
            for (var i = 0; i < cellCount; i++) grid.GetCell(i).SearchPhase = 0;

            Random.state = originalRandomState;
        }

        private void CreateRegions()
        {
            if (regions == null)
                regions = new List<MapRegion>();
            else
                regions.Clear();

            MapRegion region;
            switch (regionCount)
            {
                default:
                    region.xMin = mapBorderX;
                    region.xMax = grid.cellCountX - mapBorderX;
                    region.zMin = mapBorderZ;
                    region.zMax = grid.cellCountZ - mapBorderZ;
                    regions.Add(region);
                    break;
                case 2:
                    if (Random.value < 0.5f)
                    {
                        region.xMin = mapBorderX;
                        region.xMax = grid.cellCountX / 2 - regionBorder;
                        region.zMin = mapBorderZ;
                        region.zMax = grid.cellCountZ - mapBorderZ;
                        regions.Add(region);
                        region.xMin = grid.cellCountX / 2 + regionBorder;
                        region.xMax = grid.cellCountX - mapBorderX;
                        regions.Add(region);
                    }
                    else
                    {
                        region.xMin = mapBorderX;
                        region.xMax = grid.cellCountX - mapBorderX;
                        region.zMin = mapBorderZ;
                        region.zMax = grid.cellCountZ / 2 - regionBorder;
                        regions.Add(region);
                        region.zMin = grid.cellCountZ / 2 + regionBorder;
                        region.zMax = grid.cellCountZ - mapBorderZ;
                        regions.Add(region);
                    }

                    break;
                case 3:
                    region.xMin = mapBorderX;
                    region.xMax = grid.cellCountX / 3 - regionBorder;
                    region.zMin = mapBorderZ;
                    region.zMax = grid.cellCountZ - mapBorderZ;
                    regions.Add(region);
                    region.xMin = grid.cellCountX / 3 + regionBorder;
                    region.xMax = grid.cellCountX * 2 / 3 - regionBorder;
                    regions.Add(region);
                    region.xMin = grid.cellCountX * 2 / 3 + regionBorder;
                    region.xMax = grid.cellCountX - mapBorderX;
                    regions.Add(region);
                    break;
                case 4:
                    region.xMin = mapBorderX;
                    region.xMax = grid.cellCountX / 2 - regionBorder;
                    region.zMin = mapBorderZ;
                    region.zMax = grid.cellCountZ / 2 - regionBorder;
                    regions.Add(region);
                    region.xMin = grid.cellCountX / 2 + regionBorder;
                    region.xMax = grid.cellCountX - mapBorderX;
                    regions.Add(region);
                    region.zMin = grid.cellCountZ / 2 + regionBorder;
                    region.zMax = grid.cellCountZ - mapBorderZ;
                    regions.Add(region);
                    region.xMin = mapBorderX;
                    region.xMax = grid.cellCountX / 2 - regionBorder;
                    regions.Add(region);
                    break;
            }
        }

        private void CreateLand()
        {
            var landBudget = Mathf.RoundToInt(cellCount * landPercentage * 0.01f);
            landCells = landBudget;
            for (var guard = 0; guard < 10000; guard++)
            {
                var sink = Random.value < sinkProbability;
                for (var i = 0; i < regions.Count; i++)
                {
                    var region = regions[i];
                    var chunkSize = Random.Range(chunkSizeMin, chunkSizeMax - 1);
                    if (sink)
                    {
                        landBudget = SinkTerrain(chunkSize, landBudget, region);
                    }
                    else
                    {
                        landBudget = RaiseTerrain(chunkSize, landBudget, region);
                        if (landBudget == 0) return;
                    }
                }
            }

            if (landBudget > 0)
            {
                Debug.LogWarning("Failed to use up " + landBudget + " land budget.");
                landCells -= landBudget;
            }
        }

        private int RaiseTerrain(int chunkSize, int budget, MapRegion region)
        {
            searchFrontierPhase += 1;
            var firstCell = GetRandomCell(region);
            firstCell.SearchPhase = searchFrontierPhase;
            firstCell.Distance = 0;
            firstCell.SearchHeuristic = 0;
            searchFrontier.Enqueue(firstCell);
            var center = firstCell.coordinates;

            var rise = Random.value < highRiseProbability ? 2 : 1;
            var size = 0;
            while (size < chunkSize && searchFrontier.Count > 0)
            {
                var current = searchFrontier.Dequeue();
                var originalElevation = current.Elevation;
                var newElevation = originalElevation + rise;
                if (newElevation > elevationMaximum) continue;
                current.Elevation = newElevation;
                if (
                    originalElevation < waterLevel &&
                    newElevation >= waterLevel && --budget == 0
                )
                    break;
                size += 1;

                for (var d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    var neighbor = current.GetNeighbor(d);
                    if (neighbor && neighbor.SearchPhase < searchFrontierPhase)
                    {
                        neighbor.SearchPhase = searchFrontierPhase;
                        neighbor.Distance = neighbor.coordinates.DistanceTo(center);
                        neighbor.SearchHeuristic =
                            Random.value < jitterProbability ? 1 : 0;
                        searchFrontier.Enqueue(neighbor);
                    }
                }
            }

            searchFrontier.Clear();
            return budget;
        }

        private int SinkTerrain(int chunkSize, int budget, MapRegion region)
        {
            searchFrontierPhase += 1;
            var firstCell = GetRandomCell(region);
            firstCell.SearchPhase = searchFrontierPhase;
            firstCell.Distance = 0;
            firstCell.SearchHeuristic = 0;
            searchFrontier.Enqueue(firstCell);
            var center = firstCell.coordinates;

            var sink = Random.value < highRiseProbability ? 2 : 1;
            var size = 0;
            while (size < chunkSize && searchFrontier.Count > 0)
            {
                var current = searchFrontier.Dequeue();
                var originalElevation = current.Elevation;
                var newElevation = current.Elevation - sink;
                if (newElevation < elevationMinimum) continue;
                current.Elevation = newElevation;
                if (
                    originalElevation >= waterLevel &&
                    newElevation < waterLevel
                )
                    budget += 1;
                size += 1;

                for (var d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    var neighbor = current.GetNeighbor(d);
                    if (neighbor && neighbor.SearchPhase < searchFrontierPhase)
                    {
                        neighbor.SearchPhase = searchFrontierPhase;
                        neighbor.Distance = neighbor.coordinates.DistanceTo(center);
                        neighbor.SearchHeuristic =
                            Random.value < jitterProbability ? 1 : 0;
                        searchFrontier.Enqueue(neighbor);
                    }
                }
            }

            searchFrontier.Clear();
            return budget;
        }

        private void ErodeLand()
        {
            var erodibleCells = ListPool<HexCell>.Get();
            for (var i = 0; i < cellCount; i++)
            {
                var cell = grid.GetCell(i);
                if (IsErodible(cell)) erodibleCells.Add(cell);
            }

            var targetErodibleCount =
                (int) (erodibleCells.Count * (100 - erosionPercentage) * 0.01f);

            while (erodibleCells.Count > targetErodibleCount)
            {
                var index = Random.Range(0, erodibleCells.Count);
                var cell = erodibleCells[index];
                var targetCell = GetErosionTarget(cell);

                cell.Elevation -= 1;
                targetCell.Elevation += 1;

                if (!IsErodible(cell))
                {
                    erodibleCells[index] = erodibleCells[erodibleCells.Count - 1];
                    erodibleCells.RemoveAt(erodibleCells.Count - 1);
                }

                for (var d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    var neighbor = cell.GetNeighbor(d);
                    if (
                        neighbor && neighbor.Elevation == cell.Elevation + 2 &&
                        !erodibleCells.Contains(neighbor)
                    )
                        erodibleCells.Add(neighbor);
                }

                if (IsErodible(targetCell) && !erodibleCells.Contains(targetCell)) erodibleCells.Add(targetCell);

                for (var d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    var neighbor = targetCell.GetNeighbor(d);
                    if (
                        neighbor && neighbor != cell &&
                        neighbor.Elevation == targetCell.Elevation + 1 &&
                        !IsErodible(neighbor)
                    )
                        erodibleCells.Remove(neighbor);
                }
            }

            ListPool<HexCell>.Add(erodibleCells);
        }

        private bool IsErodible(HexCell cell)
        {
            var erodibleElevation = cell.Elevation - 2;
            for (var d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                var neighbor = cell.GetNeighbor(d);
                if (neighbor && neighbor.Elevation <= erodibleElevation) return true;
            }

            return false;
        }

        private HexCell GetErosionTarget(HexCell cell)
        {
            var candidates = ListPool<HexCell>.Get();
            var erodibleElevation = cell.Elevation - 2;
            for (var d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                var neighbor = cell.GetNeighbor(d);
                if (neighbor && neighbor.Elevation <= erodibleElevation) candidates.Add(neighbor);
            }

            var target = candidates[Random.Range(0, candidates.Count)];
            ListPool<HexCell>.Add(candidates);
            return target;
        }

        private void CreateClimate()
        {
            climate.Clear();
            nextClimate.Clear();
            var initialData = new ClimateData();
            initialData.moisture = startingMoisture;
            var clearData = new ClimateData();
            for (var i = 0; i < cellCount; i++)
            {
                climate.Add(initialData);
                nextClimate.Add(clearData);
            }

            for (var cycle = 0; cycle < 40; cycle++)
            {
                for (var i = 0; i < cellCount; i++) EvolveClimate(i);
                var swap = climate;
                climate = nextClimate;
                nextClimate = swap;
            }
        }

        private void EvolveClimate(int cellIndex)
        {
            var cell = grid.GetCell(cellIndex);
            var cellClimate = climate[cellIndex];

            if (cell.IsUnderwater)
            {
                cellClimate.moisture = 1f;
                cellClimate.clouds += evaporationFactor;
            }
            else
            {
                var evaporation = cellClimate.moisture * evaporationFactor;
                cellClimate.moisture -= evaporation;
                cellClimate.clouds += evaporation;
            }

            var precipitation = cellClimate.clouds * precipitationFactor;
            cellClimate.clouds -= precipitation;
            cellClimate.moisture += precipitation;

            var cloudMaximum = 1f - cell.ViewElevation / (elevationMaximum + 1f);
            if (cellClimate.clouds > cloudMaximum)
            {
                cellClimate.moisture += cellClimate.clouds - cloudMaximum;
                cellClimate.clouds = cloudMaximum;
            }

            var mainDispersalDirection = windDirection.Opposite();
            var cloudDispersal = cellClimate.clouds * (1f / (5f + windStrength));
            var runoff = cellClimate.moisture * runoffFactor * (1f / 6f);
            var seepage = cellClimate.moisture * seepageFactor * (1f / 6f);
            for (var d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                var neighbor = cell.GetNeighbor(d);
                if (!neighbor) continue;
                var neighborClimate = nextClimate[neighbor.Index];
                if (d == mainDispersalDirection)
                    neighborClimate.clouds += cloudDispersal * windStrength;
                else
                    neighborClimate.clouds += cloudDispersal;

                var elevationDelta = neighbor.ViewElevation - cell.ViewElevation;
                if (elevationDelta < 0)
                {
                    cellClimate.moisture -= runoff;
                    neighborClimate.moisture += runoff;
                }
                else if (elevationDelta == 0)
                {
                    cellClimate.moisture -= seepage;
                    neighborClimate.moisture += seepage;
                }

                nextClimate[neighbor.Index] = neighborClimate;
            }

            var nextCellClimate = nextClimate[cellIndex];
            nextCellClimate.moisture += cellClimate.moisture;
            if (nextCellClimate.moisture > 1f) nextCellClimate.moisture = 1f;
            nextClimate[cellIndex] = nextCellClimate;
            climate[cellIndex] = new ClimateData();
        }

        private void CreateRivers()
        {
            var riverOrigins = ListPool<HexCell>.Get();
            for (var i = 0; i < cellCount; i++)
            {
                var cell = grid.GetCell(i);
                if (cell.IsUnderwater) continue;
                var data = climate[i];
                var weight =
                    data.moisture * (cell.Elevation - waterLevel) /
                    (elevationMaximum - waterLevel);
                if (weight > 0.75f)
                {
                    riverOrigins.Add(cell);
                    riverOrigins.Add(cell);
                }

                if (weight > 0.5f) riverOrigins.Add(cell);
                if (weight > 0.25f) riverOrigins.Add(cell);
            }

            var riverBudget = Mathf.RoundToInt(landCells * riverPercentage * 0.01f);
            while (riverBudget > 0 && riverOrigins.Count > 0)
            {
                var index = Random.Range(0, riverOrigins.Count);
                var lastIndex = riverOrigins.Count - 1;
                var origin = riverOrigins[index];
                riverOrigins[index] = riverOrigins[lastIndex];
                riverOrigins.RemoveAt(lastIndex);

                if (!origin.HasRiver)
                {
                    var isValidOrigin = true;
                    for (var d = HexDirection.NE; d <= HexDirection.NW; d++)
                    {
                        var neighbor = origin.GetNeighbor(d);
                        if (neighbor && (neighbor.HasRiver || neighbor.IsUnderwater))
                        {
                            isValidOrigin = false;
                            break;
                        }
                    }

                    if (isValidOrigin) riverBudget -= CreateRiver(origin);
                }
            }

            if (riverBudget > 0) Debug.LogWarning("Failed to use up river budget.");

            ListPool<HexCell>.Add(riverOrigins);
        }

        private int CreateRiver(HexCell origin)
        {
            var length = 1;
            var cell = origin;
            var direction = HexDirection.NE;
            while (!cell.IsUnderwater)
            {
                var minNeighborElevation = int.MaxValue;
                flowDirections.Clear();
                for (var d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    var neighbor = cell.GetNeighbor(d);
                    if (!neighbor) continue;

                    if (neighbor.Elevation < minNeighborElevation) minNeighborElevation = neighbor.Elevation;

                    if (neighbor == origin || neighbor.HasIncomingRiver) continue;

                    var delta = neighbor.Elevation - cell.Elevation;
                    if (delta > 0) continue;

                    if (neighbor.HasOutgoingRiver)
                    {
                        cell.SetOutgoingRiver(d);
                        return length;
                    }

                    if (delta < 0)
                    {
                        flowDirections.Add(d);
                        flowDirections.Add(d);
                        flowDirections.Add(d);
                    }

                    if (
                        length == 1 ||
                        d != direction.Next2() && d != direction.Previous2()
                    )
                        flowDirections.Add(d);
                    flowDirections.Add(d);
                }

                if (flowDirections.Count == 0)
                {
                    if (length == 1) return 0;

                    if (minNeighborElevation >= cell.Elevation)
                    {
                        cell.WaterLevel = minNeighborElevation;
                        if (minNeighborElevation == cell.Elevation) cell.Elevation = minNeighborElevation - 1;
                    }

                    break;
                }

                direction = flowDirections[Random.Range(0, flowDirections.Count)];
                cell.SetOutgoingRiver(direction);
                length += 1;

                if (
                    minNeighborElevation >= cell.Elevation &&
                    Random.value < extaLakeProbability
                )
                {
                    cell.WaterLevel = cell.Elevation;
                    cell.Elevation -= 1;
                }

                cell = cell.GetNeighbor(direction);
            }

            return length;
        }

        private void SetTerrainType()
        {
            temperatureJitterChannel = Random.Range(0, 4);
            var rockDesertElevation =
                elevationMaximum - (elevationMaximum - waterLevel) / 2;

            for (var i = 0; i < cellCount; i++)
            {
                var cell = grid.GetCell(i);
                var temperature = DetermineTemperature(cell);
                var moisture = climate[i].moisture;
                if (!cell.IsUnderwater)
                {
                    var t = 0;
                    for (; t < temperatureBands.Length; t++)
                        if (temperature < temperatureBands[t])
                            break;
                    var m = 0;
                    for (; m < moistureBands.Length; m++)
                        if (moisture < moistureBands[m])
                            break;
                    var cellBiome = biomes[t * 4 + m];

                    if (cellBiome.terrain == 0)
                    {
                        if (cell.Elevation >= rockDesertElevation) cellBiome.terrain = 3;
                    }
                    else if (cell.Elevation == elevationMaximum)
                    {
                        cellBiome.terrain = 4;
                    }

                    if (cellBiome.terrain == 4)
                        cellBiome.plant = 0;
                    else if (cellBiome.plant < 3 && cell.HasRiver) cellBiome.plant += 1;

                    cell.TerrainTypeIndex = cellBiome.terrain;
                    cell.PlantLevel = cellBiome.plant;
                }
                else
                {
                    int terrain;
                    if (cell.Elevation == waterLevel - 1)
                    {
                        int cliffs = 0, slopes = 0;
                        for (
                            var d = HexDirection.NE;
                            d <= HexDirection.NW;
                            d++
                        )
                        {
                            var neighbor = cell.GetNeighbor(d);
                            if (!neighbor) continue;
                            var delta = neighbor.Elevation - cell.WaterLevel;
                            if (delta == 0)
                                slopes += 1;
                            else if (delta > 0) cliffs += 1;
                        }

                        if (cliffs + slopes > 3)
                            terrain = 1;
                        else if (cliffs > 0)
                            terrain = 3;
                        else if (slopes > 0)
                            terrain = 0;
                        else
                            terrain = 1;
                    }
                    else if (cell.Elevation >= waterLevel)
                    {
                        terrain = 1;
                    }
                    else if (cell.Elevation < 0)
                    {
                        terrain = 3;
                    }
                    else
                    {
                        terrain = 2;
                    }

                    if (terrain == 1 && temperature < temperatureBands[0]) terrain = 2;
                    cell.TerrainTypeIndex = terrain;
                }
            }
        }

        private float DetermineTemperature(HexCell cell)
        {
            var latitude = (float) cell.coordinates.Z / grid.cellCountZ;
            if (hemisphere == HemisphereMode.Both)
            {
                latitude *= 2f;
                if (latitude > 1f) latitude = 2f - latitude;
            }
            else if (hemisphere == HemisphereMode.North)
            {
                latitude = 1f - latitude;
            }

            var temperature =
                Mathf.LerpUnclamped(lowTemperature, highTemperature, latitude);

            temperature *= 1f - (cell.ViewElevation - waterLevel) /
                           (elevationMaximum - waterLevel + 1f);

            var jitter =
                HexMetrics.SampleNoise(cell.Position * 0.1f)[temperatureJitterChannel];

            temperature += (jitter * 2f - 1f) * temperatureJitter;

            return temperature;
        }

        private HexCell GetRandomCell(MapRegion region)
        {
            return grid.GetCell(
                Random.Range(region.xMin, region.xMax),
                Random.Range(region.zMin, region.zMax)
            );
        }

        private struct MapRegion
        {
            public int xMin, xMax, zMin, zMax;
        }

        private struct ClimateData
        {
            public float clouds, moisture;
        }

        private struct Biome
        {
            public int terrain, plant;

            public Biome(int terrain, int plant)
            {
                this.terrain = terrain;
                this.plant = plant;
            }
        }
    }
}