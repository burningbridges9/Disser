using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ComputationalServer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ComputationalServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        List<Models.Well> wells = new List<Models.Well>();
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            //Models.Well well1 = new Models.Well
            //{
            //    Q = 5.0 / (24.0 * 3600.0),
            //    P = 0,
            //    P0 = 0,
            //    Time1 = 0,
            //    Time2 = 5 * 3600,
            //    H0 = 0.1,
            //    K = 10 * Math.Pow(10, -15),
            //    Kappa = (1.0 / 3600.0) * 4,
            //    Ksi = 1,
            //    Mu = 5 * Math.Pow(10, -3),
            //    Rs = 0.3,
            //    Rw = 0.1,
            //    N = 100
            //};
            //Models.Well well2 = new Models.Well
            //{
            //    Q = 10.0 / (24.0 * 3600.0),
            //    P = 0,
            //    P0 = 0,
            //    Time1 = 5 * 3600,
            //    Time2 = 10 * 3600,
            //    H0 = 0.1,
            //    K = 10 * Math.Pow(10, -15),
            //    Kappa = (1.0 / 3600.0) * 4,
            //    Ksi = 1,
            //    Mu = 5 * Math.Pow(10, -3),
            //    Rs = 0.3,
            //    Rw = 0.1,
            //    N = 100
            //};
            //Models.Well well3 = new Models.Well
            //{
            //    Q = 15.0 / (24.0 * 3600.0),
            //    P = 0,
            //    P0 = 0,
            //    Time1 = 10 * 3600,
            //    Time2 = 15 * 3600,
            //    H0 = 0.1,
            //    K = 10 * Math.Pow(10, -15),
            //    Kappa = (1.0 / 3600.0) * 4,
            //    Ksi = 1,
            //    Mu = 5 * Math.Pow(10, -3),
            //    Rs = 0.3,
            //    Rw = 0.1,
            //    N = 100
            //};
            //List<Models.Well> wells = new List<Models.Well>();
            //wells.Add(well1); wells.Add(well2); wells.Add(well3);
            //List<double> times;
            //List<double> pressures;
            //List<double> consumptions;
            //List<int> indexes;
            //Actions.Functions.GetTimesAndPressures(wells, out times, out pressures, out indexes);
            //Actions.Functions.GetConsumtions(times, wells, wells.Count, pressures, indexes, out consumptions, wells[0].P0);
            return "value";
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post(WellsList wellsList)
        {
            //Models.Well ws = JsonConvert.DeserializeObject<Models.Well>(value);
            wells.AddRange(wellsList.Wells);
            List<double> times;
            List<double> pressures;
            List<double> consumptions;
            List<int> indexes;
            PressuresAndTimes pressuresAndTimes;
            Actions.Functions.GetTimesAndPressures(wells, out times, out pressures, out indexes, out pressuresAndTimes);
            //Actions.Functions.GetConsumtions(times, wells, wells.Count, pressures, indexes, out consumptions, wells[0].P0);

            return new JsonResult(pressuresAndTimes);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
