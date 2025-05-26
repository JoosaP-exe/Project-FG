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
    private double _pelaaja1Hp = MaxHp;
    private double _pelaaja2Hp = MaxHp;
    private PlatformCharacter _pelaaja1;
    private PlatformCharacter _pelaaja2;
    private PhysicsObject _lyonti;
    private ProgressBar _hp1;
    private ProgressBar _hp2;
    private PhysicsObject _p1Ragdoll;
    private PhysicsObject _p2Ragdoll;
    private Label _pelaaja1HpTeksti;
    private Label _pelaaja2HpTeksti;
    private double MaxAika = 120;
    private GameObject _gameVoitto1;
    private GameObject _gameVoitto2;
    private Label _tasapeli;


    private void LisaaPelaaja(Vector paikka, double leveys, double korkeus)
    {
        _pelaaja1 = new PlatformCharacter(21, 32);
        _pelaaja1.Position = paikka;
        _pelaaja1.Mass = 4.0;
        _pelaaja1.Image = _pelaajaKuva1;
        _pelaaja1.CollisionIgnoreGroup = 1;
        Add(_pelaaja1);
    }


    private void LisaaPelaaja2(Vector paikka, double leveys, double korkeus)
    {
        _pelaaja2 = new PlatformCharacter(21, 32);
        _pelaaja2.Position = paikka;
        _pelaaja2.Mass = 4.0;
        _pelaaja2.Image = _pelaajaKuva2;
        _pelaaja2.CollisionIgnoreGroup = 2;
        Add(_pelaaja2);
    }


    private void HpBar()
    {
        ProgressBar[] hPPaarit = new ProgressBar[2];
        Label[] teksti = new Label[2];
        double[] healtit = { _pelaaja1Hp, _pelaaja2Hp };
        Vector[] paarienSijanti = {
            new Vector(Screen.Left + 120, Screen.Top - 30),
            new Vector(Screen.Right - 120, Screen.Top - 30)
        };
        Vector[] labelPositions = {
            new Vector(Screen.Left + 275, Screen.Top - 30),
            new Vector(Screen.Right - 275, Screen.Top - 30)
        };
        string[] nimet = { "Pelaaja 1:", "Pelaaja 2:" };

        for (int i = 0; i < 2; i++)
        {
            hPPaarit[i] = new ProgressBar(healtit[i], 20);
            hPPaarit[i].Position = paarienSijanti[i];
            hPPaarit[i].Color = Color.Green;
            hPPaarit[i].BorderColor = Color.Black;
            Add(hPPaarit[i]);

            teksti[i] = new Label(nimet[i]);
            teksti[i].Position = labelPositions[i];
            teksti[i].TextColor = Color.White;
            teksti[i].Color = Color.Transparent;
            Add(teksti[i]);
        }

        _hp1 = hPPaarit[0];
        _hp2 = hPPaarit[1];
        _pelaaja1HpTeksti = teksti[0];
        _pelaaja2HpTeksti = teksti[1];
    }


    private void Ajastin(double maxAika)
    {
        Jypeli.Timer timer = new Jypeli.Timer();
        _tasapeli = new Label(maxAika.ToString("0"));
        _tasapeli.Position = new Vector(0, Screen.Top - 30);
        _tasapeli.TextColor = Color.White;
        _tasapeli.Color = Color.Transparent;
        Add(_tasapeli);

        timer.Interval = 1;
        timer.Timeout += () =>
        {
            maxAika -= 1;
            _tasapeli.Text = maxAika.ToString("0");

            if (_pelaaja1.IsDestroyed || _pelaaja2.IsDestroyed)
            {
                timer.Pause();
                return;
            }

            if (maxAika <= 0)
            {
                timer.Stop();

                Console.WriteLine("Tasapeli!");
                GameObject tasapeli = new GameObject(1536, 1024);
                tasapeli.Image = _aikaLoppui;
                tasapeli.Position = new Vector(0, 60);
                tasapeli.Size = new Vector(384, 256);
                Add(tasapeli);
                _pelaaja1.Destroy();
                _pelaaja2.Destroy();

                Jypeli.Timer.SingleShot(5.0, () =>
                {
                    ClearAll();
                    Begin();
                });
            }
        };

        timer.Start();
    }


    private void Lyominen(PlatformCharacter hahmo)
    {
        _lyonti = new PhysicsObject(2, 1);
        _lyonti.Position = hahmo.Position;

        if (hahmo.IsDestroyed)
        {
            return;
        }

        if (hahmo.FacingDirection == Direction.Right)
        {
            _lyonti.Velocity = new Vector(500, 0);
        }
        else
        {
            _lyonti.Velocity = new Vector(-500, 0);
        }

        _lyonti.Color = Color.Red;
        _lyonti.IgnoresGravity = true;
        _lyonti.Mass = 0;
        _lyonti.CollisionIgnoreGroup = hahmo.CollisionIgnoreGroup;

        if (hahmo == _pelaaja1)
        {
            _pelaaja1.Animation = new Animation(_bAmpuminen);
            _pelaaja1.Animation.FPS = 8;

            if (_pelaaja1.FacingDirection == Direction.Left)
            {
                Animation mbampuminen = Animation.Mirror(_pelaaja1.Animation);
            }

            _pelaaja1.Size = new Vector(32, 32);
            _pelaaja1.Animation.Start(1);
            Add(_lyonti);
            _gunShot.Play();
        }
        else
        {
            _pelaaja2.Animation = new Animation(_rAmpuminen);
            _pelaaja2.Animation.FPS = 8;

            if (_pelaaja2.FacingDirection == Direction.Left)
            {
                Animation mrampuminen = Animation.Mirror(_pelaaja2.Animation);
            }

            _pelaaja2.Size = new Vector(32, 32);
            _pelaaja2.Animation.Start(1);
            Add(_lyonti);
            _gunShot.Play();
        }

        if (hahmo == _pelaaja1)
        {
            AddCollisionHandler(_lyonti, _pelaaja2, PelaajatTormaa);
        }
        else if (hahmo == _pelaaja2)
        {
            AddCollisionHandler(_lyonti, _pelaaja1, PelaajatTormaa);
        }
    }


    private void PelaajatTormaa(PhysicsObject lyonti, PhysicsObject kohde)
    {
        if (kohde == _pelaaja1)
        {
            lyonti.Destroy();
            _pelaaja1Hp -= 20;
            Console.WriteLine("Pelaaja 1:n terveys: " + _pelaaja1Hp);
            CheckHealth(_pelaaja1Hp, _pelaaja2Hp);
            _hp1.Width = _pelaaja1Hp;
        }
        else if (kohde == _pelaaja2)
        {
            lyonti.Destroy();
            _pelaaja2Hp -= 20;
            Console.WriteLine("Pelaaja 2:n terveys: " + _pelaaja2Hp);
            CheckHealth(_pelaaja1Hp, _pelaaja2Hp);
            _hp2.Width = _pelaaja2Hp;
        }
    }


    private void CheckHealth(double player1Health, double player2Health)
    {
        if (player1Health <= 0)
        {
            _pelaaja1.Destroy();
            Console.WriteLine("Pelaaja 2 voitti!");
            _p1Ragdoll = new PhysicsObject(21, 32);
            _p1Ragdoll.Mass = 0.4925;
            _p1Ragdoll.Image = _pelaajaKuva1;
            _p1Ragdoll.CollisionIgnoreGroup = 1;
            _p1Ragdoll.Position = _pelaaja1.Position;
            _p1Ragdoll.Velocity = new Vector(300, 200);
            _p1Ragdoll.IgnoresGravity = true;
            Add(_p1Ragdoll);
            _gameVoitto2 = new GameObject(1536, 1024);
            _gameVoitto2.Image = _voitto2;
            _gameVoitto2.Position = new Vector(0, 60);
            _gameVoitto2.Size = new Vector(384, 256);
            Add(_gameVoitto2);
            _pelaaja1.Destroy();
            _pelaaja2.Destroy();

            Jypeli.Timer.SingleShot(5.0, () =>
            {
                ClearAll();
                Begin();
            });
        }
        else if (player2Health <= 0)
        {
            _pelaaja2.Destroy();
            Console.WriteLine("Pelaaja 1 voitti!");
            _p2Ragdoll = new PhysicsObject(21, 32);
            _p2Ragdoll.Mass = 0.4925;
            _p2Ragdoll.Image = _pelaajaKuva2;
            _p2Ragdoll.CollisionIgnoreGroup = 2;
            _p2Ragdoll.Position = _pelaaja2.Position;
            _p2Ragdoll.Velocity = new Vector(-300, 200);
            _p2Ragdoll.IgnoresGravity = true;
            Add(_p2Ragdoll);
            _gameVoitto1 = new GameObject(1536, 1024);
            _gameVoitto1.Image = _voitto1;
            _gameVoitto1.Position = new Vector(0, 60);
            _gameVoitto1.Size = new Vector(384, 256);
            Add(_gameVoitto1);
            _pelaaja1.Destroy();
            _pelaaja2.Destroy();

            Jypeli.Timer.SingleShot(5.0, () =>
            {
                ClearAll();
                Begin();
            });
        }

        return;
    }


    private void Barrieri(PhysicsObject pelaaja, PhysicsObject barrieri)
    {
        if (pelaaja == _pelaaja1)
        {
            _pelaaja1Hp = 0;
            _hp1.Width = 0;
            CheckHealth(_pelaaja1Hp, _pelaaja2Hp);
        }
        else if (pelaaja == _pelaaja2)
        {
            _pelaaja2Hp = 0;
            _hp2.Width = 0;
            CheckHealth(_pelaaja1Hp, _pelaaja2Hp);
        }
    }


}

