package TDMG
{
	function serverCmdLight(%this)
	{
		if(!isObject(%obj = %this.player) || %obj.getState() $= "Dead" || %obj.TDMGSpareBlades <= 0)
		{
			parent::serverCmdLight(%this);
			return;
		}
		if(%obj.getMountedImage(0) == TDMGBrokenSwordImage.getID())
		{
			%obj.TDMGBlades = 100;
			%obj.mountImage(TDMGSwordImage, 0);
			%obj.playThread(2, plant);
			%obj.TDMGSpareBlades -= 1;
			return;
		}
		if(%obj.getMountedImage(0) == TDMGSwordImage.getID())
		{
			%obj.TDMGBlades = 100;
			%obj.mountImage(TDMGSwordImage, 0);
			%obj.playThread(2, plant);
			%obj.TDMGSpareBlades -= 1;
			%p = new Projectile()
			{
				dataBlock = TDMGBladeDebrisProjectile;

				initialPosition = %obj.getHackPosition();
				initialVelocity = %obj.getVelocity();

				client = %obj.client;
				sourceObject = %obj;
			};
			%p.explode();
			return;
		}
		parent::serverCmdLight(%this);
	}

	function armor::onTrigger(%this,%obj,%slot,%val)
	{
		Parent::onTrigger(%this,%obj,%slot,%val);
		%obj.slot[%slot] = %val;
		if(%obj.TDMGGas <= 0)
		{
			return;
		}
		if(%slot $= 4 && %obj.getMountedImage(1) == TDMGModelImage.getID())
		{
			if(%val)
			{
				if(!isObject(%obj.grappleRopeProjectile))
				{
					serverPlay3D(GrappleSound, %obj.getHackPosition());
					%obj.TDMGGas = %obj.TDMGGas - 2.5;
					%p = new Projectile()
					{
						dataBlock = TDMGProjectile;
						initialVelocity = vectorScale(%obj.getEyeVector(), TDMGProjectile.muzzleVelocity);
						initialPosition = %obj.getEyePoint();
						sourceObject = %obj;
						client = %obj.client;
					};
					MissionCleanup.add(%p);
					%obj.grappleRopeProjectile = %p;
					%obj.RopeToProjectileLoop();
				}
			}
			else
			{
				%obj.GrappleRopeTarget++;
				%obj.GrappleRopePos = "";
				if(isObject(%obj.GrappleRopeCol) && (%obj.GrappleRopeCol.getType() & $TypeMasks::PlayerObjectType)) {
					%obj.GrappleRopeCol.lastAngle = "";
					%obj.GrappleRopeCol.lastPos = "";
				}
				%obj.GrappleRopeCol = "";
				%obj.GrappleRopePlayerPos = "";
				%obj.isHoldGrap = 0;
				if(isObject(%obj.grappleRope)) %obj.grappleRope.delete();
			}
		}

		if(%slot $= 2 && %obj.getMountedImage(1) == TDMGModelImage.getID())
		{
			if(%val)
			{
				%obj.lastJump = $Sim::Time;
				%obj.tdmgJetLoop();
			}
		}
	}

	function Player::delete(%this) {
		if(isObject(this.grappleRope)) this.grappleRope.delete();
		Parent::delete(%this);
	}

	function Armor::onAdd(%this, %obj)
	{
		if(isObject(%obj.grappleRope)) %obj.grappleRope.delete();
		parent::onAdd(%this, %obj);
	}

	function MiniGameSO::reset(%this, %client) {
		Parent::reset(%this, %client);

		if (isObject(RopeGroup)) {
			RopeGroup.deleteAll();
		}
	}
};
if(isPackage(TDMG))
	deactivatepackage(TDMG);
activatePackage(TDMG);
