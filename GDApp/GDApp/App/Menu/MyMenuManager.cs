using GDLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDApp
{
    public class MyMenuManager : MenuManager
    {
        public MyMenuManager(
            Game game, 
            InputManagerParameters inputManagerParameters, 
            CameraManager cameraManager,
            SpriteBatch spriteBatch, 
            EventDispatcher eventDispatcher, 
            StatusType statusType
        ) : base(game, inputManagerParameters, cameraManager, spriteBatch, eventDispatcher, statusType) {
        }

        #region Event Handling
        protected override void EventDispatcher_MenuChanged(EventData eventData)
        {
            //Call base method to show/hide the menu
            base.EventDispatcher_MenuChanged(eventData);

            //If set active
            if (eventData.EventType == EventActionType.OnActive)
            {
                SetActiveList(eventData.AdditionalParameters[0] as string);
            }

            //Then generate sound events particular to your game e.g. play background music in a menu
            if (eventData.EventType == EventActionType.OnStart)
            {
                Game.IsMouseVisible = false;
                EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Sound2D, new object[] { "meltwater" }));
                EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "awakening" }));
            }
            else if (eventData.EventType == EventActionType.OnPause)
            {
                Game.IsMouseVisible = true;
                EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "meltwater" }));
                EventDispatcher.Publish(new EventData(EventActionType.OnPause, EventCategoryType.Sound2D, new object[] { "awakening" }));
            }
        }
        #endregion

        protected override void HandleMouseOver(DrawnActor2D uiObject, GameTime gameTime)
        {
            //If mouse over object
            if (uiObject.Transform.Bounds.Contains(this.InputManagerParameters.MouseManager.Bounds))

                //If left button clicked
                if (this.InputManagerParameters.MouseManager.IsLeftButtonClickedOnce())

                    //Handle mouse click
                    HandleMouseClick(uiObject, gameTime);
        }

        protected override void HandleMouseClick(DrawnActor2D uiObject, GameTime gameTime)
        {
            switch (uiObject.ID)
            {
                case "start_button":
                    DoStart();
                    break;

                case "audio_button":
                    SetActiveList(AppData.MenuAudioID);
                    break;

                case "controls_button":
                    SetActiveList(AppData.MenuControlsID);
                    break;

                case "back_button":
                    SetActiveList(AppData.MenuMainID);
                    break;

                case "exit_button":
                    DoExit();
                    break;

                case "volume_up_button":
                    EventDispatcher.Publish(new EventData(EventActionType.OnVolumeUp, EventCategoryType.GlobalSound, new object[] { 0.1f, "Global" }));
                    break;

                case "volume_down_button": 
                    EventDispatcher.Publish(new EventData(EventActionType.OnVolumeDown, EventCategoryType.GlobalSound, new object[] { 0.1f, "Global" }));
                    break;

                case "volume_mute_button":
                    EventDispatcher.Publish(new EventData(EventActionType.OnMute, EventCategoryType.GlobalSound, new object[] { 0.0f, "Global" }));
                    break;

                case "volume_un-mute_button":
                    EventDispatcher.Publish(new EventData(EventActionType.OnUnMute, EventCategoryType.GlobalSound, new object[] { 0.5f, "Global" }));
                    break;

                case "continue_button":
                    StateManager.ContinueClicked = true;
                    break;

                case "resume_button":
                    StateManager.ResumeClicked = true;
                    break;

                case "quit_button":
                    SetActiveList(AppData.MenuMainID);
                    break;
            }
            
            DoMenuClickSound();   
        }

        private void DoMenuClickSound()
        {
            EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound2D, new object[] { "select" }));
        }

        private void DoStart()
        {
            EventDispatcher.Publish(new EventData(EventActionType.OnStart, EventCategoryType.Menu));
        }

        private void DoExit()
        {
            this.Game.Exit();
        }
    }
}