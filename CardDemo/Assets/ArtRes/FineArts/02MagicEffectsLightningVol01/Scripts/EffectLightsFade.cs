// (c) Copyright 2013 Luke Light&Magic. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animation))] // 确保对象总是有Animation组件
public class EffectLightsFade : MonoBehaviour {
  public float totalTime;
  public float fadeInTime;
  public float fadeOutTime;

  private Light dynamicLight;

  void Awake() {
    dynamicLight = GetComponent<Light>();
    if (dynamicLight != null) {
      float intensity = dynamicLight.intensity;
      Animation animLight = GetComponent<Animation>();
      
      // 如果已经有这个动画片段，就不要重复创建
      if (animLight.GetClip("light_fade") != null) return;

      if (fadeInTime < 0) fadeInTime = 0;
      if (fadeInTime > totalTime) fadeInTime = totalTime;
      if (fadeOutTime < 0) fadeOutTime = 0;
      if (fadeOutTime > totalTime) fadeOutTime = totalTime;
      if (fadeInTime + fadeOutTime > totalTime) fadeInTime = 0;

      List<Keyframe> keyList = new List<Keyframe>();
      if (fadeInTime != 0) {
        AnimationCurve linearCurve = AnimationCurve.Linear(0, 0, fadeInTime, intensity);
        keyList.Add(linearCurve[0]);
        keyList.Add(linearCurve[1]);
      }
      if (fadeOutTime != 0) {
        AnimationCurve linearCurve = AnimationCurve.Linear(totalTime - fadeOutTime, intensity, totalTime, 0);
        if (fadeInTime == 0 || fadeInTime != totalTime - fadeOutTime)
          keyList.Add(linearCurve[0]);
        else {
          Keyframe key = keyList[1];
          key.outTangent = linearCurve[0].outTangent;
          keyList[1] = key;
        }
        keyList.Add(linearCurve[1]);
      }
      AnimationCurve curve = new AnimationCurve(keyList.ToArray());
      AnimationClip clip = new AnimationClip();
      
      // 核心修复：将动画片段标记为 Legacy
      clip.legacy = true;
      
      clip.SetCurve("", typeof(Light), "m_Intensity", curve);
      
      // 修复：正确添加并播放动画
      clip.name = "light_fade";
      animLight.AddClip(clip, clip.name);
      animLight.Play(clip.name);
    }
	}

  void Reset() {
    totalTime = 1;
    fadeInTime = 0;
    fadeOutTime = 1;
  }
}
