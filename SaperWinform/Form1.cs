using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaperWinform
{
    public partial class Form1 : Form
    {
        Board board;
        public Button counter_button;
        bool is_maximized = false;
        int difficulty = -1;
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            counter_button = new Button();
            this.Controls.Add(counter_button);

            board = new Board(this, GamePanel, counter_button);
            is_maximized = true;

            counter_button.BringToFront();
            counter_button.Name = "bomb_counter_button";
            counter_button.Size = new Size(75, 23);
            counter_button.Location = new Point(this.Width - counter_button.Width - 20, 1);
            counter_button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            counter_button.Text = board.UpdateBombDisplay(0);

 
        }
        private void easyDifficluty(object sender, EventArgs e)
        {
            difficulty = 0;
            board.GameStart(difficulty);
            counter_button.Text = board.UpdateBombDisplay(board.bombs_display);
        }

        private void mediumDifficulty(object sender, EventArgs e)
        {
            difficulty = 1;
            board.GameStart(difficulty);
            counter_button.Text = board.UpdateBombDisplay(board.bombs_display);
        }

        private void hardDifficulty(object sender, EventArgs e)
        {
            difficulty = 2;
            board.GameStart(difficulty);
            counter_button.Text = board.UpdateBombDisplay(board.bombs_display);
        }

        private void WindowSizeChanged(object sender, EventArgs e)
        {
            if (difficulty != -1)
                board.UpdateButtonSizes();
        }

        private void ResetButtonClick(object sender, EventArgs e)
        {
            if(difficulty != -1)
            {
                board.GameStart(difficulty);
                counter_button.Text = board.UpdateBombDisplay(board.bombs_display);
            }
        }
    }
}
