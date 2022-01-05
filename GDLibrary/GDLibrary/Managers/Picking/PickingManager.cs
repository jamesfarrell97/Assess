using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using Microsoft.Xna.Framework.Audio;

namespace GDLibrary
{
    public class PickingManager : PausableGameComponent
    {
        #region Statics
        protected static readonly string NoObjectSelectedText = "No object selected";
        protected static readonly float DefaultMinPickPlaceDistance = 10;
        protected static readonly float DefaultMaxPickPlaceDistance = 100;
        private static readonly int DefaultDistanceToTargetPrecision = 1;
        #endregion

        #region Properties
        public CameraManager CameraManager { get; }
        public ObjectManager ObjectManager { get; }
        public InputManagerParameters InputManagerParameters { get; }
        public PickingBehaviourType PickingBehaviourType { get; }
        #endregion

        #region Constructors
        public PickingManager(
            Game game, 
            EventDispatcher eventDispatcher, 
            StatusType statusType,
            CameraManager cameraManager,
            ObjectManager objectManager,
            InputManagerParameters inputManagerParameters, 
            PickingBehaviourType pickingBehaviourType
        ) : base(game, eventDispatcher, statusType) {
            this.CameraManager = cameraManager;
            this.ObjectManager = objectManager;
            this.InputManagerParameters = inputManagerParameters;
            this.PickingBehaviourType = pickingBehaviourType;
        }
        #endregion

        #region Methods
        protected override void ApplyUpdate(GameTime gameTime)
        {
            if (this.CameraManager.ActiveCamera != null)
            {
                Ray mouseRay = this.InputManagerParameters.MouseManager.GetMouseRay(this.CameraManager.ActiveCamera);
                if (PickObject(gameTime, this.ObjectManager.OpaqueDrawList, this.ObjectManager.TransparentDrawList, mouseRay))
                {

                }
            }

            base.ApplyUpdate(gameTime);
        }

        private bool PickObject(GameTime gameTime, List<Actor3D> opaqueActors, List<Actor3D> transparantActors, Ray mouseRay)
        {
            SortedList collisions = new SortedList();

            collisions.Clear();

            foreach (IActor actor in opaqueActors.Concat(transparantActors))
            {
                if (actor is CollidablePrimitiveObject)
                {
                    CollidablePrimitiveObject collidee = actor as CollidablePrimitiveObject;

                    //Is the mouse ray intersecting with a collidable object?
                    if (collidee.CollisionPrimitive.Intersects(mouseRay, out float? distance))
                    {
                        //IF the collidee is a breakable block, or an unbreakable, non-transparent block
                        //This will allow us to remove 'pick' through transparent, unbreakable blocks
                        if (collidee.GetID().Contains("Breakable") || (collidee.GetID().Contains("Unbreakable") && !collidee.GetID().Contains("Transparent")))
                        {
                            if (!collisions.ContainsKey(distance))
                            {
                                collisions.Add(distance, collidee);
                            }
                        }
                    }

                    //Reset the alpha and color values of all objects - see HandleIntersection()
                    collidee.EffectParameters.DiffuseColor = collidee.EffectParameters.OriginalColor;

                    if (!collidee.GetID().Contains("Transparent"))
                    {
                        collidee.EffectParameters.Alpha = collidee.EffectParameters.OriginalAlpha;
                    }
                }
            }

            //If a collision has taken place
            if (collisions.Count > 0)
            {
                HandleIntersection(gameTime, collisions.GetByIndex(0) as CollidablePrimitiveObject);
                NotifyIntersection(gameTime, collisions.GetByIndex(0) as CollidablePrimitiveObject);
                return true;
            }

            //If we were picking but now we're not then reset mouse
            NotifyNoIntersection();
            return false;
        }

        private void NotifyNoIntersection()
        {
            object[] additionalParameters = { NoObjectSelectedText };
            EventDispatcher.Publish(new EventData(EventActionType.OnNonePicked, EventCategoryType.ObjectPicking, new object[] { NoObjectSelectedText }));
        }

        private void NotifyIntersection(GameTime gameTime, CollidablePrimitiveObject collidee)
        {
            if (collidee.GetID().Contains("Breakable"))
            {
                float distanceToObject = (float) Math.Round(Vector3.Distance(this.CameraManager.ActiveCamera.Transform.Translation, collidee.Transform.Translation), DefaultDistanceToTargetPrecision);
                EventDispatcher.Publish(new EventData(EventActionType.OnObjectPicked, EventCategoryType.ObjectPicking, new object[] { collidee, distanceToObject }));
            }
            else
            {
                NotifyNoIntersection();
            }
        }

        private void HandleIntersection(GameTime gameTime, CollidablePrimitiveObject collidee)
        {
            //If the picking behaviour is set to remove
            if (this.PickingBehaviourType == PickingBehaviourType.PickAndRemove && StateManager.FinishedTracking)
            {
                //If the collidee is tagged as breakable
                if (collidee.GetID().Contains("Breakable"))
                {
                    //If the user has clicked to break this block
                    if (this.InputManagerParameters.MouseManager.IsLeftButtonClickedOnce())
                    {
                        //Create an audio emitter at this location
                        AudioEmitter audioEmitter = new AudioEmitter {
                            Position = (collidee as Actor3D).Transform.Translation,
                            Forward = this.CameraManager.ActiveCamera.Transform.Look,
                            Up = this.CameraManager.ActiveCamera.Transform.Up
                        };

                        //Publish sound event
                        EventDispatcher.Publish(new EventData(EventActionType.OnPlay, EventCategoryType.Sound3D, new object[] { "break", audioEmitter }));

                        //Remove the block
                        this.ObjectManager.Remove(collidee);
                    } 

                    //Otherwise, indicate to the user that this block is 'breakable'
                    else
                    {
                        //Make object transparant
                        collidee.EffectParameters.DiffuseColor = Color.AliceBlue;
                        collidee.EffectParameters.Alpha = 0.5f;
                    }
                }  
            }
        }

        protected override void HandleMouse(GameTime gameTime)
        {
        }

        protected override void HandleKeyboard(GameTime gameTime)
        {
        }

        protected override void HandleGamePad(GameTime gameTime)
        {
        }
        #endregion
    }
}