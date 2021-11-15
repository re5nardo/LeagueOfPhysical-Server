using Entity;
using GameFramework;

public class CharacterView : EntityBasicView
{
	private Character Character => base.Entity as Character;

	#region MonoBehaviour
	private void LateUpdate()
	{
		if (Character != null)
		{
			Animator_SetFloat("Speed", Character.Velocity.magnitude);
			Animator_SetBool("Alive", Character.IsAlive);
		}
	}
	#endregion
}
