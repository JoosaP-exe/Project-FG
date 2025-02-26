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
    private ProgressBar helaBar1;
    private ProgressBar helaBar2;


    public override void Begin()
    {
        Image menukuva = LoadImage("taustakuva.png");
        Level.Background.Image = menukuva;
        SetWindowSize(1920, 1200, true);    
        string[] vaihtoehdot = { "Aloita peli", "Crediitit", "Lopeta" };
        MultiSelectWindow alkuvalikko = new MultiSelectWindow("MENU", vaihtoehdot);
        alkuvalikko.AddItemHandler(0, Alotus);
///        alkuvalikko.AddItemHandler(2, Crediitit);
        alkuvalikko.AddItemHandler(1, Exit);
        Add(alkuvalikko);
    }



/* public static void Crediitit()
{
}*/



    private void Helapaarit()
{
    helaBar1 = new ProgressBar(250, 20);
    helaBar1.X = Screen.Left + 120;
    helaBar1.Y = Screen.Top - 50;
    helaBar1.Color = Color.Green;
    helaBar1.BorderColor = Color.Black;
    helaBar1.Width = 250;
    Add(helaBar1);


    helaBar2 = new ProgressBar(250, 20);
    helaBar2.X = Screen.Right - 120;
    helaBar2.Y = Screen.Top - 50;
    helaBar2.Color = Color.Green;
    helaBar2.BorderColor = Color.Black;
    helaBar2.Width = 250;
    Add(helaBar2);
}




    
public void Alotus()
{
    Gravity = new Vector(0, -1000);
    Helapaarit(); 
    LuoKentta();
    LisaaHitbox(pelaaja1, pelaaja2);
    LisaaNappaimet();
    /// LocationListener(hitbox1, pelaaja1);

    Camera.Position = new Vector(0, 0);
    Camera.ZoomFactor = 2;
    Camera.StayInLevel = false;

    MasterVolume = 0.5;
}



        private Image pelaajakuva1 = LoadImage("playerkuva.png");
        private Image pelaajakuva2 = LoadImage("playerkuva2.png");
        private Image taustakuva = LoadImage("taustakuva.png");
        private SoundEffect maaliAani = LoadSoundEffect("maali.wav");


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
            PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus+10);
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
            Keyboard.Listen(Key.Z, ButtonState.Pressed, Boostaa, "Pelaaja BOOST!!", pelaaja1, HYPPYNOPEUS);
            Keyboard.Listen(Key.F, ButtonState.Pressed, Attacks, "Pelaaja lyö", pelaaja1);

            
            Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja2, -NOPEUS);
            Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja2, NOPEUS);
            Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja2, HYPPYNOPEUS);
            Keyboard.Listen(Key.Down, ButtonState.Pressed, Hyppaa, "Pelaaja ALAS!", pelaaja2, -HYPPYNOPEUS);
            

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


/*private MultiSelectWindow pausevalikko;

private void Pausetus()
{
    if (IsPaused)
    {
        Remove(pausevalikko);
    }
    else
    {
        pausevalikko = new MultiSelectWindow("Pause", "Aloita alusta", "Lopeta");
        pausevalikko.Closed += (handler) => Pausetus();
        pausevalikko.AddItemHandler(0, AloitaAlusta);
        pausevalikko.AddItemHandler(1, Exit);
        Add(pausevalikko);
    }
    Pause();
}

private void InitializePause()
{
    Keyboard.Listen(Key.Escape, ButtonState.Pressed, Pausetus, "Pysäyttää pelin");
}

private void Pausetus()
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
}*/



    void AloitaAlusta()
{
    ClearAll();
    Helapaarit(); 
    LuoKentta();
    LisaaHitbox(pelaaja1, pelaaja2);
    LisaaNappaimet();
}

    }
}
