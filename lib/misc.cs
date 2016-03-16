datablock staticShapeData(CubeGlowShapeData)
{
  shapeFile = $AOT::Folder @ "/res/shapes/cube_glow.dts";
};

// Search for bricks in the way of a given solid line using box searches.
// Returns 1 if an obstruction was found, 0 otherwise.
function hullTrace(%position, %target, %bounds, %masks, %ignore, %limit)
{
  if (%limit $= "")
  {
    %limit = 40;
  }

  if (%masks $= "")
  {
    %masks = $TypeMasks::All;
  }

  %delta = vectorSub(%target, %position);
  %count = vectorLen(%delta) / (vectorLen(%bounds) / 4);

  if (%count > %limit)
  {
    %count = %limit;
  }

  %step = vectorScale(%delta, 1 / %count);

  for (%i = 0; %i < %count; %i++)
  {
    %box = vectorScale(%bounds, 0.25);
    %pos = setWord(%position, 2,
      getWord(%position, 2) + getWord(%box, 2) / 2);

    initContainerBoxSearch(%position, %box, %masks);
    %position = vectorAdd(%position, %step);
    if ($HullTrace::Debug)
    {
      %obj = new StaticShape() { datablock = CubeGlowShapeData; }; MissionCleanup.add(%obj);

      %obj.setScale(%box);
      %obj.setTransform(%pos);

      if (%col)
        %color = "1 0 0 0.7";
      else
        %color = "0 1 0 0.7";

      %obj.setNodeColor("ALL", %color);
      %obj.schedule(100, delete);
    }
    %col = containerSearchNext();
  }

  return %col ? %col : 0;
}

// Determine whether the player would be in a brick if placed at *%position*.
function Player::collidesAt(%this, %position)
{
  %armor = %this.getDataBlock();

  %box = vectorScale(%this.isCrouched() ?
    %armor.crouchBoundingBox : %armor.boundingBox, 0.25);

  initContainerBoxSearch(%position, %box, $TypeMasks::FxBrickObjectType);
  return isObject(containerSearchNext());
}

// Rotate/offset *%vector* randomly by *%spread* radians.
function vectorSpread(%vector, %spread)
{
  %scalars = getRandomScalar() SPC getRandomScalar() SPC getRandomScalar();
  %scalars = vectorScale(%scalars, %spread);

  return matrixMulVector(matrixCreateFromEuler(%scalars), %vector);
}

// Find the smallest of the 3 values.
function getMin3(%a, %b, %c)
{
  return %a < %b ? (%a < %c ? %a : %c) : (%b < %c ? %b : %c);
}

// Find the biggest of the 3 values.
function getMax3(%a, %b, %c)
{
  return %a > %b ? (%a > %c ? %a : %c) : (%b > %c ? %b : %c);
}

// Return a random floating point value from -1.0f to 1.0f.
function getRandomScalar()
{
  return getRandom() * 2 - 1;
}