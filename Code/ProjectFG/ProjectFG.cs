using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using FarseerPhysics.Collision;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

namespace ProjectFG
{
    /// <author>Toxo22, Joosap, Valkohattu</author>
    /// <version>26.05.2025</version>
    /// <summary>
    /// Pelin pääluokka, jossa kaikki jossa suurin piirtein kaikki
    /// </summary>
    public partial class ProjectFG : PhysicsGame
    {
        /// <summary>Taustakuva</summary>
        private Image _taustaKuva = LoadImage("main-menu.png");
        /// <summary>Pelaaja 1 kuva</summary>
        private Image _pelaajaKuva1 = LoadImage("playerkuva.png");
        /// <summary>Pelaaja 2 kuva</summary>
        private Image _pelaajaKuva2 = LoadImage("playerkuva2.png");
        /// <summary>Pelaaja 1 voiton kuva</summary>
        private Image _voitto1 = LoadImage("pleijer1voitto.png");
        /// <summary>Pelaaja 2 voiton kuva</summary>
        private Image _voitto2 = LoadImage("pleijer2voitto.png");
        /// <summary>Ajan loppumisen kuva</summary>
        private Image _aikaLoppui = LoadImage("aikaloppui.png");
        /// <summary>Ääniefekti ampumiselle</summary>
        private SoundEffect _gunShot = LoadSoundEffect("peakgun.wav");
        /// <summary>Pelaaja 1 ampumisanimaatiot</summary>
        private Image[] _bAmpuminen = LoadImages("1ampuuframe1.png", "1ampuuframe2.png", "1ampuuframe3.png", "1ampuuframe4.png");
        /// <summary>Pelaaja 2 ampumisanimaatiot</summary>
        private Image[] _rAmpuminen = LoadImages("2ampuuframe1.png", "2ampuuframe2.png", "2ampuuframe3.png", "2ampuuframe4.png");
        /// <summary>Pelaaja 1 kävelyanimaatiot</summary>
        private Image[] _bAveleminen = LoadImages("kavelee1.png", "kavelee2.png", "kavelee3.png", "kavelee4.png", "kavelee5.png", "kavelee6.png");
        /// <summary>Pelin pausemenu</summary>
        private MultiSelectWindow _pauseValikko;

        /// <summary>
        /// Käynnistää koko höskän (pelin)
        /// </summary>
        public override void Begin()
        {
            Image menuKuva = LoadImage("main-menu.png");
            Level.Background.Image = menuKuva;

            string[] vaihtoehdot = { "PELAA", "POISTU" };
            MultiSelectWindow alkuValikko = new MultiSelectWindow("PÄÄVALIKKO", vaihtoehdot);

            alkuValikko.AddItemHandler(0, Alotus);
            alkuValikko.AddItemHandler(1, Exit);

            Add(alkuValikko);

            MediaPlayer.Play("mainmenu");
        }


        /// <summary>
        /// Aloittaa pelin ja alustaa kaiken
        /// </summary>
        public void Alotus()
        {

            pelaajienMaara = 0;
            _pelaaja1 = null;
            _pelaaja2 = null;
            Gravity = new Vector(0, -1000);

            _hp1.Value = 200;
            _hp2.Value = 200;

            LuoKentta();
            LisaaNappaimet();
            HpBar();
            Ajastin();

            Camera.Position = new Vector(0, 0);
            Camera.ZoomFactor = 2;
            Camera.StayInLevel = false;

            MediaPlayer.Volume = 0.3;
            MediaPlayer.Play("combat");

            MasterVolume = 0.5;
        }


        /// <summary>
        /// Luo kentän teksti filun avulla
        /// </summary>
        private void LuoKentta()
        {
            TileMap kentta = TileMap.FromLevelAsset("kentta1.txt");
            kentta.SetTileMethod('#', LisaaTaso);
            kentta.SetTileMethod('N', LisaaPelaaja);
            kentta.SetTileMethod('M', LisaaPelaaja);
            kentta.Execute(RuudunKoko, RuudunKoko);

            Level.Background.Image = _taustaKuva;
        }


