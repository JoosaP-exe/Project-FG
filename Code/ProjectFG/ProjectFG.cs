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

    private Image taustakuva = LoadImage("main-menu.png");
        /// <summary>
        /// Luodaan alkuvalikko, jossa voidaan valita pelin aloitus tai poistuminen. Laittaa myös taustan ja musat soimaan.
        /// </summary>
        public override void Begin()
        {
            Image menukuva = LoadImage("main-menu.png");
            Level.Background.Image = menukuva;
            string[] vaihtoehdot = { "PELAA", "POISTU" };
            MultiSelectWindow alkuvalikko = new MultiSelectWindow("PÄÄVALIKKO", vaihtoehdot);
            alkuvalikko.AddItemHandler(0, Alotus);
            alkuvalikko.AddItemHandler(1, Exit);
            Add(alkuvalikko);
            MediaPlayer.Play("mainmenu");
        }

        /// <summary>
        /// Pelin alkaessa säätää kameran, luo kentän, lisää taistelumusat ja näppaimet.
        /// </summary>
        public void Alotus()
        {
            Gravity = new Vector(0, -1000);
            player1Health = MAX_HEALTH;
            player2Health = MAX_HEALTH;
            LuoKentta();
            LisaaNappaimet();
            hpbar();
            Timer(MaxAika);
            /// LocationListener(hitbox1, pelaaja1);
            Camera.Position = new Vector(0, 0);
            Camera.ZoomFactor = 2;
            Camera.StayInLevel = false;
            MediaPlayer.Volume = 0.3;
            MediaPlayer.Play("combat");

            MasterVolume = 0.5;
            AddCollisionHandler(pelaaja1, "barrier", Barrieri);
            AddCollisionHandler(pelaaja2, "barrier", Barrieri);
        }


        private Image pelaajakuva1 = LoadImage("playerkuva.png");
        private Image pelaajakuva2 = LoadImage("playerkuva2.png");
        private Image voitto1 = LoadImage("pleijer1voitto.png");
        private Image voitto2 = LoadImage("pleijer2voitto.png");
        private Image aikaloppui = LoadImage("aikaloppui.png");
        private SoundEffect gunshot = LoadSoundEffect("peakgun.wav");
        private Image[] bampuminen = LoadImages("1ampuuframe1.png", "1ampuuframe2.png", "1ampuuframe3.png", "1ampuuframe4.png");
        private Image[] rampuminen = LoadImages("2ampuuframe1.png", "2ampuuframe2.png", "2ampuuframe3.png", "2ampuuframe4.png");
        private Image[] baveleminen = LoadImages("kavelee1.png", "kavelee2.png", "kavelee3.png", "kavelee4.png", "kavelee5.png", "kavelee6.png");

        /// <summary>
        /// Värkkää kentta1.txt tiedoston mukaan kentän, jossa on pelaajat ja taso, jota voi muokata jos jaksaa.
        /// </summary>
        private void LuoKentta()
        {
            TileMap kentta = TileMap.FromLevelAsset("kentta1.txt");
            kentta.SetTileMethod('#', LisaaTaso);
            kentta.SetTileMethod('N', LisaaPelaaja);
            kentta.SetTileMethod('M', LisaaPelaaja2);
            kentta.SetTileMethod('-', LisaaBarrier);
            kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);

            Level.Background.Image = taustakuva;
        }

        /// <summary>
        /// Tekee tasosta physicsobjektin ja muuttaa sen värin.
        /// </summary>
        private void LisaaTaso(Vector paikka, double leveys, double korkeus)
        {
            PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus + 10);
            taso.Position = paikka;
            taso.Color = Color.DarkBlue;
            Add(taso);
        }

        /// <summary>
        /// Hökälöi kaikki pelaajien näppäimet ja hommat.
        /// </summary>
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

            Keyboard.Listen(Key.A, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -NOPEUS);
            Keyboard.Listen(Key.D, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, NOPEUS);
            Keyboard.Listen(Key.W, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HYPPYNOPEUS);
            Keyboard.Listen(Key.F, ButtonState.Pressed, Lyominen, "Pelaaja lyö", pelaaja1);


            Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja2, -NOPEUS);
            Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja2, NOPEUS);
            Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja2, HYPPYNOPEUS);
            Keyboard.Listen(Key.Down, ButtonState.Pressed, Hyppaa, "Pelaaja ALAS!", pelaaja2, -HYPPYNOPEUS);
            Keyboard.Listen(Key.RightControl, ButtonState.Pressed, Lyominen, "Pelaaja lyö", pelaaja2);

            Keyboard.Listen(Key.Escape, ButtonState.Pressed, Pausetus, "Pysäyttää pelin");
        }

        /// <summary>
        /// Tekee pelaajan kävelyn animaatiot.
        /// </summary>
        private void Liikuta(PlatformCharacter hahmo, double nopeus)
        {
            hahmo.Walk(nopeus);
            pelaaja1.AnimWalk = new Animation(baveleminen);
        }

        /// <summary>
        /// Saa pelaajan hyppäämään.
        /// </summary>
        private void Hyppaa(PlatformCharacter hahmo, double nopeus)
        {
            hahmo.Jump(nopeus);
        }


        private MultiSelectWindow pausevalikko;

        /// <summary>
        /// Hökälöi pausevalikon esciä painettaessa, jossa on vaihtoehdot jatkaa peliä tai aloittaa alusta.
        /// </summary>
        private void Pausetus()
        {
            if (IsPaused)
            {
                Remove(pausevalikko);
                pausevalikko = null;
                Pause();
            }
            else
            {
                if (pausevalikko == null)
                {
                    pausevalikko = new MultiSelectWindow("TAUKO", "ALOITA ALUSTA", "POISTU");
                    pausevalikko.Closed += (handler) => SuljeValikko();
                    pausevalikko.AddItemHandler(0, AloitaAlusta);
                    pausevalikko.AddItemHandler(1, Exit);
                }
                Add(pausevalikko);
                Pause();
            }
        }

        /// <summary>
        /// Sulkee pausevalikon.
        /// </summary>
        private void SuljeValikko()
        {
            if (IsPaused)
            {
                Pause();
            }
            Remove(pausevalikko);
            pausevalikko = null;
        }

        /// <summary>
        /// Jatkaa peliä kun painetaan esciä.
        /// </summary>
        private void JatkaPelia()
        {
            if (IsPaused)
            {
                Pause();
            }
            Remove(pausevalikko);
            pausevalikko = null;
        }

        /// <summary>
        /// Tsiigaa onko pausevalikko auki vai ei.
        /// </summary>
        private void pauseeminen()
        {
            if (IsPaused)
            {
                Remove(pausevalikko);
            }
            else
            {
                Add(pausevalikko);
            }
            Pause();
        }

        /// <summary>
        /// Tekee kaikki hommat kun peli alkaa alusta.
        /// </summary>
        void AloitaAlusta()
        {
            ClearAll();
            LuoKentta();
            player1Health = MAX_HEALTH;
            player2Health = MAX_HEALTH;
            hpbar();
            LisaaNappaimet();
            Update(Time);
            Timer(MaxAika);

            Camera.Position = new Vector(0, 0);
            Camera.ZoomFactor = 2;
            Camera.StayInLevel = false;

            MasterVolume = 0.5;
        }


    }
}
