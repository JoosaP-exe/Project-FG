using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using FarseerPhysics.Collision;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
namespace ProjectFG ; 
public partial class ProjectFG 
{
    private const double NOPEUS = 200;
    private const double HYPPYNOPEUS = 500;
    private const int RUUDUN_KOKO = 40;
    private PhysicsObject hitbox1;
    private PhysicsObject hitbox2;
    private PlatformCharacter pelaaja1;
    private PlatformCharacter pelaaja2;
    private void InitalizeHitboxes()
    {
        hitbox1 = new PhysicsObject(50, 50);

        hitbox2 = new PhysicsObject(50, 50);
    }

    private void InitializePlayers()
    {
        pelaaja1 = new PlatformCharacter(50, 50);

        pelaaja2 = new PlatformCharacter(50, 50);
    }

            private void LisaaPelaaja(Vector paikka, double leveys, double korkeus)
        {
            pelaaja1 = new PlatformCharacter(21, 32);
            pelaaja1.Position = paikka;
            pelaaja1.Mass = 4.0;
            pelaaja1.Image = pelaajakuva1;
            pelaaja1.CollisionIgnoreGroup = 1;
            Add(pelaaja1);
        }

            private void LisaaPelaaja2(Vector paikka, double leveys, double korkeus)
        {
            pelaaja2 = new PlatformCharacter(21, 32);
            pelaaja2.Position = paikka;
            pelaaja2.Mass = 4.0;
            pelaaja2.Image = pelaajakuva2;
            pelaaja2.CollisionIgnoreGroup = 1;
            Add(pelaaja2);
        }

            private void LisaaHitbox(double leveys, double korkeus)
        {
            PhysicsObject hitbox1 = PhysicsObject.CreateStaticObject(21, 32);
            hitbox1.Position = pelaaja1.Position;
            hitbox1.Color = Color.Red;
            hitbox1.IgnoresGravity = true;
            hitbox1.CollisionIgnoreGroup = 1;
            Add(hitbox1);

            PhysicsObject hitbox2 = PhysicsObject.CreateStaticObject(21, 32);
            hitbox2.Position = pelaaja2.Position;
            hitbox2.Color = Color.Red;
            hitbox2.IgnoresGravity = true;
            hitbox2.CollisionIgnoreGroup = 1;
            Add(hitbox2);
        }
}

