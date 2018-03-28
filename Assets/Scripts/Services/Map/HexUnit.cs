using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hexagon.Services.Map
{
    public class HexUnit : MonoBehaviour
    {
        private const float rotationSpeed = 180f;
        private const float travelSpeed = 4f;

        public static HexUnit unitPrefab;

        private HexCell location, currentTravelLocation;

        private float orientation;
        private List<HexCell> pathToTravel;

        public HexGrid Grid { get; set; }

        public HexCell Location
        {
            get { return location; }
            set
            {
                if (location)
                {
                    Grid.DecreaseVisibility(location, VisionRange);
                    location.Unit = null;
                }

                location = value;
                value.Unit = this;
                Grid.IncreaseVisibility(value, VisionRange);
                transform.localPosition = value.Position;
            }
        }

        public float Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                transform.localRotation = Quaternion.Euler(0f, value, 0f);
            }
        }

        public int Speed
        {
            get { return 24; }
        }

        public int VisionRange
        {
            get { return 3; }
        }

        public void ValidateLocation()
        {
            transform.localPosition = location.Position;
        }

        public bool IsValidDestination(HexCell cell)
        {
            return cell.IsExplored && !cell.IsUnderwater && !cell.Unit;
        }

        public void Travel(List<HexCell> path)
        {
            location.Unit = null;
            location = path[path.Count - 1];
            location.Unit = this;
            pathToTravel = path;
            StopAllCoroutines();
            StartCoroutine(TravelPath());
        }

        private IEnumerator TravelPath()
        {
            Vector3 a, b, c = pathToTravel[0].Position;
            yield return LookAt(pathToTravel[1].Position);
            Grid.DecreaseVisibility(
                currentTravelLocation ? currentTravelLocation : pathToTravel[0],
                VisionRange
            );

            var t = Time.deltaTime * travelSpeed;
            for (var i = 1; i < pathToTravel.Count; i++)
            {
                currentTravelLocation = pathToTravel[i];
                a = c;
                b = pathToTravel[i - 1].Position;
                c = (b + currentTravelLocation.Position) * 0.5f;
                Grid.IncreaseVisibility(pathToTravel[i], VisionRange);
                for (; t < 1f; t += Time.deltaTime * travelSpeed)
                {
                    transform.localPosition = Bezier.GetPoint(a, b, c, t);
                    var d = Bezier.GetDerivative(a, b, c, t);
                    d.y = 0f;
                    transform.localRotation = Quaternion.LookRotation(d);
                    yield return null;
                }

                Grid.DecreaseVisibility(pathToTravel[i], VisionRange);
                t -= 1f;
            }

            currentTravelLocation = null;

            a = c;
            b = location.Position;
            c = b;
            Grid.IncreaseVisibility(location, VisionRange);
            for (; t < 1f; t += Time.deltaTime * travelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                var d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }

            transform.localPosition = location.Position;
            orientation = transform.localRotation.eulerAngles.y;
            ListPool<HexCell>.Add(pathToTravel);
            pathToTravel = null;
        }

        private IEnumerator LookAt(Vector3 point)
        {
            point.y = transform.localPosition.y;
            var fromRotation = transform.localRotation;
            var toRotation =
                Quaternion.LookRotation(point - transform.localPosition);
            var angle = Quaternion.Angle(fromRotation, toRotation);

            if (angle > 0f)
            {
                var speed = rotationSpeed / angle;
                for (
                    var t = Time.deltaTime * speed;
                    t < 1f;
                    t += Time.deltaTime * speed
                )
                {
                    transform.localRotation =
                        Quaternion.Slerp(fromRotation, toRotation, t);
                    yield return null;
                }
            }

            transform.LookAt(point);
            orientation = transform.localRotation.eulerAngles.y;
        }

        public int GetMoveCost(
            HexCell fromCell, HexCell toCell, HexDirection direction)
        {
            if (!IsValidDestination(toCell)) return -1;
            var edgeType = fromCell.GetEdgeType(toCell);
            if (edgeType == HexEdgeType.Cliff) return -1;
            int moveCost;
            if (fromCell.HasRoadThroughEdge(direction))
            {
                moveCost = 1;
            }
            else if (fromCell.Walled != toCell.Walled)
            {
                return -1;
            }
            else
            {
                moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
                moveCost +=
                    toCell.UrbanLevel + toCell.FarmLevel + toCell.PlantLevel;
            }

            return moveCost;
        }

        public void Die()
        {
            if (location) Grid.DecreaseVisibility(location, VisionRange);
            location.Unit = null;
            Destroy(gameObject);
        }

        public void Save(BinaryWriter writer)
        {
            location.coordinates.Save(writer);
            writer.Write(orientation);
        }

        public static void Load(BinaryReader reader, HexGrid grid)
        {
            var coordinates = HexCoordinates.Load(reader);
            var orientation = reader.ReadSingle();
            grid.AddUnit(
                Instantiate(unitPrefab), grid.GetCell(coordinates), orientation
            );
        }

        private void OnEnable()
        {
            if (location)
            {
                transform.localPosition = location.Position;
                if (currentTravelLocation)
                {
                    Grid.IncreaseVisibility(location, VisionRange);
                    Grid.DecreaseVisibility(currentTravelLocation, VisionRange);
                    currentTravelLocation = null;
                }
            }
        }

//	void OnDrawGizmos () {
//		if (pathToTravel == null || pathToTravel.Count == 0) {
//			return;
//		}
//
//		Vector3 a, b, c = pathToTravel[0].Position;
//
//		for (int i = 1; i < pathToTravel.Count; i++) {
//			a = c;
//			b = pathToTravel[i - 1].Position;
//			c = (b + pathToTravel[i].Position) * 0.5f;
//			for (float t = 0f; t < 1f; t += 0.1f) {
//				Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
//			}
//		}
//
//		a = c;
//		b = pathToTravel[pathToTravel.Count - 1].Position;
//		c = b;
//		for (float t = 0f; t < 1f; t += 0.1f) {
//			Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
//		}
//	}
    }
}