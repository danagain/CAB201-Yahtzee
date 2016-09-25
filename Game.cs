using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;

namespace Yahtzee_Game {

    public enum ScoreType {
        Ones, Twos, Threes, Fours, Fives, Sixes,
        SubTotal, BonusFor63Plus, SectionATotal,
        ThreeOfAKind, FourOfAKind, FullHouse,
        SmallStraight, LargeStraight, Chance, Yahtzee,
        YahtzeeBonus, SectionBTotal, GrandTotal
    }

    class Game {
        private BindingList<Player> players;
        private int currentPlayerIndex;
        private Player currentPlayer;
        private static int numDice = 5;
        private Die[] dice = new Die[numDice];
        private int numRolls = 1;
        private Form1 form;
        private Label[] dieLabels = new Label[numDice];

        private int numButtons = 16;
        
        private int[] dieValues = new int[numDice];
        string[] messages = {"Roll 1", "Roll 2 or choose a combination to score",
                                    "Roll 3 or choose a combination to score",
                                    "Choose a combination to score",
                                    "Your turn has ended - click OK"};
        

        public Game(Form1 form) { //The constructor
            this.form = form;
            dieLabels = form.GetDice();
            players = new BindingList<Player>();

            //Creating the new players
            for (int i = 1; i <= form.numPlayers; i++) {
                players.Add(new Player("Player " + i, form.GetScoreTotals()));
            }
            //Setting the first player
            currentPlayer = players[0];
            //Creating dice
            for (int i = 0; i < numDice; i++) {
                dice[i] = new Die(dieLabels[i]);
            }
            dieLabels = form.GetDice();
        }

        public BindingList<Player> Players {
            get {
                return players;
            }
        }

        public void NextTurn() {

            //Clearing all the boxes for the new turn
            Label[] labels = form.GetScoreTotals();
            for (int i = 0; i < labels.GetLength(0); i++) {
                labels[i].Text = "";
            }
            
            form.ShowMessage(messages[0]);
            numRolls = 1;
            //Setting the currentPlayer
            currentPlayerIndex++;
            if (currentPlayerIndex == players.Count) {
                currentPlayerIndex = 0;
            }
            //Enabling the roll button and disabling the ok button so the player plays
            currentPlayer = players[currentPlayerIndex];
            form.ShowPlayerName(currentPlayer.Name);
            form.EnableRollButton();
            form.DisableOkButton();

            //Showing currentPlayer scores
            currentPlayer.ShowScores();

            //Disabling the score buttons and clearing the dice labels
            for (int i = 0; i < numButtons; i++) {
                form.DisableScoreButton((ScoreType)i);
            }
            
            for (int i = 0; i < dice.GetLength(0); i++) {
                dieLabels[i].Text = "";
                form.DisableAndClearCheckBoxes();
            }
        }

        public void RollDice() {
            //Only enabling button if it hasnt been attempted
            for (int i = 0; i < numButtons; i++){
                if (currentPlayer.IsAvailable((ScoreType)i)) {
                    form.EnableScoreButton((ScoreType)i);
                }
            }

            
            if (numRolls == 3) {
                form.DisableRollButton();
            }
            form.ShowMessage(messages[numRolls]);
            //Rolling the dice
            for (int i = 0; i < numDice; i++) {
                if (dice[i].Active) {
                    dieLabels[i].Text = dice[i].Roll().ToString();
                }
            }
            numRolls++;
        }

        public void HoldDie(int die) {
            dice[die].Active = false;
        }

        public void ReleaseDie(int die) {
            dice[die].Active = true;
        }

        public void ScoreCombination(ScoreType combo) {
            form.ShowMessage(messages[4]);
            for (int i = 0; i < numDice; i++){
                dieValues[i] = dice[i].FaceValue;
            }

            currentPlayer.ScoreCombination(combo, dieValues);
            form.ShowOkButton();
        }

        static Game Load(Form1 form) {
            return null;
        }

        public void Save() {

        }
    }

    class Player {
        private string name;
        private int combinationsToDo;
        private static int numLabels = 19;
        private Score[] scores = new Score[numLabels];
        private int grandTotal;
        private int numButtons = 16;


