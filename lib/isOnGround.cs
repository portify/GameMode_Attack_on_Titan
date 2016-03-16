function player::isOnGround( %this )
{
	%offset[ 0 ] = "0 0";
	%offset[ 1 ] = "0.5 0";
	%offset[ 2 ] = "-0.5 0";
	%offset[ 3 ] = "0 0.5";
	%offset[ 4 ] = "0 -0.5";

	for ( %i = 0 ; %i < 5 ; %i++ )
	{
		if ( %this._isOnGround( %offset[ %i ] ) )
		{
			return true;
		}
	}
	return false;
}

function player::_isOnGround( %this, %offset )
{
	%start = vectorAdd( %this.position, %offset SPC "0.4" );
	%stop = vectorSub( %this.position, %offset SPC "0.4" );
	%raycast = containerRayCast( %start, %stop, $TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType, %this );

	return isObject( firstWord( %raycast ) );
}