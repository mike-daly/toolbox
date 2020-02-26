/*
 * Suitability as interview question?
 * 
 * One calibration point.  It took about 20 minutes of on and off thinking to get the basic algorithm
 * of creating lists of overlapping sensors.  There are two "aha!" moments to address:
 *      1) merge two lists of overlapping sensors into one (and evaluating)
 *      2) joining a new sensor to a list is not just about evaluating against the last
 *      sensor in the list, but needs to be against all sensors that may "reach" that far and overlap
 *      this tends to push the problem to an n^2 search of the existing lists.  Not nice.
 *      
 * I didn't identify the second interesting edge case (above) until after the first implementation.
 * 
 * The first (incomplete) implementation took 90 minutes of iterative implementation including test
 * cases.  Such implementation technique does not work on a whiteboard since there is lots of refactoring  
 * to get from evaluating sensors, to evaluating sensors against a single list, to handling multiple
 * lists.  The refactoring is not bad with a development environment but would not work with whiteboard.
 * 
 * Added "DeepCheck()" code to address the item 2 above.  DeepCheck() looks all of the way back until it
 * hits the end of the set, or it overlaps, or it has gone back the width of the hall.  This is an optimization
 * possible because if there is a sensor with a radius == to the width of the hall, that will make the
 * hall impassable.  a new sensor cannot overlap with a sensor farther back than the width.
 */



