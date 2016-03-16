function TDMGProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
	parent::onCollision(%this, %obj, %col, %fade, %pos, %normal);

	if(!isObject(%sourcePlayer = %obj.client.player))
	{
		return;
	}

	cancel(%sourcePlayer.RopeToProjectileLoop);
	%sourcePlayer.ropeLength = vectorLen(vectorSub(%sourcePlayer.getPosition(), %pos));
	%sourcePlayer.GrappleRopeTarget++;
	%sourcePlayer.GrappleRopeCol = %col;
	%sourcePlayer.GrappleRopePos = %pos;

	if(%col.getType() & $TypeMasks::PlayerObjectType)
	{
		%sourcePlayer.GrappleRopePlayerPos = "Test";
		%sourcePlayer.grappleColPos = %col.getHackPosition();
	}

	%sourcePlayer.grappleLoop();
}

function vectorRotate(%vector, %origin, %angle) {
	%s = mSin(%angle);
	%c = mCos(%angle);

	%px = getWord(%vector, 0);
	%py = getWord(%vector, 1);

	%ox = getWord(%origin,0);
	%oy = getWord(%origin,1);

	%nx = %c * (%px - %ox) - %s * (%py - %oy) + %ox;
	%ny = %s * (%px - %ox) + %c * (%py - %oy) + %oy;

	return %nx SPC %ny SPC getWord(%vector,2);
}

function Player::updateGrapplePosition(%this) {
	%col = %this.GrappleRopeCol;
	if(!isObject(%col) || %col.getState() $= "Dead" || !(%col.getType() & $TypeMasks::PlayerObjectType))
		return;

	%hookPos = %this.grappleRopePos;

	%objLastPos = %col.lastPos;
	if(%objLastPos $= "") {
		%col.lastPos = %col.getHackPosition();
		%this.grappleRopePos = %hookPos;
		return 1;
	}
	else {
		%currPos = %col.getHackPosition();
		%add = vectorSub(%objLastPos, %currPos);
		%pos = vectorSub(%hookPos, %add);
		%col.lastPos = %currPos;

		%curAim = %col.getForwardVector();
		%curX = getWord(%curAim,0);
		%curY = getWord(%curAim,1);
		%curZ = getWord(%curAim,2);
		%theta = mRadToDeg(mATan(%curY,%curX));

		%angle = %theta;

		%lastAngle = %col.lastAngle;

		if(%lastAngle !$= "") {
			%col.lastAngle = %angle;

			if(%lastAngle < 0 && %angle > 0) {
				if(%lastAngle < -170 && %angle > 170) {
					%diff = -%lastangle - %angle;
				}
			}

			%diff = %lastangle - %angle;

			%diff = -%diff;

			%pos = vectorRotate(%pos, %col.getHackPosition(), mDegToRad(%diff));

			%this.grappleRopePos = %pos;
		}
		else {
			%col.lastAngle = %angle;
		}
	}
}

function GrappleRope(%obj, %targetObj, %target, %pos)
{
	if(%obj.GrappleRopeTarget != %target)
	{
		if(isObject(%obj.grappleRope)) %obj.grappleRope.delete();
		%obj.grappleRopePos = "";
		return;
	}
	if(!isObject(%obj))
	{
		if(isObject(%obj.grappleRope)) %obj.grappleRope.delete();
		%obj.grappleRopePos = "";
		return;
	}

	%objpos = %obj.getHackPosition();
	%vel = %obj.getVelocity();

	%obj_next_tick = %obj.getPosition();
	%obj_distance_next_tick = vectorDist(%obj_next_tick, %pos);

	if(vectorDist(vectorAdd(%obj_next_tick, %obj.getVelocity()), %pos) > %obj.ropeLength) {
		%vel = vectorNormalize(vectorSub(%obj.getPosition(), %pos));
		%vel = vectorScale(%vel, (%obj.ropeLength - vectorDist(%obj.getPosition(), %pos)));
		%obj.addVelocity(%vel);
	}

	if(%obj_distance_next_tick < %obj.ropeLength)
		%obj.ropeLength = %obj_distance_next_tick;

	if(getWord(%obj.getVelocity(), 2) == 0) //To prevent too much friction
	{
		%obj.addVelocity("0 0 2");
	}

	if(%obj_distance_next_tick > 4) {
		%obj.addVelocity(vectorScale(vectorNormalize(vectorSub(%obj.grappleRopePos, %objpos)), 1.5));
	}

	//effects
	%a = %obj.getHackPosition();
	%b = %pos;

	if(!isObject(%obj.grappleRope))
	{
		%obj.grappleRope = createRope(%a, %b);
	}
	else
	{
		%vec1 = vectorNormalize(vectorSub(%a, %b));
		%vec2 = vectorNormalize(vectorSub(%b, %a));

		%xyz = vectorNormalize(vectorCross("1 0 0", %vec1));
		%pow = mACos(vectorDot("1 0 0", %vec1)) * -1;
		%obj.grappleRope.setScale(vectorDist(%a, %b) SPC 0.1 SPC 0.1);

		%position = vectorScale(vectorAdd(%a, %b), 0.5);
		%rotation = %xyz SPC %pow;

		%obj.grappleRope.setTransform(%position SPC %rotation);
		%obj.grappleRope.a = %a;
		%obj.grappleRope.b = %b;
	}
}

