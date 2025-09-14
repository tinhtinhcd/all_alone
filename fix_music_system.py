#!/usr/bin/env python3
"""
Fix MusicSystem.tscn ExtResource format for Godot 4.x
"""

import re

def fix_music_system():
    file_path = "/Users/lyvantinh/Documents/project/all_alone/src/autoloads/music_manager/MusicSystem.tscn"
    
    try:
        with open(file_path, 'r') as f:
            content = f.read()
        
        # Fix ext_resource format - add quotes around ID numbers and fix format
        # From: [ext_resource path="..." type="..." id=1]
        # To:   [ext_resource type="..." path="..." id="1"]
        content = re.sub(
            r'\[ext_resource path="([^"]+)" type="([^"]+)" id=(\d+)\]',
            r'[ext_resource type="\2" path="\1" id="\3"]',
            content
        )
        
        # Fix ExtResource references - add quotes around numbers
        # From: ExtResource( 1 )
        # To:   ExtResource("1")
        content = re.sub(r'ExtResource\(\s*(\d+)\s*\)', r'ExtResource("\1")', content)
        
        # Fix SubResource references - add quotes around numbers  
        # From: SubResource( 1 )
        # To:   SubResource("1")
        content = re.sub(r'SubResource\(\s*(\d+)\s*\)', r'SubResource("\1")', content)
        
        with open(file_path, 'w') as f:
            f.write(content)
        
        print(f"✅ Fixed MusicSystem.tscn ExtResource format")
        return True
        
    except Exception as e:
        print(f"❌ Error fixing MusicSystem.tscn: {e}")
        return False

if __name__ == "__main__":
    print("🔧 Fixing MusicSystem.tscn for Godot 4.x compatibility...")
    if fix_music_system():
        print("✨ MusicSystem.tscn is now Godot 4.x compatible!")
    else:
        print("❌ Failed to fix MusicSystem.tscn")
