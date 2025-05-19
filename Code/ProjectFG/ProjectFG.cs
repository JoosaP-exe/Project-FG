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


        public void Alotus()
        {
            Gravity = new Vector(0, -1000);
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
        }


        private Image pelaajakuva1 = LoadImage("playerkuva.png");
        private Image pelaajakuva2 = LoadImage("playerkuva2.png");
        private Image voitto1 = LoadImage("pleijer1voitto.png");
        private Image voitto2 = LoadImage("pleijer2voitto.png");
        private Image aikaloppui = LoadImage("aikaloppui.png");


        private void LuoKentta()
        {
            TileMap kentta = TileMap.FromLevelAsset("kentta1.txt");
            kentta.SetTileMethod('#', LisaaTaso);
            kentta.SetTileMethod('N', LisaaPelaaja);
            kentta.SetTileMethod('M', LisaaPelaaja2);
            kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);

            Level.Background.Image = taustakuva;
        }


        private void LisaaTaso(Vector paikka, double leveys, double korkeus)
        {
            PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus + 10);
            taso.Position = paikka;
            taso.Color = Color.Green;
            Add(taso);
        }


        private void LisaaNappaimet()
        {
            Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");

            Keyboard.Listen(Key.A, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -NOPEUS);
            Keyboard.Listen(Key.D, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, NOPEUS);
            Keyboard.Listen(Key.W, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HYPPYNOPEUS);
            Keyboard.Listen(Key.F, ButtonState.Pressed, Attacks, "Pelaaja lyö", pelaaja1);


            Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja2, -NOPEUS);
            Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja2, NOPEUS);
            Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja2, HYPPYNOPEUS);
            Keyboard.Listen(Key.Down, ButtonState.Pressed, Hyppaa, "Pelaaja ALAS!", pelaaja2, -HYPPYNOPEUS);
            Keyboard.Listen(Key.RightControl, ButtonState.Pressed, Attacks, "Pelaaja lyö", pelaaja2);
            
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, Pausetus, "Pysäyttää pelin");


            ControllerOne.Listen(Button.DPadLeft, ButtonState.Down, Liikuta, "Pelaaja liikkuu vasemmalle", pelaaja1, -NOPEUS);
            ControllerOne.Listen(Button.DPadRight, ButtonState.Down, Liikuta, "Pelaaja liikkuu oikealle", pelaaja1, NOPEUS);
            ControllerOne.Listen(Button.A, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HYPPYNOPEUS);

            PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        }


        private void Liikuta(PlatformCharacter hahmo, double nopeus)
        {
            hahmo.Walk(nopeus);
        }


        private void Hyppaa(PlatformCharacter hahmo, double nopeus)
        {
            hahmo.Jump(nopeus);
        }


        private void Boostaa(PlatformCharacter hahmo, double nopeus)
        {
            hahmo.Velocity = new Vector(750, 750);
        }


        private MultiSelectWindow pausevalikko;


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


        private void SuljeValikko()
        {
            if (IsPaused)
            {
                Pause();
            }
            Remove(pausevalikko);
            pausevalikko = null;
        }


        private void JatkaPelia()
        {
            if (IsPaused)
            {
                Pause();
            }
            Remove(pausevalikko);
            pausevalikko = null;
        }


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
