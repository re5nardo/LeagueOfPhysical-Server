﻿using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramework;
using Entity;
using System.Linq;
using NetworkModel.Mirror;

public class NearEntityController : LOPMonoEntityComponentBase
{
	public enum CellSeenState
	{
		Seen = 0,
		DeadBand_Seen = 1,
		DeadBand_UnSeen = 2,
	}

	private const float DEAD_BAND_WIDTH = 5f;

	private float m_fSight = LOP.Game.BROADCAST_SCOPE_RADIUS;	//	Entity 함수로 빼야함..

	private HashSet<Vector2Int> m_hashLastSubscribeCellPosition = new HashSet<Vector2Int>();

	private Dictionary<Vector2Int, CellSeenState> m_dicCellSeenState = new Dictionary<Vector2Int, CellSeenState>();

	private Dictionary<Vector2Int, Cell> m_dicSeenCell = new Dictionary<Vector2Int, Cell>();
	private Dictionary<Vector2Int, Cell> m_dicDeadBandCell = new Dictionary<Vector2Int, Cell>();

	private HashSet<int> m_hashNearEntityId = new HashSet<int>();
	private HashSet<int> m_hashNearPlayerEntityId = new HashSet<int>();

	protected override void OnAttached(IEntity entity)
	{
        Vector2Int vec2CellPosition = EntityManager.Instance.GetEntityCellPosition(Entity.EntityId);
		//var interestingCells = EntityManager.Instance.GetCells(vec2CellPosition, m_fSight + DEAD_BAND_WIDTH, true);
		//foreach (Cell cell in interestingCells)
		//{
		//	Subscribe("EntityAddedToGrid", GridPubSubService.Instance);
		//	Subscribe("EntityRemovedFromGrid", GridPubSubService.Instance);
		//	Subscribe("EntityMoveCell", GridPubSubService.Instance);

		//	m_hashLastSubscribeCellPosition.Add(cell.m_vec2Position);
		//}

        SceneMessageBroker.AddSubscriber<GameMessage.EntityAddedToGrid>(OnEntityAddedToGrid);
        SceneMessageBroker.AddSubscriber<GameMessage.EntityRemovedFromGrid>(OnEntityRemovedFromGrid);
        SceneMessageBroker.AddSubscriber<GameMessage.EntityMoveCell>(OnEntityMoveCell);

		UpdateMyEntityCellPosition(vec2CellPosition);
	}

	protected override void OnDetached()
    {
        SceneMessageBroker.RemoveSubscriber<GameMessage.EntityAddedToGrid>(OnEntityAddedToGrid);
        SceneMessageBroker.RemoveSubscriber<GameMessage.EntityRemovedFromGrid>(OnEntityRemovedFromGrid);
        SceneMessageBroker.RemoveSubscriber<GameMessage.EntityMoveCell>(OnEntityMoveCell);
    }

	#region Message Handler
	private void OnEntityAddedToGrid(GameMessage.EntityAddedToGrid message)
	{
		if (message.entityId == Entity.EntityId)
		{
			OnMyEntityAddedToGrid(message.cellPosition);
		}
		else
		{
			OnOtherEntityAddedToGrid(message.entityId, message.cellPosition);
		}
	}

	private void OnEntityRemovedFromGrid(GameMessage.EntityRemovedFromGrid message)
	{
		if (message.entityId == Entity.EntityId)
		{
			OnMyEntityRemovedFromGrid(message.entityId, message.cellPosition);
		}
		else
		{
			OnOtherEntityRemovedFromGrid(message.entityId, message.cellPosition);
		}
	}

	private void OnEntityMoveCell(GameMessage.EntityMoveCell message)
	{
		if (message.entityId == Entity.EntityId)
		{
			OnMyEntityMoveCell(message.cur);
		}
		else
		{
			OnOtherEntityMoveCell(message.entityId, message.pre, message.cur);
		}
	}
	#endregion


	private void OnMyEntityAddedToGrid(Vector2Int pos)
	{
		UpdateMyEntityCellPosition(pos);
	}

