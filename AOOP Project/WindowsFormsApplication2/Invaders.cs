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
    public partial class Invaders : BaseForm
    {
        private Form prevForm;
        private string currentUser;
        Bitmap area;
        Graphics g;
        private aliens[][] invaders=new aliens[11][];
        int numLeft = 55;
        int lives = 3;
        int score = 0;
        private int x;
        private int y;
        private bool right=true;
        private bool a;
        private Size scale;
        private player person = new player(true);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
        public List<shots> shotsFired = new List<shots>();
        Random thing = new Random(Guid.NewGuid().GetHashCode());

        public struct shots
        {
            public Rectangle shot;
            public bool up;
        }

        public struct aliens
        {
            public aliens(string kind)
            {
                type = kind;
                alive = true;
            }
            public bool alive;
            public string type;
        }

        public struct player
        {
            public player(bool junk)
            {
                Size temp = new Size(25, 12);
                ship = new Bitmap(Image.FromFile("Player.png"),temp);
                x = 0;
                y = 260;
                shotAvailable = true;
            }
            public Bitmap ship;
            public int x;
            public int y;
            public bool shotAvailable;
        }

        public Invaders(Form oldForm,string userIn)
        {
            InitializeComponent();
            prevForm = oldForm;
            area = new Bitmap(panel1.Width, panel1.Height);
            g = Graphics.FromImage(area);

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            currentUser = userIn;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(area, 0, 0);
            //for (int i = 0; i < 11; i++)
            //{
            //    for (int j = 0; j < 5; j++)
            //    {
            //        if (invaders[i][j].alive)
            //        {
            //            Bitmap alien = new Bitmap(Image.FromFile(invaders[i][j].type + (a ? "A" : "B") + ".png"), 15, 10);
            //            e.Graphics.DrawImage(alien, x + i * 20, y + j * 15);
            //        }
            //    }
            //}
            //e.Graphics.DrawImage(person.ship, person.x, person.y);
            //foreach (var c in shotsFired)
            //{
            //    e.Graphics.FillRectangle(c.up ? Brushes.White : Brushes.Red, c.shot);
            //}
        }

        private void updateGraphics()
        {
            g.Clear(Color.Black);
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (invaders[i][j].alive)
                    {
                        Bitmap alien = new Bitmap(Image.FromFile(invaders[i][j].type + (a ? "A" : "B") + ".png"), 15, 10);
                        g.DrawImage(alien, x + i * 20, y + j * 15);
                    }
                }
            }
            g.DrawImage(person.ship, person.x, person.y);
            foreach (var c in shotsFired)
            {
                g.FillRectangle(c.up ? Brushes.White : Brushes.Red, c.shot);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            person.shotAvailable = true;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (person.shotAvailable && GetAsyncKeyState(/*32*/38) != 0)
            {
                Shoot(true,person.x,person.y);
                person.shotAvailable=false;
                timer1.Start();
            }
            if (GetAsyncKeyState(37) != 0)
            {
                if (person.x > 0)
                    person.x -= 5;
            }
            else if(GetAsyncKeyState(39) != 0)
            {
                if(person.x+40<panel1.Width)
                    person.x += 5;
            }
            UpdateShots();
            updateGraphics();
            panel1.Invalidate();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            a = !a;
            MoveAliens();
        }

        private void Shoot(bool fromPlayer,int x,int y)
        {
            if(fromPlayer)
            {
                shots temp=new shots();
                temp.shot=new Rectangle(x+person.ship.Width/2-2,y-10,3,10);
                temp.up = true;
                shotsFired.Add(temp);
                timer1.Start();
            }
            else
            {
                shots temp = new shots();
                temp.shot = new Rectangle(x + 6, y + 15, 3, 10);
                temp.up = false;
                shotsFired.Add(temp);
            }
        }

        private void UpdateShots()
        {
            
            for(int i=0;i<shotsFired.Count;i++)
            {
                bool done = false;
                bool keep = true;
                shots temp = shotsFired[i];
                if(shotsFired[i].up)
                {
                    temp.shot.Y -= 8;
                    if ((temp.shot.Y - y) / 15 >= 0 && (temp.shot.Y - y) / 15 < 5 && (temp.shot.X - x) / 20 >= 0 && (temp.shot.X - x) / 20 < 11 && invaders[(temp.shot.X - x) / 20][(temp.shot.Y - y) / 15].alive)
                    {
                        invaders[(temp.shot.X - x) / 20][(temp.shot.Y - y) / 15].alive = false;
                        keep = false;
                        score+=10*Convert.ToInt32(invaders[(temp.shot.X - x) / 20][(temp.shot.Y - y) / 15].type[5]-'0');
                        label2.Text = Convert.ToString(score);
                        numLeft--;
                        if(numLeft==0)
                        {
                            x = 0;
                            right = true;
                            y = 0;
                            for (int k = 0; k < 11; k++)
                            {
                                for (int j = 4; j >= 0; j--)
                                {
                                    invaders[k][j].alive = true;
                                }
                            }
                            numLeft = 55;
                            timer3.Interval = (int)(.9*timer3.Interval);
                        }
                    }
                }
                else
                {
                    temp.shot.Y += 8;
                    if(temp.shot.Height+temp.shot.Y>260&&temp.shot.X<person.x+person.ship.Width&&temp.shot.X>=person.x)
                    {
                        lives--;
                        label4.Text = Convert.ToString(lives);
                        shotsFired.Clear();
                        done = true;
                        if (lives == 0)
                        {
                            timer3.Stop();
                            timer2.Stop();
                            updateGraphics();
                            panel1.Invalidate();
                            button1.Show();
                            button2.Show();
                            button2.Enabled = true;
                            button1.Enabled = true;
                            button3.Enabled = false;
                            button3.Hide();
                            MessageBox.Show("Game over, you died.");
                            CheckHighScore();
                        }
                    }
                }
                if (!done)
                {
                    if (temp.shot.Y > panel1.Height || temp.shot.Y < 0 || !keep)
                    {
                        shotsFired.RemoveAt(i);
                        i--;
                    }
                    else
                        shotsFired[i] = temp;
                }
            }
        }

        private void MoveAliens()
        {
            int i = 10;
            bool done=false;
            for(;i>0&&!done;i--)
            {
                foreach(var c in invaders[i])
                {
                    if(c.alive)
                    {
                        done = true;
                    }
                }
            }
            if (done)
                i++;

            done = false;
            int j = 0;
            for (; j < 10 && !done; j++)
            {
                foreach (var c in invaders[j])
                {
                    if (c.alive)
                    {
                        done = true;
                    }
                }
            }
            if (done)
                j--;

            if(right&&x+i*20+30<=panel1.Width-10)
            {
                x += 10;
            }
            else if(!right&&x+20*j>=10)
            {
                x -= 10;
            }
            else
            {
                right = !right;
                y += 10;
            }

            bool breakOut = false;
            for(i=0;i<11;i++)
            {
                
                if (breakOut)
                    break;

                for(j=4;j>=0;j--)
                {
                    if (breakOut)
                        break;
                    if(invaders[i][j].alive)
                    {
                        if(y+j*20>panel1.Height)
                        {
                            timer2.Stop();
                            timer3.Stop();
                            updateGraphics();
                            panel1.Invalidate();
                            button1.Show();
                            button2.Show();
                            button2.Enabled = true;
                            button1.Enabled = true;
                            button3.Enabled = false;
                            button3.Hide();
                            MessageBox.Show("Game over, the aliens have invaded!");
                            CheckHighScore();
                            breakOut = true;
                        }
                        if (thing.NextDouble() < (double)2.5/(numLeft+3))
                            Shoot(false, x + i * 20, y + j * 15);
                        break;
                    }
                }
            }
        }

        private void CheckHighScore()
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();
            var queryUser = (from usr in db.pongs
                             where usr.username == currentUser
                             select usr).First();
            if (queryUser.invaders_highscore < Convert.ToInt32(label2.Text))
            {
                MessageBox.Show("Congratulations! You have set a personal high score!");
                queryUser.invaders_highscore = Convert.ToInt32(label2.Text);
                db.SubmitChanges();
            }
        }

        private void Invaders_FormClosed(object sender, FormClosedEventArgs e)
        {
            prevForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 11; i++)
            {
                invaders[i] = new aliens[5];
                for (int j = 0; j < 5; j++)
                {
                    invaders[i][j].alive = true;
                    if (j == 0)
                    {
                        invaders[i][j] = new aliens("Alien3");
                    }
                    else if (j == 1 || j == 2)
                    {
                        invaders[i][j] = new aliens("Alien2");
                    }
                    else
                    {
                        invaders[i][j] = new aliens("Alien1");
                    }
                }
            }
            person.shotAvailable = true;
            label2.Text = "0";
            label4.Text = "3"; 
            lives = 3;
            numLeft = 55;
            score = 0;
            x = 0; 
            y = 0;
            timer3.Interval = 800;
            updateGraphics();
            panel1.Invalidate();
            timer3.Start();
            timer2.Start();
            button1.Hide();
            button2.Hide();
            button2.Enabled = false;
            button1.Enabled = false;
            button3.Enabled = true;
            button3.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();

            var queryUsers = from usr in db.pongs
                             orderby usr.invaders_highscore descending
                             select usr;

            int i = 1;
            string output = "";
            string header = "";
            foreach (var c in queryUsers)
            {
                if (c.username == currentUser)
                {
                    header = "Your high score: " + c.invaders_highscore + "\n" + "Your rank: " + i + "\n\n";
                }
                if (i <= 10)
                {
                    output += i + ": " + c.username + " scored " + c.invaders_highscore + "\n";
                }
                i++;
            }
            MessageBox.Show(header + output);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button1.Show();
            button2.Show();
            button2.Enabled = true;
            button1.Enabled = true;
            timer2.Stop();
            timer3.Stop();
            MessageBox.Show("You have surrendered! The aliens win!");
            CheckHighScore();
            button3.Enabled = false;
            button3.Hide();

        }
    }
}
