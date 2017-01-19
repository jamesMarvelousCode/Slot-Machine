using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SlotMachine
{
    public partial class Form1 : Form
    {
        SoundPlayer fundsChangeSound;
        SoundPlayer winSound;
        SoundPlayer slotStopSound;
        SoundPlayer houseOneSound;
        SoundPlayer houseTwoSound;
        SoundPlayer errorSound;

        Image[] slotImages;
        Image one;
        Image two;
        Image three;

        Random random = new Random();

        int imageOneIndex;
        int imageTwoIndex;
        int imageThreeIndex;
        int timerIndex;
        int currentBet;
        int currentFunds;
        int winnings;

        int STARTINGFUNDS = 10;
        int NO_FUNDS = 0;
        int TIMER_INDEX_INCREMENT = 100;
        int ADD_FUNDS = 10;
        int FIRST_STOP_INDEX = 2700;
        int SECOND_STOP_INDEX = 4900;
        int THIRD_STOP_INDEX = 7100;
        int WINMULTIPLIERBIG = 5;
        int WINMULTIPLIERSMALL = 2;
        int END_OF_ARRAY = 7;

        bool ONE_STOPPED;
        bool TWO_STOPPED;

        public Form1()
        {
            InitializeComponent();

            fundsChangeSound = new SoundPlayer(Properties.Resources.fundsChange);
            winSound = new SoundPlayer(Properties.Resources.win);
            slotStopSound = new SoundPlayer(Properties.Resources.slotStop);
            houseOneSound = new SoundPlayer(Properties.Resources.houseOne);
            houseTwoSound = new SoundPlayer(Properties.Resources.houseTwo);
            errorSound = new SoundPlayer(Properties.Resources.error);

            slotImages = new Image[8];
            slotImages[0] = Properties.Resources.slotMachineTarget;
            slotImages[1] = Properties.Resources.slotMachineRad;
            slotImages[2] = Properties.Resources.slotMachineNuka;
            slotImages[3] = Properties.Resources.slotMachineMiniNuke;
            slotImages[4] = Properties.Resources.slotMachineHeart;
            slotImages[5] = Properties.Resources.slotMachineGrenade;
            slotImages[6] = Properties.Resources.slotMachineGear;
            slotImages[7] = Properties.Resources.slotMachineBolt;

            imageOneIndex = random.Next(0,7);
            imageTwoIndex = random.Next(0, 7);
            imageThreeIndex = random.Next(0, 7);
            timerIndex = 0;

            ONE_STOPPED = true;
            TWO_STOPPED = true; 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            currentFundsLabel.Text = STARTINGFUNDS.ToString();
            betLabel.Text = NO_FUNDS.ToString();

            slotOnePictureBox.BackgroundImage = slotImages[0];
            slotTwoPictureBox.BackgroundImage = slotImages[0];
            slotThreePictureBox.BackgroundImage = slotImages[0];
        }

        private void spinButton_Click(object sender, EventArgs e)
        {
            if (int.Parse(currentFundsLabel.Text) == NO_FUNDS && int.Parse(betLabel.Text) == NO_FUNDS)
            {
                errorSound.Play();
            }
            else if (int.Parse(betLabel.Text) > NO_FUNDS)
            {
                spinTimer.Enabled = true;
                spinButton.Enabled = false;
                betDownButton.Enabled = false;
                betUpButton.Enabled = false;
            }
            else
            {
                errorSound.Play();
            }        
        }

        private void betUpButton_Click(object sender, EventArgs e)
        {
            increaseBet();
        }

        private void betDownButton_Click(object sender, EventArgs e)
        {
            decreaseBet();
        }

        private void increaseBet()
        {
            currentFunds = int.Parse(currentFundsLabel.Text);
            currentBet = int.Parse(betLabel.Text);

            if (currentFunds > NO_FUNDS)
            {
                currentBet++;
                betLabel.Text = currentBet.ToString();

                currentFunds--;
                currentFundsLabel.Text = currentFunds.ToString();
                fundsChangeSound.Play();
            }
            else
            {
                errorSound.Play();
            }
            
        }

        private void decreaseBet()
        {
            currentFunds = int.Parse(currentFundsLabel.Text);
            currentBet = int.Parse(betLabel.Text);

            if (currentBet > NO_FUNDS)
            {
                currentBet--;
                betLabel.Text = currentBet.ToString();

                currentFunds++;
                currentFundsLabel.Text = currentFunds.ToString();
                fundsChangeSound.Play();
            }
            else
            {
                errorSound.Play();
            }
        }

        private void spinTimer_Tick(object sender, EventArgs e)
        {
            if(timerIndex < THIRD_STOP_INDEX)
            {    
                slotThreePictureBox.BackgroundImage = slotImages[imageThreeIndex];            
                if (imageThreeIndex == END_OF_ARRAY)
                {
                    imageThreeIndex = 0;
                }
                ++imageThreeIndex;
                timerIndex += TIMER_INDEX_INCREMENT;

                if (timerIndex < SECOND_STOP_INDEX && TWO_STOPPED)
                { 
                    slotTwoPictureBox.BackgroundImage = slotImages[imageTwoIndex];                   
                    if (imageTwoIndex == END_OF_ARRAY)
                    {
                        imageTwoIndex = 0;
                    }
                    ++imageTwoIndex;
                    timerIndex += TIMER_INDEX_INCREMENT;

                    if (timerIndex < FIRST_STOP_INDEX && ONE_STOPPED)
                    {
                        slotOnePictureBox.BackgroundImage = slotImages[imageOneIndex];                        
                        if (imageOneIndex == END_OF_ARRAY)
                        {
                            imageOneIndex = 0;
                        }
                        ++imageOneIndex;
                        timerIndex += TIMER_INDEX_INCREMENT;
                    }
                    else if (ONE_STOPPED)
                    {
                        slotStopSound.Play();
                        ONE_STOPPED = false;
                    }
                }
                else if (TWO_STOPPED)
                {
                    slotStopSound.Play();
                    TWO_STOPPED = false;
                }
            }
            else
            {
                spinTimer.Enabled = false;
                timerIndex = 0;
                slotStopSound.Play();
                ONE_STOPPED = true;
                TWO_STOPPED = true;
                spinButton.Enabled = true;
                betUpButton.Enabled = true;
                betDownButton.Enabled = true;
                checkWinLose();
            }
        }

        private void checkWinLose()
        {
            one = slotOnePictureBox.BackgroundImage;
            two = slotTwoPictureBox.BackgroundImage;
            three = slotThreePictureBox.BackgroundImage;

            if (one == two && two == three)
            {
                int tempFunds = int.Parse(currentFundsLabel.Text);
                int tempBet = int.Parse(betLabel.Text);

                winnings = tempBet * WINMULTIPLIERBIG;

                currentFundsLabel.Text = (tempFunds + winnings).ToString();

                winSound.Play();
            }
            else if (one == two || two == three || one == three)
            {
                int tempFunds = int.Parse(currentFundsLabel.Text);
                int tempBet = int.Parse(betLabel.Text);

                winnings = tempBet * WINMULTIPLIERSMALL;

                currentFundsLabel.Text = (tempFunds + winnings).ToString();

                winSound.Play();
            }
            else if (int.Parse(currentFundsLabel.Text) == NO_FUNDS)
            {
                errorSound.Play();
            }

            betLabel.Text = NO_FUNDS.ToString();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            if (int.Parse(currentFundsLabel.Text) == NO_FUNDS)
            {
                errorSound.Play();
            }
            this.Close();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentFundsLabel.Text = STARTINGFUNDS.ToString();
            betLabel.Text = NO_FUNDS.ToString();
            slotOnePictureBox.BackgroundImage = slotImages[0];
            slotTwoPictureBox.BackgroundImage = slotImages[0];
            slotThreePictureBox.BackgroundImage = slotImages[0];
            fundsChangeSound.Play();
        }

        private void addFundsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int tempFunds = int.Parse(currentFundsLabel.Text);
            tempFunds += ADD_FUNDS;
            currentFundsLabel.Text = tempFunds.ToString();
            fundsChangeSound.Play();
        }

        #region oops
        private void slotTwoPictureBox_Click(object sender, EventArgs e)
        {

        }
        #endregion   
    }
}
