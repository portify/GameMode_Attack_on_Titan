function AIPlayer::updateTitan(%this)
{
	cancel(%this.updateTitan);

	if (%this.getState() $= "Dead")
	{
		return;
	}

	%eyePoint = %this.getEyePoint();
	%eyeVector = %this.getEyeVector();

	if (isObject(%this.target) && %this.target.getState() $= "Dead" || %this.isBlind)
	{
		%this.target = 0;
	}

	if (!isObject(%this.target) || !%this.canTitanSee(%this.target))
	{
		initContainerRadiusSearch(%eyePoint, 64, $TypeMasks::PlayerObjectType);

		while (%obj = containerSearchNext())
		{
			if (%obj.getDataBlock().isTitan)
			{
				continue;
			}

			if (!%this.canTitanSee(%obj))
			{
				continue;
			}

			%this.target = %obj;
			break;
		}
	}

	%scale = getWord(%this.getScale(), 2);

	if (isEventPending(%this.attackSchedule) || isEventPending(%this.stopBlindSchedule) || %this.isEating || %this.isAttacking)
	{
		%this.stop();
		%this.clearAim();
	}
	else if (isObject(%this.target))
	{
		%this.nextWanderTime = $Sim::Time + 3;
		%this.maxYawSpeed = %this.getDataBlock().maxYawSpeed;
		%this.setMoveSpeed(1);

		%this.setMoveObject(%this.target);
		%this.setAimObject(%this.target);
		if (%this.getDatablock().canJump && getWord(%this.target.getHackPosition(), 2) - getWord(%this.getHackPosition(), 2) >= 5 && %this.isOnGround())
		{
			// if($Sim::Time - %this.lastJump >= 15)
				%this.doJumpAttack(%this.target);
		}
		if ($Sim::Time - %this.lastAttackTime > %this.getDatablock().attackCooldown && vectorDist(%this.getHackPosition(), %this.target.getHackPosition()) <= 3 * %scale)
		{
			%this.titanCheckAttack(%this.target);//%this.titanComboSchedule();
			%this.stop();
			%this.clearAim();
			%this.setMoveX(0);
		}
	}
	else
	{
		%this.maxYawSpeed = %this.getDataBlock().maxYawSpeed * 3;

		%this.setMoveSpeed(0.8);

		%this.setMoveObject(0);
		%this.clearAim();

		if ($Sim::Time >= %this.nextWanderTime)
		{
			%angle = getRandom() * $pi * 2;
			%vector = vectorScale(mSin(%angle) SPC mCos(%angle), 25 + getRandom() * 15);

			%start = %this.getHackPosition();
			%target = vectorAdd(%start, %vector);

			%ray = containerRayCast(%start, %target, $TypeMasks::FxBrickObjectType);

			if (%ray)
			{
				%normal = getWords(%ray, 4, 6);
				%target = vectorAdd(getWords(%ray, 1, 3), vectorScale(%normal, 2 * %scale));
			}

			%this.setMoveDestination(%target);
			%this.nextWanderTime = $Sim::Time + 2 + getRandom() * 3;
		}
	}
	%this.applyStrafe();
	%this.updateTitan = %this.schedule(100, "updateTitan");
}

function AIPlayer::applyStrafe(%this, %scale, %length, %mask)
{
	if (%scale $= "")
	{
		%scale = 1;
	}

	if (%length $= "")
	{
		%length = 2;
	}

	if (%mask $= "")
	{
		%mask =
			$TypeMasks::PlayerObjectType |
			$TypeMasks::VehicleObjectType |
			$TypeMasks::FxBrickObjectType |
			$TypeMasks::StaticShapeObjectType;
	}

	%a = %this._strafeProbe(0, %length, %mask);
	%b = %this._strafeProbe(1, %length, %mask);

	%strafe = %a && %b ? %this.lastStrafe : %a + %b * -1;

	%this.setMoveX(%strafe * %scale);
	%this.lastStrafe = %strafe;
}

function Player::_strafeProbe(%this, %direction, %length, %mask)
{
	%scale = getWord(%this.getScale(), 2);
	%upward = %this.getUpVector();

	if (!%direction)
	{
		%upward = vectorScale(%upward, -1);
	}

	%forward = %this.getForwardVector();
	%origin = %this.getHackPosition();

	%side = vectorCross(%forward, %upward);
	%start = vectorAdd(%origin, vectorScale(%side, 0.6 * %scale));

	%end = vectorAdd(%start, vectorScale(%forward, %length * %scale));
	%end = vectorAdd(%end, vectorScale(%side, %length * %scale * 0.25));

	return containerRayCast(%start, %end, %mask, %this);
}

