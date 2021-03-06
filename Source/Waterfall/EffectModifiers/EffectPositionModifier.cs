﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Waterfall
{
  /// <summary>
  /// Transform scale modifier
  /// </summary>
  public class EffectPositionModifier : EffectModifier
  {
    public FloatCurve xCurve;
    public FloatCurve yCurve;
    public FloatCurve zCurve;

    Vector3 basePosition;
    public EffectPositionModifier()
    {
      xCurve = new FloatCurve();
      yCurve = new FloatCurve();
      zCurve = new FloatCurve();
      modifierTypeName = "Position";
    }
    public EffectPositionModifier(ConfigNode node) { Load(node); }

    public override void Load(ConfigNode node)
    {
      base.Load(node);
      xCurve = new FloatCurve();
      yCurve = new FloatCurve();
      zCurve = new FloatCurve();
      xCurve.Load(node.GetNode("xCurve"));
      yCurve.Load(node.GetNode("yCurve"));
      zCurve.Load(node.GetNode("zCurve"));
      modifierTypeName = "Position";
    }
    public override ConfigNode Save()
    {
      ConfigNode node = base.Save();

      node.name = WaterfallConstants.PositionModifierNodeName;

      node.AddNode(Utils.SerializeFloatCurve("xCurve", xCurve));
      node.AddNode(Utils.SerializeFloatCurve("yCurve", yCurve));
      node.AddNode(Utils.SerializeFloatCurve("zCurve", zCurve));
      return node;
    }

    public override void Init(WaterfallEffect parentEffect)
    {
      base.Init(parentEffect);
      basePosition = xforms[0].localPosition;
    }
    protected override void ApplyReplace(List<float> strengthList)
    {
      float strength = strengthList[0];
      for (int i = 0; i < xforms.Count; i++)
      {
        xforms[i].localPosition = new Vector3(xCurve.Evaluate(strength) + randomValue, yCurve.Evaluate(strength) + randomValue, zCurve.Evaluate(strength) + randomValue);
      }
    }
    protected override void ApplyAdd(List<float> strengthList)
    {
      float strength = strengthList[0];
      for (int i = 0; i < xforms.Count; i++)
      {
        xforms[i].localPosition = basePosition + new Vector3(xCurve.Evaluate(strength) + randomValue, yCurve.Evaluate(strength) + randomValue, zCurve.Evaluate(strength) + randomValue);
      }
    }
    protected override void ApplySubtract(List<float> strengthList)
    {
      float strength = strengthList[0];
      for (int i = 0; i < xforms.Count; i++)
      {
        xforms[i].localPosition = basePosition - new Vector3(xCurve.Evaluate(strength) + randomValue, yCurve.Evaluate(strength) + randomValue, zCurve.Evaluate(strength) + randomValue);
      }
    }
    protected override void ApplyMultiply(List<float> strengthList)
    {
      float strength = strengthList[0];
      for (int i = 0; i < xforms.Count; i++)
      {
        xforms[i].localPosition = new Vector3(basePosition.x * xCurve.Evaluate(strength) + randomValue, basePosition.y * yCurve.Evaluate(strength) + randomValue, basePosition.z * zCurve.Evaluate(strength) + randomValue);
      }
    }
    
  }

}
