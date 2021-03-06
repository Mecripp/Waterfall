﻿
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waterfall.UI
{
  [KSPAddon(KSPAddon.Startup.Flight, false)]
  public class WaterfallUI : UIAppToolbarWindow
  {
    public static WaterfallUI Instance { get; private set; }
    #region GUI Variables
    private string windowTitle = "";
    Vector2 effectsScrollListPosition = Vector2.zero;
    Vector2 partsScrollListPosition = Vector2.zero;

    bool useControllers = false;
    float densityControllerValue = 0f;
    float throttleControllerValue = 0f;
    Vector2 randomControllerValue;
    float rcsControllerValue;

    #endregion

    #region GUI Widgets
    UICurveEditWindow curveEditWindow;
    UIModifierPopupWindow modifierPopupWindow;
    UIAddEffectWindow effectAddWindow;
    UIModifierWindow currentModWinForCurve;
    UIMaterialEditWindow materialEditWindow;
    UIColorPickerWindow colorPickWindow;
    UITexturePickerWindow texturePickWindow;
    string currentCurveTag;
    #endregion

    #region Vessel Data
    Vessel vessel;
    List<ModuleWaterfallFX> effectsModules = new List<ModuleWaterfallFX>();
    ModuleWaterfallFX selectedModule;
    List<UIEffectWidget> effectUIWidgets = new List<UIEffectWidget>();
    #endregion


    /// <summary>
    /// Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      windowTitle = "WaterfallFX Editor";
      base.InitUI();
    }

    protected override void Awake()
    {
      base.Awake();
      Instance = this;
    }

    protected override void Start()
    {
      base.Start();
      windowPos = new Rect(200f, 200f, 800f, 600f);
      GetVesselData();
    }

    /// <summary>
    /// Draw the UI
    /// </summary>
    protected override void Draw()
    {

      base.Draw();
      foreach (UIModifierWindow modWin in editWindows)
      {
        modWin.Draw();
      }
      if (curveEditWindow != null)
      {
        curveEditWindow.Draw();
      }
      if (materialEditWindow != null)
      {
        materialEditWindow.Draw();
      }
      if (modifierPopupWindow != null)
      {
        modifierPopupWindow.Draw();
      }
      if (effectAddWindow != null)
      {
        effectAddWindow.Draw();
      }
      if (colorPickWindow != null)
      {
        colorPickWindow.Draw();
      }
      if (texturePickWindow != null)
      {
        texturePickWindow.Draw();
      }
    }


    /// <summary>
    /// Draw the window
    /// </summary>
    /// <param name="windowId">window ID</param>
    protected override void DrawWindow(int windowId)
    {
      // Draw the header/tab controls
      DrawHeader();

      // Draw the parts list
      DrawPartsList();

      // Draw the effects list


      DrawEffectsList();
      GUI.DragWindow();
    }

    protected void DrawHeader()
    {

      GUILayout.BeginHorizontal();

      GUILayout.FlexibleSpace();
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

      GUILayout.BeginHorizontal();
      DrawControllers();
      DrawExporters();
      GUILayout.EndHorizontal();
    }


    protected void DrawControllers()
    {
      GUILayout.BeginHorizontal();

      useControllers = GUILayout.Toggle(useControllers, "Link to Editor", GUILayout.Width(150));
      GUILayout.BeginVertical();
      GUILayout.Label("<b>CONTROLLERS</b>");

      // 
      GUILayout.BeginHorizontal();
      GUILayout.Label("Throttle", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      throttleControllerValue = GUILayout.HorizontalSlider(throttleControllerValue, 0f, 1f, GUILayout.MaxWidth(120f));
      GUILayout.Label(throttleControllerValue.ToString("F2"), GUIResources.GetStyle("data_field"), GUILayout.MinWidth(60f));
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Atmosphere Depth", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      densityControllerValue = GUILayout.HorizontalSlider(densityControllerValue, 0f, 1f, GUILayout.MaxWidth(120f));
      GUILayout.Label(densityControllerValue.ToString("F2"), GUIResources.GetStyle("data_field"), GUILayout.MinWidth(60f));
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Label("Randomness Min/Max", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      string xValue = GUILayout.TextArea(randomControllerValue.x.ToString(), GUILayout.MaxWidth(60f));
      string yValue = GUILayout.TextArea(randomControllerValue.y.ToString(), GUILayout.MaxWidth(60f));

      float xParsed;
      float yParsed;
      if (float.TryParse(xValue, out xParsed) && float.TryParse(yValue, out yParsed))
      {
        if (xParsed != randomControllerValue.x || yParsed != randomControllerValue.y)
          randomControllerValue = new Vector2(xParsed, yParsed);
      }

      GUILayout.EndHorizontal();


      GUILayout.BeginHorizontal();
      GUILayout.Label("RCS Throttle", GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(160f));
      rcsControllerValue = GUILayout.HorizontalSlider(rcsControllerValue, 0f, 1f, GUILayout.MaxWidth(120f));
      GUILayout.Label(rcsControllerValue.ToString("F2"), GUIResources.GetStyle("data_field"), GUILayout.MinWidth(60f));
      GUILayout.EndHorizontal();

      GUILayout.EndVertical();
      GUILayout.EndHorizontal();
    }

    protected void DrawExporters()
    {
      GUILayout.BeginVertical();
      if (GUILayout.Button("Dump all to log"))
      {
        for (int i = 0; i < effectsModules.Count; i++)
        {
          Utils.Log(effectsModules[i].Export().ToString());
        }
      }
      if (GUILayout.Button("Dump selected to log"))
      {
        Utils.Log(selectedModule.Export().ToString());

      }
      if (GUILayout.Button("Copy selected to clipboard"))
      {
        GUIUtility.systemCopyBuffer = (selectedModule.Export().ToString());

      }
      GUILayout.EndVertical();
    }

    protected void DrawPartsList()
    {
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
      GUILayout.BeginVertical();
      GUILayout.Label("<b>FX MODULES</b>");
      partsScrollListPosition = GUILayout.BeginScrollView(partsScrollListPosition, GUILayout.ExpandWidth(true), GUILayout.Height(40f));

      GUILayout.BeginHorizontal();
      for (int i = 0; i < effectsModules.Count; i++)
      {

        if (GUILayout.Button($"{effectsModules[i].moduleID} ({effectsModules[i].FX.Count} Effects)", GUILayout.MaxWidth(250f)))
        {
          SelectFXModule(effectsModules[i]);
        }
      }
      GUILayout.EndHorizontal();

      GUILayout.EndScrollView();
      GUILayout.EndVertical();
      GUILayout.EndHorizontal();
    }


    protected void DrawEffectsList()
    {
      GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
      GUILayout.BeginHorizontal();
      GUILayout.Label("<b>EFFECTS</b>");
      GUILayout.FlexibleSpace();
      if (selectedModule != null)
      {
        if (GUILayout.Button("Add"))
        {
          OpenEffectAddWindow();
        }
      }
      GUILayout.EndHorizontal();


      effectsScrollListPosition = GUILayout.BeginScrollView(effectsScrollListPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MinHeight(350f));

      for (int i = 0; i < effectUIWidgets.Count; i++)
      {
        effectUIWidgets[i].Draw();
      }

      GUILayout.EndScrollView();
      GUILayout.EndVertical();
    }
    public void SelectFXModule(ModuleWaterfallFX fxMod)
    {
      selectedModule = fxMod;
      RefreshEffectList();
    }

    public void RefreshEffectList()
    {
      effectUIWidgets.Clear();
      foreach (WaterfallEffect fx in selectedModule.FX)
      {
        effectUIWidgets.Add(new UIEffectWidget(this, fx));
      }
    }
    public void GetVesselData()
    {
      vessel = FlightGlobals.ActiveVessel;
      effectsModules = new List<ModuleWaterfallFX>();
      if (vessel != null)
      {
        foreach (Part p in vessel.Parts)
        {
          ModuleWaterfallFX[] fxModules = p.GetComponents<ModuleWaterfallFX>();
          foreach (ModuleWaterfallFX fxModule in fxModules)
          {
            effectsModules.Add(fxModule);
          }
        }
        if (effectsModules.Count > 0)
        {
          SelectFXModule(effectsModules[0]);

        }

      }
    }
    protected List<UIModifierWindow> editWindows = new List<UIModifierWindow>();



    public void OpenModifierEditWindow(EffectModifier fxMod)
    {
      foreach (UIModifierWindow editWin in editWindows.ToList())
      {
        editWindows.Remove(editWin);
      }
      try
      {
        EffectColorModifier colMod = (EffectColorModifier)fxMod;
        if (colMod != null)
        {
          editWindows.Add(new UIColorModifierWindow(colMod, true));
        }

      }
      catch (InvalidCastException e) { }
      try
      {
        EffectScaleModifier scaleMod = (EffectScaleModifier)fxMod;
        if (scaleMod != null)
        {
          editWindows.Add(new UIScaleModifierWindow(scaleMod, true));
        }
      }
      catch (InvalidCastException e) { }
      try
      {
        EffectUVScrollModifier scrollMod = (EffectUVScrollModifier)fxMod;
        if (scrollMod != null)
        {
          editWindows.Add(new UIUVScrollModifierWindow(scrollMod, true));
        }
      }
      catch (InvalidCastException e) { }
      try
      {
        EffectFloatModifier floatMod = (EffectFloatModifier)fxMod;
        if (floatMod != null)
        {
          editWindows.Add(new UIFloatModifierWindow(floatMod, true));
        }
      }
      catch (InvalidCastException e) { }
      try
      {
        EffectPositionModifier posMod = (EffectPositionModifier)fxMod;
        if (posMod != null)
        {
          editWindows.Add(new UIPositionModifierWindow(posMod, true));
        }
      }
      catch (InvalidCastException e) { }
      try
      {
        EffectRotationModifier rotMod = (EffectRotationModifier)fxMod;
        if (rotMod != null)
        {
          editWindows.Add(new UIRotationModifierWindow(rotMod, true));
        }
      }
      catch (InvalidCastException e) { }
    }

    public UICurveEditWindow OpenCurveEditor(FloatCurve toEdit)
    {
      if (curveEditWindow != null)
      {
        curveEditWindow.ChangeCurve(toEdit);
      }
      else
      {
        curveEditWindow = new UICurveEditWindow(toEdit, true);
      }
      return curveEditWindow;
    }


    public UICurveEditWindow OpenCurveEditor(FloatCurve toEdit, UIModifierWindow modWin, string tag)
    {

      currentModWinForCurve = modWin;
      currentCurveTag = tag;
      if (curveEditWindow != null)
      {
        curveEditWindow.ChangeCurve(toEdit, modWin, tag);
      }
      else
      {
        curveEditWindow = new UICurveEditWindow(toEdit, modWin, tag, true);
      }
      return curveEditWindow;
    }

    public UIMaterialEditWindow OpenMaterialEditWindow(WaterfallModel mdl)
    {

      if (materialEditWindow != null)
      {
        materialEditWindow.ChangeMaterial(mdl);
      }
      else
      {
        materialEditWindow = new UIMaterialEditWindow(mdl, true);
      }
      return materialEditWindow;
    }
    public UIColorPickerWindow OpenColorEditWindow(Color c)
    {

      if (colorPickWindow != null)
      {
        Utils.Log("[WaterfallUI] Changing Color Picker target");
        colorPickWindow.ChangeColor(c);
      }
      else
      {
        Utils.Log("[WaterfallUI] Opening Color Picker");
        colorPickWindow = new UIColorPickerWindow(c, true);
      }
      return colorPickWindow;
    }

    public UITexturePickerWindow OpenTextureEditWindow(string t, string current)
    {

      if (texturePickWindow != null)
      {
        Utils.Log("[WaterfallUI] Changing Texture Picker target");
        texturePickWindow.ChangeTexture(t, current);
      }
      else
      {
        Utils.Log("[WaterfallUI] Opening Texture Picker");
        texturePickWindow = new UITexturePickerWindow(t, current, true);
      }
      return texturePickWindow;
    }

    public string GetTextureFromPicker()
    {
      if (texturePickWindow != null)
        return texturePickWindow.GetTexturePath();
      else
      {
        return "";
      }


    }
    public Color GetColorFromPicker()
    {
      if (colorPickWindow != null)
        return colorPickWindow.GetColor();
      else
      {
        return Color.black;
      }


    }
    public void CopyEffect(WaterfallEffect toCopy)
    {
      selectedModule.CopyEffect(toCopy);
      RefreshEffectList();
    }

    public void OpenEffectAddWindow()
    {
      if (effectAddWindow == null)
      {
        effectAddWindow = new UIAddEffectWindow(true);

      }
      effectAddWindow.SetAddMode(selectedModule);
    }
    public void OpenEffectDeleteWindow(WaterfallEffect toDelete)
    {
      if (effectAddWindow == null)
      {
        effectAddWindow = new UIAddEffectWindow(true);

      }
      effectAddWindow.SetDeleteMode(selectedModule, toDelete);
    }

    public void OpenEffectModifierAddWindow(WaterfallEffect fx)
    {
      if (modifierPopupWindow == null)
      {
        modifierPopupWindow = new UIModifierPopupWindow(true);

      }
      modifierPopupWindow.SetAddMode(fx);
    }
    public void OpenEffectModifierDeleteWindow(WaterfallEffect fx, EffectModifier toDelete)
    {
      if (modifierPopupWindow == null)
      {
        modifierPopupWindow = new UIModifierPopupWindow(true);

      }
      modifierPopupWindow.SetDeleteMode(fx, toDelete);
    }



    public void UpdateCurve(FloatCurve curve)
    {
      currentModWinForCurve.UpdateCurves(curve, currentCurveTag);
    }

    public void Update()
    {
      for (int i = 0; i < effectsModules.Count; i++)
      {

        for (int j = 0; j < effectsModules[i].Controllers.Count; j++)
        {
          effectsModules[i].SetControllerOverride(effectsModules[i].Controllers[j].name, useControllers);
          if (effectsModules[i].Controllers[j].linkedTo == "throttle")
          {
            effectsModules[i].SetControllerOverrideValue(effectsModules[i].Controllers[j].name, throttleControllerValue);
          }
          if (effectsModules[i].Controllers[j].linkedTo == "atmosphere_density")
          {
            effectsModules[i].SetControllerOverrideValue(effectsModules[i].Controllers[j].name, densityControllerValue);
          }
          if (effectsModules[i].Controllers[j].linkedTo == "rcs")
          {
            effectsModules[i].SetControllerOverrideValue(effectsModules[i].Controllers[j].name, rcsControllerValue);
          }
          if (effectsModules[i].Controllers[j].linkedTo == "random")
          {
            effectsModules[i].SetControllerOverrideValue(effectsModules[i].Controllers[j].name, randomControllerValue.x);
          }


        }
      }
      for (int i = 0; i < effectUIWidgets.Count; i++)
      {
        effectUIWidgets[i].Update();
      }

      if (colorPickWindow != null)
      {
        colorPickWindow.Update();
      }
      if (texturePickWindow != null)
      {
        texturePickWindow.Update();
      }
    }
  }

}