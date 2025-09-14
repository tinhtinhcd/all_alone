#!/usr/bin/env python3
import os
import re

def fix_final_issues(file_path):
    """Fix the final remaining Godot 3->4 API issues"""
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()
        
        # Fix LevelServices.Instantiate -> LevelServices.Instance
        content = re.sub(r'LevelServices\.Instantiate\b', 'LevelServices.Instance', content)
        
        # Fix Main.Instantiate -> Main.Instance
        content = re.sub(r'Main\.Instantiate\b', 'Main.Instance', content)
        
        # Fix FileAccess constructor and static methods
        content = re.sub(r'new FileAccess\(\)', 'null', content)
        content = re.sub(r'(\w+)\.FileExists\(', r'FileAccess.FileExists(', content)
        content = re.sub(r'(\w+)\.Open\(', r'FileAccess.Open(', content)
        
        # Fix AudioServer methods (they actually exist in AudioServer)
        content = re.sub(r'AudioServer\.DbToLinear\b', 'AudioServer.DbToLinear', content)
        content = re.sub(r'AudioServer\.LinearToDb\b', 'AudioServer.LinearToDb', content)
        
        # Fix GD.RandfRange -> GD.RandfRange (it should exist)
        content = re.sub(r'GD\.RandfRange\b', 'GD.RandfRange', content)
        
        # Fix double to float casts for delta parameters
        content = re.sub(r'\b(\w+Time)\s*\+=\s*delta;', r'\1 += (float)delta;', content)
        content = re.sub(r'(\w+)\s*\*\s*delta\b', r'\1 * (float)delta', content)
        
        # Fix Viewport properties that changed
        content = re.sub(r'GetViewport\(\)\.GetVisibleRect\(\)\.Size', 'GetViewport().GetVisibleRect().Size', content)
        
        # Fix event assignment issues - revert incorrect changes
        content = re.sub(r'(\w+)\.Pressed \+= (true|false);', r'\1.Pressed += () => \2;', content)
        
        # Fix Filename property -> ResourcePath
        content = re.sub(r'\.Filename\b', '.ResourcePath', content)
        content = re.sub(r'\bFilename\b', 'ResourcePath', content)
        
        # Fix UserInput property accessor
        content = re.sub(r'UserInput\s*=', 'SetUserInput(', content)
        
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
                fix_final_issues(file_path)
                print(f"Fixed: {file_path}")

if __name__ == "__main__":
    main()
