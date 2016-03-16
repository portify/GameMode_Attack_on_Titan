function Player::playThreadIfAlive(%this, %slot, %thread)
{
	if(!isObject(%this) || %this.getState() $= "Dead")
	{
		return;
	}
	%this.playThread(%slot, %thread);
}

function Player::TitanEyeStab(%this)
{
	if ( %this.isEating )
		return;

	%this.isBlind = true;
	if(isObject(%this.getMountedObject(0)))
	{
		%this.getMountedObject(0).canDismount = true;
		%this.getMountedObject(0).isBeingEaten = false;
		%this.getMountedObject(0).dismount();
	}
	if(isObject(%this.client.camera))
	{
		%this.client.camera.setOrbitMode(%this, %pos, 0.5, 8, 8, 1);
		%this.client.camera.mode = "Orbit";
		%this.client.camera.setTransform(%this.getEyeTransform());
		%this.setControlObject(%this.client.camera);
	}

	%this.playThreadIfAlive(3, headside);
	%this.schedule(750, playThreadIfAlive, 2, activate2);
	cancel(%this.attackSchedule);
	%this.stopBlindSchedule = %this.schedule((((%timeout = %this.getDatablock().blindRecovery) > 0) ? %timeout : 2) * 1000, titanStopBlind);
}

function Player::titanStopBlind(%this)
{
	if(!isObject(%this) || %this.getState() $= "Dead")
	{
		return;
	}
	%this.playThreadIfAlive(3, root);
	%this.setControlObject(%this);
}

package PlayerTitanPackage
{
	function Armor::damage(%this, %obj, %src, %pos, %damage, %type)
	{
		if(!%this.isTitan)
		{
			return Parent::damage(%this, %obj, %src, %pos, %damage, %type);
		}

		if (%pos $= "")
		{
			%pos = %obj.getHackPosition();
		}
		if(%obj.isCrouched()) //Placeholder. Later on I'll make it so crouched players have accurate weak spot.
		{
			Parent::damage(%this, %obj, %src, %pos, 10000, %type);
			serverPlay3D(TDMGSwordCutSound, %pos);
			return;
		}
		%scale = getWord(%obj.getScale(), 2);
		%boxcenter = getword(%obj.getWorldBoxCenter(), 2);
		%nape = %boxcenter - (4 * %scale);
		%head = %boxcenter - (3.8 * %scale);
		// %eyes = %boxcenter - (3.65 * %scale);
		%forehead = %boxcenter - (3.1 * %scale);
		// %dot = vectorDot(%src.getForwardVector(), vectorSub(%obj.getHackPosition(), %pos));
		%direct = vectorSub(%pos, %obj.getHackPosition());
		%delta = vectorDot(%obj.getForwardVector(), vectorNormalize(%direct));
		if(getword(%pos, 2) > %head)
		{
			if(%delta <= -0.6 && getWord(%pos, 2) > %nape && getword(%pos, 2) < %forehead)
			{
				Parent::damage(%this, %obj, %src, %pos, 10000, %type);
				serverPlay3D(TDMGSwordCutSound, %pos);
				return;
			}
			else if(getword(%pos, 2) > %head && getword(%pos, 2) < %forehead && %delta >= 0.6 && !%obj.isBlind)
			{
				serverPlay3D(TDMGSwordCutSound, %pos);
				// %obj.setDamageFlash(1);
				// %obj.setWhiteOut(1);
				%obj.playThread(1, "root");
				%obj.playThread(2, activate2);
				%obj.TitanEyeStab();
			}
		}
	}

	function Armor::onNewDataBlock(%this, %obj)
	{
		Parent::onNewDataBlock(%this, %obj);
		if (!%this.isTitan) return;
		if (!isEventPending(%obj.updateFootsteps)) {
			%obj.updateFootsteps = %obj.schedule(0, "updateFootsteps");
		}

		%obj.clearTools();

		%obj.setShapeName(getTitanName(%this.titanType), "8564862");
		%obj.setShapeNameDistance(30);

		%obj.playThread(3, "headUp");
		%obj.mountImage(TitanFistImage, 0);

		%obj.setNodeColor("ALL", "1 0.878 0.612 1");
		%obj.setArmThread("land");

		%nameCount = BrickGroup_888888.NTObjectCount["_titanSpawn"];
		// echo(%nameCount);
		%brick = BrickGroup_888888.NTObject["_titanSpawn", getRandom(0, %nameCount - 1)];

		%transform = %brick.getSpawnPoint();

		%obj.setTransform(%transform);
	}
};

activatePackage(PlayerTitanPackage);

function getTitanName(%type)
{
	// return;
	%max = -1;

	if (%type $= "normal")
	{
		%adj[%max++] = "Vegetable Titan";
		%adj[%max++] = "Normal Titan";
		%adj[%max++] = "Walking Titan";
		%adj[%max++] = "A Spot of Fun";
		%adj[%max++] = "Walker, Texas Ranger";
		%adj[%max++] = "Slow Titan";
		%adj[%max++] = "Usual Titan";
		%adj[%max++] = "Silly Titan";
		%adj[%max++] = "Kill Me";
		%adj[%max++] = "Casual Titan";
	}
	else if (%type $= "runner")
	{
		%adj[%max++] = "Fast Titan";
		%adj[%max++] = "Spooky Titan";
		%adj[%max++] = "Peculiar Titan";
		%adj[%max++] = "Dodge Me";
		%adj[%max++] = "Running Titan";
		%adj[%max++] = "Legs for Days";
		%adj[%max++] = "Quake-Like Titan";
		%adj[%max++] = "Determined Titan";
	}
	else if (%type $= "jumper")
	{
		%adj[%max++] = "Flying Fuckazoid";
		%adj[%max++] = "Aerial Titan";
		%adj[%max++] = "Wall-Climbing Titan";
		%adj[%max++] = "GOOD AFTERNOON";
		%adj[%max++] = "Jumping Titan";
		%adj[%max++] = "Mario Titan";
		%adj[%max++] = "You Can't Hide";
		%adj[%max++] = "Leaping Titan";
		%adj[%max++] = "Literally Satan";
	}
	else if (%type $= "insane")
	{
		%adj[%max++] = "Abnormal Titan";
		%adj[%max++] = "Usain Bolt";
		%adj[%max++] = "Sonic the Titan";
		%adj[%max++] = "Hacker Titan";
		%adj[%max++] = "Insane Titan";
		%adj[%max++] = "Blind Me";
		%adj[%max++] = "Spin Master Titan";
		%adj[%max++] = "Difficult Titan";
		%adj[%max++] = "Catch-22 Titan";
	}

	%adj[%max++] = "Titan";
	%adj[%max++] = "Earth";
	%adj[%max++] = "The Last Stand";
	%adj[%max++] = "Killua";
	%adj[%max++] = "Teen Titans";
	%adj[%max++] = "Titanic";
	%adj[%max++] = "Titanic";
	%adj[%max++] = "Titanic";
	%adj[%max++] = "Titanic";
	%adj[%max++] = "Jump Over Me";
	%adj[%max++] = "Swing Around Me";
	%adj[%max++] = "I Am Groot";
	%adj[%max++] = "Building Crusher";
	%adj[%max++] = "Tower Smasher";
	%adj[%max++] = "Ceiling Defiler";
	%adj[%max++] = "Breakfast";

	return %adj[getRandom(%max)];
}
