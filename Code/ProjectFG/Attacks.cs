using Jypeli;
using Jypeli.Assets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using FarseerPhysics.Collision;
using Jypeli.Controls;
using Jypeli.Widgets;
using FarseerPhysics.Dynamics;
using Jypeli.Physics;
using System.Threading;

namespace ProjectFG;
public partial class ProjectFG
{
    private PhysicsObject lyonti;

    private void Attacks(PlatformCharacter hahmo)
    {
        lyonti = new PhysicsObject(5, 5);
        lyonti.Position = hahmo.Position;
        if (hahmo.FacingDirection == Direction.Right)
        {
            lyonti.Velocity = new Vector(500, 0);
        }
        else
        {
            lyonti.Velocity = new Vector(-500, 0);
        }
        lyonti.Color = Color.Red;
        lyonti.IgnoresGravity = true;
        lyonti.CollisionIgnoreGroup = hahmo.CollisionIgnoreGroup;
        lyonti.Mass = 0.0;
        Add(lyonti);

        if (lyonti.Position == pelaaja1.Position)
        {
            player1Health -= 10;
            healthBar1.Width = player1Health;
            Remove(lyonti);
            
        }
        else if (lyonti.Position == pelaaja2.Position)
        {
            player2Health -= 10;
            healthBar2.Width = player2Health;
            Remove(lyonti);
        }
        }
    }