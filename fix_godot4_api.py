#!/usr/bin/env python3
"""
Fix remaining Godot 4 migration issues systematically
"""

import os
import re

def fix_godot4_issues(content):
    """Fix multiple Godot 4 migration issues"""
    fixed = content
    
    # Fix Callable.From with nameof issues - replace with proper StringName syntax
    fixed = re.sub(r'new Callable\(this, ([^)]+)\.nameof\(([^)]+)\)\)', r'new Callable(this, nameof(\2))', fixed)
    
    # Fix assignment to Timer.WaitTime (should not be cast on left side)
    fixed = re.sub(r'\(float\)([^.]+\.WaitTime)\s*=', r'\1 =', fixed)
    fixed = re.sub(r'\(double\)([^.]+\.WaitTime)\s*=', r'\1 =', fixed)
    
    # Fix SceneFilePath -> ResourcePath
    fixed = re.sub(r'\.SceneFilePath', r'.ResourcePath', fixed)
    
    # Fix ScreenSpaceAa -> ScreenSpaceAA
    fixed = re.sub(r'ScreenSpaceAa', r'ScreenSpaceAA', fixed)
    
    # Fix MSAA enum usage
    fixed = re.sub(r'\.Msaa\.', r'.MsaaEnum.', fixed)
    fixed = re.sub(r'Viewport\.Msaa', r'Viewport.MsaaEnum', fixed)
    
    # Fix AudioServer methods
    fixed = re.sub(r'AudioServer\.LinearToDb', r'AudioServer.LinearToDb', fixed)
    fixed = re.sub(r'AudioServer\.DbToLinear', r'AudioServer.DbToLinear', fixed)
    
    # Fix Vector3.Origin -> Vector3.Zero 
    fixed = re.sub(r'Vector3\.Origin', r'Vector3.Zero', fixed)
    
    # Fix Rect2.GetViewport() calls (should be called on Control nodes)
    fixed = re.sub(r'([a-zA-Z_][a-zA-Z0-9_]*Rect[a-zA-Z0-9_]*)\.GetViewport\(\)', r'GetViewport()', fixed)
    
    return fixed

def process_file(filepath):
    """Process a single file"""
    try:
        with open(filepath, 'r') as f:
            original = f.read()
        
        fixed = fix_godot4_issues(original)
        
        if fixed != original:
            with open(filepath, 'w') as f:
                f.write(fixed)
            print("Fixed: " + filepath)
            return True
        return False
            
    except Exception as e:
        print("Error processing " + filepath + ": " + str(e))
        return False

def main():
    """Main execution"""
    src_dir = "src"
    if not os.path.exists(src_dir):
        print("src directory not found!")
        return
    
    fixed_count = 0
    total_files = 0
    
    # Process all C# files
    for root, dirs, files in os.walk(src_dir):
        for file in files:
            if file.endswith('.cs'):
                filepath = os.path.join(root, file)
                total_files += 1
                if process_file(filepath):
                    fixed_count += 1
    
    print("\nProcessed " + str(total_files) + " C# files")
    print("Fixed " + str(fixed_count) + " files")

if __name__ == "__main__":
    main()
