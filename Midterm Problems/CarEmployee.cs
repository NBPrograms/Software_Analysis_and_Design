using System;
using System.Collections.Generic;

namespace MT_Honest_Auto_Salary {
    internal class CarEmployee {
        static void Main(string[] args) {
            //First: Get the Employee Data
            List<Employee> employees = getTheEmployeeData();

            //Second: Output the employees
            outputEmployees(employees);

            Console.ReadLine();
        }

        private static void outputEmployees(List<Employee> employees) {
            Console.WriteLine("--------------SALES--------------");
            foreach (Employee emp in employees) {
                Console.WriteLine(emp.ToString());
            }
        }

        private static List<Employee> getTheEmployeeData() {
            //Initialization
            List<Employee> employees = new List<Employee>();

            //Adding employees
            employees.Add(new FullEmployee("Sally", "Full Time", 10, 40000));
            employees.Add(new FullEmployee("Sal", "Full Time", 9, 40000));
            employees.Add(new FullEmployee("Silvia", "Full Time", 9, 100000));
            employees.Add(new PartEmployee("Sammie", "Part Time", 0, 10000));

            return employees;
        }
    }

    /// Employee abstract class
    /// Takes in their name, type, experience, and total sales
    /// Has abstract method that determines their sales commission
    /// Enscapsulates variables with capital letters
    /// Adjusts for input in the experience and total sales attributes
    /// Overrides toString
    public abstract class Employee {
        private string name;
        private string type;
        private int yearXP;
        private double totalSales;

        protected Employee(string name, string type, int yearXP, double totalSales) {
            Name = name;
            Type = type;
            YearXP = yearXP;
            TotalSales = totalSales;
        }

        public abstract double getSalesCommission();

        public string Name {
            get { return name; }
            set { name = value; }
        }

        public string Type {
            get { return type; }
            set { type = value; }
        }

        public int YearXP {
            get { return yearXP; }
            set {
                if (value < 0) {
                    yearXP = 0;
                } else if (value >= 0) {
                    yearXP = value;
                } else {
                    Console.WriteLine("Error-Bad Input:{0}", value);
                }
            }
        }

        public double TotalSales {
            get { return totalSales; }
            set {
                if (value < 0) {
                    totalSales = 0;
                } else if (value >= 0) {
                    totalSales = value;
                } else {
                    Console.WriteLine("Error-Bad Input:{0}", value);
                }
            }
        }

        public override string ToString() {
            return string.Format("Name:{0}\tSales Type:{1}\tSales Amount:${2}\tCommission:${3}",
                name, type, totalSales, getSalesCommission());
        }
    }

    /// FullEmployee Class
    /// Extends the Employee Class
    /// If yearXP >= 10, get a 6% commission
    /// Otherwise gets a 5% commission
    /// Adjusts for invalid values
    public class FullEmployee : Employee {
        public FullEmployee(string name, string type, int yearXP, double totalSales) 
            : base(name, type, yearXP, totalSales) {
        }

        public override double getSalesCommission() {
            const double COM_RATE = .05;
            const double XP_RATE = .06;

            if (YearXP >= 10) {
                return TotalSales * XP_RATE;
            } else if (YearXP >= 0) {
                return TotalSales * COM_RATE;
            } else {
                return 0;
            }
        }
    }

    /// PartEmployee Class
    /// Extends the Employee Class
    /// Gets a 4% commission
    /// Adjusts for invalid values
    public class PartEmployee : Employee {
        public PartEmployee(string name, string type, int yearXP, double totalSales) 
            : base(name, type, yearXP, totalSales) {
        }

        public override double getSalesCommission() {
            const double COM_RATE = .04;

            if (YearXP >= 0) {
                return TotalSales * COM_RATE;
            } else {
                return 0;
            }
        }
    }
}
