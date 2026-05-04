using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using Pointo.Unit;
using System.Linq;

namespace Assets.Scripts.AI
{
	public class AiStrategies : MonoBehaviour
	{
		public static AiStrategies Instance;

		public enum Tactics
		{
			None,
			Column
		}

		public void Awake()
		{
			Instance = this;
		}
		
		public void Column(List<GameObject> wave)
		{
			List<GameObject> tilesInRange = CheckIfColumnTilesAreInRange(wave);
			if (tilesInRange.Count >= wave.Count)
			{
                GameObject startTile = new();
				while (wave[0].transform.position.x < startTile.transform.position.x) startTile = tilesInRange[Random.Range(0, tilesInRange.Count)];
				for (int i = 0; i < wave.Count; i++)
				{
					//Made with claude help
					wave[i].GetComponent<Unit>().MoveToTile(GridManager.instance.tilesList.First(tile => tile.transform.position.x == startTile.transform.position.x && tile.transform.position.y == tile.transform.position.y + i).GetComponent<Tile>());
				}
			}
		}

		private List<GameObject> CheckIfColumnTilesAreInRange(List<GameObject> wave)
		{
			//Made with claude help
			List<List<GameObject>> tilesInRangeAllUnits = new();
			foreach(GameObject waveItem in wave)
			{
				tilesInRangeAllUnits.Add(waveItem.GetComponent<Unit>().GetTilesInRange());
			}
			List<GameObject> combinedList = new();

			tilesInRangeAllUnits.Distinct();
			combinedList = tilesInRangeAllUnits.SelectMany(list => list)
				.GroupBy(tile => tile)
				.Where(group => group.Count() >= wave.Count())
				.Select(group => group.Key)
				.ToList();

			combinedList = combinedList.SelectMany(tileA => combinedList, (tileA, tileB) => new { tileA, tileB })
				.Where(pair => pair.tileA.transform.position.x != pair.tileB.transform.position.x
				&& Mathf.Abs(pair.tileA.transform.position.y - pair.tileB.transform.position.y) == 1
				&& pair.tileA.transform.position.y < pair.tileB.transform.position.y)
				.Select(pair => pair.tileB)
				.ToList();

			combinedList = combinedList.Where(tile => combinedList
			.Count(other => other.transform.position.x == tile.transform.position.x
			&& other.transform.position.y >= tile.transform.position.y
            && other.transform.position.y < tile.transform.position.y + wave.Count()) == wave.Count())
			.ToList();

			return combinedList;
		}
	}
}