using UnityEngine;
using GameFramework;

public class PlayerView : MonoViewComponentBase
{
	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(Entity.Position, LOP.Game.BROADCAST_SCOPE_RADIUS);
	}
}
