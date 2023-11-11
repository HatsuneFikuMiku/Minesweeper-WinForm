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
using Microsoft.VisualBasic;

namespace SaperWinform
{
    public class Board
    {
        public int bombs_display;


        private List<List<Button>> tiles = new List<List<Button>>();
        private List<List<bool>> hidden_map = new List<List<bool>>();
        private List<List<int>> tile_values = new List<List<int>>();



        int tiles_for_win;
        int bomb_count;
        int row_count, column_count;

        int tile_size;
        int buttonWidth;
        int buttonHeight;

        int panelWidth;
        int panelHeight;

        int offsetX;
        int offsetY;

        int location_x, location_y;
        bool first_turn = true;


        Random random = new Random();

        //Constructor
        //##########################

        private Control parentControl;
        private Panel panel;
        private Button counter_button;
        public Board(Control parentControl, Panel panel, Button counter_button)
        {
            this.parentControl = parentControl;
            this.panel = panel;
            this.counter_button = counter_button;
        }        

        //generation and board management
        //#############################

        public void GameStart(int difficulty)
        {
            first_turn = true;
            DeleteBoard();
            DifficultyLevel(difficulty);
            GenerateBoard();            
        }
        public string UpdateBombDisplay(int bombs_left)
        {
            return "Bombs: " + bombs_left.ToString();
        }
        private void DifficultyLevel(int level)
        {
            switch (level)
            {
                case 0:
                    row_count = 10;
                    column_count = 10;
                    bomb_count = 10;
                    break;
                case 1:
                    row_count = 16;
                    column_count = 16;
                    bomb_count = 40;
                    break;
                case 2:
                    row_count = 16;
                    column_count = 30;
                    bomb_count = 99;                    
                    break;
            }
            tiles_for_win = row_count * column_count - bomb_count;
            bombs_display = bomb_count;
        }
        private void ButtonAdd(Button button, int x, int y)
        {
            button.Name = x.ToString() + ";" + y.ToString();
            button.Size = new Size(buttonWidth, buttonHeight);
            button.Location = new Point(location_x, location_y);
            button.Text = "";
            button.Font = new Font(button.Font.FontFamily, 12f);

            button.MouseClick += TileLeftClicked;
            button.MouseUp += TileRightClicked;

            parentControl.Controls.Add(button); // Dodaj przycisk do kontrolki
            panel.Controls.Add(button);
        }      
        private void GenerateBoard()
        {
            buttonWidth = panel.Width / column_count;
            buttonHeight = panel.Height / row_count;

            if (buttonWidth > buttonHeight)
            {
                buttonWidth = buttonHeight;
            }
            else
            {
                buttonHeight = buttonWidth;
            }
            tile_size = buttonWidth;

            panelWidth = buttonWidth * column_count;
            panelHeight = buttonHeight * row_count;

            offsetX = (panel.Width - panelWidth) / 2;
            offsetY = (panel.Height - panelHeight) / 2;

            location_y = offsetY;

            for (int i = 0; i < row_count; i++)
            {
                List<Button> button_Row = new List<Button>(); // Nowa lista przycisków w wierszu
                List<int> numbers_Row = new List<int>();
                List<bool> hidden_Row = new List<bool>();

                location_x = offsetX;
                for (int j = 0; j < column_count; j++)
                {
                    Button button = new Button();                    
                    ButtonAdd(button, i, j);
                    button_Row.Add(button); // Dodaj przycisk do wiersza
                    numbers_Row.Add(-2);
                    hidden_Row.Add(false);
                    location_x += buttonWidth;
                }
                location_y += buttonHeight;
                tiles.Add(button_Row); // Dodaj wiersz do listy tiles
                tile_values.Add(numbers_Row);
                hidden_map.Add(hidden_Row);
            }
        }
        private void DeleteBoard()
        {
            foreach (var buttonRow in tiles)
            {
                foreach (var button in buttonRow)
                {
                    button.Dispose();
                }
            }

            tiles.Clear();
            tile_values.Clear();
            hidden_map.Clear();
        }
        public void UpdateButtonSizes()
        {
            buttonWidth = panel.Width / column_count;
            buttonHeight = panel.Height / row_count;

            if (buttonWidth > buttonHeight)
            {
                buttonWidth = buttonHeight;
            }
            else
            {
                buttonHeight = buttonWidth;
            }

            tile_size = buttonWidth;

            panelWidth = buttonWidth * column_count;
            panelHeight = buttonHeight * row_count;

            offsetX = (panel.Width - panelWidth) / 2;
            offsetY = (panel.Height - panelHeight) / 2;

            location_y = offsetY;

            foreach (var buttonRow in tiles)
            {
                location_x = offsetX;
                foreach (var button in buttonRow)
                {
                    button.Size = new Size(buttonWidth, buttonHeight);
                    button.Location = new Point(location_x, location_y);

                    location_x += buttonWidth;
                }
                location_y += buttonHeight;
            }
        }

