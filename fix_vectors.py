#!/usr/bin/env python3
import os
import re

def fix_vector_properties(file_path):
    """Fix vector properties from lowercase to uppercase (.x -> .X, .y -> .Y, .z -> .Z)"""
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()
        
        # Fix vector properties - use word boundaries to avoid replacing in other contexts
        content = re.sub(r'\.x\b', '.X', content)
        content = re.sub(r'\.y\b', '.Y', content)
        content = re.sub(r'\.z\b', '.Z', content)
        
        # Fix common Godot 3->4 API changes
        content = re.sub(r'GlobalTranslation\b', 'GlobalPosition', content)
        content = re.sub(r'FindNode\b', 'GetNode', content)
        content = re.sub(r'\.Instance\(\)', '.Instantiate()', content)
        content = re.sub(r'GD\.RandfRange\b', 'GD.RandfRange', content)
        content = re.sub(r'GD\.LinearToDb\b', 'AudioServer.LinearToDb', content)
        content = re.sub(r'GD\.DbToLinear\b', 'AudioServer.DbToLinear', content)
        content = re.sub(r'OS\.GetTicksMsec\b', 'Time.GetTicksMsec', content)
        
        with open(file_path, 'w', encoding='utf-8') as f:
            f.write(content)
        
        return True
    except Exception as e:
        print(f"Error processing {file_path}: {e}")
        return False

def main():
    src_dir = "/Users/lyvantinh/Documents/project/all_alone/src"
    
    for root, dirs, files in os.walk(src_dir):
        for file in files:
            if file.endswith('.cs'):
                file_path = os.path.join(root, file)
                fix_vector_properties(file_path)
                print(f"Fixed: {file_path}")

if __name__ == "__main__":
    main()
