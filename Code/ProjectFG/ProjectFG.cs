using System;
using System.Collections.Generic;
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
    public class ProjectFG : PhysicsGame
    {
        private const double NOPEUS = 200;
        private const double HYPPYNOPEUS = 500;
        private const int RUUDUN_KOKO = 40;


        private PlatformCharacter pelaaja1;
        private PlatformCharacter pelaaja2;
        private Image pelaajakuva1 = LoadImage("playerkuva.png");
        private Image pelaajakuva2 = LoadImage("playerkuva2.png");
        private Image tahtiKuva = LoadImage("tahti.png");

        private SoundEffect maaliAani = LoadSoundEffect("maali.wav");

        public override void Begin()
        {
            Gravity = new Vector(0, -1000);

            LuoKentta();
            LisaaNappaimet();
            Camera.Position = pelaaja1.Position;
            Camera.Position = pelaaja2.Position;


            Camera.ZoomFactor = 1.0;
            Camera.StayInLevel = true;

            MasterVolume = 0.5;
        }

        private void LuoKentta()
        {
            TileMap kentta = TileMap.FromLevelAsset("kentta1.txt");
            kentta.SetTileMethod('#', LisaaTaso);
            kentta.SetTileMethod('*', LisaaTahti);
            kentta.SetTileMethod('N', LisaaPelaaja);
            kentta.SetTileMethod('M', LisaaPelaaja2);
            kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);
            Level.CreateBorders();
            Level.Background.CreateGradient(Color.White, Color.SkyBlue);
        }

        private void LisaaTaso(Vector paikka, double leveys, double korkeus)
        {
            PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
            taso.Position = paikka;
            taso.Color = Color.Green;
            Add(taso);
        }

        private void LisaaTahti(Vector paikka, double leveys, double korkeus)
        {
            PhysicsObject tahti = PhysicsObject.CreateStaticObject(leveys, korkeus);
            tahti.IgnoresCollisionResponse = true;
            tahti.Position = paikka;
            tahti.Image = tahtiKuva;
            tahti.Tag = "tahti";
            Add(tahti);
        }

        private void LisaaPelaaja(Vector paikka, double leveys, double korkeus)
        {
            pelaaja1 = new PlatformCharacter(leveys, korkeus);
            pelaaja1.Position = paikka;
            pelaaja1.Mass = 4.0;
            pelaaja1.Image = pelaajakuva1;
            Add(pelaaja1);
        }
        
        private void LisaaPelaaja2(Vector paikka, double leveys, double korkeus)
        {
            pelaaja2 = new PlatformCharacter(leveys, korkeus);
            pelaaja2.Position = paikka;
            pelaaja2.Mass = 4.0;
            pelaaja2.Image = pelaajakuva2;
            Add(pelaaja2);
        }

        private void LisaaNappaimet()
        {
            Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

            Keyboard.Listen(Key.A, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -NOPEUS);
            Keyboard.Listen(Key.D, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, NOPEUS);
            Keyboard.Listen(Key.W, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HYPPYNOPEUS);

            
            Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja2, -NOPEUS);
            Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja2, NOPEUS);
            Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja2, HYPPYNOPEUS);
            
            ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Poistu pelistä");

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
    }

}
