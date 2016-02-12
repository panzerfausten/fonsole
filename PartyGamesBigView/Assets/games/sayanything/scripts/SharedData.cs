﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPlatform.SayAnything
{     
    
    public enum GameState
    {
        WaitForStart = 0,
        Questioning = 1,
        Answering = 2,
        ShowAnswers = 3,
        JudgingAndVoting = 4,
        Voting = 5,
        ShowWinner = 6,
        ShowScore = 7
    }
    
    public class SharedData
    {
        public const int UNDEFINED = -1;



        public GameState state = SayAnything.GameState.WaitForStart;
        
        //user id of the judge. only the connection id for now
        public int judgeUserId = -1;
        
        //simply the text of the question
        public string question = null;
        
        //will contain the player id as key and then the answer.
        public Dictionary<int, string> answers = new Dictionary<int, string>();
        
        //user id of the answer the judge has chosen
        public int judgedAnswerId = -1;
        
        //key: user id (equals the id the answer of this user has in the "answers" object)
        //value: a list of user ids the vote came from (needed to show the color badges in the end)
        //(move to local data, view only?)
        public Dictionary<int, List<int>> votes = new Dictionary<int, List<int>>();
        
        //scores in this round (move to local data, view only?)
        public Dictionary<int, int> roundScore = new Dictionary<int, int>();
        
        //scores overall (move to local data, view only?)
        public Dictionary<int, int> totalScore = new Dictionary<int, int>();

        public int timeLeft = 30;
         
         //functions to easily fill and read the data (ideall this should be done only via functions later to prevent bugs)
         
        // void addVote = function(lFrom, lTo)
        // {
        //    if(lTo in self.votes)
        //    {
        //        //user got at least one vote already -> add the new vote
        //        self.votes[lTo].push(lFrom);
        //    }else
        //    {
        //        //user didn't get a vote yet -> add a list with one vote
        //        self.votes[lTo] = [lFrom];
        //    }
        // };
         
        // //returns a list of votes a certain userid/answerid received
        // this.getVotes = function(lUserId)
        // {
        //     if(lUserId in self.votes)
        //     {
        //         return self.votes[lUserId];
        //     }
        //     else{
        //         return []; //empty list. user never received a vote
        //     }
        // };
         
         
        public void resetRoundData()
        {
            this.state = SayAnything.GameState.WaitForStart;

            this.judgeUserId = -1;
            this.question = null;
            this.answers = new Dictionary<int,string>();
            this.judgedAnswerId = -1;
            this.votes = new Dictionary<int,List<int>>();
            this.roundScore = new Dictionary<int, int>();
         }
         
        // this.awardScore = function(lUserId, lPoints)
        // {
        //     if(lUserId in self.roundScore)
        //     {
        //         self.roundScore[lUserId] += lPoints;
        //     }else{
        //         self.roundScore[lUserId] = lPoints;
        //     }
             
        //     if(lUserId in self.totalScore)
        //     {
        //         self.totalScore[lUserId] += lPoints;
        //     }else{
        //         self.totalScore[lUserId] = lPoints;
        //     }
        // };
    }
}
