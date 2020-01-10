using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace GDLibrary
{
    public class TextboxManager : PausableDrawableGameComponent
    {
        #region Properties
        public Dictionary<string, List<DrawnActor2D>> UiDictionary { get; }
        public List<DrawnActor2D> ActiveList { get; set; } = null;
        public SpriteBatch SpriteBatch { get; set; }
        public string TextboxText { get; set; }
        #endregion

        #region Constructors
        public TextboxManager(
            Game game,
            SpriteBatch spriteBatch,
            EventDispatcher eventDispatcher,
            StatusType statusType,
            string textboxText
        ) : base(game, eventDispatcher, statusType) {
            this.UiDictionary = new Dictionary<string, List<DrawnActor2D>>();
            this.SpriteBatch = spriteBatch;
            this.TextboxText = textboxText;
        }
        #endregion

        #region Event Handling
        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.TextboxChanged += EventDispatcher_TextboxChanged;
            base.RegisterForEventHandling(eventDispatcher);
        }

        protected virtual void EventDispatcher_TextboxChanged(EventData eventData)
        {
            if (eventData.EventType == EventActionType.OnPause)
            {
                this.StatusType = StatusType.Off;
            }

            else if (eventData.EventType == EventActionType.SetActive)
            {
                this.TextboxText = eventData.AdditionalParameters[0] as string;
            }
        }
        #endregion

        #region Methods
        private void ClearTextbox()
        {
            this.TextboxText = "";
        }

        public void Add(string sceneID, DrawnActor2D actor)
        {
            if (this.UiDictionary.ContainsKey(sceneID))
            {
                this.UiDictionary[sceneID].Add(actor);
            }
            else
            {
                List<DrawnActor2D> newList = new List<DrawnActor2D> {
                    actor
                };

                this.UiDictionary.Add(sceneID, newList);
            }

            //If the user forgets to set the active list then set to the sceneID of the last added item
            SetActiveList(sceneID);
        }

        public DrawnActor2D Find(string sceneID, Predicate<DrawnActor2D> predicate)
        {
            if (this.UiDictionary.ContainsKey(sceneID))
            {
                return this.UiDictionary[sceneID].Find(predicate);
            }
            return null;
        }

        public bool Remove(string sceneID, Predicate<DrawnActor2D> predicate)
        {
            DrawnActor2D foundUIObject = Find(sceneID, predicate);

            if (foundUIObject != null)
                return this.UiDictionary[sceneID].Remove(foundUIObject);

            return false;
        }

        //Return all the actor2D objects associated with the "health ui" or "inventory ui"
        public List<DrawnActor2D> FindAllBySceneID(string sceneID)
        {
            if (this.UiDictionary.ContainsKey(sceneID))
            {
                return this.UiDictionary[sceneID];
            }
            return null;
        }

        public bool SetActiveList(string sceneID)
        {
            if (this.UiDictionary.ContainsKey(sceneID))
            {
                this.ActiveList = this.UiDictionary[sceneID];
                return true;
            }

            return false;
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            if (this.ActiveList != null)
            {
                //Update all the updateable menu items (e.g. make buttons pulse etc)
                foreach (DrawnActor2D currentUIObject in this.ActiveList)
                {
                    //If update flag is set
                    if ((currentUIObject.GetStatusType() & StatusType.Update) != StatusType.Update)
                    {
                        (currentUIObject as UITextObject).Text = this.TextboxText;
                        currentUIObject.Update(gameTime);
                    }
                }
            }
        }

        protected override void ApplyDraw(GameTime gameTime)
        {
            if (this.ActiveList != null)
            {
                SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
                foreach (DrawnActor2D currentUIObject in this.ActiveList)
                {
                    if ((currentUIObject.GetStatusType() & StatusType.Drawn) != 0) //if drawn flag is set
                    {
                        currentUIObject.Draw(gameTime, SpriteBatch);
                        HandleMouseOver(currentUIObject, gameTime);
                    }
                }
                SpriteBatch.End();
            }
        }

        protected virtual void HandleMouseOver(DrawnActor2D uiObject, GameTime gameTime)
        {
            //developer implements in subclass of MenuManager - see MyMenuManager.cs
        }

        protected virtual void HandleMouseClick(DrawnActor2D uiObject, GameTime gameTime)
        {
            //developer implements in subclass of MenuManager - see MyMenuManager.cs
        }
        #endregion
    }
}