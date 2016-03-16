datablock ProjectileData(TitanFistProjectile)
{
	directDamage = 0;
	radiusDamage = 0;
	damageRadius = 0;
	particleEmitter = TitanFistTrailEmitter;

	muzzleVelocity = 10;
	velInheritFactor = 1;

	armingDelay = 0;
	lifetime = 250;
	fadeDelay = 250;
	bounceElasticity = 0;
	bounceFriction = 0;
	isBallistic = true;
	gravityMod = 0;

	hasLight = false;
	lightRadius	= 3.0;
	lightColor	= "0 0 0.5";
};

datablock ItemData(TitanFistItem)
{
	category = "Tool";
	className = "Tool";

	shapeFile = "base/data/shapes/empty.dts";
	mass = 1;
	density	= 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "T: Punch / Grab";
	iconName = "./icon_Fist";
	doColorShift = true;
	colorShiftColor = "1 1 1 1";

	image = TitanFistImage;
	canDrop	= true;
};

datablock ShapeBaseImageData(TitanFistImage)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = true;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = "0 0 0";
	correctMuzzleVector = true;
	className = "WeaponImage";

	item = TitanFistItem;
	ammo = " ";
	projectile = swordProjectile;
	projectileType = Projectile;

	melee = true;
	doRetraction = false;
	armReady = false;

	stateName[0]				= "Activate";
	stateTimeoutValue[0]	    = 0.2;
	stateTransitionOnTimeout[0] = "Ready";
	stateScript[0]				= "onRelease";
	stateSound[0]				= weaponSwitchSound;

	stateName[1]                    = "Ready";
	stateTransitionOnTriggerDown[1] = "Fire";
	stateAllowImageChange[1]        = true;

	stateName[2]				= "Fire";
	stateTransitionOnTimeout[2] = "Hold";
	stateTimeoutValue[2]	    = 0.2;
	stateFire[2]				= true;
	stateAllowImageChange[2]    = false;
	stateSequence[2]		    = "Fire";
	stateScript[2]				= "onFire";
	stateWaitForTimeout[2]	    = true;
	stateSound[2]               = TDMGSwordSliceSound;

	stateName[3]				  = "Hold";
	stateTimeoutValue[3]		  = 0.01;
	stateScript[3]				  = "onHold";
	stateTransitionOnTimeout[3]	  = "Hold";
	stateTransitionOnTriggerUp[3] = "Release";

	statename[4]				= "Release";
	stateAllowImageChange[4]    = false;
	stateTimeoutValue[4]	    = 0.01;
	stateTransitionOnTimeout[4] = "Ready";
	stateScript[4]				= "onRelease";
};

function TitanFistImage::onFire(%this, %obj, %slot)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

	%obj.playThread(1, armReadyRight);
	%obj.playThread(2, spearReady);
	%obj.playThread(3, jump);
	%obj.schedule(75, TitanPunchRight);
}

function createSphere(%pos, %scale) {
	if(getWordCount(%scale) < 2)
	{
		%scale = %scale SPC %scale SPC %scale;
	}
	%obj = new StaticShape() {
		datablock = SphereShapeData;
		scale = %scale;

		position = %pos;
	};
	// %obj.setNodeColor("ALL", "0.3 0.3 0.3 1.0" );
	return %obj;
}

function Player::TitanPunchRight(%obj)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}
	%obj.playThread(1, armReadyRight);
	%obj.playThread(2, root);
	%obj.playThread(3, plant);
	//%obj.addVelocity(vectorAdd(vectorScale(%obj.getForwardVector(), 3), "0 0 1"));
	serverPlay3d(TitanPunchSound, %obj.getHackPosition());

	%scale = getWord(%obj.getScale(), 2);
	// %start = %obj.getHackPosition();
	%start = vectorAdd(%obj.getHackPosition(), vectorCross(vectorScale(%obj.getForwardVector(), 0.5), "0 0" SPC 1 * %scale));
	%end = vectorAdd(%start, vectorScale(%obj.getForwardVector(), 2 * %scale));
	%end = vectorAdd(%end, vectorCross(vectorScale(%obj.getForwardVector(), 0.25), "0 0" SPC -1 * %scale));
	%ray = containerRayCast(%start, %end,
		$TypeMasks::PlayerObjectType  |
		$TypeMasks::fxBrickObjectType |
		$TypeMasks::StaticObjectType,
		%obj
	);
	if(isObject(%col = getWord(%ray, 0)))
	{
		%end = getWords(%ray, 1, 3);
		if(%scale > 3)
		{
			%scale = 3;
		}
		%p = new Projectile()
		{
			dataBlock = TitanFistExplosionProjectile;
			scale = %scale SPC %scale SPC %scale;//%obj.getScale();

			initialPosition = %end;
			initialVelocity = %obj.getVelocity();

			client = %obj.client;
			sourceObject = %obj;
		};
		MissionCleanup.add(%p);
		%p.explode();
		if(%col.getType() & $TypeMasks::PlayerObjectType)
			%col.damage(%obj, %end, getRandom(80, 120), $DamageType::Suicide);
	}

	%p = new Projectile()
	{
		dataBlock = TitanFistProjectile;
		scale = %obj.getScale();

		initialPosition = %start;
		initialVelocity = vectorScale(vectorNormalize(vectorSub(%end, %start)), TitanFistProjectile.muzzleVelocity * %scale);

		client = %obj.client;
		sourceObject = %obj;
	};
	MissionCleanup.add(%p);
	%obj.schedule(125, titanPunchLeft);
}

