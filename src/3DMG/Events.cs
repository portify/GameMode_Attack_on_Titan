registerOutputEvent("Player", "addGas", "int -99999 99999 0");
registerOutputEvent("Player", "setGas", "int -99999 99999 0");
registerOutputEvent("Player", "refillBlades");

function Player::addGas(%this, %val)
{
	%this.TDMGGas = %this.TDMGGas + %val;
	if(%this.TDMGGas > 5000)
	{
		%this.TDMGGas = 5000;
	}
	if(%this.TDMGGas < 0)
	{
		%this.TDMGGas = 0;
	}
}

function Player::setGas(%this, %val)
{
	%this.TDMGGas = %val;
	if(%this.TDMGGas > 5000)
	{
		%this.TDMGGas = 5000;
	}
	if(%this.TDMGGas < 0)
	{
		%this.TDMGGas = 0;
	}
}

function Player::refillBlades(%this)
{
	%this.TDMGBlades = 100;
	%this.TDMGSpareBlades = 4;
}