using System;
using System.Collections.Generic;
using System.Text;
using FileManager;

namespace Hogwit_University_HW3 {
    internal class StudentLocater {
        static void Main(string[] args) {
            //First: Get contents of the files
            string file1 = "../../../courses.txt";
            string file2 = "../../../students.txt";

            string[] coursesInfo = new FileIO(file1).readDataInToStringList();
            string[] studentsInfo = new FileIO(file2).readDataInToStringList();

            //Second: Get student list
            Dictionary<string, Student> students = getStudentData(studentsInfo, coursesInfo);

            //Fourth: Get the student the user wants
            Student student = getUserStudent(students);

            //Fifth: Output student information
            outputStudent(student);

            Console.ReadLine();
        }

        private static void outputStudent(Student student) {
            Console.WriteLine(student.ToString());
        }

        private static Student getUserStudent(Dictionary<string, Student> students) {
            while (true) {
                Console.WriteLine("Enter a student ID to search for->");
                string userIDInput = Console.ReadLine();

                if (students.ContainsKey(userIDInput.ToUpper())) {
                    return students[userIDInput.ToUpper()];
                }
                Console.WriteLine("ID:{0} was not found. Please try again.", userIDInput.ToUpper());
            }
        }

        private static Dictionary<string, Student> getStudentData(string[] studentsInfo, string[] coursesInfo) {
            //Third: Get course dictionary
            Dictionary<string, Course> courses = getCourseData(coursesInfo);
            Dictionary<string, Student> students = new Dictionary<string, Student>();
            bool first = true;

            foreach (string student in studentsInfo) {
                if (first) {
                    first = false;
                } else {
                    string[] toks = student.Split(',');
                    string id = toks[0];
                    string year = toks[1];
                    string fName = toks[2];
                    string lName = toks[3];

                    //toks[4] is a list of semi-colon separated values, the method parses them
                    string coursesTaken = toks[4];
                    List<Course> complCourses = getCompletedCourses(coursesTaken, courses);

                    //Separates the students before and after 2022
                    //Creates their object and adds them to the dictionary
                    try {
                        if (int.Parse(year) < 2022) {
                            students.Add(id, new StudBefore22(id, fName, lName, year, complCourses));
                        } else if (int.Parse(year) >= 2022) {
                            students.Add(id, new StudAfter22(id, fName, lName, year, complCourses));
                        } else {
                            Console.WriteLine("Year:{0} has no match.", year);
                        }
                    } catch (Exception e) {
                        Console.WriteLine("Year Parse Problem:{0}", e.Message);
                    }               
                }
            }
            return students;
        }

        private static List<Course> getCompletedCourses(string coursesTaken, Dictionary<string, Course> courses) {
            List<Course> complCourses = new List<Course>();
            string[] toks = coursesTaken.Split(';');

            foreach (string tok in toks) {
                if (courses.ContainsKey(tok)) {
                    complCourses.Add(courses[tok]);
                } else {
                    Console.WriteLine("Course:{0} is not in the text doc", tok);
                }
            }
            return complCourses;
        }

        private static Dictionary<string, Course> getCourseData(string[] coursesInfo) {
            Dictionary<string, Course> courses = new Dictionary<string, Course>();
            bool first = true;

            foreach (string course in coursesInfo) {
                if (first) {
                    first = false;
                } else {
                    string[] toks = course.Split(',');
                    string id = toks[0];
                    string topic = toks[1];
                    string desc = toks[2];
                    string type = toks[3];
                    courses.Add(id, new Course(id, topic, desc, type));
                }
            }
            return courses;
        }
    }

    /// Abstract Student Class
    /// Takes in the student's id, first and last name, year, and a list of courses they completed
    /// Separates the required from the elective course in the constructor
    /// Has two abstract methods to find the number of elective and required courses to graduate
    /// Encapsulates attributes
    /// Overrides toString method
    public abstract class Student {
        private string id;
        private string fName;
        private string lName;
        private string year;
        private List<Course> reqCompl;
        private List<Course> eleCompl;

        public Student(string id, string fName, string lName, string year, List<Course> courses) {
            Id = id;
            FName = fName;
            LName = lName;
            Year = year;
            ReqCompl = new List<Course>();
            EleCompl = new List<Course>();
            
            foreach(Course course in courses) {
                if (course.CourseType.Equals("Required")) {
                    ReqCompl.Add(course);
                } else if (course.CourseType.Equals("Elective")) {
                    EleCompl.Add(course);
                } else {
                    Console.WriteLine("Bad Input Course:{0}", course.ToString());
                }
            }
        }

        public abstract int getNumReqToGrad();

        public abstract int getNumElectToGrad();

        public string Id {
            get { return id; }
            set { id = value; }
        }

