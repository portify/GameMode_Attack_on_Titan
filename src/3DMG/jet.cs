function Player::tdmgJetLoop(%this)
{
	cancel(%this.tdmgJetLoop);
	if (!isObject(%this) || %this.getState() $= "Dead" || !%this.slot[2] || %this.TDMGGas <= 0)
	{
		%this.unMountImage(3);
		if(!isObject(%this.oldDatablock)) %this.oldDatablock = playerNoJet;
		%this.setDatablock(%this.oldDatablock);
		%this.stopAudio(2);
		%this.SteamSound = false;
		return;
	}
	if ($Sim::Time - %this.lastJump <= 0.25 || %this.isOnGround())
	{
		%this.unMountImage(3);
		%this.stopAudio(2);
		%this.SteamSound = false;
		%this.tdmgJetLoop = %this.schedule(32, tdmgJetLoop);
		return;
	}

	if (%this.getDatablock() != PlayerReconJetArmor.getId() && !%this.isOnGround())
	{
		%this.oldDatablock = %this.getDatablock();
		%this.setDatablock(PlayerReconJetArmor);
	}

	if(%this.getMountedImage(3) != tdmgJetEffectImage.getId())
	{
		%this.mountImage(tdmgJetEffectImage, 3);
	}

	if(!%this.SteamSound)
	{
		%this.playAudio(2, TDMGSteamSound);
		%this.SteamSound = true;
	}
	%vec = vectorScale(%this.getForwardVector(), 0.85);
	// %vec = vectorAdd(%vec, "0 0" SPC getWord(%this.getEyeVector(), 2) * 0.5);
	%this.setVelocity(vectorAdd(%this.getVelocity(), vectorAdd(%vec, "0 0 0.1")));
	%this.TDMGGas = %this.TDMGGas - 1.25;
	%this.tdmgJetLoop = %this.schedule(32, tdmgJetLoop);
}

function pushPlayerToObj(%player, %cpos)
{
	if (!isObject(%player)) return; //make sure these are still objects, get rid of checking if the collision is an object
	%ppos = getWords(%player.getTransform(), 0, 2);
	%vec = VectorSub(%cpos, %ppos); //This automatically subtracts the positions
	%len = VectorLen(%vec); //find the distance between the two coordinates
	// if (%len < 5)
	// {
	// 	return;
	// } //we don't check the same position because they won't be the same, just close to each other
	%vec = VectorNormalize(%vec); //makes it small
	if(%len < 15)
	{
		%vec = VectorScale(%vec, 30);
	}
	else
	{
		%vec = VectorScale(%vec, 40);
	}
	%player.setVelocity(%vec); //and we might wanna make this keep on going so...
	// cancel(%player.pptoTick); //this cancels schedules from happening, this will prevent more than one happening at a time
	// %player.pptoTick = schedule(100, 0, "pushPlayerToObj", %player, %cpos); //make the schedule send the collision position now//assign this to .ppTo
}