using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS.Controllers
{
    public class CommonController : Controller
    {
        private readonly LMSContext db;

        public CommonController(LMSContext _db)
        {
            db = _db;
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
        {
            var depts = db.Departments.Select(d => new { d.Name, d.Subject });
            return Json(depts.ToList());
        }



        /// <summary>
        /// Returns a JSON array representing the course catalog.
        /// Each object in the array should have the following fields:
        /// "subject": The subject abbreviation, (e.g. "CS")
        /// "dname": The department name, as in "Computer Science"
        /// "courses": An array of JSON objects representing the courses in the department.
        ///            Each field in this inner-array should have the following fields:
        ///            "number": The course number (e.g. 5530)
        ///            "cname": The course name (e.g. "Database Systems")
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetCatalog()
        {
            var catalog = db.Departments
                .Select(d => new
                {
                    subject = d.Subject,
                    dName = d.Name,
                    courses = db.Courses
                        .Where(c => c.Subject == d.Subject)
                        .Select(c => new
                        {
                            number = c.Number, 
                            cname = c.Name
                        }).ToList()
                });
            return Json(catalog.ToList());
        }

        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject">The subject abbreviation, as in "CS"</param>
        /// <param name="number">The course number, as in 5530</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {
            var courseId = db.Courses.FirstOrDefault(c => c.Subject == subject && c.Number == number);

            if (courseId == null)
            {
                return Json(new object[] { });
            }

            var query = db.Classes
                .Where(c => c.CourseId == courseId.CourseId)
                .Join(db.Professors,
                    c => c.ProfessorId,
                    p => p.UId,
                    (c,p) => new
                    {
                        season = c.Season,
                        year = c.Year,
                        location = c.Location,
                        start = c.Start,
                        end = c.End,
                        fname = p.FName,
                        lname = p.LName
                    });
            return Json(query.ToList());
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <returns>The assignment contents</returns>
        public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
        {

            var result = db.Assignments
                .Where(a => a.Name == asgname)
                .Join(
                    db.AssignmentCategories,
                    a => a.CategoryId,
                    ac => ac.CategoryId,
                    (a, ac) => new { a, ac })
                .Where(cat => cat.ac.Name == category)
                .Join(
                    db.Classes,
                    temp => temp.ac.ClassId,
                    cl => cl.ClassId,
                    (temp, cl) => new { temp.a, temp.ac, cl }
                    )
                .Where(tempc => tempc.cl.Season == season && tempc.cl.Year == year)
                .Join(
                    db.Courses,
                    temp => temp.cl.CourseId,
                    co => co.CourseId,
                    (tempCo, co) => new {tempCo.a, tempCo.ac, tempCo.cl, co}
                    )
                .Where(tempCo => tempCo.co.Subject == subject && tempCo.co.Number == num)
                .Select(cont => cont.a.Content )
                .FirstOrDefault();

            return Content(result ?? "");
        }


        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment submission.
        /// Returns the empty string ("") if there is no submission.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <param name="uid">The uid of the student who submitted it</param>
        /// <returns>The submission text</returns>
        public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
        {            
            
            var result = db.Assignments
                .Where(a => a.Name == asgname)
                .Join(
                    db.AssignmentCategories,
                    a => a.CategoryId,
                    ac => ac.CategoryId,
                    (a, ac) => new
                        { a, ac })
                .Where(cat => cat.ac.Name == category)
                .Join(
                    db.Classes,
                    temp => temp.ac.ClassId,
                    cl => cl.ClassId,
                    (temp, cl) => new { temp.a, temp.ac, cl }
                )
                .Where(tempc => tempc.cl.Season == season && tempc.cl.Year == year)
                .Join(
                    db.Courses,
                    temp => temp.cl.CourseId,
                    co => co.CourseId,
                    (tempCo, co) => new {tempCo.a, tempCo.ac, tempCo.cl, co}
                )
                .Where(tempCo => tempCo.co.Subject == subject && tempCo.co.Number == num)
                .Join(
                    db.Submissions,
                    temp => temp.a.AssignmentId,
                    s => s.AssignmentId,
                    (temp, s) => new {temp.a, temp.ac, temp.cl, temp.co, s}
                    )
                .Where(tempS => tempS.s.StudentId == uid)
                .Select(cont => cont.s.Contents )
                .FirstOrDefault();
            
            return Content(result ?? "");
        }


        /// <summary>
        /// Gets information about a user as a single JSON object.
        /// The object should have the following fields:
        /// "fname": the user's first name
        /// "lname": the user's last name
        /// "uid": the user's uid
        /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
        ///               If the user is a Professor, this is the department they work in.
        ///               If the user is a Student, this is the department they major in.    
        ///               If the user is an Administrator, this field is not present in the returned JSON
        /// </summary>
        /// <param name="uid">The ID of the user</param>
        /// <returns>
        /// The user JSON object 
        /// or an object containing {success: false} if the user doesn't exist
        /// </returns>
        public IActionResult GetUser(string uid)
        {

            var admin = 
                db.Administrators
                .Where(a => a.UId == uid)
                .Select(a => new
                {
                    fname = a.FName, 
                    lname = a.LName, 
                    uid = a.UId
                })
                .FirstOrDefault();

            if (admin != null)
            {
                return Json(admin);
            }
            
            var prof = 
                db.Professors
                    .Where(p => p.UId == uid)
                    .Join(
                        db.Departments,
                        p => p.Subject,
                        d => d.Subject,
                        (p, d) => new
                        {
                            fname = p.FName,
                            lname = p.LName,
                            uid = p.UId,
                            department = d.Name
                        }
                        )
                    .FirstOrDefault();
            
            if (prof != null)
            {
                return Json(prof);
            }
            
            var stud = 
                db.Students
                    .Where(s => s.UId == uid)
                    .Join(
                        db.Departments,
                        s => s.Major,
                        d => d.Subject,
                        (s, d) => new
                        {
                            fname = s.FName,
                            lname = s.LName,
                            uid = s.UId,
                            department = d.Name
                        }
                    )
                    .FirstOrDefault();
            
            if (stud != null)
            {
                return Json(stud);
            }
            
            return Json(new { success = false });
        }


        /*******End code to modify********/
    }
}

