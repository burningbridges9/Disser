﻿using System;
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
        [Route("pressures")]
        public IActionResult GetPressures([FromBody] WellsList wellsList)
        {                       
            PressuresAndTimes pressuresAndTimes;
            Actions.Functions.GetTimesAndPressures(wellsList, out pressuresAndTimes);
            return new JsonResult(pressuresAndTimes);
        }

        [HttpPost]
        [Route("consumptions")]
        public IActionResult GetConsumptions(WellsList wellsList)
        {
            List<double> consumptions = new List<double>();
            ConsumptionsAndTimes consumptionsAndTimes = new ConsumptionsAndTimes();
            Actions.Functions.GetConsumtions(wellsList, out consumptions);
            List<double> staticConsumptions = new List<double>();
            Actions.Functions.PrepareStaticConsumptions(wellsList, staticConsumptions);
            consumptionsAndTimes.Times = Actions.Functions.GetTimes(wellsList.Wells, false);
            consumptionsAndTimes.Consumptions = consumptions;
            consumptionsAndTimes.StaticConsumptions = staticConsumptions;
            return new JsonResult(consumptionsAndTimes);
        }

        //[HttpPost]
        //[Route("nextgradient")]
        //public IActionResult GradientMethod([FromBody] Gradient gradient)
        //{
        //    List<Well> gradientWells = new List<Well>();
        //    foreach (var v in wells)
        //        gradientWells.Add(new Well
        //        {
                    
        //            Q = v.Q,
        //            P       = v.P ,
        //            P0      = v.P0 ,
        //            Time1   = v.Time1 ,
        //            Time2   = v.Time2 ,
        //            H0      = v.H0 ,
        //            K       = v.K ,
        //            Kappa   = v.Kappa ,
        //            Ksi     = v.Ksi ,
        //            Mu      = v.Mu ,
        //            Rs      = v.Rs ,
        //            Rw      = v.Rw ,
        //            N       = v.N ,
        //        });
        //    for (int i = 0; i < gradientWells.Count; i++)
        //    {
        //        gradientWells[i].K = gradient.ChangedK;
        //        gradientWells[i].Kappa = gradient.ChangedKappa;
        //        gradientWells[i].Ksi = gradient.ChangedKsi;
        //        gradientWells[i].P0 = gradient.ChangedP0;
        //    }
        //    gradients.Add(gradient);
        //    GradientAndConsumptions gradientAndConsumptions = new GradientAndConsumptions() { Gradient = gradient };
        //    Actions.Functions.GetNextGradientIteration(gradient, gradientWells, times, pressures, indexes, out gradientAndConsumptions);
        //    if (gradientAndConsumptions.ConsumptionsAndTimes != null)
        //    {
        //        List<double> staticConsumptions = new List<double>();
        //        Actions.Functions.PrepareStaticConsumptions(wells.Count, wells, indexes, staticConsumptions, times);
        //        gradientAndConsumptions.ConsumptionsAndTimes.StaticConsumptions = staticConsumptions;
        //    }
        //    return new JsonResult(gradientAndConsumptions);
        //}


        [HttpPost]
        [Route("nextgradient")]
        public IActionResult GradientMethod([FromBody] GradientAndWellsList gradientAndWellsList)
        {
            List<Well> gradientWells = new List<Well>();
            foreach (var v in gradientAndWellsList.WellsList.Wells)
                gradientWells.Add(new Well
                {

                    Q = v.Q,
                    P = v.P,
                    P0 = v.P0,
                    Time1 = v.Time1,
                    Time2 = v.Time2,
                    H0 = v.H0,
                    K = v.K,
                    Kappa = v.Kappa,
                    Ksi = v.Ksi,
                    Mu = v.Mu,
                    Rs = v.Rs,
                    Rw = v.Rw,
                    N = v.N,
                });
            for (int i = 0; i < gradientWells.Count; i++)
            {
                gradientWells[i].K = gradientAndWellsList.Gradient.ChangedK;
                gradientWells[i].Kappa = gradientAndWellsList.Gradient.ChangedKappa;
                gradientWells[i].Ksi = gradientAndWellsList.Gradient.ChangedKsi;
                gradientWells[i].P0 = gradientAndWellsList.Gradient.ChangedP0;
            }
            GradientAndConsumptions gradientAndConsumptions = new GradientAndConsumptions() { Gradient = gradientAndWellsList.Gradient };
            Actions.Functions.GetNextGradientIteration(gradientAndWellsList, gradientWells, out gradientAndConsumptions);
            if (gradientAndConsumptions.ConsumptionsAndTimes != null)
            {
                List<double> staticConsumptions = new List<double>();
                Actions.Functions.PrepareStaticConsumptions(gradientAndWellsList.WellsList, staticConsumptions);
                gradientAndConsumptions.ConsumptionsAndTimes.StaticConsumptions = staticConsumptions;
            }
            return new JsonResult(gradientAndConsumptions);
        }




        // DELETE api/values/5
        [HttpDelete]
        public IActionResult Delete()
        {
            return Ok();
        }
    }
}
