using System.Collections.Generic;

public class ModuleAsteroidAnalysis : PartModule
{
	protected Part potatoPart;

	protected Dictionary<string, ModuleAnalysisResource> analyzers = new Dictionary<string, ModuleAnalysisResource>();

	public override void OnStart(StartState state)
	{
		GameEvents.onVesselWasModified.Add(CheckForPotato);
		if (HighLogic.LoadedSceneIsFlight)
		{
			CheckForPotato(base.vessel);
		}
	}

	protected void OnDestroy()
	{
		analyzers.Clear();
		GameEvents.onVesselWasModified.Remove(CheckForPotato);
	}

	protected void CheckForPotato(Vessel v)
	{
		ModuleAsteroid moduleAsteroid = v.FindPartModuleImplementing<ModuleAsteroid>();
		if (moduleAsteroid != null)
		{
			FindAsteroidResources(moduleAsteroid.part);
		}
		else
		{
			ClearAsteroidResources();
		}
	}

	protected void ClearAsteroidResources()
	{
		Part part = base.part;
		if (potatoPart != null)
		{
			part = potatoPart;
			potatoPart = null;
			ClearAsteroidResources();
		}
		int count = part.Modules.Count;
		while (count-- > 0)
		{
			PartModule partModule = part.Modules[count];
			if (partModule is ModuleAnalysisResource)
			{
				ModuleAnalysisResource obj = partModule as ModuleAnalysisResource;
				obj.displayAbundance = 0f;
				obj.abundance = 0f;
			}
		}
		analyzers.Clear();
	}

	protected void FindAsteroidResources(Part p)
	{
		if (potatoPart != null && potatoPart != p)
		{
			ClearAsteroidResources();
		}
		potatoPart = p;
		analyzers.Clear();
		int count = p.Modules.Count;
		for (int i = 0; i < count; i++)
		{
			PartModule partModule = p.Modules[i];
			if (partModule is ModuleAsteroidResource)
			{
				ModuleAsteroidResource moduleAsteroidResource = partModule as ModuleAsteroidResource;
				ModuleAnalysisResource moduleAnalysisResource = FindMatchingAnalyzer(p, moduleAsteroidResource.resourceName);
				if (moduleAnalysisResource != null)
				{
					moduleAnalysisResource.abundance = moduleAsteroidResource.abundance;
					moduleAnalysisResource.displayAbundance = moduleAsteroidResource.displayAbundance;
				}
			}
		}
	}

	protected ModuleAnalysisResource FindMatchingAnalyzer(Part p, string rName)
	{
		if (analyzers.TryGetValue(rName, out var value))
		{
			return value;
		}
		int count = p.Modules.Count;
		while (true)
		{
			if (count-- > 0)
			{
				PartModule partModule = p.Modules[count];
				if (partModule is ModuleAnalysisResource)
				{
					value = partModule as ModuleAnalysisResource;
					if (value.resourceName == rName)
					{
						break;
					}
				}
				continue;
			}
			analyzers.Add(rName, null);
			return null;
		}
		analyzers.Add(rName, value);
		return value;
	}
}