function AIPlayer::titanCheckAttack(%this, %target)
{
	// %this.titanComboSchedule();
	if(getWord(%target.getHackPosition(), 2) - getWord(%this.getHackPosition(), 2) <= -0.2)
	{
		%this.AITitanKick();
	}
	else
	{
		if(getRandom() > 0.35)
		{
			%this.TitanGrab();
		}
		else
		{
			%this.titanComboSchedule();
		}
	}
}

function AIPlayer::titanComboSchedule(%this, %n)
{
	cancel(%this.attackSchedule);
	if(!isObject(%this) || %this.getState() $= "Dead")
	{
		return;
	}
	%this.isAttacking = true;
	if(%n > 2)
	{
		%this.schedule(%this.getDatablock().attackRecovery * 1000, titanAttackRecovery);
		return;
	}
	if(%n == 2)
	{
		%this.TitanSlam();
		%this.isBlind = false;
	}
	else
		%this.titanPunch(%n);
	%n = %n + 1;
	%this.attackSchedule = %this.schedule(400, titanComboSchedule, %n);
}

function AIPlayer::titanAttackRecovery(%this)
{
	if(!isObject(%this) || %this.getState() $= "Dead")
	{
		return;
	}
	%this.playThreadIfAlive(1, "root");
	%this.playThreadIfAlive(2, "root");
	%this.isAttacking = false;
}

function AIPlayer::doJumpAttack(%this, %target)
{
	if(!isObject(%this) || %this.getState() $= "Dead")
	{
		return;
	}
	// if(!isObject(%target) || vectorDist(%tpos = %target.getHackPosition(), %this.getHackPosition()) > 50)
	// {
	// 	%vel = vectorAdd(%this.getVelocity(), "0 0 50");
	// }
	// else
	// {
		%vec = vectorSub(%target.getHackPosition(), %this.getHackPosition());
		%vel = %vec;
		// echo(%vel);
	// }
	// %this.lastJump = $Sim::Time;
	%this.setVelocity(%vel);
	%this.setActionThread(Jump);
	%this.isJumpAttacking = true;
	%this.jumpAttackCheck();
}

function AIPlayer::jumpAttackCheck(%this)
{
	cancel(%this.jumpAttackCheck);
	if(!isObject(%this) || %this.getState() $= "Dead")
	{
		return;
	}
	if(getWord(%this.getVelocity(), 2) < 0)
	{
		%this.isJumpAttacking = false;
		return;
	}
	%this.jumpAttackCheck = %this.schedule(100, jumpAttackCheck);
}

function AIPlayer::TitanSlam(%this)
{
	if(!isObject(%this) || %this.getState() $= "Dead")
	{
		return;
	}
	%this.playThread(1, armReadyBoth);
	%this.schedule(50, playThreadIfAlive, 2, shiftUp);
	%this.schedule(150, playThreadIfAlive, 2, shiftDown);
	%this.schedule(100, setActionThread, look);
	%this.schedule(200, setActionThread, root);
	%this.schedule(200, TitanSlamRay);
}

function AIPlayer::TitanSlamRay(%this)
{
	if(!isObject(%this) || %this.getState() $= "Dead")
	{
		return;
	}
	%this.lastAttackTime = $Sim::Time;
	%scale = getWord(%this.getScale(), 2);

	%start = %this.getHackPosition();
	// %vector = %this.getForwardVector();
	// %end = vectorAdd(%start, vectorScale(%vector, 2*%scale));
	// %start = vectorAdd(%start, "0 0" SPC 1.5 * %scale);
	// %col = hullTrace(%start, %end, %scale * 2 SPC %scale * 2 SPC %scale * 2, $TypeMasks::FxBrickObjectType | $TypeMasks::PlayerObjectType, %this);
	// if(isObject(%col))
	// {
	// 	%col.damage(%this, %hand, getRandom(80, 120), $DamageType::Suicide);
	// }
	initContainerRadiusSearch(%start, 0.25 * %scale, $TypeMasks::PlayerObjectType);

	while (%obj = containerSearchNext())
	{
		if (%obj.getDataBlock().isTitan)
		{
			continue;
		}

		%direct = vectorSub(%obj.getHackPosition(), %hand);
		%delta = vectorDot(%forwardVector, vectorNormalize(%direct));

		if (%delta >= 0.65)
		{
			%obj.damage(%this, %hand, getRandom(80, 120), $DamageType::Suicide);
			// break;
		}
	}
}

