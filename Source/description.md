Provides a vastly more customizable work tab.

# Important

Work Tab completely takes over job priorities from the vanilla game. In order to support core functionalities and other mods, it intercepts calls to get/set priorities. However, when it is told to set priorities by other modded code that is not aware of the time schedule or detailed priorities, the priority will be set for the whole day, and/or for all workgivers in a worktype.

# Features

Various usability extentions to the 'vanilla' work tab;

- Work types can be expanded (by Ctrl+clicking the column header) to allow you to set priorities for the individual tasks within each work type.
- Time scheduler to set priorities for a given time slot only - allowing you to designate a cleaning hour, or have your cook prepare meals right before dinner, etc. etc.
- Up to 9 priority levels (configurable in mod options)
- Various small UX tweaks; scrolling to increase/decrease/toggle priorities, increase/decrease priorities for whole columns/rows (by holding shift and clicking/scrolling while hovering over the column header/pawn name respectively).
- _All functions are detailed in the tooltips, take a moment to hover over and read them!_

# Known Issues

"Star Wars -- The Force" versions prior to 1.21.1 cause priorities to reset for force users. **THIS INCLUDES THE CURRENT STEAM VERSION OF STAR WARS -- THE FORCE!** (as of 25/3/20). There is an official update available by one of the collaborators on the mod on [GitHub](https://github.com/jecrell/Star-Wars---The-Force/releases).

# Notes

With great power comes great responsibility. The default priorities of tasks within a job is set for a good reason; it's (usually) a sensible default. Changing these can lead to deadlock situations, so change the priorities of individual jobs at your own risk!

Finally, there will never be an 'autolabour' mode where a mod sets priorities for you. Due to the way the AI is handled (e.g. pawns actively look for work, instead of there being a 'bulletin board' of jobs that need doing), it's not feasible to get the complete list of work that needs doing that would be needed to make this a reality, without extreme overhead and loads of special exception coding.

# Powered by Harmony

![Powered by Harmony](https://camo.githubusercontent.com/074bf079275fa90809f51b74e9dd0deccc70328f/68747470733a2f2f7332342e706f7374696d672e6f72672f3538626c31727a33392f6c6f676f2e706e67)