using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace hallway_sensors
{
    class Program
    {
        /*
         * Sort the sensors and validate them.  Any sensor beyond the start or end 
         * of the hallway is invalid.  Any sensor outside the right/left boundaries
         * of the hallway is invalid.  Invalid sensors throw exceptions.
         * once the sensors are validated as within the length of the hall, the length is ignored.
         * 
         * The sensors are ordered along the hallway (by x).
         * 
         * The basic algorithm is to collect the sensors into one or more connected chains.
         * for each sensor in the input
         *  attempt to add to existing sensor sets
         *      (if the new sensor overlaps with the tail of the existing set)
         *  if it is added to two (or more), join the sets
         *  if it is not added to any existing sensor sets, create a new set
         *  
         * The evaluation terminates when:
         *  adding a sensor to a sensor set makes the set "touch" both walls (return false)
         *  last sensor is added (return true)
         */
        static void Main(string[] args)
        {
            const int length = 100;
            const int width = 10;

            TestCase_1(length, width);
            TestCase_2(length, width);
            TestCase_3(length, width);
            TestCase_4(length, width);

            if (Debugger.IsAttached)
            {
                Console.WriteLine("done, press any key to exit.");
                Console.ReadKey();
            }
        }

        private static void TestCase_1(int length, int width)
        {
            List<Sensor> sensors = new List<Sensor>();
            sensors.Add(new Sensor() { x = 5, y = 3, r = 6 });
            sensors.Add(new Sensor() { x = 15, y = 5, r = 3 });
            Console.WriteLine($"TestCase_1 passable?  {Program.Evaluate(length, width, (List<Sensor>)sensors.OrderBy(s => s.x).ToList())}");
        }

        private static void TestCase_2(int length, int width)
        {
            List<Sensor> sensors = new List<Sensor>();
            sensors.Add(new Sensor() { x = 5, y = 0, r = 2 });
            sensors.Add(new Sensor() { x = 6, y = 0, r = 2 });
            sensors.Add(new Sensor() { x = 7, y = 0, r = 2 });
            sensors.Add(new Sensor() { x = 8, y = 0, r = 2 });
            sensors.Add(new Sensor() { x = 9, y = 0, r = 2 });
            Console.WriteLine($"TestCase_2 passable?  {Program.Evaluate(length, width, (List<Sensor>)sensors.OrderBy(s => s.x).ToList())}");
        }

        private static void TestCase_3(int length, int width)
        {
            List<Sensor> sensors = new List<Sensor>();
            sensors.Add(new Sensor() { x = 5, y = 0, r = 3 });
            sensors.Add(new Sensor() { x = 6, y = 2, r = 3 });
            sensors.Add(new Sensor() { x = 7, y = 4, r = 3 });
            sensors.Add(new Sensor() { x = 8, y = 6, r = 3 });
            sensors.Add(new Sensor() { x = 9, y = 8, r = 3 });
            Console.WriteLine($"TestCase_3 passable?  {Program.Evaluate(length, width, (List<Sensor>)sensors.OrderBy(s => s.x).ToList())}");
        }

        private static void TestCase_4(int length, int width)
        {
            List<Sensor> sensors = new List<Sensor>();
            sensors.Add(new Sensor() { x = 5, y = 0, r = 2 });
            sensors.Add(new Sensor() { x = 6, y = 8, r = 2 });
            sensors.Add(new Sensor() { x = 7, y = 2, r = 2 });
            sensors.Add(new Sensor() { x = 8, y = 6, r = 2 });
            sensors.Add(new Sensor() { x = 9, y = 4, r = 2 });
            Console.WriteLine($"TestCase_4 passable?  {Program.Evaluate(length, width, (List<Sensor>)sensors.OrderBy(s => s.x).ToList())}");
        }

        private static object Evaluate(int length, int width, List<Sensor> sensors)
        {
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width));
            foreach (Sensor s in sensors) if (!s.IsValid(length, width)) throw new ArgumentOutOfRangeException(nameof(sensors), $"{s} out of range");

            List<SensorSet> allSets = new List<SensorSet>();

            foreach (Sensor s in sensors)
            {
                SensorSet firstHit = null;

                // it would be better to just remove inactive sensor sets, save it for version two.
                foreach (SensorSet set in allSets.Where(ss => ss.active))
                {
                    if (set.CanAdd(s) || set.DeepCheck(s))
                    {
                        if (firstHit == null)
                        {
                            set.AddSensor(s);
                            if (!set.IsPassable())
                            {
                                Console.WriteLine("merged set impassable");
                                return false;
                            }

                            firstHit = set;
                        }
                        else
                        {
                            // this is the second (or subsequent) add for this sensor,
                            // move the sensors from this set to the previous set and reorder
                            // BUGBUG -- remove the second sensor set.  little tricky to avoid
                            //      immutable list errors in the scope of foreach()
                            Console.WriteLine($"merged SensorSets for {s}");
                            foreach (Sensor secondSensor in set.sensors)
                            {
                                firstHit.AddSensor(secondSensor);
                                set.active = false;
                            }
                            firstHit.sensors = (List<Sensor>)firstHit.sensors.OrderBy(i => i.x).ToList();

                            if (!firstHit.IsPassable())
                            {
                                Console.WriteLine("merged set impassable");
                                return false;
                            }
                        }
                    }
                }

                if (firstHit == null)
                {
                    SensorSet newSet = new SensorSet(width);
                    Console.WriteLine($"added new SensorSet() for {s}");
                    newSet.AddSensor(s);
                    if (!newSet.IsPassable())
                    {
                        return false;
                    }
                    allSets.Add(newSet);
                }
            }
            return true;
        }
    }

    class Sensor
    {
        public int x { get; set; }
        public int y { get; set; }
        public int r { get; set; }

        public bool IsValid(int length, int width)
        {
            if (x < 0 || x > length) return false;
            if (y < 0 || y > width) return false;
            if (r < 0) return false;
            return true;
        }

        public bool Overlap(Sensor s)
        {
            int r = this.r + s.r;
            int diffx = Math.Abs(this.x - s.x);
            int diffy = Math.Abs(this.y - s.y);

            if ((diffx * diffx + diffy * diffy) > r * r)
            {
                Console.WriteLine($"{s} does not overlap {this}");
                return false;
            }
            Console.WriteLine($"{s} does overlap {this}");
            return true;
        }


        public override string ToString()
        {
            return $"{this.x}, {this.y}, {this.r}";
        }
    }

    class SensorSet
    {
        public List<Sensor> sensors { get; set; }
        public bool hitRight { get; set; }
        public bool hitLeft { get; set; }
        public int width { get; private set; }
        public bool active { get; set; }

        public SensorSet(int width)
        {
            this.sensors = new List<Sensor>();
            this.hitLeft = false;
            this.hitRight = false;
            this.width = width;
            this.active = true;
        }

        public bool DeepCheck(Sensor s)
        {
            bool retval = false;

            int i = 0;
            foreach (Sensor sensor in this.sensors)
            {
                i++;
                if (sensor.Overlap(s))
                {
                    retval = true;
                    break;
                }

                // only need to look back the width of the hall
                if ((s.x - sensor.x) > width)
                {
                    retval = false;
                    break;
                }
            }


            Console.WriteLine($"{retval} = DeepCheck() -- depth:  {i}");
            return retval; 
        }


        public bool CanAdd(Sensor s)
        {
            if (this.sensors.Count == 0)
            {
                return true;
            }

            Sensor lastSensor = this.sensors.Last();
            if (lastSensor.Overlap(s))
            {
                return true;
            }
            return false;
        }

        public bool AddSensor(Sensor s)
        {
            if (s.y - s.r < 0)
            {
                this.hitRight = true;
                ////Console.WriteLine($"right side blocked by {s}");
            }
            if (s.y + s.r >= width)
            {
                this.hitLeft = true;
                ////Console.WriteLine($"left side blocked at {s}");
            }

            this.sensors.Add(s);
            ////Console.WriteLine($"::AddSensor() {this}");

            return false;
        }

        public bool IsPassable()
        {
            return !(this.hitLeft && this.hitRight);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"SensorSet::  IsPassable():  {this.IsPassable()}, hitRight:  {this.hitRight}, hitLeft: {this.hitLeft}, count:  {this.sensors.Count}, active:  {this.active}");
            return sb.ToString();
        }

    }
}