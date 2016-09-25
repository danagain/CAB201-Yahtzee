using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Have to disable scoreButtons using Ones, Twos and assigning them to a number.

namespace Yahtzee_Game {
    public partial class Form1 : Form {

        private static int numlabels = 19;
        private static int numDice = 5;
        private static int numButtons = 16;
        private Label[] dice = new Label[numDice];
        private Button[] scoreButtons = new Button[numButtons];
        private Label[] scoreTotals = new Label[numlabels];
        private CheckBox[] checkBoxes = new CheckBox[numDice];
        private Game game;

        //Used for changing the number of players with the numericUpDown
        public int numPlayers = 1;


        public Form1() {
            InitializeComponent();
            InitializeLabelsAndButtons();
            DisableRollButton();
        }

        private void InitializeLabelsAndButtons() {   //initialze dice array of Labels

            dice = new Label[] { die1, die2, die3, die4, die5 };
            //initalize scoreButtons array of Buttons
            scoreButtons[(int)ScoreType.Ones] = onesButton;
            scoreButtons[(int)ScoreType.Twos] = twosButton;
            scoreButtons[(int)ScoreType.Threes] = threesButton;
            scoreButtons[(int)ScoreType.Fours] = foursButton;
            scoreButtons[(int)ScoreType.Fives] = fivesButton;
            scoreButtons[(int)ScoreType.Sixes] = sixesButton;
            scoreButtons[(int)ScoreType.ThreeOfAKind] = threeKindButton;
            scoreButtons[(int)ScoreType.FourOfAKind] = fourKindButton;
            scoreButtons[(int)ScoreType.SmallStraight] = smlStraightButton;
            scoreButtons[(int)ScoreType.LargeStraight] = lrgStraightButton;
            scoreButtons[(int)ScoreType.FullHouse] = fullHouseButton;
            scoreButtons[(int)ScoreType.Chance] = chanceButton;
            scoreButtons[(int)ScoreType.Yahtzee] = yahtzeeButton;
            //initialize scoreTotals array of Labels

            scoreTotals[(int)ScoreType.Ones] = onesLabel;
            scoreTotals[(int)ScoreType.Twos] = twosLabel;
            scoreTotals[(int)ScoreType.Threes] = threesLabel;
            scoreTotals[(int)ScoreType.Fours] = foursLabel;
            scoreTotals[(int)ScoreType.Fives] = fivesLabel;
            scoreTotals[(int)ScoreType.Sixes] = sixesLabel;
            scoreTotals[(int)ScoreType.SubTotal] = subTotalLabel;
            scoreTotals[(int)ScoreType.BonusFor63Plus] = bonusScoreLabel;
            scoreTotals[(int)ScoreType.SectionATotal] = upperTotalLabel;
            scoreTotals[(int)ScoreType.ThreeOfAKind] = threeKindLabel;
            scoreTotals[(int)ScoreType.FourOfAKind] = fourKindLabel;
            scoreTotals[(int)ScoreType.SmallStraight] = smlStraightLabel;
            scoreTotals[(int)ScoreType.LargeStraight] = lrgStraightLabel;
            scoreTotals[(int)ScoreType.FullHouse] = fullHouseLabel;
            scoreTotals[(int)ScoreType.Chance] = chanceLabel;
            scoreTotals[(int)ScoreType.Yahtzee] = yahtzeeLabel;
            scoreTotals[(int)ScoreType.YahtzeeBonus] = yahtzeeBonusLabel;
            scoreTotals[(int)ScoreType.SectionBTotal] = lowerTotalLabel;
            scoreTotals[(int)ScoreType.GrandTotal] = grandTotalLabel;

            checkBoxes = new CheckBox[] { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5 };

            //Unchecks and disables all checkboxes
            for (int i = 0; i < numDice; i++) {
                checkBoxes[i].Checked = false;
                checkBoxes[i].Enabled = false;
                dice[i].Text = " ";
            }
            //Clears all the numbers in the score Totals
            for (int i = 0; i < scoreTotals.GetLength(0); i++) {
                if (scoreTotals[i] != null) {
                    scoreTotals[i].Text = "";
                }
            }
        }

        //Update playerDataGrid
        private void UpdatePlayersDataGridView() {
            game.Players.ResetBindings();
            dataGridView1.Update();
            dataGridView1.Refresh();
        }

        public Label[] GetDice() {
            return dice;
        }

        public Label[] GetScoreTotals() {
            return scoreTotals;
        }

        public void ShowPlayerName(string name) {
            playerName.Text = name;
        }

        public void EnableRollButton() {
            RollDice.Enabled = true;
        }

        public void DisableRollButton() {
            RollDice.Enabled = false;
        }

