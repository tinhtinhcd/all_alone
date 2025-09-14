#!/usr/bin/env python3
import os
import re

def fix_critical_remaining_issues(file_path):
    """Fix the most critical remaining Godot 3->4 API issues"""
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()
        
        # Fix GD.RandfRange -> GD.Randf() with range calculation
        content = re.sub(r'GD\.RandfRange\s*\(\s*([^,]+)\s*,\s*([^)]+)\s*\)', r'GD.Randf() * (\2 - \1) + \1', content)
        
        # Fix AudioServer methods - they should exist in Godot 4
        content = re.sub(r'AudioServer\.LinearToDb', 'AudioServer.LinearToDb', content)
        content = re.sub(r'AudioServer\.DbToLinear', 'AudioServer.DbToLinear', content)
        
        # Fix event connection issues - revert bad automatic changes
        content = re.sub(r'(\w+)\.Pressed \+= \(\) => (true|false);', r'\1.Pressed += \2;', content)
        
        # Fix TweenCallback calls - Godot 4 uses different syntax
        content = re.sub(r'TweenCallback\s*\(\s*([^,)]+)\s*,\s*([^)]+)\s*\)', r'TweenCallback(Callable.From(\2))', content)
        content = re.sub(r'TweenCallback\s*\(\s*([^)]+)\s*\)', r'TweenCallback(Callable.From(\1))', content)
        
        # Fix Connect/Disconnect calls - Godot 4 uses Callable syntax
        content = re.sub(r'Connect\s*\(\s*([^,]+)\s*,\s*new Callable\(([^,]+),\s*([^)]+)\)\s*,\s*([^)]+)\s*\)', r'Connect(\1, Callable.From(\2.\3))', content)
        content = re.sub(r'Disconnect\s*\(\s*([^,]+)\s*,\s*([^,]+)\s*,\s*([^)]+)\s*\)', r'Disconnect(\1, Callable.From(\2.\3))', content)
        
        # Fix UserInput property setter issue
        content = re.sub(r'UserInput\s*=\s*([^;]+);', r'UserInput = \1;', content)
        
        # Fix ResourcePath context issues
        content = re.sub(r'(\w+)\.ResourcePath\b', r'\1.SceneFilePath', content)
        
        # Fix Viewport property access
        content = re.sub(r'\.ScreenSpaceAa\b', '.ScreenSpaceAa', content)
        
        # Fix MSAA enum usage
        content = re.sub(r'Viewport\.Msaa\.([A-Za-z0-9_]+)', r'Viewport.Msaa.\1', content)
        
        # Fix double/float conversion issues
        content = re.sub(r'(\w+Time)\s*\+=\s*(delta)\b', r'\1 += (float)\2', content)
        content = re.sub(r'WaitTime\b(?=\s*[^=])', r'(float)WaitTime', content)
        
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
                fix_critical_remaining_issues(file_path)
                print(f"Fixed: {file_path}")

if __name__ == "__main__":
    main()
