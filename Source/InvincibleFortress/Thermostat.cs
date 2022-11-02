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
    public class CompProperties_Thermostat_TempControl : CompProperties_TempControl
    {
        public CompProperties_Thermostat_TempControl()
        {
            compClass = typeof(Thermostat_CompTempControl);
        }
    }

    public class Thermostat_CompTempControl : CompTempControl
    {
        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("TargetTemperature".Translate() + ": ").Append(targetTemperature.ToStringTemperature("F0"));
            return stringBuilder.ToString();
        }
    }

    public class Building_Thermostat : Building
    {

        public Thermostat_CompTempControl compTempControl;

        public override void ExposeData()
        {
            base.ExposeData();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
        }

        public override void TickRare()
        {
            if (compTempControl == null)
            {
                compTempControl = (Thermostat_CompTempControl)this.AllComps[0];
            }

            this.GetRoom().Temperature = compTempControl.targetTemperature;
        }
    }

    public class PlaceWorker_Thermostat : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            Map currentMap = Find.CurrentMap;
            Room room = center.GetRoom(currentMap);
            if (room != null && !room.UsesOutdoorTemperature)
            {
                GenDraw.DrawFieldEdges(room.Cells.ToList(), Color.magenta);
            }
        }
    }
}
