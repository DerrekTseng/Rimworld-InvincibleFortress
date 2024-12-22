using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace InvincibleFortress
{

    public class PlaceWorker_ItemRepairer_OneInMap : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {

            if (map.listerThings.AllThings.Any((Thing x) => x.def == checkingDef || x.def == checkingDef.blueprintDef || x.def == checkingDef.frameDef || x.def == checkingDef.installBlueprintDef))
            {
                return new AcceptanceReport("FortressItemRepairerPlaceOnlyOneInMap".Translate());
            }
            return true;
        }
    }

    public class GameCondition_ItemRepairer : GameCondition
    {
        public override void DoCellSteadyEffects(IntVec3 c, Map map)
        {
            base.DoCellSteadyEffects(c, map);
            foreach (Thing thing in c.GetThingList(map))
            {
                if (thing is Pawn pawn)
                {
                    TryRepairPawnHolder(pawn);
                }
                else if (!TryRepairThing(thing) && !TryRefreshThing(thing) && TryCureBlighted(thing))
                {
                    break;
                }
            }
        }

        private bool TryRepairPawnHolder(Pawn pawn)
        {
            bool result = false;
            List<Apparel> list = pawn.apparel?.WornApparel;
            if (!list.NullOrEmpty())
            {
                foreach (Apparel item in list)
                {
                    if (TryRepairThing(item))
                    {
                        result = true;
                    }
                }
            }
            List<ThingWithComps> list2 = pawn.equipment?.AllEquipmentListForReading;
            if (!list2.NullOrEmpty())
            {
                foreach (ThingWithComps item2 in list2)
                {
                    if (TryRepairThing(item2))
                    {
                        result = true;
                    }
                }
            }
            ThingOwner<Thing> thingOwner = pawn.inventory?.innerContainer;
            if (!thingOwner.NullOrEmpty())
            {
                foreach (Thing item3 in thingOwner)
                {
                    if (TryRepairThing(item3))
                    {
                        result = true;
                    }
                }
                return result;
            }
            return result;
        }

        private bool TryRepairThing(Thing thing)
        {
            bool result = false;
            if (thing.def.useHitPoints)
            {
                if (thing.HitPoints < thing.MaxHitPoints)
                {
                    thing.HitPoints += Mathf.CeilToInt((float)(thing.MaxHitPoints - thing.HitPoints) * 0.01f);
                    result = true;
                }
                else if (thing.HitPoints == thing.MaxHitPoints && thing is Apparel apparel && apparel.WornByCorpse)
                {
                    Pawn originalPawn = apparel.Wearer;
                    if (originalPawn != null)
                    {
                        apparel.Notify_PawnResurrected(originalPawn);
                        result = true;
                    }
                }
            }
            if (thing is Corpse corpse && corpse.InnerPawn != null && TryRepairPawnHolder(corpse.InnerPawn))
            {
                result = true;
            }
            return result;
        }

        private bool TryRefreshThing(Thing thing)
        {
            bool result = false;
            CompRottable compRottable = thing.TryGetComp<CompRottable>();
            if (compRottable != null)
            {
                compRottable.RotProgress -= 2000f;
                if (compRottable.RotProgress < 0f)
                {
                    compRottable.RotProgress = 0f;
                }
                result = true;
            }
            return result;
        }

        private bool TryCureBlighted(Thing thing)
        {
            if (thing is Plant plant && plant.Blighted)
            {
                plant.Blight.Destroy();
                return true;
            }
            return false;
        }
    }
}
