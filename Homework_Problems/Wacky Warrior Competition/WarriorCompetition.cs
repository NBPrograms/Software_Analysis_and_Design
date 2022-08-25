using System;
using System.Collections.Generic;
using FileManager;

namespace WackyWarriorsV3_HW6 {
    internal class WarriorCompetition {
        static void Main(string[] args) {
            string fname = "../../../Warrior.txt";
            string[] filelines = new FileIO(fname).readDataInToStringList();
            Dictionary<string, List<EventAthlete>> events = parseFileIntoEventDictionary(filelines);
            findContestWinners(events);
            Console.ReadLine();
        }

        private static void findContestWinners(Dictionary<string, List<EventAthlete>> events) {
            LeaderBoard leaders = new LeaderBoard();
            foreach (string competition in events.Keys) {
                Console.WriteLine("=========Event:{0}========", competition);
                List<EventAthlete> athletes = events[competition];
                EventResults results = getEventResults(competition, athletes);
                List<EventAthlete> winners = results.getWinners();
                leaders.updateStandings(winners);
                outputEventLeaders(winners);
            }
            leaders.outputLeaderboard();
        }

        private static void outputEventLeaders(List<EventAthlete> winners) {
            foreach (EventAthlete w in winners) {
                Console.WriteLine(w.ToString());
            }
        }

        private static EventResults getEventResults(string competition, List<EventAthlete> athletes) {
            EventResults results = new EventResults();
            if (competition.ToUpper().Equals("HURDLE")) {
                results = new EventResults(athletes, new LowScore());
            } else {
                results = new EventResults(athletes, new HighScore());
            }
            return results;
        }

        private static Dictionary<string, List<EventAthlete>> parseFileIntoEventDictionary(string[] filelines) {
            Dictionary<string, List<EventAthlete>> events = new Dictionary<string, List<EventAthlete>>();
            foreach (string line in filelines) {
                string[] toks = line.Split(',');
                if (!toks[0].ToUpper().Equals("HEADER")) {
                    if (!events.ContainsKey(toks[0])) {
                        List<EventAthlete> athletes = new List<EventAthlete>();
                        addToAthleteList(toks, athletes);
                        events.Add(toks[0], athletes);
                    } else {
                        List<EventAthlete> athletes = events[toks[0]];
                        addToAthleteList(toks, athletes);
                    }
                }
            }
            return events;
        }

        private static void addToAthleteList(string[] toks, List<EventAthlete> athletes) {
            Athlete athlete = new Athlete(toks[1], toks[2], toks[3]);
            if (toks[0].ToUpper().Equals("HURDLE")) {
                //Because of how the file is formatted (10:00), hurdleTime.TotalHours returns 10, the correct figure
                //Time is actually in seconds
                TimeSpan hurdleTime = TimeSpan.Parse(toks[4]);
                int time = (int) Math.Ceiling(hurdleTime.TotalHours);
                athletes.Add(new HurdleAthlete(toks[0], athlete, time, int.Parse(toks[5])));
            } else if (toks[0].ToUpper().Equals("PUMPKIN")) {
                athletes.Add(new PumpkinAthlete(toks[0], athlete, int.Parse(toks[4])));
            } else if (toks[0].ToUpper().Equals("CORN HOLE")) {
                athletes.Add(new CornHoleAthlete(toks[0], athlete, int.Parse(toks[4]), int.Parse(toks[5])));
            } else {
                Console.WriteLine("We do not recognize this event!");
            }
        }
    }

    public class LeaderBoard {
        private Dictionary<string, int> winnerScores = new Dictionary<string, int>();

        //The winners list is expected to be sorted list based on finish (1st place is index 0)
        //The winners list should only include athletes that will actually get points for an event
        public void updateStandings(List<EventAthlete> winners) {
            for(int placementScore = winners.Count; placementScore > 0; placementScore--) {
                string athleteName = findAthleteName(winners, placementScore);
                if (!winnerScores.ContainsKey(athleteName)) {
                    winnerScores.Add(athleteName, placementScore);
                } else {
                    winnerScores[athleteName] += placementScore;
                }
            }
        }

        private string findAthleteName(List<EventAthlete> winners, int placementScore) {
            //To match the athlete with the correct score (higher score means better finish)
            //The index is found by subtracting the number of athletes by the placement score
            int index = winners.Count - placementScore;
            EventAthlete winner = winners[index];
            Athlete athlete = winner.getAthlete();
            return athlete.Name;
        }

        public void outputLeaderboard() {
            List<int> sortedScores = sortLeaderboard();
            Console.WriteLine("=========Overall Standings========");
            foreach (string athlete in winnerScores.Keys) {
                if (winnerScores[athlete] == sortedScores[0]) {
                    Console.WriteLine("1st Place - Athlete:{0} | Score:{1}", athlete, winnerScores[athlete]);
                } else if (winnerScores[athlete] == sortedScores[1]) {
                    Console.WriteLine("2nd Place - Athlete:{0} | Score:{1}", athlete, winnerScores[athlete]);
                } else if (winnerScores[athlete] == sortedScores[2]) {
                    Console.WriteLine("3rd Place - Athlete:{0} | Score:{1}", athlete, winnerScores[athlete]);
                }
            }
        }