function Player::TitanPunchLeft(%obj)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}
	%obj.playThread(1, armReadyLeft);
	%obj.playThread(2, leftRecoil);
	%obj.playThread(3, plant);
	//%obj.addVelocity(vectorAdd(vectorScale(%obj.getForwardVector(), 3), "0 0 1"));
	serverPlay3d(TitanPunchSound, %obj.getHackPosition());

	%scale = getWord(%obj.getScale(), 2);
	// %start = %obj.getHackPosition();
	%start = vectorAdd(%obj.getHackPosition(), vectorCross(vectorScale(%obj.getForwardVector(), 0.5), "0 0" SPC -1 * %scale));
	%end = vectorAdd(%start, vectorScale(%obj.getForwardVector(), 2 * %scale));
	%end = vectorAdd(%end, vectorCross(vectorScale(%obj.getForwardVector(), 0.25), "0 0" SPC 1 * %scale));
	// %start = vectorAdd(%start, "0 0" SPC 0.5 * %scale);
	%ray = containerRayCast(%start, %end,
		$TypeMasks::PlayerObjectType  |
		$TypeMasks::fxBrickObjectType |
		$TypeMasks::StaticObjectType,
		%obj
	);
	if(isObject(%col = getWord(%ray, 0)))
	{
		%end = getWords(%ray, 1, 3);
		if(%scale > 3)
		{
			%scale = 3;
		}
		%p = new Projectile()
		{
			dataBlock = TitanFistExplosionProjectile;
			scale = %scale SPC %scale SPC %scale;//%obj.getScale();

			initialPosition = %end;
			initialVelocity = %obj.getVelocity();

			client = %obj.client;
			sourceObject = %obj;
		};
		MissionCleanup.add(%p);
		%p.explode();
		if(%col.getType() & $TypeMasks::PlayerObjectType)
			%col.damage(%obj, %end, getRandom(80, 120), $DamageType::Suicide);
	}
	%p = new Projectile()
	{
		dataBlock = TitanFistProjectile;
		scale = %obj.getScale();

		initialPosition = %start;
		initialVelocity = vectorScale(vectorNormalize(vectorSub(%end, %start)), TitanFistProjectile.muzzleVelocity * %scale);

		client = %obj.client;
		sourceObject = %obj;
	};
	MissionCleanup.add(%p);
	// %test = createRope(%start, %end);
	// %test.schedule(300, delete);

	%obj.schedule(125, titanPunchSlam);
}

function Player::TitanPunchSlam(%obj)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}
	%obj.playThread(1, armReadyBoth);
	//%obj.addVelocity(vectorAdd(vectorScale(%obj.getForwardVector(), 5), "0 0 1"));
	%obj.schedule(25/2, playThread, 2, shiftUp);
	%obj.schedule(75/2, playThread, 2, shiftDown);
	%obj.schedule(25, setActionThread, look);
	%obj.schedule(50, setActionThread, root);
	schedule(75, 0, serverPlay3d, TitanPunchSound, %obj.getHackPosition());
	%obj.schedule(50, TitanPunchSlamRay);
	%obj.schedule(500, titanPunchEnd);
}