        public Player(string playerName, Label[] label) {
            name = playerName;

            for (int i = 0; i < scores.GetLength(0); i++) {
                switch (i) {
                    case (int)ScoreType.Ones:
                    case (int)ScoreType.Twos:
                    case (int)ScoreType.Threes:
                    case (int)ScoreType.Fours:
                    case (int)ScoreType.Fives:
                    case (int)ScoreType.Sixes:
                        scores[i] = new CountingCombination((ScoreType)i, label[i]);
                        break;
                    case (int)ScoreType.ThreeOfAKind:
                    case (int)ScoreType.FourOfAKind:
                    case (int)ScoreType.Chance:
                        scores[i] = new TotalOfDice((ScoreType)i, label[i]);
                        break;
                    case (int)ScoreType.SmallStraight:
                    case (int)ScoreType.LargeStraight:
                    case (int)ScoreType.FullHouse:
                    case (int)ScoreType.Yahtzee:
                        scores[i] = new FixedScore((ScoreType)i, label[i]);
                        break;
                    case (int)ScoreType.SubTotal:
                    case (int)ScoreType.BonusFor63Plus:
                    case (int)ScoreType.SectionATotal:
                    case (int)ScoreType.YahtzeeBonus:
                    case (int)ScoreType.SectionBTotal:
                    case (int)ScoreType.GrandTotal:
                        scores[i] = new BonusOrTotal(label[i]);
                        break;
                }
            }

            //Enabling the totals label so their value can be changed
            //Finding the required paramter in scores and setting which class I want to call
            //with (BonusOrTotal) in brackets.
            //From there I call the private helper function I created to setDoneToFalse, so that
            //the label can show and update the score.
            var enableNeccessaryLabels = (BonusOrTotal)scores[(int)ScoreType.GrandTotal];
            enableNeccessaryLabels.setDoneToFalse(ScoreType.GrandTotal);
            enableNeccessaryLabels = (BonusOrTotal)scores[(int)ScoreType.SectionATotal];
            enableNeccessaryLabels.setDoneToFalse(ScoreType.SectionATotal);
            enableNeccessaryLabels = (BonusOrTotal)scores[(int)ScoreType.SectionBTotal];
            enableNeccessaryLabels.setDoneToFalse(ScoreType.SectionBTotal);
            enableNeccessaryLabels = (BonusOrTotal)scores[(int)ScoreType.SubTotal];
            enableNeccessaryLabels.setDoneToFalse(ScoreType.SubTotal);
            scores[(int)ScoreType.BonusFor63Plus].Points += 35;
            enableNeccessaryLabels = (BonusOrTotal)scores[(int)ScoreType.BonusFor63Plus];
            enableNeccessaryLabels.setDoneToFalse(ScoreType.BonusFor63Plus);
            scores[(int)ScoreType.YahtzeeBonus].Points += 100;
            enableNeccessaryLabels = (BonusOrTotal)scores[(int)ScoreType.YahtzeeBonus];
            enableNeccessaryLabels.setDoneToFalse(ScoreType.YahtzeeBonus);

        }

        public string Name {
            get {
                return name;
            } set {
                { name = value; }
            }
        }


        public void ScoreCombination(ScoreType combo, int[] faceValues){
            //Setting scoreCalculation to the scores value I want to find and also selecting the class
            //It the calculates the score by running the function for that ScoreType
            //It then shows the score on the label and updates the grandTotal
            //If the scoretype is less than 6 then it is part of the upper total so it added
            //If the scoretype is greater than 8 and lower than 16 it is part of the lower total
            //Then calculating whether the upper total has a score greater than or equal to 63
            //So then to add the bonus amount of points
            //Then Showing the different Total points
            var scoreCalculation = (Combination)scores[(int) combo];
            scoreCalculation.CalculateScore(faceValues);
            scores[(int)combo].ShowScores();
            GrandTotal += scores[(int)combo].Points;
            scores[(int) ScoreType.GrandTotal].Points += scores[(int) combo].Points;
            scores[(int)ScoreType.GrandTotal].ShowScores();
            if ((int) combo < 6) {
                scores[(int)ScoreType.SectionATotal].Points += scores[(int)combo].Points;
                scores[(int)ScoreType.SubTotal].Points += scores[(int)combo].Points;
            } else if ((int)combo > 8 && (int)combo < numButtons) {
                scores[(int)ScoreType.SectionBTotal].Points += scores[(int)combo].Points;
            }
            if (scores[(int)ScoreType.SubTotal].Points >= 63) {
                scores[(int)ScoreType.SectionATotal].Points += 35;
            }
            scores[(int)ScoreType.SectionATotal].ShowScores();
            scores[(int)ScoreType.SectionBTotal].ShowScores();
            scores[(int)ScoreType.SubTotal].ShowScores();
            scores[(int)ScoreType.BonusFor63Plus].ShowScores();
            scores[(int)ScoreType.YahtzeeBonus].ShowScores();
        }