        /// <summary>
        /// Lisää kentälle tason
        /// </summary>
        private void LisaaTaso(Vector paikka, double leveys, double korkeus)
        {
            PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus + 10);
            taso.Position = paikka;
            taso.Color = Color.DarkBlue;
            Add(taso);
        }



        /// <summary>
        /// Lisäätään näppäimet pelaajille
        /// </summary>
        private void LisaaNappaimet()
        {
            Keyboard.Listen(Key.A, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", _pelaaja1, -Nopeus);
            Keyboard.Listen(Key.D, ButtonState.Down, Liikuta, "Liikkuu oikealle", _pelaaja1, Nopeus);
            Keyboard.Listen(Key.W, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", _pelaaja1, HyppyNopeus);
            Keyboard.Listen(Key.F, ButtonState.Pressed, () => Ampuminen(_pelaaja1, _pelaaja2, new Animation(_bAmpuminen), _bAmpuminen, _gunShot), "Pelaaja lyö");


            Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", _pelaaja2, -Nopeus);
            Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu oikealle", _pelaaja2, Nopeus);
            Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", _pelaaja2, HyppyNopeus);
            Keyboard.Listen(Key.Down, ButtonState.Pressed, Hyppaa, "Pelaaja ALAS!", _pelaaja2, -HyppyNopeus);
            Keyboard.Listen(Key.RightControl, ButtonState.Pressed, () => Ampuminen(_pelaaja2, _pelaaja1, new Animation(_rAmpuminen), _rAmpuminen, _gunShot), "Pelaaja lyö");

            Keyboard.Listen(Key.Escape, ButtonState.Pressed, Pausetus, "Pysäyttää pelin");
        }


        /// <summary>
        /// Pistää pelaajan liikkumaa
        /// </summary>
        private void Liikuta(PlatformCharacter hahmo, double nopeus)
        {
            hahmo.Walk(nopeus);
            _pelaaja1.AnimWalk = new Animation(_bAveleminen);
        }


        /// <summary>
        /// antaa pelaajan hypätä
        /// </summary>
        private void Hyppaa(PlatformCharacter hahmo, double nopeus)
        {
            hahmo.Jump(nopeus);
        }


        /// <summary>
        /// Avaa tai sulkee pausemenun
        /// </summary>
        private void Pausetus()
        {
            if (IsPaused)
            {
                Remove(_pauseValikko);
                _pauseValikko = null;
                Pause(); // Jatkaa peliä
            }
            else
            {
                if (_pauseValikko == null)
                {
                    _pauseValikko = new MultiSelectWindow("TAUKO", "ALOITA ALUSTA", "POISTU");
                    _pauseValikko.Closed += (handler) => SuljeValikko();
                    _pauseValikko.AddItemHandler(0, AloitaAlusta);
                    _pauseValikko.AddItemHandler(1, Exit);
                }

                Add(_pauseValikko);
                Pause(); // Pysäyttää pelin
            }
        }


        /// <summary>
        /// Sulkee pausemenun
        /// </summary>
        private void SuljeValikko()
        {
            if (_pauseValikko != null)
            {
                Remove(_pauseValikko);
                _pauseValikko = null;
                Pause(); // Tämä jatkaa peliä!
            }
        }


        /// <summary>
        /// Aloittaa pelin alusta ja alustaa kaiken alusta
        /// </summary>
        private void AloitaAlusta()
        {

        pelaajienMaara = 0;
        _pelaaja1 = null;
        _pelaaja2 = null;

            ClearAll();
            LuoKentta();

            
            _hp1.Value = 200;
            _hp2.Value = 200;

            HpBar();
            LisaaNappaimet();
            Ajastin();

            Camera.Position = new Vector(0, 0);
            Camera.ZoomFactor = 2;
            Camera.StayInLevel = false;

            MasterVolume = 0.5;
        }

    }
}
