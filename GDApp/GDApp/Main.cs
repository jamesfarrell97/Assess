using GDLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System;

namespace GDApp
{
    public class Main : Game
    {
        #region Fields
        SpriteBatch spriteBatch;
        GraphicsDeviceManager graphics;

        private Integer2 resolution;
        private Integer2 screenCentre;

        private ContentDictionary<Texture2D> textureDictionary;
        private ContentDictionary<SpriteFont> fontDictionary;
        private Dictionary<string, Track3D> trackDictionary;
        private Dictionary<string, IVertexData> vertexDictionary;
        private Dictionary<string, RailParameters> railDictionary;
        private Dictionary<string, EffectParameters> effectDictionary;
        private Dictionary<string, DrawnActor3D> objectArchetypeDictionary;

        private EventDispatcher eventDispatcher;

        private MouseManager mouseManager;
        private KeyboardManager keyboardManager;
        private InputManagerParameters inputManagerParameters;

        private ObjectManager objectManager;
        private CameraManager cameraManager;
        private PickingManager pickingManager;
        private SoundManager soundManager;
        private TextboxManager textboxManager;
        private MyMenuManager menuManager;
        private UIManager uiManager;

        private CameraLayoutType cameraLayoutType;
        private ScreenLayoutType screenLayoutType;

        private string[] levels;
        private int[,,] levelMap;
        private Vector3 trackCameraUp;
        private Vector3 trackCameraLook;
        private Vector3 cameraPosition;
        private Vector3 cameraOrbitPoint;
        private Vector3 trackCameraPosition;
        private Vector3 orbitalCameraPosition;
        private OrbitalCamera orbitalCamera;
        private OrbitalCamera trackCamera;

        private CollidablePrimitiveObject playerObject;
        private readonly short[] levelTrackTime = new short[20];

        private int breakableBlocksStartPosition;
        private int unbreakableBlockStartPosition;
        private int goalsStartPosition;
        private int objectivesStartPosition;
        private int playerStartPosition;

        private int reservedBreakableBlockBits;
        private int reservedUnbreakableBlockBits;
        private int reservedGoalBits;
        private int reservedObjectiveBits;
        private int reservedPlayerBits;

        private int breakableBlocksShiftPosition;
        private int unbreakableBlocksShiftPosition;
        private int goalsShiftPosition;
        private int objectivesShiftPosition;
        private int playerShiftPosition;

        private int xDimension = 0;
        private int yDimension = 0;
        private int zDimension = 0;
        #endregion

        #region Constructors
        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        #endregion

        #region Initialization
        protected override void Initialize()
        {
            Window.Title = AppData.GameTitle;

            InitializeDictionaries();

            InitializeScreen();
            InitializeGraphics();
            InitializeEffects();

            InitializeEventDispatcher();
            InitializeManagers();

            LoadAssets();

            InitializeMap();
            InitializeSkyBox();

            InitializeCameras();

            InitializeMenu();
            InitializeHUD();
            InitializeMouse();
            InitializeTextbox();

            StartGame();

            base.Initialize();
        }

        private void InitializeScreen()
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            this.resolution = ScreenUtility.XVGA;
            this.screenCentre = this.resolution / 2;

            this.cameraLayoutType = CameraLayoutType.Single;
            this.screenLayoutType = ScreenLayoutType.Orbit;
        }

        private void InitializeGraphics()
        {
            this.graphics.PreferredBackBufferWidth = resolution.X;
            this.graphics.PreferredBackBufferHeight = resolution.Y;

            //Solves the skybox border problem
            SamplerState samplerState = new SamplerState {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp
            };

            this.graphics.GraphicsDevice.SamplerStates[0] = samplerState;

            //Enable alpha transparency - see ColorParameters
            this.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            this.graphics.ApplyChanges();
        }

        private void InitializeEffects()
        {
            BasicEffect basicEffect = null;
            EffectParameters effectParameters = null;

            //Unlit Wireframe
            basicEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                VertexColorEnabled = true
            };

            effectParameters = new EffectParameters(basicEffect);
            this.effectDictionary.Add(AppData.UnlitWireframeEffectID, effectParameters);

