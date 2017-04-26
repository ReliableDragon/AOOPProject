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
    public partial class PongForm : BaseForm
    {
        Form prevForm;
        string currentUser;
        private Graphics handle;
        private Rectangle paddle = new Rectangle(0, 125, 15, 75);
        private Rectangle paddle2 = new Rectangle(425, 125, 15, 75);
        private Rectangle area1;
        private Rectangle area2;
        private ballStruct theBall;
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
        linqtoregdbDataContext db = new linqtoregdbDataContext();
        Random thing = new Random(Guid.NewGuid().GetHashCode());
        
        private struct ballStruct
        {
            public RectangleF ball;
            public double speed;
            public double angle;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; //WS_EX_COMPOSITED
                return cp;
            }
        }

        public PongForm(Form inThing, string username)
        {
            prevForm = inThing;
            InitializeComponent();
            handle = panel1.CreateGraphics();

            var queryPong = from usr in db.pongs
                            where usr.username==username
                            select usr;

            if(queryPong.Count()==0)
            {
                pong usr=new pong();
                usr.username = username;
                usr.survival_highscore = 0;
                usr.invaders_highscore = 0;
                usr.snake_highscore = 0;
                usr.pacman_highscore = 0;
                db.pongs.InsertOnSubmit(usr);
                db.SubmitChanges();
            }
            currentUser=username;

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            label3.Show();
            label4.Show();

            theBall.ball = new RectangleF(panel1.Width / 2, panel1.Height / 2, 10, 10);
            theBall.speed = 3;
            theBall.angle = 0;
            
            timer4.Start();
        }
        public void drawStuff()
        {
            handle.Clear(Color.LightGray);
            handle.FillEllipse(Brushes.Blue, theBall.ball);
            handle.FillRectangle(Brushes.Blue, paddle);
        }
        public void drawStuff2()
        {
            handle.Clear(Color.LightGray);
            handle.FillEllipse(Brushes.Blue, theBall.ball);
            handle.FillRectangle(Brushes.Blue, paddle);
            handle.FillRectangle(Brushes.Blue, paddle2);
        }

        private void formClosed(object sender, FormClosedEventArgs e)
        {
            prevForm.Close();
        }

        private void PongForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            paddle.Y = 125;
            drawStuff(); //Draw paddle and ball.
            timer1.Start(); //Please comment your code, Kyle...
            timer.Start();
            timer4.Stop(); //Stop idle game timer.
            theBall.ball = new RectangleF(panel1.Width/2, panel1.Height/2, 10, 10);
            theBall.speed = 3;
            theBall.angle = 0;
            paddle2.Height = 0; //Make the other paddle invisible.
            button1.Hide();
            button2.Hide();
            button3.Hide();
            button5.Hide();
            label1.Show(); //Show score label, counter, and highscore.
            label2.Show();
            label3.Hide();
            label4.Hide();
            button6.Hide();
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            area1 = new Rectangle(10, 0, panel1.Width + 10, panel1.Height + 20);
            area2 = new Rectangle(0, 10, 10, panel1.Height);
            Invalidate(); //Redraw window.
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = Convert.ToString(timer.ElapsedMilliseconds/1000); //Update label 2 with elapsed seconds.
            updatePaddle();
            updateBall();
            //drawStuff();
            panel1.Invalidate(); //Redraw window.
        }

        private void updatePaddle()
        {
            if (GetAsyncKeyState(38) != 0 || GetAsyncKeyState(87) != 0) //If up or w are pressed.
            {
                paddle.Y -= 3;
                if (paddle.Y < 0)
                    paddle.Y = 0;
            }
            else if (GetAsyncKeyState(40) != 0 || GetAsyncKeyState(83) != 0) //If down or s are pressed.
            {
                paddle.Y += 3;
                if (paddle.Y + paddle.Height > panel1.Height)
                    paddle.Y = panel1.Height - paddle.Height;
            }
        }

        private void updatePaddle2()
        {
            if (GetAsyncKeyState(83) != 0) //If s is pressed.
            {
                paddle.Y += 3;
                if (paddle.Y + paddle.Height > panel1.Height)
                    paddle.Y = panel1.Height - paddle.Height;
            }
            else if (GetAsyncKeyState(87) != 0) //If w is pressed.
            {
                paddle.Y -= 3;
                if (paddle.Y < 0)
                    paddle.Y = 0;
            }

            if (GetAsyncKeyState(38) != 0) //If up is pressed.
            {
                paddle2.Y -= 3;
                if (paddle2.Y < 0)
                    paddle2.Y = 0;
            }
            else if (GetAsyncKeyState(40) != 0) //If down is pressed.
            {
                paddle2.Y += 3;
                if (paddle2.Y + paddle2.Height > panel1.Height)
                    paddle2.Y = panel1.Height - paddle2.Height;
            }
        }

        private void updatePaddleCPU()
        {
            if (GetAsyncKeyState(38) != 0 || GetAsyncKeyState(87) != 0) //If up or w are pressed.
            {
                paddle.Y -= 3;
                if (paddle.Y < 0)
                    paddle.Y = 0;
            }
            else if (GetAsyncKeyState(40) != 0 || GetAsyncKeyState(83) != 0) //If down or s are pressed.
            {
                paddle.Y += 3;
                if (paddle.Y + paddle.Height > panel1.Height)
                    paddle.Y = panel1.Height - paddle.Height;
            }

            if (theBall.ball.Y<=paddle2.Y+paddle2.Height&&theBall.angle>Math.PI) //If the ball is below the paddle's lower edge and 
                                                                                 //moving up, move the paddle down.
            {
                paddle2.Y -= 3; 
                if (paddle2.Y < 0) 
                    paddle2.Y = 0;
            }
            else if(theBall.ball.Y<=paddle2.Y&&theBall.angle<Math.PI) //If the ball is below the paddle's upper edge and moving down,
                                                                      //move the paddle down.
            {
                paddle2.Y -= 3;
                if (paddle2.Y < 0)
                    paddle2.Y = 0;
            }
            else if (theBall.ball.Y>=paddle2.Y&&theBall.angle<Math.PI) //Ball above upper edge and moving up, move up.
            {
                paddle2.Y += 3;
                if (paddle2.Y + paddle2.Height > panel1.Height)
                    paddle2.Y = panel1.Height - paddle2.Height;
            }
            else if(theBall.ball.Y>=paddle2.Y+paddle2.Height&&theBall.angle>Math.PI) //Ball above lower edge and moving down, move up.
            {
                paddle2.Y += 3;
                if (paddle2.Y + paddle2.Height > panel1.Height)
                    paddle2.Y = panel1.Height - paddle2.Height;
            }
        }

        private void updateBall()
        {
            theBall.ball.X += (float)(System.Math.Cos(theBall.angle) * theBall.speed); //Adjust ball's position based on it's angle of travel.
            theBall.ball.Y += (float)(System.Math.Sin(theBall.angle) * theBall.speed);
            if (theBall.ball.X >= panel1.Width-theBall.ball.Width) //If the ball is at or past the right wall border.
            {
                theBall.ball.X -= 2 * (theBall.ball.X - (panel1.Width - theBall.ball.Width)); //Move it back from the border.
                if (theBall.angle < Math.PI) //If we're moving up, bounce off the wall to the same angle, but backwards.
                    theBall.angle = Math.PI - theBall.angle;
                else
                    theBall.angle = 3 * Math.PI - theBall.angle; //If we're going down, reflect it similarly.
            }
            else if (theBall.ball.X <= paddle.Width && theBall.ball.X > 0 && theBall.ball.Y < paddle.Y + paddle.Height && theBall.ball.Y > paddle.Y - theBall.ball.Height) 
            {
                theBall.ball.X += 2 * (paddle.Width - theBall.ball.X);
                if (theBall.angle <= Math.PI)
                    theBall.angle = Math.PI - theBall.angle - ((paddle.Y - theBall.ball.Y + theBall.ball.Height / 2) + paddle.Height/2) / paddle.Height * Math.PI * .5;
                else
                    theBall.angle = 3 * Math.PI - theBall.angle - ((paddle.Y - theBall.ball.Y + theBall.ball.Height / 2) + paddle.Height / 2) / paddle.Height * Math.PI * .5;
                if (theBall.angle < Math.PI && theBall.angle > Math.PI * .4)
                    theBall.angle = Math.PI * .4;
                if (theBall.angle > Math.PI && theBall.angle < Math.PI * 1.6)
                    theBall.angle = Math.PI * 1.6;
                theBall.speed += .25;
            }
            else if (theBall.ball.X < 0)
            {
                timer.Reset();
                timer1.Stop();
                button1.Show();
                button2.Show();
                button3.Show();
                button5.Show();
                button6.Show();

                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button5.Enabled = true;
                button6.Enabled = true;
                MessageBox.Show("Game over.");
                CheckHighScore(); 
                label3.Show();
                label4.Show();
                label3.Text = "0";
                label4.Text = "0";
                theBall.ball.X = 200;
                paddle2.Height = 75;
                timer4.Start();

            }
            if (theBall.ball.Y >= panel1.Height - theBall.ball.Height)
            {
                theBall.ball.Y -= 2 * (theBall.ball.Y - (panel1.Height - theBall.ball.Height));
                theBall.angle = 2 * Math.PI - theBall.angle;
            }
            else if (theBall.ball.Y < 0)
            {
                theBall.ball.Y += 2 * (0 - theBall.ball.Y);
                theBall.angle = 2 * Math.PI - theBall.angle;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label2.Text = Convert.ToString(timer.ElapsedMilliseconds / 1000);
            updatePaddle2();
            updateBall2();
            //drawStuff2();
            panel1.Invalidate();
        }

        private void updateBall2()
        {
            theBall.ball.X += (float)(System.Math.Cos(theBall.angle) * theBall.speed);
            theBall.ball.Y += (float)(System.Math.Sin(theBall.angle) * theBall.speed);
            if (theBall.ball.X <= panel1.Width - theBall.ball.Width && theBall.ball.X >= panel1.Width - theBall.ball.Width - paddle2.Width && theBall.ball.Y < paddle2.Y + paddle2.Height && theBall.ball.Y > paddle2.Y - theBall.ball.Height)
            {
                theBall.ball.X -= 2 * (theBall.ball.X - (panel1.Width - theBall.ball.Width - paddle2.Width));
                if (theBall.angle < Math.PI)
                    theBall.angle = Math.PI - theBall.angle + ((paddle2.Y - theBall.ball.Y + theBall.ball.Height / 2) + paddle2.Height / 2) / paddle2.Height * Math.PI * .25;
                else
                    theBall.angle = 3 * Math.PI - theBall.angle + ((paddle2.Y - theBall.ball.Y + theBall.ball.Height / 2) + paddle2.Height / 2) / paddle2.Height * Math.PI * .25;

                if (theBall.angle < Math.PI * .6)
                    theBall.angle = Math.PI * .6;
                if (theBall.angle > Math.PI * 1.4)
                    theBall.angle = Math.PI * 1.4;

                theBall.speed += .1;
            }
            else if (theBall.ball.X > panel1.Width - theBall.ball.Width)
            {
                label3.Text=Convert.ToString(Convert.ToInt32(label3.Text)+1);
                paddle.Y = 125;
                paddle2.Y = 125;
                if (Convert.ToInt32(label3.Text) == 5)
                {
                    button4_Click(button4, EventArgs.Empty);
                    MessageBox.Show("Player 1 wins!");
                }
                theBall.ball = new RectangleF(panel1.Width / 2, panel1.Height / 2, 10, 10);
                theBall.speed = 3;
                theBall.angle = 0;
                drawStuff2();
            }
            else if (theBall.ball.X <= paddle.Width && theBall.ball.X > 0 && theBall.ball.Y < paddle.Y + paddle.Height && theBall.ball.Y > paddle.Y - theBall.ball.Height)
            {
                theBall.ball.X += 2 * (paddle.Width - theBall.ball.X);
                if (theBall.angle <= Math.PI)
                    theBall.angle = Math.PI - theBall.angle - ((paddle.Y - theBall.ball.Y + theBall.ball.Height / 2) + paddle.Height / 2) / paddle.Height * Math.PI * .25;
                else
                    theBall.angle = 3 * Math.PI - theBall.angle - ((paddle.Y - theBall.ball.Y + theBall.ball.Height / 2) + paddle.Height / 2) / paddle.Height * Math.PI * .25;
                if (theBall.angle < Math.PI && theBall.angle > Math.PI * .4)
                    theBall.angle = Math.PI * .4;
                if (theBall.angle > Math.PI && theBall.angle < Math.PI * 1.6)
                    theBall.angle = Math.PI * 1.6;
                theBall.speed += .1;
            }
            else if (theBall.ball.X < 0)
            {
                label4.Text = Convert.ToString(Convert.ToInt32(label4.Text) + 1);
                paddle.Y = 125;
                paddle2.Y = 125;
                if (Convert.ToInt32(label4.Text) == 5)
                {
                    button4_Click(button4, EventArgs.Empty);
                    MessageBox.Show("Player 2 wins!");
                }
                theBall.ball = new RectangleF(panel1.Width / 2, panel1.Height / 2, 10, 10);
                theBall.speed = 3;
                theBall.angle = 0;
                drawStuff2();
            }
            if (theBall.ball.Y >= panel1.Height - theBall.ball.Height)
            {
                theBall.ball.Y -= 2 * (theBall.ball.Y - (panel1.Height - theBall.ball.Height));
                theBall.angle = 2 * Math.PI - theBall.angle;
            }
            else if (theBall.ball.Y < 0)
            {
                theBall.ball.Y += 2 * (0 - theBall.ball.Y);
                theBall.angle = 2 * Math.PI - theBall.angle;
            }
        }

        private void updateBallCPU()
        {
            theBall.ball.X += (float)(System.Math.Cos(theBall.angle) * theBall.speed);
            theBall.ball.Y += (float)(System.Math.Sin(theBall.angle) * theBall.speed);
            if (theBall.ball.X <= panel1.Width - theBall.ball.Width && theBall.ball.X >= panel1.Width - theBall.ball.Width - paddle2.Width && theBall.ball.Y < paddle2.Y + paddle2.Height && theBall.ball.Y > paddle2.Y - theBall.ball.Height)
            {
                theBall.ball.X -= 2 * (theBall.ball.X - (panel1.Width - theBall.ball.Width - paddle2.Width));
                if (theBall.angle < Math.PI)
                    theBall.angle = Math.PI - theBall.angle + ((paddle2.Y - theBall.ball.Y + theBall.ball.Height / 2) + paddle2.Height / 2) / paddle2.Height * Math.PI * .25;
                else
                    theBall.angle = 3 * Math.PI - theBall.angle + ((paddle2.Y - theBall.ball.Y + theBall.ball.Height / 2) + paddle2.Height / 2) / paddle2.Height * Math.PI * .25;

                if (theBall.angle < Math.PI * .6)
                    theBall.angle = Math.PI * .6;
                if (theBall.angle > Math.PI * 1.4)
                    theBall.angle = Math.PI * 1.4;

                theBall.speed += .2;
            }
            else if (theBall.ball.X > panel1.Width - theBall.ball.Width)
            {
                label3.Text = Convert.ToString(Convert.ToInt32(label3.Text) + 1);
                paddle.Y = 125;
                paddle2.Y = 125;
                if (Convert.ToInt32(label3.Text) == 5)
                {
                    button4_Click(button4, EventArgs.Empty);
                    MessageBox.Show("You win!");
                }
                theBall.ball = new RectangleF(panel1.Width / 2, panel1.Height / 2, 10, 10);
                theBall.speed = 3;
                theBall.angle = 0;
                drawStuff2();
            }
            else if (theBall.ball.X <= paddle.Width && theBall.ball.X > 0 && theBall.ball.Y < paddle.Y + paddle.Height && theBall.ball.Y > paddle.Y - theBall.ball.Height)
            {
                theBall.ball.X += 2 * (paddle.Width - theBall.ball.X);
                if (theBall.angle <= Math.PI)
                    theBall.angle = Math.PI - theBall.angle - ((paddle.Y - theBall.ball.Y + theBall.ball.Height / 2) + paddle.Height / 2) / paddle.Height * Math.PI * .25;
                else
                    theBall.angle = 3 * Math.PI - theBall.angle - ((paddle.Y - theBall.ball.Y + theBall.ball.Height / 2) + paddle.Height / 2) / paddle.Height * Math.PI * .25;
                if (theBall.angle < Math.PI && theBall.angle > Math.PI * .4)
                    theBall.angle = Math.PI * .4;
                if (theBall.angle > Math.PI && theBall.angle < Math.PI * 1.6)
                    theBall.angle = Math.PI * 1.6;
                theBall.speed += .2;
            }
            else if (theBall.ball.X < 0)
            {
                label4.Text = Convert.ToString(Convert.ToInt32(label4.Text) + 1);
                if (Convert.ToInt32(label4.Text)==5)
                {
                    button4_Click(button4,EventArgs.Empty);
                    MessageBox.Show("The computer wins!");
                }
                paddle.Y = 125;
                paddle2.Y = 125;
                theBall.ball = new RectangleF(panel1.Width / 2, panel1.Height / 2, 10, 10);
                theBall.speed = 3;
                theBall.angle = 0;
                drawStuff2();
            }
            if (theBall.ball.Y >= panel1.Height - theBall.ball.Height)
            {
                theBall.ball.Y -= 2 * (theBall.ball.Y - (panel1.Height - theBall.ball.Height));
                theBall.angle = 2 * Math.PI - theBall.angle;
            }
            else if (theBall.ball.Y < 0)
            {
                theBall.ball.Y += 2 * (0 - theBall.ball.Y);
                theBall.angle = 2 * Math.PI - theBall.angle;
            }
        }

        private void updateBallIdle()
        {
            theBall.ball.X += (float)(System.Math.Cos(theBall.angle) * theBall.speed);
            theBall.ball.Y += (float)(System.Math.Sin(theBall.angle) * theBall.speed);
            if (theBall.ball.X <= panel1.Width - theBall.ball.Width && theBall.ball.X >= panel1.Width - theBall.ball.Width - paddle2.Width && theBall.ball.Y < paddle2.Y + paddle2.Height && theBall.ball.Y > paddle2.Y - theBall.ball.Height)
            {
                theBall.ball.X -= 2 * (theBall.ball.X - (panel1.Width - theBall.ball.Width - paddle2.Width));
                if (theBall.angle < Math.PI)
                    theBall.angle = Math.PI - theBall.angle + ((paddle2.Y - theBall.ball.Y + theBall.ball.Height / 2) + paddle2.Height / 2) / paddle2.Height * Math.PI * .25;
                else
                    theBall.angle = 3 * Math.PI - theBall.angle + ((paddle2.Y - theBall.ball.Y + theBall.ball.Height / 2) + paddle2.Height / 2) / paddle2.Height * Math.PI * .25;

                if (theBall.angle < Math.PI * .6)
                    theBall.angle = Math.PI * .6;
                if (theBall.angle > Math.PI * 1.4)
                    theBall.angle = Math.PI * 1.4;
                
                theBall.angle+=thing.NextDouble()-.5;

                theBall.speed += .2;
            }
            else if (theBall.ball.X > panel1.Width - theBall.ball.Width)
            {
                label3.Text = Convert.ToString(Convert.ToInt32(label3.Text) + 1);
                paddle.Y = 125;
                paddle2.Y = 125;
                theBall.ball = new RectangleF(panel1.Width / 2, panel1.Height / 2, 10, 10);
                theBall.speed = 3;
                theBall.angle = 0;
                drawStuff2();
            }
            else if (theBall.ball.X <= paddle.Width && theBall.ball.X > 0 && theBall.ball.Y < paddle.Y + paddle.Height && theBall.ball.Y > paddle.Y - theBall.ball.Height)
            {
                theBall.ball.X += 2 * (paddle.Width - theBall.ball.X);
                if (theBall.angle <= Math.PI)
                    theBall.angle = Math.PI - theBall.angle - ((paddle.Y - theBall.ball.Y + theBall.ball.Height / 2) + paddle.Height / 2) / paddle.Height * Math.PI * .25;
                else
                    theBall.angle = 3 * Math.PI - theBall.angle - ((paddle.Y - theBall.ball.Y + theBall.ball.Height / 2) + paddle.Height / 2) / paddle.Height * Math.PI * .25;
                if (theBall.angle < Math.PI && theBall.angle > Math.PI * .4)
                    theBall.angle = Math.PI * .4;
                if (theBall.angle > Math.PI && theBall.angle < Math.PI * 1.6)
                    theBall.angle = Math.PI * 1.6;

                theBall.angle += thing.NextDouble() - .5;

                theBall.speed += .2;
            }
            else if (theBall.ball.X < 0)
            {
                label4.Text = Convert.ToString(Convert.ToInt32(label4.Text) + 1);
                paddle.Y = 125;
                paddle2.Y = 125;
                theBall.ball = new RectangleF(panel1.Width / 2, panel1.Height / 2, 10, 10);
                theBall.speed = 3;
                theBall.angle = 0;
                drawStuff2();
            }
            if (theBall.ball.Y >= panel1.Height - theBall.ball.Height)
            {
                theBall.ball.Y -= 2 * (theBall.ball.Y - (panel1.Height - theBall.ball.Height));
                theBall.angle = 2 * Math.PI - theBall.angle;
            }
            else if (theBall.ball.Y < 0)
            {
                theBall.ball.Y += 2 * (0 - theBall.ball.Y);
                theBall.angle = 2 * Math.PI - theBall.angle;
            }
        }

        private void updatePaddleIdle()
        {
            if (theBall.ball.Y <= paddle.Y + paddle.Height && theBall.angle > Math.PI)
            {
                paddle.Y -= 3;
                if (paddle.Y < 0)
                    paddle.Y = 0;
            }
            else if (theBall.ball.Y <= paddle.Y && theBall.angle <= Math.PI)
            {
                paddle.Y -= 3;
                if (paddle.Y < 0)
                    paddle.Y = 0;
            }
            else if (theBall.ball.Y >= paddle.Y && theBall.angle <= Math.PI)
            {
                paddle.Y += 3;
                if (paddle.Y + paddle.Height > panel1.Height)
                    paddle.Y = panel1.Height - paddle.Height;
            }
            else if (theBall.ball.Y >= paddle.Y + paddle.Height && theBall.angle > Math.PI)
            {
                paddle.Y += 3;
                if (paddle.Y + paddle.Height > panel1.Height)
                    paddle.Y = panel1.Height - paddle.Height;
            }

            if (theBall.ball.Y <= paddle2.Y + paddle2.Height && theBall.angle > Math.PI)
            {
                paddle2.Y -= 3;
                if (paddle2.Y < 0)
                    paddle2.Y = 0;
            }
            else if (theBall.ball.Y <= paddle2.Y && theBall.angle < Math.PI)
            {
                paddle2.Y -= 3;
                if (paddle2.Y < 0)
                    paddle2.Y = 0;
            }
            else if (theBall.ball.Y >= paddle2.Y && theBall.angle < Math.PI)
            {
                paddle2.Y += 3;
                if (paddle2.Y + paddle2.Height > panel1.Height)
                    paddle2.Y = panel1.Height - paddle2.Height;
            }
            else if (theBall.ball.Y >= paddle2.Y + paddle2.Height && theBall.angle > Math.PI)
            {
                paddle2.Y += 3;
                if (paddle2.Y + paddle2.Height > panel1.Height)
                    paddle2.Y = panel1.Height - paddle2.Height;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            paddle.Y = 125;
            paddle2.Y = 125;
            paddle2.Height = 75;
            timer4.Stop();
            label3.Text = "0";
            label4.Text = "0";
            drawStuff2();
            timer2.Start();
            theBall.ball = new RectangleF(panel1.Width / 2, panel1.Height / 2, 10, 10);
            theBall.speed = 3;
            theBall.angle = 0;
            button1.Hide();
            button2.Hide();
            button3.Hide();
            label1.Hide();
            label2.Hide();
            label3.Show();
            label4.Show();
            button4.Show();
            button5.Hide();
            button6.Hide();
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            area1 = new Rectangle(10, 0, panel1.Width, panel1.Height + 20);
            area2 = new Rectangle(0, 10, panel1.Width + 20, panel1.Height);
            Invalidate();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            updatePaddleCPU();
            updateBallCPU();
            //drawStuff2();
            panel1.Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            paddle.Y = 125;
            paddle2.Y = 125;
            paddle2.Height = 75;
            timer4.Stop();
            label3.Text = "0";
            label4.Text = "0";
            drawStuff2();
            timer3.Start();
            theBall.ball = new RectangleF(panel1.Width / 2, panel1.Height / 2, 10, 10);
            theBall.speed = 3;
            theBall.angle = 0;
            button1.Hide();
            button2.Hide();
            button3.Hide();
            label1.Hide();
            label2.Hide();
            label3.Show();
            label4.Show();
            button4.Show();
            button5.Hide();
            button6.Hide();
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            area1 = new Rectangle(10, 0, panel1.Width, panel1.Height + 20);
            area2 = new Rectangle(0, 10, panel1.Width + 20, panel1.Height);
            Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timer2.Stop();
            timer3.Stop();
            label1.Hide();
            label2.Hide();
            label3.Show();
            label4.Show();
            button4.Hide();
            button3.Show();
            button2.Show();
            button1.Show();
            button5.Show();
            button6.Show();
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            label3.Text = "0";
            label4.Text = "0";
            theBall.ball.X = 200;
            timer4.Start();
        }

        private void CheckHighScore()
        {
            var queryUser=(from usr in db.pongs
                          where usr.username==currentUser
                          select usr).First();
            if(queryUser.survival_highscore<Convert.ToInt32(label2.Text))
            {
                MessageBox.Show("Congratulations! You have set a personal high score!");
                queryUser.survival_highscore=Convert.ToInt32(label2.Text);
                db.SubmitChanges();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.Clear(Color.LightGray);
            e.Graphics.FillEllipse(Brushes.YellowGreen, theBall.ball);
            e.Graphics.FillRectangle(Brushes.Blue, paddle);
            e.Graphics.FillRectangle(Brushes.Blue, paddle2);
        }

        private void PongForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Blue, area1);
            e.Graphics.FillRectangle(Brushes.Salmon, area2);
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var queryUsers = from usr in db.pongs
                             orderby usr.survival_highscore descending
                             select usr;

            int i=1;
            string output = "";
            string header = "";
            foreach(var c in queryUsers)
            {
                if(c.username==currentUser)
                {
                    header = "Your high score: " + c.survival_highscore+"\n" + "Your rank: " + i+"\n\n";
                }
                if(i<=10)
                {
                    output += i + ": " + c.username + " scored " + c.survival_highscore + "\n";
                }
                i++;
            }
            MessageBox.Show(header + output);
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            updatePaddleIdle();
            updateBallIdle();
            if (theBall.angle < 0)
                theBall.angle += 2 * Math.PI;
            if (theBall.angle > 2 * Math.PI)
                theBall.angle -= 2 * Math.PI;
            panel1.Invalidate();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Invaders newForm = new Invaders(this,currentUser);
            this.Hide();
            newForm.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Snake nextForm = new Snake(this, currentUser);
            nextForm.Show();
            this.Hide();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Pacman nextForm = new Pacman(this, currentUser);
            nextForm.Show();
            this.Hide();
        }
    }
}
