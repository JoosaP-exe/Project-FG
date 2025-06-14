using System;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

namespace ProjectFG;

public partial class ProjectFG
{
/// <summary>Pelaajien nopeus</summary>
    private const double Nopeus = 200;

    /// <summary>Pelaajan hyppy nopeus</summary>
    private const double HyppyNopeus = 500;

    /// <summary>Ruudun koko.</summary>
    private const int RuudunKoko = 40;
    
    /// <summary>Pelaajan max hp</summary>
    private const double MaxHp = 200;

    /// <summary>Pelaaja 1</summary>
    private PlatformCharacter _pelaaja1;

    /// <summary>Pelaaja 2</summary>
    private PlatformCharacter _pelaaja2;

    /// <summary>Pelaaja 1 hp:eet</summary
    private DoubleMeter _hp1 = new DoubleMeter(200, 0, 200);

    /// <summary>Pelaaja 2 hp:eet</summary>
    private DoubleMeter _hp2 = new DoubleMeter(200, 0, 200);

    /// <summary>
    /// Lisää pelaajat kentälle 
    /// </summary>
    /// <param name="paikka">Pelaajan aloituspaikka </param>
    /// <param name="leveys">Pelaajan leveys </param>
    /// <param name="korkeus">Pelaajan korkeus </param>
    private void LisaaPelaaja(Vector paikka, double leveys, double korkeus, int pelaajaNumero)
    {
        if (pelaajaNumero == 1)
        {
            _pelaaja1 = new PlatformCharacter(21, 32);
            _pelaaja1.Position = paikka;
            _pelaaja1.Mass = 4.0;
            _pelaaja1.Image = _pelaajaKuva1;
            _pelaaja1.CollisionIgnoreGroup = 1;
            Add(_pelaaja1);
        }
        else if (pelaajaNumero == 2)
        {
            _pelaaja2 = new PlatformCharacter(21, 32);
            _pelaaja2.Position = paikka;
            _pelaaja2.Mass = 4.0;
            _pelaaja2.Image = _pelaajaKuva2;
            _pelaaja2.CollisionIgnoreGroup = 2;
            Add(_pelaaja2);
        }
    }

    /// <summary>
    /// Luo ja lisää hp:eet pelaajille
    /// </summary>
    private void HpBar()
    {
        ProgressBar hpBar1 = new ProgressBar(MaxHp, 20);
        hpBar1.BindTo(_hp1);
        hpBar1.Position = new Vector(Screen.Left + 120, Screen.Top - 30);
        hpBar1.BarColor = Color.Blue;
        hpBar1.Width = _hp1.Value;
        hpBar1.BorderColor = Color.Black;
        Add(hpBar1);

        ProgressBar hpBar2 = new ProgressBar(MaxHp, 20);
        hpBar2.BindTo(_hp2);
        hpBar2.Position = new Vector(Screen.Right - 120, Screen.Top - 30);
        hpBar2.BarColor = Color.Red;
        hpBar2.Width = _hp1.Value;
        hpBar2.BorderColor = Color.Black;
        Add(hpBar2);

        Label hpTeksti1 = new Label("Pelaaja 1:");
        hpTeksti1.Position = new Vector(Screen.Left + 275, Screen.Top - 30);
        hpTeksti1.TextColor = Color.Blue;
        hpTeksti1.Color = Color.Transparent;
        Add(hpTeksti1);

        Label hpTeksti2 = new Label("Pelaaja 2:");
        hpTeksti2.Position = new Vector(Screen.Right - 275, Screen.Top - 30);
        hpTeksti2.TextColor = Color.Red;
        hpTeksti2.Color = Color.Transparent;
        Add(hpTeksti2);
    }

    /// <summary>
    /// Luo ammuksen ja ampuu sen
    /// </summary>
    /// <param name="ampuja">Ampuva pelaaja</param>
    /// <param name="kohde">Pelaaja johon ammus osuu</param>
    /// <param name="animaatio">Ampumisanimaatio </param>
    /// <param name="ampumiskuvat">Ampumiskuvat animaatiolle </param>
    /// <param name="aani">Ampumisääni </param>
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

    /// <summary>
    /// Katsoo osuuko pelaajaan ja vähentää hp
    /// </summary>
    /// <param name="ammus">Ammus, joka ammutaan</param>
    /// <param name="kohde">Pelaaja, johon osutaan</param>
    private void OsuukoPelaajaan(PhysicsObject ammus, PhysicsObject kohde)
    {
        if (kohde == _pelaaja1)
        {
            _hp1.Value -= 8;
        }
        else if (kohde == _pelaaja2)
        {
            _hp2.Value -= 8;
        }

        ammus.Destroy();
    }


    /// <summary>
    /// Tässä tehään pelaajan kuoleminen ja voittaminen
    /// </summary>
    /// <param name="_hp1">Pelaaja 1 hp:eet</param>
    /// <param name="_hp2">Pelaaja 2 hp:eet</param>
    private void PelaajaKuoli()
    {
        if (_hp1.Value <= 0)
        {
            GameObject _gameVoitto2 = new GameObject(1536, 1024);
            _gameVoitto2.Image = _voitto2;
            _gameVoitto2.Position = new Vector(0, 60);
            _gameVoitto2.Size = new Vector(384, 256);
            Add(_gameVoitto2);

            if (_pelaaja1 != null && !_pelaaja1.IsDestroyed)
                _pelaaja1.Destroy();
        }
        else if (_hp2.Value <= 0)
        {
            GameObject _gameVoitto1 = new GameObject(1536, 1024);
            _gameVoitto1.Image = _voitto1;
            _gameVoitto1.Position = new Vector(0, 60);
            _gameVoitto1.Size = new Vector(384, 256);
            Add(_gameVoitto1);

            if (_pelaaja2 != null && !_pelaaja2.IsDestroyed)
                _pelaaja2.Destroy();

        }
        Timer.SingleShot(5.0, () =>
        {
            ClearAll();
            Begin();
        });
    }
}

