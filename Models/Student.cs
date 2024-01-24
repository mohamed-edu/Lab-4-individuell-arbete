using System;
using System.Collections.Generic;

namespace Lab_4_individuell_arbete.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string? Class { get; set; }

    public int? FkPersonId { get; set; }

    public virtual Person? FkPerson { get; set; }
}
