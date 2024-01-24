using System;
using System.Collections.Generic;

namespace Lab_4_individuell_arbete.Models;

public partial class Teacher
{
    public int TeacherId { get; set; }

    public string? SubjectField { get; set; }

    public int? FkPersonId { get; set; }

    public decimal? Salary { get; set; }

    public virtual Person? FkPerson { get; set; }
}
