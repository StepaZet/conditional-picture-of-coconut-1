using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

namespace Resources.Rooms
{
	public class RoomGenerator : MonoBehaviour
	{
		public List<Transform> openings;
		[SerializeField] private MapGenerator mapGenerator;
		[SerializeField] private Collider2D collider2D;
		private GridObj grid;
		[SerializeField] private Transform leftDownPoint;
		[SerializeField] private Transform rightUpPoint;

		public void Start()
		{
			grid = GameObject.Find("GridActualUnity").GetComponent<GridObj>();
			mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();
			//GenerateAdjacentRooms();
		}

		public void GenerateAdjacentCorridors()
		{
			GenerateAdjacentRooms(mapGenerator.corridorPrefabs);
		}

		public void GenerateAdjacentRooms()
		{
			GenerateAdjacentRooms(mapGenerator.roomsPrefabs);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			//GenerateAdjacentRooms();
			//if(isCorridor)
			//	GenerateAdjacentRooms();
			//else
			//	GenerateAdjacentCorridors();
		}

		public void GenerateAdjacentRooms(List<GameObject> prefabs)
		{
			foreach (var opening in openings)
			{
				prefabs.Shuffle();
				var isRoomGenerated = false;
				foreach (var roomPrefab in prefabs)
				{
					var generatedRoom = GenerateRoom(opening, roomPrefab);
					if (generatedRoom == null)
						continue;
					mapGenerator.generatedRooms.Add(generatedRoom);
					isRoomGenerated = true;
					break;
				}

				if (!isRoomGenerated)
				{
					var deadEnd = Instantiate(mapGenerator.deadEnd);
					deadEnd.transform.position = opening.position;
				}
			}
		}

		public RoomGenerator GenerateRoom(Transform opening, GameObject roomPrefabObj)
		{
			var roomObj = Instantiate(roomPrefabObj, grid.transform);
			roomObj.transform.position = new Vector3(roomObj.transform.position.x, roomObj.transform.position.y, 0);
			var room = roomObj.GetComponent<RoomGenerator>();
			var canFit = false;
			foreach (var newOpening in room.openings)
			{
				room.transform.position += opening.position - newOpening.position;
				var hasCollidedWithOther = mapGenerator.generatedRooms
					.Except(new[] {room})
					.Any(r => IsColliding(room, r));
				if (hasCollidedWithOther)
					continue;
				canFit = true;
				break;
			}

			if (canFit)
				return room;
			Destroy(roomObj);
			return null;
		}

		private static bool IsColliding(RoomGenerator room1, RoomGenerator room2)
		{
			return IsBetween(room1.leftDownPoint.position, room2.leftDownPoint.position, room1.rightUpPoint.position)
			       || IsBetween(room2.leftDownPoint.position, room1.leftDownPoint.position, room2.rightUpPoint.position)
			       || IsBetween(room1.leftDownPoint.position, room2.rightUpPoint.position, room1.rightUpPoint.position)
			       || IsBetween(room2.leftDownPoint.position, room1.rightUpPoint.position, room2.rightUpPoint.position);
		}

		private static bool IsBetween(Vector3 left, Vector3 center, Vector3 right)
		{
			return left.x < center.x && center.x < right.x
				&& left.y < center.y && center.y < right.y;
		}
	}
}
