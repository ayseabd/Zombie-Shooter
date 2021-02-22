using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using System.Media;


namespace Zombie_Shooter
{
    public partial class gameScreen : Form
    {
        
        bool goLeft, goRight, goDown, goUp;
        bool gameOver;
        int score;
        string facing = "up";
        int playerHealth = 100;
        int speed = 10;
        int ammo = 10;
        int zombieSpeed = 3;
        int vaccineCounter = 0;
        Random randNum = new Random();
        private Timer GameTimer = new Timer();
        

        //keep zombies in scene
        List<PictureBox> zombiesList = new List<PictureBox>();

        public gameScreen()
        {
            InitializeComponent();
            RestartGame();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void gameScreen_Load(object sender, EventArgs e)
        {
            GameTimer.Interval = 30;
            GameTimer.Tick += new EventHandler(MainTimerEvent);
            GameTimer.Start();
            
            
        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            
            //To prevent the health bar from going below 0
            if (playerHealth > 1)
            {
                healthBar.Value = playerHealth;
            }
            else
            {
                gameOver = true;
                player.Image = Properties.Resources.dead;
                GameTimer.Stop();
                MessageBox.Show("You died! Your Score is: " + score, "Game Over!");
                this.Close();
            }

            txtAmmo.Text = "Ammo: " + ammo;
            txtScore.Text = "Kills: " + score;

            if (goLeft == true && player.Left > 0)
            {
                player.Left -= speed;
            }

            if (goRight == true && player.Left + player.Width < this.ClientSize.Width)
            {
                player.Left += speed;
            }

            if (goUp == true && player.Top > 46)
            {
                player.Top -= speed;
            }

            if (goDown == true && player.Top + player.Height < this.ClientSize.Height)
            {
                player.Top += speed;
            }

          
            // check if we can collect the ammo
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "ammo")
                {
                    // if the player bounds intersect with extra bounds  
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                         SoundPlayer collectAmmo = new SoundPlayer(Zombie_Shooter.Properties.Resources.reload);
                         collectAmmo.Play();                
                         this.Controls.Remove(x);

                        //to make sure it's gone from the scene
                        ((PictureBox)x).Dispose();

                        ammo += 5;
                    }
                }

                // collect vaccine
                foreach (Control v in this.Controls)
                {
                    if (v is PictureBox && (string)v.Tag == "vaccine")
                    {
                        //if the player bounds intersect with extra bounds
                        if (player.Bounds.IntersectsWith(v.Bounds))
                        {
                         
                            SoundPlayer collectVaccine = new SoundPlayer(Zombie_Shooter.Properties.Resources.heal);
                            collectVaccine.Play();
                           
                            this.Controls.Remove(v);

                            // to make sure it's gone from the scene
                           
                            ((PictureBox)v).Dispose();

                            playerHealth += 10;
                        }
                    }
                }

                // for the zombies to follow the player
                if (x is PictureBox && (string)x.Tag == "zombie")
                {
                    //when player hits the zombie, reduce the player health
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        playerHealth--;
                        if (playerHealth < 50 && vaccineCounter == 0)
                        {
                            DropVaccine();
                        }
                    }

                    // If the zombie is to the right of the player
                    if (x.Left > player.Left)
                    {
                        x.Left -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zleft;
                    }

                    // If the zombie is to the left of the player
                    if (x.Left < player.Left)
                    {
                        x.Left += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zright;
                    }

                    // If the zombie is below of the player
                    if (x.Top < player.Top)
                    {
                        x.Top += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zdown;
                    }

                    // If the zombie is above of the player
                    if (x.Top > player.Top)
                    {
                        x.Top -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zup;
                    }
                }