        public string FName {
            get { return fName; }
            set { fName = value; }
        }

        public string LName {
            get { return lName; }
            set { lName = value; }
        }

        public string Year {
            get { return year; }
            set { year = value; }
        }

        public List<Course> ReqCompl {
            get { return reqCompl; }
            set { reqCompl = value; }
        }

        public List<Course> EleCompl {
            get { return eleCompl; }
            set { eleCompl = value; }
        }

        public override string ToString() {
            StringBuilder reqStr = new StringBuilder();
            StringBuilder eleStr = new StringBuilder();

            foreach (Course reqCourse in reqCompl) {
                reqStr.Append("\n-----REQUIRED COURSE-----" + reqCourse.ToString());
            }

            foreach (Course eleCourse in eleCompl) {
                eleStr.Append("\n-----ELECTIVE COURSE-----" + eleCourse.ToString());
            }

            return string.Format("Id:{0}\tStart Year:{1}\tFName:{2}\tLName:{3}\tReq:{4}\tEle:{5}{6}{7}",
                id, year, fName, lName, reqCompl.Count, eleCompl.Count, reqStr, eleStr);
        }
    }

    /// StudBefore22 Class inherits Student abstract class
    /// Completes the two abstract methods from the Student parent
    /// Required courses to graduate is 8 with 2 elective courses needed as well
    /// Overrides toString method by adding onto Student parent
    public class StudBefore22 : Student {
        public StudBefore22(string id, string fName, string lName, string year, List<Course> courses) 
            : base(id, fName, lName, year, courses) {}

        public override int getNumReqToGrad() {
            const int REQ_TO_GRAD = 8;
            if (ReqCompl.Count < REQ_TO_GRAD) {
                return REQ_TO_GRAD - ReqCompl.Count;
            } else if (ReqCompl.Count >= REQ_TO_GRAD) {
                return 0;
            } else {
                return REQ_TO_GRAD;
            }
        }

        public override int getNumElectToGrad() {
            const int ELECT_TO_GRAD = 2;
            if (EleCompl.Count < ELECT_TO_GRAD) {
                return ELECT_TO_GRAD - EleCompl.Count;
            } else if (EleCompl.Count >= ELECT_TO_GRAD) {
                return 0;
            } else {
                return ELECT_TO_GRAD;
            }
        }

        public override string ToString() {
            return base.ToString() + string.Format("\n-----GRADUATE READY?-----StudentV1:{0}\tRequired Needed:{1}" +
                "\tElectives Needed:{2}", Id, getNumReqToGrad(), getNumElectToGrad());
        }
    }

    /// StudAfter22 Class does everything StudBefore22 class does
    /// Only difference is that the required courses to graduate is 10
    /// 4 elective courses are needed to graduate as well
    public class StudAfter22 : Student {
        public StudAfter22(string id, string fName, string lName, string year, List<Course> courses) 
            : base(id, fName, lName, year, courses) {}

        public override int getNumReqToGrad() {
            const int REQ_TO_GRAD = 10;
            if (ReqCompl.Count < REQ_TO_GRAD) {
                return REQ_TO_GRAD - ReqCompl.Count;
            } else if (ReqCompl.Count >= REQ_TO_GRAD) {
                return 0;
            } else {
                return REQ_TO_GRAD;
            }
        }

        public override int getNumElectToGrad() {
            const int ELECT_TO_GRAD = 4;
            if (EleCompl.Count < ELECT_TO_GRAD) {
                return ELECT_TO_GRAD - EleCompl.Count;
            } else if (EleCompl.Count >= ELECT_TO_GRAD) {
                return 0;
            } else {
                return ELECT_TO_GRAD;
            }
        }

        public override string ToString() {
            return base.ToString() + string.Format("\n-----GRADUATE READY?-----StudentV2:{0}\tRequired Needed:{1}" +
                "\tElectives Needed:{2}", Id, getNumReqToGrad(), getNumElectToGrad());
        }
    }

    /// Course class
    /// Takes in id, topic, description, and the coure type
    /// Encapsulates attributes
    /// Overrides toString method
    public class Course {
        private string id;
        private string topic;
        private string description;
        private string courseType;

        public Course (string id, string topic, string description, string courseType) {
            Id = id;
            Topic = topic;
            Description = description;
            CourseType = courseType;
        }

        public override string ToString() {
            return string.Format("Course:{0} | Topic:{1} | Desc:{2} | Type:{3}", id, topic, description, courseType);
        }

        public string Id {
            get { return id; }
            set { id = value; }
        }

        public string Topic {
            get { return topic; }
            set { topic = value; }
        }

        public string Description {
            get { return description; }
            set { description = value; }
        }

        public string CourseType {
            get { return courseType; }
            set { courseType = value; }
        }
    }
}
