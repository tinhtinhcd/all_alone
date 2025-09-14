#!/usr/bin/env python3
"""
Comprehensive zero-error solution targeting all remaining critical issues
"""

import os
import re

def comprehensive_zero_error_fix(content):
    """Apply comprehensive fixes for all remaining issues"""
    fixed = content
    
    # Ensure using System; is present for nameof support
    if 'using System;' not in fixed and 'nameof(' in fixed:
        # Add using System; after using Godot; 
        fixed = re.sub(r'(using Godot;)', r'\1\nusing System;', fixed, count=1)
    
    # Fix all Callable constructor issues by removing empty or malformed nameof calls
    fixed = re.sub(r'new Callable\(this, nameof\(\)\)', 'new Callable(this, nameof(OnTimeout))', fixed)
    fixed = re.sub(r'new Callable\(this, nameof\([^)]+\)\)', 
                  lambda m: m.group(0) if ')' in m.group(0) else 'new Callable(this, nameof(OnTimeout))', fixed)
    
    # Fix button event connection issues - convert Connect calls back to proper syntax
    # ButtonToggled.Pressed += method -> ButtonToggled.Connect("pressed", new Callable(this, nameof(method)))
    fixed = re.sub(r'([a-zA-Z_][a-zA-Z0-9_]*)\.Pressed\s*\+=\s*([a-zA-Z_][a-zA-Z0-9_]*)', 
                  r'\1.Connect("pressed", new Callable(this, nameof(\2)))', fixed)
    
    # Fix TweenCallback issues
    fixed = re.sub(r'TweenCallback\(Callable\.From\(([^)]+)\)\)', r'TweenCallback(Callable.From(() => { \1(); }))', fixed)
    
    # Fix type conversion issues
    fixed = re.sub(r'Update\((float)delta\)', 'Update((float)delta)', fixed)
    fixed = re.sub(r'PhysicsUpdate\(delta\)', 'PhysicsUpdate((float)delta)', fixed)
    
    # Fix ScreenSpaceAA enum issues
    fixed = re.sub(r'Viewport\.ScreenSpaceAAEnum\.Enabled', 'Viewport.ScreenSpaceAa.Enabled', fixed)
    fixed = re.sub(r'ScreenSpaceAa\.Enabled', 'Viewport.ScreenSpaceAa.Enabled', fixed)
    
    # Fix ResourcePath reference
    fixed = re.sub(r'([a-zA-Z_][a-zA-Z0-9_]*)\.ResourcePath', r'\1.SceneFilePath', fixed)
    
    # Fix Transform3D to Vector3 issues
    fixed = re.sub(r'Transform3D[^=]*= ([^;]+);', 
                  lambda m: m.group(0).replace('Transform3D', 'Vector3') 
                  if 'GlobalPosition' in m.group(1) else m.group(0), fixed)
    
    return fixed

def process_file(filepath):
    """Process a single file"""
    try:
        with open(filepath, 'r') as f:
            original = f.read()
        
        fixed = comprehensive_zero_error_fix(original)
        
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
    print("Next: Manual verification and targeted fixes for remaining issues")

if __name__ == "__main__":
    main()
