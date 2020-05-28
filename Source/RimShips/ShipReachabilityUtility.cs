﻿using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Vehicles.AI
{
    public static class ShipReachabilityUtility
    {
        public static bool CanReachShip(this Pawn pawn, LocalTargetInfo dest, PathEndMode peMode, Danger maxDanger, bool canBash = false, TraverseMode mode = TraverseMode.ByPawn)
        {
            return pawn.Spawned && MapExtensionUtility.GetExtensionToMap(pawn.Map).getShipReachability.CanReachShip(pawn.Position, dest, peMode, TraverseParms.For(pawn, maxDanger, mode, canBash));
        }

        public static bool CanReachShipNonLocal(this Pawn pawn, TargetInfo dest, PathEndMode peMode, Danger maxDanger, bool canBash = false, TraverseMode mode = TraverseMode.ByPawn)
        {
            return pawn.Spawned && MapExtensionUtility.GetExtensionToMap(pawn.Map).getShipReachability.CanReachShipNonLocal(pawn.Position, dest, peMode, TraverseParms.For(pawn, maxDanger, mode, canBash));
        }

        public static bool CanReachShipMapEdge(this Pawn p)
        {
            return p.Spawned && MapExtensionUtility.GetExtensionToMap(p.Map).getShipReachability.CanReachMapEdge(p.Position, TraverseParms.For(p, Danger.Deadly, TraverseMode.ByPawn, false));
        }

        public static void ClearCache()
        {
            List<Map> maps = Find.Maps;
            for (int i = 0; i < maps.Count; i++)
            {
                maps[i].reachability.ClearCache();
                MapExtensionUtility.GetExtensionToMap(maps[i]).getShipReachability.ClearCache();
            }
        }

        public static void ClearCacheFor(Pawn p)
        {
            List<Map> maps = Find.Maps;
            for (int i = 0; i < maps.Count; i++)
            {
                maps[i].reachability.ClearCacheFor(p);
                MapExtensionUtility.GetExtensionToMap(maps[i]).getShipReachability.ClearCacheFor(p);
            }
        }
    }
}
