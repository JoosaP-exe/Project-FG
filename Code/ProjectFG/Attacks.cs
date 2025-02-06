using Jypeli;
using Jypeli.Assets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using FarseerPhysics.Collision;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

namespace ProjectFG;
public partial class ProjectFG
{
    private PhysicsObject lyonti;

    private void Attacks()
    {
        lyonti = new PhysicsObject(5, 5);
        lyonti.Position = pelaaja1.Position;
        lyonti.X = pelaaja1.X + 10;
        lyonti.IgnoresGravity = true;
    }
}