using Entity;
using GameFramework;

public class CharacterView : BasicView
{
	private Character character = null;

	public override void OnAttached(IEntity entity)
	{
		base.OnAttached(entity);

		character = entity as Character;
	}

	public override void OnDetached()
	{
		base.OnDetached();

		character = null;
	}

	#region MonoBehaviour
	private void LateUpdate()
	{
		if (character != null)
		{
			Animator_SetFloat("Speed", character.Velocity.magnitude);
			Animator_SetBool("Alive", character.IsAlive);
		}
	}
	#endregion
}
