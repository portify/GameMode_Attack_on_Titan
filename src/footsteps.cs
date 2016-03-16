$FOOTSTEPS_INTERVAL = 150;
$FOOTSTEPS_MIN_LANDING = -1.5;
$FOOTSTEPS_MIN_WALKING = 2.7;
$m = "giant";

function Player::updateFootsteps(%this, %lastVert)
{
	cancel(%this.updateFootsteps);

	if (%this.getState() $= "Dead")
	{
		return;
	}

	%velocity = %this.getVelocity();

	%vert = getWord(%velocity, 2);
	%horiz = vectorLen(setWord(%velocity, 2, 0));

	if (%lastVert < $FOOTSTEPS_MIN_LANDING && %vert >= 0)
	{
		%this.getDataBlock().onLand(%this);
	}

	if (%horiz >= $FOOTSTEPS_MIN_WALKING && !%this.isCrouched() && (!%this.getDataBlock().canJet || !%this.triggerState[4]))
	{
		if (!isEventPending(%this.playFootsteps))
		{
			%this.playFootsteps(1);
		}
	}
	else if (isEventPending(%this.playFootsteps))
	{
		cancel(%this.playFootsteps);
	}

	%this.updateFootsteps = %this.schedule(32, "updateFootsteps", %vert);
}

function Player::playFootsteps(%this, %foot)
{
	cancel(%this.playFootsteps);

	if (%this.getState() $= "Dead")
	{
		return;
	}

	%this.getDataBlock().onFootstep(%this, %foot);
	// 290?
	%this.playFootsteps = %this.schedule(300, "playFootsteps", !%foot);
}

function Player::getFootPosition(%this, %foot)
{
	%base = %this.getPosition();
	%side = vectorCross(%this.getUpVector(), %this.getForwardVector());

	if (!%foot)
	{
		%side = vectorScale(%side, -1);
	}

	//return vectorAdd(%base, vectorScale(%side, 0.3));
	return vectorAdd(%base, vectorScale(%side, 0.4));
}

function Player::getFootObject(%this, %foot)
{
	%pos = %this.getFootPosition(%foot);

	return containerRayCast(
		vectorAdd(%pos, "0 0 0.1"),
		vectorSub(%pos, "0 0 1.1"),
		$TypeMasks::All, %this
	);
}

function Armor::onLand(%this, %obj)
{
	for (%i = 0; %i < 2; %i++)
	{
		%ray = %obj.getFootObject(%i);

		if (!%ray)
		{
			continue;
		}

		%material = $m $= "" ? "concrete" : $m;

		if (%ray.material !$= "")
		{
			%material = %ray.material;
		}

		%sound = nameToID("footstepSound_" @ %material @ getRandom(1, $FS::SoundCount[%material]));

		if (isObject(%sound))
		{
			serverPlay3D(%sound, getWords(%ray, 1, 3));
		}
	}
}

function Armor::onFootstep(%this, %obj, %foot)
{
	%ray = %obj.getFootObject(%foot);

	if (!%ray)
	{
		return;
	}

	%color = -1;

	if (%ray.getType() & $TypeMasks::FxBrickAlwaysObjectType)
	{
		%color = %ray.getColorID();
	}

	//%p = new Projectile()
	//{
	//  datablock = PongProjectile;
	//  initialPosition = %obj.getFootPosition(%foot);
	//  initialVelocity = "0 0 0";
	//};

	//%p.setScale("0.5 0.01 0.5");

	%material = $m $= "" ? "concrete" : $m;

	if (%ray.material !$= "")
	{
		%material = %ray.material;
	}

	%sound = "footstepSound_" @ %material @ getRandom(1, $FS::SoundCount[%material]);
	%sound = nameToID(%sound);

	if (isObject(%sound))
	{
		serverPlay3D(%sound, getWords(%ray, 1, 3));
	}
}

package FootstepsPackage
{
	function Armor::onNewDataBlock(%this, %obj)
	{
		Parent::onNewDataBlock(%this, %obj);
	}

	function Armor::onTrigger(%this, %obj, %slot, %state)
	{
		Parent::onTrigger(%this, %obj, %slot, %state);
		%obj.triggerState[%slot] = %state ? 1 : 0;
	}
};

activatePackage("FootstepsPackage");

function getNumberStart( %str )
{
	%best = -1;

	for ( %i = 0 ; %i < 10 ; %i++ )
	{
		%pos = strPos( %str, %i );

		if ( %pos < 0 )
		{
			continue;
		}

		if ( %best == -1 || %pos < %best )
		{
			%best = %pos;
		}
	}

	return %best;
}

function loadFootstepSounds()
{
	//%pattern = "Add-Ons/Script_Footsteps/sound/*.wav";
	%pattern = $AOT::Folder @ "res/sounds/footsteps/*.wav";
	%list = "generic 0";

	deleteVariables( "$FS::Sound*" );
	$FS::SoundNum = 0;

	for ( %file = findFirstFile( %pattern ) ; %file !$= "" ; %file = findNextFile( %pattern ) )
	{
		%base = fileBase( %file );
		%name = "footstepSound_" @ %base;

		echo(%name);

		if ( !isObject( %name ) )
		{
			datablock audioProfile( genericFootstepSound )
			{
				description = "audioClose3D";
				fileName = %file;
				preload = true;
			};

			if ( !isObject( %obj = nameToID( "genericFootstepSound" ) ) )
			{
				continue;
			}

			%obj.setName( %name );
		}

		if ( ( %pos = getNumberStart( %base ) ) > 0 )
		{
			%pre = getSubStr( %base, 0, %pos );
			%post = getSubStr( %base, %pos, strLen( %base ) );

			if ( $FS::SoundCount[ %pre ] < 1 || !strLen( $FS::SoundCount[ %pre ] ) )
			{
				%list = %list SPC %pre SPC $FS::SoundNum;
			}

			if ( $FS::SoundCount[ %pre ] < %post )
			{
				$FS::SoundCount[ %pre ] = %post;
			}

			$FS::SoundName[ $FS::SoundNum ] = %pre;
			$FS::SoundIndex[ %pre ] = $FS::SoundNum;
			$FS::SoundNum++;
		}
	}

	registerOutputEvent( "fxDTSBrick", "setMaterial", "list" SPC %list );
}

function fxDTSBrick::setMaterial( %this, %idx )
{
	%this.material = $FS::SoundName[ %idx ];
	echo(%idx);
}

loadFootstepSounds();

function getMaterial(%color)
{
	switch (%color)
	{
		case 38: return "grass";
		case 57: return "tile";
		case 22: return "tile";
		case 0 or 33: return "sand";
		case 47 or 48 or 49 or 50: return "wood";
		case 55: return "tile";
	}

	return "concrete";
}