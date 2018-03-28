using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Hexagon.Services.Map
{
    public class HexGrid : MonoBehaviour
    {
        public int cellCountX = 80, cellCountZ = 60;
        public Text cellLabelPrefab;

        [SerializeField] private HexMapGenerator mapGenerator;

        public HexCell cellPrefab;
        private HexCell[] cells;
        private HexCellShaderData cellShaderData;
        private int chunkCountX, chunkCountZ;
        public HexGridChunk chunkPrefab;

        private HexGridChunk[] chunks;
        private HexCell currentPathFrom, currentPathTo;

        public Texture2D noiseSource;
        private HexCellPriorityQueue searchFrontier;
        private int searchFrontierPhase;

        public int seed;
        public HexUnit unitPrefab;
        private readonly List<HexUnit> units = new List<HexUnit>();

        public bool HasPath { get; private set; }

        /*private void Awake()
        {
            HexMetrics.noiseSource = noiseSource;
            HexMetrics.InitializeHashGrid(seed);
            HexUnit.unitPrefab = unitPrefab;
            cellShaderData = gameObject.AddComponent<HexCellShaderData>();
            cellShaderData.Grid = this;
            //CreateMap(cellCountX, cellCountZ);
            mapGenerator.GenerateMap(cellCountX, cellCountZ);
        } */

        public void Prepare()
        {
            HexMetrics.noiseSource = noiseSource;
            HexMetrics.InitializeHashGrid(seed);
            HexUnit.unitPrefab = unitPrefab;
            cellShaderData = gameObject.AddComponent<HexCellShaderData>();
            cellShaderData.Grid = this;
            //CreateMap(cellCountX, cellCountZ);
            mapGenerator.GenerateMap(20, 15);
        }

        public void AddUnit(HexUnit unit, HexCell location, float orientation)
        {
            units.Add(unit);
            unit.Grid = this;
            unit.transform.SetParent(transform, false);
            unit.Location = location;
            unit.Orientation = orientation;
        }

        public void RemoveUnit(HexUnit unit)
        {
            units.Remove(unit);
            unit.Die();
        }

        public bool CreateMap(int x, int z)
        {
            if (
                x <= 0 || x % HexMetrics.chunkSizeX != 0 ||
                z <= 0 || z % HexMetrics.chunkSizeZ != 0
            )
            {
                Debug.LogError("Unsupported map size.");
                return false;
            }

            ClearPath();
            ClearUnits();
            if (chunks != null)
                for (var i = 0; i < chunks.Length; i++)
                    Destroy(chunks[i].gameObject);

            cellCountX = x;
            cellCountZ = z;
            chunkCountX = cellCountX / HexMetrics.chunkSizeX;
            chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;
            cellShaderData.Initialize(cellCountX, cellCountZ);
            CreateChunks();
            CreateCells();
            return true;
        }

        private void CreateChunks()
        {
            chunks = new HexGridChunk[chunkCountX * chunkCountZ];

            for (int z = 0, i = 0; z < chunkCountZ; z++)
            for (var x = 0; x < chunkCountX; x++)
            {
                var chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }

        private void CreateCells()
        {
            cells = new HexCell[cellCountZ * cellCountX];

            for (int z = 0, i = 0; z < cellCountZ; z++)
            for (var x = 0; x < cellCountX; x++)
                CreateCell(x, z, i++);
        }

        private void ClearUnits()
        {
            for (var i = 0; i < units.Count; i++) units[i].Die();
            units.Clear();
        }

       /* private void OnEnable()
        {
            if (!HexMetrics.noiseSource)
            {
                HexMetrics.noiseSource = noiseSource;
                HexMetrics.InitializeHashGrid(seed);
                HexUnit.unitPrefab = unitPrefab;
                ResetVisibility();
            }
        } */

        public HexCell GetCell(Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) return GetCell(hit.point);
            return null;
        }

        public HexCell GetCell(Vector3 position)
        {
            position = transform.InverseTransformPoint(position);
            var coordinates = HexCoordinates.FromPosition(position);
            var index =
                coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
            return cells[index];
        }

        public HexCell GetCell(HexCoordinates coordinates)
        {
            var z = coordinates.Z;
            if (z < 0 || z >= cellCountZ) return null;
            var x = coordinates.X + z / 2;
            if (x < 0 || x >= cellCountX) return null;
            return cells[x + z * cellCountX];
        }

        public HexCell GetCell(int xOffset, int zOffset)
        {
            return cells[xOffset + zOffset * cellCountX];
        }

        public HexCell GetCell(int cellIndex)
        {
            return cells[cellIndex];
        }

        public List<HexCell> GetCells()
        {
            return cells.ToList();
        }

        public void ShowUI(bool visible)
        {
            for (var i = 0; i < chunks.Length; i++) chunks[i].ShowUI(visible);
        }

        private void CreateCell(int x, int z, int i)
        {
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f);

            var cell = cells[i] = Instantiate(cellPrefab);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.Index = i;
            cell.ShaderData = cellShaderData;

            cell.Explorable =
                x > 0 && z > 0 && x < cellCountX - 1 && z < cellCountZ - 1;

            if (x > 0) cell.SetNeighbor(HexDirection.W, cells[i - 1]);
            if (z > 0)
                if ((z & 1) == 0)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                    if (x > 0) cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
                else
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                    if (x < cellCountX - 1) cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }

            var label = Instantiate(cellLabelPrefab);
            label.rectTransform.anchoredPosition =
                new Vector2(position.x, position.z);
            cell.uiRect = label.rectTransform;

            cell.Elevation = 0;

            AddCellToChunk(x, z, cell);
        }

        private void AddCellToChunk(int x, int z, HexCell cell)
        {
            var chunkX = x / HexMetrics.chunkSizeX;
            var chunkZ = z / HexMetrics.chunkSizeZ;
            var chunk = chunks[chunkX + chunkZ * chunkCountX];

            var localX = x - chunkX * HexMetrics.chunkSizeX;
            var localZ = z - chunkZ * HexMetrics.chunkSizeZ;
            chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(cellCountX);
            writer.Write(cellCountZ);

            for (var i = 0; i < cells.Length; i++) cells[i].Save(writer);

            writer.Write(units.Count);
            for (var i = 0; i < units.Count; i++) units[i].Save(writer);
        }

        public void Load(BinaryReader reader, int header)
        {
            ClearPath();
            ClearUnits();
            int x = 20, z = 15;
            if (header >= 1)
            {
                x = reader.ReadInt32();
                z = reader.ReadInt32();
            }

            if (x != cellCountX || z != cellCountZ)
                if (!CreateMap(x, z))
                    return;

            var originalImmediateMode = cellShaderData.ImmediateMode;
            cellShaderData.ImmediateMode = true;

            for (var i = 0; i < cells.Length; i++) cells[i].Load(reader, header);
            for (var i = 0; i < chunks.Length; i++) chunks[i].Refresh();

            if (header >= 2)
            {
                var unitCount = reader.ReadInt32();
                for (var i = 0; i < unitCount; i++) HexUnit.Load(reader, this);
            }

            cellShaderData.ImmediateMode = originalImmediateMode;
        }

        public List<HexCell> GetPath()
        {
            if (!HasPath) return null;
            var path = ListPool<HexCell>.Get();
            for (var c = currentPathTo; c != currentPathFrom; c = c.PathFrom) path.Add(c);
            path.Add(currentPathFrom);
            path.Reverse();
            return path;
        }

        public void ClearPath()
        {
            if (HasPath)
            {
                var current = currentPathTo;
                while (current != currentPathFrom)
                {
                    current.SetLabel(null);
                    current.DisableHighlight();
                    current = current.PathFrom;
                }

                current.DisableHighlight();
                HasPath = false;
            }
            else if (currentPathFrom)
            {
                currentPathFrom.DisableHighlight();
                currentPathTo.DisableHighlight();
            }

            currentPathFrom = currentPathTo = null;
        }

        private void ShowPath(int speed)
        {
            if (HasPath)
            {
                var current = currentPathTo;
                while (current != currentPathFrom)
                {
                    var turn = (current.Distance - 1) / speed;
                    current.SetLabel(turn.ToString());
                    current.EnableHighlight(Color.white);
                    current = current.PathFrom;
                }
            }

            currentPathFrom.EnableHighlight(Color.blue);
            currentPathTo.EnableHighlight(Color.red);
        }

        private void ShowPath() {
            if (HasPath) {
                var current = currentPathTo;
                while (current != currentPathFrom) {
                    current.EnableHighlight(Color.white);
                    current = current.PathFrom;
                }
            }

            currentPathFrom.EnableHighlight(Color.blue);
            currentPathTo.EnableHighlight(Color.red);
        }

        public void HighlightCells(List<HexCell> cells, Color color)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                cells[i].EnableHighlight(color);
            }
        }

        public void UnHighlightAllCells()
        {
            foreach (var hexCell in cells)
            {
                hexCell.DisableHighlight();
            }
        }

        /*public void FindPath(HexCell fromCell, HexCell toCell, HexUnit unit)
        {
            ClearPath();
            currentPathFrom = fromCell;
            currentPathTo = toCell;
            HasPath = Search(fromCell, toCell, unit);
            ShowPath(unit.Speed);
        } */

        public List<HexCell> FindPath(HexCell fromCell, HexCell toCell) {
            ClearPath();
            currentPathFrom = fromCell;
            currentPathTo = toCell;
            HasPath = Search(fromCell, toCell);
            //ShowPath();
            return GetPath();
        }

        public bool FindPath(HexCell fromCell, HexCell toCell, int range, out List<HexCell> path) {
            ClearPath();
            currentPathFrom = fromCell;
            currentPathTo = toCell;
            HasPath = Search(fromCell, toCell, range);
            if (HasPath)
            {
                path = GetPath();
                return true;
            }
            path = null;
            return false;
        }

        private bool Search(HexCell fromCell, HexCell toCell) {
            searchFrontierPhase += 2;
            if (searchFrontier == null)
                searchFrontier = new HexCellPriorityQueue();
            else
                searchFrontier.Clear();

            fromCell.SearchPhase = searchFrontierPhase;
            fromCell.Distance = 0;
            searchFrontier.Enqueue(fromCell);
            while (searchFrontier.Count > 0) {
                var current = searchFrontier.Dequeue();
                current.SearchPhase += 1;

                if (current == toCell) return true;

                for (var d = HexDirection.NE; d <= HexDirection.NW; d++) {
                    var neighbor = current.GetNeighbor(d);
                    if (
                        neighbor == null ||
                        neighbor.SearchPhase > searchFrontierPhase
                    )
                        continue;

                    var distance = current.Distance + 1;

                    if (neighbor.SearchPhase < searchFrontierPhase) {
                        neighbor.SearchPhase = searchFrontierPhase;
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        neighbor.SearchHeuristic =
                            neighbor.coordinates.DistanceTo(toCell.coordinates);
                        searchFrontier.Enqueue(neighbor);
                    }
                    else if (distance < neighbor.Distance) {
                        var oldPriority = neighbor.SearchPriority;
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        searchFrontier.Change(neighbor, oldPriority);
                    }
                }
            }

            return false;
        }

        private bool Search(HexCell fromCell, HexCell toCell, int range) {
            searchFrontierPhase += 2;
            if (searchFrontier == null)
                searchFrontier = new HexCellPriorityQueue();
            else
                searchFrontier.Clear();

            fromCell.SearchPhase = searchFrontierPhase;
            fromCell.Distance = 0;
            searchFrontier.Enqueue(fromCell);
            while (searchFrontier.Count > 0) {
                var current = searchFrontier.Dequeue();
                current.SearchPhase += 1;

                if (current == toCell) return true;

                for (var d = HexDirection.NE; d <= HexDirection.NW; d++) {
                    var neighbor = current.GetNeighbor(d);
                    if (
                        neighbor == null ||
                        neighbor.SearchPhase > searchFrontierPhase
                    )
                        continue;

                    var distance = current.Distance + 1;
                    if (distance > range)
                    {
                        continue;
                    }

                    if (neighbor.SearchPhase < searchFrontierPhase) {
                        neighbor.SearchPhase = searchFrontierPhase;
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        neighbor.SearchHeuristic =
                            neighbor.coordinates.DistanceTo(toCell.coordinates);
                        searchFrontier.Enqueue(neighbor);
                    }
                    else if (distance < neighbor.Distance) {
                        var oldPriority = neighbor.SearchPriority;
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        searchFrontier.Change(neighbor, oldPriority);
                    }
                }
            }

            return false;
        }

        public List<HexCell> SearchInRange(HexCell fromCell, int range, bool includeStart) {
            searchFrontierPhase += 2;

            List<HexCell> retCells = new List<HexCell>();

            if (searchFrontier == null)
                searchFrontier = new HexCellPriorityQueue();
            else
                searchFrontier.Clear();

            fromCell.SearchPhase = searchFrontierPhase;
            fromCell.Distance = 0;
            searchFrontier.Enqueue(fromCell);

            while (searchFrontier.Count > 0) {
                var current = searchFrontier.Dequeue();
                current.SearchPhase += 1;

                retCells.Add(current);

                for (var d = HexDirection.NE; d <= HexDirection.NW; d++) {
                    var neighbor = current.GetNeighbor(d);
                    if (
                        neighbor == null ||
                        neighbor.SearchPhase > searchFrontierPhase
                    )
                        continue;

                    var distance = current.Distance + 1;
                    if (distance > range) {
                        continue;
                    }

                    if (neighbor.SearchPhase < searchFrontierPhase) {
                        neighbor.SearchPhase = searchFrontierPhase;
                        neighbor.Distance = distance;
                        neighbor.SearchHeuristic = 0;                           
                        searchFrontier.Enqueue(neighbor);
                    }
                    else if (distance < neighbor.Distance) {
                        var oldPriority = neighbor.SearchPriority;
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        searchFrontier.Change(neighbor, oldPriority);
                    }
                }
            }

            if (!includeStart)
                retCells.Remove(fromCell);
            return retCells;
        }

        private bool Search(HexCell fromCell, HexCell toCell, HexUnit unit)
        {
            var speed = unit.Speed;
            searchFrontierPhase += 2;
            if (searchFrontier == null)
                searchFrontier = new HexCellPriorityQueue();
            else
                searchFrontier.Clear();

            fromCell.SearchPhase = searchFrontierPhase;
            fromCell.Distance = 0;
            searchFrontier.Enqueue(fromCell);
            while (searchFrontier.Count > 0)
            {
                var current = searchFrontier.Dequeue();
                current.SearchPhase += 1;

                if (current == toCell) return true;

                var currentTurn = (current.Distance - 1) / speed;

                for (var d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    var neighbor = current.GetNeighbor(d);
                    if (
                        neighbor == null ||
                        neighbor.SearchPhase > searchFrontierPhase
                    )
                        continue;
                    if (!unit.IsValidDestination(neighbor)) continue;
                    var moveCost = unit.GetMoveCost(current, neighbor, d);
                    if (moveCost < 0) continue;

                    var distance = current.Distance + moveCost;
                    var turn = (distance - 1) / speed;
                    if (turn > currentTurn) distance = turn * speed + moveCost;

                    if (neighbor.SearchPhase < searchFrontierPhase)
                    {
                        neighbor.SearchPhase = searchFrontierPhase;
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        neighbor.SearchHeuristic =
                            neighbor.coordinates.DistanceTo(toCell.coordinates);
                        searchFrontier.Enqueue(neighbor);
                    }
                    else if (distance < neighbor.Distance)
                    {
                        var oldPriority = neighbor.SearchPriority;
                        neighbor.Distance = distance;
                        neighbor.PathFrom = current;
                        searchFrontier.Change(neighbor, oldPriority);
                    }
                }
            }

            return false;
        }

        public void IncreaseVisibility(HexCell fromCell, int range)
        {
            var cells = GetVisibleCells(fromCell, range);
            for (var i = 0; i < cells.Count; i++) cells[i].IncreaseVisibility();
            ListPool<HexCell>.Add(cells);
        }

        public void DecreaseVisibility(HexCell fromCell, int range)
        {
            var cells = GetVisibleCells(fromCell, range);
            for (var i = 0; i < cells.Count; i++) cells[i].DecreaseVisibility();
            ListPool<HexCell>.Add(cells);
        }

        public void ResetVisibility()
        {
            for (var i = 0; i < cells.Length; i++) cells[i].ResetVisibility();
            for (var i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                IncreaseVisibility(unit.Location, unit.VisionRange);
            }
        }

        private List<HexCell> GetVisibleCells(HexCell fromCell, int range)
        {
            var visibleCells = ListPool<HexCell>.Get();

            searchFrontierPhase += 2;
            if (searchFrontier == null)
                searchFrontier = new HexCellPriorityQueue();
            else
                searchFrontier.Clear();

            range += fromCell.ViewElevation;
            fromCell.SearchPhase = searchFrontierPhase;
            fromCell.Distance = 0;
            searchFrontier.Enqueue(fromCell);
            var fromCoordinates = fromCell.coordinates;
            while (searchFrontier.Count > 0)
            {
                var current = searchFrontier.Dequeue();
                current.SearchPhase += 1;
                visibleCells.Add(current);

                for (var d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    var neighbor = current.GetNeighbor(d);
                    if (
                        neighbor == null ||
                        neighbor.SearchPhase > searchFrontierPhase ||
                        !neighbor.Explorable
                    )
                        continue;

                    var distance = current.Distance + 1;
                    if (distance + neighbor.ViewElevation > range ||
                        distance > fromCoordinates.DistanceTo(neighbor.coordinates)
                    )
                        continue;

                    if (neighbor.SearchPhase < searchFrontierPhase)
                    {
                        neighbor.SearchPhase = searchFrontierPhase;
                        neighbor.Distance = distance;
                        neighbor.SearchHeuristic = 0;
                        searchFrontier.Enqueue(neighbor);
                    }
                    else if (distance < neighbor.Distance)
                    {
                        var oldPriority = neighbor.SearchPriority;
                        neighbor.Distance = distance;
                        searchFrontier.Change(neighbor, oldPriority);
                    }
                }
            }

            return visibleCells;
        }
    }
}