	private void UpdateMyEntityCellPosition(Vector2Int vec2CellPosition)
	{
		//HashSet<Cell> interestingCells = EntityManager.Instance.GetCells(vec2CellPosition, m_fSight + DEAD_BAND_WIDTH, true);

		////	1. Reset interesting groups
		//foreach (var cellPos in m_hashLastSubscribeCellPosition)
		//{
		//	UnSubscribe(string.Format("[Cell{0}{1}] EntityEnter", cellPos.x, cellPos.y), GridPubSubService.Instance);
		//	UnSubscribe(string.Format("[Cell{0}{1}] EntityLeave", cellPos.x, cellPos.y), GridPubSubService.Instance);
		//}

		//foreach (Cell cell in interestingCells)
		//{
		//	Subscribe(string.Format("[Cell{0}{1}] EntityEnter", cell.m_vec2Position.x, cell.m_vec2Position.y), GridPubSubService.Instance);
		//	Subscribe(string.Format("[Cell{0}{1}] EntityLeave", cell.m_vec2Position.x, cell.m_vec2Position.y), GridPubSubService.Instance);

		//	m_hashLastSubscribeCellPosition.Add(cell.m_vec2Position);
		//}

		//	2. Refresh cell seen state
		var hashSeenNDeadBandCell = EntityManager.Instance.GetCells(vec2CellPosition, m_fSight + DEAD_BAND_WIDTH, true);
		m_dicSeenCell.Clear();
		foreach (Cell cell in EntityManager.Instance.GetCells(vec2CellPosition, m_fSight, true))
		{
			m_dicSeenCell[cell.position] = cell;
		}
		m_dicDeadBandCell.Clear();
		foreach (Cell cell in hashSeenNDeadBandCell)
		{
			if (!m_dicSeenCell.ContainsKey(cell.position))
			{
				m_dicDeadBandCell[cell.position] = cell;
			}
		}

		UpdateCellSeenState();
	}

	private void UpdateCellSeenState()
	{
		Dictionary<Vector2Int, CellSeenState> dicLastCellSeenState = new Dictionary<Vector2Int, CellSeenState>(m_dicCellSeenState);

		m_dicCellSeenState.Clear();
		foreach (Cell seenCell in m_dicSeenCell.Values)
		{
			m_dicCellSeenState[seenCell.position] = CellSeenState.Seen;
		}

		foreach (Cell deadBandCell in m_dicDeadBandCell.Values)
		{
			if (dicLastCellSeenState.ContainsKey(deadBandCell.position))
			{
				if (dicLastCellSeenState[deadBandCell.position] == CellSeenState.Seen)
				{
					m_dicCellSeenState[deadBandCell.position] = CellSeenState.DeadBand_Seen;
				}
				else if (dicLastCellSeenState[deadBandCell.position] == CellSeenState.DeadBand_Seen)
				{
					m_dicCellSeenState[deadBandCell.position] = CellSeenState.DeadBand_Seen;   //	keep state
				}
				else if (dicLastCellSeenState[deadBandCell.position] == CellSeenState.DeadBand_UnSeen)
				{
					m_dicCellSeenState[deadBandCell.position] = CellSeenState.DeadBand_UnSeen; //	keep state
				}
			}
			else
			{
				m_dicCellSeenState[deadBandCell.position] = CellSeenState.DeadBand_UnSeen;
			}
		}

		CheckAppearNDisAppearEntity(dicLastCellSeenState);
	}

	private void CheckAppearNDisAppearEntity(Dictionary<Vector2Int, CellSeenState> dicLastCellSeenState)
	{
        HashSet<int> appearEntityIds = new HashSet<int>();
        HashSet<int> disAppearEntityIds = new HashSet<int>();

        //  현재 Seen 상태의 Entity 순회
        foreach (Cell seenCell in m_dicSeenCell.Values)
		{
			if (dicLastCellSeenState.ContainsKey(seenCell.position))
			{
                //  이전 Seen 상태가 Seen 이었으면 (이미 보여지고 있던 상태면) 무시
                if (dicLastCellSeenState[seenCell.position] == CellSeenState.Seen || dicLastCellSeenState[seenCell.position] == CellSeenState.DeadBand_Seen)
				{
					continue;
				}
			}

            //  새롭게 Seen 상태가 된 Entity들
			foreach (int entityId in seenCell.hashEntityId)
			{
                appearEntityIds.Add(entityId);
            }
		}

        EntityAppear(appearEntityIds.ToList());


        foreach (var lastCell in dicLastCellSeenState)
		{
			Vector2Int lastCellPosition = lastCell.Key;
			CellSeenState lastCellState = lastCell.Value;

            //  이전 Seen 상태가 Seen 이었던 (이미 보여지고 있던) 상태의 Entity 순회
            if (lastCellState == CellSeenState.Seen || lastCellState == CellSeenState.DeadBand_Seen)
			{
                //  현재 보여지고 있는 상태가 아니면 (이전에 보여지다 현재 보여지지 않는 경우)
				if (!m_dicCellSeenState.ContainsKey(lastCellPosition))
				{
					foreach (int entityId in EntityManager.Instance.GetCell(lastCellPosition).hashEntityId)
					{
                        disAppearEntityIds.Add(entityId);
                    }
				}
			}
		}

        EntityDisAppear(disAppearEntityIds.ToList());
    }

