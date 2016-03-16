function vectorToYawAngle(%vector)
{
	return mATan(getWord(%vector, 1), getWord(%vector, 0));
}

function angleToCompass(%angle)
{
    %val = mFloor((mRadToDeg(%angle) / 22.5) + 0.5);
	%arr0  = "N";
	%arr1  = "NNE";
	%arr2  = "NE";
	%arr3  = "ENE";
	%arr4  = "E";
	%arr5  = "ESE";
	%arr6  = "SE";
	%arr7  = "SSE";
	%arr8  = "S";
	%arr9  = "SSW";
	%arr10 = "SW";
	%arr11 = "WSW";
	%arr12 = "W";
	%arr13 = "WNW";
	%arr14 = "NW";
	%arr15 = "NNW";
	return %arr[%val % 16];
}

function Player::TDMGDisplayLoop(%this)
{
	cancel(%this.TDMGDisplayLoop);

	if (!isObject(%this.client) || %this.getState() $= "Dead")
		return;

	%client = %this.client;

	if (!%this.grappleRopePos)
	{
		%end = vectorAdd(vectorScale(%this.getEyeVector(), 95), %this.getEyePoint());
		%ray = ContainerRayCast(%this.getEyePoint(), %end,
			$TypeMasks::StaticObjectType |
			$TypeMasks::PlayerObjectType |
			$TypeMasks::fxBrickObjectType,
			%this
		);
		if(isObject(getWord(%ray, 0)))
			%dist = mFloor(vectorDist(%this.getEyePoint(), getWords(%ray, 1, 3)));
		else
			%dist = "???";
		%color = (isObject(getWord(%ray, 0)) ? "<color:DDDDDD>" : "<color:AA0000>");
	}
	else
	{
		%dist = mFloor(vectorDist(%this.getHackPosition(), %this.grappleRopePos));
		%color = "<color:77FF77>";
	}
	%pref = "<br><font:Arial:42>" @ %color;
	%color = %this.TDMGBlades > 10 ? "<color:DDDDDD>" : "<color:AA0000>";
	%blades = "<color:DDDDDD><just:left>Blades:" @ %color SPC mCeil(%this.TDMGBlades) @ "%";
	%spares = "<br><color:DDDDDD><just:left>(" @ %this.TDMGSpareBlades * 2 SPC "spares)";
	%color = %this.TDMGGas > 5000 * 0.1 ? "<color:DDDDDD>" : "<color:AA0000>";
	%gas = "<color:DDDDDD><just:right>Gas:" @ %color SPC mCeil(%this.TDMGGas) @ " ";
	%speed = "<color:DDDDDD><just:right>Speed: " @ mFloor(vectorLen(%this.getVelocity()) * 10) @ " ";
	%speed = %speed @ angleToCompass(vectorToYawAngle(%this.getForwardVector())) @ " ";
	%text = %pref @ %dist @ "<br><br><br><font:Times:20>" @ %blades @ %gas @ %spares @ %speed;
	%this.client.centerPrint(%text, 1);
	%this.TDMGDisplayLoop = %this.schedule(16, "TDMGDisplayLoop");
}
