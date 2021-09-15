using System.Collections.Generic;
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

	private HashSet<int> m_hashNearEntityID = new HashSet<int>();
	private HashSet<int> m_hashNearPlayerEntityID = new HashSet<int>();

    public override void OnAttached(IEntity entity)
	{
		base.OnAttached(entity);

        Vector2Int vec2CellPosition = EntityManager.Instance.GetEntityCellPosition(Entity.EntityID);
		//var interestingCells = EntityManager.Instance.GetCells(vec2CellPosition, m_fSight + DEAD_BAND_WIDTH, true);
		//foreach (Cell cell in interestingCells)
		//{
		//	Subscribe("EntityAddedToGrid", GridPubSubService.Instance);
		//	Subscribe("EntityRemovedFromGrid", GridPubSubService.Instance);
		//	Subscribe("EntityMoveCell", GridPubSubService.Instance);

		//	m_hashLastSubscribeCellPosition.Add(cell.m_vec2Position);
		//}

		GridPubSubService.AddSubscriber(GameMessageKey.EntityAddedToGrid, OnEntityAddedToGrid);
		GridPubSubService.AddSubscriber(GameMessageKey.EntityRemovedFromGrid, OnEntityRemovedFromGrid);
		GridPubSubService.AddSubscriber(GameMessageKey.EntityMoveCell, OnEntityMoveCell);

		UpdateMyEntityCellPosition(vec2CellPosition);
	}

    public override void OnDetached()
    {
        base.OnDetached();

        GridPubSubService.RemoveSubscriber(GameMessageKey.EntityAddedToGrid, OnEntityAddedToGrid);
        GridPubSubService.RemoveSubscriber(GameMessageKey.EntityRemovedFromGrid, OnEntityRemovedFromGrid);
        GridPubSubService.RemoveSubscriber(GameMessageKey.EntityMoveCell, OnEntityMoveCell);
    }

	#region Message Handler
	private void OnEntityAddedToGrid(object[] param)
	{
		int nEntityID = (int)param[0];
		Vector2Int pos = (Vector2Int)param[1];

		if (nEntityID == Entity.EntityID)
		{
			OnMyEntityAddedToGrid(pos);
		}
		else
		{
			OnOtherEntityAddedToGrid(nEntityID, pos);
		}
	}

	private void OnEntityRemovedFromGrid(object[] param)
	{
		int nEntityID = (int)param[0];
		Vector2Int pos = (Vector2Int)param[1];

		if (nEntityID == Entity.EntityID)
		{
			OnMyEntityRemovedFromGrid(nEntityID, pos);
		}
		else
		{
			OnOtherEntityRemovedFromGrid(nEntityID, pos);
		}
	}

	private void OnEntityMoveCell(object[] param)
	{
		int nEntityID = (int)param[0];
		Vector2Int from = (Vector2Int)param[1];
		Vector2Int to = (Vector2Int)param[2];

		if (nEntityID == Entity.EntityID)
		{
			OnMyEntityMoveCell(to);
		}
		else
		{
			OnOtherEntityMoveCell(nEntityID, from, to);
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
			m_dicSeenCell[cell.m_vec2Position] = cell;
		}
		m_dicDeadBandCell.Clear();
		foreach (Cell cell in hashSeenNDeadBandCell)
		{
			if (!m_dicSeenCell.ContainsKey(cell.m_vec2Position))
			{
				m_dicDeadBandCell[cell.m_vec2Position] = cell;
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
			m_dicCellSeenState[seenCell.m_vec2Position] = CellSeenState.Seen;
		}

		foreach (Cell deadBandCell in m_dicDeadBandCell.Values)
		{
			if (dicLastCellSeenState.ContainsKey(deadBandCell.m_vec2Position))
			{
				if (dicLastCellSeenState[deadBandCell.m_vec2Position] == CellSeenState.Seen)
				{
					m_dicCellSeenState[deadBandCell.m_vec2Position] = CellSeenState.DeadBand_Seen;
				}
				else if (dicLastCellSeenState[deadBandCell.m_vec2Position] == CellSeenState.DeadBand_Seen)
				{
					m_dicCellSeenState[deadBandCell.m_vec2Position] = CellSeenState.DeadBand_Seen;   //	keep state
				}
				else if (dicLastCellSeenState[deadBandCell.m_vec2Position] == CellSeenState.DeadBand_UnSeen)
				{
					m_dicCellSeenState[deadBandCell.m_vec2Position] = CellSeenState.DeadBand_UnSeen; //	keep state
				}
			}
			else
			{
				m_dicCellSeenState[deadBandCell.m_vec2Position] = CellSeenState.DeadBand_UnSeen;
			}
		}

		CheckAppearNDisAppearEntity(dicLastCellSeenState);
	}

	private void CheckAppearNDisAppearEntity(Dictionary<Vector2Int, CellSeenState> dicLastCellSeenState)
	{
        HashSet<int> appearEntityIDs = new HashSet<int>();
        HashSet<int> disAppearEntityIDs = new HashSet<int>();

        //  현재 Seen 상태의 Entity 순회
        foreach (Cell seenCell in m_dicSeenCell.Values)
		{
			if (dicLastCellSeenState.ContainsKey(seenCell.m_vec2Position))
			{
                //  이전 Seen 상태가 Seen 이었으면 (이미 보여지고 있던 상태면) 무시
                if (dicLastCellSeenState[seenCell.m_vec2Position] == CellSeenState.Seen || dicLastCellSeenState[seenCell.m_vec2Position] == CellSeenState.DeadBand_Seen)
				{
					continue;
				}
			}

            //  새롭게 Seen 상태가 된 Entity들
			foreach (int nEntityID in seenCell.m_hashEntityID)
			{
                appearEntityIDs.Add(nEntityID);
            }
		}

        EntityAppear(appearEntityIDs.ToList());


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
					foreach (int nEntityID in EntityManager.Instance.GetCell(lastCellPosition).m_hashEntityID)
					{
                        disAppearEntityIDs.Add(nEntityID);
                    }
				}
			}
		}

        EntityDisAppear(disAppearEntityIDs.ToList());
    }

    //  List로 주변 Entity 관리하면서 추가 삭제 시 전송..??
    private void OnOtherEntityAddedToGrid(int nEntityID, Vector2Int pos)
	{
		if (m_dicSeenCell.ContainsKey(pos))
		{
            EntityAppear(new List<int> { nEntityID });
		}
	}

	private void OnMyEntityRemovedFromGrid(int nEntityID, Vector2Int pos)
	{
		
	}

	private void OnOtherEntityRemovedFromGrid(int nEntityID, Vector2Int pos)
	{
		//	Disappear 여기서 전송하는게 맞나? remove 되게 만든 루틴에서 해야 하나..? 아니면 여기서 일괄로 처리?
		if (m_dicCellSeenState.ContainsKey(pos))
		{
			if (m_dicCellSeenState[pos] == CellSeenState.Seen || m_dicCellSeenState[pos] == CellSeenState.DeadBand_Seen)
			{
                EntityDisAppear(new List<int> { nEntityID });
            }
		}
	}

	private void OnMyEntityMoveCell(Vector2Int pos)
	{
		UpdateMyEntityCellPosition(pos);
	}

	private void OnOtherEntityMoveCell(int nEntityID, Vector2Int from, Vector2Int to)
	{
		//	Check DisAppear
		if (m_dicCellSeenState.ContainsKey(from) && !m_dicCellSeenState.ContainsKey(to))
		{
			if (m_dicCellSeenState[from] == CellSeenState.Seen || m_dicCellSeenState[from] == CellSeenState.DeadBand_Seen)
			{
                EntityDisAppear(new List<int> { nEntityID });
            }
		}

		//	Check Appear
		if (m_dicSeenCell.ContainsKey(to))
		{
			if (!m_dicCellSeenState.ContainsKey(from) || m_dicCellSeenState[from] == CellSeenState.DeadBand_UnSeen)
			{
                EntityAppear(new List<int> { nEntityID });
            }
		}
	}

    private void EntityAppear(List<int> entityIDs)
    {
        foreach (var entityID in entityIDs)
        {
            m_hashNearEntityID.Add(entityID);

            var entity = Entities.Get<LOPMonoEntityBase>(entityID);
            if (entity.EntityRole == EntityRole.Player)
            {
                m_hashNearPlayerEntityID.Add(entityID);
            }
        }

        SendEntityAppear(entityIDs);
    }

    private void EntityDisAppear(List<int> entityIDs)
    {
        foreach (var entityID in entityIDs)
        {
            m_hashNearEntityID.Remove(entityID);
            m_hashNearPlayerEntityID.Remove(entityID);
        }

        SendEntityDisAppear(entityIDs);
    }

    public HashSet<int> GetNearEntityIDs()
	{
		return new HashSet<int>(m_hashNearEntityID);
	}

	private void SendEntityAppear(List<int> entityIDs)
	{
		if (LOP.Application.IsApplicationQuitting)
			return;

        //  Don't send Local Entities
        entityIDs.RemoveAll(entityID => Entities.Get<LOPMonoEntityBase>(entityID).IsLocalEntity);

        var entityAppear = new SC_EntityAppear();
		entityAppear.listEntitySnap = new List<EntitySnap>(entityIDs.Count);
		foreach (int entityID in entityIDs)
		{
			IEntity entity = Entities.Get(entityID);
			entityAppear.listEntitySnap.Add(EntityHelper.GetEntitySnap(entity));
		}
		entityAppear.tick = Game.Current.CurrentTick;

        if (IDMap.TryGetConnectionIdByEntityId(Entity.EntityID, out var connectionId))
        {
            RoomNetwork.Instance.Send(entityAppear, connectionId);
        }
    }

	private void SendEntityDisAppear(List<int> entityIDs)
	{
		if (LOP.Application.IsApplicationQuitting)
			return;

        //  Don't send Local Entities
        entityIDs.RemoveAll(entityID => Entities.Get<LOPMonoEntityBase>(entityID).IsLocalEntity);

        var entityDisAppear = new SC_EntityDisAppear();
		entityDisAppear.listEntityId = entityIDs;

        if (IDMap.TryGetConnectionIdByEntityId(Entity.EntityID, out var connectionId))
        {
            RoomNetwork.Instance.Send(entityDisAppear, connectionId);
        }
	}
}
