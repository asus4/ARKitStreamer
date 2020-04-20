# ARKit Streamer

A remote debugging tool for AR Founndation (tested on `4.0.0-preview.1`) with ARKit3 featrues. This is temporary until the Unity team has completed the AR remote functionality - [Unity forum link](https://forum.unity.com/threads/ar-remoting-simulation.720575/)

![img1](https://i.imgur.com/vZoYIs1.gif)  
Human segmantation  

![img2](https://imgur.com/tQbJ1Sl.gif)  
Sample with VFX Graph  

## Environments

- Tested on Unity 2019.3.3f1
- iPhone X or more

## Supporting ARKit features

- Human Segmentation
- Face (You need to apply the patch ar-foundation's code as described below)
- Plane tracking
- 3D body tracking

## Install

- This depends on NDI (Network Device Interface), download and install NDI SDK for iOS from [https://ndi.tv/sdk/](https://ndi.tv/sdk/)

- This supports Unity Package Manager. Open the file `Packages/manifest.json` and add following lines into `scopedRegistries` and  `dependencies` section.

```json
{
  "scopedRegistries": [
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
    "com.koki-ibukuro.arkitstream": "0.3.4",
    ...// other dependencies
  }
}
```

- Apply 2 patch files
  - ARFace.cs see [issue #2](https://github.com/asus4/ARKitStreamer/issues/2)

    ```sh
    patch -u Library/PackageCache/com.unity.xr.arfoundation@4.0.0-preview.1/Runtime/AR/ARFace.cs < Tools/ARFace.cs.patch
    ```

  - Klak.Ndi on Unity2019.3 or more [issue #7](https://github.com/asus4/ARKitStreamer/issues/7)

    ```sh
    patch -u Library/PackageCache/jp.keijiro.klak.ndi@0.2.4/Editor/PbxModifier.cs < Tools/PbxModifier.cs.patch
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

## Libraries

- [KlakNDI](https://github.com/keijiro/KlakNDI/)
- [websocket-sharp](https://github.com/sta/websocket-sharp/)
