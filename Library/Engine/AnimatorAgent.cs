﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AnimatorAgent : Component
{
    public bool enabled
    {
        get
        {
            return InternalCalls.Animator_GetEnabled(entity);
        }
        set
        {
            InternalCalls.Animator_EnableDisable(entity, value);
        }
    }

    public void SetBool(string conditionName, bool value)
    {
        InternalCalls.Animator_SetBool(entity, conditionName, value);
    }

    public bool GetBool(string conditionName)
    {
        return InternalCalls.Animator_GetBool(entity, conditionName);
    }

    public void SetFloat(string conditionName, float value)
    {
        InternalCalls.Animator_SetFloat(entity, conditionName, value);
    }

    public float GetFloat(string conditionName)
    {
        return InternalCalls.Animator_GetFloat(entity, conditionName);
    }

    public void SetTrigger(string conditionName)
    {
        InternalCalls.Animator_SetTrigger(entity, conditionName);
    }

    public void ResetTrigger(string conditionName)
    {
        InternalCalls.Animator_ResetTrigger(entity, conditionName);
    }

    public void PlayAnimState(string stateName, bool toBlend = false, float blendduration = 0.5f, bool staticBlend = false)
    {
        InternalCalls.Animator_PlayAnim(entity, stateName, toBlend, blendduration, staticBlend);
    }

    public void SetAnimSpeed(string stateName, float speed)
    {
        InternalCalls.Animator_SetAnimStateSpeed(entity, stateName, speed);
    }

    public void Pause()
    {
        InternalCalls.Animator_Pause(entity);
    }

    public void Stop()
    {
        InternalCalls.Animator_Stop(entity);
    }

    public void Play()
    {
        InternalCalls.Animator_Play(entity);
    }
}
