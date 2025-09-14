#!/usr/bin/env python3
"""
Final comprehensive fix for all remaining Godot 4 issues
"""

import os
import re

def final_godot4_fixes(content):
    """Apply comprehensive Godot 4 fixes"""
    fixed = content
    
    # Fix double conversions and weird cast patterns left by previous fixes
    fixed = re.sub(r'\(Viewport\.Msaa\)\(Viewport\.Msaa\)', '(Viewport.Msaa)', fixed)
    
    # Fix ScreenSpaceAA enum usage
    fixed = re.sub(r'GetViewport\(\)\.ScreenSpaceAA\s*=\s*([a-zA-Z_][a-zA-Z0-9_]*);', r'GetViewport().ScreenSpaceAA = (\1 ? Viewport.ScreenSpaceAAEnum.Enabled : Viewport.ScreenSpaceAAEnum.Disabled);', fixed)
    
    # Fix MsaaMode calls in ViewPort
    fixed = re.sub(r'GetViewport\(\)\.Msaa3D\s*=', 'GetViewport().Msaa3D =', fixed)
    
    # Fix MSAA saving to config 
    fixed = re.sub(r'preferencesFile\.SetValue\("user_prefs", "msaa", Msaa\);', 'preferencesFile.SetValue("user_prefs", "msaa", (int)Msaa);', fixed)
    fixed = re.sub(r'Msaa = \(Viewport\.Msaa\)preferencesFile\.GetValue\("user_prefs", "msaa"\);', 'Msaa = (Viewport.Msaa)(int)preferencesFile.GetValue("user_prefs", "msaa");', fixed)
    
    # Fix TweenCallback parameters that should be lambdas
    fixed = re.sub(r'TweenCallback\(Callable\.From\(([^)]+)\)\)', r'TweenCallback(Callable.From(() => \1()))', fixed)
    
    # Fix Rect2.GetViewport calls - should be called on Control
    fixed = re.sub(r'GetViewportRect\(\)\.GetViewport\(\)', 'GetViewport()', fixed)
    
    # Fix button Connect calls with wrong parameters
    fixed = re.sub(r'\.Connect\("pressed", ([^,]+), "([^"]+)"\)', r'.Pressed += \1.\2', fixed)
    
    # Fix Tween.Connect calls
    fixed = re.sub(r'tween\.Connect\("([^"]+)", ([^,]+), "([^"]+)"\)', r'tween.\1 += \2.\3', fixed)
    
    # Fix Timer connect calls
    fixed = re.sub(r'\.Connect\("timeout", new Callable\(this, ([^)]+)\)\)', r'.Timeout += \1', fixed)
    
    return fixed

def process_file(filepath):
    """Process a single file"""
    try:
        with open(filepath, 'r') as f:
            original = f.read()
        
        fixed = final_godot4_fixes(original)
        
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
