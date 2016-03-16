function Player::addItem(%player,%image,%client)
{
   for(%i = 0; %i < %player.getDatablock().maxTools; %i++)
   {
      %tool = %player.tool[%i];
      if(%tool == 0)
      {
         %player.tool[%i] = %image;
         %player.weaponCount++;
         messageClient(%client,'MsgItemPickup','',%i,%image);
         break;
      }
   }
}
