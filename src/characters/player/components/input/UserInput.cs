using Godot;
using System;
using GameGeneral;

namespace ZombieHoardGame.PlayerCharacter
{
    public partial class UserInput : Node
    {
        [Signal]
        public delegate void MouseMotionInputEventEventHandler(Vector2 rotation);
        [Signal]
        public delegate void ReloadEventHandler();
        [Signal]
        public delegate void ShootEventHandler();
        [Signal]
        public delegate void SwitchGunEventHandler();
        [Signal]
        public delegate void InteractEventHandler();

        public Vector3 MoveDirectionXZ{
            private set;
            get;
        }

        private UserPreferences _userPreferences;

        public bool IsSprintPressed{ private set; get; }
        public bool IsJumpPressed{ private set; get; }
        public bool IsShootPressed{ private set; get; }
        public bool IsInteractPressed{ private set; get; }

        public override void _Ready()
        {
            
            _userPreferences = GetNode<UserPreferences>("/root/UserPreferences");
        }

        public override void _PhysicsProcess(double delta)
        {
            ReadMoveInputDirection();
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventMouseMotion)
            {
                var mouseMotionEvent = (InputEventMouseMotion)@event;
                EmitSignal(nameof(MouseMotionInputEvent), mouseMotionEvent.Relative * _userPreferences.MouseSensitivity * 0.0005f);
            }
            else if (@event.IsActionPressed("player_sprint"))
            {
                IsSprintPressed = true;
            }
            else if (@event.IsActionReleased("player_sprint"))
            {
                IsSprintPressed = false;
            }
            else if (@event.IsActionPressed("player_jump"))
            {
                IsJumpPressed = true;
            }
            else if (@event.IsActionReleased("player_jump"))
            {
                IsJumpPressed = false;
            }
            else if (@event.IsActionPressed("player_reload"))
            {
                EmitSignal(nameof(Reload));
            }
            else if (@event.IsActionPressed("player_shoot"))
            {
                IsShootPressed = true;
                EmitSignal(nameof(Shoot));
            }
            else if (@event.IsActionReleased("player_shoot"))
            {
                IsShootPressed = false;
            }
            else if (@event.IsActionPressed("player_switch_gun"))
            {
                EmitSignal(nameof(SwitchGun));
            }
            else if (@event.IsActionPressed("player_interact"))
            {
                IsInteractPressed = true;
                EmitSignal(nameof(Interact));
            }
            else if (@event.IsActionReleased("player_interact"))
            {
                IsInteractPressed = false;
            }
        }


        private void ReadMoveInputDirection()
        {
            Vector3 inputStrength = new Vector3();
            inputStrength.Z = Input.GetActionRawStrength("player_backward") - Input.GetActionRawStrength("player_forward");
            inputStrength.X = Input.GetActionRawStrength("player_right") - Input.GetActionRawStrength("player_left");
            MoveDirectionXZ = inputStrength.Normalized();
        }
    }
}

