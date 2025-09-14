#!/usr/bin/env python3
import os
import re

def fix_remaining_issues(file_path):
    """Fix remaining Godot 3->4 API issues"""
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            content = f.read()
        
        # Fix remaining .Instance calls that weren't caught
        content = re.sub(r'\.Instance\b', '.Instantiate', content)
        
        # Fix GD functions that moved
        content = re.sub(r'GD\.RandfRange\b', 'GD.RandfRange', content)
        
        # Fix event connection syntax - Godot 4 doesn't support .Pressed for assignments
        content = re.sub(r'\.Pressed\s*=\s*([^;]+);', r'.Pressed += \1;', content)
        
        # Fix MSAA enum
        content = re.sub(r'SubViewport\.MSAA', 'Viewport.Msaa', content)
        
        # Fix TextureRect properties
        content = re.sub(r'\.Expand\b', '.ExpandMode', content)
        content = re.sub(r'\.Texture2D\b', '.Texture', content)
        
        # Fix AnimatedSprite3D.Frames -> SpriteFrames
        content = re.sub(r'\.Frames\.', '.SpriteFrames.', content)
        
        # Fix Tween callback syntax
        content = re.sub(r'TweenCallback\s*\(\s*([^,]+)\s*,\s*([^)]+)\s*\)', r'TweenCallback(\1)', content)
        
        # Fix double to float casts
        content = re.sub(r'\b(delta)\b(?=\s*[*/+\-])', r'(float)\1', content)
        
        # Fix Particles -> GpuParticles3D
        content = re.sub(r'\bParticles\b(?!\w)', 'GpuParticles3D', content)
        
        # Fix File -> FileAccess
        content = re.sub(r'\bFile\b(?!\w)', 'FileAccess', content)
        
        # Fix Engine.EditorHint -> Engine.IsEditorHint()
        content = re.sub(r'Engine\.EditorHint\b', 'Engine.IsEditorHint()', content)
        
        # Fix AABB
        content = re.sub(r'\bAABB\b', 'Aabb', content)
        
        # Fix Viewport properties
        content = re.sub(r'\.Size\b', '.GetViewport().GetVisibleRect().Size', content)
        content = re.sub(r'\.Fxaa\b', '.ScreenSpaceAa', content)
        
        # Fix FastNoiseLite method name
        content = re.sub(r'\.GetNoise1d\b', '.GetNoise1D', content)
        
        # Fix OS methods
        content = re.sub(r'OS\.GetTicksMsec\b', 'Time.GetTicksMsec', content)
        
        # Fix Transform3D properties
        content = re.sub(r'\.origin\b', '.Origin', content)
        content = re.sub(r'\.basis\b', '.Basis', content)
        
        # Fix MoveAndSlide syntax - no longer takes velocity parameter
        content = re.sub(r'MoveAndSlide\s*\([^)]*\)', 'MoveAndSlide()', content)
        content = re.sub(r'MoveAndSlideWithSnap\s*\([^)]*\)', 'MoveAndSlide()', content)
        
        # Fix AnimationTree root motion
        content = re.sub(r'\.GetRootMotionTransform\(\)', '.GetRootMotionPosition()', content)
        
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
                fix_remaining_issues(file_path)
                print(f"Fixed: {file_path}")

if __name__ == "__main__":
    main()
