# WebXR XR Interaction Toolkit Bridge
## Summary
- Package to correspond values from De-Panther's unity-webxr-export to InputSystem and XR Interaction Toolkit
- Currently VR is the only supported use case (and only with controllers and not hand tracking)

## Functionality
- Controllers fully functional with XRIT
  - Recommand using the XRIT prefab rig with XRIT Starter Asset Samples (get from Unity Package Manager)
  - Controllers values (position, rotation) are passed to XRIT through InputSystem
- Cameras (Left, Right) created when entering WebXR VR mode
  - Camera on XRIT prefab rig not used
  - Camera values from the rig (which is tagged as MainCamera) not copied over
  - Headsets values (position, rotation) are passed to XRIT through InputSystem

## Use
- Recommand using the XRIT prefab rig with XRIT Starter Asset Samples (get from Unity Package Manager)
- Put the XRIT prefab into scenes
- Add the included Prefab in the scene (does not need to be parented)
- Follow [unity-webxr-export page](https://de-panther.github.io/unity-webxr-export/ ) on setting up building
