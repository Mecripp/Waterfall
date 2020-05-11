﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Waterfall;

namespace Waterfall.UI
{
  public enum ModifierPopupMode
  {
    Add,
    Delete
  }
  public class UIModifierPopupWindow : UIPopupWindow
  {

    protected string windowTitle = "";
    ModifierPopupMode windowMode;
    EffectModifier modifier;
    WaterfallEffect effect;
    string controller = "throttle";
    string transformName = "writeMe";
    string newModifierName = "newModifier";
    string[] modifierTypes = new string[] { "Position", "Rotation", "Scale", "UV Scroll", "Material Color", "Material Float"};
    int modifierFlag = 0;

    int transformFlag = 0;
    string[] transformOptions;

    public UIModifierPopupWindow( bool show) : base(show)
    {
      WindowPosition = new Rect(Screen.width / 2, Screen.height / 2f, 750, 300);

    }

    public void SetDeleteMode(WaterfallEffect fx, EffectModifier mod)
    {
      showWindow = true;
      modifier = mod;
      effect = fx;
      windowMode = ModifierPopupMode.Delete;
    }

    public void SetAddMode(WaterfallEffect fx)
    {
      showWindow = true;
      effect = fx;
      windowMode = ModifierPopupMode.Add;

      Transform[] xFormOptions = fx.GetModelTransform().GetComponentsInChildren<Transform>();
      transformOptions = new string[xFormOptions.Length];
      for (int i=0;i < xFormOptions.Length; i++)
      {
        transformOptions[i] = xFormOptions[i].name;
      }
    }

    protected override void InitUI()
    {
      if (windowMode == ModifierPopupMode.Add)
        windowTitle = "Add new Modifier";
      if (windowMode == ModifierPopupMode.Delete)
        windowTitle = "Confirm Delete";
      base.InitUI();


    }
    

    protected override void DrawWindow(int windowId)
    {
      // Draw the header/tab controls
      DrawTitle();
      if (windowMode == ModifierPopupMode.Add)
        DrawAdd();
      if (windowMode == ModifierPopupMode.Delete)
        DrawDelete();
       GUI.DragWindow();

    }

    protected void DrawTitle()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label(windowTitle, GUIResources.GetStyle("window_header"), GUILayout.MaxHeight(26f), GUILayout.MinHeight(26f), GUILayout.MinWidth(350f));

      GUILayout.FlexibleSpace();

      Rect buttonRect = GUILayoutUtility.GetRect(22f, 22f);
      GUI.color = resources.GetColor("cancel_color");
      if (GUI.Button(buttonRect, "", GUIResources.GetStyle("button_cancel")))
      {
        ToggleWindow();
      }

      GUI.DrawTextureWithTexCoords(buttonRect, GUIResources.GetIcon("cancel").iconAtlas, GUIResources.GetIcon("cancel").iconRect);
      GUI.color = Color.white;
      GUILayout.EndHorizontal();
    }

   protected void DrawDelete()
    {
      GUILayout.Label($"Are you sure you want to delete {modifier.fxName}?");
      GUILayout.BeginHorizontal();
      if(GUILayout.Button("Yes"))
      {
        effect.RemoveModifier(modifier);
        showWindow = false;
      }
      if (GUILayout.Button("No"))
      {
        showWindow = false;
      }
      GUILayout.EndHorizontal();
    }
    protected void DrawAdd()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Modifier name");
      newModifierName = GUILayout.TextArea(newModifierName);
      GUILayout.EndHorizontal();
      GUILayout.Label("Modifier type");
      modifierFlag = GUILayout.SelectionGrid(modifierFlag, modifierTypes, Mathf.Min(modifierTypes.Length,4), GUIResources.GetStyle("radio_text_button"));
      GUILayout.Label("Target transform name");
      transformFlag = GUILayout.SelectionGrid(transformFlag, transformOptions, Mathf.Min(transformOptions.Length,2), GUIResources.GetStyle("radio_text_button"));
      GUILayout.BeginHorizontal();
      GUILayout.Label("Controller name");
      controller = GUILayout.TextArea(controller);
      GUILayout.EndHorizontal();
      transformName = transformOptions[transformFlag];
      if (GUILayout.Button("Add"))
      {
       
        effect.AddModifier(CreateNewModifier());
        showWindow = false;
      }
      if (GUILayout.Button("Cancel"))
      {
        showWindow = false;
      }
    }
    
    EffectModifier CreateNewModifier()
    {
      if (modifierTypes[modifierFlag] == "Position")
      {
        EffectPositionModifier newMod = new EffectPositionModifier();
        newMod.transformName = transformName;
        newMod.controllerName = controller;
        return newMod;
      }
      else if (modifierTypes[modifierFlag] == "Rotation")
      {
        EffectRotationModifier newMod = new EffectRotationModifier();
        newMod.transformName = transformName;
        newMod.controllerName = controller;
        return newMod;
      }
      else if (modifierTypes[modifierFlag] == "Scale")
      {
        EffectScaleModifier newMod = new EffectScaleModifier();
        newMod.transformName = transformName;
        newMod.controllerName = controller;
        return newMod;
      }
      else if (modifierTypes[modifierFlag] == "UV Scroll")
      {
        EffectUVScrollModifier newMod = new EffectUVScrollModifier();
        newMod.transformName = transformName;
        newMod.controllerName = controller;
        return newMod;
      }
      else if (modifierTypes[modifierFlag] == "Material Color")
      {
        EffectColorModifier newMod = new EffectColorModifier();
        newMod.transformName = transformName;
        newMod.controllerName = controller;
        return newMod;
      }
      else if (modifierTypes[modifierFlag] == "Material Float")
      {
        EffectFloatModifier newMod = new EffectFloatModifier();
        newMod.transformName = transformName;
        newMod.controllerName = controller;
        return newMod;
      }
      else
      {
        return null;
      }
    }
  }
}
