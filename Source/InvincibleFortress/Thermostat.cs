using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                compTempControl = this.TryGetComp<Thermostat_CompTempControl>();
            }

            float ambientTemperature = base.AmbientTemperature;

            float num = ((Mathf.Abs(ambientTemperature - compTempControl.targetTemperature) < 1f) ? 0f : ((ambientTemperature < compTempControl.targetTemperature - 1f) ? ((ambientTemperature < 20f) ? 1f : ((!(ambientTemperature > 200f)) ? Mathf.InverseLerp(200f, 20f, ambientTemperature) : 0f)) : ((!(ambientTemperature > compTempControl.targetTemperature + 1f)) ? 0f : ((!(ambientTemperature < -50f)) ? (-1f) : (0f - Mathf.InverseLerp(-273f, -50f, ambientTemperature))))));

            float energyLimit = 120 * num * 4.1666665f;

            float num2 = GenTemperature.ControlTemperatureTempChange(base.Position, base.Map, energyLimit, compTempControl.targetTemperature);

            if (!Mathf.Approximately(num2, 0f))
            {
                this.GetRoom().Temperature += num2;
            }
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
