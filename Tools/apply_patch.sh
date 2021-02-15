#!/bin/bash -xe

PACKAGE_VERSION=$(sed -n -E 's/ *"com.unity.xr.arfoundation": "([.0-9\-preview]*)",/\1/p' Packages/manifest.json)
ARFOUNDATION=Library/PackageCache/com.unity.xr.arfoundation@${PACKAGE_VERSION}/Runtime/AR

patch -u ${ARFOUNDATION}/ARCameraBackground.cs < Tools/ARCameraBackground.cs.patch
patch -u ${ARFOUNDATION}/ARFace.cs < Tools/ARFace.cs.patch
rm ${ARFOUNDATION}/*.cs.rej