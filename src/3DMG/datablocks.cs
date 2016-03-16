datablock staticShapeData(RopeShapeData)
{
	shapeFile = $AOT::Folder @ "/res/shapes/cylinder.dts";
};

datablock PlayerData(PlayerReconArmor : PlayerStandardArmor)
{
	horizMaxSpeed = 500;
	upMaxSpeed = 500;
	canJet = 0;
	airControl = 0.2;
	uiName = "Recon Player";
};

datablock PlayerData(PlayerReconJetArmor : PlayerReconArmor)
{
	runForce = 1500; //Slippery on the ground
	jumpForce = 0; //Can't jump unless jump key released
	airControl = 0.5;
	runSurfaceAngle = 20;
	uiName = "";
};

datablock ParticleData(tdmgJetEffectParticle) {
	textureName = "base/data/particles/smoke";
 
	dragCoefficient = 1;
	gravityCoefficient = 0.025;
	inheritedVelFactor = 0.1;
	windCoefficient = 0.2;
 
	constantAcceleration = 0;
	useInvAlpha = 1;
	spinSpeed = 10;
 
	lifetimeMS = 3000;
	lifetimeVarianceMS = 500;
 
	spinRandomMin = -100;
	spinRandomMax = 100;
 
	colors[0] = "0.8 0.8 0.8 0.2";
	colors[1] = "0.75 0.75 0.75 0.1";
	colors[2] = "0.6 0.6 0.6 0";
 
	sizes[0] = 1;
	sizes[1] = 2.5;
	sizes[2] = 5;
 
	times[0] = 0;
	times[1] = 0.5;
	times[2] = 1;
};
 
datablock ParticleEmitterData(tdmgJetEffectEmitter) {
	ejectionPeriodMS = 5;
	periodVarianceMS = 2;
 
	ejectionVelocity = 0;
	ejectionOffset = 0;
 
	velocityVariance = 0;
	overrideAdvance = 0;
 
	thetaMin = -10;
	thetaMax = 10;
 
	phiReferenceVel = 0;
	phiVariance = 10;
 
	particles = tdmgJetEffectParticle;
};

datablock ShapeBaseImageData(tdmgJetEffectImage) {
	shapeFile = "base/data/shapes/empty.dts";
	emap = false;
 
	mountPoint = 7;
	offset = "0 -0.7 0.4";
 
	stateName[0] = "Emit";
	stateEmitter[0] = tdmgJetEffectEmitter;
	stateEmitterTime[0] = 10;
	stateWaitForTimeout[0] = 1;
	stateTransitionOnTimeout[0] = "Loop";
 
	stateName[1] = "Loop";
	stateTimeoutValue[1] = 0.001;
	stateTransitionOnTimeout[1] = "Emit";
};

datablock DebrisData(TDMGBladeDebris)
{
   // emitters = "JeepTireDebrisTrailEmitter";

	shapeFile = $AOT::Folder @ "/res/shapes/bladedebris.dts";
	lifetime = 30;
	minSpinSpeed = -200.0;
	maxSpinSpeed = 200.0;
	elasticity = 0.5;
	friction = 0.2;
	numBounces = 2;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 2;
};

datablock ExplosionData(TDMGBladeExplosion)
{
   debris = TDMGBladeDebris;
   debrisNum = 2;
   debrisNumVariance = 0;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 180;
   debrisThetaMax = 360;
   debrisVelocity = 6;
   debrisVelocityVariance = 3;
};

datablock ProjectileData(TDMGBladeDebrisProjectile)
{
	explosion = TDMGBladeExplosion;
	isBallistic = true;
};

datablock ShapeBaseImageData(TDMGModelImage)
{
  shapeFile = $AOT::Folder @ "/res/shapes/3dmg.dts";
  emap = true;
  mountPoint = 2;
  offset = "0 -0.4 -0.7";
  eyeOffset = "0 0 10";
  rotation = eulerToMatrix("0 0 180");
  scale = "1 1 1";
  correctMuzzleVector = true;
  doColorShift = false;
  colorShiftColor = "1 1 1 1";
};

