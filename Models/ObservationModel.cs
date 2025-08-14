// File: Models/ObservationIndexViewModel.cs
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class ObservationIndexViewModel
    {
        public List<Observation> ControllerObservations { get; set; }
        public List<Observation> EmployeeObservations { get; set; }

        public ObservationIndexViewModel()
        {
            ControllerObservations = new List<Observation>();
            EmployeeObservations = new List<Observation>();
        }
    }
}