    //  List로 주변 Entity 관리하면서 추가 삭제 시 전송..??
    private void OnOtherEntityAddedToGrid(int entityId, Vector2Int pos)
	{
		if (m_dicSeenCell.ContainsKey(pos))
		{
            EntityAppear(new List<int> { entityId });
		}
	}

	private void OnMyEntityRemovedFromGrid(int entityId, Vector2Int pos) { }

	private void OnOtherEntityRemovedFromGrid(int entityId, Vector2Int pos)
	{
		//	Disappear 여기서 전송하는게 맞나? remove 되게 만든 루틴에서 해야 하나..? 아니면 여기서 일괄로 처리?
		if (m_dicCellSeenState.ContainsKey(pos))
		{
			if (m_dicCellSeenState[pos] == CellSeenState.Seen || m_dicCellSeenState[pos] == CellSeenState.DeadBand_Seen)
			{
                EntityDisAppear(new List<int> { entityId });
            }
		}
	}

	private void OnMyEntityMoveCell(Vector2Int pos)
	{
		UpdateMyEntityCellPosition(pos);
	}

	private void OnOtherEntityMoveCell(int entityId, Vector2Int from, Vector2Int to)
	{
		//	Check DisAppear
		if (m_dicCellSeenState.ContainsKey(from) && !m_dicCellSeenState.ContainsKey(to))
		{
			if (m_dicCellSeenState[from] == CellSeenState.Seen || m_dicCellSeenState[from] == CellSeenState.DeadBand_Seen)
			{
                EntityDisAppear(new List<int> { entityId });
            }
		}

		//	Check Appear
		if (m_dicSeenCell.ContainsKey(to))
		{
			if (!m_dicCellSeenState.ContainsKey(from) || m_dicCellSeenState[from] == CellSeenState.DeadBand_UnSeen)
			{
                EntityAppear(new List<int> { entityId });
            }
		}
	}

    private void EntityAppear(List<int> entityIds)
    {
        foreach (var entityId in entityIds)
        {
            m_hashNearEntityId.Add(entityId);

            var entity = Entities.Get<LOPMonoEntityBase>(entityId);
            if (entity.EntityRole == EntityRole.Player)
            {
                m_hashNearPlayerEntityId.Add(entityId);
            }
        }

        SendEntityAppear(entityIds);
    }

    private void EntityDisAppear(List<int> entityIds)
    {
        foreach (var entityId in entityIds)
        {
            m_hashNearEntityId.Remove(entityId);
            m_hashNearPlayerEntityId.Remove(entityId);
        }

        SendEntityDisAppear(entityIds);
    }

    public HashSet<int> GetNearEntityIds()
	{
		return new HashSet<int>(m_hashNearEntityId);
	}

	private void SendEntityAppear(List<int> entityIds)
	{
		if (LOP.Application.IsApplicationQuitting)
			return;

		//  Don't send Local Entities
		entityIds.RemoveAll(entityId => Entities.Get<LOPMonoEntityBase>(entityId).IsLocalEntity);

		using var disposer = PoolObjectDisposer<SC_EntityAppear>.Get();
		var entityAppear = disposer.PoolObject;
		entityAppear.listEntitySnap = new List<EntitySnap>(entityIds.Count);
		foreach (int entityId in entityIds)
		{
			IEntity entity = Entities.Get(entityId);
			entityAppear.listEntitySnap.Add(EntityHelper.GetEntitySnap(entity));
		}
		entityAppear.tick = Game.Current.CurrentTick;

        if (GameIdMap.TryGetConnectionIdByEntityId(Entity.EntityId, out var connectionId))
        {
            RoomNetwork.Instance.Send(entityAppear, connectionId);
        }
    }

	private void SendEntityDisAppear(List<int> entityIds)
	{
		if (LOP.Application.IsApplicationQuitting)
			return;

		//  Don't send Local Entities
		entityIds.RemoveAll(entityId => Entities.Get<LOPMonoEntityBase>(entityId).IsLocalEntity);

		using var disposer = PoolObjectDisposer<SC_EntityDisAppear>.Get();
		var entityDisAppear = disposer.PoolObject;
		entityDisAppear.listEntityId = entityIds;

        if (GameIdMap.TryGetConnectionIdByEntityId(Entity.EntityId, out var connectionId))
        {
            RoomNetwork.Instance.Send(entityDisAppear, connectionId);
        }
	}
}