function AIPlayer::titanPunch(%this, %side)
{
	if(!isObject(%this) || %this.getState() $= "Dead")
	{
		return;
	}
	if (%side)
	{
		%this.playThreadIfAlive(1, "armReadyRight");
		%this.playThreadIfAlive(2, "root");
	}
	else
	{
		%this.playThreadIfAlive(1, "armReadyLeft");
		%this.playThreadIfAlive(2, "leftRecoil");
	}

	%this.lastAttackTime = $Sim::Time;
	%this.playThreadIfAlive(3, "plant");
	%this.addVelocity(vectorAdd(vectorScale(%this.getForwardVector(), 3), "0 0 2"));

	serverPlay3D(TitanPunchSound, %this.getHackPosition());

	%scale = getWord(%this.getScale(), 2);

	%hackPosition = %this.getHackPosition();
	%forwardVector = %this.getForwardVector();

	%crossed = vectorCross(%forwardVector, "0 0" SPC %side ? 1 : -1);
	%hand = vectorAdd(%hackPosition, vectorScale(%crossed, 0.5 * %scale));
	// %end = vectorAdd(%hand, vectorScale(%vector, 2*%scale));
	// %col = hullTrace(%hand, %end, %scale SPC %scale SPC %scale, $TypeMasks::FxBrickObjectType | $TypeMasks::PlayerObjectType, %this);
	// if(isObject(%col))
	// {
	// 	%col.damage(%this, %hand, getRandom(80, 120), $DamageType::Suicide);
	// }

	initContainerRadiusSearch(%hand, 0.2 * %scale, $TypeMasks::PlayerObjectType);

	while (%obj = containerSearchNext())
	{
		if (%obj.getDataBlock().isTitan)
		{
			continue;
		}

		%direct = vectorSub(%obj.getHackPosition(), %hand);
		%delta = vectorDot(%forwardVector, vectorNormalize(%direct));
		if (%delta >= 0.65)
		{
			%obj.damage(%this, %hand, getRandom(80, 120), $DamageType::Suicide);
			// break;
		}
	}
}

function AIPlayer::AITitanKick(%this)
{
	if(!isObject(%this) || %this.getState() $= "Dead")
	{
		return;
	}
	%this.setActionThread(root);
	%this.playThreadIfAlive(1, walk);
	%this.playThreadIfAlive(2, plant);
	%this.playThreadIfAlive(3, plant);
	%this.schedule(200, playThreadIfAlive, 1, root);
	serverPlay3d(TitanPunchSound, %this.getHackPosition());
	%scale = getWord(%this.getScale(), 2);
	%start = %this.getPosition();
	%this.lastAttackTime = $Sim::Time;
	%this.isAttacking = true;
	%this.schedule(%this.getDatablock().attackRecovery * 500, titanAttackRecovery);
	initContainerRadiusSearch(%start, 0.25 * %scale, $TypeMasks::PlayerObjectType);

	while (%obj = containerSearchNext())
	{
		if (%obj.getDataBlock().isTitan)
		{
			continue;
		}

		%direct = vectorSub(%obj.getHackPosition(), %start);
		%delta = vectorDot(%this.getForwardVector(), vectorNormalize(%direct));

		if (%delta >= 0.75)
		{
			%obj.damage(%obj, %start, getRandom(80, 120), $DamageType::Suicide);
			// break;
		}
	}
}