        //Tile value assignment 
        //##################
        private void BombGeneration(int x, int y)
        {
            while(bomb_count>0)
            {
                int x_position = random.Next(row_count);
                int y_position = random.Next(column_count);

                if(tile_values[x_position][y_position] != -1)
                    bomb_count--;

                tile_values[x_position][y_position] = -1;

                if(bomb_count == 1)
                {
                    for(int i = x-1; i<=x+1; i++)
                    {
                        for (int j = y - 1; j <= y + 1; j++)
                        {
                            try
                            {
                                if (tile_values[i][j] == -1)
                                {
                                    tile_values[i][j] = -2;
                                    bomb_count++;
                                }

                            }
                            catch { }
                        }
                    }
                }
            }
        }
        private void TileValueGeneration()
        {
            for(int i=0; i<row_count; i++)
            {
                for(int j=0; j<column_count; j++)
                {
                    if (tile_values[i][j] != -1)
                    {
                        tile_values[i][j] = HowManyBombs(i, j);
                        TileValueColor(tile_values[i][j], i, j);
                    }
                }
            }
        }
        private int HowManyBombs(int x, int y)
        {
            int tileValue = 0;
            int startX = Math.Max(0, x - 1);
            int endX = Math.Min(row_count - 1, x + 1);
            int startY = Math.Max(0, y - 1);
            int endY = Math.Min(column_count - 1, y + 1);

            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    if (tile_values[i][j] == -1)
                        tileValue++;
                }
            }

