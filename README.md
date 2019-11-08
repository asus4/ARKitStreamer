# ARKit Streamer

![img](https://i.imgur.com/vZoYIs1.gif)  
![Imgur](https://imgur.com/tQbJ1Sl.gif)  

A remote debugging tool for AR Founndation (tested on `3.0.0-preview.3`) with ARKit3 featrues.  

## Environments

- Unity 2019.2 or more
- iPhone X or more

## Install

This library supports Unity Package Manager. Open the file `Packages/manifest.json` and add following lines into `scopedRegistries` and  `dependencies` section.

```json
{
  "scopedRegistries": [
    {
      "name": "Keijiro",
      "url": "https://registry.npmjs.com",
      "scopes": [ "jp.keijiro" ]
    }
  ],
  "dependencies": {
    "jp.keijiro.klak.ndi": "0.2.3",
    "com.koki-ibukuro.arkitstream": "https://github.com/asus4/ARKitStreamer.git#upm",
    ...
  }
}
```

## How to Use

- This depends on NDI, download and install NDI SDK for iOS from [https://ndi.tv/sdk/](https://ndi.tv/sdk/)

- Recomend using USB connected network instead of Wi-Fi to reduce network delay.  
![Imgur](https://imgur.com/4YVbIUP.png)

## Dependences

- [KlakNDI](https://github.com/keijiro/KlakNDI/)
- [websocket-sharp](https://github.com/sta/websocket-sharp/)
