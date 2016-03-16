$Pref::Server::GrappleRopeAnywhere = 1;
$AOT::Folder = "Add-Ons/GameMode_Attack_on_Titan/";

exec("./lib/decals.cs");
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
