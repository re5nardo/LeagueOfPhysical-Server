﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

public class EntityTransformSynchronization : MonoComponentBase, ISynchronizable
{
    #region ISynchronizable
    public bool Enable { get; set; } = true;
    public bool HasCoreChange => LastSendSnap == null ? true : !LastSendSnap.EqualsCore(CurrentSnap.Set(this));
    public bool IsDirty => isDirty || LastSendSnap == null ? true : !LastSendSnap.EqualsValue(CurrentSnap.Set(this));
    #endregion

    private bool isDirty = false;
    private int WaitingInterval => (int)(1 / Game.Current.TickInterval);
    private EntityTransformSnap LastSendSnap { get; set; } = new EntityTransformSnap();
    private EntityTransformSnap CurrentSnap { get; set; } = new EntityTransformSnap();
    private bool IsValidToSend => Enable;

    public override void OnAttached(IEntity entity)
    {
        base.OnAttached(entity);

        TickPubSubService.AddSubscriber("TickEnd", OnTickEnd);
    }

    public override void OnDetached()
    {
        base.OnDetached();

        TickPubSubService.RemoveSubscriber("TickEnd", OnTickEnd);
    }

    private void OnTickEnd(int tick)
    {
        UpdateSynchronizable();
    }

    #region ISynchronizable
    public void SetDirty()
    {
        isDirty = true;
    }

    public ISnap GetSnap()
    {
        return new EntityTransformSnap(Entity);
    }

    public void UpdateSynchronizable()
    {
        if (IsValidToSend && IsDirty)
        {
            if (HasCoreChange || WaitingInterval == -1 || Game.Current.CurrentTick - LastSendSnap.Tick > WaitingInterval)
            {
                SendSynchronization();
            }
        }
    }

    public void SendSynchronization()
    {
        SynchronizationManager.SendSnap(LastSendSnap.Set(this));

        isDirty = false;
    }

    public void OnReceiveSynchronization(ISnap snap)
    {
        throw new NotImplementedException();
    }
    #endregion
}
