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
    private ProgressBar healthBar1;
    private ProgressBar healthBar2;
private const double MAX_HEALTH = 100;
private double player1Health = MAX_HEALTH;
private double player2Health = MAX_HEALTH;


        private void InitializeHealthBars()
{
    healthBar1 = new ProgressBar(250, 20);
    healthBar1.X = Screen.Left + 120;
    healthBar1.Y = Screen.Top - 50;
    healthBar1.Color = Color.Green;
    healthBar1.BorderColor = Color.Black;
    healthBar1.Width = 200;
    Add(healthBar1);


    healthBar2 = new ProgressBar(250, 20);
    healthBar2.X = Screen.Right - 120;
    healthBar2.Y = Screen.Top - 50;
    healthBar2.Color = Color.Green;
    healthBar2.BorderColor = Color.Black;
    healthBar2.Width = 200;
    Add(healthBar2);
}






public override void Begin()
{
    Gravity = new Vector(0, -1000);
    SetWindowSize(1920, 1200, true);

    InitializePlayers();
    InitializeHealthBars();
    LuoKentta();
    LisaaHitbox(50, 50);
    LisaaNappaimet();
    InitalizeHitboxes();

    Camera.Position = new Vector(0, 0);
    Camera.ZoomFactor = 2;
    Camera.StayInLevel = false;

    MasterVolume = 0.5;
}



private void TakeDamage(ref double playerHealth, ProgressBar healthBar, double damage)
{
    playerHealth -= damage;
    if (playerHealth < 0) playerHealth = 0;

    double progressRatio = playerHealth / MAX_HEALTH;
    healthBar.Width = progressRatio * 200;

    UpdateHealthBarColor(healthBar, progressRatio);
}


private void UpdateHealthBarColor(ProgressBar healthBar, double progressRatio)
{
    if (progressRatio <= 0.3)
    {
        healthBar.Color = Color.Red;
    }
    else
    {
        healthBar.Color = Color.Green;
    }
}

        private Image pelaajakuva1 = LoadImage("playerkuva.png");
        private Image pelaajakuva2 = LoadImage("playerkuva2.png");
        private Image tahtiKuva = LoadImage("tahti.png");

        private SoundEffect maaliAani = LoadSoundEffect("maali.wav");


        private void LuoKentta()
        {
            TileMap kentta = TileMap.FromLevelAsset("kentta1.txt");
            kentta.SetTileMethod('#', LisaaTaso);
            kentta.SetTileMethod('N', LisaaPelaaja);
            kentta.SetTileMethod('M', LisaaPelaaja2);
            kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);

            Level.Background.CreateGradient(Color.White, Color.SkyBlue);
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
