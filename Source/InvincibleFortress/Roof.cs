using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace InvincibleFortress
{
    [DefOf]
    public static class RoofDefOf
    {
        public static RoofDef InvincibleFortressRoof;
    }

    public class CompFortressRoof : ThingComp
    {

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad && !DebugSettings.godMode)
            {
                parent.Map.roofGrid.SetRoof(parent.Position, RoofDefOf.InvincibleFortressRoof);
                MoteMaker.PlaceTempRoof(parent.Position, parent.Map);
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (!parent.Destroyed)
            {
                parent.Destroy();
            }
        }
    }

    public class PlaceWorker_FortressRoof : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {            
            RoofDef roofDef = loc.GetRoof(map);
            if (roofDef == null || !roofDef.isThickRoof)
            {
                return AcceptanceReport.WasAccepted;
            } else
            {
                return AcceptanceReport.WasRejected;
            }
        }

        public override void PostPlace(Map map, BuildableDef def, IntVec3 loc, Rot4 rot)
        {
            map.areaManager.BuildRoof[loc] = false;
            map.areaManager.NoRoof[loc] = false;
        }
    }

    public class Designator_RemoveFortressRoof : Designator_AreaNoRoof
    {
        public override bool Visible
        {
            get
            {
                return true;
            }
        }
        public Designator_RemoveFortressRoof()
        {
            defaultLabel = "DesignatorRemoveFortressRoof".Translate();
            defaultDesc = "DesignatorRemoveFortressRooffDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/Designators/NoRoofArea");
            soundDragSustain = SoundDefOf.Designate_DragAreaAdd;
            soundDragChanged = null;
            soundSucceeded = SoundDefOf.Designate_ZoneAdd;
            useMouseIcon = true;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!c.InBounds(base.Map))
            {
                return false;
            }
            if (c.Fogged(base.Map))
            {
                return false;
            }
            return !base.Map.areaManager.NoRoof[c] && base.Map.roofGrid.RoofAt(c) == RoofDefOf.InvincibleFortressRoof;
        }

    }

  



}
