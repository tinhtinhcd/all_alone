#!/usr/bin/env python3
"""
Final surgical fixes for zero errors
"""

import os
import re

def surgical_fixes(content):
    """Final targeted fixes for remaining errors"""
    fixed = content
    
    # Fix incorrect event conversions back to proper Connect calls
    # Pattern: button.Pressed += method -> button.Connect("pressed", new Callable(this, nameof(method)))
    fixed = re.sub(r'(\w+)\.Pressed\s*\+=\s*(\w+)', r'\1.Connect("pressed", new Callable(this, nameof(\2)))', fixed)
    
    # Fix Timer.Timeout events 
    fixed = re.sub(r'(\w+)\.Timeout\s*\+=\s*(\w+)', r'\1.Connect("timeout", new Callable(this, nameof(\2)))', fixed)
    
    # Fix remaining MsaaEnum -> Msaa
    fixed = re.sub(r'Viewport\.MsaaEnum', 'Viewport.Msaa', fixed)
    
    # Fix ScreenSpaceAA property access
    fixed = re.sub(r'UserPreferences\.ScreenSpaceAA', 'UserPreferences.ScreenSpaceAa', fixed)
    
    # Fix string->Action conversion issues (usually from malformed lambdas)
    # Look for patterns like Connect calls that got mangled
    fixed = re.sub(r'\.Timeout\s*\+=\s*"([^"]+)"', r'.Connect("timeout", new Callable(this, nameof(\1)))', fixed)
    fixed = re.sub(r'\.Pressed\s*\+=\s*"([^"]+)"', r'.Connect("pressed", new Callable(this, nameof(\1)))', fixed)
    
    # Fix TweenCallback issues
    fixed = re.sub(r'TweenCallback\(([^)]+)\)', r'TweenCallback(Callable.From(\1))', fixed)
    
    return fixed

def process_file(filepath):
    """Process a single file"""
    try:
        with open(filepath, 'r') as f:
            original = f.read()
        
        fixed = surgical_fixes(original)
        
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