        public void EnableCheckBoxes() {
            for (int i = 0; i < numDice; i++){
                checkBoxes[i].Enabled = true;
            }
        }

        public void DisableAndClearCheckBoxes() {
            for (int i = 0; i < numDice; i++) {
                checkBoxes[i].Enabled = false;
                checkBoxes[i].Checked = false;
            }
        }

        public void EnableScoreButton(ScoreType combo) {
            if (scoreButtons[(int)combo] != null) {
                scoreButtons[(int)combo].Enabled = true;
            }
        }

        public void DisableScoreButton(ScoreType combo) {
            if (scoreButtons[(int) combo] != null) {
                scoreButtons[(int) combo].Enabled = false;
            }
        }

        public void CheckCheckBox(int index) {
            checkBoxes[index].Checked = true;
        }

        public void ShowMessage(string message) {
            messageLabel.Text = message;
        }

        public void ShowOkButton() {
            okButton.Visible = true;
            okButton.Enabled = true;
        }

        //Disables Ok button
        public void DisableOkButton() {
            okButton.Visible = false;
            okButton.Enabled = false;
        }

        public void StartNewGame() {
            game = new Game(this);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e) {
            StartNewGame();
            for (int i = 0; i < scoreButtons.GetLength(0); i++) {
                if (scoreButtons[i] != null) {
                    scoreButtons[i].Enabled = false;
                }
            }
            EnableRollButton();
            messageLabel.Text = "Roll 1";
            playerName.Text = "Player 1";
            InitializeLabelsAndButtons();
            playersBindingSource.DataSource = game.Players;
        }

        private void RollDice_Click(object sender, EventArgs e) {
            game.RollDice();
            EnableCheckBoxes();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            if (checkBoxes[0].Checked) {
                CheckCheckBox(0);
                game.HoldDie(0);
            } else {
                game.ReleaseDie(0);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) {
            if (checkBoxes[1].Checked)
            {
                CheckCheckBox(1);
                game.HoldDie(1);
            } else {
                game.ReleaseDie(1);
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e) {
            if (checkBoxes[2].Checked) {
                CheckCheckBox(2);
                game.HoldDie(2);
            } else {
                game.ReleaseDie(2);
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e) {
            if (checkBoxes[3].Checked) {
                CheckCheckBox(3);
                game.HoldDie(3);
            } else {
                game.ReleaseDie(3);
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e) {
            if (checkBoxes[4].Checked) {
                CheckCheckBox(4);
                game.HoldDie(4);
            } else {
                game.ReleaseDie(4);
            }
        }

        
        private void eventHandlerButtons (ScoreType combo) {
            DisableRollButton();
            game.ScoreCombination(combo);
            for (int i = 0; i < scoreButtons.GetLength(0); i++) {
                DisableScoreButton((ScoreType)i);
            }
            UpdatePlayersDataGridView();
        }

        private void button1_Click(object sender, EventArgs e) {
            eventHandlerButtons(ScoreType.Ones);
        }

        private void button2_Click(object sender, EventArgs e) {
            eventHandlerButtons(ScoreType.Twos);
        }

        private void button3_Click(object sender, EventArgs e) {
            eventHandlerButtons(ScoreType.Threes);
        }

        private void button4_Click(object sender, EventArgs e) {
            eventHandlerButtons(ScoreType.Fours);
        }

        private void button5_Click(object sender, EventArgs e) {
            eventHandlerButtons(ScoreType.Fives);
        }

        private void button6_Click(object sender, EventArgs e) {
            eventHandlerButtons(ScoreType.Sixes);
        }

        private void threeKindButton_Click(object sender, EventArgs e) {
            eventHandlerButtons(ScoreType.ThreeOfAKind);
        }

        private void fourKindButton_Click(object sender, EventArgs e) {
            eventHandlerButtons(ScoreType.FourOfAKind);
        }

        private void fullHouseButton_Click(object sender, EventArgs e) {
            eventHandlerButtons(ScoreType.FullHouse);
        }

        private void smlStraightButton_Click(object sender, EventArgs e) {
            eventHandlerButtons(ScoreType.SmallStraight);
        }

        private void lrgStraightButton_Click(object sender, EventArgs e) {
            eventHandlerButtons(ScoreType.LargeStraight);
        }

        private void chanceButton_Click(object sender, EventArgs e) {
            eventHandlerButtons(ScoreType.Chance);
        }

        private void yahtzeeButton_Click(object sender, EventArgs e) {
            eventHandlerButtons(ScoreType.Yahtzee);
        }

        private void okButton_Click(object sender, EventArgs e) {
            game.NextTurn();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e) {
            numPlayers = (int)numericUpDown.Value;
        }
    }
}