            //Unlit Textured
            basicEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true
            };

            effectParameters = new EffectParameters(basicEffect);
            this.effectDictionary.Add(AppData.UnlitTexturedEffectID, effectParameters);
        
            //Lit Color
            basicEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                TextureEnabled = false,
                VertexColorEnabled = true
            };

            effectParameters = new EffectParameters(basicEffect);
            this.effectDictionary.Add(AppData.LitColorEffectID, effectParameters);

            //Unlit Billboard
            Effect billboardEffect = Content.Load<Effect>("Assets/Effects/Billboard");
            effectParameters = new EffectParameters(billboardEffect);
            this.effectDictionary.Add(AppData.UnlitBillboardsEffectID, effectParameters);

            //Lit Textured
            basicEffect = new BasicEffect(graphics.GraphicsDevice)
            {
                TextureEnabled = true,
                LightingEnabled = true
            };

            basicEffect.AmbientLightColor = Color.AntiqueWhite.ToVector3();
            basicEffect.PreferPerPixelLighting = false;
            basicEffect.EnableDefaultLighting();

            effectParameters = new EffectParameters(basicEffect);
            this.effectDictionary.Add(AppData.LitTexturedEffectID, effectParameters);
        }

        private void InitializeDictionaries()
        {
            this.textureDictionary = new ContentDictionary<Texture2D>("Texture Dictionary", this.Content);
            this.fontDictionary = new ContentDictionary<SpriteFont>("Font Dictionary", this.Content);

            this.railDictionary = new Dictionary<string, RailParameters>();
            this.trackDictionary = new Dictionary<string, Track3D>();

            this.effectDictionary = new Dictionary<string, EffectParameters>();
            this.vertexDictionary = new Dictionary<string, IVertexData>();

            this.objectArchetypeDictionary = new Dictionary<string, DrawnActor3D>();
        }

        private void InitializeEventDispatcher()
        {
            this.eventDispatcher = new EventDispatcher(this, 20);
            Components.Add(this.eventDispatcher);
        }

        private void InitializeManagers()
        {
            #region Keyboard
            this.keyboardManager = new KeyboardManager(this);
            Components.Add(this.keyboardManager);
            #endregion

            #region Mouse
            this.mouseManager = new MouseManager(this, true);
            this.mouseManager.SetPosition(this.screenCentre);
            Components.Add(this.mouseManager);
            #endregion

            #region Input Manager Parameters
            this.inputManagerParameters = new InputManagerParameters(
                this.mouseManager,
                this.keyboardManager
            );
            #endregion

            #region Camera Manager
            this.cameraManager = new CameraManager(
                this,
                5,
                this.eventDispatcher,
                StatusType.Off
            );

            Components.Add(this.cameraManager);
            #endregion

            #region 3D Object Manager
            this.objectManager = new ObjectManager(
                this,
                this.cameraManager,
                this.eventDispatcher,
                StatusType.Off,
                this.cameraLayoutType
            ) {
                DrawOrder = 1
            };

            Components.Add(this.objectManager);
            #endregion

            #region Sound Manager
            this.soundManager = new SoundManager(
                this,
                this.eventDispatcher,
                StatusType.Update,
                "Content/Assets/Audio/",
                "Demo2DSound.xgs",
                "WaveBank.xwb",
                "SoundBank.xsb"
            );

            Components.Add(this.soundManager);
            #endregion

            #region Menu Manager
            this.menuManager = new MyMenuManager(
                this,
                this.inputManagerParameters,
                this.cameraManager,
                this.spriteBatch,
                this.eventDispatcher,
                StatusType.Drawn | StatusType.Update
            ) {
                DrawOrder = 2
            };

            Components.Add(this.menuManager);
            #endregion

            #region UI Manager
            this.uiManager = new UIManager(
                this,
                this.spriteBatch,
                this.eventDispatcher,
                10,
                StatusType.Off
            ) {
                DrawOrder = 3
            };

            Components.Add(this.uiManager);
            #endregion

            #region Textbox Manager
            this.textboxManager = new TextboxManager(
                this,
                this.spriteBatch,
                this.eventDispatcher,
                StatusType.Off,
                ""
            );

            Components.Add(this.textboxManager);
            #endregion

            #region Picking Manager
            this.pickingManager = new PickingManager(
                this,
                this.eventDispatcher,
                StatusType.Update,
                this.cameraManager,
                this.objectManager,
                this.inputManagerParameters,
                PickingBehaviourType.PickAndRemove
            );

            Components.Add(this.pickingManager);
            #endregion
        }

        private void InitializeMap()
        {
            SetupBitArray();

            LoadProgressFromFile();
            LoadAllLevelsFromFile();

            LoadLevelMap();
        }

        private void InitializeSkyBox()
        {
            PrimitiveObject archTexturedPrimitiveObject = null;
            PrimitiveObject cloneTexturedPrimitiveObject = null;

            #region Archetype
            archTexturedPrimitiveObject = this.objectArchetypeDictionary[AppData.UnlitTexturedQuadArchetypeID] as PrimitiveObject;
            #endregion

            #region Skybox
            cloneTexturedPrimitiveObject = archTexturedPrimitiveObject.Clone() as PrimitiveObject;
            cloneTexturedPrimitiveObject.Transform.Scale *= AppData.WorldScale;
            cloneTexturedPrimitiveObject.ID = "Skybox Left";
            cloneTexturedPrimitiveObject.Transform.Translation = new Vector3(AppData.WorldScale / 2.0f, 0, 0);
            cloneTexturedPrimitiveObject.Transform.Rotation = new Vector3(0, -90, 0);
            cloneTexturedPrimitiveObject.EffectParameters.Texture = this.textureDictionary["skybox_left"];
            this.objectManager.Add(cloneTexturedPrimitiveObject);
            
            cloneTexturedPrimitiveObject = archTexturedPrimitiveObject.Clone() as PrimitiveObject;
            cloneTexturedPrimitiveObject.Transform.Scale *= AppData.WorldScale;
            cloneTexturedPrimitiveObject.ID = "Skybox Top";
            cloneTexturedPrimitiveObject.Transform.Translation = new Vector3(0, AppData.WorldScale / 2.0f, 0);
            cloneTexturedPrimitiveObject.Transform.Rotation = new Vector3(90, -90, 0);
            cloneTexturedPrimitiveObject.EffectParameters.Texture = this.textureDictionary["skybox_top"];
            this.objectManager.Add(cloneTexturedPrimitiveObject);
            
            cloneTexturedPrimitiveObject = archTexturedPrimitiveObject.Clone() as PrimitiveObject;
            cloneTexturedPrimitiveObject.Transform.Scale *= AppData.WorldScale;
            cloneTexturedPrimitiveObject.ID = "Skybox Front";
            cloneTexturedPrimitiveObject.Transform.Translation = new Vector3(0, 0, AppData.WorldScale / 2.0f);
            cloneTexturedPrimitiveObject.Transform.Rotation = new Vector3(0, 180, 0);
            cloneTexturedPrimitiveObject.EffectParameters.Texture = this.textureDictionary["skybox_front"];
            this.objectManager.Add(cloneTexturedPrimitiveObject);
            
            cloneTexturedPrimitiveObject = archTexturedPrimitiveObject.Clone() as PrimitiveObject;
            cloneTexturedPrimitiveObject.Transform.Scale *= AppData.WorldScale;
            cloneTexturedPrimitiveObject.ID = "Skybox Bottom";
            cloneTexturedPrimitiveObject.Transform.Translation = new Vector3(0, -AppData.WorldScale / 2.0f, 0);
            cloneTexturedPrimitiveObject.Transform.Rotation = new Vector3(270, -90, 0);
            cloneTexturedPrimitiveObject.EffectParameters.Texture = this.textureDictionary["skybox_bottom"];
            this.objectManager.Add(cloneTexturedPrimitiveObject);
            
            cloneTexturedPrimitiveObject = archTexturedPrimitiveObject.Clone() as PrimitiveObject;
            cloneTexturedPrimitiveObject.Transform.Scale *= AppData.WorldScale;
            cloneTexturedPrimitiveObject.ID = "Skybox Right";
            cloneTexturedPrimitiveObject.Transform.Translation = new Vector3(-AppData.WorldScale / 2.0f, 0, 0);
            cloneTexturedPrimitiveObject.Transform.Rotation = new Vector3(0, 90, 0);
            cloneTexturedPrimitiveObject.EffectParameters.Texture = this.textureDictionary["skybox_right"];
            this.objectManager.Add(cloneTexturedPrimitiveObject);
            
            cloneTexturedPrimitiveObject = archTexturedPrimitiveObject.Clone() as PrimitiveObject;
            cloneTexturedPrimitiveObject.Transform.Scale *= AppData.WorldScale;
            cloneTexturedPrimitiveObject.ID = "Skybox Back";
            cloneTexturedPrimitiveObject.Transform.Translation = new Vector3(0, 0, -AppData.WorldScale / 2.0f);
            cloneTexturedPrimitiveObject.EffectParameters.Texture = this.textureDictionary["skybox_back"];
            this.objectManager.Add(cloneTexturedPrimitiveObject);
            #endregion
        }

        private void InitializeCameras()
        {
            Viewport viewport = new Viewport(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            float width = (float) this.resolution.X / this.resolution.X * this.cameraPosition.Z;
            float height = (float) this.resolution.Y / this.resolution.X * this.cameraPosition.Z;
            float fieldOfView = MathHelper.ToRadians(60);
            float aspectRatio = this.resolution.Y / this.resolution.X;
            float nearClipPlane = 1;
            float farClipPlane = 4000;

            ProjectionParameters projectionParameters = new ProjectionParameters(
                width,
                height,
                fieldOfView,
                aspectRatio,
                nearClipPlane,
                farClipPlane
            );

            if (this.screenLayoutType == ScreenLayoutType.Orbit)
            {
                AddTrackCamera(AppData.TrackCameraID, viewport, projectionParameters);
                AddOrbitalCamera(AppData.OrbitCameraID, viewport, projectionParameters);
            }
        }

        private void InitializeMenu()
        {
            string sceneID = "";
            Texture2D texture = null;
            Transform2D transform = null;
            Vector2 position = Vector2.Zero;

            int verticalBtnSeparation = 75;
            UIButtonObject uiButtonObject = null;
            UIButtonObject clone = null;
            string buttonText = "";
            string buttonID = "";

            #region Main Menu
            sceneID = AppData.MenuMainID;
            texture = this.textureDictionary["main_menu"];

            Vector2 scale = new Vector2(
                (float)graphics.PreferredBackBufferWidth / texture.Width,
                (float)graphics.PreferredBackBufferHeight / texture.Height
            );

            this.menuManager.Add(
                sceneID,
                new UITextureObject(
                    "Main Menu Texture",
                    ActorType.UITexture,
                    StatusType.Drawn,
                    new Transform2D(scale),
                    Color.White,
                    SpriteEffects.None,
                    1,
                    texture
                )
            );

            //Start button
            buttonID = "start_button";
            buttonText = "Start";

            position = new Vector2(graphics.PreferredBackBufferWidth / 2.0f, 300);
            texture = this.textureDictionary["button_background"];

            transform = new Transform2D(
                position,
                0,
                new Vector2(1.0f, 1.0f),
                new Vector2(texture.Width / 2.0f, texture.Height / 2.0f),
                new Integer2(texture.Width, texture.Height)
            );

            uiButtonObject = new UIButtonObject(
                buttonID,
                ActorType.UIButton,
                StatusType.Update | StatusType.Drawn,
                transform,
                Color.LightGray,
                SpriteEffects.None,
                0.1f,
                texture,
                buttonText,
                this.fontDictionary["menu"],
                Color.White,
                new Vector2(0, -3)
            );

            //Attach controller
            uiButtonObject.AttachController(
                new UIScaleSineLerpController(
                    "SineScaleLerpController",
                    ControllerType.SineScaleLerp,
                    new TrigonometricParameters(0.1f, 0.2f, 1.0f)
                )
            );

            this.menuManager.Add(sceneID, uiButtonObject);

            //Audio button
            clone = (UIButtonObject)uiButtonObject.Clone();

            clone.Transform.Translation += new Vector2(0, 1 * verticalBtnSeparation);
            clone.ID = "audio_button";
            clone.Text = "Audio";

            clone.Color = Color.DarkGray;
            this.menuManager.Add(sceneID, clone);

            //Controls button
            clone = (UIButtonObject)uiButtonObject.Clone();

            clone.Transform.Translation += new Vector2(0, 2 * verticalBtnSeparation);
            clone.ID = "controls_button";
            clone.Text = "Controls";

            clone.Color = Color.Gray;
            this.menuManager.Add(sceneID, clone);

            //Exit button
            clone = (UIButtonObject)uiButtonObject.Clone();

            clone.Transform.Translation += new Vector2(0, 3 * verticalBtnSeparation);
            clone.ID = "exit_button";
            clone.Text = "Exit";

            clone.Color = Color.DimGray;
            clone.OriginalColor = clone.Color;
            this.menuManager.Add(sceneID, clone);
            #endregion

            #region Audio Menu
            sceneID = AppData.MenuAudioID;
            texture = this.textureDictionary["audio_menu"];

            scale = new Vector2(
                (float)graphics.PreferredBackBufferWidth / texture.Width,
                (float)graphics.PreferredBackBufferHeight / texture.Height
            );

            this.menuManager.Add(
                sceneID,
                new UITextureObject(
                    "Audio Menu Texture",
                    ActorType.UITexture,
                    StatusType.Drawn,
                    new Transform2D(scale),
                    Color.White,
                    SpriteEffects.None,
                    1,
                    texture
                )
            );

            //Volume up button
            clone = (UIButtonObject)uiButtonObject.Clone();

            clone.ID = "volume_up_button";
            clone.Text = "Volume Up";

            clone.Color = Color.White;
            this.menuManager.Add(sceneID, clone);

            //Volume down button
            clone = (UIButtonObject)uiButtonObject.Clone();

            clone.Transform.Translation += new Vector2(0, 1 * verticalBtnSeparation);
            clone.ID = "volume_down_button";
            clone.Text = "Volume Down";

            clone.Color = Color.LightGray;
            this.menuManager.Add(sceneID, clone);

            //Volume mute button
            clone = (UIButtonObject)uiButtonObject.Clone();

            clone.Transform.Translation += new Vector2(0, 2 * verticalBtnSeparation);
            clone.ID = "volume_mute_button";
            clone.Text = "Mute";

            clone.Color = Color.DarkGray;
            this.menuManager.Add(sceneID, clone);

            //Volume un-mute button
            clone = (UIButtonObject)uiButtonObject.Clone();

            clone.Transform.Translation += new Vector2(0, 3 * verticalBtnSeparation);
            clone.ID = "volume_un-mute_button";
            clone.Text = "Un-Mute";

            clone.Color = Color.Gray;
            this.menuManager.Add(sceneID, clone);

            //Back button
            clone = (UIButtonObject)uiButtonObject.Clone();

            clone.Transform.Translation += new Vector2(0, 4 * verticalBtnSeparation);
            clone.ID = "main_menu_button";
            clone.Text = "Main Menu";

            clone.Color = Color.DimGray;
            this.menuManager.Add(sceneID, clone);
            #endregion

            #region Controls Menu
            sceneID = AppData.MenuControlsID;
            texture = this.textureDictionary["controls_menu"];

            scale = new Vector2(
                (float)graphics.PreferredBackBufferWidth / texture.Width,
                (float)graphics.PreferredBackBufferHeight / texture.Height
            );

            this.menuManager.Add(
                sceneID,
                new UITextureObject(
                    "Controls Menu Texture",
                    ActorType.UITexture,
                    StatusType.Drawn,
                    new Transform2D(scale),
                    Color.White,
                    SpriteEffects.None,
                    1,
                    texture
                )
            );

            //Back button
            clone = (UIButtonObject)uiButtonObject.Clone();

            clone.Transform.Translation = new Vector2(graphics.PreferredBackBufferWidth - 175, graphics.PreferredBackBufferHeight - 72);
            clone.ID = "main_menu_button";
            clone.Text = "Main Menu";

            clone.Color = Color.DimGray;
            this.menuManager.Add(sceneID, clone);
            #endregion

            #region Pause Menu
            sceneID = AppData.MenuPauseID;
            texture = this.textureDictionary["pause_menu"];

            scale = new Vector2(
                (float)graphics.PreferredBackBufferWidth / texture.Width,
                (float)graphics.PreferredBackBufferHeight / texture.Height
            );

            this.menuManager.Add(
                sceneID,
                new UITextureObject(
                    "Pause Menu Texture",
                    ActorType.UITexture,
                    StatusType.Drawn,
                    new Transform2D(scale),
                    Color.White,
                    SpriteEffects.None,
                    1,
                    texture
                )
            );

            //Resume
            clone = (UIButtonObject)uiButtonObject.Clone();

            clone.ID = "resume_button";
            clone.Text = "Resume";

            clone.Color = Color.White;
            this.menuManager.Add(sceneID, clone);

            //Quit button
            clone = (UIButtonObject)uiButtonObject.Clone();

            clone.Transform.Translation += new Vector2(0, 1 * verticalBtnSeparation);
            clone.ID = "main_menu_button";
            clone.Text = "Main Menu";

            clone.Color = Color.LightGray;
            this.menuManager.Add(sceneID, clone);
            #endregion

            #region Lose Screen
            sceneID = AppData.ScreenLoseID;
            texture = this.textureDictionary["lose_screen"];

            scale = new Vector2(
                (float)graphics.PreferredBackBufferWidth / texture.Width,
                (float)graphics.PreferredBackBufferHeight / texture.Height
            );

            this.menuManager.Add(
                sceneID,
                new UITextureObject(
                    "Lose Screen Texture",
                    ActorType.UITexture,
                    StatusType.Drawn,
                    new Transform2D(scale),
                    Color.White,
                    SpriteEffects.None,
                    1,
                    texture
                )
            );

            //Back button
            clone = (UIButtonObject)uiButtonObject.Clone();

            clone.Transform.Translation = new Vector2(graphics.PreferredBackBufferWidth - 175, graphics.PreferredBackBufferHeight - 72);
            clone.ID = "continue_button";
            clone.Text = "Continue";

            clone.Color = Color.DimGray;
            this.menuManager.Add(sceneID, clone);
            #endregion

            #region Win Screen
            sceneID = AppData.ScreenWinID;
            texture = this.textureDictionary["win_screen"];

            scale = new Vector2(
                (float)graphics.PreferredBackBufferWidth / texture.Width,
                (float)graphics.PreferredBackBufferHeight / texture.Height
            );

            this.menuManager.Add(
                sceneID,
                new UITextureObject(
                    "Win Screen Texture",
                    ActorType.UITexture,
                    StatusType.Drawn,
                    new Transform2D(scale),
                    Color.White,
                    SpriteEffects.None,
                    1,
                    texture
                )
            );

            //Back button
            clone = (UIButtonObject)uiButtonObject.Clone();

            clone.Transform.Translation = new Vector2(graphics.PreferredBackBufferWidth - 175, graphics.PreferredBackBufferHeight - 72);
            clone.ID = "continue_button";
            clone.Text = "Continue";

            clone.Color = Color.DimGray;
            this.menuManager.Add(sceneID, clone);
            #endregion

            #region End Screen
            sceneID = AppData.ScreenEndID;
            texture = this.textureDictionary["end_screen"];

            scale = new Vector2(
                (float)graphics.PreferredBackBufferWidth / texture.Width,
                (float)graphics.PreferredBackBufferHeight / texture.Height
            );

            this.menuManager.Add(
                sceneID,
                new UITextureObject(
                    "End Screen Texture",
                    ActorType.UITexture,
                    StatusType.Drawn,
                    new Transform2D(scale),
                    Color.White,
                    SpriteEffects.None,
                    1,
                    texture
                )
            );

            //Main menu button
            clone = (UIButtonObject)uiButtonObject.Clone();

            clone.Transform.Translation = new Vector2(graphics.PreferredBackBufferWidth - 175, graphics.PreferredBackBufferHeight - 72);
            clone.ID = "main_menu_button";
            clone.Text = "Main Menu";

            clone.Color = Color.DimGray;
            this.menuManager.Add(sceneID, clone);
            #endregion
        }

        private void InitializeHUD()
        {
            UITextureObject textureObject = null;
            Vector2 position = Vector2.Zero;
            Vector2 scale = Vector2.Zero;

            Transform2D transform = null;
            Texture2D texture = null;

            texture = this.textureDictionary["textbox"];
            scale = new Vector2(0.75f, 0.75f);

            #region UI Textbox
            position = new Vector2(25, 25);

            transform = new Transform2D(
                position, 
                0, 
                scale,
                Vector2.Zero,
                new Integer2(texture.Width, texture.Height)
            );

            textureObject = new UITextureObject(
                AppData.UITextBoxID,
                ActorType.UITexture,
                StatusType.Drawn | StatusType.Update,
                transform,
                Color.White,
                SpriteEffects.None,
                1,
                texture
            );

            this.uiManager.Add(textureObject);
            #endregion
        }

        private void InitializeMouse()
        {
            Texture2D texture = this.textureDictionary["default_reticle"];
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);

            UIPickingMouseObject myUIMouseObject = new UIPickingMouseObject(
                "Picking Mouse Object",
                ActorType.UITexture,
                new Transform2D(Vector2.One),
                this.fontDictionary["mouse"],
                "",
                new Vector2(0, 40),
                texture,
                this.mouseManager,
                this.eventDispatcher
            );

            this.uiManager.Add(myUIMouseObject);
        }

        private void InitializeTextbox()
        {
            #region Text box
            string sceneID = "TextboxID";
            string textboxText = this.textboxManager.TextboxText;

            Transform2D transformtext = new Transform2D(
                new Vector2(40, 45),
                0,
                new Vector2(0.6f, 0.6f),
                Vector2.Zero,
                new Integer2(100, 100)
            );

            UITextObject uiTextObj = new UITextObject(
                sceneID,
                ActorType.UIText,
                StatusType.Drawn,
                transformtext,
                Color.White,
                SpriteEffects.None,
                0,
                textboxText,
                this.fontDictionary["hud"]
            );

            this.textboxManager.Add("textbox", uiTextObj);
            #endregion
        }

        private void StartGame()
        {
            //Will be received by the menu manager and screen manager and set the menu to be shown and game to be paused
            EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Menu));

            //Publish an event to set the camera
            EventDispatcher.Publish(new EventData(EventActionType.OnCameraSetActive, EventCategoryType.Camera, new object[] { AppData.TrackCameraID }));
        }
        #endregion

        #region Cameras
        private void AddTrackCamera(string id, Viewport viewport, ProjectionParameters projectionParameters)
        {
            Transform3D transform = new Transform3D(
               this.trackCameraPosition,
               Vector3.Zero,
               Vector3.Zero,
               -Vector3.UnitZ,
               Vector3.UnitY
           );

            float drawDepth = 0f;

            this.trackCamera = new OrbitalCamera(
                id,
                ActorType.Camera,
                transform,
                projectionParameters,
                viewport,
                drawDepth,
                StatusType.Update,
                this.cameraOrbitPoint
            );

            this.trackCamera.AttachController(
                new Track3DController(
                    AppData.TrackCameraControllerID,
                    ControllerType.Track,
                    this.trackDictionary["track_level_" + StateManager.CurrentLevel],
                    PlayStatusType.Play
                )
            );

            this.cameraManager.Add(this.trackCamera);
        }

        private void AddOrbitalCamera(string id, Viewport viewport, ProjectionParameters projectionParameters)
        {
            Transform3D transform = new Transform3D(
                this.orbitalCameraPosition,
                Vector3.Zero,
                Vector3.Zero,
                -Vector3.UnitZ,
                Vector3.UnitY
            );

            float drawDepth = 0f;

            this.orbitalCamera = new OrbitalCamera(
                id,
                ActorType.Camera,
                transform,
                projectionParameters,
                viewport,
                drawDepth,
                StatusType.Update,
                this.cameraOrbitPoint
            );

            this.orbitalCamera.AttachController(
                new OrbitalCameraController(
                    AppData.OrbitalCameraControllerID,
                    ControllerType.OrbitalCamera,
                    projectionParameters.Width,
                    projectionParameters.Height,
                    AppData.OrbitSpeed,
                    AppData.OrbitAngle,
                    AppData.CameraControlKeys,
                    this.inputManagerParameters,
                    this.soundManager
                )
            );

            if (this.playerObject != null) (this.playerObject.FindControllers(x => x.GetID() == AppData.PlayerControllerID)[0] as PlayerController).Camera = this.orbitalCamera;
            this.cameraManager.Add(this.orbitalCamera);
        }
        #endregion

        #region Map Setup
        private void SetupBitArray()
        {
            //Determine starting position of each component in the LevelData text file
            this.breakableBlocksStartPosition = AppData.BreakableBlocksStartPosition;
            this.unbreakableBlockStartPosition = AppData.UnbreakableBlocksStartPosition;
            this.goalsStartPosition = AppData.GoalsStartPosition;
            this.objectivesStartPosition = AppData.ObjectivesStartPosition;
            this.playerStartPosition = AppData.PlayersStartPosition;

            //Reserve bits for each map component
            this.reservedBreakableBlockBits = AppData.ReservedBreakableBlockBits;
            this.reservedUnbreakableBlockBits = AppData.ReservedUnbreakableBlockBits;
            this.reservedGoalBits = AppData.ReservedGoalBits;
            this.reservedObjectiveBits = AppData.ReservedObjectiveBits;
            this.reservedPlayerBits = AppData.ReservedPlayerBits;

            //Calculate the shift for each map component, relative to previous component
            this.breakableBlocksShiftPosition = 0;
            this.unbreakableBlocksShiftPosition = this.breakableBlocksShiftPosition + this.reservedBreakableBlockBits;
            this.goalsShiftPosition = this.unbreakableBlocksShiftPosition + this.reservedUnbreakableBlockBits;
            this.objectivesShiftPosition = this.goalsShiftPosition + this.reservedGoalBits;
            this.playerShiftPosition = this.objectivesShiftPosition + this.reservedObjectiveBits;
        }

        private void LoadProgressFromFile()
        {
            if (File.Exists("App/Data/CurrentLeve.txt"))
                StateManager.CurrentLevel = int.Parse(File.ReadAllText("App/Data/CurrentLevel.txt"));
            else
                StateManager.CurrentLevel = 1;
        }

        private void WriteLevelToFile()
        {
            if (File.Exists("App/Data/CurrentLevel.txt"))
                File.WriteAllText("App/Data/CurrentLevel.txt", StateManager.CurrentLevel.ToString());
        }

        private void LoadAllLevelsFromFile()
        {
            #region Read File
            //Store all file data
            string fileText = File.ReadAllText("App/Data/LevelData.txt");

            //Split the file into an array of levels
            this.levels = fileText.Split('*');
            #endregion
        }

        private void LoadLevelMap()
        {
            #region Load Level
            string[] level = this.levels[StateManager.CurrentLevel].Split('&');
            #endregion

            #region Determine Level Map Size
            CalculateMapSize(level);
            #endregion

            #region Create Level Map
            CreateMap(level);
            #endregion

            #region Contruct Level Map
            ConstructLevelMap();
            #endregion
        }

        private void CalculateMapSize(string[] layers)
        {
            int x = 0, y = 0, z = 0;

            //Set y - The amount of layers correspond to the amount of cells in the y dimension
            y = layers.Length;

            //Loop through each layer
            foreach (string layer in layers)
            {
                //Cleanup layer
                string cleanLayer;
                cleanLayer = layer.Trim();

                //Split the current layer into lines
                string[] lines = cleanLayer.Split('/');

                //If the amount of lines is larger than the current z (lines = z)
                if (lines.Length > z)

                    //Update z - The amount of lines correspond to the amount of cells in the z dimension
                    z = lines.Length;

                //Loop through each line
                foreach (string line in lines)
                {
                    //Cleanup line
                    string cellLine;
                    cellLine = line.Split('-')[1].Trim();            //Measures room element of the text file - see GDApp/App/Data/levelData.txt
                                                                     //Makes an assumption that all elements (rooms, sounds, items) of the map are of the same dimension
                    cellLine = cellLine.Replace('|', ' ');
                    cellLine = cellLine.Replace(" ", string.Empty);
                    string[] cells = cellLine.Split(',');

                    //If the current amount of cells in is larger than the current x (amount of cells in a line)
                    if (cells.Length > x)

                        //Update x - The amount of elements in the cells array corresponds to the amount of cells in the x dimension
                        x = cells.Length;
                }
            }

            this.levelMap = new int[x, y, z];
        }

        private void CreateMap(string[] layers)
        {
            int x = 0, y = 0, z = 0;

            //Loop through each layer
            foreach (string layer in layers)
            {
                //Cleanup layer
                string cleanLayer;
                cleanLayer = layer.Trim();

                //Split the current layer into an array of lines
                string[] lines = cleanLayer.Split('/');

                //Loop through each line
                foreach (string line in lines)
                {
                    #region Place Breakable Blocks
                    PlaceComponents(line, this.breakableBlocksStartPosition, this.breakableBlocksShiftPosition, x, y, z);
                    #endregion

                    #region Place Un-Breakable Blocks
                    PlaceComponents(line, this.unbreakableBlockStartPosition, this.unbreakableBlocksShiftPosition, x, y, z);
                    #endregion

                    #region Place Goals
                    PlaceComponents(line, this.goalsStartPosition, this.goalsShiftPosition, x, y, z);
                    #endregion

                    #region Place Objective
                    PlaceComponents(line, this.objectivesStartPosition, this.objectivesShiftPosition, x, y, z);
                    #endregion

                    #region Spawn Player
                    PlaceComponents(line, this.playerStartPosition, this.playerShiftPosition, x, y, z);
                    #endregion

                    //Iterate z
                    z++;
                }

                //Reset z
                z = 0;

                //Iterate y
                y++;
            }
        }

        private void PlaceComponents(string line, int index, int shiftPosition, int x, int y, int z)
        {
            //Filter information
            line = line.Split('-')[index].Trim();
            line = line.Replace('|', ' ');
            line = line.Replace(" ", string.Empty);

            //Split the line into an array of cells
            string[] cells = line.Split(',');

            //Loop through each cell
            foreach (string cell in cells)
            {
                //Place cell in map
                this.levelMap[x, y, z] += (int.Parse(cell) << shiftPosition);

                //Iterate x
                x++;
            }
        }

        private void ConstructLevelMap()
        {
            bool blockSet = false;

            #region Calculate Cell Dimensions
            //Calculate the width, height and depth of each cell
            float width = AppData.BlockWidth;
            float height = AppData.BlockHeight;
            float depth = AppData.BlockDepth;
            #endregion

            #region Construct Cells
            //Loop through each element in the level map matrix
            for (int x = 0; x < this.levelMap.GetLength(0); x++)
            {
                for (int y = 0; y < this.levelMap.GetLength(1); y++)
                {
                    for (int z = 0; z < this.levelMap.GetLength(2); z++)
                    {
                        #region Calculate Transform
                        //Calculate the transform position of each component in the map
                        Transform3D transform = new Transform3D(
                            new Vector3(x * width, y * height, z * depth),
                            new Vector3(0, 0, 0),
                            new Vector3(1, 1, 1),
                            -Vector3.UnitZ,
                            Vector3.UnitY
                        );
                        #endregion

                        #region Construct Breakable Block
                        //Extract the current block from the breakable block section of the level map matrix
                        int breakableBlock = BitwiseExtraction.extractKBitsFromNumberAtPositionP(this.levelMap[x, y, z], this.reservedBreakableBlockBits, this.breakableBlocksShiftPosition);

                        //If the current block has been set
                        if (breakableBlock > 0)
                        {
                            //Construct a breakable block at the current position
                            ConstructBreakableBlock(breakableBlock, transform);

                            //Update set state
                            blockSet = true;
                        }
                        #endregion

                        #region Construct Un-Breakable Block
                        //Extract the current block from the un-breakable block section of the level map matrix
                        int unbrekableBlock = BitwiseExtraction.extractKBitsFromNumberAtPositionP(this.levelMap[x, y, z], this.reservedUnbreakableBlockBits, this.unbreakableBlocksShiftPosition);

                        //If the current block has been set
                        if (unbrekableBlock > 0)
                        {
                            //Construct an un-breakable block at the current position
                            ConstructUnbreakableBlock(unbrekableBlock, transform);

                            //Update set state
                            blockSet = true;
                        }
                        #endregion

                        #region Construct Goal
                        //Extract the current block from the goal section of the level map matrix
                        int goal = BitwiseExtraction.extractKBitsFromNumberAtPositionP(this.levelMap[x, y, z], this.reservedGoalBits, this.goalsShiftPosition);

                        //If the current block has been set
                        if (goal > 0)
                        {
                            //Update level goals
                            StateManager.GoalsRemaining++;

                            //Construct a goal at the current position
                            ConstructGoal(goal, transform);

                            //Update set state
                            blockSet = true;
                        }
                        #endregion

                        #region Construct Objective
                        //Extract the current block from the objective section of the level map matrix
                        int objective = BitwiseExtraction.extractKBitsFromNumberAtPositionP(this.levelMap[x, y, z], this.reservedObjectiveBits, this.objectivesShiftPosition);

                        //If the current block has been set
                        if (objective > 0)
                        {
                            //Construct an objective at the current position
                            ConstructObjective(objective, transform);

                            //Update set state
                            blockSet = true;
                        }
                        #endregion

                        #region Spawn Player
                        //Extract the current block from the player block section of the level map matrix
                        int player = BitwiseExtraction.extractKBitsFromNumberAtPositionP(this.levelMap[x, y, z], this.reservedPlayerBits, this.playerShiftPosition);

                        //If the current block has been set
                        if (player > 0)
                        {
                            //Spawn the player at the current position
                            SpawnPlayer(player, transform);

                            //Update set state
                            blockSet = true;
                        }
                        #endregion

                        #region Size Map
                        //Update the size of the map, in each dimension
                        if (blockSet)
                        {
                            if (x > this.xDimension) this.xDimension = x;
                            if (y > this.yDimension) this.yDimension = y;
                            if (z > this.zDimension) this.zDimension = z;
                            blockSet = false;
                        }
                        #endregion
                    }
                }
            }
            #endregion

            #region Position Orbital Camera
            //Orbit about the center of the map
            this.cameraOrbitPoint.X = (float)(this.xDimension) / 2;
            this.cameraOrbitPoint.Y = (float)(this.yDimension) / 2;
            this.cameraOrbitPoint.Z = (float)(this.zDimension) / 2;

            //Place the camera at the orbit point
            this.cameraPosition = this.cameraOrbitPoint;

            //Determine the cameras position, based on the largest dimension
            //This ensures that all blocks are visible on-screen
            if (this.xDimension >= this.yDimension)
            {
                if (this.xDimension >= this.zDimension)
                {
                    this.cameraPosition.Z += this.xDimension;
                }
                else
                {
                    this.cameraPosition.Z += this.zDimension;
                }
            }

            else
            {
                if (this.yDimension >= this.zDimension)
                {
                    this.cameraPosition.Z += this.yDimension;
                }
                else
                {
                    this.cameraPosition.Z += this.zDimension;
                }
            }

            //Create some space on-screen by zooming the camera out
            this.cameraPosition.Z += 7;

            //Position the orbital and track cameras
            this.orbitalCameraPosition = this.cameraPosition;
            this.trackCameraPosition = this.cameraPosition;
            #endregion

            #region Load Track Camera
            //Load the track for this level - if not loaded already
            if (!this.trackDictionary.ContainsKey("track_level_" + StateManager.CurrentLevel))
            {
                LoadLevelTrack();
            }
            #endregion
        }

        private void ConstructBreakableBlock(int breakableBlockOpacityLevel, Transform3D transform)
        {
            //Pull from dictionary
            CollidablePrimitiveObject collidablePrimitiveObject = (objectArchetypeDictionary[AppData.LitTexturedCubeArchetypeID] as CollidablePrimitiveObject).Clone() as CollidablePrimitiveObject;
            
            //Tag appropriately
            if (breakableBlockOpacityLevel > 1)
            {
                collidablePrimitiveObject.ID = "Breakable Transparent Block";
            }
            else
            {
                collidablePrimitiveObject.ID = "Breakable Block";
            }

            //Set values
            collidablePrimitiveObject.Transform = transform;
            collidablePrimitiveObject.EffectParameters.Alpha = AppData.StandardObjectOpacity / breakableBlockOpacityLevel;
            collidablePrimitiveObject.EffectParameters.OriginalColor = Color.White;
            collidablePrimitiveObject.EffectParameters.DiffuseColor = Color.White;
            
            //Add to object manager
            this.objectManager.Add(collidablePrimitiveObject);
        }

        private void ConstructUnbreakableBlock(int unbreakableBlockOpacityLevel, Transform3D transform)
        {
            //Pull from dictionary
            CollidablePrimitiveObject collidablePrimitiveObject = (objectArchetypeDictionary[AppData.LitTexturedCubeArchetypeID] as CollidablePrimitiveObject).Clone() as CollidablePrimitiveObject;

            //Tag appropriately
            if (unbreakableBlockOpacityLevel > 1)
            {
                collidablePrimitiveObject.ID = "Unbreakable Transparent Block";
            }
            else
            {
                collidablePrimitiveObject.ID = "Unbreakable Block";
            }

            //Set values
            collidablePrimitiveObject.Transform = transform;
            collidablePrimitiveObject.EffectParameters.Alpha = AppData.StandardObjectOpacity / unbreakableBlockOpacityLevel;
            collidablePrimitiveObject.EffectParameters.OriginalColor = Color.Black;
            collidablePrimitiveObject.EffectParameters.DiffuseColor = Color.Black;

            //Add to object manager
            this.objectManager.Add(collidablePrimitiveObject);
        }

        private void ConstructGoal(int goalOpacityLevel, Transform3D transform)
        {
            //Pull from dictionary
            CollidablePrimitiveObject collidablePrimitiveObject = (objectArchetypeDictionary[AppData.LitTexturedDiamondArchetypeID] as CollidablePrimitiveObject).Clone() as CollidablePrimitiveObject;

            //Set values
            collidablePrimitiveObject.ID = "Goal Objective";
            collidablePrimitiveObject.Transform = transform;
            collidablePrimitiveObject.EffectParameters.Alpha = AppData.StandardObjectOpacity / goalOpacityLevel;
            collidablePrimitiveObject.EffectParameters.OriginalColor = Color.LightGreen;
            collidablePrimitiveObject.EffectParameters.DiffuseColor = Color.LightGreen;

            //Attach controllers
            collidablePrimitiveObject.AttachController(new SpinController("Spin Controller", ControllerType.Spin, 1));

            //Add to object manager
            this.objectManager.Add(collidablePrimitiveObject);
        }

        private void ConstructObjective(int objectiveOpacityLevel, Transform3D transform)
        {
            //Pull from dictionary
            CollidablePrimitiveObject collidablePrimitiveObject = (objectArchetypeDictionary[AppData.UnlitTexturedSphereArchetypeID] as CollidablePrimitiveObject).Clone() as CollidablePrimitiveObject;

            //Tag appropriately
            if (objectiveOpacityLevel > 1)
            {
                collidablePrimitiveObject.ID = "Transparent Main Objective";
            }
            else
            {
                collidablePrimitiveObject.ID = "Main Objective";
            }

            //Set values
            collidablePrimitiveObject.Transform = transform;
            collidablePrimitiveObject.EffectParameters.Alpha = AppData.StandardObjectOpacity / objectiveOpacityLevel;
            collidablePrimitiveObject.EffectParameters.OriginalColor = Color.Plum;
            collidablePrimitiveObject.EffectParameters.DiffuseColor = Color.Plum;

            //Attach controllers
            collidablePrimitiveObject.AttachController(new ScaleController("Scale Controller", ControllerType.Scale, -0.15f));

            //Add to object manager
            this.objectManager.Add(collidablePrimitiveObject);
        }

        private void SpawnPlayer(int playerOpacityLevel, Transform3D transform)
        {
            //Pull from dictionary
            this.playerObject = (objectArchetypeDictionary[AppData.LitTexturedCubeArchetypeID] as CollidablePrimitiveObject).Clone() as CollidablePrimitiveObject;

            //Set values
            this.playerObject.ID = "Player Block";
            this.playerObject.Transform = transform;
            this.playerObject.EffectParameters.Alpha = AppData.StandardObjectOpacity / playerOpacityLevel;
            this.playerObject.EffectParameters.OriginalColor = Color.Gold;
            this.playerObject.EffectParameters.DiffuseColor = Color.Gold;

            //Attach controllers
            this.playerObject.AttachController(new PlayerController(
                    AppData.PlayerControllerID,
                    ControllerType.Player,
                    AppData.PlayerMoveKeys,
                    this.inputManagerParameters,
                    this.orbitalCamera,
                    this.objectManager,
                    Vector3.One
                )
            );

            //Add to object manager
            this.objectManager.Add(this.playerObject);
        }
        #endregion

        #region Content
        protected override void LoadContent()
        {
        }

        protected override void UnloadContent()
        {
            this.fontDictionary.Dispose();
            this.textureDictionary.Dispose();
            
            this.railDictionary.Clear();
            this.trackDictionary.Clear();
            this.objectArchetypeDictionary.Clear();
            this.vertexDictionary.Clear();
        }

        private void LoadAssets()
        {
            LoadTextures();
            LoadFonts();

            LoadStandardVertices();
            LoadBillboardVertices();
            LoadArchetypePrimitivesToDictionary();
        }
        
        private void LoadLevelTrack()
        {
            Track3D levelTrack = null;
            this.trackCameraLook = -Vector3.UnitZ;
            this.trackCameraUp = Vector3.UnitY;

            switch(StateManager.CurrentLevel)
            {
                case 1:
                    #region Level 1
                    levelTrack = new Track3D(CurveLoopType.Constant);
                    levelTrack.Add(this.trackCameraPosition, this.trackCameraLook, this.trackCameraUp, 0);
                    
                    this.trackCameraUp = MatrixUtility.CalculateTargetUpVector(Axis.NegZ, AppData.OrbitAngle / 2, this.trackCameraUp);
                    levelTrack.Add(this.trackCameraPosition, this.trackCameraLook, this.trackCameraUp, 2);

                    this.trackCameraUp = MatrixUtility.CalculateTargetUpVector(Axis.NegZ, -AppData.OrbitAngle, this.trackCameraUp);
                    levelTrack.Add(this.trackCameraPosition, this.trackCameraLook, this.trackCameraUp, 4);

                    this.trackCameraUp = MatrixUtility.CalculateTargetUpVector(Axis.NegZ, AppData.OrbitAngle, this.trackCameraUp);
                    levelTrack.Add(this.trackCameraPosition, this.trackCameraLook, this.trackCameraUp, 6);
                    
                    this.trackCameraUp = MatrixUtility.CalculateTargetUpVector(Axis.NegZ, -AppData.OrbitAngle / 2, this.trackCameraUp);
                    levelTrack.Add(this.trackCameraPosition, this.trackCameraLook, this.trackCameraUp, 8);

                    this.trackDictionary.Add("track_level_1", levelTrack);
                    this.levelTrackTime[0] = 8;
                    #endregion
                    break;

                case 2:
                    #region Level 2
                    levelTrack = new Track3D(CurveLoopType.Constant);
                    levelTrack.Add(this.trackCameraPosition, this.trackCameraLook, Vector3.UnitY, 0);

                    this.trackCameraPosition = MatrixUtility.CalculateTargetPositionVector(Axis.NegY, AppData.OrbitAngle, this.trackCameraPosition, this.cameraOrbitPoint);
                    this.trackCameraLook = MatrixUtility.CalculateTargetLookVector(Axis.NegY, AppData.OrbitAngle, this.trackCameraLook);
                    levelTrack.Add(this.trackCameraPosition, this.trackCameraLook, this.trackCameraUp, 2);

                    this.trackCameraPosition = MatrixUtility.CalculateTargetPositionVector(Axis.NegY, AppData.OrbitAngle, this.trackCameraPosition, this.cameraOrbitPoint);
                    this.trackCameraLook = MatrixUtility.CalculateTargetLookVector(Axis.NegY, AppData.OrbitAngle, this.trackCameraLook);
                    levelTrack.Add(this.trackCameraPosition, this.trackCameraLook, this.trackCameraUp, 4);

                    this.trackCameraPosition = MatrixUtility.CalculateTargetPositionVector(Axis.NegY, AppData.OrbitAngle, this.trackCameraPosition, this.cameraOrbitPoint);
                    this.trackCameraLook = MatrixUtility.CalculateTargetLookVector(Axis.NegY, AppData.OrbitAngle, this.trackCameraLook);
                    levelTrack.Add(this.trackCameraPosition, this.trackCameraLook, this.trackCameraUp, 6);

                    this.trackCameraPosition = MatrixUtility.CalculateTargetPositionVector(Axis.NegY, AppData.OrbitAngle, this.trackCameraPosition, this.cameraOrbitPoint);
                    this.trackCameraLook = MatrixUtility.CalculateTargetLookVector(Axis.NegY, AppData.OrbitAngle, this.trackCameraLook);
                    levelTrack.Add(this.trackCameraPosition, this.trackCameraLook, this.trackCameraUp, 8);

                    this.trackDictionary.Add("track_level_2", levelTrack);
                    this.levelTrackTime[1] = 8;
                    #endregion
                    break;

                case 3:
                    #region Level 3
                    levelTrack = new Track3D(CurveLoopType.Constant);
                    levelTrack.Add(this.trackCameraPosition, this.trackCameraLook, Vector3.UnitY, 0);

                    this.trackCameraPosition = MatrixUtility.CalculateTargetPositionVector(Axis.NegY, AppData.OrbitAngle, this.trackCameraPosition, this.cameraOrbitPoint);
                    this.trackCameraLook = MatrixUtility.CalculateTargetLookVector(Axis.NegY, AppData.OrbitAngle, this.trackCameraLook);
                    levelTrack.Add(this.trackCameraPosition, this.trackCameraLook, this.trackCameraUp, 2);

                    this.trackCameraPosition = MatrixUtility.CalculateTargetPositionVector(Axis.NegY, AppData.OrbitAngle, this.trackCameraPosition, this.cameraOrbitPoint);
                    this.trackCameraLook = MatrixUtility.CalculateTargetLookVector(Axis.NegY, AppData.OrbitAngle, this.trackCameraLook);
                    levelTrack.Add(this.trackCameraPosition, this.trackCameraLook, this.trackCameraUp, 4);

                    this.trackCameraPosition = MatrixUtility.CalculateTargetPositionVector(Axis.NegY, AppData.OrbitAngle, this.trackCameraPosition, this.cameraOrbitPoint);
                    this.trackCameraLook = MatrixUtility.CalculateTargetLookVector(Axis.NegY, AppData.OrbitAngle, this.trackCameraLook);
                    levelTrack.Add(this.trackCameraPosition, this.trackCameraLook, this.trackCameraUp, 6);

                    this.trackCameraPosition = MatrixUtility.CalculateTargetPositionVector(Axis.NegY, AppData.OrbitAngle, this.trackCameraPosition, this.cameraOrbitPoint);
                    this.trackCameraLook = MatrixUtility.CalculateTargetLookVector(Axis.NegY, AppData.OrbitAngle, this.trackCameraLook);
                    levelTrack.Add(this.trackCameraPosition, this.trackCameraLook, this.trackCameraUp, 8);

                    this.trackDictionary.Add("track_level_3", levelTrack);
                    this.levelTrackTime[2] = 8;
                    #endregion
                    break;
            }
        }

        private void LoadTextures()
        {
            //Blocks
            this.textureDictionary.Load("Assets/Textures/Primitives/box_texture");
            this.textureDictionary.Load("Assets/Textures/Primitives/diamond_texture");
            this.textureDictionary.Load("Assets/Textures/Primitives/quad_texture");

            //Skybox
            this.textureDictionary.Load("skybox_left", "Assets/Textures/Skybox/left");
            this.textureDictionary.Load("skybox_top", "Assets/Textures/Skybox/top");
            this.textureDictionary.Load("skybox_front", "Assets/Textures/Skybox/front");
            this.textureDictionary.Load("skybox_bottom", "Assets/Textures/Skybox/bottom");
            this.textureDictionary.Load("skybox_right", "Assets/Textures/Skybox/right");
            this.textureDictionary.Load("skybox_back", "Assets/Textures/Skybox/back");

            //Menus
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/main_menu");
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/audio_menu");
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/controls_menu");
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/pause_menu");

            //Screens
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/exit_screen");
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/lose_screen");
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/win_screen");
            this.textureDictionary.Load("Assets/Textures/UI/Menu/Backgrounds/end_screen");

            //Buttons
            this.textureDictionary.Load("Assets/Textures/UI/Buttons/button_background");

            //UI
            this.textureDictionary.Load("Assets/Textures/UI/HUD/default_reticle");
            this.textureDictionary.Load("Assets/Textures/UI/HUD/textbox");
        }

        private void LoadFonts()
        {
            this.fontDictionary.Load("hud", "Assets/Fonts/hud");
            this.fontDictionary.Load("mouse", "Assets/Fonts/mouse");
            this.fontDictionary.Load("menu", "Assets/Fonts/menu");
        }

        private void LoadStandardVertices()
        {
            #region Unlit Textured Quad
            this.vertexDictionary.Add(
                AppData.UnlitTexturedQuadVertexDataID,
                new VertexData<VertexPositionColorTexture>(
                    VertexFactory.GetTextureQuadVertices(out PrimitiveType primitiveType, out int primitiveCount),
                    primitiveType, 
                    primitiveCount
                )
            );
            #endregion

            #region Unlit Textured Cube
            this.vertexDictionary.Add(
                AppData.UnlitTexturedCubeVertexDataID,
                new BufferedVertexData<VertexPositionColorTexture>(
                    graphics.GraphicsDevice,
                    VertexFactory.GetPositionColorTextureCubeVertices(1, out primitiveType, out primitiveCount),
                    primitiveType,
                    primitiveCount
                )
            );
            #endregion

            #region Unlit Textured Sphere
            VertexPositionColorTexture[] vertexPositionNormalTextureIndexedSphereVertices = VertexFactory.GetPositionNormalTextureSphereVertices(0.5f, 10, out primitiveType, out primitiveCount);

            this.vertexDictionary.Add(
                AppData.UnlitTexturedSphereVertexDataID,
                new BufferedIndexedVertexData<VertexPositionColorTexture>(
                    graphics.GraphicsDevice, 
                    vertexPositionNormalTextureIndexedSphereVertices,
                    VertexFactory.GetSphereIndices(0.5f, 10),
                    primitiveType,
                    primitiveCount
                )
            );
            #endregion

            #region Lit Textured Cube - Indexed Vertices - Standard Buffer
            VertexPositionNormalTexture[] vertexPositionNormalTextureIndexedCubeVertices = VertexFactory.GetPositionNormalTextureDiamondIndexedVertices(AppData.CubeDimension, out primitiveType, out primitiveCount);
            VertexBuffer vertexPositionNormalTextureCubeVertexBuffer = new VertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionNormalTexture), vertexPositionNormalTextureIndexedCubeVertices.Length, BufferUsage.WriteOnly);

            this.vertexDictionary.Add(
                AppData.LitTexturedCubeIndexedVertexDataID, 
                new BufferedIndexedVertexData<VertexPositionNormalTexture>(
                    graphics.GraphicsDevice, 
                    vertexPositionNormalTextureIndexedCubeVertices, 
                    vertexPositionNormalTextureCubeVertexBuffer,
                    VertexFactory.GetCubeIndices(),
                    primitiveType, 
                    primitiveCount
                )
            );
            #endregion

            #region Lit Textured Diamond - Indexed Vertices - Dynamic Buffer
            VertexPositionNormalTexture[] vertexPositionNormalTextureIndexedDiamondVertices = VertexFactory.GetPositionNormalTextureDiamondIndexedVertices(AppData.DiamondDimension, out primitiveType, out primitiveCount);
            DynamicVertexBuffer vertexPositionNoramlTextureDiamondDynamicVertexBuffer = new DynamicVertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionNormalTexture), vertexPositionNormalTextureIndexedDiamondVertices.Length, BufferUsage.WriteOnly);

            this.vertexDictionary.Add(
                AppData.LitTexturedDiamondIndexedVertexDataID,
                new DynamicBufferedIndexedVertexData<VertexPositionNormalTexture>(
                    graphics.GraphicsDevice,
                    vertexPositionNormalTextureIndexedDiamondVertices,
                    vertexPositionNoramlTextureDiamondDynamicVertexBuffer,
                    VertexFactory.GetDiamondIndices(),
                    primitiveType,
                    primitiveCount
                )
            );
            #endregion

            #region Lit Textured Cube - Standard Vertices - Standard Buffer
            VertexPositionNormalTexture[] vertexPositionNormalTextureCubeVertices = VertexFactory.GetPositionNormalTextureCubeVertices(AppData.CubeDimension, out primitiveType, out primitiveCount);

            this.vertexDictionary.Add(
                AppData.LitTexturedCubeVertexDataID,
                new BufferedVertexData<VertexPositionNormalTexture>(
                    graphics.GraphicsDevice,
                    vertexPositionNormalTextureCubeVertices,
                    primitiveType,
                    primitiveCount
                )
            );
            #endregion

            #region Lit Textured Diamond - Standard Vertices - Dynamic Buffer
            VertexPositionNormalTexture[] vertexPositionNormalTextureDiamondVertices = VertexFactory.GetPositionNormalTextureDiamondVertices(AppData.DiamondDimension, out primitiveType, out primitiveCount);

            this.vertexDictionary.Add(
                AppData.LitTexturedDiamondVertexDataID,
                new DynamicBufferedVertexData<VertexPositionNormalTexture>(
                    graphics.GraphicsDevice,
                    vertexPositionNormalTextureDiamondVertices,
                    primitiveType,
                    primitiveCount
                )
            );
            #endregion
        }

        private void LoadBillboardVertices()
        {
            IVertexData vertexData = null;

            #region Billboard Quad - we must use this type when creating billboards
            VertexBillboard[] verticesBillboard = VertexFactory.GetVertexBillboard(1, out PrimitiveType primitiveType, out int primitiveCount);

            vertexData = new BufferedVertexData<VertexBillboard>(this.graphics.GraphicsDevice, verticesBillboard, primitiveType, primitiveCount);

            this.vertexDictionary.Add(AppData.UnlitTexturedBillboardVertexDataID, vertexData);
            #endregion
        }

        private void LoadArchetypePrimitivesToDictionary()
        {
            PrimitiveObject primitiveObject = null;
            EffectParameters effectParameters = null;

            BoxCollisionPrimitive boxCollisionPrimitive = new BoxCollisionPrimitive();
            Transform3D transform = new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.One, Vector3.UnitZ, Vector3.UnitY);

            #region Unlit Textured Quad
            effectParameters = this.effectDictionary[AppData.UnlitTexturedEffectID].Clone() as EffectParameters;
            effectParameters.Texture = this.textureDictionary["quad_texture"];
            effectParameters.DiffuseColor = Color.White;
            effectParameters.Alpha = 1;

            //Create object and pass in the primitive
            primitiveObject = new PrimitiveObject(
                AppData.UnlitTexturedQuadArchetypeID,
                ActorType.Decorator,
                transform,
                effectParameters,
                StatusType.Drawn | StatusType.Update,
                this.vertexDictionary[AppData.UnlitTexturedQuadVertexDataID]
            );

            //Add to dictionary
            this.objectArchetypeDictionary.Add(AppData.UnlitTexturedQuadArchetypeID, primitiveObject);
            #endregion

            #region Unlit Textured Collidable Cube
            effectParameters = this.effectDictionary[AppData.UnlitTexturedEffectID].Clone() as EffectParameters;
            effectParameters.Texture = this.textureDictionary["box_texture"];
            effectParameters.DiffuseColor = Color.White;

            //Create collidable object and pass in the primitive
            primitiveObject = new CollidablePrimitiveObject(
                AppData.UnlitTexturedCubeArchetypeID,
                ActorType.CollidableArchitecture,
                transform,
                effectParameters,
                StatusType.Drawn | StatusType.Update,
                this.vertexDictionary[AppData.UnlitTexturedCubeVertexDataID],
                boxCollisionPrimitive,
                this.objectManager
            );

            //Add to dictionary
            this.objectArchetypeDictionary.Add(AppData.UnlitTexturedCubeArchetypeID, primitiveObject);
            #endregion
            
            #region Lit Textured Sphere
            effectParameters = this.effectDictionary[AppData.UnlitTexturedEffectID].Clone() as EffectParameters;
            effectParameters.Texture = this.textureDictionary["quad_texture"];
            effectParameters.DiffuseColor = Color.White;

            //Create object and pass in the primitive
            primitiveObject = new CollidablePrimitiveObject(
                AppData.UnlitTexturedSphereArchetypeID,
                ActorType.Decorator,
                transform,
                effectParameters,
                StatusType.Drawn | StatusType.Update,
                this.vertexDictionary[AppData.UnlitTexturedSphereVertexDataID],
                boxCollisionPrimitive,
                this.objectManager
            );

            //Add to dictionary
            this.objectArchetypeDictionary.Add(AppData.UnlitTexturedSphereArchetypeID, primitiveObject);
            #endregion

            #region Lit Textured Collidable Cube
            effectParameters = this.effectDictionary[AppData.LitTexturedEffectID].Clone() as EffectParameters;
            effectParameters.Texture = this.textureDictionary["box_texture"];
            effectParameters.DiffuseColor = Color.White;

            //Create collidable object and pass in the primitive
            primitiveObject = new CollidablePrimitiveObject(
                AppData.LitTexturedCubeArchetypeID,
                ActorType.CollidableArchitecture,
                transform,
                effectParameters,
                StatusType.Drawn | StatusType.Update,
                this.vertexDictionary[AppData.LitTexturedCubeVertexDataID],
                boxCollisionPrimitive,
                this.objectManager
            );

            //Add to dictionary
            this.objectArchetypeDictionary.Add(AppData.LitTexturedCubeArchetypeID, primitiveObject);
            #endregion

            #region Lit Textured Collidable Diamond
            effectParameters = this.effectDictionary[AppData.LitTexturedEffectID].Clone() as EffectParameters;
            effectParameters.Texture = this.textureDictionary["diamond_texture"];
            effectParameters.DiffuseColor = Color.LightGreen;

            //Create collidable object and pass in the primitive
            primitiveObject = new CollidablePrimitiveObject(
                AppData.LitTexturedDiamondArchetypeID,
                ActorType.CollidableArchitecture,
                transform,
                effectParameters,
                StatusType.Drawn | StatusType.Update,
                this.vertexDictionary[AppData.LitTexturedDiamondIndexedVertexDataID],
                boxCollisionPrimitive,
                this.objectManager
            );

            //Add to dictionary
            this.objectArchetypeDictionary.Add(AppData.LitTexturedDiamondArchetypeID, primitiveObject);
            #endregion
        }
        #endregion

        #region Game State
        private void UpdateTextbox(GameTime gameTime)
        {
            //Every x seconds
            if (((int) gameTime.TotalGameTime.TotalMilliseconds % (AppData.TimePerMessage * 1000)) == 0)
            {
                EventDispatcher.Publish(new EventData(EventActionType.SetActive, EventCategoryType.Textbox, new object[] { AppData.TextboxMessages[StateManager.CurrentTextInLoop] }));
                StateManager.CurrentTextInLoop = (StateManager.CurrentTextInLoop + 1) % AppData.TextboxMessages.Length;
            }
        }

        private void CheckGameState()
        {
            CheckLevelClear();
            CheckPlayerDied();

            CheckGamePaused();
            CheckMapReset();

            CheckResumeClicked();
            CheckContinueClicked();
            CheckMainMenuClicked();

            CheckTrackComplete();
        }

        private void CheckLevelClear()
        {
            if (StateManager.LevelClear)
            {
                StateManager.LevelClear = false;

                if (++StateManager.CurrentLevel == this.levels.Length)
                {
                    GameWon();
                    return;
                }

                EventDispatcher.Publish(new EventData(EventActionType.OnCameraSetActive, EventCategoryType.Camera, new object[] { AppData.TrackCameraID }));
                EventDispatcher.Publish(new EventData(EventActionType.OnActive, EventCategoryType.Menu, new object[] { AppData.ScreenWinID }));
                EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Sound2D, new object[] { "awakening" }));
                EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Sound2D, new object[] { "fall" }));
                EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Menu));
            }
        }

        private void CheckPlayerDied()
        {
            if (StateManager.PlayerDied)
            {
                EventDispatcher.Publish(new EventData(EventActionType.OnActive, EventCategoryType.Menu, new object[] { AppData.ScreenLoseID }));
                EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Sound2D, new object[] { "awakening" }));
                EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Sound2D, new object[] { "fall" }));
                EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Menu));
                StateManager.PlayerDied = false;
            }
        }

        private void CheckGamePaused()
        {
            if (!StateManager.PlayerDied && !StateManager.LevelClear)
            {
                if (this.keyboardManager.IsFirstKeyPress(AppData.MenuShowHideKey))
                {
                    if (this.menuManager.IsVisible)
                    {
                        EventDispatcher.Publish(new EventData(EventActionType.OnStart, EventCategoryType.Menu));
                    }
                    else
                    {
                        EventDispatcher.Publish(new EventData(EventActionType.OnActive, EventCategoryType.Menu, new object[] { AppData.MenuPauseID }));
                        EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Menu));
                    }
                }
            }
        }

        private void CheckMapReset()
        {
            if (this.keyboardManager.IsFirstKeyPress(AppData.ResetKey))
            {
                if (!this.menuManager.IsVisible)
                {
                    ResetMap();
                }
            }
        }

        private void CheckResumeClicked()
        {
            if (StateManager.ResumeClicked)
            {
                if (this.menuManager.IsVisible)
                {
                    EventDispatcher.Publish(new EventData(EventActionType.OnStart, EventCategoryType.Menu));
                    StateManager.ResumeClicked = false;
                }
            }
        }

        private void CheckContinueClicked()
        {
            if (StateManager.ContinueClicked)
            {
                EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Sound2D, new object[] { "meltwater" }));
                EventDispatcher.Publish(new EventData(EventActionType.OnStart, EventCategoryType.Menu));
                StateManager.ContinueClicked = false;

                ResetMap();
            }
        }

        private void CheckMainMenuClicked()
        {
            if (StateManager.MainMenuClicked)
            {
                EventDispatcher.Publish(new EventData(EventActionType.OnActive, EventCategoryType.Menu, new object[] { AppData.MenuMainID }));
                StateManager.MainMenuClicked = false;
                ResetMap();
            }
        }

        private void CheckTrackComplete()
        {
            if (StateManager.FinishedTracking) return;

            List<IController> controllers = this.trackCamera.FindControllers(controller => controller.GetID().Equals(AppData.TrackCameraControllerID));
            if ((controllers != null) && ((controllers[0] as Track3DController).ElapsedTimeInMs > (this.levelTrackTime[StateManager.CurrentLevel - 1] * 1000)))
            {
                EventDispatcher.Publish(new EventData(EventActionType.OnCameraSetActive, EventCategoryType.Camera, new object[] { AppData.OrbitCameraID }));
                (controllers[0] as Track3DController).ElapsedTimeInMs = 0;
                StateManager.FinishedTracking = true;
            }
        }

        private void ResetMap()
        {
            EventDispatcher.Publish(new EventData(EventActionType.OnCameraSetActive, EventCategoryType.Camera, new object[] { AppData.TrackCameraID }));
            StateManager.GoalsRemaining = 0;

            this.objectManager.Clear();
            this.cameraManager.Clear();

            LoadLevelMap();
            InitializeSkyBox();
            InitializeCameras();
        }

        private void GameWon()
        {
            EventDispatcher.Publish(new EventData(EventActionType.OnActive, EventCategoryType.Menu, new object[] { AppData.ScreenEndID }));
            EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Menu));
            StateManager.CurrentLevel = 1;

            ResetMap();
        }
        #endregion

        #region Update, Draw
        protected override void Update(GameTime gameTime)
        {
            UpdateTextbox(gameTime);
            CheckGameState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }
        #endregion
    }
}