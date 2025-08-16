using Godot;
using System;
using System.Collections;
using System.Collections.Generic;


namespace UI.MainMenu
{
    public partial class LevelSelect : MultiPage.Page
    {
        /// Signals ///

        /// Enums ///

        /// Constants ///
        private const String LevelDirectoryPath = "res://src/levels/";

        /// Exported Fields ///

        /// Properties - public, protected, private ///

        /// Fields - protected or private ///


        //////////////////////////////
        //       Constructors       //
        //////////////////////////////
        public LevelSelect()
        {
            return;
        }

        //////////////////////////////
        // Engine Callback Methods  //
        //////////////////////////////
        public override void _Ready()
        {
            return;
        }

        public override void _Process(double delta)
        {
            return;
        }

        public override void _PhysicsProcess(double delta)
        {
            return;
        }
        
        //////////////////////////////
        //      Public Methods      //
        //////////////////////////////


        //////////////////////////////
        //     Protected Methods    //
        //////////////////////////////


        //////////////////////////////
        // Signal Connected Methods //
        //////////////////////////////


        //////////////////////////////
        //      Private Methods     //
        //////////////////////////////
    }
}

