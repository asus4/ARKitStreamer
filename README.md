# ARKit Streamer

A remote debugging tool for AR Founndation (tested on `3.0.0-preview.4`) with ARKit3 featrues.  

![img](https://i.imgur.com/vZoYIs1.gif)  

![Imgur](https://imgur.com/tQbJ1Sl.gif)  

## Environments

- Unity 2019.2 or more
- iPhone X or more

## Supporting ARKit features

- Depth
- Face (You have to modify ar-foundation's code a bit. [See issee #2](https://github.com/asus4/ARKitStreamer/issues/2))

## Install

This library supports Unity Package Manager. Open the file `Packages/manifest.json` and add following lines into `scopedRegistries` and  `dependencies` section.

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
    "com.koki-ibukuro.arkitstream": "0.1.2",
    ...// other dependencies
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
