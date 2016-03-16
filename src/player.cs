function PlayerReconArmor::onNewDataBlock(%this, %obj)
{
	Parent::onNewDataBlock(%this, %obj);

	if (!%obj.TDMGInit)
	{
		%obj.TDMGInit = true;
		%obj.TDMGBlades = 100;
		%obj.TDMGSpareBlades = 4;
		%obj.TDMGGas = 3000;
		%obj.mountImage(TDMGModelImage,1);

		if (isObject(%obj.client))
			%obj.TDMGDisplayLoop();
	}
}

package AoTPlayerPackage
{
	//Blood things
	function Armor::damage(%this, %obj, %src, %pos, %damage, %type)
	{
		// if (getSimTime() - %obj.spawnTime < $Game::PlayerInvulnerabilityTime)
		// {
		// 	return Parent::damage(%this, %obj, %src, %pos, %damage, %type);
		// }
		// if (%pos $= "")
		// {
		// 	%pos = %obj.getHackPosition();
		// }
		// // createBloodExplosion(%pos, %obj.getVelocity(), %obj.getScale());
		// if (%obj.getDamageLevel() + %damage > %this.maxDamage)
		// {
		// 	%fatal = true;
		// }

		Parent::damage(%this, %obj, %src, %pos, %damage, %type);
	}

	function Armor::onDisabled(%this, %obj, %disabled)
	{
		// %obj.doSplatterBlood();
		Parent::onDisabled(%this, %obj, %disabled);
	}
};

activatePackage(AoTPlayerPackage);
