import os
import shutil

# Ensure file location is current directory
os.chdir(os.path.dirname(os.path.abspath(__file__)))

# SoG dependencies
sog_deps = [
    'Secrets of Grindea.exe',
    'Secrets of Grindea.pdb',
    'Lidgren.Network.dll',
    'Lidgren.Network.pdb',
    'Steamworks.NET.dll'
]

# XNA dependencies, DLLs only, omit '.dll' part
xna_deps = [
    'Microsoft.Xna.Framework',
    'Microsoft.Xna.Framework.Game',
    'Microsoft.Xna.Framework.Graphics',
    'Microsoft.Xna.Framework.Xact',
]

# Search some common paths for Secrets of Grindea install folder
sog_common_paths = [
    'C:/Program Files (x86)/Steam/steamapps/common/SecretsOfGrindea/',
    'D:/Program Files (x86)/Steam/steamapps/common/SecretsOfGrindea/',
    'E:/Program Files (x86)/Steam/steamapps/common/SecretsOfGrindea/',
    'F:/Program Files (x86)/Steam/steamapps/common/SecretsOfGrindea/'
]

# Search some common paths for XNA install path (usually the GAC)
gac_common_paths = [
    'C:/Windows/Microsoft.NET/assembly/GAC_32/'
]

sog_install_path = None
xna_install_path = None

# Search for SoG install path
for path in sog_common_paths:
    if os.path.exists(path + sog_deps[0]):
        sog_install_path = path
        break

# Search for XNA in GAC
for path in gac_common_paths:
    if os.path.exists(path + xna_deps[0]):
        xna_install_path = path
        break

if sog_install_path is not None:
    print('Found SoG install path:', sog_install_path)
else:
    print('Couldn\'t find SoG install path. Skipping...')

    
if xna_install_path is not None:
    print('Found XNA install path:', xna_install_path)
else:
    print('Couldn\'t find XNA install path. Skipping...')

if sog_install_path is not None:
    for x in sog_deps:
        shutil.copyfile(sog_install_path + x, '../Dependencies/' + x)
    print('Copied SoG dependencies!')


if xna_install_path is not None:
    for x in xna_deps:
        base_path = xna_install_path + x + '/'
        dll_path = base_path + [x for x in os.listdir(base_path) if 'v4.0' in x][0] + '/' + x + '.dll'
        shutil.copyfile(dll_path, '../Dependencies/' + x + '.dll')
    print('Copied XNA dependencies!')