        public int GrandTotal {
            get {
                return grandTotal;
            } set {
                grandTotal = value;
            }
        }

        public bool IsAvailable(ScoreType combo) {
            if (scores[(int)combo].Done) {
                return true;
            } else {
                return false;
            }
        }

        public void ShowScores() {
            for (int i = 0; i < numLabels; i++) {
                scores[i].ShowScores();
            }
        }

        public bool IsFinished() {
            if (combinationsToDo == 0) {
                return true;
            } else {
                return false;
            }
        }

        public void Load(Label[] label) {

        }
    }

    abstract class Score {

        private int points;
        Label newLabel = new Label();
        protected bool done = true;

        public Score(Label label) {
            newLabel = label;
        }

        public int Points {
            get {
                return points;
            } set {
                points = value;
            }
        }

        public bool Done {
            get {
                return done;
            }
        }
        
        //Used !done to check if the player has used a specified button
        public void ShowScores() {
            if (!done) {
                newLabel.Text = Convert.ToString(points);
            }
        }

        public void Load(Label label){

        }
    }

     class Die {
        private int faceValue;
        private bool active;
        Label label = new Label();
        static Random random;
        StreamReader rollFile;
        static bool DEBUG;

        public Die(Label dice) {
            active = true;
            random = new Random();
        }

        public int FaceValue {
            get {
                return faceValue;
            }
        }

        public bool Active {
            get {
                return active;
            } set {
               active = value;
            }
        }

        public int Roll() {
            faceValue = random.Next(1, 7);
            return faceValue;
        }

        void Load(Label label) {
        }
    }

     abstract class Combination : Score {
        public Combination(Label label) : base(label) {}

        public abstract void CalculateScore(int[] diceValues);


        public int[] Sort(int[] diceValues) {
            Array.Sort<int>(diceValues);
            return diceValues;
        }
     }

     class CountingCombination : Combination {
         //Calculating ones, twos, threes, fours, fives and sixes
         private int dieValue;
         private int score;

         public CountingCombination(ScoreType combo, Label label) : base(label) {
             dieValue = (int)combo + 1;
        }
        
         //Using dieValue to compare against all the values in dicevalues so that if they match a number can be added
         //that number is then multiplied by the dieValue
         public override void CalculateScore(int[] diceValues) {
             for (int i = 0; i < diceValues.GetLength(0); i++) {
                 if (dieValue == diceValues[i]) {
                     score++;
                 }
             }
             Points = score * dieValue;
             done = false;
         }
     }

     //Calculating score for Small Straight, Large Straight, FullHouse and Yahtzee
     class FixedScore : Combination {
         private ScoreType scoreType;
         private int straightCount;
         private int innerHouseCount = 1;
         private int outerHouseCount = 1;
         private int changeNumber;
         private int yahtzeeCount;
         private int scoreToGive;
         private int numYahtzee;