function Player::grappleLoop(%obj)
{
	cancel(%obj.grappleLoop);
	if(isObject(%obj.grappleRopeProjectile))
	{
		%obj.grappleRopeProjectile.delete();
	}

	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		if(isObject(%obj.grappleRope))
			%obj.grappleRope.delete();

		%obj.grappleRopePos = "";
		return;
	}

	if(%obj.TDMGGas <= 0)
	{
		if(isObject(%obj.grappleRope))
			%obj.grappleRope.delete();

		%obj.grappleRopePos = "";
		return;
	}

	if(%obj.GrappleRopePos $= "" || !%obj.slot[4])
	{
		if(isObject(%obj.grappleRope))
			%obj.grappleRope.delete();

		%obj.grappleRopePos = "";
		return;
	}

	%obj.isHoldGrap = 1;

	if(%obj.GrappleRopePlayerPos $= "Test")
	{
		if(!%obj.updateGrapplePosition())
		{
			%obj.GrappleRopeTarget++;
			%obj.GrappleRopePos = "";
			%obj.GrappleRopeCol = "";
			%obj.GrappleRopePlayerPos = "";
			%obj.isHoldGrap = 0;
			if(isObject(%obj.grappleRope)) %obj.grappleRope.delete();
			return;
		}

		%obj.updateGrapplePosition();
		GrappleRope(%obj, %obj.GrappleRopeCol, %obj.GrappleRopeTarget, %obj.GrappleRopePos);
	}
	else
	{
		GrappleRope(%obj, %obj.GrappleRopeCol, %obj.GrappleRopeTarget, %obj.GrappleRopePos);
		%vec = VectorSub(%obj.GrappleRopePos, %obj.getEyePoint());
	}

	%obj.TDMGGas = %obj.TDMGGas - 0.01;
	%obj.grappleLoop = %obj.schedule(16, grappleLoop);
}

function createRope(%a, %b) {
	%vec1 = vectorNormalize(vectorSub(%a, %b));
	%vec2 = vectorNormalize(vectorSub(%b, %a));

	%xyz = vectorNormalize(vectorCross("1 0 0", %vec1));
	%pow = mRadToDeg(mACos(vectorDot("1 0 0", %vec1))) * -1;

	%obj = new StaticShape() {
		datablock = RopeShapeData;
		scale = vectorDist(%a, %b) SPC 0.1 SPC 0.1;

		position = vectorScale(vectorAdd(%a, %b), 0.5);
		rotation = %xyz SPC %pow;

		a = %a;
		b = %b;
	};
	if (!isObject(RopeGroup)) {
		MissionCleanup.add(new SimGroup(RopeGroup));
	}
	RopeGroup.add(%obj);
	%obj.setNodeColor("ALL", "0.3 0.3 0.3 1.0" );
	return %obj;
}

function Player::RopeToProjectileLoop(%this)
{
	cancel(%this.RopeToProjectileLoop);
	if(%this.getState() $= "Dead" || !isObject(%this.grappleRopeProjectile))
	{
		if(isObject(%this.grappleRope)) %this.grappleRope.delete();
		return;
	}
	%a = %this.getHackPosition();
	%b = %this.grappleRopeProjectile.position;
	if(!isObject(%this.grappleRope))
	{
		%this.grappleRope = createRope(%a, %b);
	}
	else
	{
		%vec1 = vectorNormalize(vectorSub(%a, %b));
		%vec2 = vectorNormalize(vectorSub(%b, %a));

		%xyz = vectorNormalize(vectorCross("1 0 0", %vec1));
		%pow = mACos(vectorDot("1 0 0", %vec1)) * -1;
		%this.grappleRope.setScale(vectorDist(%a, %b) SPC 0.1 SPC 0.1);
		%position = vectorScale(vectorAdd(%a, %b), 0.5);
		%rotation = %xyz SPC %pow;
		%this.grappleRope.setTransform(%position SPC %rotation);
		%this.grappleRope.a = %a;
		%this.grappleRope.b = %b;
	}
	%this.RopeToProjectileLoop = %this.schedule(16, RopeToProjectileLoop);
}
