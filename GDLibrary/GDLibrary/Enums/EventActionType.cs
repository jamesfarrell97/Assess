﻿/*
Function: 		Enum to define event types generated by game entities e.g. menu, camera manager
Author: 		NMCG
Version:		1.0
Date Updated:	
Bugs:			None
Fixes:			None
*/

namespace GDLibrary
{
    public enum EventActionType : sbyte
    {
        //sent by audio, video
        OnPlay,
        OnPause,
        OnResume,
        OnStop,
        OnStopAll,

        //processed by many managers (incl. menu, sound, object, ui, physic) and video controller
        OnStart,
        OnRestart,
        OnVolumeUp,
        OnVolumeDown,
        OnVolumeSet,
        OnVolumeChange,
        OnMute,
        OnUnMute,
        OnExit,

        //sent by camera manager
        OnCameraSetActive,
        OnCameraCycle,

        //game state
        OnLose,
        OnWin,
        OnPickup,
        OnOpen,
        OnClose,

        //sent whenever we change the opacity of a drawn object - remember ObjectManager has two draw lists (opaque and transparent)
        OnOpaqueToTransparent,
        OnTransparentToOpaque,

        //sent when we want to add/remove an Actor from the game - see UIMouseObject::HandlePickedObject()
        OnAddActor,
        OnRemoveActor,

        //sent when we turn something (menu, controller, sound, debug) on/off
        OnToggle,

        //sent when a health change occurs - typically in the players state
        OnHealthDelta,
        OnHealthSet,

        //picking
        OnObjectPicked,
        OnNonePicked,
        OnAddActor2D,
        OnRemoveActor2D,
        OnTrackVolumeChange,
        OnClear,
        OnActive,
        OnReset,
        SetActive,
    }
}
