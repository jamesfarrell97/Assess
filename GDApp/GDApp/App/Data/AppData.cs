/*
Function: 		Stores common hard-coded variable values used within the game e.g. key mappings, mouse sensitivity
Author: 		NMCG
Version:		1.0
Date Updated:	5/10/17
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    public sealed class LerpSpeed
    {
        private static readonly float SpeedMultiplier = 2;

        public static readonly float VerySlow = 0.008f; 
        public static readonly float Slow = SpeedMultiplier * VerySlow;
        public static readonly float Medium = SpeedMultiplier * Slow;
        public static readonly float Fast = SpeedMultiplier * Medium;
        public static readonly float VeryFast = SpeedMultiplier * Fast;
    }

    public sealed class AppData
    {
        #region Common
        public static readonly string GameTitle = "Assess";
        #endregion

        #region Camera
        public static float OrbitSpeed = 0.1f;
        public static float OrbitAngle = 90;
        #endregion
        
        #region Blocks
        public static float CubeDimension = 1;
        public static float DiamondDimension = 0.75f;
        public static float StandardObjectOpacity = 1;
        #endregion
        
        #region Keys
        public static readonly Keys ResetKey = Keys.R;
        public static readonly Keys MenuShowHideKey = Keys.Escape;

        public static readonly Keys[] CameraControlKeys = { Keys.A, Keys.D, Keys.W, Keys.S, Keys.E, Keys.Q, Keys.O, Keys.P };
        public static readonly Keys[] PlayerMoveKeys = { Keys.Z, Keys.X };
        #endregion

        #region Map
        //Determines the amount of bits assigned to each component in the bit-shifted 3D array of map values
        public static int ReservedBreakableBlockBits = 3;      //2^3 (8) Breakable Block Types
        public static int ReservedUnbreakableBlockBits = 3;    //2^3 (8) Un-Breakable Block Types
        public static int ReservedGoalBits = 3;                //2^3 (8) Goal Types
        public static int ReservedObjectiveBits = 3;           //2^3 (8) Objective Types
        public static int ReservedPlayerBits = 3;              //2^3 (8) Players

        //Determines the position of each component in the level data text file
        public static readonly int BreakableBlocksStartPosition = 1;
        public static readonly int UnbreakableBlocksStartPosition = 2;
        public static readonly int GoalsStartPosition = 3;
        public static readonly int ObjectivesStartPosition = 4;
        public static readonly int PlayersStartPosition = 5;

        //Determines the size of a block
        public static float BlockWidth = CubeDimension;
        public static float BlockHeight = CubeDimension;
        public static float BlockDepth = CubeDimension;

        //Determines the size of the world
        public static readonly int WorldScale = 1250;
        #endregion

        #region UI
        #endregion

        #region Menu
        public static readonly string MenuMainID = "Main Menu";
        public static readonly string MenuOptionsID = "Options Menu";
        public static readonly string MenuControlsID = "Controls Menu";
        public static readonly string MenuAudioID = "Audio Menu";
        public static readonly string MenuPauseID = "Pause Menu";
        public static readonly string ScreenLoseID = "Lose Screen";
        public static readonly string ScreenWinID = "Win Screen";
        #endregion

        #region Messages
        public static readonly string[] TextboxMessages = new string[] {
            "Collect all gems to finish the level",
            "Complete each level by reaching the purple orb",
            "Press 'R' to reset",
            "Press 'Q' or 'E' to rotate",
            "Press 'W', 'A', 'S', or 'D' to orbit",
            "Click on white blocks to break them"
        };

        public static int TimePerMessage = 8;
        #endregion

        #region Archetype ID's
        public static readonly string UnlitTexturedQuadArchetypeID = "Unlit Textured Quad Archetype";
        public static readonly string UnlitTexturedSphereArchetypeID = "Unlit Textured Sphere Archetype";
        public static readonly string UnlitTexturedCubeArchetypeID = "Unlit Textured Cube Archetype";

        public static readonly string LitTexturedCubeArchetypeID = "Lit Textured Cube Archetype";
        public static readonly string LitTexturedDiamondArchetypeID = "Lit Textured Diamond Archetype";
        #endregion

        #region Vertex Data ID's
        public static readonly string WireframeOriginHelperVertexDataID = "Wire Origin Vertex Data";
        public static readonly string WireframeTriangleVertexDataID = "Wire Triangle Vertex Data";
        public static readonly string WireframeCircleVertexDataID = "Wire Circle Wireframe";
        public static readonly string UnlitTexturedQuadVertexDataID = "Unlit Textured Quad Vertex Data";
        public static readonly string UnlitTexturedBillboardVertexDataID = "Unlit Textured Billboard Vertex Data";
        public static readonly string UnlitTexturedSphereVertexDataID = "Unlit Textured Sphere Vertex Data";
        public static readonly string UnlitTexturedCubeVertexDataID = "Unlit Textured Cube Vertex Data";

        public static readonly string LitTexturedCubeVertexDataID = "Lit Textured Cube Vertex Data";
        public static readonly string LitTexturedDiamondVertexDataID = "Lit Textured Diamond Vertex Data";

        public static readonly string LitTexturedCubeIndexedVertexDataID = "Lit Textured Cube Indexed Vertex Data";
        public static readonly string LitTexturedDiamondIndexedVertexDataID = "Lit Textured Diamond Indexed Vertex Data";
        #endregion

        #region Effect Parameter ID's
        public static readonly string UnlitWireframeEffectID = "wireframe unlit effect";
        public static readonly string UnlitTexturedEffectID = "textured unlit effect";
        public static readonly string LitTexturedEffectID = "textured lit effect";
        public static readonly string UnlitBillboardsEffectID = "unlit billboards effect";
        public static readonly string UnlitColorCubeVertexDataID = "unlit color cube vertex data";
        public static readonly string LitColorCubeVertexDataID = "Lit Color Cube Vertex Data ID";
        public static readonly string LitColorEffectID = "lit color cube effect";
        #endregion

        #region Controller ID's
        public static readonly string PlayerControllerID = "Player Controller";
        public static readonly string TrackCameraControllerID = "Track Camera Controller";
        public static readonly string OrbitalCameraControllerID = "Orbital Camera Controller";
        #endregion

        #region Camera ID's
        public static readonly string TrackCameraID = "Track";
        public static readonly string OrbitCameraID = "Orbit";

        public static string UITextBoxID { get; internal set; }
        #endregion
    }
}