#!/usr/bin/env python3
"""
Zero-error push: Fix all remaining systematic issues
"""

import os
import re

def zero_error_fixes(content):
    """Fix all remaining critical issues for zero-error build"""
    fixed = content
    
    # Fix remaining nameof issues that automation missed
    # Pattern: something.nameof(Method) -> nameof(Method) 
    fixed = re.sub(r'([a-zA-Z_][a-zA-Z0-9_]*)\s*\.\s*nameof\s*\(\s*([^)]+)\s*\)', r'nameof(\2)', fixed)
    
    # Fix mismatched Callable constructors
    fixed = re.sub(r'new Callable\(this, nameof\(([^)]+)\)\)', r'new Callable(this, nameof(\1))', fixed)
    
    # Fix Gun.ResourcePath -> should be Gun.SceneFilePath in Godot 4
    fixed = re.sub(r'([a-zA-Z_][a-zA-Z0-9_]*Gun[a-zA-Z0-9_]*)\s*\.\s*ResourcePath', r'\1.SceneFilePath', fixed)
    
    # Fix Stopwatch delta conversion  
    fixed = re.sub(r'delta;', '(float)delta;', fixed)
    
    # Fix event connection syntax that got mangled
    # Pattern: something.Pressed += this.method -> something.Pressed += method
    fixed = re.sub(r'(\w+)\.Pressed\s*\+=\s*this\.(\w+)', r'\1.Pressed += \2', fixed)
    
    # Fix TweenCallback that got mangled
    fixed = re.sub(r'TweenCallback\(Callable\.From\(\(\) => ([^(]+)\(\)\)\)', r'TweenCallback(Callable.From(\1))', fixed)
    
    return fixed

def process_file(filepath):
    """Process a single file"""
    try:
        with open(filepath, 'r') as f:
            original = f.read()
        
        fixed = zero_error_fixes(original)
        
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
