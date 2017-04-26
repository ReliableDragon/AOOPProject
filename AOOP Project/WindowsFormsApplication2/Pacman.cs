using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Pacman : BaseForm
    {
        Form prevForm;
        string currentUser;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
        enum Directions { up, left, down, right }
        char[][] map = new char[31][];
        char[][] maze = new char[31][];
        char[][] mazeArchive = new char[31][];
        person pacman;
        person orange;
        person red;
        person blue;
        person pink;
        Bitmap area;
        Graphics g;
        int chase;
        int retreat;
        int scared;
        int dotsLeft;
        int score;
        int lives;
        bool open = false;
        Random thing = new Random(Guid.NewGuid().GetHashCode());
        int nextKill;
        int scatters;
        int level;

        Bitmap PowerPellet = new Bitmap("PowerPellet.png");
        Bitmap LowerLeft = new Bitmap("LowerLeft.png");
        Bitmap GhostBar = new Bitmap("GhostBar.png");
        Bitmap LowerRight = new Bitmap("LowerRight.png");
        Bitmap Pellet = new Bitmap("Pellet.png");
        Bitmap Vertical = new Bitmap("Vertical.png");
        Bitmap UpperLeft = new Bitmap("UpperLeft.png");
        Bitmap Horizontal = new Bitmap("Horizontal.png");
        Bitmap UpperRight = new Bitmap("UpperRight.png");
        Bitmap Blank = new Bitmap("Blank.png");
        Bitmap PacOpenR = new Bitmap("PacMan_Open.png");
        Bitmap PacOpenU = new Bitmap("PacMan_Open_U.png");
        Bitmap PacOpenD = new Bitmap("PacMan_Open_D.png");
        Bitmap PacOpenL = new Bitmap("PacMan_Open_L.png");
        Bitmap PacClosed = new Bitmap("PacMan_Closed.png");
        Bitmap RedGhost = new Bitmap("Red.png");
        Bitmap PinkGhost = new Bitmap("Pink.png");
        Bitmap BlueGhost = new Bitmap("Blue.png");
        Bitmap OrangeGhost = new Bitmap("Orange.png");
        Bitmap Eyes = new Bitmap("Eyes.png");
        Bitmap Scared = new Bitmap("Scared.png");
        Bitmap[] Bonus = new Bitmap[8];
        int[] BonusScores = new int[8];

        struct person
        {
            public int x;
            public int y;
            public Directions moving;
            public bool pacman;
            public bool alive;
            public bool scared;
        }

        public Pacman(Form previous, string username)
        {
            currentUser = username;
            prevForm = previous;
            Bonus[0] = new Bitmap("Cherry.png");
            Bonus[1] = new Bitmap("Strawberry.png");
            Bonus[2] = new Bitmap("Peach.png");
            Bonus[3] = new Bitmap("Apple.png");
            Bonus[4] = new Bitmap("Grapes.png");
            Bonus[5] = new Bitmap("Ship.png");
            Bonus[6] = new Bitmap("Bell.png");
            Bonus[7] = new Bitmap("Key.png");
            BonusScores[0] = 100;
            BonusScores[1] = 300;
            BonusScores[2] = 500;
            BonusScores[3] = 700;
            BonusScores[4] = 1000;
            BonusScores[5] = 2000;
            BonusScores[6] = 3000;
            BonusScores[7] = 5000;
            InitializeComponent();
            area = new Bitmap(panel1.Width, panel1.Height);
            g = Graphics.FromImage(area);

            System.IO.StreamReader input = new System.IO.StreamReader("map.in");
            for (int i = 0; i < 31; i++)
            {
                map[i] = new char[28];
                maze[i] = new char[28];
                mazeArchive[i] = new char[28];
                char c = (char)input.Read();
                for (int j = 0; j < 28; j++)
                {
                    map[i][j] = c;
                    maze[i][j] = c;
                    if (c == '5' || c == '0' || c == '2')
                        maze[i][j] = ' ';
                    mazeArchive[i][j] = maze[i][j];
                    c = (char)input.Read();
                }
                input.Read();
            }
            input.Close();
            pacman.x = 14;
            pacman.y = 23;
            pacman.moving = Directions.left;
            pacman.pacman = true;
            red.x = 14;
            red.y = 11;
            red.moving = Directions.up;
            red.pacman = false;
            orange.x = 16;
            orange.y = 14;
            orange.moving = Directions.up;
            orange.pacman = false;
            blue.x = 11;
            blue.y = 14;
            blue.moving = Directions.up;
            blue.pacman = false;
            pink.x = 14;
            pink.y = 14;
            pink.moving = Directions.up;
            pink.pacman = false;
            pink.alive = true;
            red.alive = true;
            orange.alive = true;
            blue.alive = true;
            pink.scared = false;
            red.scared = false;
            orange.scared = false;
            blue.scared = false;
            UpdateGraphics();
            panel1.Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(area, 0, 0);
        }

        void UpdateGraphics()
        {
            for (int i = 0; i < 31; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    g.DrawImage(ConvertImage(map[i][j]), j * 16, i * 16, 16, 16);
                    
                }
            }
            if (map[17][14] == '4')
                g.DrawImage(Bonus[Math.Min(level - 1, 7)], 14 * 16 - 5, 17 * 16 - 5, 26, 26);
            g.DrawImage(open ? pacman.moving==Directions.up? PacOpenU : pacman.moving==Directions.down?PacOpenD:pacman.moving==Directions.left?PacOpenL:PacOpenR: PacClosed, pacman.x * 16 - 5, pacman.y * 16 - 5, 26, 26);
            //g.FillRectangle(Brushes.Yellow, new Rectangle(pacman.x * 16 - 5, pacman.y * 16 - 5, 26, 26));

            g.DrawImage(red.alive ? red.scared ? Scared : RedGhost : Eyes, red.x * 16 - 5, red.y * 16 - 5, 26, 26);
            g.DrawImage(blue.alive ? blue.scared ? Scared : BlueGhost : Eyes, blue.x * 16 - 5, blue.y * 16 - 5, 26, 26);
            g.DrawImage(pink.alive ? pink.scared ? Scared : PinkGhost : Eyes, pink.x * 16 - 5, pink.y * 16 - 5, 26, 26);
            g.DrawImage(orange.alive ? orange.scared ? Scared : OrangeGhost : Eyes, orange.x * 16 - 5, orange.y * 16 - 5, 26, 26);
            //g.FillRectangle(red.currentColor, new Rectangle(red.x * 16 - 5, red.y * 16 - 5, 26, 26));
            //g.FillRectangle(blue.currentColor, new Rectangle(blue.x * 16 - 5, blue.y * 16 - 5, 26, 26));
            //g.FillRectangle(orange.currentColor, new Rectangle(orange.x * 16 - 5, orange.y * 16 - 5, 26, 26));
            //g.FillRectangle(pink.currentColor, new Rectangle(pink.x * 16 - 5, pink.y * 16 - 5, 26, 26));

        }

        Bitmap ConvertImage(char file)
        {
            Bitmap outImage;
            if(file == '0')
            {
                outImage = PowerPellet;
            }
            else if (file == '1')
            {
                outImage = LowerLeft;
            }
            else if (file == '2')
            {
                outImage = GhostBar;
            }
            else if (file == '3')
            {
                outImage = LowerRight;
            }
            else if (file == '5')
            {
                outImage = Pellet;
            }
            else if (file == '6')
            {
                outImage = Vertical;
            }
            else if (file == '7')
            {
                outImage = UpperLeft;
            }
            else if (file == '8')
            {
                outImage = Horizontal;
            }
            else if (file == '9')
            {
                outImage = UpperRight;
            }
            else
            {
                outImage = Blank;
            }
            return outImage;
        }

        private void MovePerson(ref person temp)
        {
            if (temp.moving == Directions.up && (map[temp.y - 1][temp.x] == '0' || map[temp.y - 1][temp.x] == ' ' || map[temp.y - 1][temp.x] == '5' || map[temp.y - 1][temp.x] == '2'))
            {
                temp.y -= 1;

            }
            else if (temp.moving == Directions.down && (map[temp.y + 1][temp.x] == '0' || map[temp.y + 1][temp.x] == ' ' || map[temp.y + 1][temp.x] == '5'||(!temp.pacman&&map[temp.y + 1][temp.x] == '2')))
            {
                temp.y += 1;
            }
            else if (temp.moving == Directions.left && (temp.x==0|| (map[temp.y][temp.x-1] == '4'||map[temp.y][temp.x - 1] == '0' || map[temp.y][temp.x - 1] == ' ' || map[temp.y][temp.x - 1] == '5')))
            {
                temp.x -= 1;
                if (temp.x == -1)
                    temp.x = 27;
            }
            else if (temp.moving == Directions.right && (temp.x==27|| (map[temp.y][temp.x+1] == '4'||map[temp.y][temp.x + 1] == '0' || map[temp.y][temp.x + 1] == ' ' || map[temp.y][temp.x + 1] == '5')))
            {
                temp.x += 1;
                if (temp.x == 28)
                    temp.x = 0;
            }
            if (temp.pacman && (map[temp.y][temp.x] == '0'))
            {
                score += 50;
                label2.Text = Convert.ToString(score);
                dotsLeft--;
                map[temp.y][temp.x] = ' ';
                label5.Text = Convert.ToString((decimal)scared * timer4.Interval / 1000);
                label5.Show();
                timer2.Stop();
                timer3.Stop();
                timer4.Start();
                scared = 20;
                pink.moving = (Directions)(((int)pink.moving + 2) % 2);
                orange.moving = (Directions)(((int)orange.moving + 2) % 2);
                blue.moving = (Directions)(((int)blue.moving + 2) % 2);
                red.moving = (Directions)(((int)red.moving + 2) % 2);
                nextKill = 1;

                red.scared = true;
                blue.scared = true;
                pink.scared = true;
                orange.scared = true;
            }
            if (temp.pacman && map[temp.y][temp.x] == '5')
            {
                score += 10;
                dotsLeft--;
                label2.Text = Convert.ToString(score);
                map[temp.y][temp.x] = ' ';
            }
            if (temp.pacman && map[temp.y][temp.x] == '4')
            {
                score += BonusScores[Math.Min(level-1,7)];
                label2.Text = Convert.ToString(score);
                map[temp.y][temp.x] = ' ';
            }
            if(dotsLeft==0)
            {
                NextLevel();
            }
            if (dotsLeft == 174 || dotsLeft == 74)
            {
                map[17][14] = '4';
                timer6.Start();
            }
        }

        void NextLevel()
        {
            level++;
            System.IO.StreamReader input = new System.IO.StreamReader("map.in");
            for (int i = 0; i < 31; i++)
            {
                map[i] = new char[28];
                maze[i] = new char[28];
                mazeArchive[i] = new char[28];
                char c = (char)input.Read();
                for (int j = 0; j < 28; j++)
                {
                    map[i][j] = c;
                    maze[i][j] = c;
                    if (c == '5' || c == '0' || c == '2')
                        maze[i][j] = ' ';
                    mazeArchive[i][j] = maze[i][j];
                    c = (char)input.Read();
                }
                input.Read();
            }
            input.Close();
            pacman.x = 14;
            pacman.y = 23;
            pacman.moving = Directions.left;
            pacman.pacman = true;
            red.x = 14;
            red.y = 11;
            red.moving = Directions.up;
            red.pacman = false;
            orange.x = 15;
            orange.y = 14;
            orange.moving = Directions.up;
            orange.pacman = false;
            blue.x = 13;
            blue.y = 14;
            blue.moving = Directions.up;
            blue.pacman = false;
            pink.x = 11;
            pink.y = 14;
            pink.moving = Directions.up;
            pink.pacman = false;
            pink.alive = true;
            red.alive = true;
            orange.alive = true;
            blue.alive = true;
            pink.scared = false;
            red.scared = false;
            orange.scared = false;
            blue.scared = false;
            UpdateGraphics();
            panel1.Invalidate();
            retreat = 20;
            scared = 0;
            chase = 0;
            dotsLeft = 244;
            if(level<=11)
            {
                timer2.Interval -= 10;
                timer3.Interval -= 10;
                timer4.Interval -= 10;
            }
            timer2.Stop();
            timer3.Start();
            timer4.Stop();
            timer5.Start();
            label5.Hide();
            scatters = 1;
        }

        void ResetLevel()
        {
            pacman.x = 14;
            pacman.y = 23;
            pacman.moving = Directions.left;
            pacman.pacman = true;
            red.x = 14;
            red.y = 11;
            red.moving = Directions.up;
            red.pacman = false;
            orange.x = 15;
            orange.y = 14;
            orange.moving = Directions.up;
            orange.pacman = false;
            blue.x = 13;
            blue.y = 14;
            blue.moving = Directions.up;
            blue.pacman = false;
            pink.x = 11;
            pink.y = 14;
            pink.moving = Directions.up;
            pink.pacman = false;
            pink.alive = true;
            red.alive = true;
            orange.alive = true;
            blue.alive = true;
            pink.scared = false;
            red.scared = false;
            orange.scared = false;
            blue.scared = false;
            UpdateGraphics();
            panel1.Invalidate();
            retreat = 20;
            scared = 0;
            chase = 0;
            timer1.Start();
            timer2.Stop();
            timer3.Start();
            timer4.Stop();
            timer5.Start();
            scatters = 1;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            open = !open;
            if (GetAsyncKeyState(38) != 0&&maze[pacman.y-1][pacman.x]==' ')
            {
                pacman.moving = Directions.up;
            }
            else if (GetAsyncKeyState(37) != 0&&(pacman.x==0||maze[pacman.y][pacman.x-1]==' '))
            {
                pacman.moving = Directions.left;
            }
            else if (GetAsyncKeyState(39) != 0 && (pacman.x==27||maze[pacman.y][pacman.x+1] == ' '))
            {
                pacman.moving = Directions.right;
            }
            else if (GetAsyncKeyState(40) != 0 && maze[pacman.y + 1][pacman.x] == ' ' && map[pacman.y + 1][pacman.x] != '2')
            {
                pacman.moving = Directions.down;
            }
            MovePerson(ref pacman);
            if ((pacman.x == blue.x && pacman.y == blue.y && blue.alive&&!blue.scared) || (pacman.x == orange.x && pacman.y == orange.y && orange.alive&&!orange.scared) || (pacman.x == red.x && pacman.y == red.y && red.alive&&!red.scared) || (pacman.x == pink.x && pacman.y == pink.y && pink.alive&&!pink.scared))
            {
                timer1.Stop();
                timer2.Stop();
                timer3.Stop();
                timer4.Stop();
                UpdateGraphics();
                panel1.Invalidate();
                lives--;
                label4.Text = Convert.ToString(lives);
                if(lives==0)
                {
                    MessageBox.Show("game over");
                    CheckHighscore();
                    button1.Show();
                    button2.Show();
                }
                else
                {
                    ResetLevel();
                }
            }
            if (blue.alive&&blue.scared && (pacman.x == blue.x && pacman.y == blue.y))
            {
                blue.alive = false;
                score += (int)Math.Pow(2, nextKill) * 100;
                nextKill++;
                label2.Text = Convert.ToString(score);
            }
            if (orange.alive&&orange.scared && pacman.x == orange.x && pacman.y == orange.y)
            {
                orange.alive = false;
                score += (int)Math.Pow(2, nextKill) * 100;
                nextKill++;
                label2.Text = Convert.ToString(score);
            }
            if (red.alive&&red.scared && pacman.x == red.x && pacman.y == red.y)
            {
                red.alive = false;
                score += (int)Math.Pow(2, nextKill) * 100;
                nextKill++;
                label2.Text = Convert.ToString(score);
            }
            if (pink.alive&&pink.scared && pacman.x == pink.x && pacman.y == pink.y)
            {
                pink.alive = false;
                score += (int)Math.Pow(2, nextKill) * 100;
                nextKill++;
                label2.Text = Convert.ToString(score);
            }
            UpdateGraphics();
            panel1.Invalidate();
        }

        Directions Red()
        {
            return FindMove(red, pacman.x, pacman.y);
        }

        Directions Orange()
        {
            if (Math.Sqrt((pacman.x - orange.x) * (pacman.x - orange.x) + (pacman.y - orange.y * (pacman.y - orange.y))) > 8)
                return FindMove(orange, pacman.x, pacman.y);
            else
                return FindMove(orange, 0, 29);
        }

        Directions Blue()
        {
            int x;
            int y;
            y = pacman.moving == Directions.left || pacman.moving == Directions.right ? pacman.y : pacman.moving == Directions.up ? pacman.y - 2 : pacman.y + 2;
            x = pacman.moving == Directions.up || pacman.moving == Directions.down ? pacman.x : pacman.moving == Directions.left ? pacman.x - 2 : pacman.x + 2;

            y += (y - red.y);
            x += (x - red.x);

            if (x < 0)
                x = 0;
            if (x > 27)
                x = 27;
            if (y < 0)
                y = 1;
            if (y > 30)
                y = 29;
            while (map[y][x] == '-')
                y++;

            return FindMove(blue, x, y);
        }

        Directions Pink()
        {
            int x;
            int y;
            y = pacman.moving == Directions.left || pacman.moving == Directions.right ? pacman.y : pacman.moving == Directions.up ? pacman.y - 4 : pacman.y + 4;
            x = pacman.moving == Directions.up || pacman.moving == Directions.down ? pacman.x : pacman.moving == Directions.left ? pacman.x - 4 : pacman.x + 4;
            
            if (x < 0)
                x = 0;
            if (x > 27)
                x = 27;
            if (y < 0)
                y = 1;
            if (y > 30)
                y = 29;
            while (map[y][x] == '-')
                y++;
            return FindMove(pink, x, y);
        }

        Directions FindMove(person ghost, int x, int y)
        {
            maze[ghost.y][ghost.x] = 'a';
            if (ghost.moving != Directions.down&&maze[ghost.y - 1][ghost.x] == ' '&&!((ghost.y==11||ghost.y==23)&&(ghost.x==12||ghost.x==15)))
                    maze[ghost.y - 1][ghost.x] = 'b';

            if (ghost.moving != Directions.up&&maze[ghost.y + 1][ghost.x] == ' ')
                    maze[ghost.y + 1][ghost.x]='b';

            if (ghost.moving != Directions.right&&maze[ghost.y][ghost.x - 1 >= 0 ? ghost.x - 1 : 27] == ' ')
                    maze[ghost.y][ghost.x - 1 >= 0 ? ghost.x - 1 : 27] ='b';

            if (ghost.moving != Directions.left&&maze[ghost.y][(ghost.x + 1)%28] == ' ')
                maze[ghost.y][(ghost.x + 1) % 28] = 'b';

            return solve(maze, ghost.x,ghost.y, x, y,'b');
        }

        Directions solve(char[][] maze, int startX, int startY, int endX, int endY, char letter)
        {
	        int moves = 0;
	        char next = letter;
            //if(letter == 'z')		//iterates which letter is being entered
            //    next = 'a';
            //else
		        next++;

	        for(int i = 0; i<31;i++)
		        for(int j = 0; j<28;j++)				//goes through every entry in the matrix to find where the previous iteration left off
			        if(maze[i][j] == letter)		//and checks to see if it is possible to move anywhere from each location
			        {
                        if (Up(maze, endX, endY, j, i))
				        {
					        maze[i-1][j] = next;
					        moves++;
                            if (i - 1 == endY && j == endX)
                                return output(maze, endX, endY, startX, startY);
				        }
                        if (Down(maze, endX, endY, j, i))
				        {
					        maze[i+1][j] = next;
					        moves++;
					        if(i+1 == endY && j == endX)
                                return output(maze, endX, endY, startX, startY);				//returns true if it is at the end of the maze
				        }
                        if (Left(maze, endX, endY, j, i))
				        {
                            if (j != 0)
                            {
                                maze[i][j - 1] = next;
                                if (i == endY && j - 1 == endX)
                                    return output(maze, endX, endY, startX, startY);
                            }
                            else
                            {
                                maze[i][27] = next;
                                if (i == endY && 27 == endX)
                                    return output(maze, endX, endY, startX, startY);
                            }
					        moves++;
				        }
                        if (Right(maze, endX, endY, j, i))
				        {
                            if (j != 27)
                            {
                                maze[i][j + 1] = next;
                                if (i == endY && j + 1 == endX)
                                    return output(maze, endX, endY, startX, startY);
                            }
                            else
                            {
                                maze[i][0] = next;
                                if (i == endY && 0 == endX)
                                    return output(maze, endX, endY, startX, startY);
                            }
					        moves++;
				        }
			        }

            if (moves == 0)		//if you could not make any moves and you have not exited because you got to the end,
            {
                for (int i = 0; i < 31; i++)
                {
                    for (int j = 0; j < 28; j++)
                    {
                        maze[i][j] = mazeArchive[i][j];
                    }
                }
                return Directions.up;	//there must be no solution to the maze so return false
            }
            else
                return solve(maze, startX, startY, endX, endY, next);
        }

        bool Up(char[][] maze, int endX, int endY, int startX, int startY)		//checks to see if it is possible to move one space up
        {
	        if(startY == 0)
		        return false;
	        if(maze[startY-1][startX] == ' '||(startX==endX && startY-1==endY))
		        return true;
	        else
		        return false;
        }

        bool Down(char[][] maze, int endX, int endY, int startX, int startY)	//checks to see if it is possible to move one space down
        {
	        if(startY == 30)
		        return false;
            if (maze[startY + 1][startX] == ' ' || (startX == endX && startY + 1 == endY))
                return true;
            else
                return false;
        }

        bool Left(char[][] maze, int endX, int endY, int startX, int startY)	//checks to see if it is possible to move one space to the left
        {
            if (startX == 0)
            {
                if (maze[startY][27] == ' ')
                    return true;
                else
                    return false;
            }
            if (maze[startY][startX - 1] == ' ' || (startX - 1 == endX && startY == endY))
		        return true;
	        else
		        return false;
        }

        bool Right(char[][] maze, int endX, int endY, int startX, int startY)	//checks to see if it is possible to move one space to the right
        {
            if (startX == 27)
            {
                if (maze[startY][0] == ' ')
                    return true;
                else
                    return false;
            }
            if (maze[startY][startX + 1] == ' ' || (startX + 1 == endX && startY == endY))
		        return true;
	        else
		        return false;
        }

        Directions output(char[][] maze, int endX, int endY, int startX, int startY)
        {
	        int x = endY;
	        int y = endX;

	        char letter = maze[x][y];


	        while(Math.Sqrt((x-startY)*(x-startY)+(y-startX)*(y-startX))>1)			//starts at the end and saves where each prior entry was into maze2, which will reconstruct the shortest path
	        {
                //if(letter == 'a')
                //    letter = 'z';
		        //else
			        letter--;

                if (y != 27 && maze[x][y + 1] == letter)
                    y++;
                else if (x != 30 && maze[x + 1][y] == letter)
                    x++;
                else if (x != 0 && maze[x - 1][y] == letter)
                    x--;
                else if (y != 0 && maze[x][y - 1] == letter)
                    y--;
                else if (y == 0 && maze[x][27] == letter)
                    y = 27;
                else if (y == 27 && maze[x][0] == letter)
                    y = 0;
	        }

            for (int i = 0; i < 31; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    maze[i][j] = mazeArchive[i][j];
                }
            }
            if (maze[x][y] != ' ')
                return Directions.up;
            if (x - startY == 1)
                return Directions.down;
            else if (startY - x == 1)
                return Directions.up;
            else if (y - startX == 1)
                return Directions.right;
            else
                return Directions.left;
	
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            chase--;
            Directions nextMove;
            if (red.alive)
            {
                nextMove = Red();
                red.moving = ((int)nextMove + (int)red.moving) % 2 == 0 && nextMove != red.moving ? CheckDirections(red) : nextMove;
                MovePerson(ref red);
            }
            if (pink.alive)
            {
                nextMove = Pink();
                pink.moving = ((int)nextMove + (int)pink.moving) % 2 == 0 && nextMove != pink.moving ? CheckDirections(pink) : nextMove;
                MovePerson(ref pink);
            }
            if (blue.alive)
            {
                nextMove = Blue();
                blue.moving = ((int)nextMove + (int)blue.moving) % 2 == 0 && nextMove != blue.moving ? CheckDirections(blue) : nextMove;
                MovePerson(ref blue);
            }
            if (orange.alive)
            {
                nextMove = Orange();
                orange.moving = ((int)nextMove + (int)orange.moving) % 2 == 0 && nextMove != orange.moving ? CheckDirections(orange) : nextMove;
                MovePerson(ref orange);
            }

            if (chase == 0)
            {
                timer3.Stop();
                scatters++;
                chase = 20;
                timer2.Start(); 
                pink.moving = (Directions)(((int)pink.moving + 2) % 2);
                orange.moving = (Directions)(((int)orange.moving + 2) % 2);
                blue.moving = (Directions)(((int)blue.moving + 2) % 2);
                red.moving = (Directions)(((int)red.moving + 2) % 2);
            }

            if ((pacman.x == blue.x && pacman.y == blue.y && blue.alive) || (pacman.x == orange.x && pacman.y == orange.y && orange.alive) || (pacman.x == red.x && pacman.y == red.y && red.alive) || (pacman.x == pink.x && pacman.y == pink.y && pink.alive))
            {
                timer1.Stop();
                timer2.Stop();
                timer3.Stop();
                timer4.Stop();
                UpdateGraphics();
                panel1.Invalidate();
                lives--;
                label4.Text = Convert.ToString(lives);
                if(lives==0)
                {
                    MessageBox.Show("game over");
                    CheckHighscore();
                    button1.Show();
                    button2.Show();
                }
                else
                {
                    ResetLevel();
                }
            }
        }

        Directions CheckDirections(person ghost)
        {
            if(ghost.moving == Directions.up)
            {
                if(maze[ghost.y-1][ghost.x]==' ')
                {
                    return ghost.moving;
                }
                else
                {
                    if (maze[ghost.y][ghost.x - 1] == ' ')
                        return Directions.left;
                    else
                        return Directions.right;
                }
            }
            else if (ghost.moving == Directions.right)
            {
                if (ghost.x==27||maze[ghost.y][ghost.x+1] == ' ')
                {
                    return ghost.moving;
                }
                else
                {
                    if (maze[ghost.y-1][ghost.x] == ' ')
                        return Directions.up;
                    else
                        return Directions.down;
                }
            }
            else if (ghost.moving == Directions.down)
            {
                if (maze[ghost.y + 1][ghost.x] == ' ')
                {
                    return ghost.moving;
                }
                else
                {
                    if (maze[ghost.y][ghost.x - 1] == ' ')
                        return Directions.left;
                    else
                        return Directions.right;
                }
            }
            else
            {
                if (ghost.x==0||maze[ghost.y][ghost.x-1] == ' ')
                {
                    return ghost.moving;
                }
                else
                {
                    if (maze[ghost.y-1][ghost.x] == ' ')
                        return Directions.up;
                    else
                        return Directions.down;
                }
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            retreat--;
            Directions nextMove;
            if (red.alive)
            {
                nextMove = RedRun();
                red.moving = ((int)nextMove + (int)red.moving) % 2 == 0 && nextMove != red.moving ? CheckDirections(red) : nextMove;
                MovePerson(ref red);
            }
            if (pink.alive)
            {
                nextMove = PinkRun();
                pink.moving = ((int)nextMove + (int)pink.moving) % 2 == 0 && nextMove != pink.moving ? CheckDirections(pink) : nextMove;
                MovePerson(ref pink);
            }
            if (blue.alive)
            {
                nextMove = BlueRun();
                blue.moving = ((int)nextMove + (int)blue.moving) % 2 == 0 && nextMove != blue.moving ? CheckDirections(blue) : nextMove;
                MovePerson(ref blue);
            }
            if (orange.alive)
            {
                nextMove = OrangeRun();
                orange.moving = ((int)nextMove + (int)orange.moving) % 2 == 0 && nextMove != orange.moving ? CheckDirections(orange) : nextMove;
                MovePerson(ref orange);
            }
            
            
            
            
            if(retreat==0)
            {
                timer3.Stop();
                if (scatters < 3)
                    chase = 100;
                else if (scatters < 4)
                    chase = 200;
                else
                    chase = 400;
                timer2.Start();
                pink.moving = (Directions)(((int)pink.moving + 2) % 2);
                orange.moving = (Directions)(((int)orange.moving + 2) % 2);
                blue.moving = (Directions)(((int)blue.moving + 2) % 2);
                red.moving = (Directions)(((int)red.moving + 2) % 2);
            }

            if ((pacman.x == blue.x && pacman.y == blue.y&&blue.alive) || (pacman.x == orange.x && pacman.y == orange.y&&orange.alive) || (pacman.x == red.x && pacman.y == red.y&&red.alive) || (pacman.x == pink.x && pacman.y == pink.y&&pink.alive))
            {
                timer1.Stop();
                timer2.Stop();
                timer3.Stop();
                timer4.Stop();
                UpdateGraphics();
                panel1.Invalidate();
                lives--;
                label4.Text = Convert.ToString(lives);
                if(lives==0)
                {
                    MessageBox.Show("game over");
                    CheckHighscore();
                    button1.Show();
                    button2.Show();
                }
                else
                {
                    ResetLevel();
                }
            }
        }

        Directions RedRun()
        {
            return FindMove(red, 27, 1);
        }

        Directions OrangeRun()
        {
            return FindMove(orange, 0, 29);
        }

        Directions BlueRun()
        {
            return FindMove(blue, 27, 29);
        }

        Directions PinkRun()
        {
            return FindMove(pink, 0, 1);
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            scared--;
            label5.Text = Convert.ToString((decimal)scared * timer4.Interval / 1000);
            if (red.alive)
            {
                red.moving = RandomDirection(red);
                MovePerson(ref red);
            }
            if (pink.alive)
            {
                pink.moving = RandomDirection(pink);
                MovePerson(ref pink);
            }
            if (orange.alive)
            {
                orange.moving = RandomDirection(orange);
                MovePerson(ref orange);
            }
            if (blue.alive)
            {
                blue.moving = RandomDirection(blue);
                MovePerson(ref blue);
            }
            
            
            
            
            
            if (scared == 0)
            {
                label5.Hide();
                timer4.Stop();
                if (chase > 0)
                    timer2.Start();
                else if (retreat > 0)
                    timer3.Start();

                red.scared = false;
                orange.scared = false;
                pink.scared = false;
                blue.scared = false;
            }

            if (blue.alive&&blue.scared&&(pacman.x == blue.x && pacman.y == blue.y))
            {
                blue.alive = false; 
                score += (int)Math.Pow(2, nextKill) * 100;
                nextKill++;
                label2.Text = Convert.ToString(score);
            }
            if (orange.alive&&orange.scared&& pacman.x == orange.x && pacman.y == orange.y) 
            {
                orange.alive=false;
                score += (int)Math.Pow(2, nextKill) * 100;
                nextKill++;
                label2.Text = Convert.ToString(score);
            }
            if (red.alive&&red.scared&&pacman.x == red.x && pacman.y == red.y) 
            {
                red.alive=false;
                score += (int)Math.Pow(2, nextKill) * 100;
                nextKill++;
                label2.Text = Convert.ToString(score);
            }
            if (pink.alive && pink.scared && pacman.x == pink.x && pacman.y == pink.y)
            {
                pink.alive = false;
                score += (int)Math.Pow(2, nextKill) * 100;
                nextKill++;
                label2.Text = Convert.ToString(score);
            }
            //if ((pacman.x == blue.x && pacman.y == blue.y) || (pacman.x == orange.x && pacman.y == orange.y) || (pacman.x == red.x && pacman.y == red.y) || (pacman.x == pink.x && pacman.y == pink.y))
            //{
            //    timer1.Stop();
            //    timer2.Stop();
            //    UpdateGraphics();
            //    panel1.Invalidate();
            //    lives--;if(lives==0)(MessageBox.Show("game over");CheckHighscore();}else{ResetLevel();}
            //}
        }

        Directions RandomDirection(person ghost)
        {
            if(ghost.moving==Directions.up)
            {
                List<Directions> options = new List<Directions>();
                if (maze[ghost.y - 1][ghost.x] == ' ')
                    options.Add(Directions.up);
                if (maze[ghost.y][(ghost.x + 1)%28] == ' ')
                    options.Add(Directions.right);
                if (maze[ghost.y][ghost.x - 1>=0?ghost.x - 1:27] == ' ')
                    options.Add(Directions.left);
                return options[thing.Next(options.Count)];
            }
            else if (ghost.moving == Directions.down)
            {
                List<Directions> options = new List<Directions>();
                if (maze[ghost.y + 1][ghost.x] == ' ')
                    options.Add(Directions.down);
                if (maze[ghost.y][(ghost.x + 1) % 28] == ' ')
                    options.Add(Directions.right);
                if (maze[ghost.y][ghost.x - 1 >= 0 ? ghost.x - 1 : 27] == ' ')
                    options.Add(Directions.left);
                return options[thing.Next(options.Count)];
            }
            else if (ghost.moving == Directions.left)
            {
                List<Directions> options = new List<Directions>();
                if (maze[ghost.y - 1][ghost.x] == ' ')
                    options.Add(Directions.up);
                if (maze[ghost.y + 1][ghost.x] == ' ')
                    options.Add(Directions.down);
                if (maze[ghost.y][ghost.x - 1 >= 0 ? ghost.x - 1 : 27] == ' ')
                    options.Add(Directions.left);
                return options[thing.Next(options.Count)];
            }
            else
            {
                List<Directions> options = new List<Directions>();
                if (maze[ghost.y - 1][ghost.x] == ' ')
                    options.Add(Directions.up);
                if (maze[ghost.y][(ghost.x + 1) % 28] == ' ')
                    options.Add(Directions.right);
                if (maze[ghost.y + 1][ghost.x] == ' ')
                    options.Add(Directions.down);
                return options[thing.Next(options.Count)];
            }
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            bool update = false;
            if(!red.alive)
            {
                update = true;
                red.moving = ReturnToBase(red);
                MovePerson(ref red);
                if (red.x == 14 && red.y == 14)
                {
                    red.alive = true;
                    red.scared = false;
                }
            }
            if (!blue.alive)
            {
                update = true;
                blue.moving = ReturnToBase(blue);
                MovePerson(ref blue);
                if (blue.x == 14 && blue.y == 14)
                {
                    blue.alive = true;
                    blue.scared = false;
                }
            }
            if (!pink.alive)
            {
                update = true;
                pink.moving = ReturnToBase(pink);
                MovePerson(ref pink);
                if (pink.x == 14 && pink.y == 14)
                {
                    pink.alive = true;
                    pink.scared = false;
                }
            }
            if (!orange.alive)
            {
                update = true;
                orange.moving = ReturnToBase(orange);
                MovePerson(ref orange);
                if(orange.x==14&&orange.y==14)
                {
                    orange.alive = true;
                    orange.scared = false;
                }
            }

            if (update)
            {
                UpdateGraphics();
                panel1.Invalidate();
            }
        }

        Directions ReturnToBase(person ghost)
        {
            return FindMove(ghost, 14,14);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.IO.StreamReader input = new System.IO.StreamReader("map.in");
            for (int i = 0; i < 31; i++)
            {
                map[i] = new char[28];
                maze[i] = new char[28];
                mazeArchive[i] = new char[28];
                char c = (char)input.Read();
                for (int j = 0; j < 28; j++)
                {
                    map[i][j] = c;
                    maze[i][j] = c;
                    if (c == '5' || c == '0' || c == '2')
                        maze[i][j] = ' ';
                    mazeArchive[i][j] = maze[i][j];
                    c = (char)input.Read();
                }
                input.Read();
            }
            input.Close();
            pacman.x = 14;
            pacman.y = 23;
            pacman.moving = Directions.left;
            pacman.pacman = true;
            red.x = 14;
            red.y = 11;
            red.moving = Directions.up;
            red.pacman = false;
            orange.x = 16;
            orange.y = 14;
            orange.moving = Directions.up;
            orange.pacman = false;
            blue.x = 11;
            blue.y = 14;
            blue.moving = Directions.up;
            blue.pacman = false;
            pink.x = 14;
            pink.y = 14;
            pink.moving = Directions.up;
            pink.pacman = false;
            pink.alive = true;
            red.alive = true;
            orange.alive = true;
            blue.alive = true;
            pink.scared = false;
            red.scared = false;
            orange.scared = false;
            blue.scared = false;
            UpdateGraphics();
            panel1.Invalidate();
            retreat = 20;
            scared = 0;
            chase = 0;
            dotsLeft = 244;
            score = 0;
            scatters = 1;
            lives = 3;
            label2.Text = "0";
            label4.Text = "3";
            level = 1;
            timer2.Interval = 200;
            timer3.Interval = 200;
            timer4.Interval = 300;
            timer1.Start();
            timer3.Start();
            timer5.Start();
            button1.Hide();
            button2.Hide();
        }

        private void timer6_Tick(object sender, EventArgs e)
        {
            map[17][14] = ' ';
            timer6.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();

            var queryUsers = from usr in db.pongs
                             orderby usr.pacman_highscore descending
                             select usr;

            int i = 1;
            string output = "";
            string header = "";
            foreach (var c in queryUsers)
            {
                if (c.username == currentUser)
                {
                    header = "Your high score: " + c.pacman_highscore + "\n" + "Your rank: " + i + "\n\n";
                }
                if (i <= 10)
                {
                    output += i + ": " + c.username + " scored " + c.pacman_highscore + "\n";
                }
                i++;
            }
            MessageBox.Show(header + output);
        }

        void CheckHighscore()
        {
            linqtoregdbDataContext db = new linqtoregdbDataContext();

            var queryUser = (from usr in db.pongs
                             where usr.username == currentUser
                             select usr).First();
            if (queryUser.pacman_highscore < Convert.ToInt32(label2.Text))
            {
                MessageBox.Show("Congratulations! You have set a personal high score!");
                queryUser.pacman_highscore = Convert.ToInt32(label2.Text);
                db.SubmitChanges();
            }
        }

        private void Pacman_FormClosed(object sender, FormClosedEventArgs e)
        {
            prevForm.Show();
        }
    }
}
