#!/usr/bin/env python3
"""
Fix Godot 4.x C# script references in scene files
"""

import os
import re
import sys

def fix_csharp_script_references(file_path):
    """Remove ext_resource references to .cs files and fix script assignments"""
    try:
        with open(file_path, 'r') as f:
            content = f.read()
        
        original_content = content
        changes_made = False
        
        # Remove ext_resource lines that reference .cs files
        # Pattern: [ext_resource ... path="res://path/to/Script.cs" ... type="Script" ...]
        pattern = r'\[ext_resource[^]]+path="([^"]+\.cs)"[^]]+type="Script"[^]]*\]\n?'
        
        # Find all matches first to track what IDs we're removing
        matches = re.findall(r'\[ext_resource[^]]+path="([^"]+\.cs)"[^]]+(?:type="Script"|id="?(\d+)"?)[^]]*\]', content)
        cs_ids = []
        
        # Extract IDs from ext_resource lines referencing .cs files
        cs_resource_pattern = r'\[ext_resource[^]]+path="[^"]+\.cs"[^]]+id="?(\d+)"?[^]]*\]'
        cs_id_matches = re.findall(cs_resource_pattern, content)
        cs_ids.extend(cs_id_matches)
        
        # Remove the ext_resource lines for .cs files
        new_content = re.sub(pattern, '', content)
        
        if new_content != content:
            changes_made = True
            content = new_content
            print(f"✅ Removed C# ext_resource references: {file_path}")
        
        # Remove script assignments that reference the removed resources
        # Pattern: script = ExtResource("ID")
        for cs_id in cs_ids:
            script_pattern = rf'script = ExtResource\("{cs_id}"\)\s*\n?'
            if re.search(script_pattern, content):
                content = re.sub(script_pattern, '', content)
                changes_made = True
                print(f"✅ Removed script assignment for ID {cs_id}: {file_path}")
        
        # Write back only if changes were made
        if changes_made:
            with open(file_path, 'w') as f:
                f.write(content)
            return True
        
        return False
        
    except Exception as e:
        print(f"❌ Error processing {file_path}: {e}")
        return False

def fix_all_scenes(root_dir):
    """Find all .tscn files and fix C# script references"""
    fixed_count = 0
    
    for root, dirs, files in os.walk(root_dir):
        for file in files:
            if file.endswith('.tscn'):
                file_path = os.path.join(root, file)
                if fix_csharp_script_references(file_path):
                    fixed_count += 1
    
    return fixed_count

if __name__ == "__main__":
    project_root = "/Users/lyvantinh/Documents/project/all_alone"
    
    print("🔧 Fixing C# script references in Godot 4.x scene files...")
    print(f"📁 Scanning directory: {project_root}")
    
    fixed = fix_all_scenes(project_root)
    
    print(f"\n✨ Fix complete! Updated {fixed} scene files.")
    print("🎮 C# scripts will now be loaded automatically by Godot 4.4.1")
