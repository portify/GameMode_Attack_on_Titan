datablock ParticleData(GreenFlareParticle : tdmgJetEffectParticle)
{
	colors[0] = "0 1 0 0.2";
	colors[1] = "0 1 0 0.1";
	colors[2] = "0 1 0 0";
	lifetimeMS = 6000;
	lifetimeVarianceMS = 500;
	times[2] = 1;
};
 
datablock ParticleEmitterData(GreenFlareEmitter : tdmgJetEffectEmitter)
{
	ejectionPeriodMS = 7;
	periodVarianceMS = 2;
	particles = GreenFlareParticle;
};

datablock ProjectileData(GreenFlareProjectile)
{
	directDamage = 0;
	radiusDamage = 0;
	damageRadius = 0;
	particleEmitter = GreenFlareEmitter;
	explosion = "";

	muzzleVelocity = 50;
	velInheritFactor = 1;

	armingDelay = 0;
	lifetime = 5000;
	fadeDelay = 2500;
	bounceElasticity = 0;
	bounceFriction = 0;
	isBallistic = true;
	gravityMod = 0.5;

	hasLight = false;
	lightRadius	= 3.0;
	lightColor	= "0 0 0.5";
};

datablock ParticleData(RedFlareParticle : GreenFlareParticle)
{
	colors[0] = "1 0 0 0.2";
	colors[1] = "1 0 0 0.1";
	colors[2] = "1 0 0 0";
	lifetimeMS = 6000;
	lifetimeVarianceMS = 500;
	times[2] = 1;
};

datablock ParticleEmitterData(RedFlareEmitter : GreenFlareEmitter)
{
	particles = RedFlareParticle;
};

datablock ProjectileData(RedFlareProjectile : GreenFlareProjectile)
{
	particleEmitter = RedFlareEmitter;
	gravityMod = 0.5;
};

datablock ParticleData(BlackFlareParticle : GreenFlareParticle)
{
	colors[0] = "0 0 0 0.2";
	colors[1] = "0 0 0 0.1";
	colors[2] = "0 0 0 0";
	lifetimeMS = 6000;
	lifetimeVarianceMS = 500;
	times[2] = 1;
};

datablock ParticleEmitterData(BlackFlareEmitter : GreenFlareEmitter)
{
	particles = BlackFlareParticle;
};

datablock ProjectileData(BlackFlareProjectile : GreenFlareProjectile)
{
	particleEmitter = BlackFlareEmitter;
	gravityMod = 0.5;
};
