using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;

public class MonoEntitySynchronization : MonoComponentBase, ISynchronizableComposite
{
    #region ISynchronizable
    public ISynchronizable Parent { get; set; } = null;
    public List<ISynchronizable> Children { get; } = new List<ISynchronizable>();
    public bool Enable { get; set; } = true;
    public bool EnableInHierarchy => Parent == null ? Enable : Parent.EnableInHierarchy && Enable;
    public bool HasCoreChange => LastSendSnap == null ? true : !LastSendSnap.EqualsCore(CurrentSnap.Set(this));
    public bool IsDirty => isDirty || LastSendSnap == null ? true : !LastSendSnap.EqualsValue(CurrentSnap.Set(this));
    #endregion

    private bool isDirty = false;
    private int WaitingInterval => -1;
    private MonoEntitySnap LastSendSnap { get; set; } = new MonoEntitySnap();
    private MonoEntitySnap CurrentSnap { get; set; } = new MonoEntitySnap();
    private bool IsValidToSend => EnableInHierarchy;

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

        Children.ForEach(child => child.SetDirty());
    }

    public ISnap GetSnap()
    {
        MonoEntitySnap monoEntitySnap = new MonoEntitySnap(Entity as Entity.MonoEntityBase);

        Children.ForEach(child => monoEntitySnap.snaps.Add(child.GetSnap()));

        return monoEntitySnap;
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

        Children.ForEach(child => child.UpdateSynchronizable());
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

    public void Reconcile(ISnap snap)
    {
        throw new NotImplementedException();
    }

    public void Add(ISynchronizable child)
    {
        Children.Add(child);

        child.Parent = this;
    }

    public void Remove(ISynchronizable child)
    {
        Children.Remove(child);

        child.Parent = null;
    }
    #endregion
}
