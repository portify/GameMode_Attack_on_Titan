function Player::fireFlare(%obj, %color)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

	%rnd = -1 + getRandom() * 2 SPC -1 + getRandom() * 2;
	%p = new Projectile()
	{
		dataBlock = %color @ FlareProjectile;
		scale = "1 1 1";//%obj.getScale();

		initialPosition = %obj.getMuzzlePoint(0);
		initialVelocity = %rnd SPC 30;

		client = %obj.client;
		sourceObject = %obj;
	};
	MissionCleanup.add(%p);

	switch$(%color)
	{
		case "red": %c = "\c0";
		case "green": %c = "\c2";
		case "black": %c = "\c7";
	}
	announce(strUpr(%c @ %color SPC "flare was fired!"));
}