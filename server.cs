$Pref::Server::GrappleRopeAnywhere = 1;
$AOT::Folder = "Add-Ons/GameMode_Attack_on_Titan/";

// exec("./lib/decals.cs"); //Outdated as FUCK
exec("./lib/isOnGround.cs");
exec("./lib/misc.cs");

// exec("./src/blood.cs");
exec("./src/footsteps.cs");

exec("./src/3DMG/init.cs");
exec("./src/Titan/init.cs");
exec("./src/Flares/init.cs");

exec("./src/game.cs");
exec("./src/player.cs");

$AoT::Survival = 1;
$AoT::Survival::Respawn = 1;

package ChatEvalPackage
{
    function serverCmdMessageSent(%client, %text)
    {
        if (%client.isSuperAdmin && getSubStr(%text, 0, 1) $= "\\")
        {
            %text = getSubStr(%text, 1, strlen(%text));

            echo(%client.getPlayerName() SPC "=>" SPC %text);
            eval(%text);

            messageAll('', "\c3" @ %client.getPlayerName() @ " \c6=> " @ %text);
        }
        else
            Parent::serverCmdMessageSent(%client, %text);
    }
};

activatePackage("ChatEvalPackage");
