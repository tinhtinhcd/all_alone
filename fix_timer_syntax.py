#!/usr/bin/env python3
"""
Fix Timer property access syntax corruption from previous automation.
Repairs malformed patterns like "_timer.(float)WaitTime" back to "(float)_timer.WaitTime"
"""

import os
import re

def fix_timer_syntax(content):
    """Fix malformed Timer property cast expressions"""
    # Pattern to match malformed timer property access
    # Example: "_cooldownTimer.(float)WaitTime" -> "(float)_cooldownTimer.WaitTime"
    pattern = r'(\w+Timer)\.\((float|double|int)\)(\w+)'
    replacement = r'(\2)\1.\3'
    
    fixed_content = re.sub(pattern, replacement, content)
    
    # Additional pattern for other malformed property access
    # Example: "timer.(float)Property" -> "(float)timer.Property"
    pattern2 = r'(\w+)\.\((float|double|int)\)(\w+)'
    replacement2 = r'(\2)\1.\3'
    
    # Only apply if it looks like a timer-related property
    if 'Timer' in content or 'WaitTime' in content or 'TimeLeft' in content:
        fixed_content = re.sub(pattern2, replacement2, fixed_content)
    
    return fixed_content

def process_file(filepath):
    """Process a single file"""
    try:
        with open(filepath, 'r') as f:
            original = f.read()
        
        fixed = fix_timer_syntax(original)
        
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
    print("Fixed syntax in " + str(fixed_count) + " files")

if __name__ == "__main__":
    main()
