using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotnetConfBot_AddingCUI
{
    public class Attendee
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AttendeeList
    {
        public string metadata { get; set; }
        public List<Attendee> value { get; set; }
    }
}