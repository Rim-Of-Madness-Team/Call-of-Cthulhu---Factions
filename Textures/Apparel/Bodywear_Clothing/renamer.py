# -*- coding: utf-8 -*-
"""
Renames all texture files requiring graphic_multi to the correct extension.

_back -> _north
_front -> _south
_side -> _east

Place this file in your mod folder for it to work.
@author: Spdskatr
"""
import os

for root, dirs, files in os.walk("."):
    for file in files:
        if file.endswith(".png") and "ShirtButton" in file:
            newName = file.replace("ShirtButton", "ShirtPlaid")
            os.rename(os.path.join(root, file), 
                      os.path.join(root, file.replace("ShirtButton", "ShirtPlaid")))
            print(file, "->", newName)