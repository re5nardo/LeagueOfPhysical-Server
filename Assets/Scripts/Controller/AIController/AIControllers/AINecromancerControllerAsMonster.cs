using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using Entity;
using GameFramework;

public class AINecromancerControllerAsMonster : AIControllerBase
{
    private const int MAX_TICK_INDEX = 30;

    private int m_nTickIndex = -1;

    private string m_strDefaultBehaviorPath = "ExternalBehaviorTree/MonsterBasicBehavior";

#region ControllerBase
    protected override void OnPossessed()
    {
        m_BehaviorTree = Entity.gameObject.AddComponent<BehaviorTree>();
        m_BehaviorTree.ExternalBehavior = Resources.Load<ExternalBehaviorTree>(m_strDefaultBehaviorPath);
        m_BehaviorTree.RestartWhenComplete = true;

        m_nTickIndex = Random.Range(0, MAX_TICK_INDEX);
    }

    protected override void OnUnPossessed()
    {
        if(m_BehaviorTree != null)
        {
            Destroy(m_BehaviorTree);
            m_BehaviorTree = null;
        }

        m_nTickIndex = -1;
    }

    protected override void UpdateBody()
    {
		bool valid = true;
		if(Entity is Character)
		{
			valid = (Entity as Character).IsAlive;
		}

		if (Time.frameCount % MAX_TICK_INDEX == m_nTickIndex && valid)
            BehaviorManager.instance.Tick(m_BehaviorTree);
    }
#endregion

#region Combat Behavior
    //    private IBehaviourTreeNode CreateCombatTree()
    //    {
    //        return new BehaviourTreeBuilder().Sequence("combat-sequence")
    //            .Condition("Condition_CheckNearEnemy", Condition_CheckNearEnemy)
    //            .Splice(Selector_Fight())
    //            .End()
    //            .Build();
    //    }
    //
    //    private bool Condition_CheckNearEnemy(TimeData t)
    //    {
    //        List<EntityBase> entities = EntityManager.Instance.GetNearEntities(m_targetEntity.GetPosition(), 10);;
    //
    //        return entities.Exists(x => x.GetEntityID() != m_targetEntity.GetEntityID());
    //    }
    //
    //    private IBehaviourTreeNode Selector_Fight()
    //    {
    //        return new BehaviourTreeBuilder().Selector("Selector_Fight")
    //            .Sequence("Sequence_Flee")
    //            .Condition("Condition_CheckAttackCoolTime", Condition_CheckAttackCoolTime)
    //            .Do("Do_Flee", Do_Flee)
    //            .End()
    //            .Do("Do_Attack", Do_Attack)
    //            .End()
    //            .Build();
    //    }
    //
    //    private bool Condition_CheckAttackCoolTime(TimeData t)
    //    {
    //        return m_Character.GetWhirlwindCoolTime() > 0;
    //    }
    //
    //    private BehaviourTreeStatus Do_Flee(TimeData t)
    //    {
    //        List<Character> listEnemy = WhirlwindGameRoom.Instance.GetCharacters(m_Character.GetPosition(), SIGHT).FindAll(x => x.GetID() != m_Character.GetID());
    //
    //        float fleeAngle = 0;
    //        if(listEnemy.Count == 0)
    //        {
    //            return BehaviourTreeStatus.Failure;
    //        }
    //        else if(listEnemy.Count == 1)
    //        {
    //            fleeAngle = Util.Math.FindDegree(new Vector2(listEnemy[0].GetPosition().x, listEnemy[0].GetPosition().z)) + 180;
    //        }
    //        else
    //        {
    //            List<KeyValuePair<float, Character>> listAngle = new List<KeyValuePair<float, Character>>();
    //            foreach(Character enemy in listEnemy)
    //            {
    //                float angle = Util.Math.FindDegree(new Vector2(enemy.GetPosition().x, enemy.GetPosition().z));
    //
    //                listAngle.Add(new KeyValuePair<float, Character>(angle, enemy));
    //            }
    //
    //            listAngle.Sort((x, y) => x.Key.CompareTo(y.Key));
    //
    //            float gap = 0;
    //            float best_gap = 0;
    //            int best = 0;
    //            for(int i = 0; i < listAngle.Count; ++i)
    //            {
    //                if(i == 0)
    //                {
    //                    gap = listAngle[i].Key + (360f - listAngle[listAngle.Count - 1].Key);
    //                }
    //                else
    //                {
    //                    gap = listAngle[i].Key - listAngle[i - 1].Key;
    //                }
    //
    //                if(best_gap < gap)
    //                {
    //                    best_gap = gap;
    //                    best = i;
    //                }
    //            }
    //
    //            if(best == 0)
    //            {
    //                fleeAngle = (listAngle[best].Key + listAngle[listAngle.Count - 1].Key) * 0.5f;
    //            }
    //            else
    //            {
    //                fleeAngle = (listAngle[best].Key + listAngle[best - 1].Key) * 0.5f;
    //            }
    //        }
    //
    //        //  flee
    //        float flee_x = Mathf.Sin(Mathf.Deg2Rad * fleeAngle);
    //        float flee_z = Mathf.Cos(Mathf.Deg2Rad * fleeAngle);
    //
    //        Vector3 dir = new Vector3(flee_x, 0, flee_z);
    //        Vector3 ve3Dest = m_Character.GetPosition() + dir.normalized * 6f;
    //
    //        TryRotationAndMove(ve3Dest);
    //
    //        return BehaviourTreeStatus.Running;
    //    }
    //
    //    private BehaviourTreeStatus Do_Attack(TimeData t)
    //    {
    //        if(m_Character.GetComponent<Behavior.Whirlwind>() == null)
    //        {
    //            m_Character.Whirlwind();
    //        }
    //
    //        List<Character> listEnemy = WhirlwindGameRoom.Instance.GetCharacters(m_Character.GetPosition(), SIGHT).FindAll(x => x.GetID() != m_Character.GetID());
    //
    //        float min_distance = float.MaxValue;
    //        Character target = null;
    //
    //        foreach(Character enemy in  listEnemy)
    //        {
    //            float distance = (m_Character.GetPosition() - enemy.GetPosition()).sqrMagnitude;
    //
    //            if(distance < min_distance)
    //            {
    //                min_distance = distance;
    //                target = enemy;
    //            }
    //        }
    //
    //        TryRotationAndMove(target.GetPosition());
    //
    //        return BehaviourTreeStatus.Running;
    //    }
#endregion

#region Item Behavior
    //    private IBehaviourTreeNode CreateItemTree()
    //    {
    //        return new BehaviourTreeBuilder().Sequence("item-sequence")
    //            .Condition("Condition_CheckNearItem", Condition_CheckNearItem)
    //            .Do("Do_Item", Do_Item)
    //            .End()
    //            .Build();
    //    }
    //
    //    private bool Condition_CheckNearItem(TimeData t)
    //    {
    //        List<ExpPotion> listExpPotion = WhirlwindGameRoom.Instance.GetExpPotions(m_Character.GetPosition(), SIGHT);
    //
    //        return listExpPotion.Count > 0;
    //    }
    //
    //    private BehaviourTreeStatus Do_Item(TimeData t)
    //    {
    //        List<ExpPotion> listExpPotion = WhirlwindGameRoom.Instance.GetExpPotions(m_Character.GetPosition(), SIGHT);
    //
    //        float min_dist = float.MaxValue;
    //        ExpPotion nearestExpPotion = null;
    //
    //        foreach(ExpPotion expPotion in listExpPotion)
    //        {
    //            float distance = (m_Character.GetPosition() - expPotion.GetPosition()).sqrMagnitude;
    //
    //            if(distance < min_dist)
    //            {
    //                nearestExpPotion = expPotion;
    //
    //                min_dist = distance;
    //            }
    //        }
    //
    //        TryRotationAndMove(nearestExpPotion.GetPosition());
    //
    //        return BehaviourTreeStatus.Running;
    //    }
#endregion

#region Idle Behavior
    //private IBehaviourTreeNode CreateIdleTree()
    //{
    //    return new BehaviourTreeBuilder().Sequence("idle-sequence")
    //        .Do("Do_Idle", Do_Idle)
    //        .End()
    //        .Build();
    //}

    //private BehaviourTreeStatus Do_Idle(TimeData t)
    //{
    //    Behavior.Idle idle = m_Entity.GetComponent<Behavior.Idle>();

    //    if(idle != null)
    //    {
    //        if(idle.GetElapsedTime() > 2f)
    //        {
    //            while(true)
    //            {
    //                Vector3 newPosition = new Vector3(m_Entity.GetPosition().x + Random.Range(-20, 20), 0, m_Entity.GetPosition().z + Random.Range(-20, 20));

    //                if(TryRotationAndMove(newPosition))
    //                    break;
    //            }
    //        }
    //    }

    //    return BehaviourTreeStatus.Running;
    //}
#endregion

    private bool TryRotationAndMove(Vector3 position)
    {
        Rect mapRect = LOP.Game.Current.GetMapRect();

        if(position.x > mapRect.xMax - 5 || position.x < mapRect.xMin + 5 || position.z > mapRect.yMax - 5 || position.z < mapRect.yMin + 5)
        {
            return false;
        }

		//Entity.Move(position);

        return true;
    }
}