                // zombies and bullets are being spawned dynamically, to chechk whether those contents collide.
                foreach (Control j in this.Controls)
                {
                    if (j is PictureBox && (string)j.Tag == "bullet" && x is PictureBox && (string)x.Tag == "zombie")
                    {
                        if (x.Bounds.IntersectsWith(j.Bounds))
                        {
                            score++;

                            // dispose both the bullet and zombie
                            this.Controls.Remove(j);
                            ((PictureBox)j).Dispose();
                            this.Controls.Remove(x);
                            ((PictureBox)x).Dispose();
                            zombiesList.Remove(((PictureBox)x));

                            SpawnZombies();
                        }
                    }
                }
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {

            if (gameOver == true)
            {
                return;
            }


            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
                facing = "left";
                player.Image = Properties.Resources.left;
            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
                facing = "right";
                player.Image = Properties.Resources.right;
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = true;
                facing = "up";
                player.Image = Properties.Resources.up;
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = true;
                facing = "down";
                player.Image = Properties.Resources.down;
            }
        }


        // if any of these keys are released stop moving
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;

            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = false;

            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = false;

            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = false;

            }

            if (e.KeyCode == Keys.Space && ammo > 0 && gameOver == false)
            {
                ammo--;
                ShootBullet(facing);

                if (ammo < 1)
                {
                    DropAmmo();
                }
            }

            if (e.KeyCode == Keys.Space && ammo < 1 && gameOver == false)
            {
                    SoundPlayer emptySound = new SoundPlayer(Zombie_Shooter.Properties.Resources.empty);
                    emptySound.Play();              
            }

            if (e.KeyCode == Keys.Enter && gameOver == true)
            {
                RestartGame();
            }

        }

        private void ShootBullet(string direction)
        {
            Bullet shootBullet = new Bullet();
            shootBullet.direction = direction;
            SoundPlayer bulletSound = new SoundPlayer(Zombie_Shooter.Properties.Resources.gunshot);
            bulletSound.Play();

            // we want the bullet to originate from middle of the player from the left part
            shootBullet.bulletLeft = player.Left + (player.Width / 2);
            shootBullet.bulletTop = player.Top + (player.Height / 2);

            // bullet does take a form as an argument. So, it needs a form as an argument
            //this script is attached to this form
            shootBullet.MakeBullet(this);
        }


        // dynamically creates zombies coming from outside
       
        private void SpawnZombies()
        {     
            PictureBox zombie = new PictureBox();           
            zombie.Tag = "zombie";
            zombie.Image = Properties.Resources.zdown;
            zombie.BackColor = Color.Transparent;

            // to make sure like the zombies are not being spawned into the same location as before
            zombie.Left = randNum.Next(0, 946);
            zombie.Top = randNum.Next(0, 700);

            // sizing up with the size of the picturees
            zombie.SizeMode = PictureBoxSizeMode.AutoSize;

            zombiesList.Add(zombie);
            this.Controls.Add(zombie);
            player.BringToFront();
        }

        private void DropAmmo()
        {
            PictureBox ammo = new PictureBox();
            ammo.Image = Properties.Resources.ammo_Image;
            ammo.SizeMode = PictureBoxSizeMode.AutoSize;

            //stays inside of the form
            ammo.Left = randNum.Next(10, this.ClientSize.Width - ammo.Width);
            //to make sure doesn't spawn  on top of the labels we gave 60
            ammo.Top = randNum.Next(60, this.ClientSize.Height - ammo.Height);

            ammo.Tag = "ammo";
            this.Controls.Add(ammo);

            ammo.BringToFront();
            player.BringToFront();

        }

        private void DropVaccine()
        {
            PictureBox vaccine = new PictureBox();
            vaccine.Image = Properties.Resources.vaccine;
            vaccine.SizeMode = PictureBoxSizeMode.AutoSize;           

            //stays inside of the form
            vaccine.Left = randNum.Next(10, this.ClientSize.Width - vaccine.Width);
            //to make sure doesn't spawn  on top of the labels we gave 60
            vaccine.Top = randNum.Next(60, this.ClientSize.Height - vaccine.Height);

            vaccine.Tag = "vaccine";
            this.Controls.Add(vaccine);

            vaccine.BringToFront();
            player.BringToFront();
            vaccineCounter++;

        }



        private void RestartGame()
        {
            player.Image = Properties.Resources.up;

            foreach (PictureBox i in zombiesList)
            {
                this.Controls.Remove(i);
            }

            zombiesList.Clear();

           for (int i = 0; i < 3; i++)
           {
               SpawnZombies();
           }

            goUp = false;
            goDown = false;
            goLeft = false;
            goRight = false;
            gameOver = false;

            playerHealth = 100;
            score = 0;
            ammo = 10;
            vaccineCounter = 0;

            GameTimer.Start();
        }
    }
}
