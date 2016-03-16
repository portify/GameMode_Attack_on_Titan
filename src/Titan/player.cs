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

		%obj.setShapeName(getTitanAdj() SPC "Titan", "8564862");
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

function getTitanAdj()
{
	// return;
	%max = -1;

	%adj[%max++] = "Cute";
	%adj[%max++] = "Boy";
	%adj[%max++] = "Sexy";
	%adj[%max++] = "Hot";
	%adj[%max++] = "Dangerous";
	%adj[%max++] = "High";
	%adj[%max++] = "Big";
	%adj[%max++] = "Small";
	%adj[%max++] = "Deadly";
	%adj[%max++] = "Living";
	%adj[%max++] = "Undead";
	%adj[%max++] = "Abrasive";
	%adj[%max++] = "Boring";
	%adj[%max++] = "Explosive";
	%adj[%max++] = "Existant";
	%adj[%max++] = "Breathing";
	%adj[%max++] = "Breaking";
	%adj[%max++] = "Bad";
	%adj[%max++] = "Good";
	%adj[%max++] = "Legal";
	%adj[%max++] = "Illegal";
	%adj[%max++] = "Safe";
	%adj[%max++] = "Saved";
	%adj[%max++] = "Deleted";
	%adj[%max++] = "Unexistant";
	%adj[%max++] = "Disgusting";
	%adj[%max++] = "Female";
	%adj[%max++] = "Male";
	%adj[%max++] = "Young";
	%adj[%max++] = "Old";
	%adj[%max++] = "Teen";
	%adj[%max++] = "Great";
	%adj[%max++] = "Short";
	%adj[%max++] = "Tall";
	%adj[%max++] = "TorqueScript";
	%adj[%max++] = "Blockhead";
	%adj[%max++] = "Ragtime";
	%adj[%max++] = "Wild West";
	%adj[%max++] = "Anime";
	%adj[%max++] = "Cartoon";
	%adj[%max++] = "Sensei";
	%adj[%max++] = "Yaoi";
	%adj[%max++] = "Shota";
	%adj[%max++] = "Doll";
	%adj[%max++] = "Darling";
	%adj[%max++] = "Lua";
	%adj[%max++] = "Cosplay";
	%adj[%max++] = "Zombie";
	%adj[%max++] = "Google";
	%adj[%max++] = "Sponsored";
	%adj[%max++] = "Demo";
	%adj[%max++] = "Trial";
	%adj[%max++] = "Free";
	%adj[%max++] = "Expensive";
	%adj[%max++] = "Showoff";
	%adj[%max++] = "Inexplicable";
	%adj[%max++] = "Automatic";
	%adj[%max++] = "Robot";
	%adj[%max++] = "Ambiguous";
	%adj[%max++] = "Ambidextrous";
	%adj[%max++] = "Nervous";
	%adj[%max++] = "Cowgirl";
	%adj[%max++] = "Cow";
	%adj[%max++] = "Farm";
	%adj[%max++] = "City";
	%adj[%max++] = "Modern";
	%adj[%max++] = "Idle";
	%adj[%max++] = "Combo";
	%adj[%max++] = "Opportune";
	%adj[%max++] = "Rookie";
	%adj[%max++] = "Sick";
	%adj[%max++] = "Mad";
	%adj[%max++] = "Shady";
	%adj[%max++] = "Hard";
	%adj[%max++] = "Gentle";
	%adj[%max++] = "Bareback";
	%adj[%max++] = "Boyfriend";
	%adj[%max++] = "Immature";
	%adj[%max++] = "Abusive";
	%adj[%max++] = "Cheating";
	%adj[%max++] = "Invalid";
	%adj[%max++] = "Wrong";
	%adj[%max++] = "The";
	%adj[%max++] = "Team";
	%adj[%max++] = "Bottom";
	%adj[%max++] = "Top";
	%adj[%max++] = "Deviant";
	%adj[%max++] = "Dominant";
	%adj[%max++] = "Submissive";
	%adj[%max++] = "Switch";
	%adj[%max++] = "Inappropriate";
	%adj[%max++] = "Swearing";
	%adj[%max++] = "Unfitting";
	%adj[%max++] = "Flat";
	%adj[%max++] = "Trap";
	%adj[%max++] = "Adventurous";
	%adj[%max++] = "Pirate";
	%adj[%max++] = "Roleplay";
	%adj[%max++] = "Shy";
	%adj[%max++] = "Adorable";
	%adj[%max++] = "Attractive";
	%adj[%max++] = "Sweet";
	%adj[%max++] = "Charming";
	%adj[%max++] = "Boyish";
	%adj[%max++] = "Lewd";
	%adj[%max++] = "Playful";
	%adj[%max++] = "Fun";
	%adj[%max++] = "Gifted";
	%adj[%max++] = "Smart";
	%adj[%max++] = "Generous";
	%adj[%max++] = "Patient";
	%adj[%max++] = "Camping";
	%adj[%max++] = "Dangerous";
	%adj[%max++] = "Irrelevant";
	%adj[%max++] = "Pointless";
	%adj[%max++] = "Unnecessary";
	%adj[%max++] = "Touching";
	%adj[%max++] = "Grabbing";
	%adj[%max++] = "Feeling";
	%adj[%max++] = "Lonely";
	%adj[%max++] = "Blushing";
	%adj[%max++] = "Yandere";
	%adj[%max++] = "Immigrant";
	%adj[%max++] = "Open Source";
	%adj[%max++] = "Documented";
	%adj[%max++] = "Communist";
	%adj[%max++] = "Democrat";
	%adj[%max++] = "Republican";
	%adj[%max++] = "Obnoxious";
	%adj[%max++] = "Political";
	%adj[%max++] = "Presidential";
	%adj[%max++] = "Open Minded";
	%adj[%max++] = "Mindful";
	%adj[%max++] = "Educated";
	%adj[%max++] = "Daddy";
	%adj[%max++] = "Mommy";
	%adj[%max++] = "Creepy";
	%adj[%max++] = "Manga";
	%adj[%max++] = "Jam";
	%adj[%max++] = "JavaScript";
	%adj[%max++] = "Notepad";
	%adj[%max++] = "Atomic";
	%adj[%max++] = "Sublime";
	%adj[%max++] = "Ultimate";
	%adj[%max++] = "Culture";
	%adj[%max++] = "Anonymous";
	%adj[%max++] = "Tan";
	%adj[%max++] = "Kinky";
	%adj[%max++] = "Flirting";
	%adj[%max++] = "Interested";
	%adj[%max++] = "Final";
	%adj[%max++] = "The Final";
	%adj[%max++] = "Other";
	%adj[%max++] = "Pet";
	%adj[%max++] = "Obedient";
	%adj[%max++] = "Remove";
	%adj[%max++] = "Bacon";
	%adj[%max++] = "Kebab";
	%adj[%max++] = "Bacon";
	%adj[%max++] = "Nude";
	%adj[%max++] = "Nudist";
	%adj[%max++] = "Naked";
	%adj[%max++] = "Topless";
	%adj[%max++] = "Bottomless";
	%adj[%max++] = "Hip";
	%adj[%max++] = "Windows";
	%adj[%max++] = "Microsoft";
	%adj[%max++] = "Mc";
	%adj[%max++] = "Apple";
	%adj[%max++] = "Instinctual";
	%adj[%max++] = "GNU";
	%adj[%max++] = "Brand";
	%adj[%max++] = "Competitive";
	%adj[%max++] = "Unique";
	%adj[%max++] = "Gambling";
	%adj[%max++] = "Git";
	%adj[%max++] = "Versioned";
	%adj[%max++] = "Master";
	%adj[%max++] = "Slave";

	return %adj[getRandom(%max)];
}
