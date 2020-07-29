using Xamarin.Forms;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;


namespace FLIPS
{
    public partial class FLIPSPage : ContentPage
    {
        static int[,] board = new int[3, 4];
        static int[] initArray = new int[] {1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6};
        static int flips = 0;
        static int sec = 100;
        static Stack<string> maStack = new Stack<string>();
        static int turn = 1;
        static bool timerOn = false;
        static int flippedX = -1;
        static int flippedY = -1;
        static Stopwatch sw = new Stopwatch();

        public FLIPSPage()
        {
            InitializeComponent();
            initGame();

            /*
            // test to see if shuffled right
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    test.Text += board[i, j];
                }
            }
            */
        }


        void initGame()
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    Image myImage = this.FindByName<Image>("c" + i.ToString() + j.ToString());
                    myImage.Source = "back.jpg";
                }
            }
            Array.Clear(board, 0, board.Length);
            sec =100;
            updateSeconds();
            flips = 0;
            updateFlips();
            turn = 1;
            timerOn = false;
            flippedX = -1;
            flippedY = -1;
            maStack.Clear();
            shuffle();
        }


        void shuffle()
        {
            Random random = new Random();
            for (int i = 0; i < initArray.Length; i++)
            {
                int tmp = initArray[i];
                int r = random.Next(i, initArray.Length);
                initArray[i] = initArray[r];
                initArray[r] = tmp;
            }
            int initArrayIndex = 0;
            for (int i = 0; i < board.GetLength(0);i++)
            {
                for (int j = 0; j < board.GetLength(1);j++)
                {
                    board[i, j] = initArray[initArrayIndex];
                    initArrayIndex++;
                }
            }
        }

        // pass 0 to num if chaning to back.jpg
        async Task changeImage(int x, int y, int num)
        {
            
            Image myImage = this.FindByName<Image>("c" + x.ToString() + y.ToString());
            if (num ==0)
            {
                //myImage.Source = "back.jpg";
                /*
                myImage.TranslateTo(100, 0, 350);
                await myImage.RotateYTo(-90, 150);
                myImage.RotationY = -270;
                myImage.Source = "back.jpg";
                myImage.RotateYTo(-360, 150);
                await myImage.TranslateTo(0, 0, 170);
                myImage.RotationY = 0;
                */

                myImage.TranslateTo(100, 0, 400);
                await myImage.RotateYTo(-90, 200);
                myImage.RotationY = -270;
                myImage.Source = "back.jpg";
                myImage.RotateYTo(-360, 200);
                await myImage.TranslateTo(0, 0, 220);
                myImage.RotationY = 0;

            }
            else
            {
                //myImage.Source = num.ToString() + ".jpg";
                myImage.TranslateTo(100, 0, 320);
                await myImage.RotateYTo(-90, 120);
                myImage.RotationY = -270;
                myImage.Source = num.ToString()+".jpg";
                myImage.RotateYTo(-360, 120);
                await myImage.TranslateTo(0, 0, 140);
                myImage.RotationY = 0;

            }
        }


        void startTimer()
        {
            if (timerOn == false)
            {
                timerOn = true;
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    Device.BeginInvokeOnMainThread(updateSeconds);
                    if (sec < 1)
                    {
                        timerOn = false;
                        DisplayAlert("Time Over", "Cards Left: " + (board.Length - maStack.Count + (turn+1) % 2).ToString(), "Retry");

                        initGame();
                        return false;
                    }
                    else if (timerOn == false)
                    {
                        return false;
                    }
                    else
                    {
                        return true;    
                    }
                });
            }
        }

        void updateSeconds()
        {
            timeText.Text = sec--.ToString();
        }

        void disableButtons()
        {
            for (int i = 0; i < board.GetLength(0);i++)
            {
                for (int j = 0; j < board.GetLength(1);j++)
                {
                    Button myButton = this.FindByName<Button>("b" + i.ToString() + j.ToString());
                    myButton.IsEnabled = false;
                }

            }
        }
        void enableButtons()
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    Button myButton = this.FindByName<Button>("b" + i.ToString() + j.ToString());
                    myButton.IsEnabled = true;
                }

            }
        }

        void updateFlips()
        {
            flipText.Text = flips.ToString();
        }

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            string boardIndex = (sender as Button).Text;


            if (maStack.Contains(boardIndex) == false)
            {
                disableButtons();
                startTimer();
                int x = (int)Char.GetNumericValue(boardIndex[0]);
                int y = (int)Char.GetNumericValue(boardIndex[1]);
                int cardNum = board[x, y];
                await changeImage(x, y, cardNum);

                if (turn == 1)
                {
                    maStack.Push(boardIndex);
                    flippedX = x;
                    flippedY = y;
                    flips++;
                    updateFlips();
                    turn++;
                }
                else
                {
                    int flippedNum = board[flippedX, flippedY];
                    if (cardNum == flippedNum)
                    {
                        maStack.Push(boardIndex);
                        if(maStack.Count==board.Length)
                        {
                            timerOn = false;
                            await DisplayAlert("Congratulations!", "Time Left: " + sec.ToString() + "\nFlips: " + flips.ToString(), "Retry");

                            initGame();

                        }
                        else
                        {
                            
                            turn--;
                        }

                    }
                    else
                    {
                        System.Threading.Tasks.Task.Delay(250).Wait();
                        changeImage(flippedX, flippedY, 0);
                        changeImage(x, y, 0);
                        maStack.Pop();
                        turn--;
                    }

                }
                enableButtons();
            }
        }

        async void Handle_Clicked_1(object sender, System.EventArgs e)
        {
            var answer = await DisplayAlert("Restart", "Would you like to restart?", "Yes", "No");
            if (answer)
            {
                initGame();
            }

        }
    }
}
