import os
import shutil

# Ensure file location is current directory
os.chdir(os.path.dirname(os.path.abspath(__file__)))

# Search some common paths for Secrets of Grindea install folder
sog_common_paths = [
    'C:/Program Files (x86)/Steam/steamapps/common/SecretsOfGrindea/',
    'D:/Program Files (x86)/Steam/steamapps/common/SecretsOfGrindea/',
    'E:/Program Files (x86)/Steam/steamapps/common/SecretsOfGrindea/',
    'F:/Program Files (x86)/Steam/steamapps/common/SecretsOfGrindea/'
]

sog_install_path = None

# Search for SoG install path
for path in sog_common_paths:
    if os.path.exists(path + 'Secrets of Grindea.exe'):
        sog_install_path = path
        break

if sog_install_path is not None:
    print('Found SoG install path:', sog_install_path)
else:
    print('Couldn\'t find SoG install path. Skipping...')
    
output_path = '../Grindless/bin/x86/Debug/net472/merged/'

if sog_install_path is not None:
    try:
        os.mkdir(sog_install_path + 'Mods')
    except:
        pass
    shutil.copyfile(output_path + 'Grindless.exe', sog_install_path + 'Grindless.exe')
    print('Installed Grindless in SoG directory.')