function AIPlayer::TitanGrab(%this)
{
	if(!isObject(%this) || %this.getState() $= "Dead")
	{
		return;
	}
	serverPlay3d(TitanPunchSound, %this.getHackPosition());
	%this.lastAttackTime = $Sim::Time;
	%this.playThreadIfAlive(1, armReadyRight);
	%this.playThreadIfAlive(2, shiftAway);
	%scale = getWord(%this.getScale(), 2);

	%hackPosition = %this.getHackPosition();
	%forwardVector = %this.getForwardVector();
	%start = vectorAdd(%hackPosition, vectorScale(%forwardVector, 0.5 * %scale));
	initContainerRadiusSearch(%start, 0.4 * %scale, $TypeMasks::PlayerObjectType);

	while (%obj = containerSearchNext())
	{
		if (%obj.getDataBlock().isTitan || %obj.isBeingEaten)
		{
			continue;
		}

		%direct = vectorSub(%obj.getHackPosition(), %start);
		%delta = vectorDot(%forwardVector, vectorNormalize(%direct));

		if (%delta >= 0.75)
		{
			%this.schedule(1500, playThreadIfAlive, 3, spearReady);
			%this.mountObject(%obj, 0);
			%obj.setControlObject(%obj);
			if(isObject(%obj.client)) serverCmdUnuseTool(%obj.client);
			%obj.canDismount = false;
			%obj.isBeingEaten = true;
			%this.isEating = true;
			%this.attackSchedule = %this.schedule(3000, TitanEat, %obj);
			%found = true;
			break;
		}
	}
	if(!%found) %this.playThreadIfAlive(1, root);
}

function AIPlayer::TitanEat(%this, %col)
{
	%col.canDismount = true;
	if(!isObject(%this) || %this.getState() $= "Dead")
	{
		return;
	}
	if(!%this.isEating || !isObject(%col) || %col.getState() $= "Dead" || %this.getMountedObject(0) != %col)
	{
		%this.playThreadIfAlive(1, root);
		%this.playThreadIfAlive(3, root);
		%this.stopEat();
		return;
	}
	%this.playThreadIfAlive(2, plant);
	%this.schedule(100, playThreadIfAlive, 1, root);
	%this.schedule(100, playThreadIfAlive, 3, root);
	%col.damage(%this, %col.getPosition(), 10000, $DamageType::Suicide);
	// createBloodExplosion(vectorAdd(%this.getHackPosition(), "0 0 2"), %this.getVelocity(), %this.getScale());
	serverPlay3d(TitanEatSound, %col.getHackPosition());
	if(isObject(%col.client))
	{
		%col.client.camera.setMode("Corpse", %col);
		%col.client.setControlObject(%col.client.camera);
	}
	%col.schedule(16, delete);
	%this.schedule(1500, StopEat);
	%this.schedule(750, playThreadIfAlive, 2, "plant");
}

function AIPlayer::StopEat(%this)
{
	if(!isObject(%this) || %this.getState() $= "Dead")
	{
		return;
	}
	%this.isEating = false;
	%this.playThreadIfAlive(2, "plant");
}

function AIPlayer::TitanEyeStab(%this)
{
	if(!isObject(%this) || %this.getState() $= "Dead")
	{
		return;
	}
	if ( %this.isEating )
		return;

	%this.stop();
	%this.clearAim();
	%this.setMoveX(getRandom(-1, 1));
	%this.isBlind = true;
	if(isObject(%this.getMountedObject(0)))
	{
		%this.getMountedObject(0).canDismount = true;
		%this.getMountedObject(0).isBeingEaten = false;
		%this.getMountedObject(0).dismount();
	}
	%this.target = 0; //Lose sight of target;
	%this.playThreadIfAlive(3, headside);
	%this.schedule(750, playThreadIfAlive, 2, activate2);
	cancel(%this.attackSchedule);
	%this.stopBlindSchedule = %this.schedule(%this.getDatablock().blindRecovery * 1000, titanStopBlind);
}

function AIPlayer::titanStopBlind(%this)
{
	if(!isObject(%this) || %this.getState() $= "Dead")
	{
		return;
	}
	%this.playThreadIfAlive(3, root);
	%this.titanComboSchedule();
}

function AIPlayer::canTitanSee(%this, %obj)
{
	if(!isObject(%this) || %this.getState() $= "Dead")
	{
		return;
	}
	%start = %this.getEyePoint();
	%end = %obj.getHackPosition();

	%direct = vectorSub(%end, %start);
	%delta = vectorDot(%this.getEyeVector(), vectorNormalize(%direct));

	return %delta >= 0 && !containerRayCast(%start, %end, $TypeMasks::FxBrickObjectType);
}

function AIPlayer::playThreadIfAlive(%this, %slot, %thread)
{
	if(!isObject(%this) || %this.getState() $= "Dead")
	{
		return;
	}
	%this.playThread(%slot, %thread);
}

