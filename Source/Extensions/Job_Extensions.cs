// Karel Kroeze
// Job_Extensions.cs
// 2017-05-30

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

        static Job_Extensions()
        {
            // initialize job icon paths
            JobIconPaths = new Dictionary<JobDef, string>();
            foreach ( var job in DefDatabase<JobDef>.AllDefsListForReading )
                JobIconPaths.Add( job, job.DefaultIconPath() );

            // initialize job icons dictionary
            JobIcons = new Dictionary<JobDef, Texture2D>();
        }

        public static Texture2D StatusIcon( this JobDef job )
        {
            if ( !JobIcons.ContainsKey( job ) )
                JobIcons.Add( job, ContentFinder<Texture2D>.Get( JobIconPaths[job] ) );
            return JobIcons[job];
        }

        public static string DefaultIconPath(this JobDef job)
        {
            // long list of vanilla jobs with presets
            // combat
            if (job == JobDefOf.AttackMelee || 
                job == JobDefOf.AttackStatic || 
                job == JobDefOf.FleeAndCower || 
                job == JobDefOf.ManTurret || 
                job == JobDefOf.WaitCombat ||
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
                job == JobDefOf.PrisonerFriendlyChat || 
                job == JobDefOf.ReleasePrisoner || 
                job == JobDefOf.TakeWoundedPrisonerToBed)
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
            if ( job == JobDefOf2.PlayBilliards ||
                 job == JobDefOf2.PlayChess ||
                 job == JobDefOf2.PlayHorseshoes || 
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
                job == JobDefOf.BuryCorpse || 
                job == JobDefOf.HaulToCell || 
                job == JobDefOf.HaulToContainer || 
                job == JobDefOf.Refuel || 
                job == JobDefOf.PrepareCaravan_GatherItems ||
                job == JobDefOf.FillFermentingBarrel ||
                job == JobDefOf.TakeBeerOutOfFermentingBarrel || 
                job == JobDefOf.GiveToPackAnimal )
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
                 job == JobDefOf.WaitMaintainPosture ||
                 job == JobDefOf.WaitSafeTemperature ||
                 job == JobDefOf.WaitWander )
                return "UI/Icons/Various/clock";

            // construct
            if (job == JobDefOf.FinishFrame || 
                job == JobDefOf.PlaceNoCostFrame || 
                job == JobDefOf.RemoveFloor || 
                job == JobDefOf.SmoothFloor || 
                job == JobDefOf.Uninstall || 
                job == JobDefOf.Deconstruct ||
                job == JobDefOf.BuildRoof || 
                job == JobDefOf.RemoveRoof )
                return "UI/Icons/Various/hammer";

            // repair
            if (job == JobDefOf.FixBrokenDownBuilding || 
                job == JobDefOf.Repair)
                return "UI/Icons/Various/wrench";

            // cook
            if ( job == JobDefOf.DeliverFood )
                return "UI/Icons/Various/chef";

            // bills
            // TODO: Try to split bills to type
            if (job == JobDefOf.DoBill)
                return "UI/Icons/Various/star";

            Logger.Debug( $"No icon set for JobDef {job.defName}." );
            return DefaultJobIconPath;
        }
    }
}