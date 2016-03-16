function TDMGSwordImage::onMount(%this,%obj,%slot)
{
	parent::onMount(%this,%obj,%slot);
	%obj.playThread(1, root);
	if(%obj.TDMGBlades <= 0)
	{
		%obj.mountImage(TDMGBrokenSwordImage, 0);
		return;
	}
	if(%obj.getMountedImage(1) != TDMGModelImage.getId())
	{
		%obj.mountImage(TDMGModelImage,1);
	}
	%obj.mountImage(TDMGSecondBladeImage,2);
}

function TDMGSwordImage::onUnMount(%this,%obj,%slot)
{
	parent::onUnMount(%this,%obj,%slot);
	// %obj.unmountImage(1);
	%obj.unmountImage(2);
}

// TODO: Make hit detection for titan weakpoints in titan's damage function, so other weapons could work.
function TDMGSwordImage::onFire(%this, %obj, %slot)
{
	if($Sim::Time - %obj.lastSwing < 0.5)
	{
		return;
	}
	if(%obj.TDMGBlades <= 0)
	{
		%obj.mountImage(TDMGBrokenSwordImage, 0);
		return;
	}
	if (!isObject(%obj) || %obj.getState() $= "Dead")
		return;
	%obj.lastSwing = $Sim::Time;
	%obj.doSwordRaycast(0);
	%rnd = getRandom(16, 100);
	%obj.swordRayCastSchedule = %obj.schedule(%rnd, doSwordRaycast, 2);
	%obj.playThread(2, shiftUp);
	%obj.playThread(3, shiftAway);
	%obj.schedule(%rnd, playThread, 3, leftRecoil);
}

function TDMGBrokenSwordImage::onMount(%this,%obj,%slot)
{
	parent::onMount(%this,%obj,%slot);
	// if(%obj.getMountedImage(1) != TDMGModelImage.getId())
	// {
	// 	%obj.mountImage(TDMGModelImage,1);
	// }
	%obj.mountImage(TDMGSecondBrokenBladeImage,2);
	%obj.playThread(1, root);
}

function TDMGBrokenSwordImage::onUnMount(%this,%obj,%slot)
{
	parent::onUnMount(%this,%obj,%slot);
	// %obj.unmountImage(1);
	%obj.unmountImage(2);
}

function TDMGBrokenSwordImage::onFire(%this, %obj, %slot)
{
}

function Player::doSwordRaycast(%obj, %slot)
{
	if (!isObject(%obj) || %obj.getState() $= "Dead")
		return;
	serverPlay3d(TDMGSwordSliceSound, %obj.getHackPosition());
	%scale = getWord(%obj.getScale(), 2);
	%start = %obj.getEyePoint();
	%pos = vectorAdd(%start, vectorScale(%obj.getEyeVector(), 5 * %scale));
	%start = %obj.getMuzzlePoint(%slot);
	%ray = containerRayCast(%start, %pos,
		$TypeMasks::StaticObjectType  |
		$TypeMasks::fxBrickObjectType |
		$TypeMasks::PlayerObjectType,
		%obj
	);
	%pos = getWords(%ray, 1, 3);
	if(isObject(%col = getWord(%ray, 0)) && ((%cn = %col.getClassName()) $= "Player" || %cn $= "AIPlayer"))
	{
		%obj.TDMGBlades = %obj.TDMGBlades - 2;
		if(%obj.TDMGBlades <= 0)
		{
			%obj.TDMGBlades = 0;
			cancel(%obj.swordRayCastSchedule);
			%obj.mountImage(TDMGBrokenSwordImage, 0);
			%p = new Projectile()
			{
				dataBlock = TDMGBladeDebrisProjectile;

				initialPosition = %obj.getHackPosition();
				initialVelocity = %obj.getVelocity();

				client = %obj.client;
				sourceObject = %obj;
			};
			%p.explode();
		}
		%p = new Projectile()
		{
			dataBlock = SwordProjectile;

			initialPosition = %pos;
			initialVelocity = %obj.getVelocity();

			client = %obj.client;
			sourceObject = %obj;
		};
		%p.explode();

		if ( %col.getClassName() $= "Player" && !%col.getDatablock().isTitan)
			return;

		%col.damage(%obj, %pos, 40, $DamageType::Sword);
	}
}