function player::TitanPunchSlamRay(%obj)
{
	%start = %obj.getHackPosition();
	%scale = getWord(%obj.getScale(), 2);
	%end = vectorAdd(%start, vectorScale(%obj.getForwardVector(), 2 * %scale));
	%end = vectorAdd(%end, "0 0" SPC -2 * %scale);
	%start = vectorAdd(%start, "0 0" SPC 1.5 * %scale);
	%ray = containerRayCast(%start, %end,
		$TypeMasks::StaticObjectType  |
		$TypeMasks::fxBrickObjectType |
		$TypeMasks::PlayerObjectType,
		%obj
	);
	if(isObject(%col = getWord(%ray, 0)))
	{
		%end = getWords(%ray, 1, 3);
		if(%scale > 3)
		{
			%scale = 3;
		}
		%p = new Projectile()
		{
			dataBlock = TitanFistExplosionProjectile;
			scale = %scale SPC %scale SPC %scale;//%obj.getScale();

			initialPosition = %end;
			initialVelocity = %obj.getVelocity();

			client = %obj.client;
			sourceObject = %obj;
		};
		MissionCleanup.add(%p);
		%p.explode();
		if(%col.getType() & $TypeMasks::PlayerObjectType)
			%col.damage(%obj, %end, getRandom(90, 150), $DamageType::Suicide);
	}

	%p = new Projectile()
	{
		dataBlock = TitanFistProjectile;
		scale = %obj.getScale();

		initialPosition = %start;
		initialVelocity = vectorScale(vectorNormalize(vectorSub(%end, %start)), TitanFistProjectile.muzzleVelocity * %scale);

		client = %obj.client;
		sourceObject = %obj;
	};

	MissionCleanup.add(%p);
}

function Player::TitanPunchEnd(%obj)
{
	%obj.playThread(1, root);
	// %obj.setActionThread(root);
}

datablock ItemData(TitanKickItem : TitanFistItem)
{
	uiName = "T: Kick / Bite";
	image = TitanKickImage;
};

datablock ShapeBaseImageData(TitanKickImage)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = true;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = "0 0 0";
	correctMuzzleVector = true;
	className = "WeaponImage";

	item = TitanKickItem;
	ammo = " ";
	projectile = swordProjectile;
	projectileType = Projectile;

	melee = true;
	doRetraction = false;
	armReady = false;

	stateName[0]				= "Activate";
	stateTimeoutValue[0]	    = 0.2;
	stateTransitionOnTimeout[0] = "Ready";
	stateScript[0]				= "onRelease";
	stateSound[0]				= weaponSwitchSound;

	stateName[1]                    = "Ready";
	stateTransitionOnTriggerDown[1] = "Fire";
	stateAllowImageChange[1]        = true;

	stateName[2]				= "Fire";
	stateTransitionOnTimeout[2] = "Hold";
	stateTimeoutValue[2]	    = 0.2;
	stateFire[2]				= true;
	stateAllowImageChange[2]    = false;
	stateSequence[2]		    = "Fire";
	stateScript[2]				= "onFire";
	stateWaitForTimeout[2]	    = true;
	stateSound[2]               = TDMGSwordSliceSound;

	stateName[3]				  = "Hold";
	stateTimeoutValue[3]		  = 0.01;
	stateScript[3]				  = "onHold";
	stateTransitionOnTimeout[3]	  = "Hold";
	stateTransitionOnTriggerUp[3] = "Release";

	statename[4]				= "Release";
	stateAllowImageChange[4]    = false;
	stateTimeoutValue[4]	    = 0.01;
	stateTransitionOnTimeout[4] = "Ready";
	stateScript[4]				= "onRelease";
};

function TitanKickImage::onFire(%this, %obj, %slot)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}
	%obj.playThread(1, root);
	%obj.playThread(2, jump);
	%obj.playThread(3, jump);
	%obj.schedule(125, TitanKick);
}

function Player::TitanKick(%obj)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}
	%obj.playThread(1, walk);
	%obj.playThread(2, plant);
	%obj.playThread(3, plant);
	%obj.schedule(50, playThread, 1, root);
	serverPlay3d(TitanPunchSound, %obj.getHackPosition());
	%scale = getWord(%obj.getScale(), 2);
	%start = vectorAdd(%obj.getPosition(), "0 0" SPC %scale * 0.5);
	%end = vectorAdd(%start, vectorScale(%obj.getForwardVector(), %scale));
	%end = vectorAdd(%end, "0 0" SPC -0.65 * %scale);
	%ray = containerRayCast(%start, %end,
		$TypeMasks::StaticObjectType  |
		$TypeMasks::fxBrickObjectType |
		$TypeMasks::PlayerObjectType,
		%obj
	);
	if(isObject(%col = getWord(%ray, 0)))
	{
		%end = getWords(%ray, 1, 3);
		if(%scale > 3)
		{
			%scale = 3;
		}
		%p = new Projectile()
		{
			dataBlock = TitanFistExplosionProjectile;
			scale = %scale SPC %scale SPC %scale;

			initialPosition = %end;
			initialVelocity = %obj.getVelocity();

			client = %obj.client;
			sourceObject = %obj;
		};
		MissionCleanup.add(%p);
		%p.explode();
		if(%col.getType() & $TypeMasks::PlayerObjectType)
			%col.damage(%obj, %end, getRandom(90, 150), $DamageType::Suicide);
		//%obj.addVelocity(vectorAdd(vectorScale(%obj.getForwardVector(), 5), "0 0 2"));

	}
	// %test = createRope(%start, %end);
	// %test.schedule(300, delete);
}

