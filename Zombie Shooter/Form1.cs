using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace Zombie_Shooter
{
    public partial class Form1 : Form

    {
        public Form1()
        {
            InitializeComponent();

        }

        private void startGame(object sender, EventArgs e)
        {
            //below the game screen form is being linked to this form
            gameScreen ingamescreen = new gameScreen();

            //since they are linked now we can show them from an event
            ingamescreen.Show();
         
        }

        private void showInstructions(object sender, EventArgs e)
        {
            //below the help screen form is being linked to this form
            howToPlayScreen inhowtoplayscreen = new howToPlayScreen();

            //since they are linked now we can show them from an event
            inhowtoplayscreen.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
