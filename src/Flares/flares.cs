datablock itemData(greenFlareItem : swordItem)
{
	shapeFile = $AOT::Folder @ "/res/shapes/flare.dts";
	image = GreenFlareImage;
	uiName = "Flare - Green";
	doColorShift = true;
	colorShiftColor = "0 1 0 1";
};

datablock ShapeBaseImageData(GreenFlareImage)
{
	shapeFile = $AOT::Folder @ "/res/shapes/flare.dts";
	emap = true;

	mountPoint = 0;
	offset = "0 0 0.2";
	rotation = eulerToMatrix("0 0 180");
	eyeOffset = "0 0 0";
	correctMuzzleVector = false;
	className = "WeaponImage";

	item = GreenFlareItem;
	ammo = " ";
	projectile = SwordProjectile;
	projectileType = Projectile;

	melee = true;
	doRetraction = false;
	armReady = false;

	doColorShift = true;
	colorShiftColor = "0 1 0 1";

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

function GreenFlareImage::onMount(%this, %obj, %slot)
{
	parent::onMount(%this, %obj, %slot);
	%obj.playThread(1, root);
}

function GreenFlareImage::onUnMount(%this, %obj, %slot)
{
	parent::onUnMount(%this, %obj, %slot);
	cancel(%obj.flareSchedule);
	cancel(%obj.flareScheduleB);
}

function GreenFlareImage::onFire(%this, %obj, %slot)
{
	if($Sim::Time - %obj.lastGreenFlare < 10)
	{
		// %obj.client.play2d(brickErrorSound);
		return;
	}
	%obj.lastGreenFlare = $Sim::Time;
	%obj.playThread(1, armReadyRight);
	%obj.playThread(2, spearReady);
	%obj.flareSchedule = %obj.schedule(500, fireFlare, "green");
	%obj.flareScheduleB = %obj.schedule(500, playThread, 3, plant);
	%obj.flareSchedule = %obj.schedule(1000, playThread, 2, root);
	%obj.flareScheduleB = %obj.schedule(1000, playThread, 1, root);
}

datablock itemData(RedFlareItem : GreenFlareItem)
{
	image = RedFlareImage;
	uiName = "Flare - Red";
	doColorShift = true;
	colorShiftColor = "1 0 0 1";
};

datablock ShapeBaseImageData(RedFlareImage : GreenFlareImage)
{
	doColorShift = true;
	colorShiftColor = "1 0 0 1";
};

function RedFlareImage::onMount(%this, %obj, %slot)
{
	parent::onMount(%this, %obj, %slot);
	%obj.playThread(1, root);
}

function RedFlareImage::onUnMount(%this, %obj, %slot)
{
	parent::onUnMount(%this, %obj, %slot);
	cancel(%obj.flareSchedule);
	cancel(%obj.flareScheduleB);
}

function RedFlareImage::onFire(%this, %obj, %slot)
{
	if($Sim::Time - %obj.lastRedFlare < 10)
	{
		// %obj.client.play2d(brickErrorSound);
		return;
	}
	%obj.lastRedFlare = $Sim::Time;
	%obj.playThread(1, armReadyRight);
	%obj.playThread(2, spearReady);
	%obj.flareSchedule = %obj.schedule(500, fireFlare, "red");
	%obj.flareScheduleB = %obj.schedule(500, playThread, 3, plant);
	%obj.flareSchedule = %obj.schedule(1000, playThread, 2, root);
	%obj.flareScheduleB = %obj.schedule(1000, playThread, 1, root);
}

datablock itemData(BlackFlareItem : GreenFlareItem)
{
	image = BlackFlareImage;
	uiName = "Flare - Black";
	doColorShift = true;
	colorShiftColor = "0 0 0 1";
};

datablock ShapeBaseImageData(BlackFlareImage : GreenFlareImage)
{
	doColorShift = true;
	colorShiftColor = "0 0 0 1";
};

function BlackFlareImage::onMount(%this, %obj, %slot)
{
	parent::onMount(%this, %obj, %slot);
	%obj.playThread(1, root);
}

function BlackFlareImage::onUnMount(%this, %obj, %slot)
{
	parent::onUnMount(%this, %obj, %slot);
	cancel(%obj.flareSchedule);
	cancel(%obj.flareScheduleB);
}

function BlackFlareImage::onFire(%this, %obj, %slot)
{
	if($Sim::Time - %obj.lastBlackFlare < 10)
	{
		// %obj.client.play2d(brickErrorSound);
		return;
	}
	%obj.lastBlackFlare = $Sim::Time;
	%obj.playThread(1, armReadyRight);
	%obj.playThread(2, spearReady);
	%obj.flareSchedule = %obj.schedule(500, fireFlare, "black");
	%obj.flareScheduleB = %obj.schedule(500, playThread, 3, plant);
	%obj.flareSchedule = %obj.schedule(1000, playThread, 2, root);
	%obj.flareScheduleB = %obj.schedule(1000, playThread, 1, root);
}