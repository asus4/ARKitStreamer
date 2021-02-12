#!/bin/bash -xe

PACKAGE_VERSION=$(sed -n -E 's/ *"com.unity.xr.arfoundation": "([.0-9\-preview]*)",/\1/p' Packages/manifest.json)

patch -u Library/PackageCache/com.unity.xr.arfoundation@${PACKAGE_VERSION}/Runtime/AR/ARCameraBackground.cs < Tools/ARCameraBackground.cs.patch
patch -u Library/PackageCache/com.unity.xr.arfoundation@${PACKAGE_VERSION}/Runtime/AR/ARFace.cs < Tools/ARFace.cs.patch