            return tileValue;
        }
        private void TileValueColor(int color, int x, int y)
        {
            switch(color)
            {
                case 0:
                    tiles[x][y].ForeColor = Color.Black;
                    break;
                case 1:
                    tiles[x][y].ForeColor = Color.Blue;
                    break;
                case 2:
                    tiles[x][y].ForeColor = Color.Green;
                    break;
                case 3:
                    tiles[x][y].ForeColor = Color.Red;
                    break;
                case 4:
                    tiles[x][y].ForeColor = Color.Purple;
                    break;
                case 5:
                    tiles[x][y].ForeColor = Color.Orange;
                    break;
                case 6:
                    tiles[x][y].ForeColor = Color.Brown;
                    break;
                case 7:
                    tiles[x][y].ForeColor = Color.DarkCyan;
                    break;
                case 8:
                    tiles[x][y].ForeColor = Color.DarkGray;
                    break;
            }
        }        

        //Controls
        //############################
        private void TileLeftClicked(object sender, MouseEventArgs e)
        {
            Button tile = (Button)sender;

            if (e.Button == MouseButtons.Left)
            {
                TileClicked(true, tile);
            }
        }//left mouse button (uncovering tiles)
        private void TileRightClicked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Button tile = (Button)sender;
                TileClicked(false, tile);
            }
        }//right mouse button (placing/removing flags)

        //Game rules
        //############################
        private void TileClicked(bool click, Button button)
        {
            string input = button.Name;
            string[] split = input.Split(';');

            int x = int.Parse(split[0]);
            int y = int.Parse(split[1]);

            if(first_turn)
            {
                BombGeneration(x, y);
                first_turn = false;
                TileValueGeneration();
            }

            if(click == true && hidden_map[x][y]!=true)
            {
                if (tile_values[x][y] == -1)
                {
                    BombDetonated();
                }
                else if (tile_values[x][y] == 0)
                {
                    NullClicked(x, y);
                }
                else
                {
                    tiles[x][y].Text = tile_values[x][y].ToString();
                    TileValueColor(tile_values[x][y], x, y);
                    tiles[x][y].BackColor = Color.LightGray;
                    hidden_map[x][y]=true;
                    tiles_for_win--;
                    
                }         
                CheckVictory(tiles_for_win);
            }
            else if(!click)
            {
                MarkBomb(x, y, button);
            }
        }
        private void MarkBomb(int x, int y, Button button)
        {
            
            if(tiles[x][y].Text == "P")
            {
                tiles[x][y].Text = "";
                hidden_map[x][y] = false;
                bombs_display++;
            }
            else if (!hidden_map[x][y] && bombs_display>0)
            {
                button.Font = new Font(button.Font.FontFamily, 12f);
                button.ForeColor = Color.Red;
                tiles[x][y].Text = "P";
                hidden_map[x][y] = true;
                bombs_display--;
            }
            counter_button.Text = UpdateBombDisplay(bombs_display);
        }
        private void CheckVictory(int x)
        {
            if(x == 0)
            {
                MessageBox.Show("Victory!");
                //DeactivateBoard();
            }

        }
        private void BombDetonated()
        {
            foreach (var buttonRow in tiles)
            {
                foreach (var button in buttonRow)
                {
                    string input = button.Name;
                    string[] split = input.Split(';');

                    int x = int.Parse(split[0]);
                    int y = int.Parse(split[1]);

                    if (tile_values[x][y] == -1 && button.Text!="P")
                    {
                        tiles[x][y].Text = "*";
                        tiles[x][y].Font = new Font(tiles[x][y].Font.FontFamily, 15f, FontStyle.Bold);
                        button.ForeColor = Color.Black;
                        button.BackColor = Color.PaleVioletRed;
                        hidden_map[x][y] = true;
                    }
                    else
                    {
                        TileValueColor(tile_values[x][y], x, y);
                        hidden_map[x][y] = true;
                    }
                }
            }
            MessageBox.Show("You lost!");
            //DeactivateBoard();
        }
        private void NullAlgorithm(int x, int y)
        {
            if (!hidden_map[x][y] || tiles[x][y].Text == "P")//tile yet to be discovered, or claimed by a ill placed flag
            {
                if(tiles[x][y].Text == "P")
                {
                    bombs_display++;
                    counter_button.Text = UpdateBombDisplay(bombs_display);
                }

                if (tile_values[x][y] == 0)//if the value of neighbour is 0, you have to do it recursivly until done
                {
                    tiles[x][y].Text = "";
                    hidden_map[x][y] = true;
                    tiles[x][y].BackColor = Color.LightGray;
                    tiles_for_win--;
                    NullClicked(x, y);


                }
                else if ((tile_values[x][y]) != -1)//skip bombs
                {
                    tiles[x][y].Text = tile_values[x][y].ToString();
                    TileValueColor(tile_values[x][y], x, y);
                    tiles[x][y].BackColor = Color.LightGray;
                    hidden_map[x][y] = true;
                    tiles_for_win--;
                }
            }

        }
        private void NullClicked(int x, int y)
        {
            int startX = Math.Max(0, x - 1);
            int endX = Math.Min(row_count - 1, x + 1);
            int startY = Math.Max(0, y - 1);
            int endY = Math.Min(column_count - 1, y + 1);

            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    NullAlgorithm(i, j);
                }
            }
        }
    }
}
