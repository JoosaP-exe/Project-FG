using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics.Tracing;
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
    private const double MAX_HEALTH = 200;
    private double player1Health = MAX_HEALTH;
    private double player2Health = MAX_HEALTH;
    private PlatformCharacter pelaaja1;
    private PlatformCharacter pelaaja2;
    private PhysicsObject lyonti;
    private ProgressBar hp1;
    private ProgressBar hp2;
    private PhysicsObject p1ragdoll;
    private PhysicsObject p2ragdoll;
    private Vector ListenedPosition1;
    private Vector ListenedPosition2;
    private Label pelaaja1HP;
    private Label pelaaja2HP;

    ///  private Image voitto1;
    /// private Image voitto2;
    /// private GameObject GameVoitto1;
    /// private GameObject GameVoitto2;



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
        pelaaja2.CollisionIgnoreGroup = 2;
        Add(pelaaja2);
    }

    private void hpbar()
    {
        if (hp1 != null) Remove(hp1);
        if (hp2 != null) Remove(hp2);

        hp1 = new ProgressBar(player1Health, 20);
        hp1.Position = new Vector(Screen.Left + 120, Screen.Top - 30);
        hp1.Color = Color.Green;
        hp1.BorderColor = Color.Black;
        Add(hp1);

        hp2 = new ProgressBar(player2Health, 20);
        hp2.Position = new Vector(Screen.Right - 120, Screen.Top - 30);
        hp2.Color = Color.Green;
        hp2.BorderColor = Color.Black;
        Add(hp2);
        
        pelaaja1HP = new Label("Pelaaja 1:");
        pelaaja1HP.Position = new Vector(Screen.Left + 275, Screen.Top - 30);
        pelaaja1HP.TextColor = Color.White;
        pelaaja1HP.Color = Color.Transparent;
        Add(pelaaja1HP);

        pelaaja2HP = new Label("Pelaaja 2:");
        pelaaja2HP.Position = new Vector(Screen.Right - 275, Screen.Top - 30);
        pelaaja2HP.TextColor = Color.White;
        pelaaja2HP.Color = Color.Transparent;
        Add(pelaaja2HP);
        }




    private void Attacks(PlatformCharacter hahmo)
    {
        if (hahmo.IsDestroyed)
        {
            return;
        }
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


    private void PelaajatTormaavatHandler(PhysicsObject lyonti, PhysicsObject kohde)
    {
        if (kohde == pelaaja1)
        {
            lyonti.Destroy(); // Tuhoaa lyöntiobjektin
            player1Health -= 10; // Vähennetään pelaaja 1:n terveyttä
            Console.WriteLine("Pelaaja 1:n terveys: " + player1Health);
            CheckHealth(player1Health, player2Health); // Tarkistaa pelaajien terveyden
            pelaaja1.Velocity = new Vector(0, 1000); // Asetetaan pelaaja 1:n nopeus
            pelaaja1.Velocity = new Vector(-1000, 0);
            hp1.Width = player2Health; // Nollaa pelaaja 1:n nopeuden
        }
        else if (kohde == pelaaja2)
        {
            lyonti.Destroy();
            player2Health -= 10; // Vähennetään pelaaja 2:n terveyttä
            Console.WriteLine("Pelaaja 2:n terveys: " + player2Health);
            CheckHealth(player1Health, player2Health); // Tarkistaa pelaajien terveyden
            pelaaja2.Velocity = new Vector(0, 1000);
            pelaaja2.Velocity = new Vector(-1000, 0);
            hp2.Width = player2Health;
        }
    }


    void CheckHealth(double player1Health, double player2Health)
    {
        if (player1Health <= 0)
        {
            pelaaja1.Destroy(); // Tuhoaa pelaaja 1:n
            Console.WriteLine("Pelaaja 2 voitti!");
            p1ragdoll = new PhysicsObject(21, 32);
            p1ragdoll.Mass = 0.0;
            p1ragdoll.Image = pelaajakuva1;
            p1ragdoll.CollisionIgnoreGroup = 1;
            p1ragdoll.Position = pelaaja1.Position;
            p1ragdoll.Velocity = new Vector(300, 200);
            p1ragdoll.IgnoresGravity = true;
            Add(p1ragdoll);
            /// Image voitto1 = LoadImage("voitto.png");

        }
        else if (player2Health <= 0)
        {
            pelaaja2.Destroy(); // Tuhoaa pelaaja 2:n
            Console.WriteLine("Pelaaja 1 voitti!");
            p2ragdoll = new PhysicsObject(21, 32);
            p2ragdoll.Mass = 0.0;
            p2ragdoll.Image = pelaajakuva2;
            p2ragdoll.CollisionIgnoreGroup = 2;
            p2ragdoll.Position = pelaaja2.Position;
            p2ragdoll.Velocity = new Vector(-300, 200);
            p2ragdoll.IgnoresGravity = true;
            Add(p2ragdoll);
            /// GameVoitto2 = new GameObject(100, 100);
            /// GameVoitto2.SetImage("voitto.png");
            /// GameVoitto2.Position = new Vector(0, 0);
            /// Add(GameVoitto2);
        }
        return;
    }

private void AddPositionListener(PlatformCharacter pelaaja1, PlatformCharacter pelaaja2)
{
    // Create a timer to listen for position changes
    Timer positionTimer = new Timer();
    positionTimer.Interval = 0.01; // seconds
    positionTimer.Timeout += () =>
    {
        // Do something with pelaaja.Position
        ListenedPosition1 = pelaaja1.Position;
        ListenedPosition2 = pelaaja2.Position;
        // You can add your own logic here
    };
    positionTimer.Start();
}
        
}

