using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Net.Mail;
using System.Xml;
using FarseerPhysics.Collision;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using Microsoft.DotNet.PlatformAbstractions;
namespace ProjectFG ; 
public partial class ProjectFG 
{
    private const double NOPEUS = 200;
    private const double HYPPYNOPEUS = 500;
    private const int RUUDUN_KOKO = 40;
    private const double MAX_HEALTH = 100;
    private double player1Health = MAX_HEALTH;
    private double player2Health = MAX_HEALTH;
    private PlatformCharacter pelaaja1;
    private PlatformCharacter pelaaja2;
    private PhysicsObject lyonti;
    private ProgressBar hp1;
    private ProgressBar hp2;


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
            pelaaja2.Mass = 0.0;
            pelaaja2.IgnoresGravity = true;
            pelaaja2.Image = pelaajakuva2;
            pelaaja2.CollisionIgnoreGroup = 2;
            Add(pelaaja2);
        }

            private void hpbar()
        {
            hp1 = new ProgressBar(250, 20);
            hp1.X = Screen.Left + 120;
            hp1.Y = Screen.Top - 50;
            hp1.Color = Color.Green;
            hp1.BorderColor = Color.Black;
            hp1.Width = player1Health;
            Add(hp1);


            hp2 = new ProgressBar(250, 20);
            hp2.X = Screen.Right - 120;
            hp2.Y = Screen.Top - 50;
            hp2.Color = Color.Green;
            hp2.BorderColor = Color.Black;
            hp2.Width = player2Health;
            Add(hp2);
        }

            private void LisaaHitbox(PlatformCharacter pelaaja1, PlatformCharacter pelaaja2)
        {
            PhysicsObject hitbox1 = PhysicsObject.CreateStaticObject(21, 32);
            AxleJoint HBL1 = new AxleJoint(hitbox1, pelaaja1);
            hitbox1.Position = pelaaja1.Position;
            hitbox1.Color = Color.Red;
            hitbox1.IsVisible = false;
            hitbox1.IgnoresGravity = true;
            hitbox1.CollisionIgnoreGroup = 1;
            HBL1.Softness = 0.0;
            pelaaja1.Add(hitbox1);

            PhysicsObject hitbox2 = PhysicsObject.CreateStaticObject(21, 32);
            AxleJoint HBL2 = new AxleJoint(hitbox2, pelaaja2);
            hitbox2.Position = pelaaja2.Position;
            hitbox2.Color = Color.Red;
            hitbox2.IgnoresGravity = true;
            hitbox2.IsVisible = false;
            hitbox2.CollisionIgnoreGroup = 2;
            HBL2.Softness = 0.0;
            pelaaja2.Add(hitbox2);
        }

        private void Attacks(PlatformCharacter hahmo)
        {
            lyonti = PhysicsObject.CreateStaticObject(5, 2);
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
            lyonti.Mass = 100;
            lyonti.CollisionIgnoreGroup = hahmo.CollisionIgnoreGroup;
            Add(lyonti);
            if (hahmo == pelaaja1)
            {
                AddCollisionHandler(lyonti, pelaaja2, PelaajatTormaavatHandler);
            }
            else if (hahmo == pelaaja2)
            {
                AddCollisionHandler(lyonti, pelaaja1, PelaajatTormaavatHandler);
            }
        }

        // This is the correct collision handler signature for AddCollisionHandler
        private void PelaajatTormaavatHandler(PhysicsObject lyonti, PhysicsObject kohde)
        {
            if (kohde == pelaaja1)
            {
                lyonti.Destroy(); // Tuhoaa lyöntiobjektin
                player1Health -= 10; // Vähennetään pelaaja 1:n terveyttä
                Console.WriteLine("Pelaaja 1:n terveys: " + player1Health);
                CheckHealth(player1Health, player2Health); // Tarkistaa pelaajien terveyden
                pelaaja1.Velocity = new Vector(-100, -100);
                hp1.Width = player2Health; // Nollaa pelaaja 1:n nopeuden
            }
            else if (kohde == pelaaja2)
            {
                lyonti.Destroy();
                player2Health -= 10; // Vähennetään pelaaja 2:n terveyttä
                Console.WriteLine("Pelaaja 2:n terveys: " + player2Health);
                CheckHealth(player1Health, player2Health); // Tarkistaa pelaajien terveyden
                pelaaja2.Velocity = new Vector(-100, -100);
                hp2.Width = player2Health;
            }
        }

        void CheckHealth(double player1Health, double player2Health)
        {
            if (player1Health <= 0)
            {
                pelaaja1.Destroy(); // Tuhoaa pelaaja 1:n
                Console.WriteLine("Pelaaja 2 voitti!");
            }
            else if (player2Health <= 0)
            {
                pelaaja2.Destroy(); // Tuhoaa pelaaja 2:n
                Console.WriteLine("Pelaaja 1 voitti!");
            }
            return;
        }
}