datablock ShapeBaseImageData(TDMGSecondBladeImage)
{
  shapeFile = $AOT::Folder @ "/res/shapes/3dmgblade.dts";
  emap = true;
  mountPoint = 1;
  offset = "0 0 0";
  eyeOffset = "0 0 0";
  rotation = eulerToMatrix("0 0 0");
  scale = "1 1 1";
  correctMuzzleVector = true;
  doColorShift = true;
  colorShiftColor = "0.471 0.471 0.471 1.000";
};

datablock ItemData(TDMGSwordsItem)
{
	category = "Tool";
	className = "Tool";

	shapeFile = $AOT::Folder @ "/res/shapes/3dmg.dts";
	mass = 1;
	density	= 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;
	
	uiName = "3D Manuever Gear";
	iconName = $AOT::Folder @ "/res/shapes/icon_3DMG";
	doColorShift = true;
	colorShiftColor = "1 1 1 1";

	image = TDMGSwordImage;
	canDrop	= true;
};

datablock ShapeBaseImageData(TDMGSwordImage)
{
	shapeFile = $AOT::Folder @ "/res/shapes/3dmgblade.dts";
	emap = true;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = "0 0 0";
	correctMuzzleVector = false;
	className = "WeaponImage";

	item = TDMGItem;
	ammo = " ";
	projectile = SwordProjectile;
	projectileType = Projectile;

	melee = true;
	doRetraction = false;
	armReady = false;

	doColorShift = true;
	colorShiftColor = "0.471 0.471 0.471 1.000";

	stateName[0]					= "Activate";
	stateTimeoutValue[0]			= 0.01;
	stateTransitionOnTimeout[0]		= "Ready";

	stateName[1]					= "Ready";
	stateAllowImageChange[1]		= true;
	stateTransitionOnTriggerDown[1] = "Fire";

	stateName[2]					= "Fire";
	stateScript[2]					= "onFire";
	stateAllowImageChange[2]		= true;
	stateTransitionOnTriggerUp[2]	= "Ready";
};
datablock ShapeBaseImageData(TDMGSecondBrokenBladeImage : TDMGSecondBladeImage)
{
	shapeFile = $AOT::Folder @ "/res/shapes/3dmgbladeempty.dts";
};

datablock ShapeBaseImageData(TDMGBrokenSwordImage : TDMGSwordImage)
{
	shapeFile = $AOT::Folder @ "/res/shapes/3dmgbladeempty.dts";
	projectile = SwordProjectile;
	projectileType = Projectile;
};
datablock ParticleData(TDMGProjectileParticle : tdmgJetEffectParticle) {
	constantAcceleration = 0.1;
	dragCoefficient = 5;
	spinSpeed = 50;
 
	lifetimeMS = 1500;
	lifetimeVarianceMS = 250;
 
	spinRandomMin = -150;
	spinRandomMax = 150;
 
	colors[0] = "0.6 0.6 0.6 0.4";
	colors[1] = "0.45 0.45 0.45 0.2";
	colors[2] = "0.3 0.3 0.3 0";
 
	sizes[0] = 0.5;
	sizes[1] = 1;
	sizes[2] = 2;
 
	times[0] = 0;
	times[1] = 0.5;
	times[2] = 1;
};

datablock ParticleEmitterData(TDMGProjectileEmitter : tdmgJetEffectEmitter) {
	ejectionPeriodMS = 2;
	periodVarianceMS = 1;
 
	ejectionVelocity = 0;
	ejectionOffset = 0;
 
	velocityVariance = 0;
	overrideAdvance = 0;
 
	thetaMin = -10;
	thetaMax = 10;
 
	phiReferenceVel = 0;
	phiVariance = 10;
 
	particles = TDMGProjectileParticle;
};

datablock ProjectileData(TDMGProjectile)
{
	directDamage = 0;
	radiusDamage = 0;
	damageRadius = 0;
	particleEmitter = TDMGProjectileEmitter;
	explosion = "";

	muzzleVelocity = 100;
	velInheritFactor = 0;

	armingDelay = 0;
	lifetime = 1000;
	fadeDelay = 1000;
	bounceElasticity = 0;
	bounceFriction = 0;
	isBallistic = true;
	gravityMod = 0;

	hasLight = false;
	lightRadius	= 3.0;
	lightColor	= "0 0 0.5";
};