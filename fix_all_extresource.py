#!/usr/bin/env python3
"""
Fix ALL ExtResource format issues in Godot 4.x scene files
"""

import os
import re

def fix_extresource_format(file_path):
    """Fix ExtResource format from Godot 3.x to 4.x in a single file"""
    try:
        with open(file_path, 'r') as f:
            content = f.read()
        
        original_content = content
        
        # Fix ExtResource references - add quotes around numbers
        # From: ExtResource( 1 ) or ExtResource(1)
        # To:   ExtResource("1")
        content = re.sub(r'ExtResource\(\s*(\d+)\s*\)', r'ExtResource("\1")', content)
        
        # Fix SubResource references - add quotes around numbers  
        # From: SubResource( 1 ) or SubResource(1)
        # To:   SubResource("1")
        content = re.sub(r'SubResource\(\s*(\d+)\s*\)', r'SubResource("\1")', content)
        
        if content != original_content:
            with open(file_path, 'w') as f:
                f.write(content)
            print(f"✅ Fixed ExtResource format: {file_path}")
            return True
        
        return False
        
    except Exception as e:
        print(f"❌ Error processing {file_path}: {e}")
        return False

def fix_all_scene_files(root_dir):
    """Fix ExtResource format in all .tscn files"""
    fixed_count = 0
    
    for root, dirs, files in os.walk(root_dir):
        for file in files:
            if file.endswith('.tscn'):
                file_path = os.path.join(root, file)
                if fix_extresource_format(file_path):
                    fixed_count += 1
    
    return fixed_count

if __name__ == "__main__":
    project_root = "/Users/lyvantinh/Documents/project/all_alone"
    
    print("🔧 Fixing ExtResource format in ALL Godot scene files...")
    print(f"📁 Scanning directory: {project_root}")
    
    fixed = fix_all_scene_files(project_root)
    
    print(f"\n✨ Fix complete! Updated {fixed} scene files with proper ExtResource format.")
    print("🎮 All scene files should now be Godot 4.x compatible")
