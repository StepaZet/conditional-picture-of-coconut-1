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
		public bool isCorridor;

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

		private void OnTriggerStay2D(Collider2D other)
		{
			if (!other.GetComponent<Game.ObjectManager>())
				return;
			GenerateAdjacentRooms();
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

		private RoomGenerator GenerateRoom(Transform opening, GameObject roomPrefabObj)
		{
			Instantiate(roomPrefabObj, grid.transform);
			var room = roomPrefabObj.GetComponent<RoomGenerator>();
			var canFit = false;
			foreach (var newOpening in room.openings)
			{
				room.transform.position += opening.position - newOpening.position;
				var hasCollidedWithOther = mapGenerator.generatedRooms
					.Except(new[] {room})
					.Any(roomObject => room.collider2D.IsTouching(roomObject.collider2D));
				if (hasCollidedWithOther)
					continue;
				canFit = true;
				break;
			}

			if (canFit)
				return room;
			Destroy(roomPrefabObj);
			return null;
		}
	}
}
