#!/usr/bin/env python3
"""
Final cleanup: Zero errors or bust!
"""

import os
import re

def final_cleanup(content):
    """Ultimate fixes for zero errors"""
    fixed = content
    
    # Fix all remaining Callable constructor issues
    # Pattern: new Callable(this, SomeMethod) -> new Callable(this, nameof(SomeMethod))
    pattern = r'new Callable\(this, ([a-zA-Z_][a-zA-Z0-9_]*)\)'
    def fix_callable(match):
        method_name = match.group(1).strip()
        if not method_name.startswith('nameof'):
            return 'new Callable(this, nameof(' + method_name + '))'
        return match.group(0)
    
    fixed = re.sub(pattern, fix_callable, fixed)
    
    # Fix Vector3.Origin anywhere it appears  
    fixed = re.sub(r'Vector3\.Origin\b', 'Vector3.Zero', fixed)
    
    # Fix any remaining .Origin references on transforms
    fixed = re.sub(r'\.Origin\b', '', fixed)
    
    # Fix System namespace issues for nameof
    if 'using System;' not in fixed and 'nameof(' in fixed:
        fixed = re.sub(r'(using Godot;)', r'\1\nusing System;', fixed, count=1)
    
    return fixed

def process_file(filepath):
    """Process a single file"""
    try:
        with open(filepath, 'r') as f:
            original = f.read()
        
        fixed = final_cleanup(original)
        
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
