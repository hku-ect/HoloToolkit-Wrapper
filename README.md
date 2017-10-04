# HoloToolkit-Wrapper
Set of wrapper classes for the Microsoft HoloToolkit for Unity &amp; HoloLens

Includes HoloToolkit version specifically suited to Unity 5.6.1 (see https://github.com/Microsoft/MixedRealityToolkit-Unity)

Works with:
  - Unity 5.6.1p4 (specifically Patch 4, because it fixes a SpatialMapping bug)
  - Microsoft UWP SDK 10.0.14393.x
  - Visual Studio 2015
  
 Install guide: https://hololens.reality.news/how-to/hololens-dev-101-build-basic-hololens-app-minutes-0175021/

May work with combinations of newer UWP SDK & Visual Studio, but in my case using latest SDK broke compatibility with VS2015 & VS2017.

# Examples
Contains two example setups:
1. Simple scene containing a moveable anchor.
2. Set of two scenes (scanner/followup) that first scans the environment, and then attempts to place a set of objects in the followup.

Each scene contains standard HoloToolkit prefabs, that have been placed inside of a recognizable GameObject.

## Simple Scene
A Cube in the scene has been anchored here using the WrappedAnchor script. This is a script that you can place on any GameObject, that will then attempt to attach itself to an anchor with the name you've provided. If no anchor is found (currently only local for this application), it will create one.

The Cube also contains a collider, and the WrappedAnchor script has indicated this object can be moved. At runtime, the script adds a "HandDraggable" instance to allow for moving of the object. It is "de-anchored" when grabbed, and "re-anchored" when dropped.

## Scanner + Followup
This setup is a little more complicated. It requires a few more steps.

First off, the idea is that you "spatial map" your room before attempting to place objects. This is why the scenes have been split up.

Second, the spatial mapping objects are placed in a separate DontDestroyOnLoad object, so they can be "saved" for the next scene. This is required to remember the scan that you've performed.

After a fixed time (you can find this in the ScanForNextScene script, right now it is 30 seconds) it continues to the followup scene, at which point the PlacableAnchor objects start checking if they can be placed. This currently freezes the application, because the PlaceObject function is rather heavy and runs on the main thread.

PlacableAnchors extend the WrappedAnchor, so can be moved if they have a collider. Other things they let you do:
- Indicate a volume in space that corresponds with the size of the (group of) objects
- Indicate which objects (preferrably children) should be activated once the anchor is placed (so objects don't pop-and-move once you enter the scene)