        private List<int> sortLeaderboard() {
            List<int> scores = getScoreList();
            sortScores(scores);
            scores.Reverse();
            getTopScorers(scores);
            return scores;
        }

        private List<int> getScoreList() {
            List<int> scores = new List<int>();
            foreach (int score in winnerScores.Values) {
                scores.Add(score);
            }
            return scores;
        }

        private void sortScores(List<int> scores) {
            scores.Sort(delegate (int s1, int s2) {
                return s1.CompareTo(s2);
            });
        }

        private void getTopScorers(List<int> scores) {
            const int NUM_PODIUM_POSITIONS = 3;
            scores.RemoveRange(NUM_PODIUM_POSITIONS , scores.Count - NUM_PODIUM_POSITIONS);
        }
    }

    public class EventResults {
        private List<EventAthlete> athletes;
        private ScoringType eventResults;

        public EventResults() { }

        public EventResults(List<EventAthlete> athletes, ScoringType eventResults) {
            this.athletes = athletes;
            this.eventResults = eventResults;
        }

        public List<EventAthlete> getWinners() {
            eventResults.getEventLeaderBoard(athletes);
            return athletes;
        }
    }

    public abstract class ScoringType {
        private const int NUM_OF_POINT_POSITIONS = 3;

        public abstract void getEventLeaderBoard(List<EventAthlete> athletes);

        protected void sortAthletes(List<EventAthlete> athletes) {
            athletes.Sort(delegate (EventAthlete a1, EventAthlete a2) {
                return a1.score().CompareTo(a2.score());
            });
        }

        protected void getTopPlacers(List<EventAthlete> athletes) {
            athletes.RemoveRange(NUM_OF_POINT_POSITIONS, athletes.Count - NUM_OF_POINT_POSITIONS);
        }
    }

    public class LowScore : ScoringType {
        public override void getEventLeaderBoard(List<EventAthlete> athletes) {
            sortAthletes(athletes);
            getTopPlacers(athletes);
        }
    }

    public class HighScore : ScoringType {
        public override void getEventLeaderBoard(List<EventAthlete> athletes) {
            sortAthletes(athletes);
            athletes.Reverse();
            getTopPlacers(athletes);
        }
    }

    public abstract class EventAthlete {
        private string eventName;
        private Athlete athlete;

        public EventAthlete(string eventName, Athlete athlete) {
            this.eventName = eventName;
            this.athlete = athlete;
        }

        public Athlete getAthlete() {
            return athlete;
        }

        public abstract int score();

        public override string ToString() {
            return string.Format("Event:{0} | {1} | Score:{2}", eventName, athlete.ToString(), score());
        }
    }

    public class HurdleAthlete : EventAthlete {
        private int time;
        private int knockedOver;
        private const int MULTIPLIER_FOR_KNOCKED_OVER_HURDLES = 5;

        public HurdleAthlete(string eventName, Athlete athlete, int time, int knockedOver) 
            : base(eventName, athlete) {
            this.time = time;
            this.knockedOver = knockedOver;
        }

        public override int score() {
            return time + knockedOver * MULTIPLIER_FOR_KNOCKED_OVER_HURDLES;
        }
    }

    public class PumpkinAthlete : EventAthlete {
        private int distance;

        public PumpkinAthlete(string eventName, Athlete athlete, int distance) 
            : base(eventName, athlete) {
            this.distance = distance;
        }

        public override int score() {
            return distance;
        }
    }

    public class CornHoleAthlete : EventAthlete {
        private int countOneFootOfHole;
        private int countInHole;
        private const int IN_HOLE_MULTIPLIER = 2;

        public CornHoleAthlete(string eventName, Athlete athlete, int countOneFootOfHole, int countInHole) 
            : base(eventName, athlete) {
            this.countOneFootOfHole = countOneFootOfHole;
            this.countInHole = countInHole;
        }

        public override int score() {
            return countOneFootOfHole + countInHole * IN_HOLE_MULTIPLIER;
        }
    }

    public class Athlete {
        private string name;
        private string age;
        private string gender;

        public Athlete(string name, string age, string gender) {
            Name = name;
            Age = age;
            Gender = gender;
        }

        public string Name {
            get { return name; }
            set { name = value; }
        }

        public string Age {
            get { return age; }
            set { age = value; }
        }

        public string Gender {
            get { return gender; }
            set { gender = value; }
        }

        public override string ToString() {
            return string.Format("Name:{0} | Age:{1} | Gender:{2}", name, Age, gender);
        }
    }
}
