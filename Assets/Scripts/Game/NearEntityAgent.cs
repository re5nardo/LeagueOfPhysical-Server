using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramework;
using Entity;

public class NearEntityAgent : MonoModelComponentBase, ISubscriber
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

	private Dictionary<Enum, Action<object[]>> m_dicMessageHandler = new Dictionary<Enum, Action<object[]>>();

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

		GridPubSubService.Instance.AddSubscriber(GameMessageKey.EntityAddedToGrid, this);
		GridPubSubService.Instance.AddSubscriber(GameMessageKey.EntityRemovedFromGrid, this);
		GridPubSubService.Instance.AddSubscriber(GameMessageKey.EntityMoveCell, this);

		m_dicMessageHandler.Add(GameMessageKey.EntityAddedToGrid, OnEntityAddedToGrid);
		m_dicMessageHandler.Add(GameMessageKey.EntityRemovedFromGrid, OnEntityRemovedFromGrid);
		m_dicMessageHandler.Add(GameMessageKey.EntityMoveCell, OnEntityMoveCell);

		UpdateMyEntityCellPosition(vec2CellPosition);
	}

    public override void OnDetached()
    {
        base.OnDetached();

        if (GridPubSubService.IsInstantiated())
        {
            GridPubSubService.Instance.RemoveSubscriber(GameMessageKey.EntityAddedToGrid, this);
            GridPubSubService.Instance.RemoveSubscriber(GameMessageKey.EntityRemovedFromGrid, this);
            GridPubSubService.Instance.RemoveSubscriber(GameMessageKey.EntityMoveCell, this);
        }

        m_dicMessageHandler.Clear();
    }

	#region ISubscriber
	public void OnMessage(Enum key, params object[] param)
	{
        m_dicMessageHandler[key](param);
	}
	#endregion

	#region Message Handler
	private void OnEntityAddedToGrid(params object[] param)
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

	private void OnEntityRemovedFromGrid(params object[] param)
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

	private void OnEntityMoveCell(params object[] param)
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
		foreach (Cell seenCell in m_dicSeenCell.Values)
		{
			if (dicLastCellSeenState.ContainsKey(seenCell.m_vec2Position))
			{
				if (dicLastCellSeenState[seenCell.m_vec2Position] == CellSeenState.Seen || dicLastCellSeenState[seenCell.m_vec2Position] == CellSeenState.DeadBand_Seen)
				{
					continue;
				}
			}

			foreach (int nEntityID in seenCell.m_hashEntityID)
			{
				m_hashNearEntityID.Add(nEntityID);

                MonoEntityBase entity = EntityManager.Instance.GetEntity(nEntityID) as MonoEntityBase;
				if (entity.EntityRole == EntityRole.Player)
				{
					m_hashNearPlayerEntityID.Add(nEntityID);
				}

				SendEntityAppear(new List<int> { nEntityID });
			}
		}

		foreach (var lastCell in dicLastCellSeenState)
		{
			Vector2Int lastCellPosition = lastCell.Key;
			CellSeenState lastCellState = lastCell.Value;

			if (lastCellState == CellSeenState.Seen || lastCellState == CellSeenState.DeadBand_Seen)
			{
				if (!m_dicCellSeenState.ContainsKey(lastCellPosition))
				{
					foreach (int nEntityID in EntityManager.Instance.GetCell(lastCellPosition).m_hashEntityID)
					{
						m_hashNearEntityID.Remove(nEntityID);
						m_hashNearPlayerEntityID.Remove(nEntityID);

						SendEntityDisAppear(new List<int> { nEntityID });
					}
				}
			}
		}
	}

    //  List로 주변 Entity 관리하면서 추가 삭제 시 전송..??

	private void OnOtherEntityAddedToGrid(int nEntityID, Vector2Int pos)
	{
		if (m_dicSeenCell.ContainsKey(pos))
		{
			m_hashNearEntityID.Add(nEntityID);

            MonoEntityBase entity = EntityManager.Instance.GetEntity(nEntityID) as MonoEntityBase;
			if (entity.EntityRole == EntityRole.Player)
			{
				m_hashNearPlayerEntityID.Add(nEntityID);
			}

			SendEntityAppear(new List<int> { nEntityID });
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
				m_hashNearEntityID.Remove(nEntityID);
				m_hashNearPlayerEntityID.Remove(nEntityID);

				SendEntityDisAppear(new List<int> { nEntityID });
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
				m_hashNearEntityID.Remove(nEntityID);
				m_hashNearPlayerEntityID.Remove(nEntityID);

				SendEntityDisAppear(new List<int> { nEntityID });
			}
		}

		//	Check Appear
		if (m_dicSeenCell.ContainsKey(to))
		{
			if (!m_dicCellSeenState.ContainsKey(from) || m_dicCellSeenState[from] == CellSeenState.DeadBand_UnSeen)
			{
				m_hashNearEntityID.Add(nEntityID);

                MonoEntityBase entity = EntityManager.Instance.GetEntity(nEntityID) as MonoEntityBase;
				if (entity.EntityRole == EntityRole.Player)
				{
					m_hashNearPlayerEntityID.Add(nEntityID);
				}

				SendEntityAppear(new List<int> { nEntityID });
			}
		}
	}

	public HashSet<int> GetNearEntityIDs()
	{
		return new HashSet<int>(m_hashNearEntityID);
	}

	private void SendEntityAppear(List<int> entityIDs)
	{
		if (LOP.Application.IsApplicationQuitting)
			return;

		SC_EntityAppear entityAppear = new SC_EntityAppear();
		entityAppear.m_listEntitySnapInfo = new List<EntitySnapInfo>(entityIDs.Count);
		foreach (int entityID in entityIDs)
		{
			IEntity entity = EntityManager.Instance.GetEntity(entityID);
			entityAppear.m_listEntitySnapInfo.Add(EntityHelper.GetEntitySnapInfo(entity));
		}
		entityAppear.m_fGameTime = Game.Current.GameTime;

		int actorID = PhotonHelper.GetActorID(Entity.EntityID);

		RoomNetwork.Instance.Send(entityAppear, actorID);
	}

	private void SendEntityDisAppear(List<int> entityIDs)
	{
		if (LOP.Application.IsApplicationQuitting)
			return;

		SC_EntityDisAppear entityDisAppear = new SC_EntityDisAppear();
		entityDisAppear.m_listEntityID = entityIDs;

		int actorID = PhotonHelper.GetActorID(Entity.EntityID);

		RoomNetwork.Instance.Send(entityDisAppear, actorID);
	}
}
