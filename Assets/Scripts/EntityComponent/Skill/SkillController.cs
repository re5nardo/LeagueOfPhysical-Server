using System.Collections.Generic;
using Skill;
using GameFramework;

public class SkillController : MonoComponentBase
{
	public Dictionary<int, float> GetEntitySkillInfo()
	{
		Dictionary<int, float> dicSkillInfo = new Dictionary<int, float>();

		foreach (Skill.SkillBase skill in GetComponents<Skill.SkillBase>())
		{
			dicSkillInfo[skill.GetSkillMasterID()] = skill.CoolTime;
		}

		return dicSkillInfo;
	}

	public void AddSkill(int nSkillMasterID)
	{
		SkillBase skill = SkillFactory.Instance.CreateSkill(gameObject, nSkillMasterID);
		if (skill != null)
		{
			Entity.AttachComponent(skill);
			skill.SetData(nSkillMasterID);
            skill.StartSkill();

        }
	}

	public void RemoveSkill(int nSkillMasterID)
	{
		var skills = Entity.GetEntityComponents<SkillBase>();
		if (skills == null)
			return;

		var found = skills.FindAll(x => x.GetSkillMasterID() == nSkillMasterID);
		if (found == null)
			return;

		for (int i = found.Count - 1; i >= 0; --i)
		{
			Entity.DetachComponent(found[i]);
			Destroy(found[i]);
		}
	}
}
