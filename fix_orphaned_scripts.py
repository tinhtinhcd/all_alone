#!/usr/bin/env python3
"""
Final comprehensive fix for Godot 4.x scene files
- Remove orphaned script assignments that reference non-existent ExtResources
"""

import os
import re

def fix_orphaned_script_assignments(file_path):
    """Remove script assignments that reference non-existent ExtResource IDs"""
    try:
        with open(file_path, 'r') as f:
            content = f.read()
        
        original_content = content
        
        # Find all ext_resource IDs that exist
        existing_ids = set()
        ext_resource_pattern = r'\[ext_resource[^]]+id="(\d+)"[^]]*\]'
        existing_ids.update(re.findall(ext_resource_pattern, content))
        
        # Find script assignments and check if they reference existing IDs
        script_assignments = re.findall(r'script = ExtResource\("(\d+)"\)', content)
        
        # Remove script assignments that reference non-existent IDs
        for script_id in script_assignments:
            if script_id not in existing_ids:
                # Remove the entire script assignment line
                pattern = rf'script = ExtResource\("{script_id}"\)\s*\n?'
                content = re.sub(pattern, '', content)
        
        if content != original_content:
            with open(file_path, 'w') as f:
                f.write(content)
            print(f"✅ Fixed orphaned script assignments: {file_path}")
            return True
        
        return False
        
    except Exception as e:
        print(f"❌ Error processing {file_path}: {e}")
        return False

def fix_all_orphaned_scripts(root_dir):
    """Fix orphaned script assignments in all .tscn files"""
    fixed_count = 0
    
    for root, dirs, files in os.walk(root_dir):
        for file in files:
            if file.endswith('.tscn'):
                file_path = os.path.join(root, file)
                if fix_orphaned_script_assignments(file_path):
                    fixed_count += 1
    
    return fixed_count

if __name__ == "__main__":
    project_root = "/Users/lyvantinh/Documents/project/all_alone"
    
    print("🔧 Removing orphaned script assignments from scene files...")
    print(f"📁 Scanning directory: {project_root}")
    
    fixed = fix_all_orphaned_scripts(project_root)
    
    print(f"\n✨ Fix complete! Cleaned up {fixed} scene files.")
    print("🎮 Scene files should now load properly in Godot 4.4.1")
