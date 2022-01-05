using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary
{
    public class MenuManager : PausableDrawableGameComponent
    {
        #region Fields
        private UIObject oldUIObjectMouseOver;
        #endregion

        #region Properties
        public Dictionary<string, List<DrawnActor2D>> MenuDictionary { get; set; }
        public List<DrawnActor2D> ActiveList { get; private set; } = null;

        public SpriteBatch SpriteBatch { get; set; }
        public InputManagerParameters InputManagerParameters { get; }
        public CameraManager CameraManager { get; }

        public bool IsVisible { get; private set; }
        #endregion

        public MenuManager(
            Game game, 
            InputManagerParameters inputManagerParameters,
            CameraManager cameraManager, 
            SpriteBatch spriteBatch, 
            EventDispatcher eventDispatcher, 
            StatusType statusType
        ) : base(game, eventDispatcher, statusType) {
            this.InputManagerParameters = inputManagerParameters;
            this.CameraManager = cameraManager;
            this.SpriteBatch = spriteBatch;

            this.MenuDictionary = new Dictionary<string, List<DrawnActor2D>>();
        }

        #region Event Handling
        protected override void EventDispatcher_MenuChanged(EventData eventData)
        {
            //we need to override this method because the menu is OFF when other components are ON and vice versa
            if (eventData.EventType == EventActionType.OnStart)
            {
                this.StatusType = StatusType.Off;
                this.IsVisible = false;
            }
            else if (eventData.EventType == EventActionType.OnPause)
            {
                this.StatusType = StatusType.Drawn | StatusType.Update;
                this.IsVisible = true;
            }
        }
        #endregion

        public void Add(string menuSceneID, DrawnActor2D actor)
        {
            if(this.MenuDictionary.ContainsKey(menuSceneID))
            {
                this.MenuDictionary[menuSceneID].Add(actor);
            }
            else
            {
                this.MenuDictionary.Add(menuSceneID, new List<DrawnActor2D> { actor });
            }

            //if the user forgets to set the active list then set to the sceneID of the last added item
            if(this.ActiveList == null)
            {
                SetActiveList(menuSceneID);
                   
            }
        }

        public DrawnActor2D Find(string menuSceneID, Predicate<DrawnActor2D> predicate)
        {
            if (this.MenuDictionary.ContainsKey(menuSceneID))
            {
                return this.MenuDictionary[menuSceneID].Find(predicate);
            }
            return null;
        }

        public bool Remove(string menuSceneID, Predicate<DrawnActor2D> predicate)
        {
            DrawnActor2D foundUIObject = Find(menuSceneID, predicate);

            if (foundUIObject != null)
                return this.MenuDictionary[menuSceneID].Remove(foundUIObject);

            return false;
        }
        
        public List<DrawnActor2D> FindAllBySceneID(string menuSceneID)
        {
            if (this.MenuDictionary.ContainsKey(menuSceneID))
            {
                return this.MenuDictionary[menuSceneID];
            }
            return null;
        }

        public bool SetActiveList(string menuSceneID)
        {
            if (this.MenuDictionary.ContainsKey(menuSceneID))
            {
                this.ActiveList = this.MenuDictionary[menuSceneID];
                return true;
            }

            return false;
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            if (this.ActiveList != null)
            {
                //update all the updateable menu items (e.g. make buttons pulse etc)
                foreach (DrawnActor2D currentUIObject in this.ActiveList)
                {
                    if ((currentUIObject.GetStatusType() & StatusType.Update) != 0) //if update flag is set
                        currentUIObject.Update(gameTime);
                }
            }

            //Check for mouse over and mouse click on a menu item
            CheckMouseOverAndClick(gameTime);
        }

        private void CheckMouseOverAndClick(GameTime gameTime)
        {
            foreach (UIObject currentUIObject in this.ActiveList)
            {
                //only handle mouseover and mouse click for buttons
                if (currentUIObject.ActorType == ActorType.UIButton)
                {
                    if (this.InputManagerParameters.KeyboardManager.IsAnyKeyPressed())
                        HandleKeyboardInput();

                    //add an if to check that this is a interactive UIButton object
                    if (currentUIObject.Transform.Bounds.Intersects(this.InputManagerParameters.MouseManager.Bounds))
                    {
                        //if mouse is over a new ui object then set old to "IsMouseOver=false"
                        if (this.oldUIObjectMouseOver != null && this.oldUIObjectMouseOver != currentUIObject)
                        {
                            oldUIObjectMouseOver.MouseOverState.Update(false);
                        }

                        //update the current state of the currently mouse-over'ed ui object
                        currentUIObject.MouseOverState.Update(true);

                        //apply any mouse over or mouse click actions
                        HandleMouseOver(currentUIObject, gameTime);

                        //store the current as old for the next update
                        this.oldUIObjectMouseOver = currentUIObject;
                    }
                    else
                    {
                        //set the mouse as not being over the current ui object
                        currentUIObject.MouseOverState.Update(false);
                    }
                }
            }
        }

        protected override void ApplyDraw(GameTime gameTime)
        {
            if (this.ActiveList != null)
            {
                SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                foreach (DrawnActor2D currentUIObject in this.ActiveList)
                {
                    if ((currentUIObject.GetStatusType() & StatusType.Drawn) != 0) //if drawn flag is set
                    {
                        currentUIObject.Draw(gameTime, SpriteBatch);
                    }
                }
                SpriteBatch.End();
            }
        }

        protected virtual void HandleMouseOver(DrawnActor2D uiObject, GameTime gameTime)
        {
        }

        protected virtual void HandleMouseClick(DrawnActor2D uiObject, GameTime gameTime)
        {
        }

        protected virtual void HandleKeyboardInput()
        {
        }
    }
}