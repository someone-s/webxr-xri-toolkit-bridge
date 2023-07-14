# WebXR XR Interaction Toolkit Bridge
## Summary
- Package to correspond values from De-Panther's unity-webxr-export to InputSystem and XR Interaction Toolkit
- Currently VR is the only supported use case (and only with controllers and not hand tracking)
- The current stage is very barebones, missing many features

## Functionality
- Controllers fully functional with XRIT
  - Recommand using the XRIT prefab rig with XRIT Starter Asset Samples (get from Unity Package Manager)
  - Controllers values (position, rotation) are passed to XRIT through InputSystem
- Cameras (Left, Right) created when entering WebXR VR mode
  - Camera on XRIT prefab rig not used
  - Camera values from the rig (which is tagged as MainCamera) not copied over
  - Headsets values (position, rotation) are passed to XRIT through InputSystem

## Use
- **Use** the XRIT prefab rig with XRIT Starter Asset Samples (get from Unity Package Manager) **\<Optional, Recommanded\>**
  - **Put** the XRIT prefab into scenes
- **Follow**, **until before build**, [unity-webxr-export page](https://de-panther.github.io/unity-webxr-export/Documentation/Getting-Started.html)
  - **Note** webxr-interaction is not used or required (and can be omited)
- **Import**
  - [![openupm](https://img.shields.io/npm/v/com.someone-s.webxr-xrit-bridge?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.someone-s.webxr-xrit-bridge/)
  - **Add** the included **Prefab** in scenes (does not need to be parented)
- **Continue** following [unity-webxr-export page](https://de-panther.github.io/unity-webxr-export/Documentation/Getting-Started.html)
  - **Note** some web server throws error relating to compression, 
    if so, disable Compression at ``Project Settings > Player > Web tab(the icon left of Android) > Publishing Settings > Compression Format``

## Missing features
- Hand tracking (bridge WebXR to com.unity.xr.hands)
- Better way to handle the Camera(s), options:
  1. Do some calculation and troubleshoothing to use the existing XRIT prefab camera
      - Create and Set correct steroViewMatrices and steroProjectionMatrices on Camera.main
      - Figure out how to enable stero on Camera.main when entering VR mode
  2. Copy Camera.main settings to the created camera
      - Have a camera (maybe a 3rd one that does not render) acting as Camera.main
        (disabling the Camera component on the GameObject tagged with MainCamera will set Camera.main to null)
      - Copy camera settings to and from the original main camera
