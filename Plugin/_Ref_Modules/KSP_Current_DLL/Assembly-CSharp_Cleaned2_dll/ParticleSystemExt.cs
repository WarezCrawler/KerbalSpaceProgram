using UnityEngine;

public static class ParticleSystemExt
{
	private static Particle[] particleBuffer = (Particle[])(object)new Particle[10000];

	public static Particle[] GetParticleBuffer(this ParticleSystem particleSystem)
	{
		if (particleBuffer.Length < particleSystem.particleCount)
		{
			particleBuffer = (Particle[])(object)new Particle[particleSystem.particleCount * 2];
		}
		particleSystem.GetParticles(particleBuffer);
		return particleBuffer;
	}
}
