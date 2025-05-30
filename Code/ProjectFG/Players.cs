using System;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

namespace ProjectFG;

public partial class ProjectFG
{
    private const double Nopeus = 200;
    private const double HyppyNopeus = 500;
    private const int RuudunKoko = 40;
    private const double MaxHp = 200;
    private double MaxAika = 120;

    private PlatformCharacter _pelaaja1;

    private PlatformCharacter _pelaaja2;
    private DoubleMeter _hp1 = new DoubleMeter(200, 0, 200);
    private DoubleMeter _hp2 = new DoubleMeter(200, 0, 200);
    private ProgressBar[] hpBar = new ProgressBar[2];
    private Label[] hpTeksti = new Label[2];
    private Label tasapeli;
    private object pelaajat;

    private void LisaaPelaaja(Vector paikka1, Vector paikka2)
    {
        if (_pelaaja1 == null)
            _pelaaja1 = new PlatformCharacter(21, 32);
        _pelaaja1.Position = paikka1;
        _pelaaja1.Mass = 4.0;
        _pelaaja1.Image = _pelaajaKuva1;
        _pelaaja1.CollisionIgnoreGroup = 1;
        Add(_pelaaja1);

        _hp1 = new DoubleMeter(MaxHp, 0, MaxHp);
        _hp1.LowerLimit += delegate { PelaajaKuoli(1); };

        if (_pelaaja2 == null)
            _pelaaja2 = new PlatformCharacter(21, 32);
        _pelaaja2.Position = paikka2;
        _pelaaja2.Mass = 4.0;
        _pelaaja2.Image = _pelaajaKuva2;
        _pelaaja2.CollisionIgnoreGroup = 2;
        Add(_pelaaja2);

        _hp2 = new DoubleMeter(MaxHp, 0, MaxHp);
        _hp2.LowerLimit += delegate { PelaajaKuoli(2); };
    }

    private void HpBar()
    {
        ProgressBar hpBar1 = new ProgressBar(MaxHp, 20);
        hpBar1.BindTo(_hp1);
        hpBar1.Position = new Vector(Screen.Left + 120, Screen.Top - 30);
        hpBar1.Color = Color.Green;
        hpBar1.BorderColor = Color.Black;
        Add(hpBar1);

        ProgressBar hpBar2 = new ProgressBar(MaxHp, 20);
        hpBar2.BindTo(_hp2);
        hpBar2.Position = new Vector(Screen.Right - 120, Screen.Top - 30);
        hpBar2.Color = Color.Green;
        hpBar2.BorderColor = Color.Black;
        Add(hpBar2);

        Label hpTeksti1 = new Label("Pelaaja 1:");
        hpTeksti1.Position = new Vector(Screen.Left + 275, Screen.Top - 30);
        hpTeksti1.TextColor = Color.White;
        hpTeksti1.Color = Color.Transparent;
        Add(hpTeksti1);

        Label hpTeksti2 = new Label("Pelaaja 2:");
        hpTeksti2.Position = new Vector(Screen.Right - 275, Screen.Top - 30);
        hpTeksti2.TextColor = Color.White;
        hpTeksti2.Color = Color.Transparent;
        Add(hpTeksti2);
    }

    private void Ampuminen(PlatformCharacter ampuja, PlatformCharacter kohde, Animation animaatio, Image[] ampumiskuvat, SoundEffect aani)
    {
        PhysicsObject ammus = new PhysicsObject(2, 1);
        ammus.Position = ampuja.Position;
        ammus.Color = Color.Red;
        ammus.IgnoresGravity = true;
        ammus.Mass = 0;
        ammus.CollisionIgnoreGroup = ampuja.CollisionIgnoreGroup;
        ammus.Velocity = ampuja.FacingDirection == Direction.Right ? new Vector(500, 0) : new Vector(-500, 0);

        ampuja.Animation = new Animation(ampumiskuvat);
        ampuja.Animation.FPS = 8;
        ampuja.Size = new Vector(32, 32);
        ampuja.Animation.Start(1);

        Add(ammus);
        aani.Play();

        AddCollisionHandler(ammus, kohde, OsuukoPelaajaan);
    }

    private void OsuukoPelaajaan(PhysicsObject ammus, PhysicsObject kohde)
    {
        if (kohde == _pelaaja1)
        {
            ammus.Destroy();
            _hp1.Value -= 8;
        }
        else if (kohde == _pelaaja2)
        {
            ammus.Destroy();
            _hp2.Value -= 8;
        }
    }

    private void PelaajaKuoli(int pelaajaNro)
    {
        PlatformCharacter kuollut = (pelaajaNro == 1) ? _pelaaja1 : _pelaaja2;
        Image voittoKuva = (pelaajaNro == 1) ? _voitto2 : _voitto1;

        // Tee ragdoll-efekti: massa pieneksi ja nopeus
        kuollut.Mass = 0.1;
        kuollut.IgnoresGravity = false;
        kuollut.Velocity = (pelaajaNro == 1) ? new Vector(300, 200) : new Vector(-300, 200);

        // Näytä voittokuva
        GameObject voitto = new GameObject(384, 256)
        {
            Image = voittoKuva,
            Position = new Vector(0, 60)
        };
        Add(voitto);

        // Molemmat pelaajat eivät voi enää liikkua
        _pelaaja1.IgnoresPhysicsLogics = true;
        _pelaaja2.IgnoresPhysicsLogics = true;

        // 5 sekunnin päästä takaisin alkuun
        Timer.SingleShot(5.0, () =>
        {
            ClearAll();
            Begin();
        });
    }

    private void Ajastin()
    {
        Timer ajastin = new Timer();
        ajastin.Interval = 1.0;
        ajastin.Timeout += () =>
        {
            MaxAika--;
            if (MaxAika <= 0)
            {
                tasapeli = new Label("Tasapeli!")
                {
                    Position = new Vector(0, 60),
                    TextColor = Color.White,
                    Color = Color.Transparent
                };
                Add(tasapeli);
                MaxAika = 0;
            }
        };
        ajastin.Start();
    }
}

