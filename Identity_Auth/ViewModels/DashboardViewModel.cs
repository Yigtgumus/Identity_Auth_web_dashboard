using Identity_Auth.Models;
using System.Collections.Generic;
using System.Data;

namespace Identity_Auth.ViewModels
{
    public class DashboardViewModel
    {
        public Dashboard Dashboard { get; set; }
        public List<GraphicDataViewModel> GraphicsData { get; set; }
    }

    public class GraphicDataViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Row { get; set; }
        public List<string> Labels { get; set; }
        public List<double> Values { get; set; }
        public List<List<object>> Data { get; set; }
        public string Query { get; set; }
        public string YAxis {  get; set; }
        public string Alias { get; set; }
        public string Priority { get; set; }

       

    }
}