function Player::TitanEat(%obj, %col)
{
	%col.canDismount = true;
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}
	if(!%obj.isEating || !isObject(%col) || %col.getState() $= "Dead" || %obj.getMountedObject(0) != %col)
	{
		%obj.playThread(1, root);
		%obj.playThread(3, root);
		%obj.setControlObject(%obj);
		return;
	}
	%obj.playThread(2, plant);
	%obj.schedule(100, playThread, 1, root);
	%obj.schedule(100, playThread, 3, root);
	%obj.schedule(500, setControlObject, %obj);
	%col.damage(%obj, %col.getPosition(), 10000, $DamageType::Suicide);
	// createBloodExplosion(vectorAdd(%obj.getHackPosition(), "0 0 2"), %obj.getVelocity(), %obj.getScale());
	serverPlay3d(TitanEatSound, %col.getHackPosition());
	if(isObject(%col.client))
	{
		%col.client.camera.setMode("Corpse", %col);
		%col.client.setControlObject(%col.client.camera);
	}
	%col.schedule(16, delete);
}

function Player::DoBite(%obj)
{
	%scale = getWord(%obj.getScale(), 2);
	%range = %scale * 0.5;
	%masks =
		$TypeMasks::playerObjectType |
		$TypeMasks::fxBrickObjectType |
		$TypeMasks::vehicleObjectType |
		$TypeMasks::ShapeBaseObjectType |
		$TypeMasks::TerrainObjectType;

	%pos = %obj.getEyePoint();
	initContainerRadiusSearch(%pos, %range, %masks);
	%test = createSphere(%pos, %range * 2);
	%test.schedule(300, delete);
	while (isObject(%col = containerSearchNext()))
	{
		%dot = vectorDot(%obj.getForwardVector(), vectorNormalize(vectorSub(%col.getHackPosition(), %obj.getHackPosition())));
		if(%col == %obj) continue;
		if(%dot < 0.75) continue;
		if(%col.getType() & $Typemasks::PlayerObjectType)
		{
			%col.damage(%obj, %col.getPosition(), 10000, $DamageType::Suicide);
			// createBloodExplosion(vectorAdd(%obj.getHackPosition(), "0 0 2"), %obj.getVelocity(), %obj.getScale());
			serverPlay3d(TitanEatSound, %col.getHackPosition());
			if(isObject(%col.client))
			{
				%col.client.camera.setMode("Corpse", %col);
				%col.client.setControlObject(%col.client.camera);
			}
			%col.schedule(16, delete);
			%found = true;
		}
	}
}

package TitanFistPackage
{
	function armor::onTrigger(%this,%obj,%slot,%val)
	{
		Parent::onTrigger(%this,%obj,%slot,%val);

		if(%slot $= 4)
		{
			if(%val)
			{
				if(%obj.getMountedImage(0) == TitanFistImage.getID() || %obj.getMountedImage(0) == TitanKickImage.getID())
				{
					if($Sim::Time - %obj.lastGrabAttempt < 0.75)
					{
						return;
					}
					%obj.lastGrabAttempt = $Sim::Time;
					serverPlay3d(TitanPunchSound, %obj.getHackPosition());
					%obj.playThread(1, armReadyRight);
					%obj.playThread(2, shiftAway);
					%scale = getWord(%obj.getScale(), 2);
					%start = %obj.getEyePoint();
					%end = vectorAdd(%start, vectorScale(%obj.getEyeVector(), 3 * %scale));
					%ray = containerRayCast(%start, %end,
						$TypeMasks::StaticObjectType  |
						$TypeMasks::fxBrickObjectType |
						$TypeMasks::PlayerObjectType,
						%obj
					);
					%end = getWords(%ray, 1, 3);
					%col = getWord(%ray, 0);
					if(isObject(%col) && (%col.getType() & $TypeMasks::PlayerObjectType) && !%col.getDatablock().isTitan)
					{
						if(isObject(%obj.client.camera))
						{
							%obj.client.camera.setOrbitMode(%obj, %pos, 0.5, 8, 8, 1);
							%obj.client.camera.mode = "Orbit";
							%obj.client.camera.setTransform(%obj.getEyeTransform());
							%obj.setControlObject(%obj.client.camera);
						}
						%obj.schedule(1500, playThread, 3, spearReady);
						%obj.mountObject(%col, 0);
						%col.canDismount = false;
						%obj.isEating = true;
						%obj.titanEatSchedule = %obj.schedule(3000, TitanEat, %col);
					}
					else
					{
						%obj.playThread(1, root);
					}
				}
			}
		}
	}

};
if(isPackage(TitanFistPackage))
	deactivatePackage(TitanFistPackage);
activatePackage(TitanFistPackage);
