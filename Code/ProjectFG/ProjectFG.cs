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
    /// @author gr301857
    /// @version 15.11.2024
    /// <summary>
    /// 
    /// </summary>
    public partial class ProjectFG : PhysicsGame
    {
        private Image _taustaKuva = LoadImage("main-menu.png");
        private Image _pelaajaKuva1 = LoadImage("playerkuva.png");
        private Image _pelaajaKuva2 = LoadImage("playerkuva2.png");
        private Image _voitto1 = LoadImage("pleijer1voitto.png");
        private Image _voitto2 = LoadImage("pleijer2voitto.png");
        private Image _aikaLoppui = LoadImage("aikaloppui.png");
        private SoundEffect _gunShot = LoadSoundEffect("peakgun.wav");
        private Image[] _bAmpuminen = LoadImages("1ampuuframe1.png", "1ampuuframe2.png", "1ampuuframe3.png", "1ampuuframe4.png");
        private Image[] _rAmpuminen = LoadImages("2ampuuframe1.png", "2ampuuframe2.png", "2ampuuframe3.png", "2ampuuframe4.png");
        private Image[] _bAveleminen = LoadImages("kavelee1.png", "kavelee2.png", "kavelee3.png", "kavelee4.png", "kavelee5.png", "kavelee6.png");

        private MultiSelectWindow _pauseValikko;


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


        public void Alotus()
        {
            Gravity = new Vector(0, -1000);

            _pelaaja1Hp = MaxHp;
            _pelaaja2Hp = MaxHp;

            LuoKentta();
            LisaaNappaimet();
            HpBar();
            Ajastin(MaxAika);

            Camera.Position = new Vector(0, 0);
            Camera.ZoomFactor = 2;
            Camera.StayInLevel = false;

            MediaPlayer.Volume = 0.3;
            MediaPlayer.Play("combat");

            MasterVolume = 0.5;

            AddCollisionHandler(_pelaaja1, "barrier", Barrieri);
            AddCollisionHandler(_pelaaja2, "barrier", Barrieri);
        }


        private void LuoKentta()
        {
            TileMap kentta = TileMap.FromLevelAsset("kentta1.txt");
            kentta.SetTileMethod('#', LisaaTaso);
            kentta.SetTileMethod('N', LisaaPelaaja);
            kentta.SetTileMethod('M', LisaaPelaaja2);
            kentta.SetTileMethod('-', LisaaBarrier);
            kentta.Execute(RuudunKoko, RuudunKoko);

            Level.Background.Image = _taustaKuva;
        }


        private void LisaaTaso(Vector paikka, double leveys, double korkeus)
        {
            PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus + 10);
            taso.Position = paikka;
            taso.Color = Color.DarkBlue;
            Add(taso);
        }


        private void LisaaBarrier(Vector paikka, double leveys, double korkeus)
        {
            PhysicsObject barrier = PhysicsObject.CreateStaticObject(leveys, korkeus);
            barrier.Position = paikka;
            barrier.Color = Color.Transparent;
            barrier.Tag = "barrier";
            barrier.IgnoresCollisionResponse = true;
            Add(barrier);
        }


        private void LisaaNappaimet()
        {
            Keyboard.Listen(Key.A, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", _pelaaja1, -Nopeus);
            Keyboard.Listen(Key.D, ButtonState.Down, Liikuta, "Liikkuu oikealle", _pelaaja1, Nopeus);
            Keyboard.Listen(Key.W, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", _pelaaja1, HyppyNopeus);
            Keyboard.Listen(Key.F, ButtonState.Pressed, Lyominen, "Pelaaja lyö", _pelaaja1);


            Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", _pelaaja2, -Nopeus);
            Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu oikealle", _pelaaja2, Nopeus);
            Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", _pelaaja2, HyppyNopeus);
            Keyboard.Listen(Key.Down, ButtonState.Pressed, Hyppaa, "Pelaaja ALAS!", _pelaaja2, -HyppyNopeus);
            Keyboard.Listen(Key.RightControl, ButtonState.Pressed, Lyominen, "Pelaaja lyö", _pelaaja2);

            Keyboard.Listen(Key.Escape, ButtonState.Pressed, Pausetus, "Pysäyttää pelin");
        }


        private void Liikuta(PlatformCharacter hahmo, double nopeus)
        {
            hahmo.Walk(nopeus);
            _pelaaja1.AnimWalk = new Animation(_bAveleminen);
        }


        private void Hyppaa(PlatformCharacter hahmo, double nopeus)
        {
            hahmo.Jump(nopeus);
        }


        private void Pausetus()
        {
            if (IsPaused)
            {
                Remove(_pauseValikko);
                _pauseValikko = null;
                Pause();
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
                Pause();
            }
        }


        private void SuljeValikko()
        {
            if (IsPaused)
            {
                Pause();
            }

            Remove(_pauseValikko);
            _pauseValikko = null;
        }


        private void JatkaPelia()
        {
            if (IsPaused)
            {
                Pause();
            }

            Remove(_pauseValikko);
            _pauseValikko = null;
        }


        private void Pauseeminen()
        {
            if (IsPaused)
            {
                Remove(_pauseValikko);
            }
            else
            {
                Add(_pauseValikko);
            }

            Pause();
        }


        private void AloitaAlusta()
        {
            ClearAll();
            LuoKentta();

            _pelaaja1Hp = MaxHp;
            _pelaaja2Hp = MaxHp;

            HpBar();
            LisaaNappaimet();
            Ajastin(MaxAika);

            Camera.Position = new Vector(0, 0);
            Camera.ZoomFactor = 2;
            Camera.StayInLevel = false;

            MasterVolume = 0.5;
        }


    }
}
