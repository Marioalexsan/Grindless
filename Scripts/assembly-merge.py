import os
import itertools

# Ensure file location is current directory
os.chdir(os.path.dirname(os.path.abspath(__file__)))

# Do not merge assemblies that may cause issues
skip_assemblies = [
    'Grindless.exe',
    'Lidgren.Network.dll',
    'Steamworks.NET.dll'
]

target_assembly = 'Grindless.exe'
ilrepack_path = os.path.abspath('ILRepack.exe')

os.chdir('../Grindless/bin/x86/Debug/net472')
assemblies = [x for x in os.listdir('./') if x.endswith('.dll') and not 'Xna' in x and not any([True for y in skip_assemblies if y in x])]

# Use ILRepack to join all assemblies into Grindless.exe
cmd = [
    ilrepack_path,
    '/out:merged/' + target_assembly,
    '/zeropekind',
    '/union',
    '/xmldocs',
    target_assembly,
    ' '.join(assemblies)
]

os.system(' '.join(cmd))
