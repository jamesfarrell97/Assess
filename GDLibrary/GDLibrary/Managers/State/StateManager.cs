/*
Function: 		The "brain" for dynamic events in the game. This class listens for events and responds with changes to the game.
                For example, if the player wins/loses the game then this class will determine what happens as a consequence.
                It may, in this case, show certain UITextObjects, play sounds, reset controllers, generate new collidable objects.
Author: 		NMCG
Version:		1.0
Date Updated:	16/11/17
Bugs:			
Fixes:			None
*/

using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class StateManager : PausableGameComponent
    {
        #region Fields
        private static int currentGoal;
        #endregion

        #region Properties
        public static int CurrentLevel { get; set; }
        public static int GoalsRemaining { get; set; }

        public static int CurrentGoalInLoop { get => currentGoal; set => currentGoal = (value % 4); }
        public static int CurrentTextInLoop { get; set; }

        public static bool IsFalling { get; set; }
        public static bool IsCameraMoving { get; set; }
        public static bool IsCharacterMoving { get; set; }
        public static bool InProximityOfATrigger { get; set; }
        public static bool InProximityOfAnItem { get; set; }
        public static bool InProximityOfAGate { get; set; }
        public static bool ContinueClicked { get; set; }
        public static bool ResumeClicked { get; set; }
        public static bool RestartClicked { get; set; }
        public static bool PlayerDied { get; set; }
        public static bool LevelClear { get; set; }
        public static bool FinishedTracking { get; set; }
        public static bool IncompletePlayed { get; internal set; }
        #endregion

        #region Constructor
        public StateManager(
            Game game,
            EventDispatcher eventDispatcher,
            StatusType statusType
        ) : base(game, eventDispatcher, statusType) {
            DefaultState();
        }
        #endregion

        #region Event Handling
        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            eventDispatcher.LevelChanged += EventDispatcher_LevelChanged;
        }

        protected override void EventDispatcher_MenuChanged(EventData eventData)
        {
            //Did the event come from the main menu and is it a start game event
            if (eventData.EventType == EventActionType.OnStart)
            {
                //Turn on update and draw i.e. hide the menu
                this.StatusType = StatusType.Update | StatusType.Drawn;
            }

            //Did the event come from the main menu and is it a pause game event
            else if (eventData.EventType == EventActionType.OnPause)
            {
                //Turn off update and draw i.e. show the menu since the game is paused
                this.StatusType = StatusType.Off;
            }
        }

        private void EventDispatcher_LevelChanged(EventData eventData)
        {
            //Did the event come from the game being won?
            if (eventData.EventType == EventActionType.OnClear)
            {
                CurrentLevel++;
            }

            if (eventData.EventType == EventActionType.OnReset)
            {
                //Do nothing
            }
        }
        #endregion

        #region Methods
        public static void DefaultState()
        {
            IsFalling = false;
            IsCameraMoving = false;
            IsCharacterMoving = false;
            InProximityOfATrigger = false;
            InProximityOfAnItem = false;
            InProximityOfAGate = false;
            ContinueClicked = false;
            RestartClicked = false;
            PlayerDied = false;
            LevelClear = false;
        }
        #endregion
    }
}