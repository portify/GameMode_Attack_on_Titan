function MiniGameSO::endAoT(%this, %winner)
{
	if (isEventPending(%this.resetSchedule))
		return;

	if ($AoT::Survival)
		%text = "humans survived for \c3" @ %this.wave @ " wave(s)\c5";
	else
		%text = %winner $= "titans" ? "titans win" : "humans win";
	
	%this.messageAll('', "\c5The " @ %text @ "! A new game will begin in 10 sceonds.");

	%this.scheduleReset(10000);
}

function MiniGameSO::startWave(%this, %wave)
{
	if ($AoT::Survival::Respawn)
	{
		for (%i = 0; %i < %this.numMembers; %i++)
		{
			%member = %this.member[%i];

			if (!isObject(%member.player))
				%member.instantRespawn();
		}
	}

	%titanCount = getMin(mCeil(5 * (%wave / 2)), 30);
	%this.wave = %wave;
	%this.messageAll('', "\c5Starting \c4Wave " @ %wave @ " \c5- " @ %titanCount @ " titans to defeat!");

	for (%i = 0; %i < %titanCount; %i++)
		spawnTitan();
}

package AoTGamePackage
{
	// Reset when the first player joins.
	function MiniGameSO::addMember(%this, %member)
	{
		%empty = %this.numMembers == 0;
		Parent::addMember(%this, %member);

		if (%this.owner == 0 && %empty)
			%this.reset(0);
	}

	// Clean up and start new round on reset.
	function MiniGameSO::reset(%this, %client)
	{
		Parent::reset(%this, %client);

		if (%this.owner != 0 || %this.numMembers == 0)
			return;

		if (%this.lastResetTime != getSimTime())
			return;

		clearDecals();

		if (isObject(BotGroup))
			BotGroup.deleteAll();

		%this.startWave(1);
	}

	function MiniGameSO::checkLastManStanding(%this)
	{
		if (%this != $DefaultMiniGame)
			return Parent::checkLastManStanding(%this);

		if (%this.numMembers < 1 || isEventPending(%this.scheduleReset))
			return 0;

		%numHuman = 0;
		%numTitan = 0;

		for (%i = 0; %i < %this.numMembers; %i++)
		{
			%member = %this.member[%i];

			if (isObject(%member.player))
				%numHuman++;
		}

		for (%i = 0; %i < BotGroup.getCount(); %i++)
		{
			if (BotGroup.getObject(%i).getState() !$= "Dead")
				%numTitan++;
		}

		if (%numHuman == 0)
		{
			%this.endAoT("titans");
			return 0;
		}

		if (%numTitan == 0)
		{
			if ($AoT::Survival)
				%this.startWave(%this.wave + 1);
			else
				%this.endAoT("humans");

			return 0;
		}

		return 0;
	}

	function MinigameCanDamage(%a, %b)
	{
		return %b.isTitan;
	}
};

activatePackage("AoTGamePackage");