function spawnTitan(%transform)
{
	%miniGame = $DefaultMiniGame;

	if (!isObject(%miniGame))
	{
		%miniGame = findLocalClient().miniGame;

		if (!isObject(%miniGame))
		{
			messageAll('', "ERROR: No mini-game could be found.");
			return 0;
		}
	}

	if (%transform $= "")
	{
		%nameCount = BrickGroup_888888.NTObjectCount["_titanSpawn"];
		// echo(%nameCount);
		%brick = BrickGroup_888888.NTObject["_titanSpawn", getRandom(0, %nameCount - 1)];

		%transform = %brick.getSpawnPoint();
		// %transform = %miniGame.pickSpawnPoint();
		// %transform = vectorAdd("38 103 0.1",
		// 	getRandom(-70, 70) SPC
		// 	getRandom(-70, 70)
		// 	SPC getRandom(50, 80));
	}
 	%rnd = getRandom();
 	if(%rnd > 0.95)
 		%datablock = TitanInsaneArmor;
 	else if(%rnd > 0.8)
 		%datablock = TitanJumperArmor;
  	else if(%rnd > 0.7)
 		%datablock = TitanRunnerArmor;
 	else
 		%datablock = TitanArmor;
	%obj = new AIPlayer()
	{
		datablock = %datablock;
		miniGame = %miniGame;
		isTitan = true;
	};
	if (!isObject(BotGroup)) {
		MissionCleanup.add(new SimGroup(BotGroup));
	}
	BotGroup.add(%obj);
	// MissionCleanup.add(%obj);
	%scale = 2.5 + getRandom() * 2;
	%obj.setTransform(%transform);
	%obj.schedule(100, "updateTitan");

	%obj.setScale(%scale SPC %scale SPC %scale);
	%obj.playThread(3, "headUp");
	%obj.mountImage(TitanFistImage, 0);

	%obj.setNodeColor("ALL", "1 0.878 0.612 1");
	%obj.setArmThread("land");
}

package TitanPackage
{
	function getMiniGameFromObject(%obj)
	{
		if (isObject($DefaultMinigame))
		{
			return $DefaultMinigame;
		}

		return %obj;
	}

	function Armor::damage(%this, %obj, %src, %pos, %damage, %type)
	{
		%obj.lastDamage = $Sim::Time;
		%obj.lastDamageAmt = %damage;
		%obj.lastDamageSource = %src;
		%obj.lastDamageType = %type;
		%obj.lastDamagePos = %pos;
		if(%this.isTitan && %src.getType() & $TypeMasks::PlayerObjectType)
		{
			%obj.target = %src;
		}
		Parent::damage(%this, %obj, %src, %pos, %damage, %type);
	}

	function Armor::onDisabled(%this, %obj, %disabled)
	{
		%name = %obj.getShapeName();
		parent::onDisabled(%this, %obj, %disabled);
		if(!isObject(%obj))
		{
			return;
		}
		if(%this.isTitan)
		{
			if(isObject(%obj.lastDamageSource.client))
			{
				%damage = mClamp(vectorLen(%obj.lastDamageSource.getVelocity()) * 10, 10, 9999);
				announce("\c3" @ %obj.lastDamageSource.client.getPlayerName() SPC "\c0killed a\c3" SPC %this.titanType SPC"titan (" @ %name @ ") \c0with\c4" SPC %damage SPC "damage\c0!");
				%obj.lastDamageSource.client.setScore(%obj.lastDamageSource.client.score + 1);
			}
			if ( isObject($DefaultMinigame) )
				$DefaultMinigame.checkLastManStanding();
		}
	}

	function Armor::onCollision(%this, %obj, %col, %fade, %pos, %norm)
	{
		parent::onCollision(%this, %obj, %col, %fade, %pos, %norm);
		if(!isObject(%obj) || %obj.getState() $= "Dead")
		{
			return;
		}
		if (%obj.isJumpAttacking && %col.getType() & $TypeMasks::PlayerObjectType)
		{
			if(!%obj.isOnGround() && getWord(%col.getHackPosition(), 2) - getWord(%obj.getHackPosition(), 2) > 1)
				%col.damage(%this, %col.getPosition(), 10000, $DamageType::Suicide);
		}
	}
};
if(isPackage(TitanPackage))
	deactivatepackage(TitanPackage);
activatePackage("TitanPackage");
