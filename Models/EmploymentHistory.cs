using System;
using System.Collections.Generic;

namespace Lab_4_individuell_arbete.Models;

public partial class EmploymentHistory
{
    public int EmploymentId { get; set; }

    public int FkPersonId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual Person FkPerson { get; set; } = null!;
}
