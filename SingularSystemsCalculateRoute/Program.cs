using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingularSystemsCalculateRoute
{
    class Program
    {
        static void Main(string[] args)
        {
            List<FlightPlan> routes = new List<FlightPlan>();
            List<Planet> objectsInSpaceList = GetUniverseDetails();

            objectsInSpaceList.ToList()
                .ForEach(x =>x.Distance = CalculateDistance(objectsInSpaceList[0].X, objectsInSpaceList[0].Y, objectsInSpaceList[0].Z, x.X, x.Y, x.Z)); //Populate the distance to every other planet in the universe from the current one
            var nearestObjects = objectsInSpaceList.OrderBy(x => x.Distance).ToList();  //.Where(x => x.Habbitable == true && x.Planet == true && x.Controlled == false)

            routes.Add(new FlightPlan() { Objects = new List<Planet>()});
            routes[0].Objects.Add(nearestObjects[0]); //Add homeworld as my starting point for my route



            //max travel time to the next destination that will be considered is less than 30 minutes (or 1800 seconds). 
            //Because in 20 minutes I could get passed a space monster (if it's my nearest neighbour) and add another 10 minutes to get to it's nearest neighbour (assuming it's a habbitable planet)... alot of time wasted!
            //for (int loop = 1; ; loop++) //loop starts at 1 as objectsInSpaceList[0] would be the planet I'm currently standing on as it's distance from us is 0
            //{//travelTime < 1800
            //    routeCounter = routes.Count;
            //    if (loop == 1)                
            //        travelTime = 10;                                    
            //    else
            //        travelTime = nearestObjects[loop].Distance / speed;

            //    if ( travelTime < 1800)
            //    {
            //        if(nearestObjects[loop].Monster == false && nearestObjects[loop].Habbitable == true)
            //        {

            //        }
            //        routeCounter++;

            //    }
            //}
            //double travelTime = 0;


            //for (int loop = 0; ; loop++)
            //{
            int loop = 0;
                using (var writer = new StreamWriter("..\\..\\..\\FlightPlan.txt"))
                {
                UpdatedMap map = new UpdatedMap();
                do
                {
                    map = FindClosestHabitablePlanet(nearestObjects, routes[loop]); //
                    routes[loop] = map.Route;
                    nearestObjects = map.Map;
                } while (map.EndIndicator == false);


                writer.WriteLine("**********************THE 24 HOUR COLONIZATION FLIGHT PLAN DEPARTING FROM HOME**********************");
                routes[0].Objects.Remove(routes[0].Objects[0]);//remove home planet
                    foreach (Planet item in routes[loop].Objects)
                    {
                        writer.WriteLine(
                        string.Format("X : {0} | Y : {1} | Z : {2} | Space on planet : {3} Million square kilometers",
                        item.X.ToString("0,0").PadLeft(11),
                        item.Y.ToString("0,0").PadLeft(11),
                        item.Z.ToString("0,0").PadLeft(11),
                        item.Space.ToString().PadLeft(3)
                        ));
                    }

                    writer.WriteLine(
                        string.Format(" Time : {0} hours, Colonized : {1} square kilometers, Colonized a total of {2} planets!!!",
                        (routes[loop].Time/3600),
                        routes[loop].TotalConquredSpace,
                        routes[loop].TotalPlanetsColonized
                        ));
                }
                
            //}
            
        }

        static UpdatedMap FindClosestHabitablePlanet(List<Planet> OrderedPlanetList, FlightPlan route)
        {            
            double speed = OrderedPlanetList[1].Distance / 600; //10 minutes to the immediate neighbour
            for (int loop = 1; ; loop++)
            {
                if ((OrderedPlanetList[loop].Monster == false) && (OrderedPlanetList[loop].Habbitable == true) && (OrderedPlanetList[loop].Colonized == false))
                {
                    UpdatedMap map = new UpdatedMap();
                    map.Route = ColonisePlanet(route, OrderedPlanetList[loop]);
                    map.Route.Time += (OrderedPlanetList[loop].Distance / speed);
                    if (map.Route.Time < 86400)
                    {
                        OrderedPlanetList.ForEach(c => c.Distance =
                        CalculateDistance(OrderedPlanetList[loop].X,
                        OrderedPlanetList[loop].Y,
                        OrderedPlanetList[loop].Z,
                        c.X,
                        c.Y,
                        c.Z));
                        map.Map = OrderedPlanetList;
                        map.Map[loop].Colonized = true;
                    }
                    else
                    {
                        map.Route.Time -= (OrderedPlanetList[loop].Distance / speed);
                        map.Route.Time -= map.Route.TimeTakenToColonizeLastPlanet;
                        map.EndIndicator = true;
                    }
                        

                    return map;
                }
                //else if (OrderedPlanetList[loop].Monster == true)
                //{

                //}
            }            
        }

        static FlightPlan ColonisePlanet(FlightPlan CurrentFlightPlan, Planet PlanetToColonise)
        {
            double timeTakenToColonize = (PlanetToColonise.Space/2 +1)* 0.043;            //***ASSUMPTION : I have taken the rate of colonization as 0.043 per MILLION square kilometers. I believe this keeps the question interesting 
            if ((CurrentFlightPlan.Time + timeTakenToColonize) < 86400)                    //as at 0.043 per sqkm it would take almost 6 hours to colonise 50% of the smallest 1 mil sqkm planet there by only allowing a maximum
            {                                                                             //of four of the smallest planets if they're near enough ***
                CurrentFlightPlan.Time += timeTakenToColonize;
                CurrentFlightPlan.TotalConquredSpace += PlanetToColonise.Space;
                CurrentFlightPlan.TotalPlanetsColonized++;
                CurrentFlightPlan.Objects.Add(PlanetToColonise);
            }
            else
                CurrentFlightPlan.TimeTakenToColonizeLastPlanet = timeTakenToColonize;

            return CurrentFlightPlan;
        }
        
        static double CalculateDistance(int fromPlanetX, int fromPlanetY, int fromPlanetZ, int toPlanetX, int toPlanetY, int toPlanetZ)
        {
            if ((fromPlanetX != toPlanetX) && (fromPlanetY != toPlanetY) && (fromPlanetZ != toPlanetZ)) //check that we're not trying to travel to the very planet we're on!
            {
                double distance = Math.Sqrt(
                Math.Pow(fromPlanetX - toPlanetX, 2)           //Pythagorean 
                + Math.Pow(fromPlanetY - toPlanetY, 2)
                + Math.Pow(fromPlanetZ - toPlanetZ, 2));

                return distance;
            }
            else
                return 0;
        }

        static List<Planet> GetUniverseDetails()
        {
            List<Planet> planetList = new List<Planet>();

            Planet homePlanet = new Planet(123123991, 098098111, 456456999,true,true,0,true); //Adding my home world

            planetList.Add(homePlanet);

            foreach (string line in File.ReadAllLines("..\\..\\..\\Universe.txt"))
            {
                string[] variables = line.Split(',');
                planetList.Add(new Planet( // Adding all the other objects in the universe
                    int.Parse(variables[0]),
                    int.Parse(variables[1]),
                    int.Parse(variables[2]),
                    bool.Parse(variables[3]),
                    bool.Parse(variables[4]),
                    int.Parse(variables[5]),
                    false));
            }
            return planetList;
        }

        public abstract class ObjectInSpace
        {
            
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }

            public double Distance { get; set; }            
        }


        public class Planet : ObjectInSpace
        {
            public Planet(int _X, int _Y, int _Z, bool _Habbitable, bool _Monster, int _Space, bool _Colonized)
            {
                this.X = _X;
                this.Y = _Y;
                this.Z = _Z;
                this.Habbitable = _Habbitable;
                this.Monster = _Monster;
                this.Space = _Space;
                this.Colonized = _Colonized;
            }
            public bool Habbitable { get; set; }
            public bool Monster { get; set; }
            public int Space { get; set; }
            public bool Colonized { get; set; }
        }

        public class FlightPlan
        {
            public List<Planet> Objects { get; set; }
            public double Time { get; set; }
            public int TotalPlanetsColonized { get; set; }
            public int TotalConquredSpace { get; set; }

            public double TimeTakenToColonizeLastPlanet { get; set; }
        }

        public class UpdatedMap
        {
            public List<Planet> Map { get; set; }
            public FlightPlan Route { get; set; }
            public bool EndIndicator { get; set; }
        }
    }
}
