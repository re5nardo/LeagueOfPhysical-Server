using System.Collections.Generic;
using Skill;
using GameFramework;

public class SkillController : LOPMonoEntityComponentBase
{
	public Dictionary<int, double> GetEntitySkillInfo()
	{
		Dictionary<int, double> dicSkillInfo = new Dictionary<int, double>();

		foreach (Skill.SkillBase skill in GetComponents<Skill.SkillBase>())
		{
			dicSkillInfo[skill.MasterDataId] = skill.CoolTime;
		}

		return dicSkillInfo;
	}

	public void AddSkill(int nSkillMasterID)
	{
		SkillBase skill = SkillFactory.Instance.CreateSkill(gameObject, nSkillMasterID);
		if (skill != null)
		{
			Entity.AttachEntityComponent(skill);
			skill.Initialize(new SkillParam(nSkillMasterID));
            skill.StartSkill();

        }
	}

	public void RemoveSkill(int nSkillMasterID)
	{
		var skills = Entity.GetEntityComponents<SkillBase>();
		if (skills == null)
			return;

		var found = skills.FindAll(x => x.MasterDataId == nSkillMasterID);
		if (found == null)
			return;

		for (int i = found.Count - 1; i >= 0; --i)
		{
			Entity.DetachEntityComponent(found[i]);
			Destroy(found[i]);
		}
	}
}
