// Karel Kroeze
// Job_Extensions.cs
// 2017-05-30

using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WorkTab
{
    [StaticConstructorOnStartup]
    public static class Job_Extensions
    {
        public static Dictionary<JobDef, string> JobIconPaths;
        public static Dictionary<JobDef, Texture2D> JobIcons;
        public const string DefaultJobIconPath = "UI/Icons/Various/unknown";
        public static Texture2D DefaultJobIcon;

        static Job_Extensions()
        {
            LongEventHandler.ExecuteWhenFinished( Initialize );
        }

        private static void Initialize()
        {
            // initialize job icon paths
            JobIconPaths = new Dictionary<JobDef, string>();
            foreach (var job in DefDatabase<JobDef>.AllDefsListForReading)
                JobIconPaths.Add(job, job.DefaultIconPath());

            // initialize job icons dictionary
            JobIcons = new Dictionary<JobDef, Texture2D>();

            // default icon as a fallback
            DefaultJobIcon = ContentFinder<Texture2D>.Get(DefaultJobIconPath);
        }

        public static Texture2D StatusIcon( this JobDef job )
        {
            Texture2D icon;
            if (JobIcons.TryGetValue(job, out icon))
                return icon;

            // TODO: had some odd reports of missing keys - remove the try block once resolved
            try
            {
                Logger.Debug($"Loading icon '{JobIconPaths[job]}' for job '{job.defName}'");
                icon = ContentFinder<Texture2D>.Get(JobIconPaths[job]);
                JobIcons.Add(job, icon);
            }
            catch ( Exception e )
            {
                icon = DefaultJobIcon;
                Logger.Error( $"Error while loading icon '{JobIconPaths[job]}' for job '{job.defName}';\n{e}");
            }

            return icon;
        }

        public static string DefaultIconPath(this JobDef job)
        {
            if (job == null)
                return DefaultJobIconPath;

            // long list of vanilla jobs with presets
            // combat
            if (job == JobDefOf.AttackMelee || 
                job == JobDefOf.AttackStatic || 
                job == JobDefOf.FleeAndCower || 
                job == JobDefOf.ManTurret || 
                job == JobDefOf.Wait_Combat ||
                job == JobDefOf.Flee )
                return "UI/Icons/Various/combat";

            // social fight
            if ( job == JobDefOf.SocialFight )
                return "UI/Icons/Various/fist";

            // warden
            if (job == JobDefOf.Arrest || 
                job == JobDefOf.Capture || 
                job == JobDefOf.EscortPrisonerToBed || 
                job == JobDefOf.PrisonerAttemptRecruit || 
                job == JobDefOf.PrisonerExecution || 
                job == JobDefOf.ReleasePrisoner || 
                job == JobDefOf.TakeWoundedPrisonerToBed ||
                job == JobDefOf.Kidnap ||
                job == JobDefOf.CarryDownedPawnToExit )
                return "UI/Icons/Various/handcuffs";

            // trade 
            if ( job == JobDefOf.PrepareCaravan_GatherPawns ||
                 job == JobDefOf.TradeWithPawn )
                return "UI/Icons/Various/business";

            // marry
            if ( job == JobDefOf.MarryAdjacentPawn )
                return "UI/Icons/Various/rings";

            // lovin
            if ( job == JobDefOf.Lovin )
                return "UI/Icons/Various/hearts";

            // games 
            if ( job == JobDefOf2.Play_Billiards ||
                 job == JobDefOf2.Play_Chess ||
                 job == JobDefOf2.Play_Horseshoes || 
                 job == JobDefOf2.BuildSnowman )
                return "UI/Icons/Various/dice";

            // pray
            if ( job == JobDefOf2.Pray ||
                 job == JobDefOf2.Meditate || 
                 job == JobDefOf2.VisitGrave )
                return "UI/Icons/Various/pray";

            // watch
            if ( job == JobDefOf2.Skygaze ||
                 job == JobDefOf2.WatchTelevision ||
                 job == JobDefOf2.UseTelescope ||
                 job == JobDefOf2.ViewArt )
                return "UI/Icons/Various/eye";

            // doctor
            if (job == JobDefOf.FeedPatient || 
                job == JobDefOf.Rescue || 
                job == JobDefOf.TakeToBedToOperate || 
                job == JobDefOf.TendPatient || 
                job == JobDefOf.VisitSickPawn)
                return "UI/Icons/Various/health";

            // haul
            if (job == JobDefOf.CarryToCryptosleepCasket || 
                job == JobDefOf.HaulCorpseToPublicPlace || 
                job == JobDefOf.HaulToCell || 
                job == JobDefOf.HaulToContainer || 
                job == JobDefOf.Refuel || 
                job == JobDefOf.PrepareCaravan_GatherItems ||
                job == JobDefOf.FillFermentingBarrel ||
                job == JobDefOf.TakeBeerOutOfFermentingBarrel || 
                job == JobDefOf.GiveToPackAnimal ||
                job == JobDefOf.RearmTurret || 
                job == JobDefOf.RearmTurretAtomic )
                return "UI/Icons/Various/haul";

            // clean
            if (job == JobDefOf.Clean || 
                job == JobDefOf.ClearSnow)
                return "UI/Icons/Various/clean";

            // farm
            if (job == JobDefOf.CutPlant || 
                job == JobDefOf.Harvest || 
                job == JobDefOf.Sow)
                return "UI/Icons/Various/farming";

            // animal handling
            if (job == JobDefOf.Milk || 
                job == JobDefOf.Shear || 
                job == JobDefOf.Slaughter || 
                job == JobDefOf.Tame ||
                job == JobDefOf.Train )
                return "UI/Icons/Various/livestock";

            // hunting
            if (job == JobDefOf.Hunt)
                return "UI/Icons/Various/hunt";

            // sleep
            if (job == JobDefOf.LayDown)
                return "UI/Icons/Various/zzz";

            // social
            if (job == JobDefOf.SocialRelax || 
                job == JobDefOf.SpectateCeremony || 
                job == JobDefOf.StandAndBeSociallyActive ||
                job == JobDefOf.Insult ||
                job == JobDefOf.UseCommsConsole )
                return "UI/Icons/Various/social";

            // eat
            if (job == JobDefOf.Ingest)
                return "UI/Icons/Various/eat";

            // fire extinguishing
            if (job == JobDefOf.BeatFire || 
                job == JobDefOf.TriggerFirefoamPopper || 
                job == JobDefOf.ExtinguishSelf)
                return "UI/Icons/Various/extinguish";

            // moving
            if (job == JobDefOf.Goto || 
                job == JobDefOf.GotoSafeTemperature || 
                job == JobDefOf.GotoWander ||
                job == JobDefOf2.GoForWalk )
                return "UI/Icons/Various/move";

            // research
            if (job == JobDefOf.Research)
                return "UI/Icons/Various/research";

            // mining
            if (job == JobDefOf.Mine ||
                job == JobDefOf.OperateDeepDrill )
                return "UI/Icons/Various/mine";

            // waiting
            if ( job == JobDefOf.Wait ||
                 job == JobDefOf.Wait_MaintainPosture ||
                 job == JobDefOf.Wait_SafeTemperature ||
                 job == JobDefOf.Wait_Wander )
                return "UI/Icons/Various/clock";

            // construct
            if (job == JobDefOf.FinishFrame || 
                job == JobDefOf.PlaceNoCostFrame || 
                job == JobDefOf.RemoveFloor || 
                job == JobDefOf.SmoothFloor || 
                job == JobDefOf.SmoothWall ||
                job == JobDefOf.Uninstall || 
                job == JobDefOf.Deconstruct ||
                job == JobDefOf.BuildRoof || 
                job == JobDefOf.RemoveRoof)
                return "UI/Icons/Various/hammer";

            // repair
            if (job == JobDefOf.FixBrokenDownBuilding || 
                job == JobDefOf.Repair ||
                job == JobDefOf.Maintain ||
                job == JobDefOf.RearmTrap )
                return "UI/Icons/Various/wrench";

            // cook
            if ( job == JobDefOf.DeliverFood )
                return "UI/Icons/Various/chef";

            // sick
            if (job == JobDefOf.Wait_Downed ||
                job == JobDefOf.Vomit )
                return "UI/Icons/Various/sick";

            // wear 
            if (job == JobDefOf.Equip ||
                job == JobDefOf.Wear )
                return "UI/Icons/Various/wear";

            // undress
            if (job == JobDefOf.RemoveApparel ||
                job == JobDefOf.Strip ||
                job == JobDefOf.DropEquipment)
                return "UI/Icons/Various/wear";

            // take
            if (job == JobDefOf.TakeInventory)
                return "UI/Icons/Various/take";

            // drop
            if (job == JobDefOf.UnloadInventory ||
                job == JobDefOf.UnloadYourInventory)
                return "UI/Icons/Various/drop";

            // action
            if (job == JobDefOf.Open ||
                job == JobDefOf.EnterCryptosleepCasket ||
                job == JobDefOf.UseNeurotrainer ||
                job == JobDefOf.UseArtifact ||
                job == JobDefOf.Flick ||
                job == JobDefOf.EnterTransporter ||
                job == JobDefOf2.UseItem ||
                job == JobDefOf.Ignite ||
                job == JobDefOf.Steal)
                return "UI/Icons/Various/hand";

            // bills
            // TODO: Try to split bills to type
            if (job == JobDefOf.DoBill)
                return "UI/Icons/Various/star";

            // known unknown
            if (job == JobDefOf.UseVerbOnThing)
                return DefaultJobIconPath;

            Logger.Debug( $"No icon set for JobDef {job.defName}." );
            return DefaultJobIconPath;
        }
    }
}