using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {

        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var query =
                from co in db.Courses
                where co.Subject == subject && co.Number == num
                join cl in db.Classes on co.CourseId equals cl.CourseId
                where cl.Season == season && cl.Year == year
                join en in db.Enrolleds on cl.ClassId equals en.ClassId
                join st in db.Students on en.StudentId equals st.UId
                select new
                {
                    fname = st.FName,
                    lname = st.LName,
                    uid = st.UId,
                    dob = st.Dob,
                    grade = en.Grade
                };

            return Json(query.ToArray());  
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
                var query =
                    from co in db.Courses
                    where co.Subject == subject && co.Number == num
                    join cl in db.Classes on co.CourseId equals cl.CourseId
                    where cl.Season == season && cl.Year == year
                    join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                    where category == null || ac.Name == category
                    join a in db.Assignments on ac.CategoryId equals a.CategoryId
                    select new
                    {
                        aname = a.Name,
                        cname = ac.Name,
                        due = a.Due,
                        submissions = db.Submissions.Count(s => s.AssignmentId == a.AssignmentId)
                    };

                return Json(query.ToArray());  
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var query =
                from co in db.Courses
                where co.Subject == subject && co.Number == num
                join cl in db.Classes on co.CourseId equals cl.CourseId
                where cl.Season == season && cl.Year == year
                join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                select new
                {
                    name = ac.Name,
                    weight = ac.Weight
                };

            return Json(query.ToArray());        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            var cl = (from co in db.Courses
                      where co.Subject == subject && co.Number == num
                      join c in db.Classes on co.CourseId equals c.CourseId
                      where c.Season == season && c.Year == year
                      select c).SingleOrDefault();

            if (cl == null)
            {
                return Json(new { success = false });
            }

            bool exists = db.AssignmentCategories.Any(ac => ac.ClassId == cl.ClassId && ac.Name == category);

            if (exists)
            {
                return Json(new { success = false });
            }

            db.AssignmentCategories.Add(new AssignmentCategory
            {
                Name = category,
                Weight = (uint)catweight,
                ClassId = cl.ClassId
            });

            db.SaveChanges();
            return Json(new { success = true });    
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            var cat = (from co in db.Courses
                       where co.Subject == subject && co.Number == num
                       join cl in db.Classes on co.CourseId equals cl.CourseId
                       where cl.Season == season && cl.Year == year
                       join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                       where ac.Name == category
                       select new { ac.CategoryId, cl.ClassId }).SingleOrDefault();

            if (cat == null)
            {
                return Json(new { success = false });
            }

            db.Assignments.Add(new Assignment
            {
                Name = asgname,
                MaxPoints = (uint)asgpoints,
                Due = asgdue,
                Content = asgcontents,
                CategoryId = cat.CategoryId
            });

            db.SaveChanges();
            
            var enrollments = db.Enrolleds
                .Where(e => e.ClassId == cat.ClassId).ToList();

            foreach(var en in enrollments)
                UpdateGrade(en.StudentId, cat.ClassId);

            return Json(new { success = true });
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var query =
                from co in db.Courses
                where co.Subject == subject && co.Number == num
                join cl in db.Classes on co.CourseId equals cl.CourseId
                where cl.Season == season && cl.Year == year
                join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                where ac.Name == category
                join a in db.Assignments on ac.CategoryId equals a.CategoryId
                where a.Name == asgname
                join s in db.Submissions on a.AssignmentId equals s.AssignmentId
                join st in db.Students on s.StudentId equals st.UId
                select new
                {
                    fname = st.FName,
                    lname = st.LName,
                    uid = st.UId,
                    time = s.Time,
                    score = s.Score
                };

            return Json(query.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            var asg = (from co in db.Courses
                       where co.Subject == subject && co.Number == num
                       join cl in db.Classes on co.CourseId equals cl.CourseId
                       where cl.Season == season && cl.Year == year
                       join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                       where ac.Name == category
                       join a in db.Assignments on ac.CategoryId equals a.CategoryId
                       where a.Name == asgname
                       select new { a.AssignmentId, cl.ClassId }).SingleOrDefault();

            if (asg == null)
            {
                return Json(new { success = false });
            }

            var sub = db.Submissions
                .SingleOrDefault(s => s.AssignmentId == asg.AssignmentId && s.StudentId == uid);

            if (sub == null)
            {
                return Json(new { success = false });
            }

            sub.Score = (uint)score;
            db.SaveChanges();
            UpdateGrade(uid, asg.ClassId);
            return Json(new { success = true });
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {            
            var query = 
                from cl in db.Classes
                where cl.ProfessorId == uid
                join co in db.Courses on cl.CourseId equals co.CourseId
                select new
                {
                    subject = co.Subject,
                    number = co.Number,
                    name = co.Name,
                    season = cl.Season,
                    year = cl.Year
                };

            return Json(query.ToArray());
        }

        // Helpers
        
        /// <summary>
        /// Updates the grade for a student in a class
        /// </summary>
        /// <param name="uid">The student's uid</param>
        /// <param name="classId">The class id</param>
        private void UpdateGrade(string uid, uint classId)
        {
            var categories = db.AssignmentCategories
                .Where(ac => ac.ClassId == classId).ToList();

            double totalScaled = 0;
            double totalWeight = 0;

            foreach(var cat in categories)
            {
                var assingments = db.Assignments
                    .Where(a => a.CategoryId == cat.CategoryId).ToList();

                if(!assingments.Any())
                    continue; // skip empty categories

                double earned = 0;
                double maxPoints = 0;

                foreach(var a in assingments)
                {
                    maxPoints += a.MaxPoints;
                    var sub = db.Submissions
                        .SingleOrDefault(s => s.AssignmentId == a.AssignmentId && s.StudentId == uid);
                    earned += sub?.Score ?? 0;
                }

                double pct = maxPoints > 0 ? earned / maxPoints : 0;
                totalScaled += cat.Weight * pct;
                totalWeight += cat.Weight;
            }
           if(totalWeight == 0)
                return;

            double finalPct = (totalScaled / totalWeight) * 100;
            string letter = ToLetterGrade(finalPct);

            var enroll = db.Enrolleds
                .SingleOrDefault(e => e.StudentId == uid && e.ClassId == classId);

            if(enroll != null)  
            {
                enroll.Grade = letter;
                db.SaveChanges();
            }
    
        }

        /// <summary>
        /// Converts a percentage to a letter grade
        /// </summary>
        /// <param name="pct">The percentage to convert</param>
        /// <returns>The letter grade corresponding to the percentage</returns>
        private string ToLetterGrade(double pct)
        {
            if(pct >= 93) return "A";
            if(pct >= 90) return "A-";
            if(pct >= 87) return "B+";
            if(pct >= 83) return "B";
            if(pct >= 80) return "B-";
            if(pct >= 77) return "C+";
            if(pct >= 73) return "C";
            if(pct >= 70) return "C-";
            if(pct >= 67) return "D+";
            if(pct >= 63) return "D";
            if(pct >= 60) return "D-";
            return "E";
        }
        
        /*******End code to modify********/
    }
}

