using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Snake : BaseForm
    {
        enum Directions { up, down, left, right }
        private Form prevForm;
        private string currentUser;
        Bitmap area;
        Graphics g;
        Directions moving;
        List<body> snake;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
        body food;
        Random thing = new Random(Guid.NewGuid().GetHashCode());

        private struct body
        {
            public int x;
            public int y;
            public bool Same(body obj)
            {
                return (x == obj.x && y == obj.y);
            }
        }

        public Snake(Form previous, string userIn)
        {
            InitializeComponent();
            area = new Bitmap(panel1.Width, panel1.Height);
            g = Graphics.FromImage(area);
            prevForm = previous;
            currentUser = userIn;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(area, 0, 0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (GetAsyncKeyState(38) != 0 && moving != Directions.down)
            {
                moving = Directions.up;
            }
            else if (GetAsyncKeyState(37) != 0 && moving != Directions.right)
            {
                moving = Directions.left;
            }
            else if (GetAsyncKeyState(39) != 0 && moving != Directions.left)
            {
                moving = Directions.right;
            }
            else if (GetAsyncKeyState(40) != 0 && moving != Directions.up)
            {
                moving = Directions.down;
            }

            body temp = snake[0];
            if(moving==Directions.up)
            {
                temp.y-=10;
            }
            else if(moving==Directions.down)
            {
                temp.y+=10;
            }
            else if(moving==Directions.left)
            {
                temp.x-=10;
            }
            else if(moving==Directions.right)
            {
                temp.x+=10;
            }
            if (temp.x >= panel1.Width || temp.x < 0 || temp.y >= panel1.Height || temp.y < 0 || snake.Contains(temp))
            {
                timer1.Stop();
                MessageBox.Show("Game over!");
                CheckHighScore();
                button2.Show();
                button1.Show();
            }
            snake.Insert(0, temp);
            if(!food.Same(temp))
            {
                Rectangle tailRefresh = new Rectangle(snake[snake.Count - 1].x, snake[snake.Count - 1].y, 10, 10);
                snake.RemoveAt(snake.Count - 1);
                Rectangle headRefresh = new Rectangle(snake[0].x, snake[0].y, 10, 10);
                UpdateGraphics();
                panel1.Invalidate(tailRefresh);
                panel1.Invalidate(headRefresh);
            }
            else
            {
                label2.Text = Convert.ToString(Convert.ToInt32(label2.Text)+1);
                food.x = thing.Next(panel1.Width) / 10 * 10;
                food.y = thing.Next(panel1.Height) / 10 * 10;
                Rectangle foodRefresh = new Rectangle(food.x, food.y, 10, 10);
                Rectangle headRefresh = new Rectangle(snake[0].x,snake[0].y,10,10);
                UpdateGraphics();
                panel1.Invalidate(foodRefresh);
                panel1.Invalidate(headRefresh);
            }

            //UpdateGraphics();
            //panel1.Invalidate();
        }

        private void UpdateGraphics()
        {
            g.Clear(Color.Black);
            foreach(var c in snake)
            {
                g.FillRectangle(Brushes.White, c.x, c.y, 10, 10);
            }
            g.FillRectangle(Brushes.Red, food.x, food.y, 10, 10);
        }

        private void Snake_FormClosed(object sender, FormClosedEventArgs e)
        {
            prevForm.Show();
        }

        private void CheckHighScore()
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();
            var queryUser = (from usr in db.pongs
                             where usr.username == currentUser
                             select usr).First();
            if (queryUser.snake_highscore < Convert.ToInt32(label2.Text))
            {
                MessageBox.Show("Congratulations! You have set a personal high score!");
                queryUser.snake_highscore = Convert.ToInt32(label2.Text);
                db.SubmitChanges();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            moving = Directions.right;
            label2.Text = "0";
            snake = new List<body>();
            body temp = new body();
            temp.x = 100;
            temp.y = 100;
            snake.Add(temp);
            food = new body();
            food.x = thing.Next(panel1.Width) / 10 * 10;
            food.y = thing.Next(panel1.Height) / 10 * 10;
            UpdateGraphics();
            panel1.Invalidate();
            timer1.Start();
            button1.Hide();
            button2.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();

            var queryUsers = from usr in db.pongs
                             orderby usr.snake_highscore descending
                             select usr;

            int i = 1;
            string output = "";
            string header = "";
            foreach (var c in queryUsers)
            {
                if (c.username == currentUser)
                {
                    header = "Your high score: " + c.snake_highscore + "\n" + "Your rank: " + i + "\n\n";
                }
                if (i <= 10)
                {
                    output += i + ": " + c.username + " scored " + c.snake_highscore + "\n";
                }
                i++;
            }
            MessageBox.Show(header + output);
        }

    }
}
