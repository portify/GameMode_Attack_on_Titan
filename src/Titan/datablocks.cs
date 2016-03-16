datablock TSShapeConstructor(titanDts)
{
	baseShape = $AOT::Folder @ "/res/shapes/titan.dts";
	sequence1 = $AOT::Folder @ "/res/shapes/titan_run.dsq walk";
	sequence2 = $AOT::Folder @ "/res/shapes/titan_look.dsq look";
	sequence3 = $AOT::Folder @ "/res/shapes/titan_root.dsq root";
	sequence4 = $AOT::Folder @ "/res/shapes/titan_run.dsq run";
	sequence5 = $AOT::Folder @ "/res/shapes/titan_death1.dsq death1";
	sequence6 = $AOT::Folder @ "/res/shapes/titan_root.dsq crouch";
	sequence7 = $AOT::Folder @ "/res/shapes/titan_root.dsq crouchrun";
	sequence8 = $AOT::Folder @ "/res/shapes/titan_root.dsq crouchback";
	sequence9 = $AOT::Folder @ "/res/shapes/titan_root.dsq crouchside";
	sequence10 = $AOT::Folder @ "/res/shapes/titan_run.dsq back";
	sequence11 = $AOT::Folder @ "/res/shapes/titan_run.dsq side";
	sequence12 = $AOT::Folder @ "/res/shapes/titan_eat.dsq eat";
	sequence13 = $AOT::Folder @ "/res/shapes/titan_grab.dsq grab";
	sequence14 = $AOT::Folder @ "/res/shapes/titan_slam.dsq slam";
};

datablock PlayerData(TitanArmor : PlayerStandardArmor)
{
	runForce = 2000;
	maxForwardSpeed = 2.5;
	maxBackwardSpeed = 0;
	maxSideSpeed = 0;
	minImpactSpeed = 18;

	airControl = 0;

	maxForwardCrouchSpeed = 0;
	maxBackwardCrouchSpeed = 0;
	maxSideCrouchSpeed = 0;

	jumpForce = 0;
	jumpEnergyDrain = 0;
	minJumpEnergy = 0;
	jumpDelay = 5;

	canJet = 0;

	cameraMaxDist = 3;
	cameraVerticalOffset = 2.25;
	maxFreelookAngle = 0;


	//AI Things
	attackCooldown = 3;
	attackRecovery = 1; //Seconds of inactivity after attack
	maxYawSpeed = 2;
	blindRecovery = 1;

	//

	// shapeFile = $AOT::Folder @ "/res/shapes/teste.dts";
	uiName = "Titan Playertype";
	maxTools = 4;
	maxWeapons = 4;

	repairRate = 0.5;

	canRide = 1;
	isTitan = true;
	titanType = "normal";
	mass = 200;
};

datablock PlayerData(TitanCrawlerArmor : TitanArmor)
{
	runForce = 9000;
	maxForwardSpeed = 7;
	maxBackwardSpeed = 1;
	maxSideSpeed = 2;
	jumpForce = 3000;
	jumpDelay = 60;
	isTitan = true;
	maxFreelookAngle = 1;
	titanType = "crawler";
	boundingBox = "5 5 4";
	uiName = "Titan - Cralwer";
};

datablock PlayerData(TitanJumperArmor : TitanArmor)
{
	maxForwardSpeed = 4;
	airControl = 1;
	isTitan = true;
	jumpForce = 4000;
	jumpDelay = 60;
	titanType = "jumper";
	uiName = "Titan - Jumper";

	//AI Things
	attackCooldown = 1;
	attackRecovery = 0.5; //Seconds of inactivity after attack
	blindRecovery = 0.75;
	maxYawSpeed = 3;
	canJump = true;
};

datablock PlayerData(TitanRunnerArmor : TitanArmor)
{
	runForce = 5000;
	maxForwardSpeed = 5;
	isTitan = true;
	titanType = "runner";
	//AI Things
	attackCooldown = 1;
	attackRecovery = 0.5; //Seconds of inactivity after attack
	blindRecovery = 0.75;
	maxYawSpeed = 3;
};

datablock PlayerData(TitanInsaneArmor : TitanArmor)
{
	runForce = 8000;
	maxForwardSpeed = 7;
	airControl = 1;
	isTitan = true;
	jumpForce = 7000;
	jumpDelay = 60;
	titanType = "insane";
	//AI Things
	attackCooldown = 0.25;
	attackRecovery = 0.25; //Seconds of inactivity after attack
	blindRecovery = 0.75;
	maxYawSpeed = 6;
};

datablock ParticleData(TitanFistTrailParticle)
{
	dragCoefficient = 1;
	windCoefficient = 0.2;
	gravityCoefficient = 0;
	inheritedVelFactor = 0.2;
	constantAcceleration = 0;
	lifetimeMS = 400;
	lifetimeVarianceMS = 100;
	textureName = "base/data/particles/cloud";
	spinSpeed = 0;
	spinRandomMin = -900;
	spinRandomMax = 900;
	useInvAlpha = false;

	colors[0] = "0.33 0.33 0.33 1";
	colors[1] = "0.67 0.67 0.67 0.8";
	colors[2] = "1 1 1 0.2";
	sizes[0] = 0.4;
	sizes[1] = 0.25;
	sizes[2] = 0.1;
	times[0] = 0;
	times[1] = 0.2;
	times[2] = 1;
};

datablock ParticleEmitterData(TitanFistTrailEmitter)
{
	ejectionPeriodMS = 5;
	periodVarianceMS = 0;
	ejectionVelocity = 0.0;
	velocityVariance = 0.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 10;
	phiReferenceVel  = 90;
	phiVariance      = 10;
	overrideAdvance = false;
	particles = "TitanFistTrailParticle";
	uiName = "";
};

datablock ExplosionData(TitanFistExplosion : spearExplosion)
{
	//impulse
	impulseRadius = 3.5;
	impulseForce = 2000;

	//radius damage
	radiusDamage        = 100;
	damageRadius        = 3.5;
};

datablock ProjectileData(TitanFistExplosionProjectile : spearProjectile)
{
	explosion = TitanFistExplosion;
	uiName = "";
	directDamageType  = $DamageType::Suicide;
	radiusDamageType  = $DamageType::Suicide;
};
