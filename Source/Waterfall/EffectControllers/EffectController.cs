﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Waterfall
{

  /// <summary>
  /// A generic effect controller
  /// </summary>
  public class WaterfallController
  {
    // 
    public string name = "unnamedController";
    public string linkedTo = "unnamedController";
    protected bool overridden = false;
    protected float overrideValue = 0.0f;
    protected float value = 0.0f;
    protected ModuleWaterfallFX parentModule;

    /// <summary>
    /// Get the value of the controller. 
    /// </summary>
    /// <returns></returns>
    public virtual List<float> Get() {
      if (overridden)
        return new List<float>() { overrideValue };
      else
        return new List<float>() { 0f };
    }

    /// <summary>
    ///  Initialzies the controller
    /// </summary>
    /// <param name="host"></param>
    public virtual void Initialize(ModuleWaterfallFX host) {
      parentModule = host;
    }
    /// <summary>
    /// Sets the value of the controller
    /// </summary>
    /// <param name="mode"></param>
    public virtual void Set(float newValue)
    {
      value = newValue;
    }

    /// <summary>
    /// Sets whether this controller is overridden, likely controlled by the UI
    /// </summary>
    /// <param name="mode"></param>
    public virtual void SetOverride(bool mode)
    {
      overridden = mode;
    }
    /// <summary>
    /// Sets the override value, not controlled by the game, likely an editor UI
    /// </summary>
    /// <param name="value"></param>
    public virtual void SetOverrideValue(float value)
    {
      overrideValue = value;
    }
  }

  

  
}