         public FixedScore(ScoreType score, Label label) : base(label) {
             scoreType = score;
         }
         public override void CalculateScore(int[] diceValues) {
             Array.Sort<int>(diceValues);
             //Used for finding smallStraights, it loops through the list and checks if the next value
             //is 1 greater than the current value an int is updated and if that int is greater than
             //or equal to 3 then it is a small straight
             if (scoreType == ScoreType.SmallStraight) {
                 for (int i = 0; i < diceValues.GetLength(0) - 1; i++) {
                     if (diceValues[i] == (diceValues[i + 1] - 1)) {
                         straightCount++;
                     }
                 }
                 if (straightCount >= 3) {
                     scoreToGive = 30;
                 } 
                 //Similar method for largeStraights just has to find another match
             } else if (scoreType == ScoreType.LargeStraight) {
                 for (int i = 0; i < diceValues.GetLength(0) - 1; i++) {
                     if (diceValues[i] == diceValues[i + 1] - 1) {
                         straightCount++;
                     }
                 }
                 if (straightCount == 4) {
                     scoreToGive = 40;
                 } 

                 //Counts the number of dice at the start of the list, if that number is equal to 2
                 //it sets the changeNumber so that the next 2 or 3 numbers in the list are not the same
                 //then if the next value is different to the current it breaks the for loop
                 //The next for loop is executed compares the current value against the changeNumber
                 //it makes sure the current number is different so that a yahtzee won't count
                 //as a fullhouse. It then checks if this current number contains 2 or 3 in the list
                 //The if statement then checks if either innerHouseCount == 2 or 3 and outerHouseCount
                 // is equal to 2 or 3. The next if statement checks if they both add up to 5, so that
                 //two pairs in the set wont count as a fullHouse.

             } else if (scoreType == ScoreType.FullHouse) {
                 for (int i = 0; i < diceValues.GetLength(0) - 1; i++) {
                     if (diceValues[i] == diceValues[i + 1]) {
                         innerHouseCount++;
                     } else if (diceValues[i] != diceValues[i + 1]) {
                         break;
                     }
                     if (innerHouseCount == 2) {
                         changeNumber = diceValues[i];
                     }
                 }
                 for (int i = 0; i < diceValues.GetLength(0) - 1; i++) {
                     if (diceValues[i] != changeNumber) {
                         if (diceValues[i] == diceValues[i + 1]) {
                             outerHouseCount++;
                         }
                     }
                 }
                 if ((innerHouseCount == 3 || outerHouseCount == 3) && (innerHouseCount == 2 || outerHouseCount == 2)) {
                     if (innerHouseCount + outerHouseCount == 5) {
                         scoreToGive = 25;
                     }
                 }
                 //Upon 5 identical matches the scoreToGive is set to give the player the yahtzee  points
             } else if (scoreType == ScoreType.Yahtzee) {
                 for (int i = 0; i < diceValues.GetLength(0); i++) {
                     if (diceValues[0] == diceValues[i]) {
                         yahtzeeCount++;
                     }
                 }
                 if (yahtzeeCount == 5 && numYahtzee == 0) {
                     scoreToGive = 50;
                     numYahtzee++;
                 } else if (yahtzeeCount == 5 && numYahtzee == 1) {
                     scoreToGive = 100;
                 }
             }
             Points = Points + scoreToGive;
             done = false;
         }
     }

     class TotalOfDice : Combination {
         private int numberOfOneKind;
         private int amountOfValues;
         private int score;
         private bool requirementsMet;

         public TotalOfDice(ScoreType score, Label label) : base(label) {
             if (score == ScoreType.ThreeOfAKind) {
                 numberOfOneKind = 2;
             } else if (score == ScoreType.FourOfAKind) {
                 numberOfOneKind = 3;
             } else if (score == ScoreType.Chance) {
                 numberOfOneKind = 0;
             }
         }

         public override void CalculateScore(int[] diceValues) {
             Array.Sort<int>(diceValues);
             //NumberOfOneKind is used for chance because no requirements, then sets requirementsMet bool
             //to true so that the scores can be added.
             if (numberOfOneKind == 0) {
                 requirementsMet = true;
                //Used for finding ThreeOfAkind, it loops through the list and checks if the next value
                 //is the same as the current value, if so an int is updated and if that int is greater than
                 //or equal to 2 then it is a three of a kind
             } else if (numberOfOneKind == 2) {
                 for (int i = 0; i < diceValues.GetLength(0) - 1; i++) {
                     if (diceValues[i] == diceValues[i + 1]) {
                         amountOfValues++;
                     } else {
                         amountOfValues = 0;
                     }
                     if (amountOfValues >= numberOfOneKind) {
                         requirementsMet = true;
                     }
                 }
                 //Similar method involved for four of a kind just looks for an extra value to be the same
             } else if (numberOfOneKind == 3) {
                 for (int i = 0; i < diceValues.GetLength(0) - 1; i++) {
                     if (diceValues[i] == diceValues[i + 1]) {
                         amountOfValues++;
                     } else {
                         amountOfValues = 0;
                     }
                     if (amountOfValues >= numberOfOneKind) {
                         requirementsMet = true;
                     }
                 }
             }
             //all the methods above pass the requirementsMet bool if they meet the requirements
             if (requirementsMet) {
                 for (int i = 0; i < diceValues.GetLength(0); i++) {
                     score += diceValues[i];
                 }
                 Points = score;
             }
             done = false;
         }
     }

     class BonusOrTotal : Score {

         public BonusOrTotal(Label label) : base(label) {
         }

         //Used for ScoreTypes to done so that their value can be changed
         public void setDoneToFalse(ScoreType combo){
             done = false;
         }

     }
}

