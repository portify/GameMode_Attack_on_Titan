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

	if (getRandom(1))
	{
		%adj[%max++] = "Cute Titan";
		%adj[%max++] = "Boy Titan";
		%adj[%max++] = "Sexy Titan";
		%adj[%max++] = "Hot Titan";
		%adj[%max++] = "Dangerous Titan";
		%adj[%max++] = "High Titan";
		%adj[%max++] = "Big Titan";
		%adj[%max++] = "Small Titan";
		%adj[%max++] = "Deadly Titan";
		%adj[%max++] = "Living Titan";
		%adj[%max++] = "Undead Titan";
		%adj[%max++] = "Abrasive Titan";
		%adj[%max++] = "Boring Titan";
		%adj[%max++] = "Explosive Titan";
		%adj[%max++] = "Existant Titan";
		%adj[%max++] = "Breathing Titan";
		%adj[%max++] = "Breaking Titan";
		%adj[%max++] = "Bad Titan";
		%adj[%max++] = "Good Titan";
		%adj[%max++] = "Legal Titan";
		%adj[%max++] = "Illegal Titan";
		%adj[%max++] = "Safe Titan";
		%adj[%max++] = "Saved Titan";
		%adj[%max++] = "Deleted Titan";
		%adj[%max++] = "Unexistant Titan";
		%adj[%max++] = "Disgusting Titan";
		%adj[%max++] = "Female Titan";
		%adj[%max++] = "Male Titan";
		%adj[%max++] = "Young Titan";
		%adj[%max++] = "Old Titan";
		%adj[%max++] = "Teen Titan";
		%adj[%max++] = "Great Titan";
		%adj[%max++] = "Short Titan";
		%adj[%max++] = "Tall Titan";
		%adj[%max++] = "TorqueScript Titan";
		%adj[%max++] = "Blockhead Titan";
		%adj[%max++] = "Ragtime Titan";
		%adj[%max++] = "Wild West Titan";
		%adj[%max++] = "Anime Titan";
		%adj[%max++] = "Cartoon Titan";
		%adj[%max++] = "Sensei Titan";
		%adj[%max++] = "Yaoi Titan";
		%adj[%max++] = "Shota Titan";
		%adj[%max++] = "Doll Titan";
		%adj[%max++] = "Darling Titan";
		%adj[%max++] = "Lua Titan";
		%adj[%max++] = "Cosplay Titan";
		%adj[%max++] = "Zombie Titan";
		%adj[%max++] = "Google Titan";
		%adj[%max++] = "Sponsored Titan";
		%adj[%max++] = "Demo Titan";
		%adj[%max++] = "Trial Titan";
		%adj[%max++] = "Free Titan";
		%adj[%max++] = "Expensive Titan";
		%adj[%max++] = "Showoff Titan";
		%adj[%max++] = "Inexplicable Titan";
		%adj[%max++] = "Automatic Titan";
		%adj[%max++] = "Robot Titan";
		%adj[%max++] = "Ambiguous Titan";
		%adj[%max++] = "Ambidextrous Titan";
		%adj[%max++] = "Nervous Titan";
		%adj[%max++] = "Cowgirl Titan";
		%adj[%max++] = "Cow Titan";
		%adj[%max++] = "Farm Titan";
		%adj[%max++] = "City Titan";
		%adj[%max++] = "Modern Titan";
		%adj[%max++] = "Idle Titan";
		%adj[%max++] = "Combo Titan";
		%adj[%max++] = "Opportune Titan";
		%adj[%max++] = "Rookie Titan";
		%adj[%max++] = "Sick Titan";
		%adj[%max++] = "Mad Titan";
		%adj[%max++] = "Shady Titan";
		%adj[%max++] = "Hard Titan";
		%adj[%max++] = "Gentle Titan";
		%adj[%max++] = "Bareback Titan";
		%adj[%max++] = "Boyfriend Titan";
		%adj[%max++] = "Immature Titan";
		%adj[%max++] = "Abusive Titan";
		%adj[%max++] = "Cheating Titan";
		%adj[%max++] = "Invalid Titan";
		%adj[%max++] = "Wrong Titan";
		%adj[%max++] = "The Titan";
		%adj[%max++] = "Team Titan";
		%adj[%max++] = "Bottom Titan";
		%adj[%max++] = "Top Titan";
		%adj[%max++] = "Deviant Titan";
		%adj[%max++] = "Dominant Titan";
		%adj[%max++] = "Submissive Titan";
		%adj[%max++] = "Switch Titan";
		%adj[%max++] = "Inappropriate Titan";
		%adj[%max++] = "Swearing Titan";
		%adj[%max++] = "Unfitting Titan";
		%adj[%max++] = "Flat Titan";
		%adj[%max++] = "Trap Titan";
		%adj[%max++] = "Adventurous Titan";
		%adj[%max++] = "Pirate Titan";
		%adj[%max++] = "Roleplay Titan";
		%adj[%max++] = "Shy Titan";
		%adj[%max++] = "Adorable Titan";
		%adj[%max++] = "Attractive Titan";
		%adj[%max++] = "Sweet Titan";
		%adj[%max++] = "Charming Titan";
		%adj[%max++] = "Boyish Titan";
		%adj[%max++] = "Lewd Titan";
		%adj[%max++] = "Playful Titan";
		%adj[%max++] = "Fun Titan";
		%adj[%max++] = "Gifted Titan";
		%adj[%max++] = "Smart Titan";
		%adj[%max++] = "Generous Titan";
		%adj[%max++] = "Patient Titan";
		%adj[%max++] = "Camping Titan";
		%adj[%max++] = "Dangerous Titan";
		%adj[%max++] = "Irrelevant Titan";
		%adj[%max++] = "Pointless Titan";
		%adj[%max++] = "Unnecessary Titan";
		%adj[%max++] = "Touching Titan";
		%adj[%max++] = "Grabbing Titan";
		%adj[%max++] = "Feeling Titan";
		%adj[%max++] = "Lonely Titan";
		%adj[%max++] = "Blushing Titan";
		%adj[%max++] = "Yandere Titan";
		%adj[%max++] = "Immigrant Titan";
		%adj[%max++] = "Open Source Titan";
		%adj[%max++] = "Documented Titan";
		%adj[%max++] = "Communist Titan";
		%adj[%max++] = "Democrat Titan";
		%adj[%max++] = "Republican Titan";
		%adj[%max++] = "Obnoxious Titan";
		%adj[%max++] = "Political Titan";
		%adj[%max++] = "Presidential Titan";
		%adj[%max++] = "Open Minded Titan";
		%adj[%max++] = "Mindful Titan";
		%adj[%max++] = "Educated Titan";
		%adj[%max++] = "Daddy Titan";
		%adj[%max++] = "Mommy Titan";
		%adj[%max++] = "Creepy Titan";
		%adj[%max++] = "Manga Titan";
		%adj[%max++] = "Jam Titan";
		%adj[%max++] = "JavaScript Titan";
		%adj[%max++] = "Notepad Titan";
		%adj[%max++] = "Atomic Titan";
		%adj[%max++] = "Sublime Titan";
		%adj[%max++] = "Ultimate Titan";
		%adj[%max++] = "Culture Titan";
		%adj[%max++] = "Anonymous Titan";
		%adj[%max++] = "Tan Titan";
		%adj[%max++] = "Kinky Titan";
		%adj[%max++] = "Flirting Titan";
		%adj[%max++] = "Interested Titan";
		%adj[%max++] = "Final Titan";
		%adj[%max++] = "The Final Titan";
		%adj[%max++] = "Other Titan";
		%adj[%max++] = "Pet Titan";
		%adj[%max++] = "Obedient Titan";
		%adj[%max++] = "Remove Titan";
		%adj[%max++] = "Bacon Titan";
		%adj[%max++] = "Kebab Titan";
		%adj[%max++] = "Bacon Titan";
		%adj[%max++] = "Nude Titan";
		%adj[%max++] = "Nudist Titan";
		%adj[%max++] = "Naked Titan";
		%adj[%max++] = "Topless Titan";
		%adj[%max++] = "Bottomless Titan";
		%adj[%max++] = "Hip Titan";
		%adj[%max++] = "Windows Titan";
		%adj[%max++] = "Microsoft Titan";
		%adj[%max++] = "McTitan";
		%adj[%max++] = "Apple Titan";
		%adj[%max++] = "Instinctual Titan";
		%adj[%max++] = "GNU Titan";
		%adj[%max++] = "Brand Titan";
		%adj[%max++] = "Competitive Titan";
		%adj[%max++] = "Unique Titan";
		%adj[%max++] = "Gambling Titan";
		%adj[%max++] = "Git Titan";
		%adj[%max++] = "Versioned Titan";
		%adj[%max++] = "Master Titan";
		%adj[%max++] = "Slave Titan";
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

	return %adj[getRandom(%max)];
}
