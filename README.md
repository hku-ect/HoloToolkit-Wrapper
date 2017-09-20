# HoloToolkit-Wrapper
Set of wrapper classes for the Microsoft HoloToolkit for Unity &amp; HoloLens

Includes HoloToolkit version specifically suited to Unity 5.6.1 (see https://github.com/Microsoft/MixedRealityToolkit-Unity)

Works with:
  - Unity 5.6.1p4 (specifically Patch 4, because it fixes a SpatialMapping bug)
  - Microsoft UWP SDK 10.0.14393.x
  - Visual Studio 2015

May work with combinations of newer UWP SDK & Visual Studio, but in my case using latest SDK broke compatibility with VS2015 & VS2017.

# Examples
Contains two example setups:
1. Simple scene containing a moveable anchor.
2. Set of two scenes (scanner/followup) that first scans the environment, and then attempts to place a set of objects in the followup.
