using System;
using System.Collections.Generic;

namespace NPC
{
    public struct Decision_NPC
    {
        public int score;
        public int positive1;
        public int positive2;
        public int negative1;
        public int negative2;

        public void Init()
        {
            score = 0;
            positive1 = 0;
            positive2 = 0;
            negative1 = 0;
            negative2 = 0;
        }
    }

    public struct NPC
    {
        //Big 5 Personality
        public int extroversion;
        public int agreeability;
        public int openness;
        public int conscientiousness;
        public int neuroticism;

        //Confidence/Ability/Social rank
        public int self_belief;

        //Emotion
        public int fear;
        public int anger;
        public int happy;
        public int sad;

        public int shock;
        public int pain;

        //Physical
        public int stamina;
        public int health;

        //Disposition
        public int love;
        public int max_love; //Damage - can only go down
        public int trust;
        public int max_trust; //Damage - can only go down
        public int admiration;

        //Submissivness is calc from self reliance vs admiration and affects decision making
        public int submissivness;

        //Bargaining power is calc from personality/health/Disposition and affects decision making
        public int bargaining_power;

        //History
        public List<int> keys, values;
        public List<string> comments;   //

        public void Init()
        {
            extroversion = 50;
            agreeability = 50;
            openness = 50;
            conscientiousness = 50;
            neuroticism = 50;

            //Confidence/Ability/Social rank
            self_belief = 10;

            //Emotion
            fear = 0;
            anger = 0;
            happy = 0;
            sad = 0;

            shock = 0;
            pain = 0;

            //Physical
            stamina = 10;
            health = 10;

            //Disposition
            love = 50;
            max_love = 100; //Damage - can only go down
            trust = 50;
            max_trust = 100; //Damage - can only go down
            admiration = 0;

            //Submissivness is calc from self reliance vs admiration and affects decision making
            submissivness = 0;

            //Bargaining power is calc from personality/health/Disposition and affects decision making
            bargaining_power = 50;
        }

        /// <summary>
        /// Respond to an event
        /// For example someone asks NPC to sell item. NPC response maybe Yes or No, 
        /// they maybe bullied or charmed or awed or pity you to say yes
        /// they maybe provoked or paranoid or disgusted or prey on you to say no
        /// Strongest indicator will be primary, while secondary will influence tone
        /// 
        /// int[] personality_req refers to Big5 requirements when deciding how to respond to a request
        /// 
        /// </summary>
        /// <returns>
        /// Decision_NPC Returns a score of likelihood of decision, with biggest two negative and positive factors - that will influence tone
        /// </returns>
        public Decision_NPC Respond(int[] personality_req)
        {
            Decision_NPC treturn = new Decision_NPC();
            treturn.Init();

            //Check submissivness self vs admiration
            submissivness = admiration - self_belief;

            //Generate initial score based on personality


            //Score gets modified by Disposition

            //Score gets modified by emotion

            return treturn;
        }
    }
}