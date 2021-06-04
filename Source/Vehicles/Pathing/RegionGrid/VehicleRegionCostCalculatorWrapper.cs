﻿using System.Collections.Generic;
using UnityEngine;
using Verse;
using SmashTools;

namespace Vehicles.AI
{
	public class VehicleRegionCostCalculatorWrapper
	{
		private Map map;
		private IntVec3 endCell;
		private HashSet<VehicleRegion> destRegions = new HashSet<VehicleRegion>();

		private int moveTicksCardinal;
		private int moveTicksDiagonal;

		private VehicleRegionCostCalculator vehicleRegionCostCalculator;
		private VehicleRegion cachedRegion;
		private VehicleRegionLink cachedBestLink;
		private VehicleRegionLink cachedSecondBestLink;

		private int cachedBestLinkCost;
		private int cachedSecondBestLinkCost;
		private bool cachedRegionIsDestination;
		private VehicleRegion[] regionGrid;

		public VehicleRegionCostCalculatorWrapper(Map map)
		{
			this.map = map;
			vehicleRegionCostCalculator = new VehicleRegionCostCalculator(map);
		}

		public void Init(CellRect end, TraverseParms traverseParms, int moveTicksCardinal, int moveTicksDiagonal, ByteGrid avoidGrid, Area allowedArea, bool drafted, List<int> disallowedCorners)
		{
			this.moveTicksCardinal = moveTicksCardinal;
			this.moveTicksDiagonal = moveTicksDiagonal;
			endCell = end.CenterCell;
			cachedRegion = null;
			cachedBestLink = null;
			cachedSecondBestLink = null;
			cachedBestLinkCost = 0;
			cachedSecondBestLinkCost = 0;
			cachedRegionIsDestination = false;
			regionGrid = map.GetCachedMapComponent<VehicleMapping>().VehicleRegionGrid.DirectGrid;
			destRegions.Clear();
			if (end.Width == 1 && end.Height == 1)
			{
				VehicleRegion region = VehicleGridsUtility.GetRegion(endCell, map, RegionType.Set_Passable);
				if (region != null)
				{
					destRegions.Add(region);
				}
			}
			else
			{
				foreach (IntVec3 intVec in end)
				{
					if (intVec.InBoundsShip(map) && !disallowedCorners.Contains(map.cellIndices.CellToIndex(intVec)))
					{
						VehicleRegion region2 = VehicleGridsUtility.GetRegion(intVec, map, RegionType.Set_Passable);
						if (region2 != null)
						{
							if (region2.Allows(traverseParms, true))
							{
								destRegions.Add(region2);
							}
						}
					}
				}
			}
			if (destRegions.Count == 0)
			{
				Log.Error("Couldn't find any destination regions. This shouldn't ever happen because we've checked reachability.");
			}
			vehicleRegionCostCalculator.Init(end, destRegions, traverseParms, moveTicksCardinal, moveTicksDiagonal, avoidGrid, allowedArea, drafted);
		}

		public int GetPathCostFromDestToRegion(int cellIndex)
		{
			VehicleRegion region = regionGrid[cellIndex];
			IntVec3 cell = map.cellIndices.IndexToCell(cellIndex);
			if (region != cachedRegion)
			{
				cachedRegionIsDestination = destRegions.Contains(region);
				if (cachedRegionIsDestination)
				{
					return OctileDistanceToEnd(cell);
				}
				cachedBestLinkCost = vehicleRegionCostCalculator.GetRegionBestDistances(region, out cachedBestLink, out cachedSecondBestLink, out cachedSecondBestLinkCost);
				cachedRegion = region;
			}
			else if (cachedRegionIsDestination)
			{
				return OctileDistanceToEnd(cell);
			}
			if (cachedBestLink != null)
			{
				int num = vehicleRegionCostCalculator.RegionLinkDistance(cell, cachedBestLink, 1);
				int num3;
				if (cachedSecondBestLink != null)
				{
					int num2 = vehicleRegionCostCalculator.RegionLinkDistance(cell, cachedSecondBestLink, 1);
					num3 = Mathf.Min(cachedSecondBestLinkCost + num2, cachedBestLinkCost + num);
				}
				else
				{
					num3 = cachedBestLinkCost + num;
				}
				return num3 + OctileDistanceToEndEps(cell);
			}
			return 10000;
		}

		private int OctileDistanceToEnd(IntVec3 cell)
		{
			int dx = Mathf.Abs(cell.x - endCell.x);
			int dz = Mathf.Abs(cell.z - endCell.z);
			return GenMath.OctileDistance(dx, dz, moveTicksCardinal, moveTicksDiagonal);
		}

		private int OctileDistanceToEndEps(IntVec3 cell)
		{
			int dx = Mathf.Abs(cell.x - endCell.x);
			int dz = Mathf.Abs(cell.z - endCell.z);
			return GenMath.OctileDistance(dx, dz, 2, 3);
		}
	}
}