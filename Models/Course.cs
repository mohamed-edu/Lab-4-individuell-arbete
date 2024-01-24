using System;
using System.Collections.Generic;

namespace Lab_4_individuell_arbete.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string? CourseName { get; set; }
    public bool IsActive { get; set; }
}
