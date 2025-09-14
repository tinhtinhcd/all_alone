#!/usr/bin/env python3
"""
Fix remaining Callable.nameof issues and other critical bugs
"""

import os
import re

def fix_callable_issues(content):
    """Fix Callable constructor and nameof issues"""
    fixed = content
    
    # Fix Callable with nameof issues
    # Pattern: new Callable(this, this.nameof(MethodName)) -> new Callable(this, nameof(MethodName))
    fixed = re.sub(r'new Callable\(this, this\.nameof\(([^)]+)\)\)', r'new Callable(this, nameof(\1))', fixed)
    
    # Fix assignments that look like casting issues
    fixed = re.sub(r'\(float\)\s*([^.=]+)\.([a-zA-Z]+)\s*=', r'\1.\2 =', fixed)
    
    # Fix Vector3.Origin -> Vector3.Zero
    fixed = re.sub(r'Vector3\.Origin\b', 'Vector3.Zero', fixed)
    
    return fixed

def process_file(filepath):
    """Process a single file"""
    try:
        with open(filepath, 'r') as f:
            original = f.read()
        
        fixed = fix_callable_issues(original)
        
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
