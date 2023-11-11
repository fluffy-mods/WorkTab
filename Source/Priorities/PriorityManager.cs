// PriorityManager.cs
// Copyright Karel Kroeze, 2020-2020

using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WorkTab {
    public class PriorityManager: GameComponent {
        private static PriorityManager _instance;
        private static int             _nextId;

        private static bool                                  _showScheduler;
        private static Dictionary<Pawn, PawnPriorityTracker> priorities = new Dictionary<Pawn, PawnPriorityTracker>();
        private        List<PawnPriorityTracker>             pawnPriorityTrackersScribe;
        private        List<Pawn>                            pawnsScribe;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "RimWorld requires a constructor with parameter")]
        public PriorityManager(Game game) : this() {
        }

        public PriorityManager() {
            _instance = this;
            _nextId = 0;
            _showScheduler = false;
            priorities = new Dictionary<Pawn, PawnPriorityTracker>();
        }

        public static PriorityManager Get {
            get {
                if (_instance == null) {
                    throw new NullReferenceException("Accessing PriorityManager before it was constructed.");
                }

                return _instance;
            }
        }

        public static bool ShowPriorities {
            get => Find.PlaySettings.useWorkPriorities;
            set {
                if (value == Find.PlaySettings.useWorkPriorities) {
                    return;
                }

                // update setting
                Find.PlaySettings.useWorkPriorities = value;

                // force re-cache of all pawns
                foreach (Pawn pawn in priorities.Keys.ToList()) {
                    pawn?.workSettings?.Notify_UseWorkPrioritiesChanged();
                }
            }
        }

        public static bool ShowScheduler {
            get => _showScheduler;
            set {
                if (value == _showScheduler) {
                    return;
                }

                _showScheduler = value;
                MainTabWindow_WorkTab.Instance.RecacheTimeBarRect();
            }
        }

        public PriorityTracker this[Pawn pawn] {
            get {
                Favourite favourite = FavouriteManager.Get[pawn];
                if (favourite != null) {
                    return favourite;
                }

                if (priorities.TryGetValue(pawn, out PawnPriorityTracker tracker)) {
                    return tracker;
                }

                tracker = new PawnPriorityTracker(pawn);
                priorities.Add(pawn, tracker);
                return tracker;
            }
        }

        public static int GetNextID() {
            return _nextId++;
        }

        public override void ExposeData() {
            base.ExposeData();

            // purge null pawn elements, note that this also neatly keeps track of periodic garbage collection on autosaves
            List<Pawn> pawns = priorities.Keys.ToList();
            foreach (Pawn pawn in pawns) {
                if (pawn?.Destroyed ?? true) // null or destroyed
                {
                    priorities.Remove(pawn);
                }
            }

            Scribe_Collections.Look(ref priorities, "Priorities", LookMode.Reference, LookMode.Deep, ref pawnsScribe,
                                     ref pawnPriorityTrackersScribe);
            Scribe_Values.Look(ref _showScheduler, "ShowScheduler");
            Scribe_Values.Look(ref _nextId, "NextId");
        }
    }
}
