﻿using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;
using RimWorld.Planet;
using SmashTools;

namespace Vehicles
{
	/// <summary>
	/// Unit Testing
	/// </summary>
	public static class StartupTesting
	{
		/// <summary>
		/// Load up game, get first settlement, find available vehicle, initiate strafing run
		/// </summary>
		[UnitTest(Category = "Vehicle Framework", Name = "Strafe Run")]
		private static void UnitTestStrafing()
		{
			Map sourceMap = null;
			Map targetMap = null;
			VehiclePawn vehicle = null;
			foreach (Settlement settlement in Find.WorldObjects.Settlements.Where(s => s.Faction == Faction.OfPlayer))
			{
				Map map = GetOrGenerateMapUtility.GetOrGenerateMap(settlement.Tile, null);
				if (map.spawnedThings.FirstOrDefault(t => t is VehiclePawn vehicleCheck && vehicleCheck.CompVehicleLauncher != null) is VehiclePawn vehicleTarget)
				{
					sourceMap = map;
					vehicle = vehicleTarget;
				}
				else
				{
					targetMap = map;
				}
			}
			if (sourceMap is null || targetMap is null)
			{
				SmashLog.Error($"Unable to execute unit test <method>UnitTestStrafing</method> post load.");
				return;
			}
			Current.Game.CurrentMap = targetMap;
			LaunchTargeter.Instance.RegisterActionOnTile(targetMap.Tile, new AerialVehicleArrivalAction_StrafeMap(vehicle, targetMap.Parent));
		}

		/// <summary>
		/// Load up game, find available vehicle with upgrade tree, focus camera on vehicle
		/// </summary>
		[UnitTest(Category = "Vehicle Framework", Name = "Upgrade Tree")]
		private static void UnitTestUpgradeMenu()
		{
			Map map = Find.CurrentMap ?? Find.Maps.FirstOrDefault();
			VehiclePawn vehicle = (VehiclePawn)map.mapPawns.AllPawns.FirstOrDefault(p => p is VehiclePawn vehicle && vehicle.CompUpgradeTree != null);
			if (map is null || vehicle is null)
			{
				SmashLog.Error($"Unable to execute unit test <method>UnitTestUpgradeMenu</method> post load.");
				return;
			}
			CameraJumper.TryJump(vehicle);
			Find.Selector.Select(vehicle);
		}

		[UnitTest(Category = "Vehicle Framework", Name = "Color Dialog", GameState = GameState.Playing)]
		private static void UnitTestColorDialog()
		{
			Map map = Find.CurrentMap ?? Find.Maps.FirstOrDefault();
			VehiclePawn vehicle = (VehiclePawn)map.mapPawns.AllPawns.FirstOrDefault(p => p is VehiclePawn vehicle && vehicle.VehicleGraphic.Shader.SupportsRGBMaskTex());
			if (map is null || vehicle is null)
			{
				SmashLog.Error($"Unable to execute unit test <method>UnitTestColorDialog</method> post load.");
				return;
			}
			CameraJumper.TryJump(vehicle);
			Find.Selector.Select(vehicle);
		}

		/// <summary>
		/// Load up game, open update menu for all previous versions
		/// </summary>
		[UnitTest(Category = "Vehicle Framework", Name = "Previous Versions Menu", GameState = GameState.OnStartup)]
		private static void UnitTestShowUpdates()
		{
			VehicleMod.settings.debug.ShowAllUpdates();
		}

		/// <summary>
		/// Load up game, open Mod Settings
		/// </summary>
		[UnitTest(Category = "Vehicle Framework", Name = "Mod Settings", GameState = GameState.OnStartup)]
		private static void UnitTestModSettings()
		{
			Dialog_ModSettings settings = new Dialog_ModSettings(VehicleMod.mod);
			Find.WindowStack.Add(settings);
		}

		/// <summary>
		/// Load up game, open route planner
		/// </summary>
		[UnitTest(Category = "Vehicle Framework", Name = "World Route Planner", GameState = GameState.Playing)]
		private static void UnitTestRoutePlanner()
		{
			Prefs.DevMode = true;
			CameraJumper.TryShowWorld();
			VehicleRoutePlanner.Instance.Start();
		}

		[UnitTest(Category = "Vehicle Framework", Name = "Regions", GameState = GameState.Playing)]
		private static void UnitTestRegionsOn()
		{
			Prefs.DevMode = true;
			CameraJumper.TryHideWorld();
			VehicleMod.settings.debug.RegionDebugMenu();
		}

		[UnitTest(Category = "Vehicle Framework", Name = "Vehicle Area Manager", GameState = GameState.Playing)]
		private static void UnitTestVehicleAreaManager()
		{
			Prefs.DevMode = true;
			CameraJumper.TryHideWorld();
			if (Find.CurrentMap is Map map)
			{
				Find.WindowStack.Add(new Dialog_ManageAreas(map));
			}
			else
			{
				SmashLog.Error($"Tried to unit test <type>VehicleAreaManager</type> with null current map.");
			}
		}
	}
}
