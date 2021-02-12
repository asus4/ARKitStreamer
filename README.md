# ARKit Streamer

A remote debugging tool for AR Founndation (tested on `4.1.3`) with ARKit3 featrues. This is temporary until the Unity team has completed the AR remote functionality - [Unity forum link](https://forum.unity.com/threads/ar-remoting-simulation.720575/)

![img1](https://i.imgur.com/vZoYIs1.gif)  
Human segmantation  

![img2](https://user-images.githubusercontent.com/357497/89782917-66e0e680-db16-11ea-856b-fb6782ec0b23.gif)  
Sample with VFX Graph  

## Environments

- Tested on Unity 2019.4.17f1
- iPhone X or more

## Supporting ARKit features

- Basic camera position tracking
- Send camera image via NDI
- Human Segmentation / Depth
- Face
- Plane tracking
- 3D body tracking

## How to Install

- Depends on NDI (Network Device Interface), download the NDI SDK for iOS from [https://ndi.tv/sdk/](https://ndi.tv/sdk/)

- Open the file `Packages/manifest.json` and add following lines into `scopedRegistries` and  `dependencies` section.

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
    "com.koki-ibukuro.arkitstream": "0.3.6",
    ...// other dependencies
  }
}
```

- Apply a patch file

Some source codes in AR foundation packages need to be modified to work with Unity Editor. Apply patch files in the Tools folder.

```sh
# Sample command on macOS terminal
sh Tools/apply_patch.sh
```

## How to Use

### Set up Sender iPhone

Download this reposidoty and install on iPhone. Then run the app on iPhone.

Or you can simply add `ARKitSender` to your custom ARKit scene.  
![Imgur](https://imgur.com/tevPT1n.png)

Recomend using USB connected network instead of Wi-Fi to reduce network delay.  
![Imgur](https://imgur.com/4YVbIUP.png)

### Simurate on Editor

Add `ARKitReceiver` to the scene which you want to simuirate on Editor. If sender-iPhone app is running, you can select the NDI source name. also confirm the iPhone's IP Address and port are correct.

![Imgur](https://imgur.com/u10iUBc.gif)

For more infomation, please check Assets/Sample directory.

### Setting for LWRP / URP

If you use LWRP / URP, you need also add the ARBackgroundRemoteRendererFeature to the list of render features. See [AR Foundation Document](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/api/UnityEngine.XR.ARFoundation.ARCameraBackground.html) for more information.

![Imgur](https://imgur.com/CRC99iQ.png)

## Libraries

- [KlakNDI](https://github.com/keijiro/KlakNDI/)
- [websocket-sharp](https://github.com/sta/websocket-sharp/)
