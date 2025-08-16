using Godot;
using System;
using System.Collections;
using System.Collections.Generic;


namespace GameGeneral
{
    [Tool]
    public partial class NavMeshInstTool : NavigationRegion3D
    {
        [Export]
        private bool UpdateSwitch
        {
            set{
                UpdateBake();
            }
            get{ return true; }
        }

        public override void _Ready()
        {
            if (!Engine.IsEditorHint())
            {
                GetNode<MeshInstance3D>("NavArea").QueueFree();
            }
        }

        private void UpdateBake()
        {
            if (Engine.IsEditorHint())
            {
                MeshInstance3D boundingBox = GetNode<MeshInstance3D>("NavArea");
                Aabb filterAabb = new Aabb();
                filterAabb = boundingBox.GetAabb();
                // Vector3 overflow = new Vector3(Navmesh.AgentRadius, 0, Navmesh.AgentRadius) * 2;
                // filterAabb.Position -= overflow;
                // filterAabb.End += overflow;
                NavigationMesh.FilterBakingAabb = filterAabb;
                GD.Print("Baking");
                BakeNavigationMesh();
            }
        }
    }
}

