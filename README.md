# ARKit Streamer

[![openupm](https://img.shields.io/npm/v/com.koki-ibukuro.arkitstream?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.koki-ibukuro.arkitstream/)

A remote debugging tool for AR Founndation with ARKit4 featrues. This is temporary solution until the Unity team has completed the AR remote functionality - See [Unity forum](https://forum.unity.com/threads/ar-remoting-simulation.720575/) for more information.

![img1](https://i.imgur.com/vZoYIs1.gif)  
Human segmantation  

![img2](https://user-images.githubusercontent.com/357497/89782917-66e0e680-db16-11ea-856b-fb6782ec0b23.gif)  
Sample with VFX Graph  

## Environments

- Tested on Unity 2020.3.1f1
- ARFoundation 4.1.7
- iPhone X or more

## Supporting ARKit features

- Basic camera position tracking
- Send camera image via NDI
- Human Segmentation / Depth
- Face
- Plane tracking
- 3D body tracking

## How to Install

### Install NDI SDK

Depends on NDI (Network Device Interface), download the NDI SDK for iOS from [https://ndi.tv/sdk/](https://ndi.tv/sdk/)

### Install dependencies

Open the file `Packages/manifest.json` and add following lines into `scopedRegistries` and `dependencies` section.

```json
{
  "scopedRegistries": [
    {
      "name": "Unity NuGet",
      "url": "https://unitynuget-registry.azurewebsites.net",
      "scopes": [ "org.nuget" ]
    },
    {
      "name": "npm",
      "url": "https://registry.npmjs.com",
      "scopes": [
        "jp.keijiro",
        "com.koki-ibukuro"
      ]
    }
  ],
  "dependencies": {
    "com.koki-ibukuro.arkitstream": "0.4.2",
    ...// other dependencies
  }
}
```

### Apply patch files

Some sources in the AR foundation packages need to be modified to work with Unity Editor. Apply the patch command from `Window/Tools/ARKit Stream/Apply Patches` in the Unity Editor.  

Also you can revert the patches from `Window/Tools/ARKit Stream/Revert Patches`

## How to Use

### Set up Sender iPhone

Download this reposidoty and install on iPhone. Then run the app on iPhone.

Or you can simply add `ARKitSender` to your custom ARKit scene.  
![Imgur](https://imgur.com/tevPT1n.png)

Open the project settings and enable "ARKit Stream" as an XR plug-in for Unity Editor.
![fig](https://imgur.com/pVxnPm4.png)

Recomend using USB connected network instead of Wi-Fi to reduce network delay.  
![Imgur](https://imgur.com/4YVbIUP.png)

### Simurate on Editor

Add `ARKitReceiver` to the scene which you want to simuirate on Editor. Make sure the iPhone's IP Address and port are correct.

![fig](https://imgur.com/xzTMJUi.png)

See the `Assets/Sample` for more infomation.

### Setting for LWRP / URP

If you use LWRP / URP, you need also add the ARBackgroundRemoteRendererFeature to the list of render features. See [AR Foundation Document](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/api/UnityEngine.XR.ARFoundation.ARCameraBackground.html) for more information.

![Imgur](https://imgur.com/CRC99iQ.png)

## Libraries

- [KlakNDI](https://github.com/keijiro/KlakNDI/)
- [websocket-sharp](https://github.com/sta/websocket-sharp/)
