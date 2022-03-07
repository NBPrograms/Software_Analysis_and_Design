using System;
using System.Collections.Generic;
using System.Text;

namespace MT_Honest_Auto_Lot {
    internal class CarLot {
        static void Main(string[] args) {
            //First: Read in the cars
            List<Car> cars = getTheCars();

            //Second: Output the cars
            outputCars(cars);

            Console.ReadLine();
        }

        private static void outputCars(List<Car> cars) {
            foreach (Car car in cars) {
                Console.WriteLine(car.ToString() + "\n");
            }
        }

        private static List<Car> getTheCars() {
            //Initialization
            Dictionary<string, List<Option>> options = getCarOptions();
            List<Car> cars = new List<Car>();

            //Adding cars
            cars.Add(new Car("GMC", "Sierra", 2022, 44000, options["Sierra"]));
            cars.Add(new Car("Chevy", "Blazer", 2022, 25000, options["Blazer"]));
            cars.Add(new Car("Buick", "Envision", 2022, 35600, options["Envision"]));

            return cars;
        }

        private static Dictionary<string, List<Option>> getCarOptions() {
            //Intialization
            Dictionary<string, List<Option>> options = new Dictionary<string, List<Option>>();
            List<Option> sierraOpts = new List<Option>();
            List<Option> blazerOpts = new List<Option>();
            List<Option> envisionOpts = new List<Option>();

            //Sierra
            sierraOpts.Add(new Option("Sensor Pack", 1000.00, "Factory"));
            sierraOpts.Add(new Option("Sport Package", 1500.00, "Factory"));
            sierraOpts.Add(new Option("Remote Starter", 1499.99, "After Market"));

            //Blazer
            blazerOpts.Add(new Option("Rust Prevention", 999.00, "Weird Al’s Auto Shop"));

            //Envision
            envisionOpts.Add(new Option("Sensor Pack", 1000.00, "Factory"));
            envisionOpts.Add(new Option("Remote Starter", 1499.99, "After Market"));
            
            //Add to dictionary
            options.Add("Sierra", sierraOpts);
            options.Add("Blazer", blazerOpts);
            options.Add("Envision", envisionOpts);

            return options;
        }
    }

    /// Car class
    /// Takes in the make, model, year, price, and its list of options
    /// Encapsulated variables with capital letters
    /// Adjusts for bad input in the year and price attributes
    /// Overrides toString
    public class Car {
        private string make;
        private string model;
        private int year;
        private double price;
        private List<Option> options;

        public Car(string make, string model, int year, double price, List<Option> options) {
            Make = make;
            Model = model;
            Year = year;
            Price = price;
            Options = options;
        }

        public string Make {
            get { return make; }
            set { make = value; }
        }

        public string Model {
            get { return model; }
            set { model = value; }
        }

        public int Year {
            get { return year;}
            set {
                if (value < 0) {
                    year = 0;
                } else if (value >= 0) {
                    year = value;
                } else {
                    Console.WriteLine("Error-Bad value:{0}", value);
                }
            }
        }

        private double Price {
            get { return price; }
            set {
                if (value < 0) {
                    price = 0;
                } else if (value >= 0) {
                    price = value;
                } else {
                    Console.WriteLine("Error-Bad value:{0}", value);
                }
            }
        }

        public List<Option> Options {
            get { return options; }
            set { options = value; }
        }

        public override string ToString() {
            StringBuilder optionStr = new StringBuilder();

            foreach (Option option in options) {
                optionStr.Append(string.Format("\n-----{0}", option.ToString()));
            }

            return string.Format("Make:{0}\tModel:{1}\tYear:{2}\tPrice:${3:0.00}{4}",
                make, model, year, price, optionStr);
        }
    }

    /// Option class
    /// Takes in option name, price, and type
    /// Encapsulated variables with capital letters
    /// Adjusts for bad input in the price and type attributes
    /// Overrides toString
    public class Option {
        private string name;
        private double price;
        private string type;

        public Option(string name, double price, string type) {
            Name = name;
            Price = price;
            Type = type;
        }

        public string Name {
            get { return name; }
            set { name = value; }
        }

        public double Price {
            get { return price; }
            set {
                if (value < 0) {
                    price = 0;
                } else if (value >= 0) {
                    price = value;
                } else {
                    Console.WriteLine("Error-Bad value:{0}", value);
                }
            }
        }

        public string Type {
            get { return type; }
            set {
                if (value.ToLower().Equals("factory") || value.ToLower().Equals("after market")) {
                    type = value;
                } else {
                    type = "Unknown";
                }
            }
        }

        public override string ToString() {
            return string.Format("Option:{0}\tCost:${1:0.00}\tType:{2}", name, price, type);
        }
    }
}
