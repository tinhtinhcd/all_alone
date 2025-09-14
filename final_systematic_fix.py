#!/usr/bin/env python3
"""
Final systematic fix for all remaining compilation errors
"""

import os
import re

def fix_remaining_errors(content):
    """Fix all remaining error patterns"""
    fixed = content
    
    # Ensure using System; is present for nameof support
    if 'using System;' not in fixed and 'nameof(' in fixed:
        fixed = re.sub(r'(using Godot;)', r'\1\nusing System;', fixed, count=1)
    
    # Fix PackedScene.SceneFilePath -> ResourcePath in Godot 4
    fixed = re.sub(r'\.SceneFilePath', '.ResourcePath', fixed)
    
    # Fix malformed Callable constructors
    # Pattern: new Callable(nameof(Method)) -> new Callable(this, nameof(Method))
    fixed = re.sub(r'new Callable\(nameof\(([^)]+)\)\)', r'new Callable(this, nameof(\1))', fixed)
    
    # Fix Callable.From with nested Callable
    fixed = re.sub(r'Callable\.From\(new Callable\(nameof\(([^)]+)\)\)\)', r'new Callable(this, nameof(\1))', fixed)
    
    # Fix Transform3D to Vector3 assignments
    def fix_transform(match):
        return 'Vector3 ' + match.group(1) + ' = ' + match.group(2) + '.Origin;'
    
    fixed = re.sub(r'Vector3 ([a-zA-Z_][a-zA-Z0-9_]*) = ([^;]*Transform3D[^;]*);', 
                  fix_transform, fixed)
    
    return fixed

def add_using_system(filepath):
    """Add using System; to files that need it"""
    try:
        with open(filepath, 'r') as f:
            content = f.read()
        
        # Check if file uses nameof but doesn't have using System
        if 'nameof(' in content and 'using System;' not in content:
            # Add using System after using Godot
            content = re.sub(r'(using Godot;)', r'\1\nusing System;', content, count=1)
            
            with open(filepath, 'w') as f:
                f.write(content)
            return True
        return False
    except Exception as e:
        print(f"Error processing {filepath}: {e}")
        return False

def process_file(filepath):
    """Process a single file"""
    try:
        with open(filepath, 'r') as f:
            original = f.read()
        
        fixed = fix_remaining_errors(original)
        
        if fixed != original:
            with open(filepath, 'w') as f:
                f.write(fixed)
            print(f"Fixed: {filepath}")
            return True
        return False
            
    except Exception as e:
        print(f"Error processing {filepath}: {e}")
        return False

def main():
    """Main execution"""
    src_dir = "src"
    if not os.path.exists(src_dir):
        print("src directory not found!")
        return
    
    fixed_count = 0
    using_added = 0
    total_files = 0
    
    # Process all C# files
    for root, dirs, files in os.walk(src_dir):
        for file in files:
            if file.endswith('.cs'):
                filepath = os.path.join(root, file)
                total_files += 1
                
                # First add using System if needed
                if add_using_system(filepath):
                    using_added += 1
                
                # Then apply other fixes
                if process_file(filepath):
                    fixed_count += 1
    
    print(f"\nProcessed {total_files} C# files")
    print(f"Added using System to {using_added} files")  
    print(f"Applied fixes to {fixed_count} files")

if __name__ == "__main__":
    main()
